using Cliente_Front_end.Services;
using Contrato.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Cliente_Front_end.Pages.Autor
{
    public class CreateModel : ApiPageModel
    {
        [BindProperty]
        public TutorDTO Autor { get; set; } = new();

        public CreateModel(ApiService api, IConfiguration configuration)
            : base(api, configuration) { }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var (ok, error) = await Api.PostAsync("api/Tutor", Autor);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty,
                    error ?? "No se pudo registrar el autor.");
                return Page();
            }

            TempData["Mensaje"] = "Autor registrado correctamente.";
            return RedirectToPage("Index");
        }
    }
}
