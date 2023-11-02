using System.Net;
using System.Net.Sockets;

namespace Chat.ChatMembers;

public sealed class ChatServer : ChatMember
{
    public override bool IsActive { get; set; }

    private readonly TcpListener _listener;

    private TcpClient? _client;


    public ChatServer(int port)
    {
        _listener = new TcpListener(IPAddress.Loopback, port);
        IsActive = false;
    }


    public override async Task SendMessageAsync(string message)
    {
        if (IsActive)
        {
            await SendMessageAsync(_client, message);
        }
    }

    public override async Task StartAsync()
    {
        Console.WriteLine("Starting Server...");
        IsActive = true;
        while (IsActive)
        {
            _client = await _listener.AcceptTcpClientAsync();
            await HandleClientAsync(_client);
        }
    }

    public override void Dispose()
    {
        IsActive = false;
        _listener.Stop();
    }
}