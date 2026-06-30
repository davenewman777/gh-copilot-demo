import request from "supertest";
import { beforeEach, describe, expect, it } from "vitest";
import { app } from "./app.js";
import { resetAlbums, sampleAlbums, type AlbumInput } from "./albums.js";

const newAlbum: AlbumInput = {
  title: "Container Hearts Club Band",
  artist: "The Rolling Pods",
  year: 2024,
  price: 11.99,
  image_url: "https://aka.ms/albums-containerappslogo"
};

describe("album api", () => {
  beforeEach(() => {
    resetAlbums();
  });

  it("lists the seeded albums in the Vue-compatible shape", async () => {
    const response = await request(app).get("/albums").expect(200);

    expect(response.body).toEqual(sampleAlbums);
  });

  it("filters albums by year", async () => {
    await request(app).post("/albums").send(newAlbum).expect(201);

    const response = await request(app).get("/albums?year=2024").expect(200);

    expect(response.body).toEqual([{ ...newAlbum, id: 7 }]);
  });

  it("sorts albums by title, artist, and price", async () => {
    const byTitle = await request(app).get("/albums?sortBy=title").expect(200);
    const byArtist = await request(app).get("/albums?sortBy=artist").expect(200);
    const byPrice = await request(app).get("/albums?sortBy=price").expect(200);

    expect(byTitle.body.map((album: { title: string }) => album.title)).toEqual([
      "Lock Down Your Love",
      "Lost in Translation",
      "Scale It Up",
      "Seven Revision Army",
      "Sweet Container O' Mine",
      "You, Me and an App Id"
    ]);
    expect(byArtist.body.map((album: { artist: string }) => album.artist)).toEqual([
      "Daprize",
      "Guns N Probeses",
      "KEDA Club",
      "MegaDNS",
      "The Blue-Green Stripes",
      "V is for VNET"
    ]);
    expect(byPrice.body.map((album: { price: number }) => album.price)).toEqual([
      10.99,
      12.99,
      12.99,
      13.99,
      13.99,
      14.99
    ]);
  });

  it("rejects unsupported sort fields", async () => {
    const response = await request(app).get("/albums?sortBy=year").expect(400);

    expect(response.text).toBe("sortBy must be title, artist, or price.");
  });

  it("gets an album by id", async () => {
    const response = await request(app).get("/albums/1").expect(200);

    expect(response.body).toEqual(sampleAlbums[0]);
  });

  it("returns not found for a missing album", async () => {
    await request(app).get("/albums/404").expect(404);
  });

  it("creates an album with the next id", async () => {
    const response = await request(app).post("/albums").send({ ...newAlbum, id: 100 }).expect(201);

    expect(response.headers.location).toBe("/albums/7");
    expect(response.body).toEqual({ ...newAlbum, id: 7 });

    const listResponse = await request(app).get("/albums").expect(200);
    expect(listResponse.body).toHaveLength(7);
  });

  it("rejects invalid create bodies", async () => {
    const response = await request(app).post("/albums").send({ title: "Missing fields" }).expect(400);

    expect(response.text).toBe("Album is required.");
  });

  it("updates an existing album", async () => {
    const response = await request(app).put("/albums/2").send({ ...newAlbum, id: 100 }).expect(200);

    expect(response.body).toEqual({ ...newAlbum, id: 2 });
  });

  it("returns not found when updating a missing album", async () => {
    await request(app).put("/albums/404").send(newAlbum).expect(404);
  });

  it("rejects invalid update bodies", async () => {
    const response = await request(app).put("/albums/1").send({}).expect(400);

    expect(response.text).toBe("Album is required.");
  });

  it("deletes an album", async () => {
    await request(app).delete("/albums/1").expect(204);
    await request(app).get("/albums/1").expect(404);
  });

  it("returns not found when deleting a missing album", async () => {
    await request(app).delete("/albums/404").expect(404);
  });
});