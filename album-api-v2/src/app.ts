import cors from "cors";
import express from "express";
import {
  createAlbum,
  deleteAlbum,
  getAlbum,
  isAlbumInput,
  listAlbums,
  updateAlbum,
  type AlbumSortKey
} from "./albums.js";

export const app = express();

const validSortKeys = new Set(["title", "artist", "price"]);

app.use(cors());
app.use(express.json());

app.get("/", (_request, response) => {
  response.send("Hit the /albums endpoint to retrieve albums.");
});

app.get("/albums", (request, response) => {
  const sortBy = typeof request.query.sortBy === "string" ? request.query.sortBy.toLowerCase() : undefined;
  const yearQuery = typeof request.query.year === "string" ? request.query.year : undefined;

  if (sortBy !== undefined && sortBy !== "" && !validSortKeys.has(sortBy)) {
    response.status(400).send("sortBy must be title, artist, or price.");
    return;
  }

  const year = yearQuery === undefined || yearQuery === "" ? undefined : Number(yearQuery);

  if (year !== undefined && !Number.isInteger(year)) {
    response.status(400).send("year must be an integer.");
    return;
  }

  response.json(listAlbums({
    sortBy: sortBy === "" ? undefined : sortBy as AlbumSortKey | undefined,
    year
  }));
});

app.get("/albums/:id", (request, response) => {
  const id = Number(request.params.id);

  if (!Number.isInteger(id)) {
    response.sendStatus(404);
    return;
  }

  const album = getAlbum(id);

  if (!album) {
    response.sendStatus(404);
    return;
  }

  response.json(album);
});

app.post("/albums", (request, response) => {
  if (!isAlbumInput(request.body)) {
    response.status(400).send("Album is required.");
    return;
  }

  const createdAlbum = createAlbum(request.body);

  response.status(201).location(`/albums/${createdAlbum.id}`).json(createdAlbum);
});

app.put("/albums/:id", (request, response) => {
  const id = Number(request.params.id);

  if (!Number.isInteger(id)) {
    response.sendStatus(404);
    return;
  }

  if (!isAlbumInput(request.body)) {
    response.status(400).send("Album is required.");
    return;
  }

  const updatedAlbum = updateAlbum(id, request.body);

  if (!updatedAlbum) {
    response.sendStatus(404);
    return;
  }

  response.json(updatedAlbum);
});

app.delete("/albums/:id", (request, response) => {
  const id = Number(request.params.id);

  if (!Number.isInteger(id) || !deleteAlbum(id)) {
    response.sendStatus(404);
    return;
  }

  response.sendStatus(204);
});