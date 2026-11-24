using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Contrato.DTO;
using Examen2_CarlosFlores.Data;
using Examen2_CarlosFlores.Interfaces;
using Examen2_CarlosFlores.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Examen2_CarlosFlores.Services
{
    public class AuthService : IAuthInterface
    {
        private readonly BaseDatos _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<Usuario> _passwordHasher = new();

        public AuthService(BaseDatos context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void Registrar(LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                throw new InvalidOperationException("Usuario y contraseña son requeridos.");
            }

            var nombreUsuario = dto.NombreUsuario.Trim();

            if (_context.usuarios.Any(u => u.NombreUsuario == nombreUsuario))
            {
                throw new InvalidOperationException("El nombre de usuario ya está registrado.");
            }

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = nombreUsuario,
                Rol = "Usuario"
            };

            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, dto.Password);

            _context.usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public LoginRespuestaDTO? Login(LoginDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreUsuario) ||
                string.IsNullOrWhiteSpace(dto.Password))
            {
                return null;
            }

            var usuario = _context.usuarios
                .FirstOrDefault(u => u.NombreUsuario == dto.NombreUsuario.Trim());

            if (usuario is null)
            {
                return null;
            }

            var verificacion = _passwordHasher.VerifyHashedPassword(
                usuario, usuario.PasswordHash, dto.Password);

            if (verificacion == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (verificacion == PasswordVerificationResult.SuccessRehashNeeded)
            {
                usuario.PasswordHash = _passwordHasher.HashPassword(usuario, dto.Password);
                _context.SaveChanges();
            }

            var expira = DateTime.UtcNow.AddMinutes(ObtenerMinutosExpiracion());

            return new LoginRespuestaDTO
            {
                Token = GenerarToken(usuario, expira),
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol,
                Expira = expira
            };
        }

        private int ObtenerMinutosExpiracion()
        {
            return int.TryParse(_configuration["Jwt:ExpiraMinutos"], out var minutos)
                ? minutos
                : 60;
        }

        private string GenerarToken(Usuario usuario, DateTime expira)
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("Falta configurar Jwt:Key.");

            var issuer = _configuration["Jwt:Issuer"] ?? "Examen2CarlosFlores";
            var audience = _configuration["Jwt:Audience"] ?? "Examen2-Visual";

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, usuario.NombreUsuario),
                new(ClaimTypes.Name, usuario.NombreUsuario),
                new(ClaimTypes.Role, usuario.Rol),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var credenciales = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expira,
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
