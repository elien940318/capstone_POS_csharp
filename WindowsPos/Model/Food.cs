using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPos.Model
{
    public class Food
    {
        public int ProductCode { get; set; }
        public string ProductName { get; set; }
        public int ProductPrice { get; set; }
        public int CategoryNum { get; set; }
        public string CategoryName { get; set; }

        public Food(int code, string name, int price, int catnum)
        {
            ProductCode = code;
            ProductName = name;
            ProductPrice = price;
            CategoryNum = catnum;
        }
        public Food(int code, string name, int price, int catnum, string catName) : this(code,name,price,catnum)
        {
            CategoryName = catName;
        }

        public Food(DataRow drRow)
        {
            ProductCode  = Int32.Parse(drRow[0].ToString());
            ProductName  = drRow[1].ToString();
            ProductPrice = Int32.Parse(drRow[2].ToString());
            CategoryNum  = Int32.Parse(drRow[3].ToString());
        }
    }
}
