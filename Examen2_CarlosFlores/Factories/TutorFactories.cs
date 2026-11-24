using Bogus;
using Examen2_CarlosFlores.Data;
using Examen2_CarlosFlores.Models;

namespace Examen2_CarlosFlores.Factories
{
    public static class TutorFactories
    {
        private const int CantidadEjemplo = 10;

        public static void Seed(BaseDatos context)
        {
            // Completa hasta 10 sin borrar los tutores registrados desde el frontend.
            var faltantes = CantidadEjemplo - context.tutores.Count();
            if (faltantes <= 0)
            {
                return;
            }

            var especialidades = new[]
            {
                "Ingeniería de Software", "Inteligencia Artificial", "Redes",
                "Base de Datos", "Sistemas", "Matemáticas", "Física",
                "Química", "Biología", "Economía"
            };

            var faker = new Faker<Tutor>("es")
                .RuleFor(t => t.Id, f => Guid.NewGuid())
                .RuleFor(t => t.Nombre, f => f.Name.FirstName())
                .RuleFor(t => t.Especialidad, f => f.PickRandom(especialidades))
                .RuleFor(t => t.Correo, f => f.Internet.Email());

            context.tutores.AddRange(faker.Generate(faltantes));
            context.SaveChanges();
        }
    }
}
