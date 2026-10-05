using System.Data;
using Application.Comun;
using Application.Tareas;
using Domain.Tareas;
using Microsoft.Data.SqlClient;

namespace Infrastructure.Tareas
{
    public class SqlTareaRepository : ITareaRepository
    {
        private readonly string _connectionString;

        public SqlTareaRepository(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException(
                    "La cadena de conexión es obligatoria.",
                    nameof(connectionString));
            }

            _connectionString = connectionString;
        }

        public async Task<ResultadoPaginado<Tarea>> BuscarAsync(
            BusquedaTareas busqueda,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(busqueda);

            await using var connection =
                new SqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            var totalElementos = await ContarAsync(
                connection,
                busqueda,
                cancellationToken);

            var tareas = await BuscarPaginaAsync(
                connection,
                busqueda,
                cancellationToken);

            return new ResultadoPaginado<Tarea>(
                tareas,
                busqueda.Pagina,
                busqueda.TamanoPagina,
                totalElementos);
        }

        public async Task<Tarea?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            const string sql = """
                SELECT
                    [Id],
                    [Titulo],
                    [Descripcion],
                    [Estado],
                    [FechaCreacion],
                    [FechaVencimiento]
                FROM [dbo].[Tareas]
                WHERE [Id] = @Id;
                """;

            await using var connection =
                new SqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            await using var command =
                new SqlCommand(sql, connection);

            command.Parameters
                .Add("@Id", SqlDbType.Int)
                .Value = id;

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                return null;
            }

            return MapearTarea(reader);
        }

        private static async Task<int> ContarAsync(
            SqlConnection connection,
            BusquedaTareas busqueda,
            CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT COUNT(*)
                FROM [dbo].[Tareas]
                WHERE
                    (
                        @Texto IS NULL
                        OR [Titulo] LIKE N'%' + @Texto + N'%'
                        OR [Descripcion] LIKE N'%' + @Texto + N'%'
                    )
                    AND
                    (
                        @Estado IS NULL
                        OR [Estado] = @Estado
                    );
                """;

            await using var command =
                new SqlCommand(sql, connection);

            AgregarParametrosDeBusqueda(command, busqueda);

            var resultado =
                await command.ExecuteScalarAsync(cancellationToken);

            return Convert.ToInt32(resultado);
        }

        private static async Task<List<Tarea>> BuscarPaginaAsync(
            SqlConnection connection,
            BusquedaTareas busqueda,
            CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT
                    [Id],
                    [Titulo],
                    [Descripcion],
                    [Estado],
                    [FechaCreacion],
                    [FechaVencimiento]
                FROM [dbo].[Tareas]
                WHERE
                    (
                        @Texto IS NULL
                        OR [Titulo] LIKE N'%' + @Texto + N'%'
                        OR [Descripcion] LIKE N'%' + @Texto + N'%'
                    )
                    AND
                    (
                        @Estado IS NULL
                        OR [Estado] = @Estado
                    )
                ORDER BY
                    [FechaCreacion] DESC,
                    [Id] DESC
                OFFSET @Offset ROWS
                FETCH NEXT @TamanoPagina ROWS ONLY;
                """;

            await using var command =
                new SqlCommand(sql, connection);

            AgregarParametrosDeBusqueda(command, busqueda);

            var offset = checked(
                (busqueda.Pagina - 1) * busqueda.TamanoPagina);

            command.Parameters
                .Add("@Offset", SqlDbType.Int)
                .Value = offset;

            command.Parameters
                .Add("@TamanoPagina", SqlDbType.Int)
                .Value = busqueda.TamanoPagina;

            var tareas = new List<Tarea>();

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                tareas.Add(MapearTarea(reader));
            }

            return tareas;
        }

        private static void AgregarParametrosDeBusqueda(
            SqlCommand command,
            BusquedaTareas busqueda)
        {
            command.Parameters
                .Add("@Texto", SqlDbType.NVarChar, -1)
                .Value = busqueda.Texto is null
                    ? DBNull.Value
                    : busqueda.Texto;

            command.Parameters
                .Add("@Estado", SqlDbType.NVarChar, 20)
                .Value = busqueda.Estado.HasValue
                    ? busqueda.Estado.Value.ToString()
                    : DBNull.Value;
        }

        private static Tarea MapearTarea(SqlDataReader reader)
        {
            var estadoTexto = reader.GetString(
                reader.GetOrdinal("Estado"));

            if (!Enum.TryParse<EstadoTarea>(
                    estadoTexto,
                    out var estado))
            {
                throw new InvalidOperationException(
                    $"El estado '{estadoTexto}' almacenado en la base de datos no es válido.");
            }

            var fechaCreacion = DateTime.SpecifyKind(
                reader.GetDateTime(
                    reader.GetOrdinal("FechaCreacion")),
                DateTimeKind.Utc);

            var descripcionOrdinal =
                reader.GetOrdinal("Descripcion");

            var vencimientoOrdinal =
                reader.GetOrdinal("FechaVencimiento");

            return new Tarea(
                reader.GetInt32(reader.GetOrdinal("Id")),
                reader.GetString(reader.GetOrdinal("Titulo")),
                reader.IsDBNull(descripcionOrdinal)
                    ? null
                    : reader.GetString(descripcionOrdinal),
                estado,
                fechaCreacion,
                reader.IsDBNull(vencimientoOrdinal)
                    ? null
                    : DateOnly.FromDateTime(
                        reader.GetDateTime(vencimientoOrdinal)));
        }
    }
}