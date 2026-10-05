using Application.Tareas;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Api.Errores
{
    public class TareaNoEncontradaExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _problemDetailsService;

        public TareaNoEncontradaExceptionHandler(
            IProblemDetailsService problemDetailsService)
        {
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            if (exception is not TareaNoEncontradaException excepcion)
            {
                return false;
            }

            httpContext.Response.StatusCode =
                StatusCodes.Status404NotFound;

            return await _problemDetailsService.TryWriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    Exception = exception,
                    ProblemDetails = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Tarea no encontrada",
                        Detail = excepcion.Message,
                        Instance = httpContext.Request.Path
                    }
                });
        }
    }
}