namespace Contrato.DTO
{
    public class ImagenDTO
    {
        public Guid Id_Tesis { get; set; }

        // Solo se guarda la ubicación (ruta) de la imagen, no el archivo.
        public string Ruta { get; set; } = string.Empty;
    }
}
