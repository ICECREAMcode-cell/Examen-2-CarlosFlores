using System.ComponentModel.DataAnnotations;

namespace Contrato.DTO
{
    public class TutorDTO
    {
        // Se usa para lectura (la API devuelve el Id). Al crear se ignora.
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        public string Especialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio")]
        public string Correo { get; set; } = string.Empty;
    }
}
