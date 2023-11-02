namespace Chat.Exceptions
{
    /// <summary>
    /// Exception thrown when an operation is attempted on a chat member that is not currently active.
    /// </summary>
    public class ChatMemberIsNotActiveException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ChatMemberIsNotActiveException class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that describes the exception.</param>
        public ChatMemberIsNotActiveException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChatMemberIsNotActiveException class.
        /// </summary>
        public ChatMemberIsNotActiveException() : base()
        {
        }
    }
}