using Cliente_Front_end.Services;
using Contrato.DTO;

namespace Cliente_Front_end.Pages.Autor
{
    public class IndexModel : ApiPageModel
    {
        public List<TutorDTO> Autores { get; set; } = new();

        public Dictionary<Guid, int> TesisPorTutor { get; set; } = new();

        public IndexModel(ApiService api, IConfiguration configuration)
            : base(api, configuration) { }

        public async Task OnGetAsync()
        {
            Autores = await Api.GetAsync<List<TutorDTO>>("api/Tutor") ?? new();

            var tesis = await Api.GetAsync<List<TesisDTO>>("api/Tesis") ?? new();
            TesisPorTutor = tesis
                .GroupBy(t => t.Id_Tutor)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public int CantidadTesis(Guid idTutor)
        {
            return TesisPorTutor.TryGetValue(idTutor, out var cantidad) ? cantidad : 0;
        }
    }
}
