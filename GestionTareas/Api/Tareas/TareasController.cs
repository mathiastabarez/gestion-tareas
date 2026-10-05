using System.ComponentModel.DataAnnotations;
using Api.Tareas.Dtos;
using Application.Tareas;
using Microsoft.AspNetCore.Mvc;

namespace Api.Tareas
{
    [ApiController]
    [Route("api/tareas")]
    public class TareasController : ControllerBase
    {
        private readonly ITareaService _tareaService;

        public TareasController(ITareaService tareaService)
        {
            _tareaService = tareaService;
        }

        [HttpGet]
        [ProducesResponseType(
            typeof(ResultadoPaginadoDto<TareaDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ValidationProblemDetails),
            StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResultadoPaginadoDto<TareaDto>>> Buscar(
            [FromQuery] BusquedaTareasDto busqueda,
            CancellationToken cancellationToken)
        {
            var resultado = await _tareaService.BuscarAsync(
                busqueda.Texto,
                busqueda.Estado,
                busqueda.Pagina,
                busqueda.TamanoPagina,
                cancellationToken);

            return Ok(TareaMapper.Mapear(resultado));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(TareaDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(
            typeof(ValidationProblemDetails),
            StatusCodes.Status400BadRequest)]
        [ProducesResponseType(
            typeof(ProblemDetails),
            StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TareaDto>> ObtenerPorId(
            [FromRoute, Range(1, int.MaxValue)] int id,
            CancellationToken cancellationToken)
        {
            var tarea = await _tareaService.ObtenerPorIdAsync(
                id,
                cancellationToken);

            return Ok(TareaMapper.Mapear(tarea));
        }
    }
}