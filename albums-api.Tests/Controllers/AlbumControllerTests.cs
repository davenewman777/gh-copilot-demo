using albums_api.Controllers;
using albums_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace albums_api.Tests.Controllers;

public class AlbumControllerTests
{
    private readonly AlbumController _sut = new();

    private static Artist CreateArtist(string name = "Test Artist")
    {
        return new Artist(name, new DateOnly(1980, 1, 1), "Test City");
    }

    [Fact]
    public void GivenAlbum_WhenCreate_ReturnsCreatedAlbum()
    {
        // Arrange
        var album = new Album(0, "New Album", CreateArtist("New Artist"), 2024, 11.99, "https://example.com/cover.png");

        // Act
        var result = _sut.Create(album);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var createdAlbum = Assert.IsType<Album>(createdResult.Value);
        Assert.True(createdAlbum.Id > 0);
        Assert.Equal("New Album", createdAlbum.Title);
        Assert.Equal("New Artist", createdAlbum.Artist.Name);
    }

    [Fact]
    public void GivenInvalidSortBy_WhenGet_ReturnsBadRequest()
    {
        // Act
        var result = _sut.Get("unknown", null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("sortBy must be title, artist, or price.", badRequestResult.Value);
    }

    [Fact]
    public void GivenNullAlbum_WhenCreate_ReturnsBadRequest()
    {
        // Act
        var result = _sut.Create(null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Album is required.", badRequestResult.Value);
    }

    [Fact]
    public void GivenNullAlbum_WhenUpdate_ReturnsBadRequest()
    {
        // Act
        var result = _sut.Update(1, null);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Album is required.", badRequestResult.Value);
    }

    [Fact]
    public void GivenSortByArtist_WhenGet_ReturnsAlbumsSortedByArtistName()
    {
        // Act
        var result = _sut.Get("artist", null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var albums = Assert.IsAssignableFrom<IEnumerable<Album>>(okResult.Value).ToList();
        Assert.Equal(albums.OrderBy(album => album.Artist.Name), albums);
    }

    [Fact]
    public void GivenExistingAlbum_WhenDelete_ReturnsNoContent()
    {
        // Arrange
        var album = Album.Create(new Album(0, "Delete Me", CreateArtist(), 2024, 9.99, "https://example.com/delete.png"));

        // Act
        var result = _sut.Delete(album.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void GivenExistingAlbum_WhenGetById_ReturnsAlbum()
    {
        // Arrange
        var album = Album.Create(new Album(0, "Find Me", CreateArtist(), 2024, 9.99, "https://example.com/find.png"));

        try
        {
            // Act
            var result = _sut.Get(album.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAlbum = Assert.IsType<Album>(okResult.Value);
            Assert.Equal(album.Id, returnedAlbum.Id);
        }
        finally
        {
            Album.Delete(album.Id);
        }
    }

    [Fact]
    public void GivenExistingAlbum_WhenUpdate_ReturnsUpdatedAlbum()
    {
        // Arrange
        var album = Album.Create(new Album(0, "Before", CreateArtist(), 2024, 9.99, "https://example.com/before.png"));
        var update = album with { Title = "After", Price = 12.99 };

        // Act
        var result = _sut.Update(album.Id, update);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var updatedAlbum = Assert.IsType<Album>(okResult.Value);
        Assert.Equal(album.Id, updatedAlbum.Id);
        Assert.Equal("After", updatedAlbum.Title);
        Assert.Equal(12.99, updatedAlbum.Price);
    }

    [Fact]
    public void GivenMissingAlbum_WhenDelete_ReturnsNotFound()
    {
        // Act
        var result = _sut.Delete(-1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GivenMissingAlbum_WhenGetById_ReturnsNotFound()
    {
        // Act
        var result = _sut.Get(-1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GivenMissingAlbum_WhenUpdate_ReturnsNotFound()
    {
        // Arrange
        var album = new Album(-1, "Missing", CreateArtist(), 2024, 9.99, "https://example.com/missing.png");

        // Act
        var result = _sut.Update(-1, album);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GivenYear_WhenGet_ReturnsAlbumsFromYear()
    {
        // Arrange
        var album = Album.Create(new Album(0, "Year Match", CreateArtist(), 2030, 9.99, "https://example.com/year.png"));

        try
        {
            // Act
            var result = _sut.Get(null, 2030);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<IEnumerable<Album>>(okResult.Value);
            Assert.Contains(albums, item => item.Id == album.Id && item.Year == 2030);
        }
        finally
        {
            Album.Delete(album.Id);
        }
    }
}
