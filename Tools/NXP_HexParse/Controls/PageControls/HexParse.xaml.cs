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
    public partial class HexParse : BeatfanControls.UserControls.MyUserControlModel
    {
        #region 数据定义


        /// <summary>
        /// 定时捕获定时器
        /// </summary>
        private System.Windows.Threading.DispatcherTimer m_AutoCaptureTimer = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// DataGrid绑定源
        /// </summary>
        DataSource.AddrDataListClass m_AddrDataList1 = new DataSource.AddrDataListClass();

        /// <summary>
        /// DataGrid绑定源
        /// </summary>
        DataSource.AddrDataListClass m_AddrDataList2 = new DataSource.AddrDataListClass();

        /// <summary>
        /// 搜索DataGrid绑定源
        /// </summary>
        DataSource.AddrDataListClass m_SearchAddrDataList1 = new DataSource.AddrDataListClass();

        /// <summary>
        /// 搜索DataGrid绑定源
        /// </summary>
        DataSource.AddrDataListClass m_SearchAddrDataList2 = new DataSource.AddrDataListClass();

        /// <summary>
        /// 比对后不同的索引
        /// </summary>
        List<int> m_CompareIndexList = new List<int>();

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
        public HexParse()
        {
            InitializeComponent();
            
            m_AutoCaptureTimer.Interval = new TimeSpan(0, 0, 1);
            //m_AutoCaptureTimer.Tick += m_AutoCaptureTimer_Tick;

            m_AddrDataList1.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();
            m_SearchAddrDataList1.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();

            m_AddrDataList2.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();
            m_SearchAddrDataList2.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();

        }
        
        #endregion
        
        #region UI变换
        /// <summary>
        /// 开发模式的UI
        /// 0为CAN，1为串口
        /// </summary>
        private void Connect_UIChange(bool bState)
        {
            ;//gdUartInfo.IsEnabled =  !bState;
            ;//gdMsg.IsEnabled = gdBody.IsEnabled = bState;

            if (bState)
                ;//btnUartConnect.Content = "DisConnect";
            else
                ;//btnUartConnect.Content = "Connect";

        }

        /// <summary>
        /// 启动UI变化
        /// </summary>
        private void Run_UIChange(bool bRun)
        {

            if (bRun)
                ;//btnStartBurn.Content = "停止烧录";
            else
                ;// btnStartBurn.Content = "开始烧录";

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
        /// 选择Hex文件，并解析
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="searchDataList"></param>
        /// <returns>返回文件名</returns>
        private string ConvertHexString(ref DataSource.AddrDataListClass dataList, ref DataSource.AddrDataListClass searchDataList,string filenameStr=null)
        {
            string filename = string.Empty;
            if (string.IsNullOrEmpty(filenameStr))
            {
                OpenFileDialog opfDialog = new OpenFileDialog();
                opfDialog.InitialDirectory = Directory.GetCurrentDirectory().ToString();
                opfDialog.Filter = "(*.hex)|*.hex|All files(*.*)|*.*";
                if (opfDialog.ShowDialog().Value)
                    filename = opfDialog.FileName;

            }
            else
                filename = filenameStr;

            if (string.IsNullOrEmpty(filename))
                return filename;
                    
            #region 根据文件处理hex
            //读取数据
            string[] allLines = File.ReadAllLines(filename);

            SpecialFunctions.HexParse.MicrochipHexParse microchipHexParse = new SpecialFunctions.HexParse.MicrochipHexParse();

            List<SpecialFunctions.HexParse.MicrochipHexParse.HexParseDataOneAddrStruct> lsDataList = microchipHexParse.GetDataFromMicrochipINHX32(ref allLines);

            dataList.AddrDataCollection.Clear(); //清除原来的数据
            searchDataList.AddrDataCollection.Clear();

            //逐步增加，填充省略的地址
            Int32 fillAddr = 0;

            for (int i = 0; i < lsDataList.Count; i++)
            {
                //其中有跳过的地址，补充进入
                if (fillAddr < lsDataList[i].addr)
                {
                    #region 补全缺失地址为0xffffff
                    //补充跳过的地址为ffffff
                    //实际地址为i*2，从i*2开始，到包含的地址结束，填充地址
                    //地址仅为偶数，所以步长为2
                    for (int t = fillAddr; t < lsDataList[i].addr; t += 2)
                    {
                        DataSource.AddrData addrData1 = new DataSource.AddrData();
                        addrData1.IsInsert = true;
                        addrData1.No = t / 2;
                        addrData1.RealAddr = Convert.ToUInt32(t);
                        addrData1.Data = 0x00ffffff;
                        addrData1.Remark = "Hex无，插入FFFFFF";
                        addrData1.WarnningColor = System.Windows.Media.Brushes.LightGray;
                        dataList.AddrDataCollection.Add(addrData1);
                        searchDataList.AddrDataCollection.Add(addrData1);

                    }
                    #endregion
                    //自身
                    DataSource.AddrData addrData2 = new DataSource.AddrData();
                    addrData2.No = Convert.ToInt32(lsDataList[i].addr / 2);
                    addrData2.RealAddr = Convert.ToUInt32(lsDataList[i].addr);
                    addrData2.Data = lsDataList[i].data;
                    addrData2.LineNum = lsDataList[i].lineNum;
                    //addrData2.WarnningColor = System.Windows.Media.Brushes.LightGreen;
                    dataList.AddrDataCollection.Add(addrData2);
                    searchDataList.AddrDataCollection.Add(addrData2);
                }
                else
                {
                    DataSource.AddrData addrData = new DataSource.AddrData();
                    addrData.No = Convert.ToInt32(fillAddr / 2);
                    addrData.RealAddr = lsDataList[i].addr;
                    addrData.Data = lsDataList[i].data;
                    addrData.LineNum = lsDataList[i].lineNum;
                    //addrData.WarnningColor = System.Windows.Media.Brushes.LightGreen;
                    dataList.AddrDataCollection.Add(addrData);
                    searchDataList.AddrDataCollection.Add(addrData);

                }
                fillAddr = Convert.ToInt32(lsDataList[i].addr + 2);

            }

            #endregion

            
            return filename;
        }

        //选择Hex文件1，并解析
        private void btnSelectHexFile1_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfDialog = new OpenFileDialog();
            opfDialog.InitialDirectory = Directory.GetCurrentDirectory().ToString();
            opfDialog.Filter = "(*.hex)|*.hex|All files(*.*)|*.*";
            if (opfDialog.ShowDialog().Value)
            {
                //DataSource.HexStringListClass tmp = null;
                //tbSelectedHexFilePath1.Text = m_CommonFunc.ConvertHexString(ref tmp, ref m_AddrDataList1, opfDialog.FileName);
                //m_CommonFunc.AddrDataListClassCopy(ref m_AddrDataList1, ref m_SearchAddrDataList1);
                //dgMsg1.ItemsSource = m_SearchAddrDataList1.AddrDataCollection;
                //lbFilter1.Content = m_SearchAddrDataList1.AddrDataCollection.Count;
            }

            if (m_SearchAddrDataList1.AddrDataCollection.Count > 0 && m_SearchAddrDataList2.AddrDataCollection.Count > 0)
                btnCompare.IsEnabled = true;
            else
                btnCompare.IsEnabled = false;
        }

        //选择Hex文件2，并解析
        private void btnSelectHexFile2_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfDialog = new OpenFileDialog();
            opfDialog.InitialDirectory = Directory.GetCurrentDirectory().ToString();
            opfDialog.Filter = "(*.hex)|*.hex|All files(*.*)|*.*";
            if (opfDialog.ShowDialog().Value)
            {
                //DataSource.HexStringListClass tmp = null;
                //tbSelectedHexFilePath2.Text = m_CommonFunc.ConvertHexString(ref tmp, ref m_AddrDataList2, opfDialog.FileName);
                //m_CommonFunc.AddrDataListClassCopy(ref m_AddrDataList2, ref m_SearchAddrDataList2);
                //dgMsg2.ItemsSource = m_SearchAddrDataList2.AddrDataCollection;
                //lbFilter2.Content = m_SearchAddrDataList2.AddrDataCollection.Count;
            }

            if (m_SearchAddrDataList1.AddrDataCollection.Count > 0 && m_SearchAddrDataList2.AddrDataCollection.Count > 0)
                btnCompare.IsEnabled = true;
            else
                btnCompare.IsEnabled = false;
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
        //图标效果
        private void dgMsg2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;
        }
        //获取文件名，添加进入
        private void dgMsg1_Drop(object sender, DragEventArgs e)
        {
            //string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            //DataSource.HexStringListClass tmp = null;
            //tbSelectedHexFilePath1.Text = m_CommonFunc.ConvertHexString(ref tmp, ref m_AddrDataList1, fileName);
            //m_CommonFunc.AddrDataListClassCopy(ref m_AddrDataList1, ref m_SearchAddrDataList1);
            //dgMsg1.ItemsSource = m_SearchAddrDataList1.AddrDataCollection;
            //lbFilter1.Content = m_SearchAddrDataList1.AddrDataCollection.Count;

            if (m_SearchAddrDataList1.AddrDataCollection.Count > 0 && m_SearchAddrDataList2.AddrDataCollection.Count > 0)
                btnCompare.IsEnabled = true;
            else
                btnCompare.IsEnabled = false;
        }
        //获取文件名，添加进入
        private void dgMsg2_Drop(object sender, DragEventArgs e)
        {
            //string fileName = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            //DataSource.HexStringListClass tmp = null;
            //tbSelectedHexFilePath2.Text = m_CommonFunc.ConvertHexString(ref tmp, ref m_AddrDataList2, fileName);
            //m_CommonFunc.AddrDataListClassCopy(ref m_AddrDataList2, ref m_SearchAddrDataList2);
            //dgMsg2.ItemsSource = m_SearchAddrDataList2.AddrDataCollection;
            //lbFilter2.Content = m_SearchAddrDataList2.AddrDataCollection.Count;

            if (m_SearchAddrDataList1.AddrDataCollection.Count > 0 && m_SearchAddrDataList2.AddrDataCollection.Count > 0)
                btnCompare.IsEnabled = true;
            else
                btnCompare.IsEnabled = false;
        }
        #endregion

        #endregion

        #region 筛选
        /// <summary>
        /// 数据变化，搜索筛选
        /// </summary>
        private void SearchData(string[] no_addr_data,ref DataSource.AddrDataListClass dataList, ref DataSource.AddrDataListClass searchDataList)
        {
            string no = no_addr_data[0];
            string addr = no_addr_data[1];
            string data = no_addr_data[2];

            searchDataList.AddrDataCollection.Clear();

            for (int i = 0; i < dataList.AddrDataCollection.Count; i++)
            {
                bool checkResult1 = false, checkResult2 = false, checkResult3 = false;
                checkResult1 = string.IsNullOrEmpty(no) ||
                    (!string.IsNullOrEmpty(no) && dataList.AddrDataCollection[i].No.ToString("X6").Contains(no.ToUpper()));
                checkResult2 = string.IsNullOrEmpty(addr) ||
                    (!string.IsNullOrEmpty(addr) && dataList.AddrDataCollection[i].RealAddr.ToString("X6").Contains(addr.ToUpper()));
                checkResult3 = string.IsNullOrEmpty(data) ||
                    (!string.IsNullOrEmpty(data) && dataList.AddrDataCollection[i].Data.ToString("X6").Contains(data.ToUpper()));

                if (checkResult1 && checkResult2 && checkResult3)
                {
                    if (dataList.AddrDataCollection[i].WarnningColor == System.Windows.Media.Brushes.Yellow)
                        dataList.AddrDataCollection[i].WarnningColor = System.Windows.Media.Brushes.Transparent;
                    searchDataList.AddrDataCollection.Add(dataList.AddrDataCollection[i]);
                }
            }
            //有变化，不支持滚动到对应差异地方
            slScrollToCompare.IsEnabled = btnLastN.IsEnabled = btnNextN.IsEnabled = false; 
        }
        

        /// <summary>
        /// 筛选1
        /// </summary>
        private void tbFilter1_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchData(new string[] { tbNoSearch1.Text, tbAddrSearch1.Text, tbDataSearch1.Text },
                ref m_AddrDataList1,ref m_SearchAddrDataList1);
            lbFilter1.Content = m_SearchAddrDataList1.AddrDataCollection.Count;
        }

        /// <summary>
        /// 筛选2
        /// </summary>
        private void tbFilter2_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchData(new string[] { tbNoSearch2.Text, tbAddrSearch2.Text, tbDataSearch2.Text },
                ref m_AddrDataList2, ref m_SearchAddrDataList2);
            lbFilter2.Content = m_SearchAddrDataList2.AddrDataCollection.Count;
        }

        #endregion


        //比较
        private void btnCompare_Click(object sender, RoutedEventArgs e)
        {
            if (m_SearchAddrDataList1.AddrDataCollection.Count != m_SearchAddrDataList2.AddrDataCollection.Count)
                lbCompareMsg.Content = "长度不同";
            else
            {
                for (int i = 0; i < m_SearchAddrDataList1.AddrDataCollection.Count; i++)
                {
                    if (m_SearchAddrDataList1.AddrDataCollection[i].RealAddr != m_SearchAddrDataList2.AddrDataCollection[i].RealAddr
                        || m_SearchAddrDataList1.AddrDataCollection[i].Data != m_SearchAddrDataList2.AddrDataCollection[i].Data)
                    {
                        m_CompareIndexList.Add(i);
                        m_SearchAddrDataList1.AddrDataCollection[i].WarnningColor = m_SearchAddrDataList2.AddrDataCollection[i].WarnningColor = System.Windows.Media.Brushes.Yellow;
                    }
                }
                lbCompareMsg.Content = "比较完成，黄色标记";
            }
            //支持滚动到对应差异地方
            btnLastN.IsEnabled = false;
            btnNextN.IsEnabled = true;
            slScrollToCompare.IsEnabled = true;
            
            
            m_CompareIndexIndex = -1;
            lbCompareTotalCount.Content = m_CompareIndexList.Count;
            lbCompareCount.Content = m_CompareIndexIndex+1;
            slScrollToCompare.Maximum = m_CompareIndexList.Count - 1;
        }

        /// <summary>
        /// 滚动到相应行
        /// </summary>
        private void GoToCompareLine()
        {

            if (0 < m_CompareIndexIndex && m_CompareIndexIndex < m_CompareIndexList.Count - 1)
                btnLastN.IsEnabled = btnNextN.IsEnabled = true;
            else if (m_CompareIndexIndex == 0)
                btnLastN.IsEnabled = false;
            else if (m_CompareIndexIndex == m_CompareIndexList.Count - 1)
                btnNextN.IsEnabled = false;
            else
                return;


            dgMsg1.SelectedIndex = m_CompareIndexList[m_CompareIndexIndex];
            dgMsg1.ScrollIntoView(dgMsg1.SelectedItem);
            dgMsg2.SelectedIndex = m_CompareIndexList[m_CompareIndexIndex];
            dgMsg2.ScrollIntoView(dgMsg2.SelectedItem);
            lbCompareCount.Content = m_CompareIndexIndex+1;
        }
        //查看不同的上一个
        private void btnLastN_Click(object sender, RoutedEventArgs e)
        {
            m_CompareIndexIndex--;
            GoToCompareLine();
        }

        //滚动查看
        private void slScrollToCompare_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_CompareIndexIndex = Convert.ToInt32(slScrollToCompare.Value);
            GoToCompareLine();
        }

        //查看不同的下一个
        private void btnNextN_Click(object sender, RoutedEventArgs e)
        {
            m_CompareIndexIndex++;
            GoToCompareLine();
        }

        private void sbScrollbar_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {
            if (m_SearchAddrDataList1.AddrDataCollection.Count != m_SearchAddrDataList2.AddrDataCollection.Count)
                return;
            sbScrollbar.Maximum = m_SearchAddrDataList1.AddrDataCollection.Count - 1;

            dgMsg2.SelectedIndex = dgMsg1.SelectedIndex = Convert.ToInt32(sbScrollbar.Value);
            dgMsg1.ScrollIntoView(dgMsg1.SelectedItem);
            dgMsg2.ScrollIntoView(dgMsg2.SelectedItem);
        }



        //鼠标滚动事件
        private void sbScrollbar_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            sbScrollbar.Value -= (m_SearchAddrDataList1.AddrDataCollection.Count/100)*e.Delta/120/100;
            sbScrollbar_Scroll(sbScrollbar, null);
        }


        //保存
        private void dgMsg1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgMsg2_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    #endregion


}
