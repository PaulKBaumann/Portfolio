using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HotKeys;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace Music_Manager
{

    /**
     * The Library is a form that shows all of the available music. It also handles incoming commands from hotkeys or the command console.
     * 
     * 
     * */


    public partial class Library : Form
    {
        private HotKeys.GlobalHotkey ghk_space, ghk_numplus, ghk_numminus;
        private CommandLine cl;
        public BindingList<Song> Songs;
        private int id = 1;
        public SQLiteInterface sql;
        public MyPlaylist master;
        private List<MyPlaylist> playlists;
        public AxWMPLib.AxWindowsMediaPlayer MediaPlayer;

        public Library()
        {
            InitializeComponent();
            Debug.Print("Component initialized");
            ghk_space = new HotKeys.GlobalHotkey(Constants.CTRL + Constants.ALT, Keys.Space, this);
            ghk_numplus = new HotKeys.GlobalHotkey(Constants.CTRL + Constants.ALT, Keys.Add, this);
            ghk_numminus = new HotKeys.GlobalHotkey(Constants.CTRL + Constants.ALT, Keys.Subtract, this);

            MediaPlayer = new AxWMPLib.AxWindowsMediaPlayer();
            playlists = new List<MyPlaylist>();
            sql = new SQLiteInterface();
            master = new MyPlaylist(MediaPlayer, sql);
            createListOfSongs();
        }

        private void OpenConsole()
        {
            cl = new CommandLine(this);
            cl.Activate();
        }

        private void PlayPause()
        {
            if (master.playing)
            {
                master.Pause();
            }
            else
            {
                master.Play();
            }
        }

        private void Skip()
        {
            master.Skip();
        }

        private Keys GetKey(IntPtr LParam)
        {
            return (Keys)((LParam.ToInt32()) >> 16);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == HotKeys.Constants.WM_HOTKEY_MSG_ID)
            {
                switch (GetKey(m.LParam))
                {
                    case Keys.Space:
                        OpenConsole();
                        break;
                    case Keys.Add:
                        Skip();
                        break;
                    case Keys.Subtract:
                        PlayPause();
                        break;
                }
            }

            base.WndProc(ref m);
        }

        private void Library_Load(object sender, EventArgs e)
        {
            //WriteLine("Trying to register hotkey");
            if (ghk_space.Register())
            {
                Debug.Print("registered");
            }
            else
            {
                MessageBox.Show("failed to register");
            }
            ghk_numplus.Register();
            ghk_numminus.Register();
            

            setupSongTable();
        }

        private void Library_FormClosing(object sender, EventArgs e)
        {
            if (!ghk_space.Unregiser())
            {
                MessageBox.Show("Failed to unregister hotkey space!");
            }
            if (!ghk_numplus.Unregiser())
            {
                MessageBox.Show("Failed to unregister hotkey +!");
            }
            if (!ghk_numminus.Unregiser())
            {
                MessageBox.Show("Failed to unregister hotkey -!");
            }
            sql.close();
        }

        private void SongTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void SongTable_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            master.Play(e.RowIndex);
        }

        private void setupSongTable()
        {
            SongTable.DataSource = Songs;
        }

        private void createListOfSongs()
        {
            Songs = new BindingList<Song>();
            foreach (Song s in sql.pullLibrary())
            {
                Songs.Add(s);
                master.AddSong(s);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBD = new FolderBrowserDialog();
            String folderName = "";
            DialogResult result = folderBD.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderName = folderBD.SelectedPath;
                searchMusicFiles(folderName);
            }
            
        }

        private void searchMusicFiles(String directory)
        {
            try
            {
                foreach (String f in Directory.GetFiles(directory))
                {
                    AddToLibrary(f);
                }
                foreach (String d in Directory.GetDirectories(directory))
                {
                    searchMusicFiles(d);
                }
            }
            catch (System.Exception e)
            {
                Debug.Print(e.Message);
            }
        }

        private void AddToLibrary(string file)
        {
            if (file.EndsWith(".mp3")){
                Song song = new Song(file);
                foreach (Song s in Songs)
                {
                    if (song.matches(s)){
                        return; 
                    }
                }

                if (sql.writeSongToDatabase(song))
                { Songs.Add(song); }
                
            }
        }

        public void processCommand(String command)
        {
            CommandParser.ParseCommand(this, command);
        }

        public void setNewPlaylist(List<Song> songs)
        {
            Songs.Clear();
            //Songs = new BindingList<Song>();
            master.DeletePlaylist();
            foreach (Song s in songs)
            {
                Songs.Add(s);
                master.AddSong(s);
            }
        }

        private void commandConsoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenConsole();
        }

        private void playPauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlayPause();
        }

        private void skipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Skip();
        }
    }
}
