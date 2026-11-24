using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examen2_CarlosFlores.Models
{
    public class Tesis
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "El título es obligatorio")]
        public string? Titulo { get; set; }
        [Required(ErrorMessage = "El estudiante es obligatorio")]
        public string? Estudiante { get; set; }
        [Required(ErrorMessage = "El estado es obligatorio")]
        public string? Estado { get; set; }
        public string? Image { get; set; }
        [Required(ErrorMessage = "El tutor es obligatorio")]
        public Guid Id_Tutor { get; set; }
        [Required(ErrorMessage = "El tutor es obligatorio")]
        [ForeignKey(nameof(Id_Tutor))]
        public virtual Tutor? Tutor { get; set; }
    }

}
