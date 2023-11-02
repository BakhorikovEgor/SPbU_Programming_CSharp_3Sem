// See https://aka.ms/new-console-template for more information

using System.Net;
using Chat.ChatMembers;

var helpMessage = """
                  Wrong argument.
                  How to run:
                  dotnet run server port 
                    OR 
                  dotnet run client ip port
                  """;

if (args.Length != 2 && args.Length != 3)
{
    Console.WriteLine(helpMessage);
    return;
}

if (args[0] == "server")
{
    if (int.TryParse(args[1], out var port))
    {
        var server = new ChatServer(port);
        server.StartAsync();
        
        Console.WriteLine("Enter messages");
        while (server.IsActive)
        {
            var message = Console.ReadLine();
            server.SendMessageAsync(message);
        }
    }
    else
    {
        Console.WriteLine(helpMessage);
    }
}
else if (args[0] == "client")
{
    if (IPAddress.TryParse(args[1], out var ip) && int.TryParse(args[2], out var port))
    {
        var client = new ChatClient(new IPEndPoint(ip, port));
        client.StartAsync();

        Console.WriteLine("Enter messages");
        while (client.IsActive)
        {
            var message = Console.ReadLine();
            client.SendMessageAsync(message);
        }
}
    else
    {
        Console.WriteLine(helpMessage);
    }
}
else
{
    Console.WriteLine(helpMessage);
}