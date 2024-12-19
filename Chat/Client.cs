namespace Chat;

using System.Net;
using System.Net.Sockets;

/// <summary>
/// Represents a client for a simple-FTP protocol connection.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="Client"/> class.
/// </remarks>
/// <param name="ip">The IP address of the server.</param>
/// <param name="port">The port number of the server.</param>
public class Client(IPEndPoint endPoint)
{
    private static readonly CancellationTokenSource Cts = new ();

    private readonly IPEndPoint endPoint = endPoint;

    /// <summary>
    /// Run Client.
    /// </summary>
    /// <returns>Task.</returns>
    public async Task RunAsync()
    {
        using var client = new TcpClient();
        await client.ConnectAsync(endPoint);

        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        await Task.WhenAny(Request.ReadMeesage(reader, Cts.Token), Request.WriteMessage(writer, Cts.Token));
        client.Close();
    }
}