using FileMagicNet.Models;

namespace FileMagicNet;

/// <summary>
/// Validates files by reading their magic bytes and matching against known signatures.
/// </summary>
public sealed class MagicBytesValidator
{
    private readonly SignatureRegistry _registry;

    /// <summary>Create a validator using the default built-in signature registry.</summary>
    public MagicBytesValidator() : this(SignatureRegistry.Default) { }

    /// <summary>Create a validator using a custom registry.</summary>
    public MagicBytesValidator(SignatureRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    // -------------------------------------------------------------------------
    // Sync overloads
    // -------------------------------------------------------------------------

    /// <summary>Validate a file by its path.</summary>
    public ValidationResult Validate(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return Validate(fs);
    }

    /// <summary>Validate a byte array (e.g. from a memory buffer or IFormFile).</summary>
    public ValidationResult Validate(byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var ms = new MemoryStream(data);
        return Validate(ms);
    }

    /// <summary>Validate a stream. The stream does not need to be seekable; reads are forward-only.</summary>
    public ValidationResult Validate(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Read enough bytes to satisfy the deepest offset + signature in the registry
        int needed = ComputeBufferSize();
        byte[] buffer = new byte[needed];
        int bytesRead = ReadFully(stream, buffer);

        return MatchSignatures(buffer, bytesRead);
    }

    // -------------------------------------------------------------------------
    // Async overloads
    // -------------------------------------------------------------------------

    /// <summary>Asynchronously validate a file by its path.</summary>
    public async Task<ValidationResult> ValidateAsync(string filePath, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));
        await using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return await ValidateAsync(fs, ct).ConfigureAwait(false);
    }

    /// <summary>Asynchronously validate a byte array.</summary>
    public async Task<ValidationResult> ValidateAsync(byte[] data, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(data);
        using var ms = new MemoryStream(data);
        return await ValidateAsync(ms, ct).ConfigureAwait(false);
    }

    /// <summary>Asynchronously validate a stream.</summary>
    public async Task<ValidationResult> ValidateAsync(Stream stream, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        int needed = ComputeBufferSize();
        byte[] buffer = new byte[needed];
        int bytesRead = await ReadFullyAsync(stream, buffer, ct).ConfigureAwait(false);

        return MatchSignatures(buffer, bytesRead);
    }

    // -------------------------------------------------------------------------
    // MIME / extension helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Returns true if the stream matches the given expected MIME type.
    /// Useful for quick guard checks (e.g. ensure uploaded file is image/png).
    /// </summary>
    public bool IsMatch(Stream stream, string expectedMimeType)
    {
        var result = Validate(stream);
        return result.IsMatch &&
               result.MimeType!.Equals(expectedMimeType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Same as <see cref="IsMatch(Stream, string)"/> but async.</summary>
    public async Task<bool> IsMatchAsync(Stream stream, string expectedMimeType, CancellationToken ct = default)
    {
        var result = await ValidateAsync(stream, ct).ConfigureAwait(false);
        return result.IsMatch &&
               result.MimeType!.Equals(expectedMimeType, StringComparison.OrdinalIgnoreCase);
    }

    // -------------------------------------------------------------------------
    // Static convenience API (uses Default registry)
    // -------------------------------------------------------------------------

    private static readonly MagicBytesValidator _defaultInstance = new();

    public static ValidationResult DetectFrom(string filePath)  => _defaultInstance.Validate(filePath);
    public static ValidationResult DetectFrom(byte[] data)      => _defaultInstance.Validate(data);
    public static ValidationResult DetectFrom(Stream stream)    => _defaultInstance.Validate(stream);

    public static Task<ValidationResult> DetectFromAsync(string filePath, CancellationToken ct = default)
        => _defaultInstance.ValidateAsync(filePath, ct);
    public static Task<ValidationResult> DetectFromAsync(Stream stream, CancellationToken ct = default)
        => _defaultInstance.ValidateAsync(stream, ct);

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private ValidationResult MatchSignatures(byte[] buffer, int bytesRead)
    {
        var matches = new List<FileSignature>();
        ReadOnlySpan<byte> data = buffer.AsSpan(0, bytesRead);

        foreach (var sig in _registry.Signatures)
        {
            if (sig.Offset + sig.MagicBytes.Length > bytesRead)
                continue;

            if (IsSignatureMatch(data, sig))
                matches.Add(sig);
        }

        return matches.Count > 0
            ? ValidationResult.Match(matches)
            : ValidationResult.NoMatch();
    }

    private static bool IsSignatureMatch(ReadOnlySpan<byte> buffer, FileSignature sig)
    {
        ReadOnlySpan<byte> magicBytes = sig.MagicBytes.AsSpan();

        for (int i = 0; i < magicBytes.Length; i++)
        {
            byte actual = buffer[sig.Offset + i];
            byte expected = magicBytes[i];

            if (sig.Mask != null)
                actual &= sig.Mask[i];

            if (actual != expected)
                return false;
        }
        return true;
    }

    private int ComputeBufferSize()
    {
        int max = 0;
        foreach (var sig in _registry.Signatures)
        {
            int end = sig.Offset + sig.MagicBytes.Length;
            if (end > max) max = end;
        }
        return Math.Max(max, 16); // minimum 16 bytes
    }

    private static int ReadFully(Stream stream, byte[] buffer)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = stream.Read(buffer, totalRead, buffer.Length - totalRead);
            if (read == 0) break;
            totalRead += read;
        }
        return totalRead;
    }

    private static async Task<int> ReadFullyAsync(Stream stream, byte[] buffer, CancellationToken ct)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer.AsMemory(totalRead, buffer.Length - totalRead), ct)
                                   .ConfigureAwait(false);
            if (read == 0) break;
            totalRead += read;
        }
        return totalRead;
    }
}
