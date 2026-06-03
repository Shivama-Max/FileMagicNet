using FileMagicNet.Models;

namespace FileMagicNet.Signatures;

internal static class ImageSignatures
{
    public static IEnumerable<FileSignature> All =>
    [
        new("PNG",  "image/png",  ".png",  [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]),
        new("JPEG", "image/jpeg", ".jpg",  [0xFF, 0xD8, 0xFF]),
        new("GIF87a", "image/gif", ".gif", [0x47, 0x49, 0x46, 0x38, 0x37, 0x61]),
        new("GIF89a", "image/gif", ".gif", [0x47, 0x49, 0x46, 0x38, 0x39, 0x61]),
        new("BMP",  "image/bmp",  ".bmp",  [0x42, 0x4D]),
        new("TIFF (little-endian)", "image/tiff", ".tiff", [0x49, 0x49, 0x2A, 0x00]),
        new("TIFF (big-endian)",    "image/tiff", ".tiff", [0x4D, 0x4D, 0x00, 0x2A]),
        new("ICO",  "image/x-icon", ".ico", [0x00, 0x00, 0x01, 0x00]),
        new("HEIC", "image/heic",  ".heic", [0x66, 0x74, 0x79, 0x70, 0x68, 0x65, 0x69, 0x63], offset: 4),
        new("AVIF", "image/avif",  ".avif", [0x66, 0x74, 0x79, 0x70, 0x61, 0x76, 0x69, 0x66], offset: 4),
        // WebP: starts with RIFF....WEBP
        new("WebP", "image/webp", ".webp",  [0x52, 0x49, 0x46, 0x46]),  // matched with secondary check in validator
    ];
}
