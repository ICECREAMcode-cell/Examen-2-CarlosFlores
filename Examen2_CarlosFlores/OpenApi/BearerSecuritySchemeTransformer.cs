using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Examen2_CarlosFlores.OpenApi
{
    /// <summary>
    /// Agrega el esquema Bearer/JWT al documento OpenAPI para que Swagger UI
    /// muestre el botón Authorize y envíe el token en las peticiones.
    /// </summary>
    public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        public Task TransformAsync(
            OpenApiDocument document,
            OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??=
                new Dictionary<string, IOpenApiSecurityScheme>();

            document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Pegue aquí el token JWT obtenido en /api/Auth/Login (sin la palabra Bearer)."
            };

            document.Security ??= new List<OpenApiSecurityRequirement>();
            document.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
            });

            return Task.CompletedTask;
        }
    }
}
