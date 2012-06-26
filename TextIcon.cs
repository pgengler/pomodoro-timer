using System;
using System.Drawing;

namespace Pomodoro
{
    class TextIcon
    {
        private Brush brush = new SolidBrush(Color.Black);
        private Bitmap bitmap = new Bitmap(16, 16);
        private Graphics graphics;
        private Font font = new Font(FontFamily.GenericSansSerif, 8);
        private Icon icon;

        public TextIcon(string text) {
            graphics = Graphics.FromImage(bitmap);
            graphics.DrawString(text, font, brush, 0, 0);
            IntPtr hIcon = bitmap.GetHicon();
            icon = Icon.FromHandle(hIcon);
        }

        public Icon get()
        {
            return icon;
        }

        ~TextIcon()
        {
            icon = null;
            font = null;
            graphics = null;
            bitmap = null;
            brush = null;
        }

    }
}
