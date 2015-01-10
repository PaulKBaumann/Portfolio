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


namespace Music_Manager
{
    public partial class CommandLine : Form
    {

        /**
         * The CommandLine object is a very simple windows form that is used to send commands to the music manager. It is created whenever a specific hotkey is pressed - CTRL+ALT+SPACEBAR
         * Commands typed in here are sent to CommandParser to be interpreted and executed.
         * The window is destroyed when a commanded is entered with the Enter key or the Escape key is pressed.
         * 
         * */
        private Library library;
        public CommandLine(Library library)
        {
            this.library = library;
            InitializeComponent();
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void CommandLine_Load(object sender, EventArgs e)
        {
            commandLineBox.Focus();
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void CommandLine_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true; 
                this.Close();
            }
            if (e.KeyCode == Keys.Enter)
            {
                String command = commandLineBox.Text;
                library.processCommand(command);
                e.Handled = true;
                this.Close();
            }
        }
    }
}
