using Contrato.DTO;
using Examen2_CarlosFlores.Models;

namespace Examen2_CarlosFlores.Interfaces
{
    public interface ITutorInterface
    {
        IEnumerable<Tutor> ObtenerTodos();
        Tutor? ObtenerPorId(Guid id);
        void Crear(TutorDTO tutorDTO);
    }
}
