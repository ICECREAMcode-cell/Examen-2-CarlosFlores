using Cliente_Front_end.Services;
using Contrato.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Cliente_Front_end.Pages.Tesis
{
    public class CreateModel : ApiPageModel
    {
        [BindProperty]
        public TesisDTO Tesis { get; set; } = new();

        public List<TutorDTO> AutoresDisponibles { get; set; } = new();

        public CreateModel(ApiService api, IConfiguration configuration)
            : base(api, configuration) { }

        public async Task OnGetAsync()
        {
            await CargarAutoresAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await CargarAutoresAsync();
                return Page();
            }

            var (ok, error) = await Api.PostAsync("api/Tesis", Tesis);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty,
                    error ?? "No se pudo registrar la tesis.");
                await CargarAutoresAsync();
                return Page();
            }

            TempData["Mensaje"] = "Tesis registrada correctamente.";
            return RedirectToPage("Index");
        }

        private async Task CargarAutoresAsync()
        {
            var tutores = await Api.GetAsync<List<TutorDTO>>("api/Tutor") ?? new();
            var tesis = await Api.GetAsync<List<TesisDTO>>("api/Tesis") ?? new();

            var conteo = tesis
                .GroupBy(t => t.Id_Tutor)
                .ToDictionary(g => g.Key, g => g.Count());

            // Solo se muestran autores que aún pueden recibir tesis.
            AutoresDisponibles = tutores
                .Where(t => !conteo.TryGetValue(t.Id, out var cantidad) || cantidad < 3)
                .ToList();
        }
    }
}
