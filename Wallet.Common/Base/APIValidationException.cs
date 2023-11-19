using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Entities.Base;

public class APIException : HttpRequestException //ValidationException
{
    public IReadOnlyCollection<ValidationResult>? ValidationResults { get; }

    public APIException()
           : this(null, null)
    { ValidationResults = null; }

    public APIException(string? message)
        : this(message, null)
    { ValidationResults = null; }

    public APIException(IReadOnlyCollection<ValidationResult>? validationResults)
        : this(null, null)
    { ValidationResults = validationResults; }

    public APIException(string? message, Exception? inner)
        : base(message, inner)
    {
        ValidationResults = null; 
        
        if (inner != null)
        {
            HResult = inner.HResult;
        }
    }

    public APIException(string? message , HttpStatusCode statusCode)
        : base(message, null , statusCode)
    { ValidationResults = null; }


    public APIException(HttpStatusCode? statusCode , string? message, IReadOnlyCollection<ValidationResult> validationResults)
        : base(message , null, statusCode)
    { ValidationResults = validationResults; }

    //======================
    //public NAPValidationException() : base()
    //{ ValidationResults = null; }

    //public NAPValidationException(ReadOnlyCollection<ValidationResult> validationResults) : base()
    //{ ValidationResults = validationResults; }

    //public NAPValidationException(string? message) : base(message)
    //{ ValidationResults = null; }

    //public NAPValidationException(string? message, ReadOnlyCollection<ValidationResult> validationResults) : base(message)
    //{ ValidationResults = validationResults; }

    //public NAPValidationException(
    //    string? message,
    //    Exception? innerException,
    //    ReadOnlyCollection<ValidationResult> validationResults
    //    ) : base(message, innerException)
    //{
    //    ValidationResults = validationResults;
    //}
}
