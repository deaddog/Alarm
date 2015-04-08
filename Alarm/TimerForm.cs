using DeadDog.Audio.Playback;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Alarm
{
    public partial class TimerForm : Form
    {
        private bool canclose = false;
        private CounterIcon icon;
        private AudioControl<string> player;

        public TimerForm()
        {
            InitializeComponent();

            panel1.BackColor = textBox1.BackColor = Color.FromArgb(30, 30, 30);

            this.icon = new CounterIcon(this.notifyIcon1);
            this.icon.Tick += icon_Tick;
            this.icon.Elapsed += icon_Elapsed;

            this.player = new AudioControl<string>(x => x);

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
                this.textBox1.Text = "break -t 5";
            else
                this.textBox1.Text = "work -t 25";

            this.textBox1.Focus();

            this.Left = Cursor.Position.X - this.Width / 2;
            this.Top = Cursor.Position.Y - this.Height / 2;
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

                this.Left = Cursor.Position.X - this.Width / 2;
                if (this.Left + this.Width > Screen.PrimaryScreen.WorkingArea.Width)
                    this.Left = Screen.PrimaryScreen.WorkingArea.Right - this.Width - 5;
                this.Top = Screen.PrimaryScreen.WorkingArea.Bottom - this.Height - 5;
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
                prettierLabel1.Text2 = message.GetMessage();
            else if (text != "quit")
            {
                prettierLabel1.Text2 = "";
                player.Open("start.mp3");
                player.Play();
            }

            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }
}
