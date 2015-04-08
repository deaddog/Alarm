using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alarm
{
    public class PrettierLabel : Control
    {
        private Font font2;
        private Color color2;
        private string text2;

        public PrettierLabel()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint,
                true);
        }

        public Font Font2
        {
            get { return font2; }
            set { font2 = value; this.Invalidate(); }
        }
        public Color ForeColor2
        {
            get { return color2; }
            set { color2 = value; this.Invalidate(); }
        }
        public string Text2
        {
            get { return text2; }
            set { text2 = value; this.Invalidate(); }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnTextChanged(e);
        }
        protected override void OnFontChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnFontChanged(e);
        }
        protected override void OnForeColorChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnForeColorChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var size = e.Graphics.MeasureString(this.Text, this.Font);
            var point = new PointF((this.Width - size.Width) / 2f, (this.Height - size.Height) / 2f);

            //e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            using (SolidBrush brush = new SolidBrush(this.ForeColor))
                e.Graphics.DrawString(this.Text, this.Font, brush, point);

            if (font2 != null && text2 != null)
                using (SolidBrush brush = new SolidBrush(color2))
                    e.Graphics.DrawString(text2, font2, brush, new PointF(0, 14));

            base.OnPaint(e);
        }
    }
}
