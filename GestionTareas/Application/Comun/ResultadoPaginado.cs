namespace Application.Comun
{
    public class ResultadoPaginado<T>
    {
        public IReadOnlyList<T> Elementos { get; }
        public int Pagina { get; }
        public int TamanoPagina { get; }
        public int TotalElementos { get; }
        public int TotalPaginas { get; }

        public ResultadoPaginado(
            IEnumerable<T> elementos,
            int pagina,
            int tamanoPagina,
            int totalElementos)
        {
            ArgumentNullException.ThrowIfNull(elementos);

            if (pagina < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pagina));
            }

            if (tamanoPagina < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tamanoPagina));
            }

            if (totalElementos < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalElementos));
            }

            Elementos = elementos.ToList();
            Pagina = pagina;
            TamanoPagina = tamanoPagina;
            TotalElementos = totalElementos;

            TotalPaginas = totalElementos == 0
                ? 0
                : (int)Math.Ceiling(
                    totalElementos / (double)tamanoPagina);
        }
    }
}