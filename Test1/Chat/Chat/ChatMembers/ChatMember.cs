using System.Net.Sockets;
using Chat.Exceptions;

namespace Chat.ChatMembers
{
    /// <summary>
    /// An abstract base class representing a chat member for communication in a chat application.
    /// </summary>
    public abstract class ChatMember : IDisposable
    {
        /// <summary>
        /// Gets or sets the TCP client used for communication with a remote chat member.
        /// </summary>
        public abstract TcpClient RemoteClient { get; protected set; }

        /// <summary>
        /// Gets or sets the text writer for sending messages to a remote chat member.
        /// </summary>
        public abstract TextWriter OutStream { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the chat member is currently active.
        /// </summary>
        public abstract bool IsActive { get; protected set; }

        private const string ExitMessage = "exit";

        
        /// <summary>
        /// Asynchronously starts the chat member.
        /// </summary>
        /// <returns>A task representing the asynchronous chat start operation.</returns>
        public virtual async Task StartAsync()
        {
            IsActive = true;
            await OutStream.WriteLineAsync("Chat started");
            await HandleMessagesAsync();
        }

        
        /// <summary>
        /// Stops the chat member, deactivating it and releasing resources.
        /// </summary>
        public void Stop()
        {
            IsActive = false;
            Dispose();
        }

        
        /// <summary>
        /// Asynchronously sends a message to the remote chat member.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A task representing the asynchronous message sending operation.</returns>
        /// <exception cref="ChatMemberIsNotActiveException">Thrown when the chat member is not active.</exception>
        public async Task SendMessageAsync(string message)
        {
            if (!IsActive)
            {
                throw new ChatMemberIsNotActiveException("Member is not active.");
            }

            var writer = new StreamWriter(RemoteClient.GetStream());
            await writer.WriteLineAsync(message);
            await writer.FlushAsync();

            if (message == ExitMessage)
            {
                Stop();
            }
        }

        
        /// <summary>
        /// Asynchronously handles incoming messages from the remote chat member.
        /// </summary>
        /// <returns>A task representing the asynchronous message handling operation.</returns>
        private async Task HandleMessagesAsync()
        {
            using var reader = new StreamReader(RemoteClient.GetStream());
            while (IsActive)
            {
                var message = await reader.ReadLineAsync();
                if (message == ExitMessage)
                {
                    Stop();
                }

                await OutStream.WriteLineAsync($"From remote: {message}");
            }

            await OutStream.WriteLineAsync("End.");
        }

        
        /// <summary>
        /// Releases the resources used by the chat member.
        /// </summary>
        public virtual void Dispose()
        {
            RemoteClient.Dispose();
        }
    }
}