using System.Net;
using homework4;

static void PrintHelp()
{
    Console.WriteLine("""

        This is the client that support SimpleFTP protocol.

        There are 2 requests:
        List - "1 <path: String>\n"
        Get - "2 <path: String>\n"

        Usage: dotnet run {ip} {port}

        """);
}

if (args.Length != 2)
{
    PrintHelp();
    return;
}

if (!IPAddress.TryParse(args[0], out var ip) || !int.TryParse(args[1], out var port)
|| port < 0 || port > 65536)
{
    PrintHelp();
    return;
}

var endPoint = new IPEndPoint(ip, port);
var client = new Client(endPoint);

var line = Console.ReadLine();

while (line != string.Empty)
{
    switch (line!.Split().First())
    {
    case "1" when line.Split().Length == 2:
        Console.WriteLine(await client.List(line[2..]));
        break;
    case "2" when line.Split().Length == 2:
        Console.WriteLine(await client.Get(line[2..]));
        break;
    default:
        PrintHelp();
        return;
    }

    line = Console.ReadLine();
}
