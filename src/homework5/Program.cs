using homework5.Utils;

static void PrintHelp()
{
    Console.WriteLine("""

            This is tool for run your C# test program

            Usage: dotnet run {path}

            """);
}

if (args.Length != 1)
{
    PrintHelp();
    return;
}

var path = args[0];

try
{
    var result = await MyNUnitUtils.RunTestsAsync(path);
    Parallel.ForEach(result, (reports) =>
    {
        Console.WriteLine($"Results of test class {reports.ClassType}:");
        Parallel.ForEach(reports.Results, (report) =>
        {
            var methodSucces = report.IsSuccess ? "succes" : "failed";
            Console.WriteLine($"Method {report.MethodName} was {methodSucces} with time {report.Time}");
        });
    });
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
    return;
}