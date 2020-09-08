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

                    command = new MySqlCommand("sp_select_tableorderlist_seatno", connection);
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

                    string query = "SELECT * FROM product WHERE cat_code=" + senderBtn.Tag + ";";

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

                return;
            }

            using (connection = new MySqlConnection(connStr))
            {
                try
                {
                    connection.Open();
                    
                    string query = "SELECT pro_price FROM product WHERE pro_code=" + btn.Tag + ";";

                    command = new MySqlCommand(query, connection);
                    MySqlDataReader reader = command.ExecuteReader();
                    reader.Read();

                    DateTime dt = DateTime.Now;
                    // 상품명 수량 총금액 할인금액 주문시간
                    dtOrderlist.Rows.Add(btn.Content.ToString(), 1, reader.GetString(0), 0, dt.ToString("HH:mm:ss"), btn.Tag, null);


                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
        private void ButtonSaveOnClick(object sender, RoutedEventArgs e)
        {
            MySqlTransaction transaction = null;
            MySqlCommand command = new MySqlCommand();

            bool addOrder = false;

            using (connection = new MySqlConnection(connStr))
            {
                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    command = connection.CreateCommand();
                    
                    command.Connection = connection;
                    command.Transaction = transaction;
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    connection.Close();
                }

                string query = string.Empty;

                try
                {
                    foreach (DataRow drRow in dtOrderlist.Rows)
                    {
                        switch (drRow.RowState)
                        {
                            case DataRowState.Added:
                                if (!addOrder)
                                {
                                    query += "INSERT INTO tableorder(seat_no, usr_id) VALUES(" + TableNum + ", '" + MainSystem.GetInstance._member.Id + "');";
                                    addOrder = !addOrder;
                                }
                                query += "INSERT INTO sale(order_no, pro_code, sale_count) values ((SELECT MAX(order_no) FROM tableorder WHERE seat_no = " + TableNum + "), " + drRow["pro_code"] + ", 1)";

                                break;
                            case DataRowState.Deleted:
                                query += "DELETE FROM sale WHERE order_no=" + drRow["order_no"] + " AND pro_code=" + drRow["pro_code"] + ";";
                                break;
                            case DataRowState.Modified:
                                break;
                            default:
                                break;
                        }

                        command.CommandText = query;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    transaction.Rollback();
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        private void ButtonCloseOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        
    }
}
