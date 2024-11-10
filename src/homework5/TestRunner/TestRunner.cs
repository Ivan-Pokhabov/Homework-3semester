using System.Diagnostics;
using System.Reflection;
using homework5.Attributes;

namespace homework5.TestRunner;

public record MyTestRunner(Type ClassType, Dictionary<Type, MethodInfo[]> Methods)
{
    public static MyTestRunner MakeTestByClassType(Type classType)
    {
        var methods = classType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);

        var methodTypes = methods
            .Select(method => (Method: method, Attributes: method.GetCustomAttributes<MyNUnitAttribute>(inherit: true).ToArray()))
            .Where(tuple => tuple.Attributes.Length == 1 && tuple.Method.GetParameters().Length == 0)
            .GroupBy(tuple => tuple.Attributes[0].GetType(), tuple => tuple.Method)
            .ToDictionary(group => group.Key, group => group.ToArray());

        return new MyTestRunner(classType, methodTypes);
    }

    public async Task<TestClassResult> RunTestsAsync()
    {
        if (!CheckBeforeAndAfterClassAttributesForStatic())
        {
            throw new InvalidDataException("Methods with BeforeClassAttribute or AfterClassAttribute should be static.");
        }

        var obj = Activator.CreateInstance(ClassType) ?? throw new InvalidOperationException("Failed to create an instance of the class.");

        if (!Methods.ContainsKey(typeof(TestAttribute)))
        {
            return new TestClassResult(ClassType, []);
        }

        await TestMethodsByAttribute(typeof(BeforeClassAttribute), obj);

        var testReports = new TestResult[Methods[typeof(TestAttribute)].Length];
        var testTasks = new Task[Methods[typeof(TestAttribute)].Length];
        for (int i = 0; i < Methods[typeof(TestAttribute)].Length; ++i)
        {
            var testMethod = Methods[typeof(TestAttribute)][i];
            testTasks[i] = RunTestAsync(testMethod, obj, i, testReports);
        }

        await Task.WhenAll(testTasks);
        await TestMethodsByAttribute(typeof(AfterClassAttribute), obj);

        return new TestClassResult(ClassType, testReports);
    }

    private bool CheckBeforeAndAfterClassAttributesForStatic()
    {
        var check = true;

        foreach (var attribute in new Type[] {typeof(BeforeClassAttribute), typeof(AfterClassAttribute)})
        {
            if (!Methods.TryGetValue(attribute, out MethodInfo[]? value))
            {
                continue;
            }

            Parallel.ForEach(Methods[attribute], (method) => {if (!method.IsStatic) check = false; });
        }

        return check;
    }

    private async Task RunTestAsync(MethodInfo testMethod, object obj, int index, TestResult[] testReports)
    {
        await TestMethodsByAttribute(typeof(BeforeAttribute), obj);
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        try
        {
            testMethod.Invoke(obj, null);
            stopwatch.Stop();

            testReports[index] = new TestResult(testMethod.Name, true, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception e)
        {
            stopwatch.Stop();
            var attribute = (TestAttribute) Attribute.GetCustomAttribute(testMethod, typeof(TestAttribute))!;
            testReports[index] = (attribute.ExpectedException == e.GetBaseException().GetType()) ?
            new TestResult(testMethod.Name, true, stopwatch.ElapsedMilliseconds)
            : new TestResult(testMethod.Name, false, -1);
        }
        finally
        {
            await TestMethodsByAttribute(typeof(AfterAttribute), obj);
        }
    }


    private async Task TestMethodsByAttribute(Type attribute, object? obj)
    {
        if (!Methods.TryGetValue(attribute, out MethodInfo[]? value))
        {
            return;
        }

        var tasks = value.Select(method => Task.Run(() => method.Invoke(obj, null)))
            .ToArray();

        await Task.WhenAll(tasks);
    }
}

public record TestResult(string MethodName, bool IsSuccess, long Time);

public record TestClassResult(Type ClassType, TestResult[] Results);