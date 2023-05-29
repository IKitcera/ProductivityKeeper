using System.Drawing;

namespace ProductivityKeeperWeb.Domain.Utils
{
    public static class ColorUtil
    {
        public static string GenerateColorHex()
        {
            System.Random r = new System.Random();
            var color = Color.FromArgb((ushort)r.Next(0, 256), (ushort)r.Next(0, 256), (ushort)r.Next(0, 256), (ushort)r.Next(50, 256));
            return ColorTranslator.ToHtml(color);
        }

        public static string GenerateTextColorHex(string color)
        {
            var rgb = ColorTranslator.FromHtml(color);
            var brightness = (0.2126 * rgb.R + 0.7152 * rgb.G + 0.0722 * rgb.B) / 255;
            var resColor = brightness > 0.5 ? Color.Black : Color.White;
            return ColorTranslator.ToHtml(resColor);
        }
    }
}
