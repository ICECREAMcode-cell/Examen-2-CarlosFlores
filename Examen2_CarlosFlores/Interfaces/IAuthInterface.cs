using Contrato.DTO;

namespace Examen2_CarlosFlores.Interfaces
{
    public interface IAuthInterface
    {
        void Registrar(LoginDTO dto);
        LoginRespuestaDTO? Login(LoginDTO dto);
    }
}
