using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPos.Model
{
    class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Category(int n, string str)
        {
            CategoryId = n;
            CategoryName = str;
        }
    }
}
