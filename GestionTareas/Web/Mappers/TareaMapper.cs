using Web.Tareas.Models;

namespace Web.Tareas
{
    public static class TareaMapper
    {
        public static TareaViewModel Mapear(TareaApiDto tarea)
        {
            return new TareaViewModel
            {
                Id = tarea.Id,
                Titulo = tarea.Titulo,
                Descripcion = tarea.Descripcion,
                Estado = tarea.Estado,
                FechaCreacion = tarea.FechaCreacion,
                FechaVencimiento = tarea.FechaVencimiento
            };
        }

        public static ListaTareasViewModel Mapear(
            ResultadoPaginadoApiDto<TareaApiDto> resultado,
            string? texto,
            string? estado)
        {
            return new ListaTareasViewModel
            {
                Tareas = resultado.Elementos
                    .Select(Mapear)
                    .ToList(),

                Texto = texto,
                Estado = estado,
                Pagina = resultado.Pagina,
                TamanoPagina = resultado.TamanoPagina,
                TotalElementos = resultado.TotalElementos,
                TotalPaginas = resultado.TotalPaginas
            };
        }
    }
}