// See https://aka.ms/new-console-template for more information

using System.Net;
using Chat.ChatMembers;


class Program
{
    private static readonly TextWriter Writer = Console.Out;

    private const string HelpMessage = """
                                       Wrong argument.
                                       How to run:
                                       dotnet run server port
                                         OR
                                       dotnet run client ip port
                                       """;

    public static void Main(string[] args)
    {
        if (args.Length != 2 && args.Length != 3)
        {
            Console.WriteLine(HelpMessage);
        }

        switch (args[0])
        {
            case "server" when int.TryParse(args[1], out var port):
            {
                var server = ChatServer.CreateAsync(port, Writer).Result;

#pragma warning disable CS4014
                server.StartAsync();

                while (server.IsActive)
                {
                    Task.Run(async () => await SendMessageAsync(server));
                }

                break;
            }
            case "client" when IPAddress.TryParse(args[1], out var ip) && int.TryParse(args[2], out var port):
            {
                var client = new ChatClient(new IPEndPoint(ip, port), Writer);
                client.StartAsync();

                while (client.IsActive)
                {
                    Task.Run(async () => await SendMessageAsync(client));
                }

                break;
            }
            default:
                Console.WriteLine(HelpMessage);
                break;
        }
    }

    private static async Task SendMessageAsync(ChatMember member)
    {
        var message = Console.ReadLine();
        if (message == null)
        {
            return;
        }

        await member.SendMessageAsync(message);
    }
}