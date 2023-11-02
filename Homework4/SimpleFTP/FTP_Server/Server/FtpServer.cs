using System.Net;
using System.Net.Sockets;
using System.Text;
using FTP_Server.Exceptions;
using FTP_Server.Utils;

namespace FTP_Server.Server;

public class FtpServer
{
    private volatile bool _isStarted;
    private CancellationTokenSource _source;

    private const int BufferSize = 512;

    public IPEndPoint EndPoint { get; }


    public FtpServer(IPEndPoint endPoint)
    {
        EndPoint = endPoint;

        _isStarted = false;
        _source = new CancellationTokenSource();
    }


    public async Task StartAsync()
    {
        _isStarted = _isStarted
            ? throw new FtpServerAlreadyStartedException("This server is already started.")
            : true;

        var listener = new TcpListener(EndPoint.Address, EndPoint.Port);
        try
        {
            listener.Start();

            Console.WriteLine($"Server is working...  \n Ip: {EndPoint.Address}  Port: {EndPoint.Port}");

            var tasks = new List<Task>();
            while (!_source.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                var clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;

                Console.WriteLine($"\n New client \n Ip: {clientEndPoint.Address} Port: {clientEndPoint.Port}");

                tasks.Add(Task.Run(async () => await _handleClientAsync(client, clientEndPoint)));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("Server stopped");
        }
        finally
        {
            listener.Stop();
            _reset();
        }
    }


    public void Stop()
    {
        if (_isStarted)
        {
            _source.Cancel();
        }
    }


    private async Task _handleClientAsync(TcpClient client, IPEndPoint clientEndPoint)
    {
        try
        {
            var stream = client.GetStream();

            while (client.Connected || !_source.IsCancellationRequested)
            {
                var buffer = new byte[BufferSize];
                var builder = new StringBuilder();
                int bytesCount;
                do
                {
                    bytesCount = await stream.ReadAsync(buffer);
                    builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesCount));
                } while (buffer[bytesCount - 1] != '\n');

                var requestParts = builder.ToString().Split();
                if (requestParts.Length < 2)
                {
                    await RequestHandlers.SendStringAsync("No such request.", stream);
                    return;
                }

                switch (requestParts[0])
                {
                    case "1":
                        await RequestHandlers.ListFilesAsync(requestParts[1], stream);
                        break;
                    case "2":
                        await RequestHandlers.GetFileAsync(requestParts[1], stream);
                        break;
                    default:
                        await RequestHandlers.SendStringAsync("No such request.", stream);
                        break;
                }
            }
        }
        finally
        {
            client.Dispose();
            Console.WriteLine($"\n Disconnect client \n Ip: {clientEndPoint.Address} Port: {clientEndPoint.Port}");
        }
    }

    private void _reset()
    {
        _isStarted = false;
        _source = new CancellationTokenSource();
    }
}