namespace Chat;

/// <summary>
/// Class for making requests.
/// </summary>
public class Request
{
    /// <summary>
    /// Request of writting.
    /// </summary>
    /// <param name="writer">StreamWriter.</param>
    /// <param name="token">Cancelation tiken.</param>
    /// <returns>Task.</returns>
    public static async Task WriteMessage(StreamWriter writer, CancellationToken token)
    {
        string? line;
        while ((line = Console.ReadLine()) != "exit" && !token.IsCancellationRequested)
        {
            await writer.WriteAsync(line);
        }

        Console.WriteLine("You was disconnected.");
    }

    /// <summary>
    /// Request of reading.
    /// </summary>
    /// <param name="reader">StreamReader.</param>
    /// <param name="token">Cancelation tiken.</param>
    /// <returns>Task.</returns>
    public static async Task ReadMeesage(StreamReader reader, CancellationToken token)
    {
        string? line;
        while ((line = await reader.ReadLineAsync(token)) != "exit")
        {
            Console.WriteLine(line);
        }

        Console.WriteLine("Interlocutor was disconnected.");
    }
}