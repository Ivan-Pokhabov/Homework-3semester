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
    var result = await homework5.Utils.MyNUnitUtils.RunTestsAsync(path);
    Parallel.ForEach(result, (reports) =>
    {
        Console.WriteLine($"Results of test class {reports.classType}:");
        Parallel.ForEach(reports.results, (report) =>
        {
            var methodSucces = report.isSuccess ? "passed" : "failed";
            var ignoreMessage = report.ignoreMessage ?? "test was not ignored";
            Console.WriteLine($"Method {report.methodName} was {methodSucces} with time {report.time}, ignore meaasge : {ignoreMessage}");
        });
    });
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
    return;
}