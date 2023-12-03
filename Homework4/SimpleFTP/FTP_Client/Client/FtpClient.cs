using System.Net;
using System.Net.Sockets;
using Protocol;

namespace FTP_Client.Client;

/// <summary>
///     The FtpClient class represents an FTP client for interacting with an FTP server.
/// </summary>
public class FtpClient : IDisposable
{
    private const int BufferSize = 512;
    private const int LongSizeBuffer = 8;
    private readonly TcpClient _client;
    private readonly Semaphore _semaphore = new(1, 1);


    private FtpClient(TcpClient client)
    {
        _client = client;
    }


    /// <summary>
    ///     Releases the resources used by the FtpClient.
    /// </summary>
    public void Dispose()
    {
        _client.Dispose();
        _semaphore.Dispose();
    }

    /// <summary>
    ///     Asynchronously creates a new instance of the FtpClient class and connects to a remote FTP server.
    /// </summary>
    /// <param name="remoteEndPoint">An IPEndPoint representing the remote FTP server.</param>
    /// <returns>An instance of FtpClient connected to the remote FTP server.</returns>
    public static async Task<FtpClient> CreateAsync(IPEndPoint remoteEndPoint)
    {
        var client = new TcpClient();
        await client.ConnectAsync(remoteEndPoint);

        return new FtpClient(client);
    }

    /// <summary>
    ///     Asynchronously handles an FTP request and returns the response from the server.
    /// </summary>
    /// <param name="request">The FTP request to be processed.</param>
    /// <returns>The response from the FTP server.</returns>
    public async Task<Response> HandleRequestAsync(Request request)
    {
        _semaphore.WaitOne();
        try
        {
            var writer = new StreamWriter(_client.GetStream());
            await writer.WriteAsync(request.ToString());
            await writer.FlushAsync();

            return request switch
            {
                Request.List => await _handleListResponseAsync(),
                Request.Get => await _handleGetResponseAsync(),
                Request.Unknown => _handleNoneResponse()
            };
        }
        finally
        {
            _semaphore.Release();
        }
    }


    private async Task<Response> _handleListResponseAsync()
    {
        var reader = new StreamReader(_client.GetStream());
        return Response.List.Parse(await reader.ReadLineAsync());
    }


    private async Task<Response> _handleGetResponseAsync()
    {
        var stream = _client.GetStream();
        var sizeBuffer = new byte[LongSizeBuffer];

        await stream.ReadAsync(sizeBuffer);
        var size = BitConverter.ToInt64(sizeBuffer, 0);

        var buffer = new byte[BufferSize];
        var bytes = new List<byte>();
        while (bytes.Count < size)
        {
            var bytesCount = await stream.ReadAsync(buffer);
            for (var i = 0; i < bytesCount; ++i) bytes.Add(buffer[i]);
        }

        return Response.Get.Parse(size, bytes);
    }


    private Response _handleNoneResponse()
    {
        return Response.None.Instance;
    }
}