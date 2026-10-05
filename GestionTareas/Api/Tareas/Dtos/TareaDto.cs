using Domain.Tareas;

namespace Api.Tareas.Dtos
{
    public class TareaDto
    {
        public int Id { get; }
        public string Titulo { get; }
        public string? Descripcion { get; }
        public EstadoTarea Estado { get; }
        public DateTime FechaCreacion { get; }
        public DateOnly? FechaVencimiento { get; }

        public TareaDto(
            int id,
            string titulo,
            string? descripcion,
            EstadoTarea estado,
            DateTime fechaCreacion,
            DateOnly? fechaVencimiento)
        {
            Id = id;
            Titulo = titulo;
            Descripcion = descripcion;
            Estado = estado;
            FechaCreacion = fechaCreacion;
            FechaVencimiento = fechaVencimiento;
        }
    }
}