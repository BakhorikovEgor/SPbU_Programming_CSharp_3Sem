using System.Net;

namespace ChatTests;

public class Tests
{
    private ChatServer _server;
    private ChatClient _client;
    
    [SetUp]
    public void SetUp()
    {
        _client = new ChatClient(new IPEndPoint(IPAddress.Loopback, 8888));
        _server = new ChatServer(8888);
    }
    
    [Test]
    public void ClientAddMessageServerGet()
    {
        _client.StartAsync();
        _server.StartAsync();
        
        _client.SendMessageAsync()
        
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}