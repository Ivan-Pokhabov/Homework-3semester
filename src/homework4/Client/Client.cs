using System.Net;
using System.Net.Sockets;

namespace homework4;

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
    private readonly IPEndPoint endPoint = endPoint;

    /// <summary>
    /// Lists the files at the specified path on the FTP server.
    /// </summary>
    /// <param name="path">The path on the server to list files from.</param>
    /// <returns>A string representing the response from the server.</returns>
    public async Task<string?> List(string path)
        => await Request($"1 {path}");

    /// <summary>
    /// Get a file from the specified path on the server.
    /// </summary>
    /// <param name="path">The path on the server to retrieve the file from.</param>
    /// <returns>A string representing the size of the file retrieved from the server.</returns>
    public async Task<string?> Get(string path)
        => await Request($"2 {path}");

    private async Task<string> Request(string request)
    {
        using var client = new TcpClient();
        await client.ConnectAsync(endPoint);

        using var stream = client.GetStream();
        using var writer = new StreamWriter(stream) { AutoFlush = true };
        using var reader = new StreamReader(stream);

        await writer.WriteLineAsync(request);
        var response = await reader.ReadToEndAsync();

        return response;
    }
}