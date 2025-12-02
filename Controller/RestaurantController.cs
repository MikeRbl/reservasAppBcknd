using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservasApp.Models;
using reservasApp.DTOs;
using System.Security.Claims;

namespace reservasApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Restaurante,Admin")] // Dueños y Admins
    public class RestaurantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RestaurantController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/restaurant/mis-reservas
        [HttpGet("mis-reservas")]
        public async Task<ActionResult<List<ReservasModel>>> GetReservas([FromQuery] DateTime? fecha)
        {
            // Obtenemos el ID del usuario logueado desde el Token
            // Nota: Esto funcionará cuando tengas el AuthController listo y uses Tokens
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Usuario no identificado.");
            
            int userId = int.Parse(userIdClaim.Value);

            // Buscamos el restaurante de este usuario
            var restaurante = await _context.Restaurantes.FirstOrDefaultAsync(r => r.UsuarioId == userId);
            if (restaurante == null) return NotFound("No tienes un restaurante registrado.");

            var query = _context.Reservaciones
                                .Where(r => r.RestauranteId == restaurante.Id);

            // Filtro opcional por fecha (Día, Mes, etc.)
            if (fecha.HasValue)
            {
                query = query.Where(r => r.FechaHora.Date == fecha.Value.Date);
            }

            return await query.Include(r => r.Cliente).ToListAsync();
        }

        // POST: api/restaurant/mesas
        [HttpPost("mesas")]
        public async Task<ActionResult> CrearMesa([FromBody] MesaModel mesa)
        {
            // Aquí deberías asignar el RestauranteId automáticamente basado en el usuario logueado
            // Por simplicidad, asumimos que viene en el body o lo validamos
            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();
            return Ok("Mesa creada exitosamente.");
        }

        // PUT: api/restaurant/aceptar-reserva
        [HttpPut("aceptar-reserva")]
        public async Task<IActionResult> AceptarReserva([FromBody] AceptarReservaDTO dto)
        {
            var reserva = await _context.Reservaciones.FindAsync(dto.ReservaId);
            if (reserva == null) return NotFound("Reserva no encontrada.");

            var mesa = await _context.Mesas.FindAsync(dto.MesaId);
            if (mesa == null) return NotFound("Mesa no encontrada.");

            // Validar capacidad, disponibilidad, etc. aquí

            reserva.Estado = "Aceptada";
            reserva.MesaId = dto.MesaId;

            await _context.SaveChangesAsync();
            return Ok("Reserva aceptada y mesa asignada.");
        }

        [HttpGet("publicos")]
        [AllowAnonymous] // Permitir que cualquiera vea la lista
        public async Task<ActionResult<List<RestaurantModel>>> GetRestaurantesDisponibles()
        {
            // Solo devolvemos los que el Admin aprobó
            return await _context.Restaurantes.Where(r => r.EstaAprobado).ToListAsync();
        }
    
    }
}