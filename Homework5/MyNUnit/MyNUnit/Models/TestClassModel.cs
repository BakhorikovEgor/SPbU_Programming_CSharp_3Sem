using System.Reflection;
using MyNUnit.Attributes;
using MyNUnit.Models;

public record TestClassModel(
    Dictionary<string, MethodInfo[]> MethodCategories)
{
    public static TestClassModel GenerateFromClass(Type classType)
    {
        var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        var methodCategories = methods
            .Select(method => (Method: method,
                Attributes: method.GetCustomAttributes<TestMethodAttribute>(inherit: true).ToArray()))
            .Where(tuple => tuple.Attributes.Length == 1 && tuple.Method.GetParameters().Length == 0 && (
                tuple.Attributes[0] is StaticTestMethodAttribute
                    ? tuple.Method.IsStatic
                    : !tuple.Method.IsStatic))
            .GroupBy(tuple => tuple.Attributes[0].GetType(), tuple => tuple.Method)
            .ToDictionary(group => group.Key.Name, group => group.ToArray());

        return new TestClassModel(methodCategories);
    }

    public async Task<TestClassReportModel> RunTestsAsync()
    {
        throw new NotImplementedException();
    }
}