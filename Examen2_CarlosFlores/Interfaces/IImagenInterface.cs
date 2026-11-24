using Contrato.DTO;
using Microsoft.AspNetCore.Http;

namespace Examen2_CarlosFlores.Interfaces
{
    public interface IImagenInterface
    {
        ImagenDTO Subir(Guid idTesis, IFormFile imagen);
    }
}
