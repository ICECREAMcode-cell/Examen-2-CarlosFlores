using System.Net.Http.Headers;
using System.Net.Http.Json;
using Contrato.DTO;
using Microsoft.AspNetCore.Http;

namespace Cliente_Front_end.Services
{
    /// <summary>
    /// Consume la API usando el token JWT guardado en la sesión del usuario.
    /// </summary>
    public class ApiService
    {
        public const string TokenKey = "JwtToken";

        private readonly HttpClient _http;
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(
            IHttpClientFactory factory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _http = factory.CreateClient("ApiExamen");
            _baseUrl = (configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7235")
                .TrimEnd('/');
            _httpContextAccessor = httpContextAccessor;
        }

        public string BaseUrl => _baseUrl;

        private string? Token =>
            _httpContextAccessor.HttpContext?.Session.GetString(TokenKey);

        public string RutaImagenPorDefecto => "/imagenes/default.svg";

        /// <summary>Inicia sesión contra la API y devuelve el token y los datos del usuario.</summary>
        public async Task<(LoginRespuestaDTO? Datos, string? Error)> LoginAsync(
            string usuario, string password)
        {
            using var respuesta = await _http.PostAsJsonAsync(
                $"{_baseUrl}/api/Auth/Login",
                new { nombreUsuario = usuario, password = password });

            if (!respuesta.IsSuccessStatusCode)
            {
                var error = await respuesta.Content.ReadAsStringAsync();
                return (null, string.IsNullOrWhiteSpace(error)
                    ? "Usuario o contraseña incorrectos."
                    : error.Trim('"'));
            }

            var datos = await respuesta.Content.ReadFromJsonAsync<LoginRespuestaDTO>();
            return (datos, null);
        }

        private HttpRequestMessage CrearSolicitud(HttpMethod metodo, string ruta)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token))
            {
                throw new InvalidOperationException("No hay una sesión activa.");
            }

            var solicitud = new HttpRequestMessage(metodo, $"{_baseUrl}/{ruta.TrimStart('/')}");
            solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return solicitud;
        }

        public async Task<T?> GetAsync<T>(string ruta)
        {
            using var solicitud = CrearSolicitud(HttpMethod.Get, ruta);
            using var respuesta = await _http.SendAsync(solicitud);

            if (!respuesta.IsSuccessStatusCode)
            {
                return default;
            }

            return await respuesta.Content.ReadFromJsonAsync<T>();
        }

        public async Task<(bool Ok, string? Error)> PostAsync(string ruta, object datos)
        {
            using var solicitud = CrearSolicitud(HttpMethod.Post, ruta);
            solicitud.Content = JsonContent.Create(datos);
            using var respuesta = await _http.SendAsync(solicitud);

            if (respuesta.IsSuccessStatusCode)
            {
                return (true, null);
            }

            var error = await respuesta.Content.ReadAsStringAsync();
            return (false, string.IsNullOrWhiteSpace(error)
                ? "No se pudo completar la operación."
                : error.Trim('"'));
        }

        public async Task<(bool Ok, string? Ruta, string? Error)> SubirImagenAsync(
            Guid idTesis, IFormFile imagen)
        {
            using var contenido = new MultipartFormDataContent();
            contenido.Add(new StringContent(idTesis.ToString()), "idTesis");

            using var archivo = new StreamContent(imagen.OpenReadStream());
            archivo.Headers.ContentType =
                new MediaTypeHeaderValue(imagen.ContentType ?? "application/octet-stream");
            contenido.Add(archivo, "imagen", imagen.FileName);

            using var solicitud = CrearSolicitud(HttpMethod.Post, "api/Imagen");
            solicitud.Content = contenido;

            using var respuesta = await _http.SendAsync(solicitud);
            if (respuesta.IsSuccessStatusCode)
            {
                var datos = await respuesta.Content.ReadFromJsonAsync<ImagenDTO>();
                return (true, datos?.Ruta, null);
            }

            var error = await respuesta.Content.ReadAsStringAsync();
            return (false, null, error);
        }
    }
}
