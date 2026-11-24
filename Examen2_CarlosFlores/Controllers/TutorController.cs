using Contrato.DTO;
using Examen2_CarlosFlores.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Examen2_CarlosFlores.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TutorController : ControllerBase
    {
        private readonly ITutorInterface _tutorService;

        public TutorController(ITutorInterface tutorService)
        {
            _tutorService = tutorService;
        }

        [HttpGet(Name = "Listar Tutor")]
        public IActionResult Get()
        {
            return Ok(_tutorService.ObtenerTodos());
        }

        // Mostrar un tutor
        [HttpGet("{id}", Name = "Mostrar Tutor")]
        public IActionResult Get(Guid id)
        {
            var tutor = _tutorService.ObtenerPorId(id);
            if (tutor == null)
            {
                return NotFound();
            }
            return Ok(tutor);
        }

        [HttpPost(Name = "Guardar Tutor")]
        public IActionResult Post(TutorDTO tutorDto)
        {
            try
            {
                _tutorService.Crear(tutorDto);
                return Ok("Tutor registrado correctamente");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
