namespace Chat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Server for ftp requests.
/// </summary>
/// <param name="port">Number of port of client.</param>
public class Server(int port) : IDisposable
{
    private readonly CancellationTokenSource cts = new ();

    private bool isServerStop = false;

    /// <summary>
    /// Run server.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task RunAsync()
    {
        if (isServerStop)
        {
            Console.WriteLine("Server was already stoped. Please make new.");
            return;
        }

        using var tcpListener = new TcpListener(IPAddress.Any, port);

        Console.WriteLine("Server started.");

        tcpListener.Start();

        var client = await tcpListener.AcceptTcpClientAsync(cts.Token);

        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        await Task.WhenAny(Request.ReadMeesage(reader, cts.Token), Request.WriteMessage(writer, cts.Token));

        Stop();
    }

    /// <summary>
    /// Stops the server and stops listening for client connections.
    /// </summary>
    public void Stop()
    {
        isServerStop = true;
        cts.Cancel();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        cts.Dispose();
    }
}