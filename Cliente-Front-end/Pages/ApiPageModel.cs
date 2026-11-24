using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Cliente_Front_end.Pages
{
    /// <summary>Base común para las páginas que consumen la API.</summary>
    public abstract class ApiPageModel : PageModel
    {
        protected readonly Services.ApiService Api;
        public string ApiBaseUrl { get; }

        protected ApiPageModel(Services.ApiService api, IConfiguration configuration)
        {
            Api = api;
            ApiBaseUrl = (configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7235")
                .TrimEnd('/');
        }
    }
}
