namespace FileMagicNet.Models;

/// <summary>
/// Represents a file format identified by its magic bytes signature.
/// </summary>
public sealed class FileSignature
{
    /// <summary>Human-readable format name (e.g. "PNG", "PDF").</summary>
    public string Name { get; init; }

    /// <summary>MIME type (e.g. "image/png").</summary>
    public string MimeType { get; init; }

    /// <summary>Typical file extension including the dot (e.g. ".png").</summary>
    public string Extension { get; init; }

    /// <summary>Magic bytes that must match at the given <see cref="Offset"/>.</summary>
    public byte[] MagicBytes { get; init; }

    /// <summary>Byte offset from the start of the file where the signature begins. Default is 0.</summary>
    public int Offset { get; init; }

    /// <summary>
    /// Optional mask applied bitwise to the read bytes before comparison.
    /// When null, a direct byte comparison is performed.
    /// </summary>
    public byte[]? Mask { get; init; }

    /// <summary>Creates a new file signature with validation.</summary>
    public FileSignature(
        string name,
        string mimeType,
        string extension,
        byte[] magicBytes,
        int offset = 0,
        byte[]? mask = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be empty.", nameof(name));
        if (string.IsNullOrWhiteSpace(mimeType)) throw new ArgumentException("MimeType cannot be empty.", nameof(mimeType));
        if (string.IsNullOrWhiteSpace(extension)) throw new ArgumentException("Extension cannot be null or empty.", nameof(extension));
        if (magicBytes is null || magicBytes.Length == 0) throw new ArgumentException("MagicBytes cannot be empty.", nameof(magicBytes));
        if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "Offset cannot be negative.");
        if (mask != null && mask.Length != magicBytes.Length)
            throw new ArgumentException("Mask length must equal MagicBytes length.", nameof(mask));

        Name       = name;
        MimeType   = mimeType;
        Extension  = extension;
        MagicBytes = magicBytes;
        Offset     = offset;
        Mask       = mask;
    }
}
