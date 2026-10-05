using Application.Comun;
using Domain.Tareas;

namespace Application.Tareas
{
    public interface ITareaService
    {
        Task<ResultadoPaginado<Tarea>> BuscarAsync(
            string? texto,
            EstadoTarea? estado,
            int pagina = BusquedaTareas.PaginaPredeterminada,
            int tamanoPagina = BusquedaTareas.TamanoPaginaPredeterminado,
            CancellationToken cancellationToken = default);

        Task<Tarea> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}