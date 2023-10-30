using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FTP_Client.Client;

public class FtpClient : IDisposable
{
    private readonly TcpClient _client;
    private readonly Semaphore _semaphore = new Semaphore(1,1);

    private const int BufferSize = 512;

    public FtpClient()
    {
        _client = new TcpClient();
    }

    public FtpClient(IPEndPoint localEndPoint)
    {
        _client = new TcpClient(localEndPoint);
    }

    public async Task HandleRequestAsync(string request, IPEndPoint remoteEndPoint)
    {
        _semaphore.WaitOne();
        try
        {
            await _client.ConnectAsync(remoteEndPoint);

            if (_client.Connected == false)
            {
                Console.WriteLine("No such end point");
                return;
            }

            var stream = _client.GetStream();
            await stream.WriteAsync(Encoding.UTF8.GetBytes(request));
            await stream.FlushAsync();

            var buffer = new byte[BufferSize];
            var bytes = await stream.ReadAsync(buffer);
            var response = Encoding.UTF8.GetString(buffer, 0, bytes);

            Console.WriteLine(response);
        }
        catch
        {
            // ignored
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _client.Dispose();
        _semaphore.Dispose();
    }
}