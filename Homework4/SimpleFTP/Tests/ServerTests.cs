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
    public async Task StartServerTwiceThrowsException()
    {
        Assert.Throws<FtpServerAlreadyStartedException>(async () => await _server.StartAsync());
    }
}