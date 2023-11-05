namespace Protocol;

/// <summary>
/// The Request class defines abstract request types for interacting with an FTP server.
/// </summary>
public abstract record Request
{
    private const string UnknownString = "Unknown";

    /// <summary>
    /// Represents a "List" request for listing files and directories at a specified path.
    /// </summary>
    public sealed record List(string Path) : Request
    {
        /// <inheritdoc />
        public override string ToString()
            => $"1 {Path}\n";
    }

    /// <summary>
    /// Represents a "Get" request for retrieving the contents of a file at a specified path.
    /// </summary>
    public sealed record Get(string Path) : Request
    {
        /// <inheritdoc />
        public override string ToString()
            => $"2 {Path}\n";
    }

    /// <summary>
    /// Represents an "Unknown" request when the request type is not recognized.
    /// </summary>
    public sealed record Unknown : Request
    {
        private static readonly Lazy<Unknown> Lazy = new(() => new Unknown());

        /// <summary>
        /// Gets the singleton instance of the Unknown request.
        /// </summary>
        public static Unknown Instance => Lazy.Value;

        /// <inheritdoc />
        public override string ToString()
            => UnknownString + '\n';

        private Unknown()
        {
        }
    }

    /// <summary>
    /// Parses a string into the appropriate Request type.
    /// </summary>
    /// <param name="request">The string representation of the request.</param>
    /// <returns>An instance of a derived Request type based on the input string.</returns>
    public static Request Parse(string? request)
    {
        if (request is null || request.Length <= 2)
        {
            return Unknown.Instance;
        }

        if (request == UnknownString)
        {
            return Unknown.Instance;
        }

        return request[0] switch
        {
            '1' => new List(request[2..]),
            '2' => new Get(request[2..]),
            _ => Unknown.Instance
        };
    }

    /// <inheritdoc />
    public abstract override string ToString();

    private Request()
    {
    }
}