using System.Net;

namespace Tests;

[TestFixture]
public class FtpClientTests
{
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

    private FtpServer _server;
    private FtpClient _client;

    private const int Port = 12345;
    private const string MainPath = "../../../../Tests/";


    [TestCase(MainPath + "TestFiles/WarAndPeace.fb2")]
    public async Task HandleGetRequestAsync_ValidRequest_ValidResponse(string path)
    {
        var request = new Request.Get(path);
        var data = await File.ReadAllBytesAsync(path);

        var response = (Response.Get)await _client.HandleRequestAsync(request);

        Assert.That(data.SequenceEqual(response.Bytes));
    }


    [TestCase(MainPath + "TestDirectories/SimpleDirectory")]
    public async Task HandleListRequestAsync_ValidRequest_ValidResponse(string path)
    {
        var request = new Request.List(path);
        var response = (Response.List)await _client.HandleRequestAsync(request);

        var subDirs = Directory.GetFileSystemEntries(path);
        var entries = response.ListEntries;

        Assert.That(subDirs, Has.Length.EqualTo(entries.Count));

        Array.Sort(subDirs);
        entries.Sort((x, y) => string.CompareOrdinal(x.Name, y.Name));

        for (var i = 0; i < subDirs.Length; ++i) Assert.That(Path.GetFileName(subDirs[i]), Is.EqualTo(entries[i].Name));
    }


    [Test]
    public async Task TestHandleUnknownRequestAsync()
    {
        using var client = await FtpClient.CreateAsync(new IPEndPoint(IPAddress.Loopback, Port));
        var request = Request.Unknown.Instance;
        var response = await client.HandleRequestAsync(request);

        Assert.That(response, Is.InstanceOf<Response.None>());
    }
}