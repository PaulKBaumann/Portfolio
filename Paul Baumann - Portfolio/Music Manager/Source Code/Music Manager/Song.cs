using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace Music_Manager
{
    
    public class Song
    {
        /**
         * The Song class is used to manage all of the information about each song. This information is read from the database or taken from the .mp3 file using the TagLib library.
         * When songs are brought in from the database, a rating is generated based off of the number of times a song has been played to completion over the number of times the song was played.
         * The more often a song is skipped, the lower its rating will be. 
         * 
         * */
        static int globalIdCount = 0;

        public int id {get; set; }
        public string name { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string filepath { get; set; }
        public int trackNumber { get; set; }
        public string genre { get; set; }
        public int playcount { get; set; }
        public int skipcount { get; set; }
        public double rating { get; set; }

        public Song() { }

        public Song(int id, string name, string artist, string album, string filepath, int trackNumber, string genre, int playcount, int skipcount)
        {
            this.id = id;
            globalIdCount = Math.Max(globalIdCount, id) + 1;
            this.name = name;
            this.artist = artist;
            this.album = album;
            this.filepath = filepath;
            this.trackNumber = trackNumber;
            this.genre = genre;
            this.playcount = playcount;
            this.skipcount = skipcount;
            this.rating = 10 * (((double)playcount - (double)skipcount) / (double)playcount);
        }

        public Song(String filepath)
        {
            this.id = globalIdCount++;
            this.filepath = filepath;
            TagLib.File file = TagLib.File.Create(filepath);
            if (file == null) return;
            this.name = file.Tag.Title;
            if (this.name == null || this.name == "")
            {
                String[] folders = filepath.Split(Path.DirectorySeparatorChar);
                this.name = folders[folders.Length - 1];
            }
            this.artist = file.Tag.FirstPerformer;
            this.album = file.Tag.Album;
            try
            {
                this.trackNumber = (int)file.Tag.Track;
            }
            catch (Exception e) { }
            try
            {
                this.genre = file.Tag.Genres[0];
            }
            catch (Exception e) {
                this.genre = "";
            }
            this.playcount = 0;
            this.skipcount = 0;

        }

        public bool matches(Song song)
        {
            Debug.Print("Checking if null");
            if (song == null || name.Equals("")) return false;
            Debug.Print("Checking member variables");
            return this.name == song.name && this.artist == song.artist && this.album == song.album && compareFileSize(song);
        }
        public bool compareFileSize(Song song)
        {
            Debug.Print("Checking file size");
            FileInfo thisFile = new FileInfo(this.filepath);
            FileInfo thatFile = new FileInfo(song.filepath);
            return (thisFile.Length == thatFile.Length);
        }
    }
}
