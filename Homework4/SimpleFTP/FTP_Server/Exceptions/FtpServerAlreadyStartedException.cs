namespace FTP_Server.Exceptions;

/// <summary>
///     Exception thrown when attempting to start an FTP server that is already running.
/// </summary>
public class FtpServerAlreadyStartedException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the FtpServerAlreadyStartedException class with the specified error message.
    /// </summary>
    /// <param name="message">A description of the error that occurred.</param>
    public FtpServerAlreadyStartedException(string message) : base(message)
    {
    }
}