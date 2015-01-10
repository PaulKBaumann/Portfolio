using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;
using AxWMPLib;
using System.Diagnostics;

namespace Music_Manager
{

    /**
     * Replacement for the playlist class included in WMPLib. 
     * 
     * The playlist is customized to use the filepaths included in the Song objects that are passed in. The next song is played automatically.
     * The Play() and Lookup() functions are overloaded to allow for multiple ways to lookup songs.
     * 
     * */

    public class MyPlaylist
    {
        public bool SongEnded = true;
        private System.Windows.Forms.Timer CheckSong;
        private System.ComponentModel.IContainer play_components;
        public List<Song> songList { get; set; }
        private int Index = 0;
        public WMPLib.IWMPMedia temp;
        public AxWMPLib.AxWindowsMediaPlayer MediaPlayer;
        public bool playing;
        private SQLiteInterface sql;

        public MyPlaylist(AxWMPLib.AxWindowsMediaPlayer Player, SQLiteInterface sql)
        {
            this.sql = sql;
            MediaPlayer = Player;
            Index = 0;
            songList = new List<Song>();
            playing = false;

            this.play_components = new System.ComponentModel.Container();
            this.CheckSong = new System.Windows.Forms.Timer(this.play_components);
            this.CheckSong.Tick += new System.EventHandler(this.CheckSong_Tick);
            MediaPlayer.PlayStateChange +=
                new AxWMPLib._WMPOCXEvents_PlayStateChangeEventHandler(MediaPlayer_PlayStateChange);
            MediaPlayer.CreateControl();
            Volume = 100;
        }

        public void AddSongs(List<Song> songs)
        {
            foreach (Song song in songs){
                AddSong(song);
            }
        }

        public void AddSong(Song song)
        {
            songList.Add(song);
        }

        public int Volume
        {
            set { MediaPlayer.settings.volume = value; }
            get { return MediaPlayer.settings.volume; }
        }

        public void DeleteSong(Song song)
        {
            if (song.filepath == songList[Index].filepath)
            {
                MediaPlayer.Ctlcontrols.stop();
                Index--;
            }
            songList.Remove(song);
            MediaPlayer.Ctlcontrols.play();
        }

        public void DeletePlaylist()
        {
            MediaPlayer.Ctlcontrols.stop();
            songList.Clear();
            Index = 0;
        }

        public void Play()
        {
            if (songList.Count > 0){
                if (songList[Index] != null)
                {
                    MediaPlayer.URL = songList[Index].filepath;
                    playing = true;
                }
            }
        }

        public void Play(int Slot)
        {
            if (songList.Count >= Slot)
            {
                if (songList[Slot] != null)
                {
                    MediaPlayer.URL = songList[Slot].filepath;
                    Index = Slot;
                    playing = true;
                }
            }
        }

        public void Play(Song song)
        {
            int slot = Lookup(song);
            if (slot >= 0 && slot < songList.Count)
            {
                MediaPlayer.URL = songList[slot].filepath;
                playing = true;
            }
        }

        public void Play(String song)
        {
            int slot = Lookup(song);
            if (slot >= 0 && slot < songList.Count)
            {
                MediaPlayer.URL = songList[slot].filepath;
                playing = true;
            }
        }

        public void PlayArtist(String artist)
        {
            int slot = LookupArtist(artist);
            if (slot >= 0 && slot < songList.Count)
            {
                MediaPlayer.URL = songList[slot].filepath;
                playing = true;
            }
        }

        public void PlayAlbum(String album)
        {
            int slot = LookupAlbum(album);
            if (slot >= 0 && slot < songList.Count)
            {
                MediaPlayer.URL = songList[slot].filepath;
                playing = true;
            }
        }

        public void Pause()
        {
            MediaPlayer.Ctlcontrols.pause();
            playing = false;
        }

        public void Stop()
        {
            MediaPlayer.Ctlcontrols.stop();
            playing = false;
        }

        public void NextSong()
        {
            if (Index != songList.Count - 1)
            {
                Index++;
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.URL = songList[Index].filepath;
                MediaPlayer.Ctlcontrols.play();
            }
            else
            {
                Index = 0;
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.URL = songList[0].filepath;
                MediaPlayer.Ctlcontrols.play();
            }
        }

        public void PrevSong()
        {
            if (Index != 0)
            {
                Index--;
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.URL = songList[Index].filepath;
                MediaPlayer.Ctlcontrols.play();
            }
            else
            {
                Index = songList.Count - 1;
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.URL = songList[Index].filepath;
                MediaPlayer.Ctlcontrols.play();
            }
        }

        private void CheckSong_Tick(object sender, System.EventArgs e)
        {
            if (SongEnded)
            {
                NextSong();
                SongEnded = false;
                CheckSong.Stop();
            }
        }

        public void MediaPlayer_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
        {
            switch (MediaPlayer.playState)
            {
                case WMPLib.WMPPlayState.wmppsMediaEnded:
                    SongEnded = true;
                    String filepath = songList[Index].filepath;
                    sql.songCompleted(filepath);
                    CheckSong.Start();
                    break;
                default:
                    break;
            }
        }

        public int Lookup(Song song)
        {
            int r = -1;
            for (int i = 0; i < songList.Count; i++)
            {
                if (songList[i].filepath == song.filepath)
                {
                    r = i;
                }
            }
            return r;
        }

        public int Lookup(string song)
        {
            int r = -1;
            for (int i = 0; i < songList.Count; i++)
            {
                if (songList[i].name == song)
                {
                    r = i;
                    break;
                }
            }
            return r;
        }

        public int LookupArtist(string artist)
        {
            int r = -1;
            for (int i = 0; i < songList.Count; i++)
            {
                if (String.Compare(songList[i].artist, artist, true) == 0)
                
                {
                    r = i;
                    break;
                }
            }
            return r;
        }

        public int LookupAlbum(string album)
        {
            int r = -1;
            for (int i = 0; i < songList.Count; i++)
            {
                if (String.Compare(songList[i].album, album, true) == 0)
                {
                    r = i;
                    break;
                }
            }
            return r;
        }

        public void Skip(){
            String filepath = songList[Index].filepath;
            sql.songSkipped(filepath);
            NextSong();
        }
    }
}
