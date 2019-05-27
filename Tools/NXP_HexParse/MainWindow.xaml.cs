using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NXP_HexParse.Datas;
using System.ComponentModel;
using NXP_HexParse.Config;
using NXP_HexParse.Controls.Windows;

namespace NXP_HexParse
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow:Window
    {
        /// <summary>
        /// 用户类，例如写ini文件
        /// </summary>
        ClassUser clsUser = new ClassUser();


        #region 构造函数及初始化
        public MainWindow()
        {

            InitializeComponent();

            LoadConfig(); //加载默认配置

            //myHexParse.InitialControl(this, myHexParse);
            myHexParseWithText.InitialControl(this, myHexParseWithText);
            myHexBurn.InitialControl(this, myHexBurn);
            //添加到后面，以便于最后再执行
            this.Closing += Window_AfterClosing;

            //menuNormal.Click += Menu_Click;
            //menuReliabilityOne.Click += Menu_Click;
            //menuReliabilityTwo.Click += Menu_Click;
            //menuTemperatureRise.Click += Menu_Click;

            //加载上一次窗体位置
            this.Left = CommonData.defaultLocation.X;
            this.Top = CommonData.defaultLocation.Y;


            //获取默认大小
            this.Width = CommonData.defaultSize.Width;
            this.Height = CommonData.defaultSize.Height;

            //设置登录信息
            //myMainPage.SetLoginMsg(loginMsg);

            //隐藏日志文件夹
            //SpecialFunctions.Log.NormalLog.HideErrorLog(true);
            //SpecialFunctions.Log.NormalLog.WriteErrorLog("测试");

            //默认界面和菜单
            ((MenuItem)(InterfaceSelectorMenu.Items[CommonData.defaultPage])).IsChecked = true;
            CheckedEvent(InterfaceSelectorMenu.Items[CommonData.defaultPage]);
            
        }


        #endregion

        #region 界面选择事件
        /// <summary>
        /// 选择事件
        /// </summary>
        /// <param name="sender"></param>
        private void CheckedEvent(object sender)
        {
            //父控件语言栏
            MenuItem menuItemSelector = (MenuItem)((FrameworkElement)sender).Parent;

            //非当前选中控件全部取消
            for (int i = 0; i < menuItemSelector.Items.Count; i++)
            {
                if ((MenuItem)menuItemSelector.Items[i] == (MenuItem)((FrameworkElement)sender))
                {
                    tabControl1.SelectedIndex = i;
                    ((TabItem)tabControl1.Items[i]).Visibility = Visibility.Visible;
                                        
                    continue;
                }

                ((MenuItem)menuItemSelector.Items[i]).IsChecked = false;
                
                ((TabItem)tabControl1.Items[i]).Visibility = Visibility.Collapsed;
            }
            
        }

       
        private void Menu_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            //在点击事件触发前，checked状态已经变化，此时unchecked表明之前是checked
            if (!mi.IsChecked) 
            {
                mi.IsChecked = true;
                return;
            }
            CheckedEvent(sender);
        }


        #endregion

        #region 配置文件
        /// <summary>
        /// 加载配置
        /// </summary>
        private void LoadConfig()
        {
            clsUser.GetDefaultLocation();
            clsUser.GetDefaultSize();
            clsUser.GetDefaultPage();
            

        }

        /// <summary>
        /// 保存配置
        /// </summary>
        private void SaveConfig()
        {
            //将当前位置写入配置
            clsUser.WriteDefaultLocation(new Point(this.Left, this.Top));

            //当前窗体大小
            clsUser.WriteDefaultSize(new Size(this.ActualWidth, this.ActualHeight));

            //将当前页面索引写入配置，下次从这个页面启动
            clsUser.WriteDefaultPage(tabControl1.SelectedIndex);

        }
        #endregion

        #region 事件
        
        /// <summary>
        /// 关闭后期执行
        /// </summary>
        private void Window_AfterClosing(object sender, CancelEventArgs e)
        {
            //若是全屏，则先恢复正常状态
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = System.Windows.WindowState.Normal;


            SaveConfig();//保存配置
        }


        //帮助窗体
        private void helpMenu_Click(object sender, RoutedEventArgs e)
        {
            Controls.Windows.HelpWindow helpWindow = new Controls.Windows.HelpWindow(new Point(this.Left,this.Top), new Size(this.Width,this.Height));

            helpWindow.ShowDialog();
        }


        #endregion



        private void miNormalPage_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("说明书\\电机电控测试上位机使用说明.pdf");
        }

        private void miReliablityOnePage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void miReliablityTwoPage_Click(object sender, RoutedEventArgs e)
        {

        }

        #region 始终最前
        private void miTopMe_Checked(object sender, RoutedEventArgs e)
        {
            this.Topmost = true;
        }

        private void miTopMe_Unchecked(object sender, RoutedEventArgs e)
        {
            this.Topmost = false;
        }

        #endregion


    }
}
