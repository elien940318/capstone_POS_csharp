using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsPos.Model;

namespace WindowsPos.View
{
    public partial class OrderPage : Page
    {
        int cnt;
        int price;
        int discprice;

        public OrderPage()
        {
            InitializeComponent();
        }
        public OrderPage(TableButton btn) : this()
        {
            cnt = 0;
            price = 0;
            discprice = 0;


            using (MySqlConnection connection = new MySqlConnection("Server=175.200.94.253;Port=3306;Database=capstone;Uid=capstone;Pwd=capstone"))
            {
                try
                {
                    connection.Open();

                    //| seat_no | seat_xpos | seat_ypos | usr_id | seat_totprc |
                    string query = "select c.pro_name, b.sale_count, b.sale_totprc, b.sale_discount, DATE_FORMAT(b.sale_time, '%H:%i:%s') sale_datetime " +
                        "from seat a join (sale b join product c on b.pro_code = c.pro_code) on a.seat_no = b.seat_no and a.seat_no=" + btn.TableNum + ";";

                    //MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    MySqlCommand command = new MySqlCommand(query, connection);
                    DataTable dt = new DataTable();

                    dt.Load(command.ExecuteReader());
                    orderList.DataContext = dt.DefaultView;

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void CategoryButtonOnClick(object sender, RoutedEventArgs e)
        {
            var temp = sender as CustomButton;
            // temp.Content.ToString();
            MenuListGrid.Children.Clear();

            List<Food> fdlst = new List<Food>();
            foreach (var item in MainSystem.GetInstance._menulist)
            {
                if (item.Category == (int)temp.Tag)
                    fdlst.Add(item);
            }
            foreach (var item in fdlst)
            {
                CustomButton btntemp = new CustomButton();
                btntemp.Content = item.ProductName;
                btntemp.Tag = item.ProductCode;
                btntemp.Click += MenuButtonOnClick;
                MenuListGrid.Children.Add(btntemp);
            }
            for (int i = 0; i < 25 - fdlst.Count; i++)
            {
                Border bdr = new Border();
                bdr.Background = Brushes.AliceBlue;
                bdr.CornerRadius = new CornerRadius(5);
                bdr.Margin = new Thickness(2);
                MenuListGrid.Children.Add(bdr);
            }
        }

        private void MenuButtonOnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as CustomButton;

            foreach (var item in MainSystem.GetInstance._menulist)
            {
                if ((int)btn.Tag == item.ProductCode)
                {
                    GridViewData temp = new GridViewData(++cnt, item.ProductName.ToString(),
                        1, item.ProductPrice, 0);
                    orderList.Items.Add(temp);
                    price += item.ProductPrice;
                    txtPrice.Content = price;
                    txtTotalPrice.Content = price - discprice;
                    return;
                }
            }
        }

        private void ButtonCloseOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }

    class GridViewData
    {
        public int Num { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }

        public GridViewData(int n, string pn, int c, int p, int d)
        {
            Num = n;
            ProductName = pn;
            Count = c;
            Price = p;
            Discount = d;
        }
    }
}
