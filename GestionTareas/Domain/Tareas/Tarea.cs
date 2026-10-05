namespace Domain.Tareas
{
    public class Tarea
    {
        public int Id { get; private set; }
        public string Titulo { get; private set; }
        public string? Descripcion { get; private set; }
        public EstadoTarea Estado { get; private set; }
        public DateTime FechaCreacion { get; private set; }
        public DateOnly? FechaVencimiento { get; private set; }

        public Tarea(string titulo, string? descripcion, DateOnly? fechaVencimiento)
        {
            FechaCreacion = DateTime.UtcNow;
            var (tituloValidado, descripcionValidada) = ValidarYSanitizar(titulo, descripcion, fechaVencimiento);
            Titulo = tituloValidado;
            Descripcion = descripcionValidada;
            Estado = EstadoTarea.Pendiente;
            FechaVencimiento = fechaVencimiento;
            
        }
        public Tarea(int id, string titulo, string? descripcion, EstadoTarea estado, DateTime fechaCreacion, DateOnly? fechaVencimiento)
        {
            Id = id;
            Titulo = titulo;
            Descripcion = descripcion;
            Estado = estado;
            FechaCreacion = fechaCreacion;
            FechaVencimiento = fechaVencimiento;
        }
        public void CambiarEstado(EstadoTarea nuevoEstado)
        {
            Estado = nuevoEstado;
        }
        public void Actualizar(string titulo, string? descripcion, DateOnly? fechaVencimiento)
        {
            var (tituloValidado, descripcionValidada) = ValidarYSanitizar(titulo, descripcion, fechaVencimiento);
            Titulo = tituloValidado;
            Descripcion = descripcionValidada;
            FechaVencimiento = fechaVencimiento;
        }

        private (string Titulo, string? Descripcion) ValidarYSanitizar(string titulo, string? descripcion, DateOnly? fechaVencimiento)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                throw new ArgumentException("El título no puede estar vacío.");
            }
            titulo = titulo.Trim();
            if (titulo.Length > 150)
            {
                throw new ArgumentException("El título no puede tener más de 150 caracteres.");
            }
            if (descripcion != null)
            {
                descripcion = descripcion.Trim();
            }
            if (descripcion != null && descripcion.Length > 1000)
            {
                throw new ArgumentException("La descripción no puede tener más de 1000 caracteres.");
            }
            if (fechaVencimiento != null && fechaVencimiento < DateOnly.FromDateTime(FechaCreacion))
            {
                throw new ArgumentException("La fecha de vencimiento no puede ser anterior a la fecha de creación.");
            }
            return (titulo, descripcion);
        }
    }
}
