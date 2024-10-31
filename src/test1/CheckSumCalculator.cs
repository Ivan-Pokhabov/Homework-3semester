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
    /// Gets result of calculation.
    /// </summary>
    public static byte[] GetResult => [.. Result];

    /// <summary>
    /// Clear result of calculating.
    /// </summary>
    public static void Clear()
    {
        Result.Clear();
    }

    /// <summary>
    /// Calculate check-sum with 1 thread.
    /// </summary>
    /// <param name="path">Path to entry.</param>
    public static void Calculate(string path)
    {
        if (!Directory.Exists(path) && !File.Exists(path))
        {
            throw new ArgumentException("Invalid path");
        }

        FileAttributes attribute = File.GetAttributes(path);
        if (attribute.HasFlag(FileAttributes.Directory))
        {
            var entries = Directory.GetFileSystemEntries(path).Order();
            var directory = new DirectoryInfo(path);
            var directoryNameHash = MD5.HashData(Encoding.UTF32.GetBytes(directory.Name));
            Result.AddRange(directoryNameHash);

            foreach (var entry in entries)
            {
                Calculate(entry);
            }

            return;
        }

        using var fileContent = File.Open(path, FileMode.Open);
        var fileHash = MD5.HashData(fileContent);
        Result.AddRange(fileHash);
    }

    /// <summary>
    /// Calculate check-sum using multithreading.
    /// </summary>
    /// <param name="path">Path to entry.</param>
    public static void MultithreadingCalculate(string path)
    {
        if (!Directory.Exists(path) && !File.Exists(path))
        {
            throw new ArgumentException("Invalid path");
        }

        FileAttributes attribute = File.GetAttributes(path);
        if (attribute.HasFlag(FileAttributes.Directory))
        {
            var entries = Directory.GetFileSystemEntries(path).Order();
            var directory = new DirectoryInfo(path);
            var directoryNameHash = MD5.HashData(Encoding.UTF32.GetBytes(directory.Name));
            Result.AddRange(directoryNameHash);

            foreach (var entry in entries.AsParallel().AsOrdered())
            {
                Calculate(entry);
            }

            return;
        }

        using var fileContent = File.Open(path, FileMode.Open);
        var fileHash = MD5.HashData(fileContent);
        Result.AddRange(fileHash);
    }
}