namespace test1;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Class that calculate check-sum with MD5 hash for system entries.
/// </summary>
public static class CheckSumCalculator
{
    private static readonly List<byte> Result = [];

    /// <summary>
    /// Calculate check-sum with 1 thread.
    /// </summary>
    /// <param name="path">Path to entry.</param>
    /// <returns>MD5 hash.</returns>
    public static byte[] Calculate(string path)
    {
        FileAttributes attribute = File.GetAttributes(path);
        if (attribute.HasFlag(FileAttributes.Directory))
        {
            var entries = Directory.GetFileSystemEntries(path).Order();
            var directory = new DirectoryInfo(path);
            var directoryNameHash = MD5.HashData(Encoding.UTF32.GetBytes(directory.Name));
            Result.AddRange(directoryNameHash);

            foreach (var entry in entries)
            {
                Result.AddRange(Calculate(entry));
            }

            return [.. Result];
        }

        using var fileContent = File.Open(path, FileMode.Open);
        var fileHash = MD5.HashData(fileContent);
        Result.AddRange(fileHash);

        return [.. Result];
    }

    /// <summary>
    /// Calculate check-sum using multithreading.
    /// </summary>
    /// <param name="path">Path to entry.</param>
    /// <returns>MD5 hash.</returns>
    public static byte[] MultithreadingCalculate(string path)
    {
        FileAttributes attribute = File.GetAttributes(path);
        if (attribute.HasFlag(FileAttributes.Directory))
        {
            var entries = Directory.GetFileSystemEntries(path).Order();
            var directory = new DirectoryInfo(path);
            var directoryNameHash = MD5.HashData(Encoding.UTF32.GetBytes(directory.Name));
            Result.AddRange(directoryNameHash);

            Parallel.ForEach(entries, entry =>
            {
                Result.AddRange(Calculate(entry));
            });
        }

        using var fileContent = File.Open(path, FileMode.Open);
        var fileHash = MD5.HashData(fileContent);
        Result.AddRange(fileHash);

        return [.. Result];
    }
}