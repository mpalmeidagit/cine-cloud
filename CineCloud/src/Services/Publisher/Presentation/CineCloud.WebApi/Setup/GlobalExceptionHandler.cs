using BuildingBlocks.Core.DomainObjects;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace CineCloud.WebApi.Setup;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        this.logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (int statusCode, string errorMessage) = exception switch
        {
            ArgumentNullException argumentException => (500, argumentException.Message),
            DomainException domainException => (500, domainException.Message),
            SqlException sqlException => (500, sqlException.Message),
            ValidationException validationException => (400, FormatValidationErrors(validationException)),
            DbUpdateException dbUpdateException => MapDbUpdateException(dbUpdateException),
            _ => (500, "Algo deu errado")
        };

        logger.LogError(exception, exception.Message);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(errorMessage, cancellationToken);
        return true;
    }

    private static string FormatValidationErrors(ValidationException exception) =>
        string.Join(" | ", exception.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

    private const int FOREIGN_KEY_VIOLATION_ERROR_NUMBER = 547;

    private static (int StatusCode, string ErrorMessage) MapDbUpdateException(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: FOREIGN_KEY_VIOLATION_ERROR_NUMBER }
            ? (400, "Referência inválida: verifique se os identificadores informados existem.")
            : (500, "Algo deu errado");
}