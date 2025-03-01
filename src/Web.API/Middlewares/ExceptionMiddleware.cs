using FluentValidation;
using System.Text.Json;
using Web.Application.Interfaces;
using Web.Domain.Entities;

namespace Web.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (ValidationException validationException)
            {
                _logger.LogWarning($"Erro de validação: {validationException.Message}");
                await HandleValidationExceptionAsync(httpContext, validationException);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro capturado: {ex.Message}");

                // Registrar erro no banco de dados
                var errorLogService = httpContext.RequestServices.GetRequiredService<IErrorLogService>();
                var errorLog = new ErrorLog
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    DateOccurred = DateTime.UtcNow
                };

                await errorLogService.LogErrorAsync(errorLog);

                // Lidar com erros gerais
                await HandleGenericExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            var validationErrors = exception.Errors.Select(e => new
            {
                Field = e.PropertyName,
                Error = e.ErrorMessage
            });

            var response = new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Erro de validação. Verifique os detalhes e tente novamente.",
                Errors = validationErrors
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private async Task HandleGenericExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new
            {
                StatusCode = StatusCodes.Status500InternalServerError,
                Message = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}