---
title: Album API v2
description: Setup and usage instructions for the TypeScript music albums API
---

## Album API v2

`album-api-v2` is a TypeScript Node.js API for managing music albums. It rewrites
the previous .NET `albums-api` service with in-memory data and routes that match
the existing Vue app calls.

The API runs on port `3000` by default. The Vue app already proxies `/albums` to
`http://localhost:3000` during local development.

## Requirements

* Node.js
* npm

## Install dependencies

From the `album-api-v2` folder, install packages:

```bash
npm install
```

## Run the API

Build the TypeScript project, then start the compiled API:

```bash
npm run build
npm start
```

The server listens at `http://localhost:3000`.

For local development with automatic restart, run:

```bash
npm run dev
```

## Scripts

* `npm run build` compiles TypeScript into `dist/`
* `npm start` starts the compiled API from `dist/server.js`
* `npm run dev` starts the TypeScript server in watch mode
* `npm test` runs the Vitest unit test suite

## Routes

| Method | Route        | Description                     |
|--------|--------------|---------------------------------|
| GET    | `/`          | Returns a basic API message     |
| GET    | `/albums`    | Lists albums                    |
| GET    | `/albums/:id` | Gets one album by ID            |
| POST   | `/albums`    | Adds an album                   |
| PUT    | `/albums/:id` | Updates an existing album by ID |
| DELETE | `/albums/:id` | Deletes an album by ID          |

## Query parameters

`GET /albums` supports the same query behavior as the previous .NET API:

* `sortBy=title` sorts albums by title
* `sortBy=artist` sorts albums by artist name
* `sortBy=price` sorts albums by price
* `year=2021` filters albums by release year

Unsupported `sortBy` values return `400` with this message:

```text
sortBy must be title, artist, or price.
```

## Album shape

Album responses use the flattened shape expected by the Vue app:

```json
{
  "id": 1,
  "title": "You, Me and an App Id",
  "artist": "Daprize",
  "year": 2021,
  "price": 10.99,
  "image_url": "https://aka.ms/albums-daprlogo"
}
```

The API starts with the same six sample albums from the previous .NET API. Data
is stored in memory and resets when the server restarts.

## Validate the API

Run the build and unit tests:

```bash
npm run build
npm test
```

Start the API and confirm the seeded album collection responds:

```bash
npm start
curl http://localhost:3000/albums
```

The `/albums` response should contain six albums.