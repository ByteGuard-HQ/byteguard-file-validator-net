using System.IO.Compression;
using System.Text;

namespace ByteGuard.FileValidator.Tests.Unit.TestHelpers;

public static class ZipTestFactory
{
    public static MemoryStream CreateZipWithEntry(string entryName, string contents = "x")
    {
        var stream = new MemoryStream();

        using var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true);
        var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
        using var entryStream = entry.Open();
        using var writer = new StreamWriter(entryStream, Encoding.UTF8, leaveOpen: false);
        writer.Write(contents);

        stream.Position = 0;

        return stream;
    }

    public static MemoryStream CreateZipWithEntries(Dictionary<string, string> entries)
    {
        var stream = new MemoryStream();

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: true))
        {
            foreach (var kvp in entries)
            {
                var entry = archive.CreateEntry(kvp.Key, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                using var writer = new StreamWriter(entryStream, Encoding.UTF8, leaveOpen: false);
                writer.Write(kvp.Value);
            }
        }

        stream.Position = 0;

        return stream;
    }

    public static byte[] CreateZipWithFixedSizeEntries(int entryCount, int bytesPerEntry, CompressionLevel compression = CompressionLevel.NoCompression)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var payload = new byte[bytesPerEntry];
            for (int i = 0; i < entryCount; i++)
            {
                var entry = archive.CreateEntry($"file{i}.bin", compression);
                using var s = entry.Open();
                s.Write(payload, 0, payload.Length);
            }
        }

        return memoryStream.ToArray();
    }

    public static byte[] CreateZipWithHiglyCompressibleEntry(
        string entryName = "payload.bin",
        int uncompressedBytes = 5 * 1024 * 1024) // 5 MB
    {
        var data = new byte[uncompressedBytes];
        using var memoryStream = new MemoryStream();

        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
            using var entryStream = entry.Open();
            entryStream.Write(data, 0, data.Length);
        }

        return memoryStream.ToArray();
    }

    public static double GetCompressionRate(byte[] zipBytes)
    {
        using var zipStream = new MemoryStream(zipBytes);
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read, leaveOpen: true);
        var entry = archive.Entries.First();

        if (entry.Length == 0) return 0;
        if (entry.CompressedLength == 0) return double.PositiveInfinity;

        return (double)entry.Length / entry.CompressedLength;
    }
}
