using FileMagicNet.Models;

namespace FileMagicNet.Signatures;

internal static class DocumentSignatures
{
    public static IEnumerable<FileSignature> All =>
    [
        new("PDF",  "application/pdf", ".pdf",
            [0x25, 0x50, 0x44, 0x46]),                                    // %PDF

        // Office Open XML (DOCX, XLSX, PPTX) — all ZIP-based PK\x03\x04
        new("DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document",   ".docx",
            [0x50, 0x4B, 0x03, 0x04]),
        new("XLSX", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",         ".xlsx",
            [0x50, 0x4B, 0x03, 0x04]),
        new("PPTX", "application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx",
            [0x50, 0x4B, 0x03, 0x04]),

        // Legacy OLE2 Compound Document (DOC, XLS, PPT)
        new("DOC",  "application/msword",                    ".doc",
            [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1]),
        new("XLS",  "application/vnd.ms-excel",              ".xls",
            [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1]),
        new("PPT",  "application/vnd.ms-powerpoint",         ".ppt",
            [0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1]),

        // OpenDocument formats
        new("ODT",  "application/vnd.oasis.opendocument.text",        ".odt",
            [0x50, 0x4B, 0x03, 0x04]),
        new("ODS",  "application/vnd.oasis.opendocument.spreadsheet", ".ods",
            [0x50, 0x4B, 0x03, 0x04]),
        new("ODP",  "application/vnd.oasis.opendocument.presentation",".odp",
            [0x50, 0x4B, 0x03, 0x04]),

        new("RTF",  "application/rtf", ".rtf",
            [0x7B, 0x5C, 0x72, 0x74, 0x66]),                            // {\rtf

        new("EPUB", "application/epub+zip", ".epub",
            [0x50, 0x4B, 0x03, 0x04]),
    ];
}
