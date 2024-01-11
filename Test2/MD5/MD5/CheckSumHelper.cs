using static System.Security.Cryptography.MD5;
using static System.Text.Encoding;

namespace MD5;

/// <summary>
/// Provides methods for calculating MD5 checksums for files and directories.
/// </summary>
public static class CheckSumHelper
{
    /// <summary>
    /// Calculates the MD5 checksum for a file or the contents of a single file in a directory.
    /// </summary>
    /// <param name="path">The path to the file or directory.</param>
    /// <returns>The MD5 checksum as a byte array.</returns>
    /// <exception cref="ArgumentException"> file or directory does not exist</exception>
    /// <exception cref="IOException"> file is in use</exception>
    public static byte[] SingleThreadCheckSum(string path)
    {
        if (File.Exists(path))
        {
            return GetFileHash(path);
        }

        if (!Directory.Exists(path))
        {
            throw new ArgumentException("No such file or directory");
        }

        return GetDirectoryHash(path);
    }

    /// <summary>
    /// Calculates the MD5 checksum for a file or the contents of a single file in a directory using multithreading
    /// based on using ThreadPool Tasks.
    /// </summary>
    /// <param name="path">The path to the file or directory.</param>
    /// <returns>The MD5 checksum as a byte array.</returns>
    /// <exception cref="ArgumentException"> file or directory does not exist</exception>
    /// <exception cref="IOException"> file is in use</exception>
    public static async Task<byte[]> MultiThreadCheckSum(string path)
    {
        if (File.Exists(path))
        {
            return await GetFileHashAsync(path);
        }

        if (!Directory.Exists(path))
        {
            throw new ArgumentException("No such file or directory");
        }

        return await GetDirectoryHashAsync(path);
    }


    private static byte[] GetFileHash(string filePath)
        => HashData(File.ReadAllBytes(filePath));


    private static async Task<byte[]> GetFileHashAsync(string filePath)
    {
        var bytes = await File.ReadAllBytesAsync(filePath);
        return HashData(bytes);
    }
    

    private static byte[] GetDirectoryHash(string directoryPath)
    {
        var subDirPaths = Directory.GetDirectories(directoryPath);
        var filePaths = Directory.GetFiles(directoryPath);

        Array.Sort(subDirPaths);
        Array.Sort(filePaths);

        var preparedBytes = new List<byte>(UTF8.GetBytes(Path.GetFileName(directoryPath) ?? string.Empty));
        foreach (var innerFilePath in filePaths)
        {
            preparedBytes.AddRange(GetFileHash(innerFilePath));
        }

        foreach (var innerDirPath in subDirPaths)
        {
            preparedBytes.AddRange(GetDirectoryHash(innerDirPath));
        }

        return preparedBytes.ToArray();
    }

    private static async Task<byte[]> GetDirectoryHashAsync(string directoryPath)
    {
        var subDirPaths = Directory.GetDirectories(directoryPath);
        var filePaths = Directory.GetFiles(directoryPath);

        Array.Sort(subDirPaths);
        Array.Sort(filePaths);

        var fileCacheTasks = new Task<byte[]>[filePaths.Length];
        var directoryCacheTasks = new Task<byte[]>[subDirPaths.Length];

        for (var i = 0; i < filePaths.Length; i++)
        {
            fileCacheTasks[i] = GetFileHashAsync(filePaths[i]);
        }

        for (var i = 0; i < subDirPaths.Length; ++i)
        {
            directoryCacheTasks[i] = GetDirectoryHashAsync(subDirPaths[i]);
        }

        await Task.WhenAll(fileCacheTasks);
        await Task.WhenAll(directoryCacheTasks);

        var result = new List<byte>(UTF8.GetBytes(Path.GetFileName(directoryPath) ?? string.Empty));

        foreach (var fileCacheTask in fileCacheTasks)
        {
            result.AddRange(fileCacheTask.Result);
        }

        foreach (var directoryCacheTask in directoryCacheTasks)
        {
            result.AddRange(directoryCacheTask.Result);
        }

        return result.ToArray();
    }
}