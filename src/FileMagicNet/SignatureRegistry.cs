using FileMagicNet.Models;
using FileMagicNet.Signatures;

namespace FileMagicNet;

/// <summary>
/// Central registry of all known file signatures.
/// Supports custom signature registration.
/// Thread-safe for reads; use locking when registering at startup.
/// </summary>
public sealed class SignatureRegistry
{
    private readonly List<FileSignature> _signatures = [];
    private static readonly Lazy<SignatureRegistry> _default = new(() =>
    {
        var r = new SignatureRegistry();
        r.LoadDefaults();
        return r;
    });

    /// <summary>The default registry pre-loaded with all built-in signatures.</summary>
    public static SignatureRegistry Default => _default.Value;

    /// <summary>All currently registered signatures (snapshot).</summary>
    public IReadOnlyList<FileSignature> Signatures => _signatures.AsReadOnly();

    /// <summary>Register a custom signature. Prepends to give user signatures priority.</summary>
    public void Register(FileSignature signature)
    {
        ArgumentNullException.ThrowIfNull(signature);
        _signatures.Insert(0, signature);
    }

    /// <summary>Register multiple custom signatures.</summary>
    public void Register(IEnumerable<FileSignature> signatures)
    {
        ArgumentNullException.ThrowIfNull(signatures);
        foreach (var sig in signatures)
            Register(sig);
    }

    /// <summary>Remove all signatures matching the given name (case-insensitive).</summary>
    public int Remove(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));
        return _signatures.RemoveAll(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Create a new empty registry (without defaults).</summary>
    public static SignatureRegistry CreateEmpty() => new();

    private void LoadDefaults()
    {
        _signatures.AddRange(ImageSignatures.All);
        _signatures.AddRange(DocumentSignatures.All);
        _signatures.AddRange(ArchiveSignatures.All);
        _signatures.AddRange(MediaSignatures.All);
        _signatures.AddRange(ExecutableSignatures.All);
        _signatures.AddRange(FontSignatures.All);
    }
}
