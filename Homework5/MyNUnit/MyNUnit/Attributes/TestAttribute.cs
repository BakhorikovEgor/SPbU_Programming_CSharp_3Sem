namespace MyNUnit.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class TestAttribute : NonStaticTestMethodAttribute
{
}