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
        
        MySqlConnection connection;
        MySqlCommand command;
        DataTable dtOrderlist;  // 메뉴 주문 리스트

        string connStr = "Server=175.200.94.253;Port=3306;Database=capstone;Uid=capstone;Pwd=capstone";
        string updatequery = "START TRANSACTION;";

        public int TableNum { get; set; }
        public OrderPage() { InitializeComponent(); }

        public OrderPage(TableButton btn) : this()
        {
            TableNum = btn.TableNum;

            using (connection = new MySqlConnection(connStr))
            {
                try
                {
                    connection.Open();

                    command = new MySqlCommand("sp_s_tableorderlist", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add(new MySqlParameter("TABLE_NUM", TableNum));
                    command.Parameters["TABLE_NUM"].Direction = ParameterDirection.Input;

                    dtOrderlist = new DataTable();
                    dtOrderlist.Load(command.ExecuteReader());
                    orderList.DataContext = dtOrderlist.DefaultView;

                    //---------------------------------------------------------------
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

                    //---------------------------------------------------------------
                    query = "SELECT * FROM product WHERE cat_code = " + dtCat.Rows[0][0] + ";";
                    command = new MySqlCommand(query, connection);
                    DataTable dtMenu = new DataTable();
                    dtMenu.Load(command.ExecuteReader());

                    foreach (DataRow drRow in dtMenu.Rows)
                    {
                        CustomButton tempBtn = new CustomButton(drRow);
                        tempBtn.Content = drRow[1].ToString();  // 제품명을 content로...
                        tempBtn.Tag = drRow[0].ToString();      // 제품코드를 tag로...
                        
                        tempBtn.Style = Resources["ButtonOption"] as Style;
                        tempBtn.Click += MenuButtonOnClick;
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
                        CustomButton tempBtn = new CustomButton(drRow);
                        tempBtn.Content = drRow[1].ToString();
                        tempBtn.Tag = drRow[0].ToString();
                        tempBtn.Style = Resources["ButtonOption"] as Style;
                        tempBtn.Click += MenuButtonOnClick;
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
            var size = dtOrderlist.Rows.Count;

            // 데이터테이블 마지막 열의 값과 동일한 경우 (특정 음식을 한번 더 누른 경우)
            if (dtOrderlist.Rows[size - 1][0].ToString() == btn.Content.ToString())
            {
                dtOrderlist.Rows[size - 1][1] = Int32.Parse(dtOrderlist.Rows[size - 1][1].ToString()) + 1;
                //updatequery += "UPDATE sale SET sale_count = sale_count + 1, sale_totprc = sale_totprc + " + btn.foodOption.ProductPrice 
                //    + " WHERE ;";
                return;
            }

            using (connection = new MySqlConnection(connStr))
            {
                try
                {
                    connection.Open();
                    
                    //sale_no | sale_time | sale_count | sale_discount | sale_totprc | seat_no | pro_code | usr_id
                    string query = "SELECT pro_price FROM product WHERE pro_code=" + btn.Tag + ";";

                    command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    // DataTable에 추가
                    dtOrderlist.Rows.Add(btn.Content.ToString(), 1, reader.GetString(0), 0, DateTime.Now.ToString("hh:mm:ss"));

                    updatequery += "INSERT INTO sale(sale_count, seat_no, pro_code, usr_id) VALUES (1, " + TableNum + ", " + btn.Tag + ", '" +
                        MainSystem.GetInstance._member.Id.ToString() + "'); ";


                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }

        private void ButtonCloseOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
