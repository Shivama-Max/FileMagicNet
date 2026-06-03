using FileMagicNet;
using FileMagicNet.Models;

namespace FileMagicNet.Tests;

public class MagicBytesValidatorTests
{
    private readonly MagicBytesValidator _validator = new();

    // -------------------------------------------------------------------------
    // Image tests
    // -------------------------------------------------------------------------

    [Fact]
    public void Validate_PngBytes_ReturnsPngMatch()
    {
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00];
        var result = _validator.Validate(png);

        Assert.True(result.IsMatch);
        Assert.Equal("PNG", result.Format);
        Assert.Equal("image/png", result.MimeType);
        Assert.Equal(".png", result.Extension);
    }

    [Fact]
    public void Validate_JpegBytes_ReturnsJpegMatch()
    {
        byte[] jpeg = [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10];
        var result = _validator.Validate(jpeg);

        Assert.True(result.IsMatch);
        Assert.Equal("image/jpeg", result.MimeType);
    }

    [Fact]
    public void Validate_GifBytes_ReturnsGifMatch()
    {
        byte[] gif = [0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x00, 0x00]; // GIF89a
        var result = _validator.Validate(gif);

        Assert.True(result.IsMatch);
        Assert.Equal("image/gif", result.MimeType);
    }

    // -------------------------------------------------------------------------
    // Document tests
    // -------------------------------------------------------------------------

    [Fact]
    public void Validate_PdfBytes_ReturnsPdfMatch()
    {
        byte[] pdf = [0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E]; // %PDF-1.
        var result = _validator.Validate(pdf);

        Assert.True(result.IsMatch);
        Assert.Equal("PDF", result.Format);
        Assert.Equal("application/pdf", result.MimeType);
    }

    [Fact]
    public void Validate_ZipBytes_ReturnsZipMatch()
    {
        byte[] zip = [0x50, 0x4B, 0x03, 0x04, 0x00, 0x00];
        var result = _validator.Validate(zip);

        Assert.True(result.IsMatch);
        Assert.Contains(result.AllMatches, m => m.MimeType == "application/zip");
    }

    // -------------------------------------------------------------------------
    // Archive tests
    // -------------------------------------------------------------------------

    [Fact]
    public void Validate_GzipBytes_ReturnsGzipMatch()
    {
        byte[] gz = [0x1F, 0x8B, 0x08, 0x00];
        var result = _validator.Validate(gz);

        Assert.True(result.IsMatch);
        Assert.Equal("GZIP", result.Format);
    }

    [Fact]
    public void Validate_SevenZipBytes_Returns7zMatch()
    {
        byte[] sevenZ = [0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C];
        var result = _validator.Validate(sevenZ);

        Assert.True(result.IsMatch);
        Assert.Equal("7-Zip", result.Format);
    }

    // -------------------------------------------------------------------------
    // No match
    // -------------------------------------------------------------------------

    [Fact]
    public void Validate_RandomBytes_ReturnsNoMatch()
    {
        byte[] random = [0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07];
        var result = _validator.Validate(random);

        Assert.False(result.IsMatch);
        Assert.Null(result.Format);
        Assert.Null(result.MimeType);
    }

    [Fact]
    public void Validate_EmptyBytes_ReturnsNoMatch()
    {
        var result = _validator.Validate(Array.Empty<byte>());
        Assert.False(result.IsMatch);
    }

    // -------------------------------------------------------------------------
    // Custom signature registration
    // -------------------------------------------------------------------------

    [Fact]
    public void Register_CustomSignature_MatchesCorrectly()
    {
        var registry = SignatureRegistry.CreateEmpty();
        registry.Register(new FileSignature(
            name: "MyCustomFormat",
            mimeType: "application/x-custom",
            extension: ".myf",
            magicBytes: [0xDE, 0xAD, 0xBE, 0xEF]
        ));

        var validator = new MagicBytesValidator(registry);
        var result = validator.Validate(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0x00 });

        Assert.True(result.IsMatch);
        Assert.Equal("MyCustomFormat", result.Format);
    }

    [Fact]
    public void Register_CustomSignature_TakesPriorityOverBuiltIn()
    {
        var registry = new SignatureRegistry(); // starts with defaults
        registry.Register(new FileSignature(
            name: "MyPNG Override",
            mimeType: "application/x-custom-png",
            extension: ".cpng",
            magicBytes: [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]
        ));

        var validator = new MagicBytesValidator(registry);
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
        var result = validator.Validate(png);

        // Custom signature should be first in AllMatches
        Assert.Equal("MyPNG Override", result.Format);
    }

    // -------------------------------------------------------------------------
    // Async tests
    // -------------------------------------------------------------------------

    [Fact]
    public async Task ValidateAsync_PngStream_ReturnsPngMatch()
    {
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00];
        using var ms = new MemoryStream(png);
        var result = await _validator.ValidateAsync(ms);

        Assert.True(result.IsMatch);
        Assert.Equal("image/png", result.MimeType);
    }

    // -------------------------------------------------------------------------
    // IsMatch helper
    // -------------------------------------------------------------------------

    [Fact]
    public void IsMatch_CorrectMime_ReturnsTrue()
    {
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
        using var ms = new MemoryStream(png);
        Assert.True(_validator.IsMatch(ms, "image/png"));
    }

    [Fact]
    public void IsMatch_WrongMime_ReturnsFalse()
    {
        byte[] png = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00];
        using var ms = new MemoryStream(png);
        Assert.False(_validator.IsMatch(ms, "image/jpeg"));
    }

    // -------------------------------------------------------------------------
    // Static API
    // -------------------------------------------------------------------------

    [Fact]
    public void DetectFrom_StaticApi_WorksCorrectly()
    {
        byte[] pdf = [0x25, 0x50, 0x44, 0x46];
        var result = MagicBytesValidator.DetectFrom(pdf);
        Assert.Equal("PDF", result.Format);
    }
}
