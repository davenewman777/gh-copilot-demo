$ SET SOURCEFORMAT"FREE"

IDENTIFICATION DIVISION.
PROGRAM-ID. MonthTable.
AUTHOR. Michael Coughlan.

*> Defines the external album data file used by the program.
*> The file is read as a line-sequential text file from ALBUMS.DAT.
ENVIRONMENT DIVISION.
INPUT-OUTPUT SECTION.
FILE-CONTROL.
    SELECT AlbumFile ASSIGN TO "ALBUMS.DAT"
        ORGANIZATION IS LINE SEQUENTIAL.

DATA DIVISION.
FILE SECTION.

*> Describes one album record read from ALBUMS.DAT.
*> Each record contains the album id, artist, title, release date, and genre.
FD AlbumFile.
01 AlbumDetails.

   *> Condition name used to detect the logical end of the album file.
   88 EndOfAlbumFile VALUE HIGH-VALUES.

   *> Seven-digit unique album identifier.
   02 AlbumId PIC 9(7).

   *> Album name details, split into artist and title fields.
   02 AlbumName.

       *> Artist name, stored as up to 8 characters.
       03 Artist PIC X(8).

       *> Album title, stored as up to 20 characters.
       03 Title PIC X(20).

   *> Album release date stored as separate year, month, and day fields.
   02 ReleaseDate.

       *> Four-digit year of release.
       03 YORelease PIC 9(4).

       *> Two-digit month of release.
       03 MORelease PIC 9(2).

       *> Two-digit day of release.
       03 DORelease PIC 9(2).

   *> Album genre, stored as up to 10 characters.
   02 Genre PIC X(10).

WORKING-STORAGE SECTION.

*> Lookup table containing month names.
*> The table stores two fixed-width month names per row.
01 MonthTable.
   02 TableValues.
      03 FILLER PIC X(18) VALUE "January  February".
      03 FILLER PIC X(18) VALUE "March    April".
      03 FILLER PIC X(18) VALUE "May      June".
      03 FILLER PIC X(18) VALUE "July     August".
      03 FILLER PIC X(18) VALUE "SeptemberOctober".
      03 FILLER PIC X(18) VALUE "November December".