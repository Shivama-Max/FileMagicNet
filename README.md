# FileMagicNet

A modern, actively maintained .NET library for file type detection using **magic bytes** (file signatures). Never trust a file extension — validate the actual content.

## Features

- ✅ 60+ built-in signatures across images, documents, archives, media, executables, and fonts
- ✅ Async-first API (`ValidateAsync`)
- ✅ Works with `Stream`, `byte[]`, or file path
- ✅ Returns format name, MIME type, and file extension
- ✅ Offset-aware matching (e.g. MP4, HEIC)
- ✅ Custom signature registration
- ✅ No external dependencies
- ✅ Targets .NET 10

## Installation

```bash
dotnet add package FileMagicNet
```

## Quick Start

```csharp
using FileMagicNet;

// From file path
var result = MagicBytesValidator.DetectFrom("upload.png");
Console.WriteLine(result.Format);    // PNG
Console.WriteLine(result.MimeType);  // image/png
Console.WriteLine(result.Extension); // .png
Console.WriteLine(result.IsMatch);   // true

// From stream (e.g. ASP.NET Core IFormFile)
using var stream = formFile.OpenReadStream();
var result = await MagicBytesValidator.DetectFromAsync(stream);

// Guard check — ensure upload is actually a PDF
bool isPdf = validator.IsMatch(stream, "application/pdf");
```

## Validate Against Expected Type

```csharp
var validator = new MagicBytesValidator();

// Quick guard
bool isValidImage = validator.IsMatch(stream, "image/png");

// Full result
var result = validator.Validate(stream);
if (!result.IsMatch)
{
    return BadRequest("Unknown file type.");
}

if (result.MimeType != "image/jpeg" && result.MimeType != "image/png")
{
    return BadRequest("Only JPEG and PNG are allowed.");
}
```

## Register Custom Signatures

```csharp
var registry = new SignatureRegistry(); // starts with all built-in signatures
registry.Register(new FileSignature(
    name:       "MyFormat",
    mimeType:   "application/x-myformat",
    extension:  ".myf",
    magicBytes: new byte[] { 0x4D, 0x59, 0x46 }
));

var validator = new MagicBytesValidator(registry);
```

Custom signatures are given priority over built-in ones.

## Supported Formats

### Images
PNG, JPEG, GIF, BMP, TIFF, ICO, HEIC, AVIF, WebP

### Documents
PDF, DOCX, XLSX, PPTX, DOC, XLS, PPT, ODT, ODS, ODP, RTF, EPUB

### Archives
ZIP, RAR, 7-Zip, GZIP, BZIP2, XZ, ZSTD, LZ4, TAR, ISO, CAB

### Audio
MP3, WAV, FLAC, OGG, AAC, AIFF, M4A, MIDI, WMA

### Video
MP4, MOV, AVI, MKV, WebM, FLV, WMV, MPEG, 3GP

### Executables
ELF, PE/EXE, Mach-O, DEX, JVM Class, WASM

### Fonts
TTF, OTF, WOFF, WOFF2, EOT

## Multiple Matches

When a file matches multiple signatures (e.g. ZIP-based formats like DOCX, XLSX, PPTX), all matches are returned:

```csharp
var result = validator.Validate(docxBytes);
Console.WriteLine(result.Format);           // Primary match
Console.WriteLine(result.AllMatches.Count); // All matching signatures
```

> **Note:** For ZIP-based Office formats (DOCX/XLSX/PPTX), inspect the archive contents (look for `[Content_Types].xml`) to disambiguate definitively.

## ASP.NET Core Integration

```csharp
// Program.cs
builder.Services.AddSingleton<MagicBytesValidator>();

// Controller
[HttpPost("upload")]
public async Task<IActionResult> Upload(IFormFile file)
{
    await using var stream = file.OpenReadStream();
    var result = await _validator.ValidateAsync(stream);

    if (!result.IsMatch || result.MimeType != "image/jpeg")
        return BadRequest("Only JPEG images are accepted.");

    // proceed with safe upload...
}
