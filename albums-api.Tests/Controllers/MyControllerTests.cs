using UnsecureApp.Controllers;

namespace albums_api.Tests.Controllers;

public class MyControllerTests
{
    private readonly MyController _sut = new();

    [Fact]
    public void GivenExistingFile_WhenReadFile_ReturnsFileContents()
    {
        // Arrange
        using var fileStream = new MockFileStream("album data");

        // Act
        var actual = _sut.ReadFile(fileStream);

        // Assert
        Assert.StartsWith("album data", actual);
    }

    [Fact]
    public void GivenMissingFile_WhenReadFile_ThrowsFileNotFoundException()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _sut.ReadFile(filePath));
    }

    [Fact]
    public void GivenProductName_WhenGetProduct_ThrowsInvalidOperationException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _sut.GetProduct("Album"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GivenMissingProductName_WhenGetProduct_ThrowsArgumentException(string? productName)
    {
        // Act & Assert
        Assert.ThrowsAny<ArgumentException>(() => _sut.GetProduct(productName!));
    }

    [Fact]
    public void WhenGetObject_DoesNotThrow()
    {
        // Act
        var exception = Record.Exception(() => _sut.GetObject());

        // Assert
        Assert.Null(exception);
    }

    private class MockFileStream : FileStream
    {
        private readonly MemoryStream _stream;

        public MockFileStream(string contents)
            : base(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, 4096, FileOptions.DeleteOnClose)
        {
            _stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(contents));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}