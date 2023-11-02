using System.Net;
using System.Net.Sockets;

namespace Chat.ChatMembers;

public sealed class ChatClient : ChatMember
{
    public override bool IsActive { get; set; }

    private readonly TcpClient _client;

    private readonly IPEndPoint _remoteEndPoint;


    public ChatClient(IPEndPoint remoteEndPoint)
    {
        _client = new TcpClient();
        IsActive = false;

        _remoteEndPoint = remoteEndPoint;
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
        Console.WriteLine("Starting client...");
        await _client.ConnectAsync(_remoteEndPoint);

        IsActive = true;
        await HandleClientAsync(_client);
    }


    public override void Dispose()
    {
        IsActive = false;
        _client.Dispose();
    }
}