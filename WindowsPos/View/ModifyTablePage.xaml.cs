using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace WindowsPos.View
{
    /// <summary>
    /// TablePage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ModifyTablePage : Page
    {
        double ctrlX1, ctrlY1;
        double ctrlWidth, ctrlHeight;
        TableButton MovingObject;
        DataTable dt;
        public ModifyTablePage()
        {
            InitializeComponent(); 
        }

        private void ModifyTablePageOnLoad(object sender, RoutedEventArgs e)
        {
            using (MySqlConnection connection = new MySqlConnection(
            "Server=175.200.94.253;Port=3306;Database=capstone;Uid=capstone;Pwd=capstone"))
            {
                try
                {
                    connection.Open();

                    string query = "SELECT * FROM seat WHERE usr_id=@usr_id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.Add("@usr_id", MySqlDbType.VarChar, 15);
                    command.Parameters["@usr_id"].Value = MainSystem.GetInstance._member.Id;

                    dt = new DataTable();
                    dt.Load(command.ExecuteReader());


                    foreach (DataRow dataRow in dt.Rows)
                    {
                        TableButton tblbtn = new TableButton(Int32.Parse(dataRow[0].ToString()));
                        Canvas.SetLeft(tblbtn, Double.Parse(dataRow[1].ToString()));
                        Canvas.SetTop(tblbtn, Double.Parse(dataRow[2].ToString()));
                        DesigningCanvas.Children.Add(tblbtn);
                    }

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                    // 서버 연결이 안되므로 로컬 저장된 데이터 활용할 것...
                }
            }


            // 이벤트 추가
            foreach (Control control in DesigningCanvas.Children)
            {
                control.PreviewMouseLeftButtonDown += this.MouseLeftButtonDown;
                control.PreviewMouseLeftButtonUp += this.PreviewMouseLeftButtonUp;
                control.Cursor = Cursors.Hand;
            }

            // Setting the MouseMove event for our parent control(In this case it is DesigningCanvas).
            DesigningCanvas.PreviewMouseMove += this.MouseMove;
        }


        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if(chkboxMove.IsChecked == true)
            //{
            //    // 버튼 안에서의 마우스 좌표 읽기
            //    ctrlX1 = e.GetPosition(sender as Control).X;
            //    ctrlY1 = e.GetPosition(sender as Control).Y;
            //    ctrlWidth = (sender as Control).ActualWidth;
            //    ctrlHeight = (sender as Control).ActualHeight;

            //    MovingObject = sender as TableButton;
            //}

            ctrlX1 = e.GetPosition(sender as Control).X;
            ctrlY1 = e.GetPosition(sender as Control).Y;
            ctrlWidth = (sender as Control).ActualWidth;
            ctrlHeight = (sender as Control).ActualHeight;

            MovingObject = sender as TableButton;
        }
        private void MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && DesigningCanvas.Children.Contains((UIElement)MovingObject) && chkboxMove.IsChecked == true)
            {
                var canv = (MovingObject as FrameworkElement).Parent as FrameworkElement;

                // canvas 기준 control의 x1, y1값
                var x1 = e.GetPosition(canv).X - ctrlX1;
                var y1 = e.GetPosition(canv).Y - ctrlY1;
                var x2 = x1 + ctrlWidth;
                var y2 = y1 + ctrlHeight;

                if (x1 > 0 && y1 > 0 && canv.ActualWidth > x2 && canv.ActualHeight > y2)
                {
                    // 테이블 위치를 옮긴다.
                    (MovingObject as FrameworkElement).SetValue(Canvas.LeftProperty, x1);
                    (MovingObject as FrameworkElement).SetValue(Canvas.TopProperty, y1);

                }
            }
        }
        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // In this event, we should set the lines visibility to Hidden

            TableButton btn = sender as TableButton;

            var canv = (MovingObject as FrameworkElement).Parent as FrameworkElement;
            var xPos = e.GetPosition(canv).X - ctrlX1;
            var yPos = e.GetPosition(canv).Y - ctrlY1;

            dt.Rows[btn.TableNum - 1][1] = xPos;
            dt.Rows[btn.TableNum - 1][2] = yPos;
        }

        private void btnInsertClicked(object sender, RoutedEventArgs e)
        {
            var tbnum = Int32.Parse(dt.Rows[(dt.Rows.Count) - 1][0].ToString()) + 1;
            dt.Rows.Add(tbnum, 50, 50, MainSystem.GetInstance._member.Id);

            TableButton btn = new TableButton(tbnum);
            DesigningCanvas.Children.Add(btn);
            btn.SetValue(Canvas.LeftProperty, (double)50);
            btn.SetValue(Canvas.TopProperty, (double)50);
            btn.PreviewMouseLeftButtonDown += this.MouseLeftButtonDown;
            btn.PreviewMouseLeftButtonUp += this.PreviewMouseLeftButtonUp;
            btn.Cursor = Cursors.Hand;
        }

        private void btnDeleteClicked(object sender, RoutedEventArgs e)
        {
            TableButton btn = sender as TableButton;
            foreach (Control control in DesigningCanvas.Children)
            {
                if (control != MovingObject)
                    continue;
                DesigningCanvas.Children.Remove(control);

                //IEnumerable<DataRow> drRows = dt.Select().Where(x => x["table_num"].ToString() == btn.TableNum.ToString());
                
                return;
            }
        }

        
        private void ButtonCloseOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void ButtonSaveOnClick(object sender, RoutedEventArgs e)
        {
            string query = "START TRANSACTION;";

            foreach (DataRow dr in dt.Rows)
            {
                switch (dr.RowState)
                {
                    case DataRowState.Added:
                        query += "INSERT INTO seat VALUES (" + dr[0] + ", " + dr[1] + ", " + dr[2] + ", '" + MainSystem.GetInstance._member.Id + "');";
                        break;
                    case DataRowState.Deleted:
                        query += "";
                        break;
                    case DataRowState.Modified:
                        query += "UPDATE seat SET seat_xpos=" + dr[1] + ", seat_ypos=" + dr[2] + " WHERE seat_no=" + dr[0] + " AND usr_id='" + MainSystem.GetInstance._member.Id + "';";
                        break;
                    default:
                        break;
                }
            }

            query += "COMMIT;";

            using (MySqlConnection connection = new MySqlConnection(
            "Server=175.200.94.253;Port=3306;Database=capstone;Uid=capstone;Pwd=capstone"))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            MainSystem.GetInstance.SetTableList(dt);
            // 수정된 DataTable 싱글톤으로 저장해둘것.
        }        
    }
}
