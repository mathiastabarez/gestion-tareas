namespace Web.Tareas.Models
{
    public class ListaTareasViewModel
    {
        public IReadOnlyList<TareaViewModel> Tareas { get; init; } =
            Array.Empty<TareaViewModel>();

        public string? Texto { get; init; }
        public string? Estado { get; init; }
        public int Pagina { get; init; }
        public int TamanoPagina { get; init; }
        public int TotalElementos { get; init; }
        public int TotalPaginas { get; init; }
    }
}