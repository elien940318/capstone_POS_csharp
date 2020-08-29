using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPos.Model
{
    class Food
    {
        int product_code;
        string product_name;
        int product_price;
        int category;

        public Food(int code, string name, int price, int cat)
        {
            product_code = code;
            product_name = name;
            product_price = price;
            category = cat;
        }

        public int ProductCode
        {
            get { return product_code; }
            set { product_code = value; }            
        }
        public string ProductName
        {
            get { return product_name; }
            set { product_name = value; }
        }
        public int ProductPrice
        {
            get { return product_price; }
            set { product_price = value; }
        }
        public int Category
        {
            get { return category; }
            set { category = value; }
        }
    }
}
