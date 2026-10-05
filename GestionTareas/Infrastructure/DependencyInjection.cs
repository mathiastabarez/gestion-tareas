using Application.Tareas;
using Infrastructure.Tareas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("GestionTareas");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "No se configuró ConnectionStrings:GestionTareas.");
            }

            services.AddScoped<ITareaRepository>(_ =>
                new SqlTareaRepository(connectionString));

            return services;
        }
    }
}