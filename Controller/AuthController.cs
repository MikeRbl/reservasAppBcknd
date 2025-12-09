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
        // ==========================================
        // 1. LOGIN (MODIFICADO CON SEGURIDAD)
        // ==========================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest login)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == login.Email && u.Password == login.Password);
            if (usuario == null) return Unauthorized("Credenciales inválidas");

            // --- BLOQUEO DE SEGURIDAD PARA RESTAURANTES ---
            if (usuario.Rol == "Restaurante")
            {
                // Buscamos la ficha del restaurante asociada a este usuario
                var restaurante = await _context.Restaurantes.FirstOrDefaultAsync(r => r.UsuarioId == usuario.Id);
                
                // Si existe y NO está aprobado, bloqueamos el acceso
                if (restaurante != null && !restaurante.EstaAprobado)
                {
                    return Unauthorized("Tu cuenta está en revisión. Un administrador debe aprobar tu restaurante antes de poder ingresar.");
                }
            }
            // ----------------------------------------------

            var token = GenerarToken(usuario);
            return Ok(new { token, rol = usuario.Rol, usuarioId = usuario.Id });
        }

        // ==========================================
        // 2. REGISTRO USUARIO (CLIENTE)
        // ==========================================
        [HttpPost("registro-usuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] RegistroUsuarioDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El correo ya está registrado.");

            var usuario = new UserModel
            {
                Nombre = dto.Nombre,
                ApellidoPaterno = dto.ApellidoPaterno,
                ApellidoMaterno = dto.ApellidoMaterno,
                Email = dto.Email,
                Password = dto.Password, // ¡Recuerda hashear en el futuro!
                Telefono = dto.Telefono,
                Rol = "Cliente"
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario registrado exitosamente" });
        }

        // ==========================================
        // 3. REGISTRO RESTAURANTE (SIMPLIFICADO)
        // ==========================================
        [HttpPost("registro-restaurante")]
        public async Task<IActionResult> RegistrarRestaurante([FromBody] RegistroRestauranteDTO dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("El correo ya está registrado.");

            // Creamos el Usuario (Representa al negocio)
            var usuarioNegocio = new UserModel
            {
                Nombre = dto.NombreRestaurante, // El nombre del usuario es el del restaurante
                Email = dto.Email,
                Password = dto.Password,
                Telefono = dto.Telefono,
                Rol = "Restaurante"
            };

            _context.Usuarios.Add(usuarioNegocio);
            await _context.SaveChangesAsync();

            // Creamos la ficha del Restaurante (Pendiente de aprobación)
            var restaurante = new RestaurantModel
            {
                Nombre = dto.NombreRestaurante,
                Direccion = "", // Se llenará en el perfil después
                UsuarioId = usuarioNegocio.Id,
                EstaAprobado = false // <--- Esto dispara la solicitud al admin
            };

            _context.Restaurantes.Add(restaurante);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Solicitud enviada. Tu cuenta se activará cuando un administrador la apruebe." });
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