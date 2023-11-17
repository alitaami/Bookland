using Newtonsoft.Json.Converters;
using System.Net;
using System.Text.Json.Serialization;

namespace Entities.Base
{
    public class ApiResult
    {
        public ApiResult(HttpStatusCode httpStatusCode, ErrorCodeEnum errorCode, string? errorMessage, IEnumerable<FieldErrorItem>? errors)
        {
            HttpStatusCode = (int)httpStatusCode;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            Errors = errors;
        }

        public int HttpStatusCode { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorCodeEnum ErrorCode { get; }

        public string? ErrorMessage { get; }

        public IEnumerable<FieldErrorItem>? Errors { get; }
    }

    public enum ErrorCodeEnum
    {
        None = 0,
        Unknown = 1,
        ValidationError = 2,
        RegistrationError = 3,
        DuplicateError = 4,
        VerificationError = 5,
        InternalError = 6,
        SubSystemError = 7,
        NotFound = 8,
        BadRequest = 9,
        BadGateway = 10,
        GeneralErrorTryAgain = 11,
        UserAlreadyExists = 12,
        ServerError = 13,
        UnAuthorized = 14,
        UpdateError = 15,
        ForeignKeyVonstraintViolation = 16,
        ThereIsNoUserWithThisInformation = 17,
        RoleNotFound = 18,
        UserIsNotActive = 19,
        DuplicateKey = 20,
        DatabaseWriteError =21,
        DatabaseConnectionError =22,
        NullField =23
    }

    public class FieldErrorItem
    {
        public FieldErrorItem(string fieldName, List<string> fieldError)
        {
            FieldName = fieldName;
            FieldError = fieldError;
        }

        public string FieldName { get; }

        public List<string> FieldError { get; }
    }

    public class ServiceResult
    {
        public ServiceResult(object? data, ApiResult result)
        {
            Result = result;
            Data = data;
        }

        public ApiResult Result { get; }
        public object? Data { get; set; }
    }
}
