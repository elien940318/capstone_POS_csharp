using System;
using System.Collections.Generic;
using System.Configuration;
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

        public ModifyTablePage()
        {
            InitializeComponent();

            foreach (Model.Table tbl in MainSystem.GetInstance._tablelist)
            {
                //TableButton tblbtn = new TableButton(tbl);
                //Canvas.SetLeft(tblbtn,tbl.XPos);
                //Canvas.SetTop(tblbtn, tbl.YPos);
                //DesigningCanvas.Children.Add(tblbtn);
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

        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // In this event, we should set the lines visibility to Hidden

            TableButton btn = sender as TableButton;
            var temp = MainSystem.GetInstance._tablelist[btn.TableNum-1];
            temp.XPos = (int)Canvas.GetLeft(btn);
            temp.YPos = (int)Canvas.GetTop(btn);
        }

        private void btnInsertClicked(object sender, RoutedEventArgs e)
        {
            Button btn = new TableButton();
            DesigningCanvas.Children.Add(btn);
            btn.SetValue(Canvas.LeftProperty, (double)50);
            btn.SetValue(Canvas.TopProperty, (double)50);
            btn.PreviewMouseLeftButtonDown += this.MouseLeftButtonDown;
            btn.PreviewMouseLeftButtonUp += this.PreviewMouseLeftButtonUp;
            btn.Cursor = Cursors.Hand;
        }

        private void btnDeleteClicked(object sender, RoutedEventArgs e)
        {
            foreach (Control control in DesigningCanvas.Children)
            {
                if (control != MovingObject)
                    continue;
                DesigningCanvas.Children.Remove(control);
                return;
            }

        }

        private void ButtonCloseOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
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

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(chkboxMove.IsChecked == true)
            {
                // 버튼 안에서의 마우스 좌표 읽기
                ctrlX1 = e.GetPosition(sender as Control).X;
                ctrlY1 = e.GetPosition(sender as Control).Y;
                ctrlWidth = (sender as Control).ActualWidth;
                ctrlHeight = (sender as Control).ActualHeight;
                
                MovingObject = sender as TableButton;
            }
        }
    }
}
