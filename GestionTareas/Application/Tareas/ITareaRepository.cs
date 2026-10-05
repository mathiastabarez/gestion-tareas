using Application.Comun;
using Domain.Tareas;

namespace Application.Tareas
{
    public interface ITareaRepository
    {
        Task<ResultadoPaginado<Tarea>> BuscarAsync(
            BusquedaTareas busqueda,
            CancellationToken cancellationToken = default);

        Task<Tarea?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default);
    }
}