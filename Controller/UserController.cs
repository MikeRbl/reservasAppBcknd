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
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/user/reservar
        [HttpPost("reservar")]
        [Authorize(Roles = "Cliente")] // Solo clientes
        public async Task<IActionResult> CrearReserva([FromBody] ReservacionCreacionDTO dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            // 1. Validar que el restaurante existe
            var existeRestaurante = await _context.Restaurantes.AnyAsync(r => r.Id == dto.RestauranteId);
            if (!existeRestaurante) return BadRequest("Restaurante no válido");

            // 2. Crear la reserva
            var nuevaReserva = new ReservasModel
            {
                ClienteId = userId,
                RestauranteId = dto.RestauranteId,
                FechaHora = dto.FechaHora,
                Estado = "Pendiente", // Siempre nace pendiente
                // La mesa se puede asignar automáticamente aquí o manualmente por el admin después
                MesaId = null 
            };

            _context.Reservaciones.Add(nuevaReserva);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Reserva solicitada. Espera confirmación del restaurante." });
        }

        // GET: api/user/mis-reservas
        [HttpGet("mis-reservas")]
        [Authorize(Roles = "Cliente")]
        public async Task<ActionResult> GetMisReservas()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var reservas = await _context.Reservaciones
                                         .Where(r => r.ClienteId == userId)
                                         .Include(r => r.Restaurant)
                                         .OrderByDescending(r => r.FechaHora)
                                         .ToListAsync();
            return Ok(reservas);
        }
    }
}