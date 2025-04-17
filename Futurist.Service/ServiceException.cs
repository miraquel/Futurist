using System.Net;

namespace Futurist.Service;

/// <summary>
/// Represents errors that occur within the service layer.
/// </summary>
public class ServiceException : Exception
{
    /// <summary>
    /// Gets the HTTP status code that is associated with the exception.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceException"/> class.
    /// </summary>
    public ServiceException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public ServiceException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message and HTTP status code.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="statusCode">The HTTP status code to associate with the exception.</param>
    public ServiceException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceException"/> class with a specified error message, a reference to the inner exception that is the cause of this exception, and an HTTP status code.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="statusCode">The HTTP status code to associate with the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ServiceException(string message, HttpStatusCode statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}