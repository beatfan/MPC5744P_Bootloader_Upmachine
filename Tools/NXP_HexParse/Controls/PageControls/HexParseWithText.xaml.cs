using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using NXP_HexParse.Datas;
using System.IO;
using NXP_HexParse.Config;
using Microsoft.Win32;


namespace NXP_HexParse.Controls.PageControls
{
    /// <summary>
    /// 常规页面
    /// </summary>
    public partial class HexParseWithText : BeatfanControls.UserControls.MyUserControlModel
    {
        #region 数据定义


        /// <summary>
        /// 定时捕获定时器
        /// </summary>
        private System.Windows.Threading.DispatcherTimer m_AutoCaptureTimer = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// HexString集合
        /// </summary>
        DataSource.HexStringListClass m_HexStringList = new DataSource.HexStringListClass();

        /// <summary>
        /// DataGrid绑定源
        /// </summary>
        DataSource.AddrDataListClass m_AddrDataList1 = new DataSource.AddrDataListClass();
        
        /// <summary>
        /// 搜索DataGrid绑定源
        /// </summary>
        DataSource.AddrDataListClass m_SearchAddrDataList1 = new DataSource.AddrDataListClass();


        /// <summary>
        /// 通用函数
        /// </summary>
        Common.NXPHexCommonFunctions m_CommonFunc = new Common.NXPHexCommonFunctions();

        /// <summary>
        /// 显示的比对的差异的条目索引的索引
        /// </summary>
        int m_CompareIndexIndex = 0;
        #endregion

        #region 构造函数
        public HexParseWithText()
        {
            InitializeComponent();
            
            m_AutoCaptureTimer.Interval = new TimeSpan(0, 0, 1);
            //m_AutoCaptureTimer.Tick += m_AutoCaptureTimer_Tick;

            m_AddrDataList1.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();
            m_SearchAddrDataList1.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();

            m_HexStringList.HexStringCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.HexString>();
        }
        
        #endregion
        
        
        
        #region 事件

        protected override void OnVisible()
        {

        }
        
        /// <summary>
        /// 关闭前操作
        /// </summary>
        protected override void OnClosing()
        {
            
        }

        #region 选择文件

        /// <summary>
        /// 将hex字符串解析显示
        /// </summary>
        private void RefreshShowHexFile(ref DataSource.HexStringListClass hexStringList)
        {

            lvHexString.SelectionChanged -= lvHexString_SelectionChanged;
            dgMsg1.SelectionChanged -= dgMsg1_SelectionChanged;

            m_AddrDataList1.AddrDataCollection.Clear();
            m_SearchAddrDataList1.AddrDataCollection.Clear();

            m_AddrDataList1 = m_CommonFunc.HexStringToDataList(hexStringList); //转换为数据列表
            //m_AddrDataList1 = m_CommonFunc.InsertSkipToDataList(ref m_AddrDataList1); //插入跳过的地址
            //复制一份，用作筛选显示
            m_CommonFunc.AddrDataListClassCopy(ref m_AddrDataList1, ref m_SearchAddrDataList1);

            //指定显示的源
            lvHexString.ItemsSource = m_HexStringList.HexStringCollection;
            dgMsg1.ItemsSource = m_SearchAddrDataList1.AddrDataCollection;
            lbFilter1.Content = m_SearchAddrDataList1.AddrDataCollection.Count;

            dgMsg1.SelectionChanged += dgMsg1_SelectionChanged;
            lvHexString.SelectionChanged += lvHexString_SelectionChanged;
        }

        /// <summary>
        /// 将hex字符串解析显示
        /// </summary>
        private void RefreshShowHexFile(string[] allLines)
        {

            lvHexString.SelectionChanged -= lvHexString_SelectionChanged;
            dgMsg1.SelectionChanged -= dgMsg1_SelectionChanged;

            m_HexStringList.HexStringCollection.Clear();
            m_AddrDataList1.AddrDataCollection.Clear();
            
            m_HexStringList = m_CommonFunc.ConvertStringToHexString(allLines); //获取字符串列表
            m_AddrDataList1 = m_CommonFunc.ConvertStringToDataList(allLines); //转换为数据列表
            //m_AddrDataList1 = m_CommonFunc.InsertSkipToDataList(ref m_AddrDataList1); //插入跳过的地址
            //复制一份，用作筛选显示
            m_CommonFunc.AddrDataListClassCopy(ref m_AddrDataList1, ref m_SearchAddrDataList1);

            //指定显示的源
            lvHexString.ItemsSource = m_HexStringList.HexStringCollection;
            dgMsg1.ItemsSource = m_SearchAddrDataList1.AddrDataCollection;
            lbFilter1.Content = m_SearchAddrDataList1.AddrDataCollection.Count;

            dgMsg1.SelectionChanged += dgMsg1_SelectionChanged;
            lvHexString.SelectionChanged += lvHexString_SelectionChanged;
        }

        //选择Hex文件1，并解析
        private void btnSelectHexFile1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfDialog = new OpenFileDialog();
            opfDialog.InitialDirectory = Directory.GetCurrentDirectory().ToString();
            opfDialog.Filter = "(*.hex)|*.hex|All files(*.*)|*.*";
            if (opfDialog.ShowDialog().Value)
            {

                string[] allLines = File.ReadAllLines(opfDialog.FileName);
                tbSelectedHexFilePath1.Text = opfDialog.FileName;
                RefreshShowHexFile(allLines);
            }

        }
        
        #region 拖拽获取文件
        //图标效果
        private void dgMsg1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;
        }
        
        //获取文件名，添加进入
        private void dgMsg1_Drop(object sender, DragEventArgs e)
        {
            string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            string[] allLines = File.ReadAllLines(fileName);
            tbSelectedHexFilePath1.Text = fileName;
            RefreshShowHexFile(allLines);
        }

        #endregion

        #endregion

        #region 筛选

        

        /// <summary>
        /// 筛选1
        /// </summary>
        private void tbFilter1_TextChanged(object sender, TextChangedEventArgs e)
        {
            m_CommonFunc.SearchData(new string[] { tbNoSearch1.Text, tbAddrSearch1.Text, tbDataSearch1.Text },
                ref m_AddrDataList1,ref m_SearchAddrDataList1);
            lbFilter1.Content = m_SearchAddrDataList1.AddrDataCollection.Count;
        }


        #endregion


        #region 根据选择定位行
        /// <summary>
        /// 点击Hex字符串定位到解析的地址中
        /// </summary>
        private void lvHexString_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dgMsg1.SelectionChanged -= dgMsg1_SelectionChanged;
            for (int i = 0; i < m_SearchAddrDataList1.AddrDataCollection.Count; i++)
            {
                if(m_SearchAddrDataList1.AddrDataCollection[i].WarnningColor!= System.Windows.Media.Brushes.Gray)
                    m_SearchAddrDataList1.AddrDataCollection[i].WarnningColor = System.Windows.Media.Brushes.Transparent;
                if (m_SearchAddrDataList1.AddrDataCollection[i].LineNum == lvHexString.SelectedIndex)
                {
                    m_SearchAddrDataList1.AddrDataCollection[i].WarnningColor = System.Windows.Media.Brushes.LightBlue;
                    dgMsg1.SelectedIndex = i;
                    dgMsg1.ScrollIntoView(dgMsg1.SelectedItem);
                }
            }
            dgMsg1.SelectionChanged += dgMsg1_SelectionChanged;
        }

        /// <summary>
        /// 点击地址，定位到Hex对应行
        /// </summary>
        private void dgMsg1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lvHexString.SelectionChanged -= lvHexString_SelectionChanged;
            lvHexString.SelectedIndex = m_SearchAddrDataList1.AddrDataCollection[dgMsg1.SelectedIndex].LineNum;
            lvHexString.ScrollIntoView(lvHexString.SelectedItem);
            lvHexString.SelectionChanged += lvHexString_SelectionChanged;
        }
        #endregion

        #region 插入删除
        //保存修改过的Hex文件
        private void lvHexStringSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfg = new SaveFileDialog();
            sfg.InitialDirectory = Directory.GetCurrentDirectory().ToString();
            sfg.Filter = "(*.hex)|*.hex|All files(*.*)|*.*";
            if (sfg.ShowDialog().Value)
            {

                string[] allLines = new string[m_HexStringList.HexStringCollection.Count];
                for (int i = 0; i < allLines.Length; i++)
                    allLines[i] = m_HexStringList.HexStringCollection[i].Text;

                File.WriteAllLines(sfg.FileName,allLines);
            }
        }

        //插入行
        private void lvHexStringInsert_Click(object sender, RoutedEventArgs e)
        {
            Controls.Windows.InsertHexString ihs = new Windows.InsertHexString(m_WinLocation, m_WinSize, lvHexString.SelectedIndex, ref m_AddrDataList1, ref m_HexStringList);
            ihs.ShowDialog();
            //重新显示
            RefreshShowHexFile(ref m_HexStringList);
        }

        //删除行
        private void lvHexStringDelete_Click(object sender, RoutedEventArgs e)
        {
            if (lvHexString.SelectedItems.Count > 1)
                return;

            lvHexString.SelectionChanged -= lvHexString_SelectionChanged;
            dgMsg1.SelectionChanged -= dgMsg1_SelectionChanged;

            m_HexStringList.HexStringCollection.RemoveAt(lvHexString.SelectedIndex);

            lvHexString.SelectionChanged += lvHexString_SelectionChanged;
            dgMsg1.SelectionChanged += dgMsg1_SelectionChanged;

            //重新显示
            RefreshShowHexFile(ref m_HexStringList);
            
        }

        //修改行
        private void lvHexStringChange_Click(object sender, RoutedEventArgs e)
        {
            Controls.Windows.ChangeHexString chs = new Windows.ChangeHexString(m_WinLocation, m_WinSize, lvHexString.SelectedIndex, ref m_AddrDataList1, ref m_HexStringList);
            chs.ShowDialog();
            //重新显示
            RefreshShowHexFile(ref m_HexStringList);
        }

        //监测是否选中了一行，否则不显示插入行和删除行
        private void lvHexString_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            menuInsert.IsEnabled = menuDelete.IsEnabled = menuChange.IsEnabled = menuSave.IsEnabled = lvHexString.SelectedIndex >= 0;
            //e.Handled = true;
        }
        #endregion
    }
    #endregion


}
