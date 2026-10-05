namespace Web.Tareas.Models
{
    public class ResultadoPaginadoApiDto<T>
    {
        public List<T> Elementos { get; init; } = [];
        public int Pagina { get; init; }
        public int TamanoPagina { get; init; }
        public int TotalElementos { get; init; }
        public int TotalPaginas { get; init; }
    }
}