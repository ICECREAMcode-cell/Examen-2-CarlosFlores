using Contrato.DTO;
using Examen2_CarlosFlores.Models;

namespace Examen2_CarlosFlores.Interfaces
{
    public interface ITesisInterface
    {
        IEnumerable<Tesis> ObtenerTodos();
        Tesis? ObtenerPorId(Guid id);
        IEnumerable<Tesis> Buscar(string termino);
        void Crear(TesisDTO tesisDTO);
    }
}
