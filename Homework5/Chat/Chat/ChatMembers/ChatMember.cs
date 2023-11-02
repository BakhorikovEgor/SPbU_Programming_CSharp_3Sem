using System.Net.Sockets;
using Chat.Exceptions;

namespace Chat.ChatMembers;

public abstract class ChatMember : IDisposable
{
    private static readonly string ExitString = "exit";

    public abstract bool IsActive { get; set; }


    protected async Task HandleClientAsync(TcpClient client)
    {
        while (IsActive)
        {
            Console.WriteLine(await ReceiveMessageAsync(client));
            Console.WriteLine();
        }
    }

    private async Task<string> ReceiveMessageAsync(TcpClient client)
    {
        if (!IsActive)
        {
            throw new ChatMemberIsNotActiveException("Member is not active.");
        }

        using var reader = new StreamReader(client.GetStream());
        var message = await reader.ReadLineAsync();

        if (message == ExitString)
        {
            Dispose();
        }

        return $"From remote: {message}";
    }


    public async Task SendMessageAsync(TcpClient client, string message)
    {
        if (IsActive)
        {
            throw new ChatMemberIsNotActiveException("Member is not active.");
        }

        if (message == ExitString)
        {
            Dispose();
        }

        await using var writer = new StreamWriter(client.GetStream());
        await writer.WriteLineAsync(message);
    }


    public abstract Task SendMessageAsync(string message);
    public abstract Task StartAsync();


    public abstract void Dispose();
}