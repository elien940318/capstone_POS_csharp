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

namespace WindowsPos.View
{
    public partial class CustomButton : Button
    {
        public Food foodOption;
        public CustomButton()
        {
            InitializeComponent();
        }
        public CustomButton(DataRow drRow) : this()
        {
            foodOption = new Food(drRow);
        }
    }
}
