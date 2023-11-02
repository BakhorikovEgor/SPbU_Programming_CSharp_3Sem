namespace Chat.Exceptions;

public class ChatMemberIsNotActiveException : Exception
{
    public ChatMemberIsNotActiveException(string message) : base(message)
    {
    }

    public ChatMemberIsNotActiveException() : base()
    {
    }
}