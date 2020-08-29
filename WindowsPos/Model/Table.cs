using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsPos.Model;

namespace WindowsPos.Model
{
    public class Table
    {
        public Table()
        {
            TableNum = -1;
            XPos = 50;
            YPos = 50;
        }

        public Table(int num, int x, int y)
        {
            TableNum = num;
            XPos = x;
            YPos = y;
        }
        public int TableNum { get; set; }
        public int XPos { get; set; }
        public int YPos { get; set; }
    }
}
