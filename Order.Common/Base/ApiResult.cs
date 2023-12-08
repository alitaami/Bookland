using Newtonsoft.Json.Converters;
using System.Net;
using System.Text.Json.Serialization;
using Wallet.Common.Utilities;

namespace Entities.Base
{
    public class ApiResult
    {
        public ApiResult(HttpStatusCode httpStatusCode, ErrorCodeEnum errorCode, string? errorMessage, IEnumerable<FieldErrorItem>? errors)
        {
            Http_Status_Code = (int)httpStatusCode;
            Error_Code = errorCode;
            Error_Message = errorMessage;
            Errors = errors;
        }

        public int Http_Status_Code { get; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ErrorCodeEnum Error_Code { get; }

        public string? Error_Message { get; }

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
        DatabaseWriteError = 21,
        DatabaseConnectionError = 22,
        NullField = 23,
        PermissionDenied = 24,
        JwtExpired = 25,
        JwtTimeIsNull = 26,
        AmountError = 27,
        DepositError = 28,
        CodeNotFound = 29,
        CodeUsed = 30,
        CodeExpired = 31,
        CodeFinished = 32,
        WalletAmountError = 33,
        BookNotFound = 34,
        BookPurchased = 35,
        WelcomeCodeError = 36,
        AuthorizationHeaderMissing = 37,
        TokenTypeError = 38,
        RoleIdClaimMissing = 39,
        UserIdClaimMissing = 40
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
