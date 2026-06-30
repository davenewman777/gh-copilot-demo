export interface Album {
  id: number;
  title: string;
  artist: string;
  year: number;
  price: number;
  image_url: string;
}

export type AlbumInput = Omit<Album, "id"> & Partial<Pick<Album, "id">>;

export type AlbumSortKey = "title" | "artist" | "price";

export const sampleAlbums: Album[] = [
  {
    id: 1,
    title: "You, Me and an App Id",
    artist: "Daprize",
    year: 2021,
    price: 10.99,
    image_url: "https://aka.ms/albums-daprlogo"
  },
  {
    id: 2,
    title: "Seven Revision Army",
    artist: "The Blue-Green Stripes",
    year: 2021,
    price: 13.99,
    image_url: "https://aka.ms/albums-containerappslogo"
  },
  {
    id: 3,
    title: "Scale It Up",
    artist: "KEDA Club",
    year: 2021,
    price: 13.99,
    image_url: "https://aka.ms/albums-kedalogo"
  },
  {
    id: 4,
    title: "Lost in Translation",
    artist: "MegaDNS",
    year: 2021,
    price: 12.99,
    image_url: "https://aka.ms/albums-envoylogo"
  },
  {
    id: 5,
    title: "Lock Down Your Love",
    artist: "V is for VNET",
    year: 2021,
    price: 12.99,
    image_url: "https://aka.ms/albums-vnetlogo"
  },
  {
    id: 6,
    title: "Sweet Container O' Mine",
    artist: "Guns N Probeses",
    year: 2021,
    price: 14.99,
    image_url: "https://aka.ms/albums-containerappslogo"
  }
];

let albums: Album[] = [...sampleAlbums];

function cloneAlbum(album: Album): Album {
  return { ...album };
}

function cloneAlbums(source: Album[]): Album[] {
  return source.map(cloneAlbum);
}

export function resetAlbums(): void {
  albums = cloneAlbums(sampleAlbums);
}

export function listAlbums(options: { sortBy?: AlbumSortKey; year?: number } = {}): Album[] {
  const filteredAlbums = options.year === undefined
    ? albums
    : albums.filter((album) => album.year === options.year);

  const sortedAlbums = [...filteredAlbums];

  switch (options.sortBy) {
    case "title":
      sortedAlbums.sort((left, right) => left.title.localeCompare(right.title));
      break;
    case "artist":
      sortedAlbums.sort((left, right) => left.artist.localeCompare(right.artist));
      break;
    case "price":
      sortedAlbums.sort((left, right) => left.price - right.price);
      break;
    case undefined:
      break;
  }

  return cloneAlbums(sortedAlbums);
}

export function getAlbum(id: number): Album | undefined {
  const album = albums.find((candidate) => candidate.id === id);

  return album ? cloneAlbum(album) : undefined;
}

export function createAlbum(album: AlbumInput): Album {
  const nextId = albums.length === 0 ? 1 : Math.max(...albums.map((candidate) => candidate.id)) + 1;
  const createdAlbum = { ...album, id: nextId };

  albums.push(createdAlbum);

  return cloneAlbum(createdAlbum);
}

export function updateAlbum(id: number, album: AlbumInput): Album | undefined {
  const index = albums.findIndex((candidate) => candidate.id === id);

  if (index < 0) {
    return undefined;
  }

  const updatedAlbum = { ...album, id };
  albums[index] = updatedAlbum;

  return cloneAlbum(updatedAlbum);
}

export function deleteAlbum(id: number): boolean {
  const index = albums.findIndex((candidate) => candidate.id === id);

  if (index < 0) {
    return false;
  }

  albums.splice(index, 1);

  return true;
}

export function isAlbumInput(value: unknown): value is AlbumInput {
  if (typeof value !== "object" || value === null) {
    return false;
  }

  const candidate = value as Record<string, unknown>;

  return typeof candidate.title === "string"
    && candidate.title.trim().length > 0
    && typeof candidate.artist === "string"
    && candidate.artist.trim().length > 0
    && typeof candidate.year === "number"
    && Number.isInteger(candidate.year)
    && typeof candidate.price === "number"
    && Number.isFinite(candidate.price)
    && typeof candidate.image_url === "string"
    && candidate.image_url.trim().length > 0;
}