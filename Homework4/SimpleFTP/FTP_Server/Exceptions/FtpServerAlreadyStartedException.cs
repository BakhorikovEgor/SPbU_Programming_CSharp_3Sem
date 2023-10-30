namespace FTP_Server.Exceptions;

public class FtpServerAlreadyStartedException : Exception
{
    public FtpServerAlreadyStartedException(string message) : base(message)
    {
    }
}