using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using reservasApp.Models;
using reservasApp.DTOs;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity.Data;

namespace reservasApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // LOGIN (Ya lo tenías, asegúrate que funcione)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);
            if (usuario == null) return Unauthorized("Credenciales inválidas");

            var token = GenerarToken(usuario);
            return Ok(new { token, rol = usuario.Rol, usuarioId = usuario.Id });
        }

        // REGISTRO USUARIO NORMAL
        [HttpPost("registro-usuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] RegistroUsuarioDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El correo ya está registrado.");

            var usuario = new UserModel
            {
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno, // Puede ser null
                Email = dto.Email,
                Password = dto.Password, // En producción usar Hash!
                Telefono = dto.Telefono,
                Rol = "Cliente" // Rol automático
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado exitosamente" });
        }

        // REGISTRO RESTAURANTE
        [HttpPost("registro-restaurante")]
        public async Task<IActionResult> RegistrarRestaurante([FromBody] RegistroRestauranteDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El correo ya está registrado.");

            // 1. Crear Usuario Dueño
            var dueño = new UserModel
            {
                Nombre = dto.NombreDueño,
                Email = dto.Email,
                Password = dto.Password,
                Rol = "Restaurante" // Rol específico
            };

            _context.Usuarios.Add(dueño);
            await _context.SaveChangesAsync(); // Guardamos para obtener el ID

            // 2. Crear Restaurante (Pendiente de aprobación)
            var restaurante = new RestaurantModel
            {
                Nombre = dto.NombreRestaurante,
                Direccion = dto.Direccion,
                UsuarioId = dueño.Id,
                EstaAprobado = false // ¡Importante! El Admin debe aprobarlo
            };

            _context.Restaurantes.Add(restaurante);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Solicitud de restaurante enviada. Espera aprobación del administrador." });
        }

        private string GenerarToken(UserModel usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}