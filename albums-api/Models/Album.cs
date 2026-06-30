namespace albums_api.Models
{
    public record Album(int Id, string Title, Artist Artist, int Year, double Price, string Image_url)
    {
        private static readonly List<Album> Albums = new()
        {
            new Album(1, "You, Me and an App Id", new Artist("Daprize", new DateOnly(1984, 3, 12), "Seattle"), 2021, 10.99, "https://aka.ms/albums-daprlogo"),
            new Album(2, "Seven Revision Army", new Artist("The Blue-Green Stripes", new DateOnly(1979, 7, 4), "Detroit"), 2021, 13.99, "https://aka.ms/albums-containerappslogo"),
            new Album(3, "Scale It Up", new Artist("KEDA Club", new DateOnly(1990, 1, 22), "Austin"), 2021, 13.99, "https://aka.ms/albums-kedalogo"),
            new Album(4, "Lost in Translation", new Artist("MegaDNS", new DateOnly(1988, 11, 9), "London"), 2021, 12.99,"https://aka.ms/albums-envoylogo"),
            new Album(5, "Lock Down Your Love", new Artist("V is for VNET", new DateOnly(1986, 5, 17), "Amsterdam"), 2021, 12.99, "https://aka.ms/albums-vnetlogo"),
            new Album(6, "Sweet Container O' Mine", new Artist("Guns N Probeses", new DateOnly(1982, 8, 31), "Los Angeles"), 2021, 14.99, "https://aka.ms/albums-containerappslogo")
        };

        private static readonly object SyncRoot = new();

        public static List<Album> GetAll()
        {
            lock (SyncRoot)
            {
                return Albums.ToList();
            }
        }

        public static Album? GetById(int id)
        {
            lock (SyncRoot)
            {
                return Albums.FirstOrDefault(a => a.Id == id);
            }
        }

        public static List<Album> SearchByYear(int year)
        {
            lock (SyncRoot)
            {
                return Albums.Where(a => a.Year == year).ToList();
            }
        }

        public static Album Create(Album album)
        {
            lock (SyncRoot)
            {
                var nextId = Albums.Count == 0 ? 1 : Albums.Max(a => a.Id) + 1;
                var createdAlbum = album with { Id = nextId };

                Albums.Add(createdAlbum);

                return createdAlbum;
            }
        }

        public static Album? Update(int id, Album album)
        {
            lock (SyncRoot)
            {
                var index = Albums.FindIndex(a => a.Id == id);

                if (index < 0)
                {
                    return null;
                }

                var updatedAlbum = album with { Id = id };
                Albums[index] = updatedAlbum;

                return updatedAlbum;
            }
        }

        public static bool Delete(int id)
        {
            lock (SyncRoot)
            {
                var album = Albums.FirstOrDefault(a => a.Id == id);

                if (album is null)
                {
                    return false;
                }

                Albums.Remove(album);

                return true;
            }
        }
    }
}
