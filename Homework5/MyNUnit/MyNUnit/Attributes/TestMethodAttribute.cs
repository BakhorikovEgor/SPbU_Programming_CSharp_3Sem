namespace MyNUnit.Attributes;

public abstract class TestMethodAttribute : Attribute
{
    
}

public abstract class StaticTestMethodAttribute : TestMethodAttribute
{
    
}

public abstract class NonStaticTestMethodAttribute : TestMethodAttribute
{
    
}