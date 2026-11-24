using System.ComponentModel.DataAnnotations;

namespace Examen2_CarlosFlores.Models
{
    public class Tutor
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string? Nombre { get; set; }
        [Required(ErrorMessage = "La especialidad es obligatoria")]
        public string? Especialidad { get; set; }
        [Required(ErrorMessage = "El correo es obligatorio")]
        public string? Correo { get; set; }
    }
}
