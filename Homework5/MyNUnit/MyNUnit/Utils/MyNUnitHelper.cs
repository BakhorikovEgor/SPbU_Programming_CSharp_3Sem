using System.Reflection;
using MyNUnit.Models;

namespace MyNUnit.Utils;

public static class MyNUnitHelper
{
    public static async Task<IEnumerable<TestClassReportModel>> RunTestsAsync(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            throw new ArgumentException("No such file or directory !");
        }

        var assemblies = _collectAllAssembliesByPath(path);
        var testClassModels = await _collectTestClassModelsByAssembliesAsync(assemblies.ToArray());

        return await _runTestsByTestClassModelsAsync(testClassModels.ToArray());
    }

    private static IEnumerable<Assembly> _collectAllAssembliesByPath(string path)
        => File.Exists(path)
            ? Path.GetExtension(path).Equals(".dll")
                ? new[] { Assembly.LoadFrom(path) }
                : Array.Empty<Assembly>()
            : Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom);


    private static async Task<IEnumerable<TestClassModel>> _collectTestClassModelsByAssembliesAsync(
        Assembly[] assemblies)
    {
        var classTypes = new List<Type>();
        foreach (var assembly in assemblies)
        {
            classTypes.AddRange(assembly.GetTypes().Where(type => type.IsClass).ToArray());
        }

        var result = new TestClassModel[classTypes.Count()];
        var tasks = new Task[classTypes.Count()];
        for (var i = 0; i < result.Length; ++i)
        {
            var locI = i;
            tasks[i] = Task.Run(() => { result[locI] = TestClassModel.GenerateFromClass(classTypes[locI]); });
        }

        await Task.WhenAll(tasks);
        return result;
    }

    private static async Task<IEnumerable<TestClassReportModel>> _runTestsByTestClassModelsAsync(
        TestClassModel[] testClassModels)
    {
        var result = new TestClassReportModel[testClassModels.Length];
        var tasks = new Task[testClassModels.Length];
        for (var i = 0; i < testClassModels.Length; ++i)
        {
            var locI = i;
            tasks[i] = testClassModels[i].RunTestsAsync().ContinueWith(reportTask =>
            {
                result[locI] = reportTask.Result;
            });
        }

        await Task.WhenAll(tasks);
        return result;
    }
}