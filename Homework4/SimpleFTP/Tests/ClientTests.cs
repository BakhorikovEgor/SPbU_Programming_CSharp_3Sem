using System.Net;
using Protocol.Models;

namespace Tests;

[TestFixture]
public class FtpClientTests
{
    private FtpServer _server;
    private FtpClient _client;

    private const int Port = 12345;
    private const string MainPath = "../../../../Tests/";

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        var endPoint = new IPEndPoint(IPAddress.Loopback, Port);

        _server = new FtpServer(endPoint);
        _server.StartAsync();

        _client = await FtpClient.CreateAsync(endPoint);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        _server.Stop();
        _client.Dispose();
    }
    
    [Test]
    public async Task HandleGetRequestAsync_ValidRequest_ValidResponse()
    {
        const string path = MainPath + "TestFiles/WarAndPeace.fb2";
        var request = new Request.Get(path);
        var data = await File.ReadAllBytesAsync(path);

        var response = (Response.Get)await _client.HandleRequestAsync(request);

        Assert.That(data.SequenceEqual(response.Bytes));
    }
}