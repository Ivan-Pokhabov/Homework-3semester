namespace homework5.Utils;

using System.Reflection;
using homework5.TestRunner;

/// <summary>
/// Class for run all test files by path.
/// </summary>
public static class MyNUnitUtils
{
    /// <summary>
    /// Method for running all test dlls by path.
    /// </summary>
    /// <param name="path">Path to assemblies.</param>
    /// <returns>Task of test results.</returns>
    /// <exception cref="ArgumentException">Path should be correct.</exception>
    public static async Task<TestClassResult[]> RunTestsAsync(string path)
    {
        if (!File.Exists(path) && !Directory.Exists(path))
        {
            throw new ArgumentException("Invalid path.");
        }

        var assemblies = CollectAllAssemblysByPath(path);
        var tests = await CollectTestsFromAssemblies(assemblies);
        return await RunTestsFromClassesAsync(tests);
    }

    private static IEnumerable<Assembly> CollectAllAssemblysByPath(string path)
        => File.Exists(path)
        ? Path.GetExtension(path).Equals(".dll")
            ? [Assembly.LoadFrom(path)]
            : Array.Empty<Assembly>()
        : Directory.GetFiles(path, "*.dll").Select(Assembly.LoadFrom);

    private static async Task<MyTestRunner[]> CollectTestsFromAssemblies(IEnumerable<Assembly> assemblies)
    {
        var classTypes = new List<Type>();

        foreach (var assembly in assemblies)
        {
            classTypes.AddRange(assembly.GetTypes().Where(type => type.IsClass));
        }

        var result = new MyTestRunner[classTypes.Count];
        var tasks = classTypes.Select((type, index) => Task.Run(() =>
        {
            result[index] = MyTestRunner.MakeTestByClassType(type);
        })).ToArray();

        await Task.WhenAll(tasks);

        return result;
    }

    private static async Task<TestClassResult[]> RunTestsFromClassesAsync(MyTestRunner[] tests)
    {
        var result = new TestClassResult[tests.Length];
        var tasks = new Task[tests.Length];
        for (var i = 0; i < tests.Length; ++i)
        {
            var locI = i;
            tasks[i] = tests[i].RunTestsAsync().ContinueWith(reportTask =>
            {
                result[locI] = reportTask.Result;
            });
        }

        await Task.WhenAll(tasks);
        return result;
    }
}