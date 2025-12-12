using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservasApp.DTOs;
using reservasApp.Models;
using System.Security.Claims;

namespace reservasApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. GESTIÓN DEL PERFIL (Requisito Básico)
        // ==========================================
        [HttpGet("publicos")]
        [AllowAnonymous] 
        public async Task<ActionResult<List<RestaurantModel>>> GetRestaurantesPublicos()
        {
            return await _context.Restaurantes
                                 .Where(r => r.EstaAprobado)
                                 .Include(r => r.Dueno)
                                 .ToListAsync();
        }

        [HttpGet("mi-perfil")]
        [Authorize(Roles = "Restaurante")]
        public async Task<ActionResult<RestaurantModel>> GetMiPerfil()
        {
            var restaurante = await GetRestauranteDelUsuarioActual();
            if (restaurante == null) return NotFound("No tienes un restaurante asociado.");
            return restaurante;
        }

        [HttpPut("actualizar-perfil")]
        [Authorize(Roles = "Restaurante")]
        public async Task<IActionResult> ActualizarPerfil(RestaurantModel datos)
        {
            var restaurante = await GetRestauranteDelUsuarioActual();
            if (restaurante == null) return NotFound();

            restaurante.Direccion = datos.Direccion;
            restaurante.Nombre = datos.Nombre;
            // Aquí puedes agregar más campos si tu modelo los tiene
            
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Perfil actualizado correctamente" });
        }

        // ==========================================
        // 2. GESTIÓN DE MESAS
        // ==========================================
        [HttpGet("mesas")]
        [Authorize(Roles = "Restaurante")]
        public async Task<ActionResult<List<MesaModel>>> GetMisMesas()
        {
            var restaurante = await GetRestauranteDelUsuarioActual();
            return await _context.Mesas.Where(m => m.RestauranteId == restaurante.Id).ToListAsync();
        }

        [HttpPost("mesas")]
        [Authorize(Roles = "Restaurante")]
        public async Task<IActionResult> CrearMesa(MesaModel mesa)
        {
            var restaurante = await GetRestauranteDelUsuarioActual();
            mesa.RestauranteId = restaurante.Id;
            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Mesa creada", mesa });
        }

        [HttpDelete("mesas/{id}")]
        [Authorize(Roles = "Restaurante")]
        public async Task<IActionResult> EliminarMesa(int id)
        {
            var restaurante = await GetRestauranteDelUsuarioActual();
            var mesa = await _context.Mesas.FirstOrDefaultAsync(m => m.Id == id && m.RestauranteId == restaurante.Id);
            if (mesa == null) return NotFound("Mesa no encontrada");

            _context.Mesas.Remove(mesa);
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Mesa eliminada" });
        }

        // ==========================================
        // 3. GESTIÓN DE RESERVAS & ALGORITMO
        // ==========================================
        [HttpGet("mis-reservas")]
        [Authorize(Roles = "Restaurante")]
        public async Task<ActionResult> GetReservasDelRestaurante()
        {
            var restaurante = await GetRestauranteDelUsuarioActual();
            var reservas = await _context.Reservaciones
                                         .Where(r => r.RestauranteId == restaurante.Id)
                                         .Include(r => r.Cliente)
                                         .Include(r => r.Mesa)
                                         .OrderByDescending(r => r.FechaHora)
                                         .ToListAsync();
            return Ok(reservas);
        }

        // --- ENDPOINT ALGORÍTMICO: ACEPTAR Y ASIGNAR ---
        [HttpPut("gestionar-reserva")]
        [Authorize(Roles = "Restaurante")]
        public async Task<IActionResult> GestionarReserva([FromBody] AceptarReservaDTO dto)
        {
            var reserva = await _context.Reservaciones.FindAsync(dto.ReservaId);
            if (reserva == null) return NotFound("Reserva no encontrada");

            if (!dto.Aprobada)
            {
                reserva.Estado = "Rechazada";
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Reserva rechazada" });
            }

            // ALGORITMO: Asignación de Mesa
            // Si el admin no eligió mesa (null), buscamos la mejor automáticamente.
            if (dto.MesaId == null)
            {
                // 1. Buscar mesas del restaurante
                var mesasDisponibles = await _context.Mesas
                    .Where(m => m.RestauranteId == reserva.RestauranteId)
                    .ToListAsync();

                // 2. Filtrar mesas que ya están ocupadas a esa hora (simple check)
                // (Para una app real, revisaríamos rangos de horas, aquí simplificamos por fecha exacta)
                var mesasOcupadas = await _context.Reservaciones
                    .Where(r => r.RestauranteId == reserva.RestauranteId 
                             && r.FechaHora == reserva.FechaHora 
                             && r.Estado == "Confirmada"
                             && r.MesaId != null)
                    .Select(r => r.MesaId)
                    .ToListAsync();

                // 3. Lógica Algorítmica: "Best Fit" (Mejor Ajuste)
                // Buscamos una mesa libre donde quepan, ordenadas por capacidad ascendente
                // (Para no dar una mesa de 10 a una pareja).
                // Nota: Asumimos que ReservasModel tiene un campo 'NumeroPersonas', si no, asigna cualquiera libre.
                var mesaIdeal = mesasDisponibles
                    .Where(m => !mesasOcupadas.Contains(m.Id)) // Que esté libre
                    // .Where(m => m.Capacidad >= reserva.NumeroPersonas) // Si tuvieras este campo
                    .OrderBy(m => m.Capacidad) // La más pequeña que sirva
                    .FirstOrDefault();

                if (mesaIdeal != null)
                {
                    reserva.MesaId = mesaIdeal.Id;
                }
                else
                {
                    return BadRequest("No hay mesas disponibles automáticamente para esa hora. Libera alguna o rechaza.");
                }
            }
            else
            {
                // Asignación manual
                reserva.MesaId = dto.MesaId.Value;
            }

            reserva.Estado = "Confirmada";
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Reserva Confirmada en mesa {reserva.MesaId}" });
        }

        // Helper privado
        private async Task<RestaurantModel?> GetRestauranteDelUsuarioActual()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return null;
            int userId = int.Parse(userIdClaim);
            return await _context.Restaurantes.FirstOrDefaultAsync(r => r.UsuarioId == userId);
        }
    }
}