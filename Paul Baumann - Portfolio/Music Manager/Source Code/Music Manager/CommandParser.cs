using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Manager
{
    class CommandParser
    {
        /**
         * ParseCommand is a static method contained in the CommandParser class. It is used to interpret commands given to the music library. 
         * 
         * More commands and support for multiple playlists will be added 
         * 
         * Commands:
         * 
         * Play [Artist <artist>] | [Album <album>] | [Song <song>]
         * Rating <rating>
         * 
         **/
        public static void ParseCommand(Library library, String command){

            Debug.Print("Command: " + command);

            //Splits by empty spaces
            string[] commands = command.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
            if (commands.Length > 0)
            {
                if (commands[0].ToLower().Equals("play"))
                {
                    ParsePlay(library, command);
                }
                if (commands[0].ToLower().Equals("rating"))
                {
                    ParseRating(library, commands);
                }
            }

        }

        public static void ParsePlay(Library library, string command)
        {
            string[] commands = command.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries);
            if (commands.Length > 1)
            {
                if (commands[1].ToLower().Equals("artist"))
                {
                    string artist = command.Remove(12);
                    artist = command.Replace(artist, "");
                    Debug.Print(artist);
                    library.master.PlayArtist(artist);
                }
                else if (commands[1].ToLower().Equals("album"))
                {
                    string album = command.Remove(11);
                    album = command.Replace(album, "");
                    library.master.PlayAlbum(album);
                }
                else if (commands[1].ToLower().Equals("song"))
                {
                    library.master.Play(commands[2]);
                }
            }
            else
            {
                library.master.Play();
            }
        }

        public static void ParseRating(Library library, String[] commands){
            
            try{
                Debug.Print("Attempting to get double");
                double rating = Double.Parse(commands[1]);
                Debug.Print("Got double");
                List<Song> newSongs = new List<Song>();

                foreach (Song song in library.sql.pullLibrary())
                {
                    Debug.Print("checking song: " + song.name);
                    if (song.rating >= rating)
                    {
                        Debug.Print("new song: " + song.name);
                        newSongs.Add(song);
                    }
                }
                library.setNewPlaylist(newSongs);
            } catch (Exception e){

            }
        }
    }
}
