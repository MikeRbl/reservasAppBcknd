using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using reservasApp.DTOs;
using reservasApp.Models;
using reservasApp.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace reservasApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Cualquier usuario logueado puede usar esto
    public class UserController : ControllerBase
    {
        private readonly IReservasService _reservasService;
        private readonly AppDbContext _context;

        public UserController(IReservasService reservasService, AppDbContext context)
        {
            _reservasService = reservasService;
            _context = context;
        }

        [HttpPost("reservar")]
        public async Task<IActionResult> Reservar([FromBody] ReservacionCreacionDTO dto)
        {
            // Obtener ID del usuario desde el Token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0; // 0 o manejar error si no hay token

            if (userId == 0) return Unauthorized("Debes iniciar sesión.");

            var resultado = await _reservasService.CrearReservacion(dto, userId);

            if (resultado)
                return Ok("Reservación creada con éxito.");
            
            return BadRequest("No se pudo crear la reservación.");
        }

        [HttpPut("cancelar/{id}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;

            var resultado = await _reservasService.CancelarReservacion(id, userId);

            if (resultado)
                return Ok("Reservación cancelada.");

            return BadRequest("No se encontró la reservación o no es tuya.");
        }
        
        [HttpGet("mis-reservas")]
        public async Task<ActionResult<List<ReservasModel>>> GetMisReservas()
        {
            // Obtener ID del usuario (Simulado por ahora, luego usarás Claims)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized("Usuario no identificado.");
            int userId = int.Parse(userIdClaim.Value);

            return await _context.Reservaciones.Where(r => r.ClienteId == userId).Include(r => r.Restaurant).Include(r => r.Mesa).OrderByDescending(r => r.FechaHora).ToListAsync();
}
    }
}