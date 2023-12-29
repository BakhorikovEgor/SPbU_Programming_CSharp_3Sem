using Protocol.Models;

namespace Protocol;

/// <summary>
///     The Response class defines abstract response types for interactions with an FTP client.
/// </summary>
public abstract record Response
{
    /// <summary>
    ///     Initializes a new instance of the Response class.
    /// </summary>
    private Response()
    {
    }

    /// <summary>
    ///     Represents a "List" response containing a list of entries.
    /// </summary>
    public record List(List<ListEntry> ListEntries, bool IsQueryCorrect) : Response
    {
        /// <summary>
        ///     Parses a string response into a List response with a list of entries.
        /// </summary>
        /// <param name="response">The string representation of the response.</param>
        /// <returns>A List response with the list of entries and an indication of correctness.</returns>
        public static List Parse(string? response)
        {
            var listEntries = new List<ListEntry>();
            if (response is null || _parseSize(response) == -1) return new List(listEntries, false);

            var parts = response.Split();
            for (var i = 1; i < parts.Length - 1; i += 2)
                listEntries.Add(new ListEntry(parts[i], bool.Parse(parts[i + 1])));

            return new List(listEntries, true);
        }

        /// <inheritdoc />
        public override string ToString() =>  ListEntries.Count == 0
            ? "-1\n"
            : $"{ListEntries.Count} {string.Join(' ', ListEntries)}\n";


        private static long _parseSize(string response) => response[0] == '-'
            ? -1
            : response.TakeWhile(t => t != ' ')
                .Aggregate(0, (current, t) => current * 10 + (t - '0'));    }

    /// <summary>
    ///     Represents a "Get" response containing a list of bytes.
    /// </summary>
    public record Get(List<byte> Bytes, bool IsQueryCorrect) : Response
    {
        /// <summary>
        ///     Parses a Get response based on size and byte data.
        /// </summary>
        /// <param name="size">The size of the response in bytes.</param>
        /// <param name="responseBytes">The byte data of the response.</param>
        /// <returns>A Get response with byte data and an indication of correctness.</returns>
        public static Get Parse(long size, List<byte> responseBytes) => size == -1
            ? new Get(responseBytes, false)
            : new Get(responseBytes, true);
    }

    /// <summary>
    ///     Represents a "None" response indicating an empty response.
    /// </summary>
    public sealed record None : Response
    {
        private static readonly Lazy<None> Lazy = new(() => new None());

        private None()
        {
        }

        /// <summary>
        ///     Gets the singleton instance of the None response.
        /// </summary>
        public static None Instance => Lazy.Value;


        /// <inheritdoc />
        public override string ToString() => "None Response\n";
    }
}