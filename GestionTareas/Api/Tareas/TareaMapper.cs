using Api.Tareas.Dtos;
using Application.Comun;
using Domain.Tareas;

namespace Api.Tareas
{
    public static class TareaMapper
    {
        public static TareaDto Mapear(Tarea tarea)
        {
            return new TareaDto(
                tarea.Id,
                tarea.Titulo,
                tarea.Descripcion,
                tarea.Estado,
                tarea.FechaCreacion,
                tarea.FechaVencimiento);
        }

        public static ResultadoPaginadoDto<TareaDto> Mapear(
            ResultadoPaginado<Tarea> resultado)
        {
            var elementos = resultado.Elementos
                .Select(Mapear)
                .ToList();

            return new ResultadoPaginadoDto<TareaDto>(
                elementos,
                resultado.Pagina,
                resultado.TamanoPagina,
                resultado.TotalElementos,
                resultado.TotalPaginas);
        }
    }
}