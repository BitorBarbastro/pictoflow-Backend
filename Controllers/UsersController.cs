using Microsoft.AspNetCore.Mvc;
using pictoflow_Backend.Services;
using pictoflow_Backend.DTOs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using pictoflow_Backend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace pictoflow_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly UserManager _userManager;
        private readonly pictoflow_Backend.Models.ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public UsersController(IMemoryCache cache, UserManager userManager, pictoflow_Backend.Models.ApplicationDbContext context, IConfiguration config)
        {
            _cache = cache;
            _userManager = userManager;
            _context = context;
            _config = config;
        }

        private string GenerateJwtToken(User user)
        {
            var secretKey = _config["Jwt:Key"];
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Email)
                    // Puedes agregar más claims aquí si es necesario
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpPost("register1")]
        public IActionResult Register1([FromBody] UserDto userDto)
        {
            if (userDto == null || string.IsNullOrEmpty(userDto.Email))
            {
                return BadRequest("Los datos del usuario son inválidos.");
            }

            // Generar un token único
            string token = Guid.NewGuid().ToString();

            // Guardar los datos del usuario y el token en la caché
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(token, userDto, cacheEntryOptions);

            // Devolver el token en la respuesta
            return Ok(new { Token = token });
        }

        [HttpGet("register2")]
        public IActionResult Register2(string token)
        {
            // Obtener los datos del usuario de la caché utilizando el token
            if (_cache.TryGetValue(token, out UserDto userDto))
            {
                // Devolver los datos del usuario en la respuesta
                return Ok(new { Email = userDto.Email });
            }

            // Si no se encuentra el token en la caché, devolver un error
            return NotFound("Token inválido o expirado.");
        }

        [HttpPost("register2")]
        public IActionResult Register2([FromBody] PasswordDto passwordDto)
        {
            try
            {
                if (_cache.TryGetValue(passwordDto.Token, out UserDto userDto))
                {
                    if (passwordDto.Password != passwordDto.ConfirmPassword)
                    {
                        return BadRequest("Las contraseñas no coinciden.");
                    }

                    var user = new pictoflow_Backend.Models.User
                    {
                        Email = userDto.Email,
                        IsPhotographer = userDto.IsPhotographer
                    };

                    _userManager.CreateUser(user, passwordDto.Password);

                    _cache.Remove(passwordDto.Token);

                    // Generar y devolver el token JWT
                    var token = GenerateJwtToken(user);
                    return Ok(new { Token = token, message = "Registro exitoso." });
                }

                return NotFound("Token inválido o expirado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar el usuario: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == loginDto.Email);

                if (user == null)
                {
                    return BadRequest("Credenciales inválidas.");
                }

                bool isPasswordValid = _userManager.CheckPassword(user, loginDto.Password);

                if (!isPasswordValid)
                {
                    return BadRequest("Credenciales inválidas.");
                }

                // Generar y devolver el token JWT
                var token = GenerateJwtToken(user);
                return Ok(new { Token = token, message = "Inicio de sesión exitoso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al iniciar sesión: {ex.Message}");
            }
        }
    }
}
