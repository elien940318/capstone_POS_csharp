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
using WindowsPos.Model;

namespace WindowsPos.View
{
    /// <summary>
    /// MenuPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }
        
        private void ButtonSalesOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new TablePage());
        }

        private void ButtonMoneyOnClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonTableOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ModifyTablePage());
        }

        private void ButtonMenuOnClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAnalysisOnClick(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonLogoutOnClick(object sender, RoutedEventArgs e)
        {
            MainSystem.GetInstance.ReleaseInstance();
            this.NavigationService.GoBack();
        }
    }
}
