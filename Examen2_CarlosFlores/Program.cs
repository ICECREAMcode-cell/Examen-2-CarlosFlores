using System.Security.Claims;
using System.Text;
using Examen2_CarlosFlores.Data;
using Examen2_CarlosFlores.Factories;
using Examen2_CarlosFlores.Interfaces;
using Examen2_CarlosFlores.OpenApi;
using Examen2_CarlosFlores.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Base de datos
var conexion = builder.Configuration.GetConnectionString("ConexionDB");
builder.Services.AddDbContext<BaseDatos>(options => options.UseSqlServer(conexion));

// Servicios
builder.Services.AddScoped<ITutorInterface, TutorService>();
builder.Services.AddScoped<ITesisInterface, TesisService>();
builder.Services.AddScoped<IImagenInterface, ImagenService>();
builder.Services.AddScoped<IAuthInterface, AuthService>();

builder.Services.AddControllers();

// Configuración de JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Falta configurar Jwt:Key en appsettings.json.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Examen 2 API v1");
    });
}

// Usuario inicial para probar el login: admin / Admin123*
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<BaseDatos>();
        UsuarioFactories.Seed(context);
        TutorFactories.Seed(context);
        TesisFactories.Seed(context);
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex,
            "No se pudo crear el usuario inicial. Aplique las migraciones con: dotnet ef database update");
    }
}

// Permite servir las imágenes subidas desde /imagenes/...
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
