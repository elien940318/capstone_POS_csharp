﻿using System;
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
using System.Windows.Shapes;

namespace WindowsPos.View
{
    /// <summary>
    /// CashPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CashPage : Window
    {
        public CashPage()
        {
            InitializeComponent();            
        }

        private void btnPay_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

    }
}
