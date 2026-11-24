using Contrato.DTO;
using Examen2_CarlosFlores.Data;
using Examen2_CarlosFlores.Interfaces;
using Examen2_CarlosFlores.Models;
using Microsoft.EntityFrameworkCore;

namespace Examen2_CarlosFlores.Services
{
    public class TesisService : ITesisInterface
    {
        private readonly BaseDatos _context;

        public TesisService(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<Tesis> ObtenerTodos()
        {
            return _context.tesis
                .Include(t => t.Tutor)
                .ToList();
        }

        public Tesis? ObtenerPorId(Guid id)
        {
            return _context.tesis
                .Include(t => t.Tutor)
                .FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Tesis> Buscar(string termino)
        {
            var texto = (termino ?? string.Empty).Trim().ToLower();

            return _context.tesis
                .Include(t => t.Tutor)
                .Where(t => texto == string.Empty
                    || (t.Titulo != null && t.Titulo.ToLower().Contains(texto))
                    || (t.Estudiante != null && t.Estudiante.ToLower().Contains(texto))
                    || (t.Estado != null && t.Estado.ToLower().Contains(texto))
                    || (t.Tutor != null && t.Tutor.Nombre != null && t.Tutor.Nombre.ToLower().Contains(texto)))
                .ToList();
        }

        public void Crear(TesisDTO tesisDTO)
        {
            var titulo = tesisDTO.Titulo.Trim();

            if (!_context.tutores.Any(t => t.Id == tesisDTO.Id_Tutor))
            {
                throw new InvalidOperationException("El tutor seleccionado no existe.");
            }

            // Regla: no se puede duplicar una tesis (mismo título).
            if (_context.tesis.Any(t => t.Titulo != null &&
                    t.Titulo.ToLower() == titulo.ToLower()))
            {
                throw new InvalidOperationException("Ya existe una tesis con ese título.");
            }

            // Regla: un tutor solo puede tener hasta 3 tesis.
            var cantidadTesis = _context.tesis.Count(t => t.Id_Tutor == tesisDTO.Id_Tutor);
            if (cantidadTesis >= 3)
            {
                throw new InvalidOperationException(
                    "El tutor ya tiene el máximo de 3 tesis asignadas.");
            }

            var tesis = new Tesis
            {
                Id = Guid.NewGuid(),
                Titulo = titulo,
                Estudiante = tesisDTO.Estudiante,
                Estado = tesisDTO.Estado,
                // Si no se envía imagen, se guarda la ruta de la imagen por defecto.
                Image = string.IsNullOrWhiteSpace(tesisDTO.Image)
                    ? "/imagenes/default.svg"
                    : tesisDTO.Image,
                Id_Tutor = tesisDTO.Id_Tutor
            };

            _context.tesis.Add(tesis);
            _context.SaveChanges();
        }
    }
}
