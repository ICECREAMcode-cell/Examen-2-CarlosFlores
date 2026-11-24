using Cliente_Front_end.Services;
using Contrato.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Cliente_Front_end.Pages.Tesis
{
    public class IndexModel : ApiPageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Termino { get; set; }

        [BindProperty]
        public Guid IdTesis { get; set; }

        [BindProperty]
        public IFormFile? Imagen { get; set; }

        public List<TesisDTO> Tesis { get; set; } = new();

        public Dictionary<Guid, string> Autores { get; set; } = new();

        public IndexModel(ApiService api, IConfiguration configuration)
            : base(api, configuration) { }

        public async Task OnGetAsync()
        {
            var tutores = await Api.GetAsync<List<TutorDTO>>("api/Tutor") ?? new();
            Autores = tutores.ToDictionary(t => t.Id, t => t.Nombre);

            if (string.IsNullOrWhiteSpace(Termino))
            {
                Tesis = await Api.GetAsync<List<TesisDTO>>("api/Tesis") ?? new();
            }
            else
            {
                Tesis = await Api.GetAsync<List<TesisDTO>>(
                    $"api/Tesis/buscar?termino={Uri.EscapeDataString(Termino)}") ?? new();
            }
        }

        public string NombreAutor(Guid idTutor)
        {
            return Autores.TryGetValue(idTutor, out var nombre) ? nombre : "Sin autor";
        }

        public async Task<IActionResult> OnPostSubirImagenAsync()
        {
            if (Imagen is null || Imagen.Length == 0)
            {
                TempData["Error"] = "Selecciona una imagen antes de subirla.";
                return RedirectToPage("Index");
            }

            var (ok, ruta, error) = await Api.SubirImagenAsync(IdTesis, Imagen);

            if (ok)
            {
                TempData["Mensaje"] = $"Imagen subida correctamente: {ruta}";
            }
            else
            {
                TempData["Error"] = error ?? "No se pudo subir la imagen.";
            }

            return RedirectToPage("Index");
        }
    }
}
