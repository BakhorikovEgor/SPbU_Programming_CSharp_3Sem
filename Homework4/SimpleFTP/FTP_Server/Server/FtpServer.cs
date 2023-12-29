using System.Net;
using System.Net.Sockets;
using FTP_Server.Exceptions;
using FTP_Server.Utils;
using Protocol;

namespace FTP_Server.Server;

/// <summary>
///     The FtpServer class represents an FTP server for handling FTP client requests.
/// </summary>
public class FtpServer
{
    private readonly TcpListener _listener;
    private volatile bool _isStarted;

    /// <summary>
    ///     Initializes a new instance of the FtpServer class with the specified endpoint.
    /// </summary>
    /// <param name="endPoint">The endpoint at which the FTP server should listen for incoming connections.</param>
    public FtpServer(IPEndPoint endPoint)
    {
        _listener = new TcpListener(endPoint);
        _isStarted = false;
    }

    /// <summary>
    ///     Asynchronously starts the FTP server and begins listening for incoming client connections.
    /// </summary>
    /// <exception cref="FtpServerAlreadyStartedException">Thrown if the server is already started.</exception>
    public async Task StartAsync()
    {
        _isStarted = _isStarted
            ? throw new FtpServerAlreadyStartedException("This server is already started.")
            : true;

        try
        {
            _listener.Start();

            Console.WriteLine("Server is working...");

            var tasks = new List<Task>();
            while (_isStarted)
            {
                var client = await _listener.AcceptTcpClientAsync();
                var clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;

                Console.WriteLine($"\nNew client \nIp: {clientEndPoint.Address} Port: {clientEndPoint.Port}");

                tasks.Add(Task.Run(async () => await _handleClientAsync(client, clientEndPoint)));
            }

            await Task.WhenAll(tasks);

            Console.WriteLine("Server stopped");
        }
        finally
        {
            Stop();
        }
    }

    /// <summary>
    ///     Stops the FTP server and closes the listening socket.
    /// </summary>
    public void Stop()
    {
        _listener.Stop();
        _isStarted = false;
    }


    private async Task _handleClientAsync(TcpClient client, IPEndPoint clientEndPoint)
    {
        try
        {
            using var reader = new StreamReader(client.GetStream());
            while (client.Connected && _isStarted)
            {
                var request = Request.Parse(await reader.ReadLineAsync());

                Console.WriteLine(
                    $"\nRequest from client \nIp: {clientEndPoint.Address} Port: {clientEndPoint.Port} \n" +
                    $"Request: {request}");

                switch (request)
                {
                    case Request.List list:
                        await RequestHandlers.SendListResponseAsync(list, client.GetStream());
                        break;
                    case Request.Get get:
                        await RequestHandlers.SendGetResponseAsync(get, client.GetStream());
                        break;
                    case Request.Unknown none:
                        await RequestHandlers.SendNoneResponseAsync(none, client.GetStream());
                        break;
                }
            }
        }
        finally
        {
            client.Dispose();
            Console.WriteLine($"\nDisconnect client \nIp: {clientEndPoint.Address} Port: {clientEndPoint.Port}");
        }
    }
}