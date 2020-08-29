using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using WindowsPos.Model;

namespace WindowsPos.View
{
    /// <summary>
    /// LoginPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginPage : Page
    {
        MySqlConnection connection;
        public LoginPage()
        {
            InitializeComponent();
        }

        private void SubmitOnClick(object sender, RoutedEventArgs e)
        {
            using (connection = new MySqlConnection(
                "Server=175.200.94.253;Port=3306;Database=capstone;Uid=capstone;Pwd=capstone"))
            {   
                try
                {
                    connection.Open();

                    if (!CheckLogin())
                        return;

                    GetTableList();
                    //GetOrderList();
                    

                    // 메뉴목록 페이지 출력해주기.
                    this.NavigationService.Navigate(new MenuPage());
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        private bool CheckLogin()
        {
            if (String.IsNullOrEmpty(txtUsername.Text))
            {
                txtboxLog.Text = "아이디를 입력하세요";
                txtUsername.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(txtPassword.Password))
            {
                txtboxLog.Text = "비밀번호를 입력하세요";
                txtPassword.Focus();
                return false;
            }

            string query = "SELECT * FROM user WHERE usr_id=@usr_id AND usr_passwd=@usr_passwd";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.Add("@usr_id", MySqlDbType.VarChar, 15);
            command.Parameters.Add("@usr_passwd", MySqlDbType.VarChar, 15);
            command.Parameters["@usr_id"].Value = txtUsername.Text;
            command.Parameters["@usr_passwd"].Value = txtPassword.Password;

            DataTable dt = new DataTable();
            dt.Load(command.ExecuteReader());

            if (dt.Rows.Count == 0)
            {
                txtboxLog.Text = "로그인 정보가 정확하지 않습니다.";
                return false;
            }

            txtboxLog.Text = "로그인 성공. 메뉴 페이지로 넘어갑니다.";

            //해당 user 데이터 싱글톤으로 저장한다.
            MainSystem.GetInstance.SetMember(dt.Rows[0]);
            return true;
        }
        private void GetTableList()
        {
            try
            {
                //| seat_no | seat_xpos | seat_ypos | usr_id | seat_totprc |
                string query = "SELECT seat_no, seat_xpos, seat_ypos FROM seat WHERE usr_id=@usr_id;";
                MySqlCommand command = new MySqlCommand(query, connection);
                var ID = new MySqlParameter("@usr_id", MainSystem.GetInstance._member.Id);
                command.Parameters.Add(ID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // 테이블리스트
                    List<Table> tablelist = new List<Table>();

                    // seat_no | seat_xpos | seat_ypos | usr_id | seat_totprc 
                    while (reader.Read()) 
                    { 
                        tablelist.Add(new Table((int)reader["seat_no"], (int)reader["seat_xpos"], (int)reader["seat_ypos"]));
                    }
                    MainSystem.GetInstance.SetTableList(tablelist);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());             
            }
        }

        private void GetOrderList()
        {
            try
            {
                //| seat_no | seat_xpos | seat_ypos | usr_id | seat_totprc |
                string query = "SELECT seat_no, seat_xpos, seat_ypos FROM seat WHERE usr_id=@usr_id;";
                MySqlCommand command = new MySqlCommand(query, connection);
                var ID = new MySqlParameter("@usr_id", MainSystem.GetInstance._member.Id);
                command.Parameters.Add(ID);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // 테이블리스트
                    List<Table> tablelist = new List<Table>();

                    // seat_no | seat_xpos | seat_ypos | usr_id | seat_totprc 
                    while (reader.Read())
                    {
                        tablelist.Add(new Table((int)reader["seat_no"], (int)reader["seat_xpos"], (int)reader["seat_ypos"]));
                    }
                    MainSystem.GetInstance.SetTableList(tablelist);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }
    }
}
