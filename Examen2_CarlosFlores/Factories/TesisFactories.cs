using Bogus;
using Examen2_CarlosFlores.Data;
using Examen2_CarlosFlores.Models;

namespace Examen2_CarlosFlores.Factories
{
    public static class TesisFactories
    {
        private const int CantidadEjemplo = 20;

        public static void Seed(BaseDatos context)
        {
            // Completa hasta 20 sin borrar las tesis registradas desde el frontend.
            var faltantes = CantidadEjemplo - context.tesis.Count();
            if (faltantes <= 0)
            {
                return;
            }

            var tutores = context.tutores.Select(t => t.Id).ToList();
            if (tutores.Count == 0)
            {
                return;
            }

            var estados = new[] { "En proceso", "Aprobada", "Pendiente", "Revisión" };
            var faker = new Faker("es");
            var titulosUsados = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var nuevas = new List<Tesis>();
            for (var i = 0; i < faltantes; i++)
            {
                string titulo;
                do
                {
                    titulo = faker.Lorem.Sentence(5).TrimEnd('.');
                }
                while (!titulosUsados.Add(titulo));

                nuevas.Add(new Tesis
                {
                    Id = Guid.NewGuid(),
                    Titulo = titulo,
                    Estudiante = faker.Name.FullName(),
                    Estado = faker.PickRandom(estados),
                    Image = "/imagenes/default.svg",
                    // Reparto equilibrado: como máximo 3 tesis por autor.
                    Id_Tutor = tutores[i % tutores.Count]
                });
            }

            context.tesis.AddRange(nuevas);
            context.SaveChanges();
        }
    }
}
