﻿using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WindowsPos.Model;

namespace WindowsPos.View
{
    public partial class OrderPage : Page, INotifyPropertyChanged
    {
        
        MySqlConnection connection;
        MySqlCommand command;

        // pro_name, sale_count, sale_totprc, sale_discount, order_date, pro_code, order_no
        DataTable dtOrderlist;  // 메뉴 주문 리스트

        string connStr;


        private string calctext;

        public string CalcText
        {
            get { return calctext; }
            set
            {
                calctext = value;
                OnPropertyChanged("CalcText");
            }
        }

        public int TableNum { get; set; }
        public int tempProductCode { get; set; }
        public bool firstOrder { get; set; }


        private int price;
        public int Price
        {
            get { return price; }
            set
            {
                price = value;
                OnPropertyChanged("Price");
            }
        }

        private int discount;
        public int Discount
        {
            get { return discount; }
            set
            {
                discount = value;
                OnPropertyChanged("Discount");
            }
        }

        private int totalprice;
        public int TotalPrice
        {
            get { return totalprice; }
            set
            {
                totalprice = value;
                OnPropertyChanged("TotalPrice");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }


        public OrderPage() 
        {
            this.DataContext = this;
            InitializeComponent();
            connStr = "Server=175.200.94.253;Port=3306;Database=capstone;Uid=capstone;Pwd=capstone";
            tempProductCode = 0;
            CalcText = string.Empty;
        }

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
                    // 클래스화 하여 넣어주자... Discount나 Count수량 변화할때 자동으로 TotPrice 변화하도록......                    
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

            CalculateMoney();
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

            if (dtOrderlist.Rows.Count == 0)
                tempProductCode = 0;

            // 1. temp의 제품코드와 Button의 제품코드가 동일한가?
            if (Int32.Parse(btn.Tag.ToString()) == tempProductCode && dtOrderlist.Rows[size - 1]["pro_name"].ToString() == btn.Content.ToString())
            {
                // 수량과 총가격을 계산하자.
                dtOrderlist.Rows[size - 1]["sale_count"] = (int)dtOrderlist.Rows[size - 1]["sale_count"] + 1;
                dtOrderlist.Rows[size - 1]["sale_totprc"] = (int)dtOrderlist.Rows[size - 1]["sale_totprc"] + btn.foodOption.ProductPrice;
                CalculateMoney();
                return;
            }

            if (dtOrderlist.Rows.Count == 0 || firstOrder == false)
                firstOrder = true;
            
            // 상품명 수량 총금액 할인금액 주문시간
            dtOrderlist.Rows.Add(btn.Content.ToString(), 1, btn.foodOption.ProductPrice, 0, DateTime.Now.ToString("HH:mm:ss"), btn.Tag, null);
            tempProductCode = Int32.Parse(btn.Tag.ToString());

            CalculateMoney();
            //dtOrderlist.AcceptChanges();
        }

        private void ButtonSaveOnClick(object sender, RoutedEventArgs e)
        {
            MySqlTransaction transaction = null;
            MySqlCommand command = new MySqlCommand();

            

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

                // 쿼리문 필요없음 -> 스토어드 프로시저 사용함...
                // string query = string.Empty;

                try
                {
                    int tempOrderNo = -1;

                    foreach (DataRow drRow in dtOrderlist.Rows)
                    {
                        // DataTable UI 구성상 갖고있는 컬럼정보
                        // pro_name | sale_count | sale_totprc | sale_discount | order_date | pro_code | order_no 


                        // tableorder: usr_id, seat_no
                        // sale: order_no, pro_code                        
                        
                        switch (drRow.RowState)
                        {
                            case DataRowState.Added:
                                if (firstOrder)
                                {
                                    // 주문번호를 가져와야 INSERT가 가능해진다...
                                    command = new MySqlCommand("SP_INSERT_TABLEORDER", connection);
                                    command.Parameters.Add(new MySqlParameter("USR_ID", MainSystem.GetInstance._member.Id.ToString()));
                                    command.Parameters.Add(new MySqlParameter("SEAT_NO", TableNum.ToString()));
                                    command.Parameters.Add("@ORDER_NO", MySqlDbType.Int32, 11);
                                    command.Parameters["USR_ID"].Direction = ParameterDirection.Input;
                                    command.Parameters["SEAT_NO"].Direction = ParameterDirection.Input;
                                    command.Parameters["@ORDER_NO"].Direction = ParameterDirection.Output;

                                    command.CommandType = CommandType.StoredProcedure;

                                    // ------- tempOrderNo로 주문번호가 넘어오질 않음...
                                    using (var reader = command.ExecuteReader())
                                    {
                                        while (reader.Read())
                                        {
                                            tempOrderNo = (int)reader["order_no"];
                                        }
                                    }
                                    
                                    firstOrder = !firstOrder;
                                }
                                command = new MySqlCommand("SP_INSERT_SALE", connection);
                                command.Parameters.Add(new MySqlParameter("SEAT_NO", TableNum.ToString()));
                                command.Parameters.Add(new MySqlParameter("ORDER_NO", tempOrderNo));
                                command.Parameters.Add(new MySqlParameter("PRO_CODE", drRow["pro_code"]));
                                command.Parameters.Add(new MySqlParameter("SALE_COUNT", drRow["sale_count"]));
                                command.Parameters.Add(new MySqlParameter("SALE_DISCOUNT", drRow["sale_discount"]));
                                command.Parameters["SEAT_NO"].Direction = ParameterDirection.Input;
                                command.Parameters["ORDER_NO"].Direction = ParameterDirection.Input;
                                command.Parameters["PRO_CODE"].Direction = ParameterDirection.Input;
                                command.Parameters["SALE_COUNT"].Direction = ParameterDirection.Input;
                                command.Parameters["SALE_DISCOUNT"].Direction = ParameterDirection.Input;
                                command.CommandType = CommandType.StoredProcedure;
                                command.ExecuteNonQuery();
                                break;

                                
                            case DataRowState.Deleted:
                                dtOrderlist.RejectChanges();
                                command = new MySqlCommand("SP_DELETE_SALE", connection);
                                command.Parameters.Add(new MySqlParameter("ORDERNO", drRow["order_no"]));
                                command.Parameters.Add(new MySqlParameter("PROCODE", drRow["pro_code"]));
                                command.Parameters["ORDERNO"].Direction = ParameterDirection.Input;
                                command.Parameters["PROCODE"].Direction = ParameterDirection.Input;
                                command.CommandType = CommandType.StoredProcedure;
                                command.ExecuteNonQuery();
                                break;


                            case DataRowState.Modified:
                                command = new MySqlCommand("SP_UPDATE_SALE", connection);
                                command.Parameters.Add(new MySqlParameter("ORDERNO", drRow["order_no"]));
                                command.Parameters.Add(new MySqlParameter("PROCODE", drRow["pro_code"]));
                                command.Parameters.Add(new MySqlParameter("SALECOUNT", drRow["sale_count"]));
                                command.Parameters.Add(new MySqlParameter("SALEDISCOUNT", drRow["sale_discount"]));
                                command.Parameters["ORDERNO"].Direction = ParameterDirection.Input;
                                command.Parameters["PROCODE"].Direction = ParameterDirection.Input;
                                command.Parameters["SALECOUNT"].Direction = ParameterDirection.Input;
                                command.Parameters["SALEDISCOUNT"].Direction = ParameterDirection.Input;
                                command.CommandType = CommandType.StoredProcedure;
                                command.ExecuteNonQuery();
                                break;

                            default:
                                break;
                        }
                    }

                    transaction.Commit();
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
            this.NavigationService.GoBack();
        }


        private void ButtonCloseOnClick(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void btnDeleteSelection_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as CustomButton;

            var index = orderList.SelectedIndex;
            dtOrderlist.Rows[index].Delete();
            //dtOrderlist.Rows.RemoveAt(index);

            CalculateMoney();
        }

        private void btnCountPlus_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as CustomButton;
            var index = orderList.SelectedIndex;


            dtOrderlist.Rows[index]["sale_count"] = (int)dtOrderlist.Rows[index]["sale_count"] + 1;
            dtOrderlist.Rows[index]["sale_totprc"] = MainSystem.GetInstance._productList[dtOrderlist.Rows[index]["pro_name"].ToString()] * (int)dtOrderlist.Rows[index]["sale_count"] - (int)dtOrderlist.Rows[index]["sale_discount"];
            dtOrderlist.Rows[index]["order_date"] = DateTime.Now.ToString("hh:MM:ss");

            CalculateMoney();
        }

        private void btnCountMinus_Click(object sender, RoutedEventArgs e)
        {
            var index = orderList.SelectedIndex;


            if (! ((int)dtOrderlist.Rows[index]["sale_count"] == 1)) {
                dtOrderlist.Rows[index]["sale_count"] = (int)dtOrderlist.Rows[index]["sale_count"] - 1;
            }

            

            if (! ((int)dtOrderlist.Rows[index]["sale_totprc"] - MainSystem.GetInstance._productList[dtOrderlist.Rows[index]["pro_name"].ToString()] == 0)) {
                dtOrderlist.Rows[index]["sale_totprc"] = MainSystem.GetInstance._productList[dtOrderlist.Rows[index]["pro_name"].ToString()] * (int)dtOrderlist.Rows[index]["sale_count"] - (int)dtOrderlist.Rows[index]["sale_discount"];
            }

            CalculateMoney();
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if(orderList.SelectedIndex == -1)
            {
                orderList.SelectedIndex = 0;
            }
            else if(orderList.SelectedIndex == dtOrderlist.Rows.Count - 1)
            {
                orderList.SelectedIndex = 0;
            }
            else
                orderList.SelectedIndex += 1;

            CalculateMoney();
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (orderList.SelectedIndex == -1)
            {
                orderList.SelectedIndex = dtOrderlist.Rows.Count - 1;
            }
            else
                orderList.SelectedIndex -= 1;

            CalculateMoney();
        }

        private void CalculateMoney()
        {
            Price = Discount = TotalPrice = 0;
            foreach (DataRow drRow in dtOrderlist.Rows)
            {
                if (drRow.RowState == DataRowState.Deleted)
                {
                    continue;
                }
                Price += (int)drRow["sale_totprc"];
                Discount += (int)drRow["sale_discount"];
                TotalPrice = Price - Discount;
            }
        }

        private void btnKakaopay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = "https://kapi.kakao.com/v1/payment/ready";

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.Host = "kapi.kakao.com";
                req.Headers.Add("Authorization", "KakaoAK 82df68f87186848290c1d00d2c341cc2");
                req.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

                List<KeyValuePair<string, object>> data = new List<KeyValuePair<string, object>>()
                {
                    new KeyValuePair<string, object>("cid", "TC0ONETIME"),
                    new KeyValuePair<string, object>("partner_order_id", "partner_order_id"),
                    new KeyValuePair<string, object>("partner_user_id", "partner_user_id"),
                    new KeyValuePair<string, object>("item_name", "JinjuBeer"),
                    new KeyValuePair<string, object>("quantity", 1),
                    new KeyValuePair<string, object>("total_amount", 22500),
                    new KeyValuePair<string, object>("tax_free_amount", 20455),
                    //new KeyValuePair<string, object>("approval_url", "https://elienDesktop.iptime.org:3515"),
                    //new KeyValuePair<string, object>("fail_url", "https://elienDesktop.iptime.org:3515"),
                    //new KeyValuePair<string, object>("cancel_url", "https://elienDesktop.iptime.org:3515"),
                    new KeyValuePair<string, object>("approval_url", "https://localhost:8080"),
                    new KeyValuePair<string, object>("fail_url", "https://localhost:8080"),
                    new KeyValuePair<string, object>("cancel_url", "https://localhost:8080"),
                };

                StringBuilder builder = new StringBuilder();

                foreach (KeyValuePair<string, object> kvp in data)
                    builder.Append(kvp.Key + "=" + kvp.Value + "&");

                // 여기서 builder 디버깅 보자.
                // data들을 UTF8형식으로 바이트 변환함.            
                byte[] bytes = Encoding.UTF8.GetBytes(builder.ToString());
                req.ContentLength = bytes.Length;

                // HttpWebRequest req에 변환된 데이터들을 넣자.
                using (Stream reqStream = req.GetRequestStream())
                    reqStream.Write(bytes, 0, bytes.Length);

                // 받은 내용 저장 변수
                string responseText = string.Empty;

                // HttpWebRequest req로부터 response한 결과 클래스
                using (var resp = req.GetResponse())
                {
                    Stream respStream = resp.GetResponseStream();
                    StreamReader reader = new StreamReader(respStream);

                    string responseFromServer = reader.ReadToEnd();

                    Console.WriteLine(responseFromServer);
                }
            }
            catch (WebException ex)
            {
                //var response = (HttpWebResponse)ex.Response;
                var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();

                dynamic obj = JsonConvert.DeserializeObject(resp);
                var messagefromserver = obj.error.message;
            }
        }

        private void btnPayCash_Click(object sender, RoutedEventArgs e)
        {
            CashPage cashPage = new CashPage();
            cashPage.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            cashPage.WindowState = WindowState.Normal;
            cashPage.Owner = System.Windows.Application.Current.MainWindow;
            cashPage.ShowDialog();
            this.NavigationService.GoBack();
        }

        // 123456789 0 00 clr < enter
        private void btnCalcClick(object sender, RoutedEventArgs e)
        {
            Button btn = e.Source as Button;

            if (btn == null)
                return;

            string strButton = btn.Content.ToString();
            char chButton = strButton[0];
            
            if (strButton == "Enter")
                chButton = '=';

            if (Char.IsNumber(chButton))
            {
                CalcText += strButton;
            }
            else
            {
                switch (chButton)
                {
                    case 'E':
                        break;
                    case '<':                        
                        CalcText = CalcText.Substring(0, CalcText.Length - 1);
                        break;
                    case 'C':
                        CalcText = string.Empty;
                        break;
                    default:                        
                        break;
                }
            }
        }

        private void btnApplyDiscount_Click(object sender, RoutedEventArgs e)
        {
            var index = orderList.SelectedIndex;

            if (Int32.Parse(CalcText) > (int)dtOrderlist.Rows[index]["sale_totprc"])
            {
                
                return;
            }            
            if (CalcText != string.Empty)
            {
                dtOrderlist.Rows[index]["sale_discount"] = CalcText;
                dtOrderlist.Rows[index]["sale_totprc"] = MainSystem.GetInstance._productList[dtOrderlist.Rows[index]["pro_name"].ToString()] * (int)dtOrderlist.Rows[index]["sale_count"] - (int)dtOrderlist.Rows[index]["sale_discount"];
                dtOrderlist.Rows[index]["order_date"] = DateTime.Now.ToString("hh:MM:ss");
                CalcText = string.Empty;
            }
        }
    }
}
