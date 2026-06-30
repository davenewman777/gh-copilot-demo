using albums_api.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace albums_api.Controllers
{
    [Route("albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        // GET: api/album
        [HttpGet]
        public IActionResult Get([FromQuery] string? sortBy, [FromQuery] int? year)
        {
            var albums = year.HasValue ? Album.SearchByYear(year.Value) : Album.GetAll();

            var sortedAlbums = sortBy?.ToLowerInvariant() switch
            {
                null or "" => albums,
                "title" => albums.OrderBy(album => album.Title).ToList(),
                "artist" => albums.OrderBy(album => album.Artist.Name).ToList(),
                "price" => albums.OrderBy(album => album.Price).ToList(),
                _ => null
            };

            if (sortedAlbums is null)
            {
                return BadRequest("sortBy must be title, artist, or price.");
            }

            return Ok(sortedAlbums);
        }

        // GET api/<AlbumController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var album = Album.GetById(id);

            if (album is null)
            {
                return NotFound();
            }

            return Ok(album);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Album? album)
        {
            if (album is null)
            {
                return BadRequest("Album is required.");
            }

            var createdAlbum = Album.Create(album);

            return CreatedAtAction(nameof(Get), new { id = createdAlbum.Id }, createdAlbum);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Album? album)
        {
            if (album is null)
            {
                return BadRequest("Album is required.");
            }

            var updatedAlbum = Album.Update(id, album);

            if (updatedAlbum is null)
            {
                return NotFound();
            }

            return Ok(updatedAlbum);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!Album.Delete(id))
            {
                return NotFound();
            }

            return NoContent();
        }

    }
}
