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
    /// TableButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TableButton : Button
    {
        public int TableNum { get; set; }


        public TableButton()
        {
            InitializeComponent();
        }

        public TableButton(int tableno, DataSet ds) : this()
        {
            TableNum = tableno;
            txtTableName.Text = tableno + "번 테이블";

            if (ds.Tables.Count > 0)
            {
                int cnt = 0;
                int table_prc = 0;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    table_prc += Convert.ToInt32(row["sale_totprc"]);
                    if (cnt < 3)
                    {
                        TextBlock txtblk = new TextBlock();
                        txtblk.TextWrapping = TextWrapping.Wrap;
                        txtblk.FontSize = 8;

                        txtblk.Text = row["pro_name"].ToString() + "\t" + row["sale_count"];
                        pnlTableMenuList.Children.Add(txtblk);
                    }
                    ++cnt;
                }
                if (cnt - 3 > 0) {
                    TextBlock txtblk = new TextBlock();
                    txtblk.Inlines.Add(new Run("...그 외 " + (cnt - 3) + "개") { FontStyle = FontStyles.Italic, FontSize = 6 });
                    pnlTableMenuList.Children.Add(txtblk);
                }
                txtTablePrice.Text = table_prc.ToString() + "원";
            }
        }
    }
}
