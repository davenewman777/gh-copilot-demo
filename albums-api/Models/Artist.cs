namespace albums_api.Models
{
    /// <summary>
    /// Represents an album artist.
    /// </summary>
    /// <param name="Name">The artist name.</param>
    /// <param name="Birthdate">The artist birthdate.</param>
    /// <param name="BirthPlace">The artist birthplace.</param>
    public record Artist(string Name, DateOnly Birthdate, string BirthPlace);
}