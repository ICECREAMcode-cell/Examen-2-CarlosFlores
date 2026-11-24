using Contrato.DTO;
using Examen2_CarlosFlores.Data;
using Examen2_CarlosFlores.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Examen2_CarlosFlores.Services
{
    public class ImagenService : IImagenInterface
    {
        private static readonly string[] ExtensionesPermitidas =
            { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private const long TamanoMaximo = 5 * 1024 * 1024; // 5 MB

        private readonly BaseDatos _context;
        private readonly IWebHostEnvironment _env;

        public ImagenService(BaseDatos context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public ImagenDTO Subir(Guid idTesis, IFormFile imagen)
        {
            var tesis = _context.tesis.Find(idTesis)
                ?? throw new InvalidOperationException("La tesis seleccionada no existe.");

            if (imagen == null || imagen.Length == 0)
            {
                throw new InvalidOperationException("No se seleccionó ningún archivo de imagen.");
            }

            if (imagen.Length > TamanoMaximo)
            {
                throw new InvalidOperationException("La imagen no debe superar los 5 MB.");
            }

            var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();
            if (!ExtensionesPermitidas.Contains(extension))
            {
                throw new InvalidOperationException(
                    "Formato de imagen no permitido. Use jpg, jpeg, png, gif o webp.");
            }

            var webRoot = _env.WebRootPath
                ?? Path.Combine(_env.ContentRootPath, "wwwroot");

            var carpeta = Path.Combine(webRoot, "imagenes");
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid():N}{extension}";
            var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                imagen.CopyTo(stream);
            }

            // En la base de datos solo se guarda la ubicación de la imagen.
            var ruta = $"/imagenes/{nombreArchivo}";
            tesis.Image = ruta;
            _context.SaveChanges();

            return new ImagenDTO
            {
                Id_Tesis = idTesis,
                Ruta = ruta
            };
        }
    }
}
