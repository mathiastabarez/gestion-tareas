using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Web.Tareas.Models;

namespace Web.Tareas
{
    public class TareasApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TareasApiClient(
            IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResultadoPaginadoApiDto<TareaApiDto>> BuscarAsync(
            string? texto,
            string? estado,
            int pagina,
            int tamanoPagina,
            CancellationToken cancellationToken = default)
        {
            var parametros = new Dictionary<string, string?>
            {
                ["pagina"] = pagina.ToString(
                    CultureInfo.InvariantCulture),

                ["tamanoPagina"] = tamanoPagina.ToString(
                    CultureInfo.InvariantCulture)
            };

            if (!string.IsNullOrWhiteSpace(texto))
            {
                parametros["texto"] = texto;
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                parametros["estado"] = estado;
            }

            var url = QueryHelpers.AddQueryString(
                "api/tareas",
                parametros);

            var client = _httpClientFactory.CreateClient(
                "GestionTareasApi");

            using var response = await client.GetAsync(
                url,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<
                    ResultadoPaginadoApiDto<TareaApiDto>>(
                        cancellationToken)
                ?? throw new InvalidOperationException(
                    "La API devolvió una respuesta vacía.");
        }

        public async Task<TareaApiDto?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient(
                "GestionTareasApi");

            using var response = await client.GetAsync(
                $"api/tareas/{id}",
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TareaApiDto>(
                    cancellationToken);
        }
    }
}