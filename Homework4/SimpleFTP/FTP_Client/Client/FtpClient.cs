using System.Net;
using System.Net.Sockets;
using System.Text;

namespace FTP_Client.Client;

public class FtpClient : IDisposable
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    private readonly Semaphore _semaphore = new Semaphore(1, 1);

    private const int BufferSize = 512;


    public enum RequestType
    {
        List,
        Get
    }

    public FtpClient(IPEndPoint remoteEndPoint)
    {
        _client = new TcpClient();

        _client.Connect(remoteEndPoint);

        _stream = _client.GetStream();
    }

    public FtpClient(IPEndPoint localEndPoint, IPEndPoint remoteEndPoint)
    {
        _client = new TcpClient(localEndPoint);

        _client.Connect(remoteEndPoint);

        _stream = _client.GetStream();
    }

    public async Task HandleRequestAsync(RequestType type, string path)
    {
        _semaphore.WaitOne();
        try
        {
            switch (type)
            {
                case RequestType.List:
                {
                    await _stream.WriteAsync(Encoding.UTF8.GetBytes($"2 {path}\n"));
                    await _stream.FlushAsync();
                    await _handleListResponse();
                    break;
                }
                case RequestType.Get:
                {
                    await _stream.WriteAsync(Encoding.UTF8.GetBytes($"2 {path}\n"));
                    await _stream.FlushAsync();
                    await _handleGetResponse();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }


    private async Task _handleListResponse()
    {
        var size = _readSize();
        if (size == -1)
        {
            Console.WriteLine("No such directory");
            return;
        }

        var buffer = new byte[BufferSize];
        var builder = new StringBuilder();
        int bytesCount;
        do
        {
            bytesCount = await _stream.ReadAsync(buffer);
            builder.Append(Encoding.UTF8.GetString(buffer, 0, bytesCount));
        } while (buffer[bytesCount - 1] != '\n');

        Console.WriteLine(builder.ToString());
    }


    private async Task _handleGetResponse()
    {
        var size = _readSize();
        if (size == -1)
        {
            Console.WriteLine("No such file");
            return;
        }
        
        var buffer = new byte[BufferSize];
        var bytes = new List<byte>();
        while (bytes.Count != size)
        {
            
        }
    }


    private int _readSize()
    {
        var sizeBytes = new List<byte>();

        int singleByte;
        while ((singleByte = _stream.ReadByte()) != ' ')
        {
            sizeBytes.Add((byte)singleByte);
        }

        return int.Parse(Encoding.UTF8.GetString(sizeBytes.ToArray()));
    }

    public void Dispose()
    {
        _client.Dispose();
        _semaphore.Dispose();
    }
}