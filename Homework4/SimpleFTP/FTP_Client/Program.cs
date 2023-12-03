using System.Net;
using FTP_Client.Client;
using Protocol;

const string filePath = "../../../DownloadedFiles/ClientDownloadedFile";
const string exitMessage = "Exit";
const string showHelpMessage = "Help";
const string wrongRequestMessage = "Wrong request.";
const string helpMessage = $"""

                            Hello, I`m FTP client :)
                            --------------------------------
                            How to run:

                            dotnet run <string remoteIp> <int remotePort>
                            --------------------------------
                            How to create request:

                            1) 1 <string directory_path> - Listing
                            2) 2 <string file_path> - Getting
                            --------------------------------
                            Also:

                            {exitMessage} - exit command.
                            {showHelpMessage} - show help message.
                            """;


if (args.Length != 2 || !IPAddress.TryParse(args[0], out var ip)
                     || !int.TryParse(args[1], out var port))
{
    Console.WriteLine(helpMessage);
    return;
}

var endPoint = new IPEndPoint(ip, port);
var client = await FtpClient.CreateAsync(endPoint);
while (true)
{
    Console.Write("\nEnter request: ");
    var line = Console.ReadLine();

    if (line == exitMessage) break;

    switch (line)
    {
        case showHelpMessage:
            Console.WriteLine(helpMessage);
            break;
        case null:
            Console.WriteLine(wrongRequestMessage);
            continue;
    }

    var lineArgs = line.Split();

    if (lineArgs.Length < 2)
    {
        Console.WriteLine(wrongRequestMessage);
        continue;
    }

    var response = lineArgs[0] switch
    {
        "1" => await client.HandleRequestAsync(new Request.List(lineArgs[1])),
        "2" => await client.HandleRequestAsync(new Request.Get(lineArgs[1])),
        _ => await client.HandleRequestAsync(Request.Unknown.Instance)
    };

    switch (response)
    {
        case Response.Get get:
        {
            if (get.IsQueryCorrect)
            {
                await File.WriteAllBytesAsync(filePath, get.Bytes.ToArray());
                Console.WriteLine("File created.");
            }
            else
            {
                Console.WriteLine("File does not exist");
            }

            break;
        }
        case Response.List list:
        {
            Console.WriteLine(list.IsQueryCorrect
                ? $"List response: \n{list}"
                : "Directory does not exist");
            break;
        }
        case Response.None none:
        {
            Console.WriteLine(none);
            break;
        }
    }
}