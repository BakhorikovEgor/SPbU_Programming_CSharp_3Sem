using System.Net;
using FTP_Client.Client;


Console.WriteLine("""
                  
                  Hello, I`m FTP client :)
                  --------------------------------
                  I can handle requests to remote server!
                  
                  1) 1 <string directory_path> <string remoteIp> <int remotePort> - Listing
                  2) 2 <string file_path> <string remoteIp> <int remotePort> - Getting
                  --------------------------------
                  How to stop:
                  
                  "Exit" command.
                  
                  """);

while (true)
{
    var line = Console.ReadLine();

    if (line == "Exit")
    {
        break;
    }
    
    if (line is null || line.Split().Length != 4)
    {
        continue;
    }

    try
    {
        var lineArgs = line.Split();
        var endPoint = new IPEndPoint(IPAddress.Parse(lineArgs[2]), int.Parse(lineArgs[3]));
        var client = new FtpClient(new IPEndPoint(IPAddress.Loopback, 9000));

        await client.HandleRequestAsync($"{lineArgs[0]} {lineArgs[1]}", endPoint);
    }
    catch
    {
        // ignored
    }
}
