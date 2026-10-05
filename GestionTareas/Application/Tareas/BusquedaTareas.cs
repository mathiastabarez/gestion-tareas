using Domain.Tareas;

namespace Application.Tareas
{
    public class BusquedaTareas
    {
        public const int PaginaPredeterminada = 1;
        public const int TamanoPaginaPredeterminado = 10;
        public const int TamanoPaginaMaximo = 50;

        public string? Texto { get; }
        public EstadoTarea? Estado { get; }
        public int Pagina { get; }
        public int TamanoPagina { get; }

        public BusquedaTareas(
            string? texto = null,
            EstadoTarea? estado = null,
            int pagina = PaginaPredeterminada,
            int tamanoPagina = TamanoPaginaPredeterminado)
        {
            if (pagina < 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(pagina),
                    "La página debe ser mayor que cero.");
            }

            if (tamanoPagina < 1 ||
                tamanoPagina > TamanoPaginaMaximo)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(tamanoPagina),
                    $"El tamaño de página debe estar entre 1 y {TamanoPaginaMaximo}.");
            }

            if (estado.HasValue &&
                !Enum.IsDefined(estado.Value))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(estado),
                    "El estado no es válido.");
            }

            Texto = string.IsNullOrWhiteSpace(texto)
                ? null
                : texto.Trim();

            Estado = estado;
            Pagina = pagina;
            TamanoPagina = tamanoPagina;
        }
    }
}