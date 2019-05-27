using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NXP_HexParse.Datas;
using NXP_HexParse.Config;
using System.Text.RegularExpressions;
using SpecialFunctions.HexParse;

namespace NXP_HexParse.Controls.Windows
{
    /// <summary>
    /// AdministratorWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridTest : Window
    {
        MicrochipHexParse m_HexParse = new MicrochipHexParse();


        DataSource.AddrDataListClass m_DataList;


        public DataGridTest(Point location, Size size, ref DataSource.AddrDataListClass dataList)
        {
            InitializeComponent();


            this.Left = location.X + (size.Width - this.Width) / 2;
            this.Top = location.Y + (size.Height - this.Height) / 2;

            m_DataList = dataList;

            dgMsg1.ItemsSource = dataList.AddrDataCollection;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            
        }


        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            
            DialogResult = true;
            //this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        

    }
}
