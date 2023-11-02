using Protocol.Models;

namespace Protocol;

public abstract record Response
{
    public abstract override string ToString();

    public record List(List<ListEntry> ListEntries) : Response
    {
        public static List Parse(string response)
        {
            var listEntries = new List<ListEntry>();
            
            var size = _parseSize(response);
            if (size == -1)
            {
                return new List(listEntries);
            }

            throw new NotImplementedException();

        }

        public override string ToString()
        {
            return ListEntries.Count == 0
                ? "-1"
                : $"{ListEntries.Count} {string.Join(' ', ListEntries)}\n";
        }
    }

    private Response() {}

    private static int _parseSize(string response)
    {
        if (response[0] == '-') return -1;
        
        return response.TakeWhile(t => t != ' ')
            .Aggregate(0, (current, t) => current * 10 + (t - '0'));
    }
}