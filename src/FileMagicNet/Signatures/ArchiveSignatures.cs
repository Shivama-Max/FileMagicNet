using FileMagicNet.Models;

namespace FileMagicNet.Signatures;

internal static class ArchiveSignatures
{
    public static IEnumerable<FileSignature> All =>
    [
        new("ZIP",   "application/zip",             ".zip",  [0x50, 0x4B, 0x03, 0x04]),
        new("ZIP (empty)", "application/zip",       ".zip",  [0x50, 0x4B, 0x05, 0x06]),
        new("ZIP (spanned)", "application/zip",     ".zip",  [0x50, 0x4B, 0x07, 0x08]),
        new("RAR4",  "application/x-rar-compressed",".rar",  [0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00]),
        new("RAR5",  "application/x-rar-compressed",".rar",  [0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00]),
        new("7-Zip", "application/x-7z-compressed", ".7z",   [0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C]),
        new("GZIP",  "application/gzip",            ".gz",   [0x1F, 0x8B]),
        new("BZIP2", "application/x-bzip2",         ".bz2",  [0x42, 0x5A, 0x68]),
        new("XZ",    "application/x-xz",            ".xz",   [0xFD, 0x37, 0x7A, 0x58, 0x5A, 0x00]),
        new("ZSTD",  "application/zstd",            ".zst",  [0x28, 0xB5, 0x2F, 0xFD]),
        new("LZ4",   "application/x-lz4",           ".lz4",  [0x04, 0x22, 0x4D, 0x18]),
        new("TAR",   "application/x-tar",           ".tar",  [0x75, 0x73, 0x74, 0x61, 0x72], offset: 257),
        new("ISO",   "application/x-iso9660-image", ".iso",  [0x43, 0x44, 0x30, 0x30, 0x31], offset: 32769),
        new("CAB",   "application/vnd.ms-cab-compressed", ".cab", [0x4D, 0x53, 0x43, 0x46]),
    ];
}
