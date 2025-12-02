using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservasApp.Models;

namespace reservasApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Solo el Admin puede entrar aquí
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/admin/solicitudes
        // Muestra los restaurantes que aún no han sido aprobados
        [HttpGet("solicitudes")]
        public async Task<ActionResult<List<RestaurantModel>>> GetSolicitudes()
        {
            return await _context.Restaurantes
                                 .Where(r => !r.EstaAprobado)
                                 .Include(r => r.Dueño) // Incluimos datos del dueño por si los necesitas
                                 .ToListAsync();
        }

        // PUT: api/admin/aprobar/5
        // Aprueba un restaurante
        [HttpPut("aprobar/{id}")]
        public async Task<IActionResult> AprobarRestaurante(int id)
        {
            var restaurante = await _context.Restaurantes.FindAsync(id);

            if (restaurante == null)
            {
                return NotFound("Restaurante no encontrado.");
            }

            restaurante.EstaAprobado = true;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"El restaurante {restaurante.Nombre} ha sido aprobado." });
        }

        // GET: api/admin/metricas
        // Un ejemplo de dashboard simple
        [HttpGet("metricas")]
        public async Task<ActionResult> GetMetricas()
        {
            var totalUsuarios = await _context.Usuarios.CountAsync();
            var totalReservas = await _context.Reservaciones.CountAsync();
            var totalRestaurantes = await _context.Restaurantes.CountAsync();

            return Ok(new 
            { 
                TotalUsuarios = totalUsuarios, 
                TotalReservas = totalReservas, 
                TotalRestaurantes = totalRestaurantes 
            });
        }
    }
}