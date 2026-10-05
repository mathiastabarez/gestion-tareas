using Microsoft.AspNetCore.Mvc;
using Web.Tareas.Models;

namespace Web.Tareas
{
    [Route("tareas")]
    public class TareasController : Controller
    {
        private readonly TareasApiClient _tareasApiClient;

        public TareasController(
            TareasApiClient tareasApiClient)
        {
            _tareasApiClient = tareasApiClient;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(
            string? texto,
            string? estado,
            int pagina = 1,
            int tamanoPagina = 10,
            CancellationToken cancellationToken = default)
        {
            var resultado = await _tareasApiClient.BuscarAsync(
                texto,
                estado,
                pagina,
                tamanoPagina,
                cancellationToken);

            var modelo = TareaMapper.Mapear(
                resultado,
                texto,
                estado);

            return View(modelo);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Detalle(
            int id,
            CancellationToken cancellationToken = default)
        {
            var tarea = await _tareasApiClient.ObtenerPorIdAsync(
                id,
                cancellationToken);

            if (tarea is null)
            {
                return NotFound();
            }

            return View(TareaMapper.Mapear(tarea));
        }
    }
}