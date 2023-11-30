using System.Net;
using System.Net.Sockets;

namespace Chat.ChatMembers
{
    /// <summary>
    /// Represents a chat server that accepts incoming connections from chat clients.
    /// </summary>
    public sealed class ChatServer : ChatMember
    {
        /// <summary>
        /// Gets or sets the TCP client used for communication with a remote chat client.
        /// </summary>
        public override TcpClient RemoteClient { get; protected set; }

        /// <summary>
        /// Gets or sets the text writer for sending messages to the remote chat client.
        /// </summary>
        public override TextWriter OutStream { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the chat server is currently active.
        /// </summary>
        public override bool IsActive { get; protected set; }

        private readonly TcpListener _listener;

        
        /// <summary>
        /// Initializes a new instance of the ChatServer class.
        /// </summary>
        /// <param name="writer">The text writer for sending messages.</param>
        /// <param name="client">The TCP client for communication with a remote chat client.</param>
        /// <param name="listener">The TCP listener for accepting connections.</param>
        private ChatServer(TextWriter writer, TcpClient client, TcpListener listener)
        {
            _listener = listener;
            RemoteClient = client;
            OutStream = writer;
            IsActive = false;
        }

        
        /// <summary>
        /// Creates a new ChatServer instance asynchronously.
        /// </summary>
        /// <param name="port">The port to listen on for incoming connections.</param>
        /// <param name="writer">The text writer for sending messages.</param>
        /// <returns>A ChatServer instance representing the chat server.</returns>
        public static async Task<ChatServer> CreateAsync(int port, TextWriter writer)
        {
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();
            return new ChatServer(writer, client, listener);
        }

        
        /// <summary>
        /// Disposes of the chat server resources, stopping the listener.
        /// </summary>
        public override void Dispose()
        {
            _listener.Stop();
            base.Dispose();
        }
    }
}