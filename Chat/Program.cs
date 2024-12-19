using System.Net;
using System.Net.Sockets;
using Chat;

static void PrintHelp()
{
    Console.WriteLine("""
        This programme is designed to run a chat which,
        depending on the command line options, runs either as a client or as a server. 

        Usage:
        If only the port is specified, the application starts as a server and listens on the port:
        -------------------------------------------
        dotnet run <port>
        -------------------------------------------

        If an IP address and port are specified,
        the application starts as a client and connects to the specified server:
        -------------------------------------------
        dotnet run <ip> <port>
        -------------------------------------------

        When someone types "exit" into the console, the connection is closed and the applications are terminated.
        """);
}

switch (args.Length)
{
    case 1:
        if (args[0] == "help")
        {
            PrintHelp();
            break;
        }

        if (!int.TryParse(args[0], out var port) || port < 0 || port > 65535)
        {
            Console.WriteLine("Incorrect port value.");
            break;
        }

        var server = new Server(port);
        try
        {
            await server.RunAsync();
        }
        catch (SocketException)
        {
            Console.WriteLine("Client connection error.");
        }

        break;

    case 2:
        if (!IPAddress.TryParse(args[0], out var ip))
        {
            Console.WriteLine("Incorrect IP-adress.");
            break;
        }

        if (!int.TryParse(args[1], out port) || port < 0 || port > 65535)
        {
            Console.WriteLine("Incorrect port value.");
            break;
        }

        var client = new Client(new IPEndPoint(ip, port));

        try
        {
            await client.RunAsync();
        }
        catch (SocketException)
        {
            Console.WriteLine("Connection to server error.");
        }

        break;

    default:
        PrintHelp();
        break;
}