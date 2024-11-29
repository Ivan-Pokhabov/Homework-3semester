namespace homework5.TestRunner;

using System.Diagnostics;
using System.Reflection;
using homework5.Attributes;

public record MyTestRunner(Type classType, Dictionary<Type, MethodInfo[]> methods)
{
    /// <summary>
    /// Make instance of MyTestRunner by type of test class.
    /// </summary>
    /// <param name="classType">Type of test class.</param>
    /// <returns>MyTestRunner instance.</returns>
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

    /// <summary>
    /// Run test from class asynchronously.
    /// </summary>
    /// <returns>Task with report in record TestClassResullt.</returns>
    /// <exception cref="InvalidDataException">Methods with BeforeClassAttribute or AfterClassAttribute should be static.</exception>
    /// <exception cref="InvalidOperationException">Something goes wrong and creating an instance of the class is failed.</exception>
    public async Task<TestClassResult> RunTestsAsync()
    {
        if (!CheckBeforeAndAfterClassAttributesForStatic())
        {
            throw new InvalidDataException("Methods with BeforeClassAttribute or AfterClassAttribute should be static.");
        }

        var obj = Activator.CreateInstance(classType) ?? throw new InvalidOperationException("Failed to create an instance of the class.");

        if (!methods.ContainsKey(typeof(TestAttribute)))
        {
            return new TestClassResult(classType, []);
        }

        await TestMethodsByAttribute(typeof(BeforeClassAttribute), obj);

        var testReports = new TestResult[methods[typeof(TestAttribute)].Length];
        var testTasks = new Task[methods[typeof(TestAttribute)].Length];
        for (int i = 0; i < methods[typeof(TestAttribute)].Length; ++i)
        {
            var testMethod = methods[typeof(TestAttribute)][i];
            testTasks[i] = RunTestAsync(testMethod, obj, i, testReports);
        }

        await Task.WhenAll(testTasks);
        await TestMethodsByAttribute(typeof(AfterClassAttribute), obj);

        return new TestClassResult(classType, testReports);
    }

    private bool CheckBeforeAndAfterClassAttributesForStatic()
    {
        var check = true;

        foreach (var attribute in new Type[] { typeof(BeforeClassAttribute), typeof(AfterClassAttribute) })
        {
            if (!methods.TryGetValue(attribute, out MethodInfo[]? value))
            {
                continue;
            }

            Parallel.ForEach(methods[attribute], (method) =>
            {
                if (!method.IsStatic)
                {
                    check = false;
                }
            });
        }

        return check;
    }

    private async Task RunTestAsync(MethodInfo testMethod, object obj, int index, TestResult[] testReports)
    {
        await TestMethodsByAttribute(typeof(BeforeAttribute), obj);
        var stopwatch = new Stopwatch();
        var attribute = (TestAttribute)Attribute.GetCustomAttribute(testMethod, typeof(TestAttribute)) !;
        if (attribute.IgnoreMessage is not null)
        {
            testReports[index] = new TestResult(testMethod.Name, null, 0, attribute.IgnoreMessage);
            await TestMethodsByAttribute(typeof(AfterAttribute), obj);
            return;
        }

        stopwatch.Start();
        try
        {
            testMethod.Invoke(obj, null);
            stopwatch.Stop();

            testReports[index] = new TestResult(testMethod.Name, null, stopwatch.ElapsedMilliseconds, null);
        }
        catch (Exception e)
        {
            stopwatch.Stop();

            testReports[index] = (attribute.ExpectedException == e.GetBaseException().GetType()) ?
            new TestResult(testMethod.Name, null, stopwatch.ElapsedMilliseconds, null)
            : new TestResult(testMethod.Name, e.GetBaseException().GetType(), stopwatch.ElapsedMilliseconds, null);
        }
        finally
        {
            await TestMethodsByAttribute(typeof(AfterAttribute), obj);
        }
    }

    private async Task TestMethodsByAttribute(Type attribute, object? obj)
    {
        if (!methods.TryGetValue(attribute, out MethodInfo[]? value))
        {
            return;
        }

        var tasks = value.Select(method => Task.Run(() => method.Invoke(obj, null)))
            .ToArray();

        await Task.WhenAll(tasks);
    }
}

public record TestResult(string methodName, Type? testException, long time, string? ignoreMessage);

public record TestClassResult(Type classType, TestResult[] results);