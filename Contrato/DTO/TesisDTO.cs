using System.ComponentModel.DataAnnotations;

namespace Contrato.DTO
{
    public class TesisDTO
    {
        // Se usa para lectura (la API devuelve el Id). Al crear se ignora.
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estudiante es obligatorio")]
        public string Estudiante { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estado es obligatorio")]
        public string Estado { get; set; } = string.Empty;

        public string? Image { get; set; }

        [Required(ErrorMessage = "El tutor es obligatorio")]
        public Guid Id_Tutor { get; set; }
    }
}
