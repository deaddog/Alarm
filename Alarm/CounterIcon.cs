using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alarm
{
    public class CounterIcon : IDisposable
    {
        private Timer timer;
        private NotifyIcon icon;
        private int count;
        private bool work;
        private readonly Color initialColor;
        private Color color;
        private Bitmap[] numbers = new Bitmap[10];

        public CounterIcon(NotifyIcon icon)
        {
            this.timer = new Timer();
            this.timer.Tick += timer_Tick;
            this.icon = icon;

            this.Count = 0;
            this.work = true;
            this.Work = false;
            this.color = this.initialColor = Properties.Resources._0.GetPixel(2, 2);
        }

        public void Start(TimeSpan time, bool work)
        {
            if (time.TotalMilliseconds < 0)
                throw new ArgumentOutOfRangeException("time");

            this.timespan = time;
            start = DateTime.Now;
            this.Work = work;
            timer.Start();
        }

        public void Pause()
        {
            timer.Enabled = !timer.Enabled;
            if (!timer.Enabled) timespan = remaining;
            else start = DateTime.Now;
        }

        private TimeSpan remaining;
        public TimeSpan Remaining
        {
            get { return remaining; }
        }

        public event EventHandler Tick, Elapsed;

        public int Count
        {
            get { return count; }
            private set
            {
                if (count == value)
                    return;
                else
                {
                    count = value;
                    setIcon(icon, count, work);
                }
            }
        }
        public bool Work
        {
            get { return work; }
            private set
            {
                if (work == value)
                    return;
                else
                {
                    work = value;
                    setIcon(icon, count, work);
                }
            }
        }
        public Color Color
        {
            get { return color; }
            set
            {
                if (color == value)
                    return;

                var temp = numbers;
                numbers = new Bitmap[10];
                for (int i = 0; i < temp.Length; i++)
                    if (temp[i] != null) temp[i].Dispose();
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = CharSet.Auto)]
        extern static bool DestroyIcon(IntPtr handle);

        private void setIcon(NotifyIcon notifyIcon, int count, bool work)
        {
            Icon icon;
            using (Bitmap myBitmap = new Bitmap(16, 16, System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
            {
                Bitmap a = getImage((count % 100) / 10);
                Bitmap b = getImage(count % 10);

                using (Graphics graphics = Graphics.FromImage(myBitmap))
                {
                    if (count / 10 != 0 || count >= 100)
                    {
                        int x = 8 - (a.Width + b.Width - 1) / 2;
                        graphics.DrawImage(a, x, 3);
                        graphics.DrawImage(b, x + a.Width - 1, 3);
                    }
                    else
                    {
                        int x = 8 - b.Width / 2;
                        graphics.DrawImage(b, x, 3);
                    }
                    graphics.DrawImage(work ? Properties.Resources.work : Properties.Resources._break, 2, 12);
                }
                icon = Icon.FromHandle(myBitmap.GetHicon());
            }

            notifyIcon.Icon = icon;

            DestroyIcon(icon.Handle);
        }
        private Bitmap getImage(int c)
        {
            if (color == initialColor)
                return getBaseImage(c);

            else if (c >= numbers.Length)
                return null;

            else if (numbers[c] == null)
            {

            }

            return numbers[c];
        }
        private static Bitmap getBaseImage(int c)
        {
            switch (c)
            {
                case 0: return Properties.Resources._0;
                case 1: return Properties.Resources._1;
                case 2: return Properties.Resources._2;
                case 3: return Properties.Resources._3;
                case 4: return Properties.Resources._4;
                case 5: return Properties.Resources._5;
                case 6: return Properties.Resources._6;
                case 7: return Properties.Resources._7;
                case 8: return Properties.Resources._8;
                case 9: return Properties.Resources._9;
                default: return null;
            }
        }

        private DateTime start;
        private TimeSpan timespan;
        private void timer_Tick(object sender, EventArgs e)
        {
            var diff = DateTime.Now - start;

            remaining = timespan - diff;

            if (remaining.TotalSeconds >= 61)
                Count = (int)Math.Round(remaining.TotalMinutes);

            else if (remaining.TotalSeconds > 0)
                Count = (int)remaining.TotalSeconds;
            else
            {
                remaining = new TimeSpan(0);
                Count = 0;
                timer.Stop();
            }

            if (Tick != null)
                Tick(this, EventArgs.Empty);

            if (!timer.Enabled && Elapsed != null)
                Elapsed(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            timer.Stop();
            timer.Dispose();
        }
    }
}
