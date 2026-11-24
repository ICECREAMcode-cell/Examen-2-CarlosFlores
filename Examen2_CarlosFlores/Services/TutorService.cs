using Contrato.DTO;
using Examen2_CarlosFlores.Data;
using Examen2_CarlosFlores.Interfaces;
using Examen2_CarlosFlores.Models;

namespace Examen2_CarlosFlores.Services
{
    public class TutorService : ITutorInterface
    {
        private readonly BaseDatos _context;

        public TutorService(BaseDatos context)
        {
            _context = context;
        }

        public IEnumerable<Tutor> ObtenerTodos()
        {
            return _context.tutores.ToList();
        }

        public Tutor? ObtenerPorId(Guid id)
        {
            return _context.tutores.Find(id);
        }

        public void Crear(TutorDTO tutorDTO)
        {
            var nombre = tutorDTO.Nombre.Trim();
            var correo = tutorDTO.Correo.Trim();

            // Regla: no se puede duplicar un tutor (por nombre o correo).
            if (_context.tutores.Any(t => t.Nombre != null &&
                    t.Nombre.ToLower() == nombre.ToLower()))
            {
                throw new InvalidOperationException("Ya existe un tutor con ese nombre.");
            }

            if (_context.tutores.Any(t => t.Correo != null &&
                    t.Correo.ToLower() == correo.ToLower()))
            {
                throw new InvalidOperationException("Ya existe un tutor con ese correo.");
            }

            var tutor = new Tutor
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Especialidad = tutorDTO.Especialidad,
                Correo = correo
            };

            _context.tutores.Add(tutor);
            _context.SaveChanges();
        }
    }
}
