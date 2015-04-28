using CommandLineParsing;
using DeadDog.Audio.Playback;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Stufkan.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace Alarm
{
    public partial class TimerForm : Form
    {
        private const int MARGIN = 5;
        private bool canclose = false;
        private CounterIcon icon;
        private AudioControl<string> player;

        List<GlobalHotkey> hotkeys;

        public TimerForm()
        {
            InitializeComponent();

            panel1.BackColor = textBox1.BackColor = Color.FromArgb(30, 30, 30);

            this.icon = new CounterIcon(this.notifyIcon1);
            this.icon.Tick += icon_Tick;
            this.icon.Elapsed += icon_Elapsed;

            this.player = new AudioControl<string>(x => x);

            this.Load+=TimerForm_Load;
            hotkeys = new List<GlobalHotkey>();
            hotkeys.Add(new GlobalHotkey("Next", Constants.ALT + Constants.CTRL, Keys.A, this));

            this.Disposed += (s, e) => { icon.Dispose(); player.Dispose(); };
        }

        void icon_Tick(object sender, EventArgs e)
        {
            prettierLabel1.Text = icon.Remaining.ToString("mm\\:ss");
        }
        void icon_Elapsed(object sender, EventArgs e)
        {
            player.Open("stop.mp3");
            player.Play();

            this.Visible = true;
            icon_Tick(null, null);
            prettierLabel1.Text2 = "";
            if (icon.Work)
                this.textBox1.Text = MyCommand.BREAK_COMMAND + " 5";
            else
                this.textBox1.Text = MyCommand.WORK_COMMAND + " 25";

            this.textBox1.Focus();

            ensureInsideScreen(Cursor.Position);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            this.Visible = false;
            base.OnLostFocus(e);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !canclose;
            base.OnClosing(e);
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            e.Graphics.DrawRectangle(Pens.Black, 0, 0, this.Width - 1, this.Height - 1);
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Left)
                return;

            if (this.Visible)
                this.Visible = false;
            else
            {
                this.Visible = true;
                icon_Tick(null, null);
                prettierLabel1.Text2 = "";
                this.textBox1.Text = "";

                this.textBox1.Focus();

                ensureInsideScreen(Cursor.Position);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                this.Visible = false;
                return;
            }
            if (e.KeyCode != Keys.Enter)
                return;

            string text = textBox1.Text;
            textBox1.Text = "";

            var message = new MyCommand(this).ParseAndExecute(text);
            if (message.IsError)
                prettierLabel1.Text2 = ColorConsole.ClearColors(message.GetMessage());
            else
              prettierLabel1.Text2 = "";

            if (text == MyCommand.BREAK_COMMAND || text == MyCommand.WORK_COMMAND)
            {
                player.Open("start.mp3");
                player.Play();
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }

        private void ensureInsideScreen(Point cursorPosition)
        {
            this.Left = cursorPosition.X - this.Width / 2;
            if (this.Left + this.Width > Screen.PrimaryScreen.WorkingArea.Width)
                this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width - MARGIN;

            if (this.Left < MARGIN) this.Left = MARGIN;


            this.Top = cursorPosition.Y - this.Height / 2;

            if (this.Top > Screen.PrimaryScreen.WorkingArea.Bottom - this.Height - MARGIN)
                this.Top = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height - MARGIN;

            if (this.Top < MARGIN) this.Top = MARGIN;
        }

        //global hotkey
        public const int WM_HOTKEY_MSG_ID = 786;
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {

            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)

                switch (GetKey(m.LParam))
                {
                    case Keys.A:
                        showForm();
                        break;
                }

            base.WndProc(ref m);

        }

        private void TimerForm_Load(object sender, EventArgs e)
        {
            foreach (var hk in hotkeys)
            {
                if (hk.Register())
                    Debug.WriteLine(hk.Name + " successfully registered");
                else
                    Debug.WriteLine("Error registering hotkey \"" + hk.Name + "\"");
            }
        }

        private void showForm()
        {
            this.Visible = true;
            ensureInsideScreen(Cursor.Position);
        }

        private Keys GetKey(IntPtr LParam)
        {
            return (Keys)((LParam.ToInt32()) >> 16); // not all of the parenthesis are needed, I just found it easier to see what's happening
        }
    }
}
