using System.Net;
using System.Net.Sockets;
using SimpleFTP.Exceptions;
using SimpleFTP.Utils;

namespace SimpleFTP.FtpServer;

public class FtpServer
{
    private volatile bool _isStarted;
    private CancellationTokenSource _source;
    public IPAddress Ip { get; }

    public int Port { get; }


    public FtpServer(IPEndPoint endPoint)
    {
        Ip = endPoint.Address;
        Port = endPoint.Port;

        _isStarted = false;
        _source = new CancellationTokenSource();
    }


    public async Task StartAsync()
    {
        _isStarted = _isStarted
            ? throw new FtpServerAlreadyStartedException("This server is already started.")
            : true;

        var listener = new TcpListener(Ip, Port);
        try
        {
            listener.Start();

            Console.WriteLine($"Server is working...  \n Ip: {Ip} \n Port: {Port}");

            var tasks = new List<Task>();
            while (!_source.IsCancellationRequested)
            {
                var client = await listener.AcceptTcpClientAsync();
                tasks.Add(Task.Run(async () => await _handleClientAsync(client)));
            }

            await Task.WhenAll(tasks);
        }
        finally
        {
            listener.Stop();
            _reset();
        }
    }


    public void Stop()
        => _source.Cancel();


    private async Task _handleClientAsync(TcpClient client)
    {
        try
        {
            var stream = client.GetStream();
            var reader = new StreamReader(stream);

            while (!_source.IsCancellationRequested)
            {
                var request = await reader.ReadLineAsync();
                if (request is null)
                {
                    continue;
                }

                var requestParts = request.Split();
                if (requestParts.Length < 2)
                {
                    await RequestHandlers.SendStringAsync("No such request.", stream);
                    continue;
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
        }
    }

    private void _reset()
    {
        _isStarted = false;
        _source = new CancellationTokenSource();
    }
}