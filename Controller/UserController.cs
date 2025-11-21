using Microsoft.AspNetCore.Mvc;
using reservasApp.DTOs;
using reservasApp.Services;
using System.Security.Claims;

namespace reservasApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IReservasService _reservasService;

        public UserController(IReservasService reservasService)
        {
            _reservasService = reservasService;
        }

        [HttpPost("reservar")]
        public async Task<IActionResult> Reservar([FromBody] ReservacionCreacionDTO dto)
        {
            // Nota: Aquí simulamos el ID del usuario (por ejemplo, 1) porque aún no tienes el login activo.
            // Cuando tengas Auth, usarás: int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            int userId = 1; 

            var resultado = await _reservasService.CrearReservacion(dto, userId);

            if (resultado)
            {
                return Ok("Reservación creada con éxito");
            }
            return BadRequest("No se pudo crear la reservación");
        }

        [HttpPut("cancelar/{id}")]
        public async Task<IActionResult> Cancelar(int id)
        {
            int userId = 1; // Simulado igual que arriba
            
            var resultado = await _reservasService.CancelarReservacion(id, userId);

            if (resultado)
            {
                return Ok("Reservación cancelada");
            }
            return NotFound("No se encontró la reservación o no tienes permiso");
        }
    }
}