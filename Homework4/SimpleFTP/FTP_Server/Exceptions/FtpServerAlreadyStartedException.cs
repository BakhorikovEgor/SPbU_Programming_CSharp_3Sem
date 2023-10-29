namespace SimpleFTP.Exceptions;

public class FtpServerAlreadyStartedException : Exception
{
    public FtpServerAlreadyStartedException(string message) : base(message)
    {
    }
}