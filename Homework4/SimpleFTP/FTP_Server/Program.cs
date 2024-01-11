using System.Net;
using FTP_Server.Server;

const string exitMessage = "Exit";
const string showHelpMessage = "Help";
const string helpMessage = $"""

                            Hello, I`m server :)
                            --------------------------------------------
                            I can handle two types of requests:

                            1) 1 <string directory_path> - Listing
                            2) 2 <string file_path> - Getting
                            --------------------------------------------
                            How to start me:

                            dotnet run <string ip> <int port>
                            --------------------------------------------

                            Also:

                            {exitMessage} - exit command.
                            {showHelpMessage} - show help message.
                            """;

if (args.Length != 3 || !Directory.Exists(args[0]) || !IPAddress.TryParse(args[1], out var ip)
    || !int.TryParse(args[2], out var port))
{
    Console.WriteLine(helpMessage);
    return;
}

var endPoint = new IPEndPoint(ip, port);
var server = new FtpServer(endPoint);

#pragma warning disable
server.StartAsync();

while (true)
{
    var line = Console.ReadLine();

    if (line == exitMessage)
    {
        server.Stop();
        break;
    }

    if (line == showHelpMessage) Console.WriteLine(helpMessage);
}