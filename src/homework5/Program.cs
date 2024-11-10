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
    foreach (var reports in result)
    {
        Console.WriteLine($"Results of test class {reports.classType}:");
        foreach (var report in reports.results)
        {
            var methodSucces = report.isSuccess ? "passed" : "failed";
            var ignoreMessage = report.ignoreMessage ?? "test was not ignored";
            Console.ForegroundColor = report.isSuccess ? (report.ignoreMessage is null ? ConsoleColor.DarkGreen : ConsoleColor.Yellow) : ConsoleColor.Red;
            Console.WriteLine($"Method {report.methodName} was {methodSucces} with time {report.time}, ignore meaasge : {ignoreMessage}");
        }
    }
}
catch (ArgumentException e)
{
    Console.WriteLine(e.Message);
    return;
}