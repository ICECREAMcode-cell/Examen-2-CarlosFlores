using Examen2_CarlosFlores.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Examen2_CarlosFlores.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ImagenController : ControllerBase
    {
        private readonly IImagenInterface _imagenService;

        public ImagenController(IImagenInterface imagenService)
        {
            _imagenService = imagenService;
        }

        // Subir imagen: el archivo se guarda en wwwroot/imagenes y en la BD
        // solo se almacena la ruta (/imagenes/nombre.jpg).
        [HttpPost(Name = "Subir Imagen")]
        public IActionResult Post([FromForm] Guid idTesis, [FromForm] IFormFile imagen)
        {
            try
            {
                return Ok(_imagenService.Subir(idTesis, imagen));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
