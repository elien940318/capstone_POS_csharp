using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsPos.ViewModel;

namespace WindowsPos.Model
{
    public class Order : Notifier
    {
        // pro_name, sale_count, sale_totprc, sale_discount, order_date, pro_code, order_no

        public string ProName { get; set; }

        private int saleCount;
        public int SaleCount
        {
            get { return saleCount; }
            set 
            {
                saleCount = value;
                OnPropertyChanged("SaleCount");
                OnPropertyChanged("SaleTotPrice");
            }
        }

        private int saleDiscount;
        public int SaleDiscount
        {
            get { return saleDiscount; }
            set 
            {
                saleDiscount = value;
                OnPropertyChanged("SaleDiscount");
                OnPropertyChanged("SaleTotPrice");
            }
        }

        private int saleTotPrice;
        public int SaleTotPrice
        {
            get { return saleTotPrice; }
            set 
            {
                saleTotPrice = MainSystem.GetInstance._productList[ProName] * SaleCount - SaleDiscount;
                OnPropertyChanged("SaleTotPrice");
            }
        }

        public string OrderDate { get; set; }
        public int ProductCode { get; set; }
        public int OrderNo { get; set; }

        public Order(string pn, int sc, int sd, int st, string od, int pc, int on)
        {
            ProName = pn;
            SaleCount = sc;
            SaleDiscount = sd;
            SaleTotPrice = st;
            OrderDate = od;
            ProductCode = pc;
            OrderNo = on;
        }

        public Order(DataRow drRow)
        {
            ProName         = drRow[0].ToString();
            SaleCount       = (int)drRow[1];
            SaleDiscount    = (int)drRow[3];
            SaleTotPrice    = (int)drRow[2];
            OrderDate       = drRow[4].ToString();
            ProductCode     = (int)drRow[5];
            OrderNo         = (int)drRow[6];
        }
    }
}
