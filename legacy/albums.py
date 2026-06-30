#!/usr/bin/env python3
# Copyright (c) Microsoft Corporation.
# SPDX-License-Identifier: MIT
"""Read fixed-width album records from ALBUMS.DAT.

This script is a Python conversion of the COBOL album file layout in
``albums.cbl``. It reads line-sequential records where each album contains a
seven-digit id, fixed-width artist and title fields, a release date, and a
genre.

Example:
    Run with the default COBOL data file name::

        python legacy/albums.py

    Run with a specific input file::

        python legacy/albums.py path/to/ALBUMS.DAT
"""

from __future__ import annotations

import argparse
import logging
import sys
from datetime import date
from pathlib import Path
from typing import NamedTuple

EXIT_SUCCESS = 0
EXIT_FAILURE = 1
EXIT_ERROR = 2

DEFAULT_ALBUM_FILE = Path("ALBUMS.DAT")
MONTH_NAMES = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
]
RECORD_LENGTH = 53

logger = logging.getLogger(__name__)


class Album(NamedTuple):
    """Album record converted from the COBOL fixed-width layout.

    Args:
        album_id: Seven-digit album identifier.
        artist: Artist name from the fixed-width artist field.
        title: Album title from the fixed-width title field.
        release_date: Album release date.
        genre: Album genre from the fixed-width genre field.
    """

    album_id: int
    artist: str
    title: str
    release_date: date
    genre: str


def create_parser() -> argparse.ArgumentParser:
    """Create and configure the command-line argument parser.

    Returns:
        Configured argument parser for this script.
    """
    parser = argparse.ArgumentParser(
        description="Read fixed-width album records from ALBUMS.DAT."
    )
    parser.add_argument(
        "album_file",
        nargs="?",
        type=Path,
        default=DEFAULT_ALBUM_FILE,
        help="Path to the line-sequential album data file.",
    )
    parser.add_argument(
        "--verbose",
        action="store_true",
        help="Enable debug logging.",
    )
    return parser


def configure_logging(verbose: bool = False) -> None:
    """Configure logging for the script.

    Args:
        verbose: If True, emit debug-level log messages.
    """
    level = logging.DEBUG if verbose else logging.INFO
    logging.basicConfig(level=level, format="%(levelname)s: %(message)s")


def parse_album_record(record: str) -> Album:
    """Parse one COBOL album record into an ``Album`` instance.

    Args:
        record: Fixed-width album record without the trailing newline.

    Returns:
        Parsed album record.

    Raises:
        ValueError: If the record is too short or contains an invalid date.
    """
    if len(record) < RECORD_LENGTH:
        raise ValueError(
            f"Album record must be at least {RECORD_LENGTH} characters; "
            f"received {len(record)}."
        )

    album_id_text = record[0:7]
    artist = record[7:15].rstrip()
    title = record[15:35].rstrip()
    year = int(record[35:39])
    month = int(record[39:41])
    day = int(record[41:43])
    genre = record[43:53].rstrip()

    return Album(
        album_id=int(album_id_text),
        artist=artist,
        title=title,
        release_date=date(year, month, day),
        genre=genre,
    )


def read_albums(album_file: Path) -> list[Album]:
    """Read all album records from a line-sequential data file.

    Args:
        album_file: Path to the album data file.

    Returns:
        List of parsed album records.

    Raises:
        FileNotFoundError: If the album file does not exist.
        ValueError: If any record cannot be parsed.
    """
    albums: list[Album] = []

    with album_file.open("r", encoding="utf-8") as file:
        for line_number, line in enumerate(file, start=1):
            record = line.rstrip("\r\n")
            if not record:
                logger.debug("Skipping blank line %s", line_number)
                continue

            try:
                albums.append(parse_album_record(record))
            except ValueError as exc:
                raise ValueError(f"Invalid album record on line {line_number}: {exc}") from exc

    return albums


def format_album(album: Album) -> str:
    """Format one album for console output.

    Args:
        album: Album record to format.

    Returns:
        Human-readable album summary.
    """
    month_name = MONTH_NAMES[album.release_date.month - 1]
    release_date_text = (
        f"{month_name} {album.release_date.day}, {album.release_date.year}"
    )
    return (
        f"{album.album_id:07d} | {album.artist} | {album.title} | "
        f"{release_date_text} | {album.genre}"
    )


def run(album_file: Path) -> int:
    """Read and print album records from the supplied file.

    Args:
        album_file: Path to the album data file.

    Returns:
        Process exit code.
    """
    albums = read_albums(album_file)

    for album in albums:
        print(format_album(album))

    return EXIT_SUCCESS


def main() -> int:
    """Main entry point with top-level error handling.

    Returns:
        Process exit code.
    """
    parser = create_parser()
    args = parser.parse_args()
    configure_logging(args.verbose)

    try:
        return run(args.album_file)
    except FileNotFoundError:
        print(f"Error: file not found: {args.album_file}", file=sys.stderr)
        return EXIT_ERROR
    except KeyboardInterrupt:
        print("\nInterrupted by user", file=sys.stderr)
        return 130
    except BrokenPipeError:
        sys.stderr.close()
        return EXIT_FAILURE
    except Exception as exc:
        print(f"Error: {exc}", file=sys.stderr)
        return EXIT_FAILURE


if __name__ == "__main__":
    sys.exit(main())
