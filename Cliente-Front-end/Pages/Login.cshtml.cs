using System.Security.Claims;
using Cliente_Front_end.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cliente_Front_end.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApiService _api;

        [BindProperty]
        public string Usuario { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public LoginModel(ApiService api)
        {
            _api = api;
        }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Usuario) || string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "Ingresa usuario y contraseña.");
                return Page();
            }

            var (datos, error) = await _api.LoginAsync(Usuario.Trim(), Password);

            if (datos is null || string.IsNullOrEmpty(datos.Token))
            {
                ModelState.AddModelError(string.Empty,
                    error ?? "Usuario o contraseña incorrectos.");
                return Page();
            }

            // El token JWT se guarda en la sesión y viaja en cada llamada a la API.
            HttpContext.Session.SetString(ApiService.TokenKey, datos.Token);
            HttpContext.Session.SetString("UsuarioNombre", datos.NombreUsuario);
            HttpContext.Session.SetString("UsuarioRol", datos.Rol);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, datos.NombreUsuario),
                new(ClaimTypes.Role, datos.Rol)
            };

            var identidad = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identidad));

            return RedirectToPage("/Index");
        }
    }
}
