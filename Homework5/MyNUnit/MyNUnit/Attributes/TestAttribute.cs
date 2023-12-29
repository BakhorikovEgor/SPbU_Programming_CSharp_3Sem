namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : NonStaticTestMethodAttribute
{
    public Type? Expected { get; set; }
    public string? Ignore { get; set; }

    public TestAttribute()
    {
    }
    
}