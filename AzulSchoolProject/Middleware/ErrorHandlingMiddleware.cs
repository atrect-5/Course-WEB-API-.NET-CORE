using System.Net;
using System.Text.Json;

namespace AzulSchoolProject.Middleware
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                _logger.LogError("Exception type: {ExceptionType}", ex.GetType().FullName);
                _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string message;

            switch (exception)
            {
                case ArgumentException argEx when argEx.Message.Contains("ya está en uso"):
                    statusCode = HttpStatusCode.Conflict; // 409
                    message = argEx.Message;
                    break;
                case ArgumentException argEx:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    message = argEx.Message;
                    break;
                case InvalidOperationException opEx when opEx.Message.Contains("could not be translated"):
                    // Captura errores de traducción de LINQ para evitar exponer detalles internos.
                    statusCode = HttpStatusCode.BadRequest; // 400
                    message = "La consulta no se pudo procesar. Por favor, revise los filtros aplicados.";
                    break;
                case InvalidOperationException opEx:
                    statusCode = HttpStatusCode.BadRequest; // 400
                    message = opEx.Message;
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError; // 500
                    message = "Ha ocurrido un error interno inesperado.";
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new { error = message });
            return context.Response.WriteAsync(result);
        }
    }
}