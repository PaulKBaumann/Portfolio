using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Diagnostics;

namespace Music_Manager
{
    public class SQLiteInterface
    {
        /**
         * The SQLiteInterface class is used to consolidate all of the database work into one class. It is responsible for reading and writing song information to the database.
         * Included are functions that read the whole playlist from the database, write songs to the database, and update entries when a song is skipped or played to completion.
         * 
         * */
        SQLiteConnection conn;
        public SQLiteInterface()
        {
            Debug.Print("Creating SQLite interface");
            if (File.Exists("MusicLibrary.sqlite"))
            {
                Debug.Print("Database already exists: opening...");
                conn = new SQLiteConnection("Data Source=MusicLibrary.sqlite;Version=3;");
            }
            else
            {
                Debug.Print("Creating database...");
                SQLiteConnection.CreateFile("MusicLibrary.sqlite");
                conn = new SQLiteConnection("Data Source=MusicLibrary.sqlite;Version=3;");
                conn.Open();
                String sql = "CREATE TABLE songs (id int, name varchar(255), artist varchar(255), album varchar(255), filepath varchar(255), trackNumber int, genre varchar(255), playcount int, skipcount int)";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();

                conn.Close();
            }

        }
        public bool writeSongToDatabase(Song song)
        {
            try
            {
                Debug.Print("Opening connection");
                conn.Open();
                String sql = "insert into songs (id, name, album, artist, filepath, trackNumber, genre, playcount, skipcount) values ("
                    + song.id + ", '"
                    + song.name.Replace("'", "''") + "', '"
                    + song.artist.Replace("'", "''") + "', '"
                    + song.album.Replace("'", "''") + "', '"
                    + song.filepath.Replace("'", "''") + "', "
                    + song.trackNumber + ", '"
                    + song.genre.Replace("'", "''") + "', "
                    + song.playcount + ", "
                    + song.skipcount + ")";
                Debug.Print(sql);
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                Debug.Print("Executing statment");
                command.ExecuteNonQuery();
                Debug.Print("Closing Connection");
                conn.Close();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public List<Song> pullLibrary()
        {
            List<Song> songs = new List<Song>();
            try
            {
                conn.Open();
                Debug.Print("Opened connection to load songs");
                string sql = "select * from songs";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    int id = (int)reader["id"];
                    string name = (String)reader["name"];
                    string album = (String)reader["album"];
                    string artist = (String)reader["artist"];
                    string filepath = (String)reader["filepath"];
                    int trackNumber = (int)reader["trackNumber"];
                    string genre = (String)reader["genre"];
                    int playcount = (int)reader["playcount"];
                    int skipcount = (int)reader["skipcount"];
                    Debug.Print("Loaded song: " + name);
                    songs.Add(new Song(id, name, album, artist, filepath, trackNumber, genre, playcount, skipcount));
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
            finally
            {
                conn.Close();
            }

            return songs;
        }

        public void close()
        {
            conn.Close();
        }

        public void songSkipped(String filepath)
        {
            try
            {
                conn.Open();
                String sql = "update songs set playcount = playcount + 1, skipcount = skipcount + 1 where filepath = '" + filepath.Replace("'", "''") + "'";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public void songCompleted(String filepath)
        {
            try
            {
                conn.Open();
                String sql = "update songs set playcount = playcount + 1 where filepath = '" + filepath.Replace("'", "''") + "'";
                SQLiteCommand command = new SQLiteCommand(sql, conn);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }
            finally
            {
                conn.Close();
            }

        }

    }
}
