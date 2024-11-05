namespace test1;

using System.Security.Cryptography;
using System.Text;

/// <summary>
/// Class that calculate check-sum with MD5 hash for system entries.
/// </summary>
public static class CheckSumCalculator
{
    /// <summary>
    /// Calculate check-sum with 1 thread.
    /// </summary>
    /// <param name="path">Path to entry.</param>
    /// <returns>MD5 hash.</returns>
    public static byte[] Calculate(string path)
    {
        if (!Directory.Exists(path) && !File.Exists(path))
        {
            throw new ArgumentException("Invalid path");
        }

        var result = new List<byte>();

        FileAttributes attribute = File.GetAttributes(path);
        if (attribute.HasFlag(FileAttributes.Directory))
        {
            var entries = Directory.GetFileSystemEntries(path).Order();
            var directory = new DirectoryInfo(path);
            var directoryNameHash = MD5.HashData(Encoding.UTF32.GetBytes(directory.Name));
            result.AddRange(directoryNameHash);

            foreach (var entry in entries)
            {
                result.AddRange(Calculate(entry));
            }

            return [.. result];
        }

        using var fileContent = File.Open(path, FileMode.Open);
        var fileHash = MD5.HashData(fileContent);
        result.AddRange(fileHash);
        return [.. result];
    }

    /// <summary>
    /// Calculate check-sum using multithreading.
    /// </summary>
    /// <param name="path">Path to entry.</param>
    /// <returns>MD5 hash.</returns>
    public static byte[] MultithreadingCalculate(string path)
    {
        if (!Directory.Exists(path) && !File.Exists(path))
        {
            throw new ArgumentException("Invalid path");
        }

        var result = new List<byte>();
        FileAttributes attribute = File.GetAttributes(path);
        if (attribute.HasFlag(FileAttributes.Directory))
        {
            var entries = Directory.GetFileSystemEntries(path).Order();
            var directory = new DirectoryInfo(path);
            var directoryNameHash = MD5.HashData(Encoding.UTF32.GetBytes(directory.Name));
            result.AddRange(directoryNameHash);

            foreach (var entry in entries.AsParallel().AsOrdered())
            {
                result.AddRange(Calculate(entry));
            }

            return [.. result];
        }

        using var fileContent = File.Open(path, FileMode.Open);
        var fileHash = MD5.HashData(fileContent);
        result.AddRange(fileHash);
        return [.. result];
    }
}