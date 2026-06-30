using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace albums_api.Controllers
{
    [Route("albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        // GET: api/album
        [HttpGet]
        public IActionResult Get([FromQuery] string? sortBy)
        {
            var albums = Album.GetAll();

            var sortedAlbums = sortBy?.ToLowerInvariant() switch
            {
                null or "" => albums,
                "title" => albums.OrderBy(album => album.Title).ToList(),
                "artist" => albums.OrderBy(album => album.Artist).ToList(),
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

    }
}
