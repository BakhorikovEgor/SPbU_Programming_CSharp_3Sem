namespace Protocol.Models;

/// <summary>
///     The ListEntry record represents an entry in a list of files and directories.
/// </summary>
public record ListEntry(string Name, bool IsDirectory)
{
    /// <summary>
    ///     Converts the ListEntry to a string representation in the format "Name IsDirectory".
    /// </summary>
    /// <returns>A string representation of the ListEntry.</returns>
    public override string ToString() => $"{Name} {IsDirectory}";
}