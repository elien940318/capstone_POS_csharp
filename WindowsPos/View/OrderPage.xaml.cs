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

        DataTable dtOrderlist;
        MySqlConnection connection;
        MySqlCommand command;

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


                    command = new MySqlCommand("sp_s_tableorder", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new MySqlParameter("TABLE_NUM", TableNum));
                    command.Parameters["TABLE_NUM"].Direction = ParameterDirection.Input;

                    dtOrderlist = new DataTable();

                    dtOrderlist.Load(command.ExecuteReader());
                    orderList.DataContext = dtOrderlist.DefaultView;




                    string query = "SELECT * FROM category";

                    command = new MySqlCommand(query, connection);
                    DataTable dtCat = new DataTable();

                    dtCat.Load(command.ExecuteReader());

                    foreach (DataRow drRow in dtCat.Rows)
                    {
                        Button tempBtn = new Button();
                        tempBtn.Content = drRow[1].ToString();
                        tempBtn.Tag = drRow[0].ToString();
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
                        tempBtn.Tag = drRow[0].ToString();
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
            Button senderBtn = sender as Button;
            using (connection = new MySqlConnection(connStr))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT pro_code, pro_name FROM product WHERE cat_code=" + senderBtn.Tag + ";";

                    command = new MySqlCommand(query, connection);
                    DataTable dtMenu = new DataTable();
                    dtMenu.Load(command.ExecuteReader());

                    MenuListGrid.Children.Clear();

                    foreach (DataRow drRow in dtMenu.Rows)
                    {
                        Button tempBtn = new Button();
                        tempBtn.Content = drRow[1].ToString();
                        tempBtn.Tag = drRow[0].ToString();
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void MenuButtonOnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as CustomButton;


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
