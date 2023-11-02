namespace Protocol;

public abstract record Request
{
    public sealed record List(string Path) : Request
    {
        public override string ToString()
            => $"1 {Path}\n";
    }

    public sealed record Get(string Path) : Request
    {
        public override string ToString()
            => $"2 {Path}\n";
    }
    
    public static Request Parse(string request)
    {
        if (request.Length <= 2)
        {
            throw new ArgumentException("Wrong request format");
        }

        return request[0] switch
        {
            '1' => new List(request[2..]),
            '2' => new Get(request[2..]),
            _ => throw new ArgumentException("Wrong request type.")
        };
    }
    
    public abstract override string ToString();

    private Request() {}
}

