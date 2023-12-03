using System.Reflection;
using MyNUnit.Models;

namespace MyNUnit.Utils;

public static class MyNUnitHelper
{
    public static async Task RunTests(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            throw new ArgumentException("No such file or directory !");
        }

        var assemblies = _collectAllAssembliesByPath(path);
    }

    private static IEnumerable<Assembly> _collectAllAssembliesByPath(string path)
        => File.Exists(path)
            ? Path.GetExtension(path).Equals(".dll")
                ? new[] { Assembly.LoadFrom(path) }
                : Array.Empty<Assembly>()
            : Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom);


    private static async Task<IEnumerable<TestClassModel>> GetTestClassesFromAssembly(Assembly assembly)
    {
        var models = new List<TestClassModel>();
        var classes = assembly.GetTypes().Where(t => t.IsClass);

        foreach (var @class in classes)
        {
        }

        return null;
    }

    private static void GetTestClassByClass(Type classType)
    {
        var afterClassMethods = new List<MethodInfo>();
        var beforeClassMethods = new List<MethodInfo>();
        var afterMethods = new List<MethodInfo>();
        var beforeMethods = new List<MethodInfo>();
        var testMethods = new List<MethodInfo>();

        foreach (var method in classType.GetMethods())
        {
        }
    }
}