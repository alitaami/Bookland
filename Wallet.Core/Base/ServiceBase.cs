using Entities.Base;
using Microsoft.Extensions.Logging;
using Npgsql; // Add this for PostgreSQL support
using System.ComponentModel.DataAnnotations;
using System.Net;
using Wallet.Common.Resources;

public class ServiceBase<Tclass>
{
    protected readonly ILogger<Tclass> _logger;

    public ServiceBase(ILogger<Tclass> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected virtual ServiceResult HandleException(Exception ex)
    {
        try
        {
            if (ex is PostgresException pgEx) // Handle PostgreSQL-specific exceptions
            {
                return HandlePostgresException(pgEx);
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
            throw;
        }
    }

    protected virtual ServiceResult Ok(object data)
    {
        if (data == null)
        {
            return Ok();
        }

        return new ServiceResult(data, new ApiResult( HttpStatusCode.OK, ErrorCodeEnum.None, null, null));
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

    protected virtual ServiceResult HandlePostgresException(PostgresException pgEx)
    {
        // Handle specific PostgreSQL errors. For example, unique constraint violations.
        if (pgEx.SqlState == "23505") // Unique constraint violation
        {
            return new ServiceResult(
                null,
                new ApiResult(HttpStatusCode.BadRequest, ErrorCodeEnum.DuplicateKey, "Duplicate entry detected.", null)
            );
        }
        else
        {
            return new ServiceResult(
                null,
                new ApiResult(HttpStatusCode.InternalServerError, ErrorCodeEnum.DatabaseWriteError, "An error occurred while writing to the database.", null)
            );
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
