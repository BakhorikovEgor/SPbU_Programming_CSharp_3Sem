using System.Net;
using System.Net.Sockets;

namespace Chat.ChatMembers
{
    /// <summary>
    /// Represents a chat client that connects to a remote chat server.
    /// </summary>
    public sealed class ChatClient : ChatMember
    {
        /// <summary>
        /// Gets or sets the TCP client used for communication with the remote chat server.
        /// </summary>
        public override TcpClient RemoteClient { get; protected set; }

        /// <summary>
        /// Gets or sets the text writer for sending messages to the remote chat server.
        /// </summary>
        public override TextWriter OutStream { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the chat client is currently active.
        /// </summary>
        public override bool IsActive { get; protected set; }

        private readonly IPEndPoint _remoteEndPoint;

        
        /// <summary>
        /// Initializes a new instance of the ChatClient class with the specified remote end point and text writer.
        /// </summary>
        /// <param name="remoteEndPoint">The remote end point (IP address and port) to connect to.</param>
        /// <param name="writer">The text writer for sending messages.</param>
        public ChatClient(IPEndPoint remoteEndPoint, TextWriter writer)
        {
            RemoteClient = new TcpClient();
            OutStream = writer;
            IsActive = false;
            _remoteEndPoint = remoteEndPoint;
        }

        
        /// <summary>
        /// Asynchronously starts the chat client by connecting to the remote chat server.
        /// </summary>
        /// <returns>A task representing the asynchronous chat client start operation.</returns>
        public override async Task StartAsync()
        {
            IsActive = true;
            await RemoteClient.ConnectAsync(_remoteEndPoint);
            await base.StartAsync();
        }
    }
}