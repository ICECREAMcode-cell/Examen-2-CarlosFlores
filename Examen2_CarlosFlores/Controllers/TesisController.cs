using Contrato.DTO;
using Examen2_CarlosFlores.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen2_CarlosFlores.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TesisController : ControllerBase
    {
        private readonly ITesisInterface _tesisService;

        public TesisController(ITesisInterface tesisService)
        {
            _tesisService = tesisService;
        }

        [HttpGet(Name = "Listar Tesis")]
        public IActionResult Get()
        {
            return Ok(_tesisService.ObtenerTodos());
        }

        // Buscar tesis por título, estudiante, estado o nombre del tutor.
        [HttpGet("buscar", Name = "Buscar Tesis")]
        public IActionResult Buscar([FromQuery] string? termino)
        {
            return Ok(_tesisService.Buscar(termino ?? string.Empty));
        }

        [HttpGet("{id}", Name = "Mostrar Tesis")]
        public IActionResult Get(Guid id)
        {
            var tesis = _tesisService.ObtenerPorId(id);
            if (tesis == null)
            {
                return NotFound();
            }
            return Ok(tesis);
        }

        [HttpPost(Name = "Guardar Tesis")]
        public IActionResult Post(TesisDTO tesisDto)
        {
            try
            {
                _tesisService.Crear(tesisDto);
                return Ok("Tesis registrada correctamente");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
