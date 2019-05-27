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
using NXP_HexParse.Config;
using NXP_HexParse.Datas;

namespace NXP_HexParse.Controls.Windows
{
    /// <summary>
    /// ConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HelpWindow : Window
    {


        #region 构造函数
        /// <summary>
        /// 窗体位置，大小，已连接CAN数量
        /// </summary>
        /// <param name="location"></param>
        /// <param name="size"></param>
        /// <param name="connectNum"></param>
        public HelpWindow(Point location, Size size)
        {
            InitializeComponent();

            this.Left = location.X + (size.Width-this.Width)/2;
            this.Top = location.Y + (size.Height - this.Height) / 2;


        }
        #endregion


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


    }
}
