using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsPos.Model;
using WindowsPos.ViewModel;

namespace WindowsPos.View
{
    /// <summary>
    /// TablePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TablePage : Page
    {
        public TablePage()
        {
            InitializeComponent();


            MySqlConnection connection;

            using (connection = new MySqlConnection(
                "Server=175.200.94.253;Port=3306;Database=capstone;Uid=capstone;Pwd=capstone"))
            {
                try
                {
                    connection.Open();
                    
                    foreach (Model.Table tbl in MainSystem.GetInstance._tablelist)
                    {
                        //| seat_no | seat_xpos | seat_ypos | usr_id | seat_totprc |
                        string query = "select c.pro_name, b.sale_count, b.sale_totprc, b.sale_discount from seat a join (sale b join product c on b.pro_code = c.pro_code) on a.seat_no = b.seat_no and a.seat_no=@seat_no;";
                        MySqlDataAdapter adapter = new MySqlDataAdapter();
                        MySqlCommand command = new MySqlCommand(query, connection);
                        var tableNum = new MySqlParameter("@seat_no", tbl.TableNum);
                        command.Parameters.Add(tableNum);
                        adapter.SelectCommand = command;
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);

                        TableButton tblbtn = new TableButton(tbl.TableNum, ds);

                        Canvas.SetLeft(tblbtn, tbl.XPos);
                        Canvas.SetTop(tblbtn, tbl.YPos);
                        DesigningCanvas.Children.Add(tblbtn);

                        tblbtn.Click += Tblbtn_Click;
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private void Tblbtn_Click(object sender, RoutedEventArgs e)
        {
            TableButton btn = e.Source as TableButton;
            this.NavigationService.Navigate(new OrderPage(btn));
        }

        private void ButtonCloseOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

    }
}
