using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityKeeperWeb.Models.TaskRelated
{
    [Owned]
    public class Color
    {
        public int Id { get; set; }
        public ushort R { get; set; }
        public ushort G { get; set; }
        public ushort B { get; set; }
        public ushort A { get; set; }
        public Color()
        {

        }

        public Color(ushort r, ushort g, ushort b)
        {
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public Color(ushort r, ushort g, ushort b, ushort a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
