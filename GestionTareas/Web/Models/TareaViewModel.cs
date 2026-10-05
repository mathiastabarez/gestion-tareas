namespace Web.Tareas.Models
{
    public class TareaViewModel
    {
        public int Id { get; init; }
        public string Titulo { get; init; } = string.Empty;
        public string? Descripcion { get; init; }
        public string Estado { get; init; } = string.Empty;
        public DateTime FechaCreacion { get; init; }
        public DateOnly? FechaVencimiento { get; init; }
    }
}