namespace Api.Tareas.Dtos
{
    public class ResultadoPaginadoDto<T>
    {
        public IReadOnlyList<T> Elementos { get; }
        public int Pagina { get; }
        public int TamanoPagina { get; }
        public int TotalElementos { get; }
        public int TotalPaginas { get; }

        public ResultadoPaginadoDto(
            IReadOnlyList<T> elementos,
            int pagina,
            int tamanoPagina,
            int totalElementos,
            int totalPaginas)
        {
            Elementos = elementos;
            Pagina = pagina;
            TamanoPagina = tamanoPagina;
            TotalElementos = totalElementos;
            TotalPaginas = totalPaginas;
        }
    }
}