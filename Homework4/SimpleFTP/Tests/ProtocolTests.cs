using Protocol.Models;

namespace Tests;

[TestFixture]
public class ProtocolTests
{
    [Test]
    public void ListRequestToString()
    {
        var request = new Request.List("/path/to/list");
        const string expected = "1 /path/to/list\n";
        Assert.That(request.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void GetRequestToString()
    {
        var request = new Request.Get("/path/to/get");
        const string expected = "2 /path/to/get\n";
        Assert.That(request.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void ParseListRequest()
    {
        const string input = "1 /path/to/list\n";
        var parsedRequest = Request.Parse(input);
        Assert.Multiple(() =>
        {
            Assert.That(parsedRequest, Is.InstanceOf<Request.List>());
            Assert.That(((Request.List)parsedRequest).Path, Is.EqualTo("/path/to/list\n"));
        });
    }

    [Test]
    public void ParseGetRequest()
    {
        const string input = "2 /path/to/get\n";
        var parsedRequest = Request.Parse(input);
        Assert.Multiple(() =>
        {
            Assert.That(parsedRequest, Is.InstanceOf<Request.Get>());
            Assert.That(((Request.Get)parsedRequest).Path, Is.EqualTo("/path/to/get\n"));
        });
    }

    [Test]
    public void ParseInvalidRequest()
    {
        const string input = "InvalidRequest\n";
        var parsedRequest = Request.Parse(input);
        Assert.That(parsedRequest, Is.SameAs(Request.Unknown.Instance));
    }


    [Test]
    public void ListResponseToStringWithEntries()
    {
        var listEntries = new List<ListEntry>
        {
            new("file1.txt", true),
            new("file2.txt", false)
        };
        var response = new Response.List(listEntries, true);

        const string expected = "2 file1.txt True file2.txt False\n";
        Assert.That(response.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void ListResponseToStringWithoutEntries()
    {
        var listEntries = new List<ListEntry>();
        var response = new Response.List(listEntries, false);

        const string expected = "-1\n";
        Assert.That(response.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void ListResponseParseValidResponse()
    {
        const string responseString = "2 file1.txt True file2.txt False\n";
        var parsedResponse = Response.List.Parse(responseString);

        Assert.That(parsedResponse.IsQueryCorrect, Is.True);
        CollectionAssert.AreEqual(new ListEntry[]
        {
            new("file1.txt", true),
            new("file2.txt", false)
        }, parsedResponse.ListEntries);
    }

    [Test]
    public void GetResponseParseValidResponse()
    {
        var responseBytes = new List<byte> { 65, 66, 67, 68, 69 };
        var parsedResponse = Response.Get.Parse(5, responseBytes);

        Assert.That(parsedResponse.IsQueryCorrect, Is.True);
        CollectionAssert.AreEqual("ABCDE"u8.ToArray(), parsedResponse.Bytes);
    }

    [Test]
    public void GetResponseParseInvalidResponse()
    {
        var parsedResponse = Response.Get.Parse(-1, new List<byte>());
        Assert.Multiple(() =>
        {
            Assert.That(parsedResponse.IsQueryCorrect, Is.False);
            Assert.That(parsedResponse.Bytes, Is.Empty);
        });
    }

    [Test]
    public void NoneResponseToString()
    {
        var response = Response.None.Instance;

        const string expected = "None Response\n";
        Assert.That(response.ToString(), Is.EqualTo(expected));
    }
}