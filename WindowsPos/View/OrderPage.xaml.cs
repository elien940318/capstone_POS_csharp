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
        public int TableNum { get; set; }
        int cnt;
        int price;
        int discprice;

        DataTable dtOrderlist;
        MySqlConnection connection;

        string connStr = "Server=175.200.94.253;Port=3306;Database=capstone;Uid=capstone;Pwd=capstone";
        
        
        public OrderPage()
        {
            InitializeComponent();
        }

        public OrderPage(TableButton btn) : this()
        {
            cnt = 0;
            price = 0;
            discprice = 0;
            TableNum = btn.TableNum;

            using (connection = new MySqlConnection(connStr))
            {
                try
                {
                    connection.Open();

                    //| seat_no | seat_xpos | seat_ypos | usr_id | seat_totprc |
                    string query = "select c.pro_name, b.sale_count, b.sale_totprc, b.sale_discount, DATE_FORMAT(b.sale_time, '%H:%i:%s') sale_datetime " +
                        "from seat a join (sale b join product c on b.pro_code = c.pro_code) on a.seat_no = b.seat_no and a.seat_no=" + TableNum + ";";

                    //MySqlDataAdapter adapter = new MySqlDataAdapter(query, connection);
                    MySqlCommand command = new MySqlCommand(query, connection);
                    dtOrderlist = new DataTable();

                    dtOrderlist.Load(command.ExecuteReader());
                    orderList.DataContext = dtOrderlist.DefaultView;

                    




                    query = "SELECT * FROM category";

                    command = new MySqlCommand(query, connection);
                    DataTable dtCat = new DataTable();

                    dtCat.Load(command.ExecuteReader());

                    foreach (DataRow drRow in dtCat.Rows)
                    {
                        Button tempBtn = new Button();
                        tempBtn.Content = drRow[1].ToString();
                        tempBtn.Style = Resources["ButtonOption"] as Style;
                        tempBtn.Click += CategoryButtonOnClick;
                        MenuCategoryGrid.Children.Add(tempBtn);
                    }
                    for (int i = 0; i < 10 - dtCat.Rows.Count; i++)
                    {
                        Border border = new Border();
                        border.Background = Brushes.AliceBlue;
                        border.CornerRadius = new CornerRadius(5);
                        border.Margin = new Thickness(2);
                        MenuCategoryGrid.Children.Add(border);
                    }





                    query = "SELECT * FROM product WHERE cat_code = " + dtCat.Rows[0][0] + ";";
                    command = new MySqlCommand(query, connection);
                    DataTable dtMenu = new DataTable();
                    dtMenu.Load(command.ExecuteReader());

                    foreach (DataRow drRow in dtMenu.Rows)
                    {
                        Button tempBtn = new Button();
                        tempBtn.Content = drRow[1].ToString();
                        tempBtn.Style = Resources["ButtonOption"] as Style;
                        MenuListGrid.Children.Add(tempBtn);
                    }
                    for (int i = 0; i < 25 - dtMenu.Rows.Count; i++)
                    {
                        Border border = new Border();
                        border.Background = Brushes.AliceBlue;
                        border.CornerRadius = new CornerRadius(5);
                        border.Margin = new Thickness(2);
                        MenuListGrid.Children.Add(border);
                    }


                    connection.Close();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

            }
        }

        // 여기 수정하자...
        private void CategoryButtonOnClick(object sender, RoutedEventArgs e)
        {
            using (connection = new MySqlConnection(connStr))
            {
                try
                {
                    connection.Open();

                    //| seat_no | seat_xpos | seat_ypos | usr_id | seat_totprc |
                    string query = "select c.pro_name, b.sale_count, b.sale_totprc, b.sale_discount, DATE_FORMAT(b.sale_time, '%H:%i:%s') sale_datetime " +
                        "from seat a join (sale b join product c on b.pro_code = c.pro_code) on a.seat_no = b.seat_no and a.seat_no=" + TableNum + ";";

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
