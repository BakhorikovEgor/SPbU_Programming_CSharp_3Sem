using System.Diagnostics;
using System.Reflection;
using MyNUnit.Attributes;

namespace MyNUnit.Models;

public record TestClassModel(Type ClassType,
    Dictionary<Type, MethodInfo[]> MethodCategories)
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
            .ToDictionary(group => group.Key, group => group.ToArray());

        return new TestClassModel(classType, methodCategories);
    }

    public async Task<TestClassReportModel> RunTestsAsync()
    {
        var obj = Activator.CreateInstance(ClassType);

        await _runMethodsByType(obj, typeof(BeforeClassAttribute));

        var testTasks = new Task[MethodCategories[typeof(TestMethodAttribute)].Length];
        var testReports = new TestReportModel[MethodCategories[typeof(TestMethodAttribute)].Length];
        for (var index = 0; index < MethodCategories[typeof(TestMethodAttribute)].Length; index++)
        {
            var locI = index;
            var testMethod = MethodCategories[typeof(TestMethodAttribute)][index];
            testTasks[index] = _runMethodsByType(obj, typeof(BeforeAttribute)).ContinueWith(async (_) =>
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    testMethod.Invoke(obj, null);
                    stopwatch.Stop();
                    testReports[locI] = new TestReportModel("sucess", stopwatch.ElapsedMilliseconds);
                }
                catch
                {
                    testReports[locI] = new TestReportModel("thrown exception", -1);
                }

                testMethod.Invoke(obj, null);
                await _runMethodsByType(obj, typeof(AfterAttribute));
            });
        }

        await Task.WhenAll(testTasks);
        await _runMethodsByType(obj, typeof(AfterClassAttribute));

        return new TestClassReportModel(ClassType, testReports);
    }

    private async Task _runMethodsByType(Object? obj, Type type)
    {
        var beforeClassTasks = new Task[MethodCategories[type].Length];
        for (var index = 0; index < MethodCategories[type].Length; index++)
        {
            var beforeClassMethod = MethodCategories[type][index];
            beforeClassTasks[index] = Task.Run(() => { beforeClassMethod.Invoke(obj, null); });
        }

        await Task.WhenAll(beforeClassTasks);
    }
}