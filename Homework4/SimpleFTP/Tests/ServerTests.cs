using System.Net;
using FTP_Server.Exceptions;

namespace Tests;

[TestFixture]
public class FtpServerTests
{
    [SetUp]
    public async Task Setup()
    {
        var endPoint = new IPEndPoint(IPAddress.Loopback, Port);

        _server = new FtpServer(endPoint);
        
        #pragma warning disable
        _server.StartAsync();

        _client = await FtpClient.CreateAsync(endPoint);
    }

    [TearDown]
    public void OneTimeTearDown()
    {
        _server.Stop();
        _client.Dispose();
    }

    private const int Port = 8888;

    private FtpServer _server;
    private FtpClient _client;


    [Test]
    public void StartServerTwiceThrowsException() 
        => Assert.Throws<AggregateException>( () => _server.StartAsync().Wait());
}