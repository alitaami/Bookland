using Entities.Base;
using Microsoft.Extensions.Logging;
using NLog;
using Npgsql;
using Order.Common.Resources;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Sockets;

public class ServiceBase<Tclass>
{
    protected readonly ILogger<Tclass> _logger;
    public ServiceBase(
       ILogger<Tclass> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    protected virtual ServiceResult HandleException(Exception ex)
    {
        try
        {

            if (ex is PostgresException pgEx) // Handle PostgreSQL-specific exceptions
            {
                return HandleNpgsqlException(pgEx);
            }
            else if (ex is APIException napEx)
            {
                return HandleAPIException(napEx);
            }
            else
            {
                return InternalServerError(ErrorCodeEnum.None, Resource.GeneralErrorTryAgain, null);
            }
        }
        catch (Exception generalEx)
        {
            _logger.LogError(generalEx, null);

            throw ex; //Todo : درسته ایا یا باید همینجا هندل بشه
        }
    }

    protected virtual ServiceResult Ok(object data)
    {
        if (data == null)
        {
            return Ok();
        }

        return new ServiceResult(data, new ApiResult(HttpStatusCode.OK, ErrorCodeEnum.None, null, null));
    }

    protected virtual ServiceResult Ok()
    {
        return new ServiceResult(null, new ApiResult(HttpStatusCode.OK, ErrorCodeEnum.None, null, null));
    }

    protected virtual ServiceResult BadRequest(ErrorCodeEnum errorCode, string errorMessage, List<FieldErrorItem> errors)
    {
        return new ServiceResult(null, new ApiResult(HttpStatusCode.BadRequest, errorCode, errorMessage, errors));
    }

    protected virtual ServiceResult NotFound(ErrorCodeEnum errorCode, string errorMessage, List<FieldErrorItem> errors)
    {
        return new ServiceResult(null, new ApiResult(HttpStatusCode.NotFound, errorCode, errorMessage, errors));
    }

    protected virtual ServiceResult InternalServerError(ErrorCodeEnum errorCode, string errorMessage, List<FieldErrorItem> errors)
    {
        return new ServiceResult(null, new ApiResult(HttpStatusCode.InternalServerError, errorCode, errorMessage, errors));
    }
    protected virtual void ValidateModel(object model)
    {
        if (model is null)
            throw new ArgumentNullException(nameof(model));

        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true))
            throw new APIException(HttpStatusCode.BadRequest, Resource.EnterParametersCorrectlyAndCompletely, validationResults.AsReadOnly());
    }

    protected ServiceResult HandleNpgsqlException(NpgsqlException pgEx)
    {
        if (pgEx.InnerException is SocketException)
        {
            // SocketException indicates a network-related error (e.g., database server not reachable)
            return InternalServerError(ErrorCodeEnum.DatabaseConnectionError, "Error connecting to the database", null);
        }
        else if (pgEx.SqlState == "23505")
        {
            // Unique constraint violation
            return BadRequest(ErrorCodeEnum.DuplicateKey, "Duplicate entry detected.", null);
        }
        else
        {
            return InternalServerError(ErrorCodeEnum.DatabaseWriteError, "An error occurred while writing to the database.", null);
        }
    }

    private ServiceResult HandleAPIException(APIException napEx)
    {
        if (napEx is null)
            throw new ArgumentNullException(nameof(napEx));

        if (napEx.ValidationResults == null)
            return BadRequest(ErrorCodeEnum.None, "خطای اعتبارسنجی فیلدها", null);

        var validationResults = new List<FieldErrorItem>();
        napEx.ValidationResults
            .ToList()
            .ForEach(v =>
                validationResults
                .Add(new FieldErrorItem(v.MemberNames.FirstOrDefault() ?? string.Empty, new List<string>() { v.ErrorMessage ?? string.Empty })));

        return BadRequest(ErrorCodeEnum.None, "خطای اعتبارسنجی فیلدها", validationResults);
    }
}
