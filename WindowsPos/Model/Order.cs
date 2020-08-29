using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsPos.Model
{
    public class Order
    {
        // sale_no | sale_time | sale_count | sale_discount | sale_totprc | seat_no | pro_code | usr_id 

        public int Sale_No { get; set; }
        public DateTime Sale_Time { get; set; }
        public int Sale_Count { get; set; }
        public int Sale_Discount { get; set; }
        public int Sale_TotalPrice { get; set; }

        public Order()
        {

        }
    }
}
