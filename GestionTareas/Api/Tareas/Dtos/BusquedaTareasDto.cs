using System.ComponentModel.DataAnnotations;
using Domain.Tareas;

namespace Api.Tareas.Dtos
{
    public class BusquedaTareasDto
    {
        public string? Texto { get; init; }

        [EnumDataType(typeof(EstadoTarea))]
        public EstadoTarea? Estado { get; init; }

        [Range(1, int.MaxValue)]
        public int Pagina { get; init; } = 1;

        [Range(1, 50)]
        public int TamanoPagina { get; init; } = 10;
    }
}