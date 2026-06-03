namespace FileMagicNet.Models;

/// <summary>
/// Result of a magic-bytes validation check.
/// </summary>
public sealed class ValidationResult
{
    /// <summary>Whether a matching signature was found.</summary>
    public bool IsMatch { get; init; }

    /// <summary>The matched signature, or null if no match was found.</summary>
    public FileSignature? Signature { get; init; }

    /// <summary>Convenience: format name of the matched file, or null.</summary>
    public string? Format => Signature?.Name;

    /// <summary>Convenience: MIME type of the matched file, or null.</summary>
    public string? MimeType => Signature?.MimeType;

    /// <summary>Convenience: file extension of the matched file, or null.</summary>
    public string? Extension => Signature?.Extension;

    /// <summary>All signatures that were matched (useful when multiple signatures apply).</summary>
    public IReadOnlyList<FileSignature> AllMatches { get; init; } = [];

    internal static ValidationResult NoMatch() =>
        new() { IsMatch = false, Signature = null, AllMatches = [] };

    internal static ValidationResult Match(IReadOnlyList<FileSignature> matches) =>
        new() { IsMatch = true, Signature = matches[0], AllMatches = matches };

    public override string ToString() =>
        IsMatch ? $"Match: {Format} ({MimeType})" : "No match found";
}
