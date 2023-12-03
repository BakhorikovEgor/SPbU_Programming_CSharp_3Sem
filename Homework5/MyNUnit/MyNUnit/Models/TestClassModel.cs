using System.Reflection;

namespace MyNUnit.Models;

public record TestClassModel(MethodInfo[] BeforeClassMethods, MethodInfo[] AfterClassMethods,
    MethodInfo[] BeforeMethods, MethodInfo[] AfterMethods, MethodInfo[] TestMethods)
{
    
}