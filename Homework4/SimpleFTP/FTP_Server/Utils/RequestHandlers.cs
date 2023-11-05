using Protocol;
using Protocol.Models;

namespace FTP_Server.Utils;

/// <summary>
/// The RequestHandlers class provides utility methods for handling FTP client requests.
/// </summary>
public static class RequestHandlers
{
    /// <summary>
    /// Asynchronously sends a "List" response to the client based on the specified request.
    /// </summary>
    /// <param name="request">The "List" request containing the path to list.</param>
    /// <param name="stream">The stream to send the response to.</param>
    public static async Task SendListResponseAsync(Request.List request, Stream stream)
    {
        if (!Directory.Exists(request.Path))
        {
            await SendResponseAsync(new Response.List(new List<ListEntry>(), false), stream);
            return;
        }

        var innerPaths = Directory.EnumerateFileSystemEntries(request.Path).ToArray();
        var listEntries = innerPaths.Select(innerPath
            => new ListEntry(Path.GetFileName(innerPath), Directory.Exists(innerPath))).ToList();

        await SendResponseAsync(new Response.List(listEntries, true), stream);
    }

    /// <summary>
    /// Asynchronously sends a "Get" response to the client based on the specified request.
    /// </summary>
    /// <param name="request">The "Get" request containing the path to retrieve.</param>
    /// <param name="stream">The stream to send the response to.</param>
    public static async Task SendGetResponseAsync(Request.Get request, Stream stream)
    {
        if (!File.Exists(request.Path))
        {
            await SendResponseAsync(new Response.Get(new List<byte>(), false), stream);
            return;
        }

        var bytes = await File.ReadAllBytesAsync(request.Path);
        await SendResponseAsync(new Response.Get(bytes.ToList(), true), stream);
    }

    /// <summary>
    /// Asynchronously sends a "None" response to the client based on the specified request.
    /// </summary>
    /// <param name="request">The "None" request.</param>
    /// <param name="stream">The stream to send the response to.</param>
    public static async Task SendNoneResponseAsync(Request.Unknown request, Stream stream)
        => await SendResponseAsync(Response.None.Instance, stream);

    
    private static async Task SendResponseAsync(Response response, Stream stream)
    {
        var writer = new StreamWriter(stream);
        switch (response)
        {
            case Response.List list:
            {
                await writer.WriteAsync(list.ToString());
                break;
            }
            case Response.Get get:
            {
                await stream.WriteAsync(BitConverter.GetBytes(get.Bytes.Count > 0
                    ? (long)get.Bytes.Count
                    : -1L));
                await stream.WriteAsync(get.Bytes.ToArray());
                break;
            }
            case Response.None none:
            {
                await writer.WriteAsync(none.ToString());
                break;
            }
        }

        await writer.FlushAsync();
        await stream.FlushAsync();
    }
}