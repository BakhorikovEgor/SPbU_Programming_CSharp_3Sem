using System.Text;

namespace SimpleFTP.Utils;

public static class RequestHandlers
{
    private const int LackOfFile = -1;

    public static async Task ListFilesAsync(string path, Stream stream)
    {
        if (!Directory.Exists(path))
        {
            await SendStringAsync(LackOfFile.ToString(), stream);
            return;
        }

        var innerPaths = Directory.EnumerateFileSystemEntries(path).ToArray();
        var builder = new StringBuilder(innerPaths.Length.ToString());

        foreach (var innerPath in innerPaths)
        {
            builder.Append($" {Path.GetFileName(path)} {Directory.Exists(path)}");
        }

        builder.Append('\n');

        await SendStringAsync(builder.ToString(), stream);
    }

    public static async Task GetFileAsync(string path, Stream stream)
    {
        if (!File.Exists(path))
        {
            await SendStringAsync(LackOfFile.ToString(), stream);
        }

        var bytes = await File.ReadAllBytesAsync(path);
        await SendStringAsync(bytes.Length.ToString(), stream);
        await SendBytesAsync(bytes, stream);
    }

    public static async Task SendStringAsync(string message, Stream stream)
    {
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(message);
        await writer.FlushAsync();
    }

    public static async Task SendBytesAsync(byte[] bytes, Stream stream)
    {
        await stream.WriteAsync(bytes);
        await stream.WriteAsync("\n"u8.ToArray());
        await stream.FlushAsync();
    }
}