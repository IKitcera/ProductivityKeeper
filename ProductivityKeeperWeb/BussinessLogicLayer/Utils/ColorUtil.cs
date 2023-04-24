using ProductivityKeeperWeb.Models.TaskRelated;
using System.Drawing;

namespace ProductivityKeeperWeb.BussinessLogicLayer.Utils
{
    public static class ColorUtil
    {
        public  static string GenerateColorHex()
        {
            System.Random r = new System.Random();
            var color = Color.FromArgb((ushort)r.Next(0, 256), (ushort)r.Next(0, 256), (ushort)r.Next(0, 256), (ushort)r.Next(50, 256));
            return ColorTranslator.ToHtml(color);
        }
    }
}
