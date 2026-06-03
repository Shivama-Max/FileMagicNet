using FileMagicNet.Models;

namespace FileMagicNet.Signatures;

internal static class MediaSignatures
{
    public static IEnumerable<FileSignature> All =>
    [
        // Audio
        new("MP3 (ID3)",  "audio/mpeg",    ".mp3",  [0x49, 0x44, 0x33]),           // ID3
        new("MP3",        "audio/mpeg",    ".mp3",  [0xFF, 0xFB]),
        new("MP3 (sync)", "audio/mpeg",    ".mp3",  [0xFF, 0xF3]),
        new("WAV",        "audio/wav",     ".wav",  [0x52, 0x49, 0x46, 0x46]),      // RIFF (secondary check needed)
        new("FLAC",       "audio/flac",    ".flac", [0x66, 0x4C, 0x61, 0x43]),      // fLaC
        new("OGG",        "audio/ogg",     ".ogg",  [0x4F, 0x67, 0x67, 0x53]),      // OggS
        new("AAC",        "audio/aac",     ".aac",  [0xFF, 0xF1]),
        new("AAC (ADTS)", "audio/aac",     ".aac",  [0xFF, 0xF9]),
        new("AIFF",       "audio/aiff",    ".aiff", [0x46, 0x4F, 0x52, 0x4D]),      // FORM
        new("M4A",        "audio/mp4",     ".m4a",  [0x66, 0x74, 0x79, 0x70, 0x4D, 0x34, 0x41, 0x20], offset: 4),
        new("MIDI",       "audio/midi",    ".mid",  [0x4D, 0x54, 0x68, 0x64]),      // MThd
        new("WMA",        "audio/x-ms-wma",".wma",  [0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11]),

        // Video
        new("MP4",        "video/mp4",     ".mp4",  [0x66, 0x74, 0x79, 0x70], offset: 4),
        new("MOV",        "video/quicktime",".mov",  [0x66, 0x74, 0x79, 0x70, 0x71, 0x74], offset: 4),
        new("AVI",        "video/x-msvideo",".avi",  [0x52, 0x49, 0x46, 0x46]),     // RIFF (secondary check needed)
        new("MKV",        "video/x-matroska",".mkv", [0x1A, 0x45, 0xDF, 0xA3]),
        new("WebM",       "video/webm",    ".webm", [0x1A, 0x45, 0xDF, 0xA3]),
        new("FLV",        "video/x-flv",   ".flv",  [0x46, 0x4C, 0x56, 0x01]),      // FLV\x01
        new("WMV",        "video/x-ms-wmv",".wmv",  [0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11]),
        new("MPEG",       "video/mpeg",    ".mpg",  [0x00, 0x00, 0x01, 0xBA]),
        new("3GP",        "video/3gpp",    ".3gp",  [0x66, 0x74, 0x79, 0x70, 0x33, 0x67], offset: 4),
    ];
}
