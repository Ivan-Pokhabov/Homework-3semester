using System.Net;
using System.Net.Sockets;
using System.Text;

namespace homework4;

/// <summary>
/// Server for ftp requests.
/// </summary>
/// <param name="port">Number of port of client.</param>
public class Server(int port) : IDisposable
{
    private readonly TcpListener tcpListener = new (IPAddress.Any, port);

    private readonly CancellationTokenSource cts = new ();

    private bool isServerStop = false;

    /// <summary>
    /// Run server.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public async Task Run()
    {
        if (isServerStop)
        {
            Console.WriteLine("Server was already stoped. Please make new.");
            return;
        }

        Console.WriteLine("Server started.");

        var tasks = new List<Task>();
        tcpListener.Start();

        while (!cts.IsCancellationRequested)
        {
            var client = await tcpListener.AcceptTcpClientAsync(cts.Token);

            tasks.Add(Task.Run(() => MakeRequest(client)));
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Stops the server and stops listening for client connections.
    /// </summary>
    public void Stop()
    {
        isServerStop = true;
        cts.Cancel();
        tcpListener.Stop();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        tcpListener.Dispose();
    }

    private static async Task MakeRequest(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream);
        using var writer = new StreamWriter(stream) { AutoFlush = true };

        var data = await reader.ReadLineAsync();
        if (data != null)
        {
            if (data[..2] == "1 ")
            {
                await List(data[2..], writer);
            }

            if (data[..2] == "2 ")
            {
                await Get(data[2..], writer);
            }
        }
    }

    private static async Task Get(string path, StreamWriter writer)
    {
        if (!File.Exists(path))
        {
            await writer.WriteAsync("-1");
            return;
        }

        var content = await File.ReadAllBytesAsync(path);
        await writer.WriteAsync($"{content.Length} {Encoding.UTF8.GetString(content)}");
    }

    private static async Task List(string path, StreamWriter writer)
    {
        if (!Directory.Exists(path))
        {
            await writer.WriteAsync("-1");
            return;
        }

        var fileSystemEntries = Directory.GetFileSystemEntries(path);
        Array.Sort(fileSystemEntries);
        var size = fileSystemEntries.Length;

        await writer.WriteAsync($"{size}");
        foreach (var entry in fileSystemEntries)
        {
            await writer.WriteAsync($" {entry} {Directory.Exists(entry)}");
        }

        await writer.WriteAsync("\n");
    }
}