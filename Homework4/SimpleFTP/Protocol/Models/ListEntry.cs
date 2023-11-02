namespace Protocol.Models;

public record ListEntry(string Name, bool IsDirectory)
{
    public override string ToString()
        => $"{Name} {IsDirectory}";
}