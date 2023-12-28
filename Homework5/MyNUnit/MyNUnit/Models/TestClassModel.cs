using System.Reflection;
using MyNUnit.Attributes;

namespace MyNUnit.Models;

public record TestClassModel(MethodInfo[] BeforeClassMethods, MethodInfo[] AfterClassMethods,
    MethodInfo[] BeforeMethods, MethodInfo[] AfterMethods, MethodInfo[] TestMethods)
{
    public static TestClassModel Parse(Type classType)
    {
        var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic |
                                           BindingFlags.Instance | BindingFlags.Static);

        var afterClassMethods = new List<MethodInfo>();
        var beforeClassMethods = new List<MethodInfo>();
        var afterMethods = new List<MethodInfo>();
        var beforeMethods = new List<MethodInfo>();
        var testMethods = new List<MethodInfo>();
        foreach (var methodInfo in classType.GetMethods())
        {
            if (methodInfo.GetParameters().Length != 0)
            {
                throw new NotImplementedException();
            }

            var attributes = methodInfo.CustomAttributes
                .Where(attr => typeof(TestMethodAttribute).IsAssignableFrom(attr.AttributeType)).ToArray();

            if (attributes.Length != 1)
            {
                throw new NotImplementedException();
            }

            if (attributes[0])
                switch (attributes[0].AttributeType)
                {
                    case afterClassAttribute:
                    {
                        if (methodInfo.IsStatic)
                        {
                            beforeClassMethods.Add(methodInfo);
                        }

                        afterClassMethods.Add(methodInfo);
                        break;
                    }
                    case AfterAttribute:
                    {
                        afterMethods.Add(methodInfo);
                        break;
                    }
                    case BeforeClassAttribute:
                    {
                        if (methodInfo.IsStatic)
                        {
                            beforeClassMethods.Add(methodInfo);
                        }

                        break;
                    }
                    case BeforeAttribute:
                    {
                        beforeMethods.Add(methodInfo);
                        break;
                    }
                    case TestAttribute:
                    {
                        testMethods.Add(methodInfo);
                        break;
                    }
                }
        }


        return new TestClassModel(beforeClassMethods.ToArray(), afterClassMethods.ToArray(),
            beforeMethods.ToArray(), afterMethods.ToArray(), testMethods.ToArray());
    }
}