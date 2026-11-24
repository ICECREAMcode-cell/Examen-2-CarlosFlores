using System.Security.Claims;
using Contrato.DTO;
using Examen2_CarlosFlores.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen2_CarlosFlores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthInterface _authService;

        public AuthController(IAuthInterface authService)
        {
            _authService = authService;
        }

        [HttpPost("Registrar")]
        [AllowAnonymous]
        public IActionResult Registrar(LoginDTO dto)
        {
            try
            {
                _authService.Registrar(dto);
                return Ok("Usuario registrado correctamente.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO dto)
        {
            var respuesta = _authService.Login(dto);

            if (respuesta is null)
            {
                return Unauthorized("Usuario o contraseña incorrectos.");
            }

            return Ok(respuesta);
        }

        [HttpGet("Perfil")]
        [Authorize]
        public IActionResult Perfil()
        {
            return Ok(new
            {
                usuario = User.Identity?.Name,
                rol = User.FindFirstValue(ClaimTypes.Role)
            });
        }
    }
}
