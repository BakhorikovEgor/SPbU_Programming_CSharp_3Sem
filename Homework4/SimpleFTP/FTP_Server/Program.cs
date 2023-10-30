using System.Net;
using FTP_Server.Server;

if (args.Length == 1 && Directory.Exists(args[0]))
{
    Directory.SetCurrentDirectory(args[0]);
    
    var endPoint = new IPEndPoint(IPAddress.Loopback, 8888);
    var server = new FtpServer(endPoint);
    await server.StartAsync();

    return;
}

Console.WriteLine("""
                  
                  Hello, I`m server :)
                  --------------------------------------------
                  I can handle two types of requests:
                  
                  1) 1 <string directory_path> - Listing 
                  2) 2 <string file_path> - Getting
                  --------------------------------------------
                  How to start me:
                  
                  dotnet run <string directory_path>
                  
                  (directory_path - path to base directory)
                  """);
