using FileMagicNet.Models;

namespace FileMagicNet.Signatures;

internal static class ExecutableSignatures
{
    public static IEnumerable<FileSignature> All =>
    [
        new("ELF",        "application/x-elf",           ".elf",  [0x7F, 0x45, 0x4C, 0x46]),  // \x7FELF
        new("PE (EXE)",   "application/vnd.microsoft.portable-executable", ".exe", [0x4D, 0x5A]),  // MZ
        new("Mach-O 64",  "application/x-mach-binary",   ".macho",[0xCF, 0xFA, 0xED, 0xFE]),
        new("Mach-O 32",  "application/x-mach-binary",   ".macho",[0xCE, 0xFA, 0xED, 0xFE]),
        new("DEX (Android)", "application/vnd.android.dex", ".dex",[0x64, 0x65, 0x78, 0x0A]), // dex\n
        new("Class (JVM)","application/x-java-applet",   ".class",[0xCA, 0xFE, 0xBA, 0xBE]),
        new("WASM",       "application/wasm",            ".wasm", [0x00, 0x61, 0x73, 0x6D]),
        new("PyC",        "application/x-python-code",   ".pyc",  [0x0D, 0x0D, 0x0A, 0x0D]), // Python 3.8+
    ];
}

internal static class FontSignatures
{
    public static IEnumerable<FileSignature> All =>
    [
        new("TTF",  "font/ttf",        ".ttf",  [0x00, 0x01, 0x00, 0x00, 0x00]),
        new("OTF",  "font/otf",        ".otf",  [0x4F, 0x54, 0x54, 0x4F]),   // OTTO
        new("WOFF", "font/woff",       ".woff", [0x77, 0x4F, 0x46, 0x46]),   // wOFF
        new("WOFF2","font/woff2",      ".woff2",[0x77, 0x4F, 0x46, 0x32]),   // wOF2
        new("EOT",  "application/vnd.ms-fontobject", ".eot", [0x4C, 0x50]),
    ];
}
