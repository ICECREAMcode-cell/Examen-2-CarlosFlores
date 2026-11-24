using Examen2_CarlosFlores.Data;
using Examen2_CarlosFlores.Models;
using Microsoft.AspNetCore.Identity;

namespace Examen2_CarlosFlores.Factories
{
    public static class UsuarioFactories
    {
        public static void Seed(BaseDatos context)
        {
            if (context.usuarios.Any())
            {
                return;
            }

            var hasher = new PasswordHasher<Usuario>();
            var admin = new Usuario
            {
                Id = Guid.NewGuid(),
                NombreUsuario = "admin",
                Rol = "Administrador"
            };

            admin.PasswordHash = hasher.HashPassword(admin, "Admin123*");

            context.usuarios.Add(admin);
            context.SaveChanges();
        }
    }
}
