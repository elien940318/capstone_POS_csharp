using System;
using System.Collections.Generic;
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

namespace WindowsPos
{
    /// <summary>
    /// KeyPad.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyPad : Grid
    {
        public KeyPad()
        {
            InitializeComponent();
        }

        private void btnNum_Click(object sender, RoutedEventArgs e)
        {
            if (btnKeyValue.Text.Equals("0") || btnKeyValue.Text.Equals("00"))
            {
                btnKeyValue.Text = "";
            }

            btnKeyValue.Text = btnKeyValue.Text + ((Button)sender).Content;
        }

        private void btnFlag_Click(object sender, RoutedEventArgs e)
        {
            string txt = btnKeyValue.Text;
            if ((string)((Button)sender).Name == "btnKeyClr")
            {
                txt = "";
            }
            else if ((string)((Button)sender).Name == "btnKeyBackspc")
            {
                if (txt.Length != 0)
                {
                    txt = txt.Substring(0, txt.Length - 1);
                }
            }
            btnKeyValue.Text = txt;
        }
    }
}
