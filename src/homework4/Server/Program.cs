using homework4;

static void PrintHelp()
{
    Console.WriteLine("""

        This is the server that supports SimpleFTP protocol.
        Usage: dotnet run {port}

        """);
}

if (args.Length != 1)
{
    PrintHelp();
    return;
}

if (!int.TryParse(args[0], out var port) || port < 0 || port > 65536)
{
    PrintHelp();
    return;
}

var server = new Server(port);
await server.Run();
