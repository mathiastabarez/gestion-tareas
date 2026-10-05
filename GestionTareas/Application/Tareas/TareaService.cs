using Application.Comun;
using Domain.Tareas;

namespace Application.Tareas
{
    public class TareaService : ITareaService
    {
        private readonly ITareaRepository _tareaRepository;

        public TareaService(ITareaRepository tareaRepository)
        {
            _tareaRepository = tareaRepository;
        }

        public Task<ResultadoPaginado<Tarea>> BuscarAsync(
            string? texto,
            EstadoTarea? estado,
            int pagina = BusquedaTareas.PaginaPredeterminada,
            int tamanoPagina = BusquedaTareas.TamanoPaginaPredeterminado,
            CancellationToken cancellationToken = default)
        {
            var textoNormalizado = string.IsNullOrWhiteSpace(texto)
                ? null
                : texto.Trim();

            var busqueda = new BusquedaTareas(
                textoNormalizado,
                estado,
                pagina,
                tamanoPagina);

            return _tareaRepository.BuscarAsync(
                busqueda,
                cancellationToken);
        }

        public async Task<Tarea> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(id),
                    "El identificador debe ser mayor que cero.");
            }

            var tarea = await _tareaRepository.ObtenerPorIdAsync(
                id,
                cancellationToken);

            if (tarea is null)
            {
                throw new TareaNoEncontradaException(id);
            }

            return tarea;
        }
    }
}