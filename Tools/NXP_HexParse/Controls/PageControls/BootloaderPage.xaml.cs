using NXP_HexParse.Common;
using NXP_HexParse.Config;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using NXP_HexParse.Communications;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using NXP_HexParse.Datas;
using NXP_HexParse.Controls.Windows;

namespace NXP_HexParse.Controls.PageControls
{
    /// <summary>
    /// 常规页面
    /// </summary>
    public partial class BootloaderPage : BeatfanControls.UserControls.MyUserControlModel
    {


        #region 数据定义
        /// <summary>
        /// 是否已经打开CAN
        /// </summary>
        bool m_BOpen = false;

        /// <summary>
        /// 电控1开发命令
        /// </summary>
        MotorControl1CmdForDevelop m_Mtcl1DevelopCmd;

        /// <summary>
        /// 定时发送定时器
        /// </summary>
        private System.Windows.Threading.DispatcherTimer m_AutoSendTimer = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// Hex文件转换
        /// </summary>
        NXPHexCommonFunctions m_NxpCommonFunction = new NXPHexCommonFunctions();

        /// <summary>
        /// 烧录中
        /// </summary>
        bool m_IsBurnning = false;

        /// <summary>
        /// 发送数据是否收到回馈
        /// </summary>
        //bool m_SendHasReponse = false;

        /// <summary>
        /// CAN索引集合，即第几路CAN
        /// </summary>
        uint[] m_CanIndexs;

        /// <summary>
        /// 波特率集合
        /// </summary>
        uint[] m_Baudrates;

        /// <summary>
        /// CAN数据解析类
        /// </summary>
        ClassBootloaderForCAN m_CanDataParse = new ClassBootloaderForCAN();

        /// <summary>
        /// 所有行数据
        /// </summary>
        //List<byte[]> m_AllLineDatas;


        /// <summary>
        /// 所有行数据的拆解后的byte数组集合
        /// </summary>
        //List<ClassBootloaderForUart.OneFrame> m_AllLineSplitSendData = new List<ClassBootloaderForUart.OneFrame>();

        /// <summary>
        /// 每个地址对应的数据
        /// </summary>
        DataSource.AddrDataListClass m_AllAddrData = new DataSource.AddrDataListClass();

        /// <summary>
        /// 已发送数据统计
        /// </summary>
        int m_SendDataCount = 0;

        /// <summary>
        /// 已接收数据统计
        /// </summary>
        int m_ReceiveDataCount = 0;

        /// <summary>
        /// 是否处于Bootloader中
        /// </summary>
        bool m_InBootloader = false;


        /// <summary>
        /// 芯片用户存储部分是否已经擦除完毕
        /// </summary>
        bool m_McuFlashIsErased = false;
        #endregion

        #region 构造函数
        public BootloaderPage()
        {
            InitializeComponent();

            //初始化命令处理类
            m_Mtcl1DevelopCmd = new MotorControl1CmdForDevelop(ReceiveData);
            m_AutoSendTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            //m_AutoSendTimer.Tick += M_AutoSendTimer_Tick;

            //初始化下拉框
            InitialAll();
        }




        #endregion

        #region 初始化
        /// <summary>
        /// 初始化设备索引，第几路CAN，波特率选择框
        /// </summary>
        private void InitialAll()
        {
            //加载上次配置
            LoadConfig();

            //初始化绑定源
            InitialBindingData();

            //UI
            Connect_UIChange(false);
        }


        /// <summary>
        /// 绑定数据处理
        /// </summary>
        private void InitialBindingData()
        {
            #region 下拉框初始化
            //CAN类型初始化，与ClassUsbCan中的CanType保持一致
            string[] canTypeItems = Enum.GetNames(typeof(CANDevices.ZlgCAN.ZlgCanType));
            cbCanType.ItemsSource = canTypeItems;
            cbCanType.SelectedIndex = CommonData.normalCanCanType;

            //设备索引初始化
            int[] deviceItems = new int[] { 0, 1, 2 };
            cbDeviceIndex.ItemsSource = deviceItems;
            cbDeviceIndex.SelectedIndex = CommonData.normalCanDeviceIndex;

            //CAN索引初始化
            int[] canItems = new int[] { 0, 1, 2 };
            cbCanIndex.ItemsSource = canItems;
            cbCanIndex.SelectedIndex = CommonData.normalCanCanIndex;

            //波特率列表初始化
            int[] baudrateItems = new int[] { 1000, 800, 500, 250, 125, 100, 50, 20, 10, 5 };
            cbCanBaudrate.ItemsSource = baudrateItems;
            cbCanBaudrate.SelectedIndex = CommonData.normalCanBaudrateIndex; //默认250K
            #endregion

        }
        #endregion

        #region UI变换
        /// <summary>
        /// 开发模式的UI
        /// 0为CAN，1为串口
        /// </summary>
        private void Connect_UIChange(bool bState)
        {

            gdUartInfo.IsEnabled = !bState;
            gdMsg.IsEnabled = gdBody.IsEnabled = bState;

            if (bState)
                btnCanConnect.Content = "DisConnect";
            else
                btnCanConnect.Content = "Connect";

        }

        /// <summary>
        /// 启动UI变化
        /// </summary>
        private void RunBurn_UIChange(bool bRun)
        {
            if (btnStartBurn.Dispatcher.Thread != Thread.CurrentThread)
            {
                btnStartBurn.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (bRun)
                        btnStartBurn.Content = "停止烧录";
                    else
                        btnStartBurn.Content = "开始烧录";
                }));
            }
            else
            {
                if (bRun)
                    btnStartBurn.Content = "停止烧录";
                else
                    btnStartBurn.Content = "开始烧录";
            }

        }
        #endregion

        #region 连接断开

        /// <summary>
        /// 连接CAN
        /// </summary>
        bool Connect()
        {
            if (m_BOpen)
                return true;

            MotorControl1CmdForDevelop.Scm_DevelopCanConfig scm_DevCanConfig = new MotorControl1CmdForDevelop.Scm_DevelopCanConfig();
            scm_DevCanConfig.canType = (CANDevices.ZlgCAN.ZlgCanType)Enum.Parse(typeof(CANDevices.ZlgCAN.ZlgCanType), cbCanType.SelectedItem.ToString());
            scm_DevCanConfig.devIndex = (uint)cbDeviceIndex.SelectedIndex;
            scm_DevCanConfig.canIndexs = m_CanIndexs;
            scm_DevCanConfig.baudrates = m_Baudrates;
            scm_DevCanConfig.idList = null;
            scm_DevCanConfig.sheildRule = new bool[] { false };
            MotorControl1CmdForDevelop.Ecm_CanInitialErrorTypes result = m_Mtcl1DevelopCmd.Connect(scm_DevCanConfig);//设置参数

            if (result != MotorControl1CmdForDevelop.Ecm_CanInitialErrorTypes.OK)
            {
                return false;
            }

            m_BOpen = true;
            Connect_UIChange(true);
            return true;

        }

        /// <summary>
        /// 断开CAN
        /// </summary>
        bool DisConnect()
        {
            if (!m_BOpen)
                return true;

            if (!m_Mtcl1DevelopCmd.DisConnect())
                return false;

            m_BOpen = false;
            Connect_UIChange(false);
            return true;
        }

        /// <summary>
        /// 连接串口
        /// </summary>
        private void btnConnectCan_Click(object sender, RoutedEventArgs e)
        {
            if (m_BOpen)
            {
                if (!DisConnect())
                {
                    Controls.Windows.UserMessageBox userMsg = new Windows.UserMessageBox(m_WinLocation, m_WinSize, "CAN异常", "关闭失败!");
                    userMsg.ShowDialog();
                }
            }
            else
            {
                if (!Connect())
                {
                    Controls.Windows.UserMessageBox userMsg = new Windows.UserMessageBox(m_WinLocation, m_WinSize, "CAN异常", "打开失败!");
                    userMsg.ShowDialog();
                }
            }


        }

        #endregion

        #region 数据收发



        #region 发送数据
        /// <summary>
        /// 开始烧录
        /// </summary>
        private void btnStartBurn_Click(object sender, RoutedEventArgs e)
        {
            BurnHex();
        }

        Thread m_SendThread;
        /// <summary>
        /// 开始烧录、停止烧录
        /// </summary>
        private void BurnHex()
        {
            if (m_IsBurnning)
            {
                m_AutoSendTimer.Stop();

                m_IsBurnning = false;
                RunBurn_UIChange(m_IsBurnning);
                if (m_SendThread != null)
                    m_SendThread.Abort();
            }
            else
            {
                m_SendThread = new Thread(() =>
                {
                    DateTime startTime;

                    m_InBootloader = m_McuFlashIsErased = false;

                
                    //检查是否处于Bootloader中
                    TextBlockAddMsg("检查是否处于Bootloader中", System.Windows.Media.Brushes.Black);
                    m_Mtcl1DevelopCmd.SendNormalBytes(0, m_CanDataParse.g_CheckBootloaderCmd);
                    startTime = DateTime.Now; //开始计时
                    while (!m_InBootloader)
                    {
                        uint count = 0;
                        while (count < 100000)
                            count++;
                        if (DateTime.Now.Subtract(startTime).TotalSeconds > 5) //是否超过5s没有接收到
                            break;
                    }

                    if (!m_InBootloader)
                    {
                        TextBlockAddMsg("请先进入bootloader模式", System.Windows.Media.Brushes.Red, 1);
                        return;
                    }
                    TextBlockAddMsg("已处于Bootloader中!", System.Windows.Media.Brushes.Blue);

                    TextBlockAddMsg("检查是否已擦除Flash", System.Windows.Media.Brushes.Black);
                    m_Mtcl1DevelopCmd.SendNormalBytes(0, m_CanDataParse.CanErasePackage(m_AllAddrData.AddrDataCollection[0].RealAddr, m_AllAddrData.AddrDataCollection[m_AllAddrData.AddrDataCollection.Count-1].RealAddr));

                    startTime = DateTime.Now; //开始计时
                    while (!m_McuFlashIsErased)
                    {
                        uint count = 0;
                        while (count < 100000)
                            count++;
                        if (DateTime.Now.Subtract(startTime).TotalSeconds > 10) //是否超过10s没有接收到
                            break;
                    }
                    if (!m_McuFlashIsErased)
                    {
                        TextBlockAddMsg("擦除超时", System.Windows.Media.Brushes.Red, 1);
                        return;
                    }
                    string ts = DateTime.Now.Subtract(startTime).TotalSeconds.ToString();
                    TextBlockAddMsg("擦除完毕!，总耗时："+ts+"s", System.Windows.Media.Brushes.Blue);

                    TextBlockAddMsg("开始发送数据，总指令数: "+m_AllAddrData.AddrDataCollection.Count.ToString() , System.Windows.Media.Brushes.Black);
                    
                
                    SendData(); //发送数据

                });
                m_SendThread.Start();
                //m_AutoSendTimer.Start();

                m_IsBurnning = true;
                RunBurn_UIChange(m_IsBurnning);
            }

            
        }

        /// <summary>
        /// 发送等待接收
        /// </summary>
        /// <summary>
        /// 发送数据
        /// </summary>
        private void SendData()
        {

            m_SendDataCount = 0;
            m_ReceiveDataCount = 0;
            DateTime dataSartTime = DateTime.Now; //开始计时
            
            while (m_SendDataCount < m_AllAddrData.AddrDataCollection.Count)
            {
                //m_SendHasReponse = false;
                byte[] data = m_CanDataParse.CanDataPackage(m_AllAddrData.AddrDataCollection[m_SendDataCount]); //地址会减去0x800000

                //TextBlockAddMsg("SendData:" + BitConverter.ToString(data), System.Windows.Media.Brushes.DarkOrange);
                TextBlockChangeMsg("发送进度", "已发送指令: " + (m_SendDataCount+1).ToString() + " ", System.Windows.Media.Brushes.Gray);

                m_Mtcl1DevelopCmd.SendNormalBytes(0, data); //实际发送
                
                //m_CanDataParse.UserFlash_DataParseAddrData(data, (byte)data.Length); //测试

                m_SendDataCount++;
                double percent = m_SendDataCount * 1.0 / m_AllAddrData.AddrDataCollection.Count * 100;
                ProcessUpdate(percent);
                SendDataCheck_Wait(ref m_SendDataCount, ref m_ReceiveDataCount, 100);

                int tryAgainCount = 0;
                while (m_SendDataCount != m_ReceiveDataCount && tryAgainCount<3) //未发送成功,尝试3次
                {
                    m_Mtcl1DevelopCmd.SendNormalBytes(0, data); //再次发送
                    SendDataCheck_Wait(ref m_SendDataCount, ref m_ReceiveDataCount, 100);
                    tryAgainCount++;
                }

                if (m_SendDataCount != m_ReceiveDataCount) //发送中断
                {
                    TextBlockAddMsg("发送中断: ", System.Windows.Media.Brushes.Red);
                    return;
                }

            }
            SendDataCheck_Wait(ref m_SendDataCount, ref m_ReceiveDataCount, 300);

            //发送完毕，停止发送

            TimeSpan ts = DateTime.Now.Subtract(dataSartTime);

            TextBlockAddMsg("发送数据结束，耗时: " + ts.TotalSeconds.ToString() + "s", System.Windows.Media.Brushes.Black);
                
            m_Mtcl1DevelopCmd.SendNormalBytes(0, m_CanDataParse.g_DataEndCmd); //数据结束命令
            m_SendDataCount++;
            SendDataCheck_Wait(ref m_SendDataCount, ref m_ReceiveDataCount, 300);
            BurnHex();
            //m_CanDataParse.UserFlash_EndCheck(); //测试
            


        }

        #endregion

        #region 接收数据

        /// <summary>
        /// 接收数据，回调函数
        /// </summary>
        private void ReceiveData(uint canIndex, byte[] datas)
        {
            
            //判断回馈类型
            ClassBootloaderForCAN.CmdType cmdType = m_CanDataParse.ReceiveDataParse(ref datas);
            //TextBlockAddMsg("ReceiveData:"+BitConverter.ToString(datas), System.Windows.Media.Brushes.DarkOrange);
            switch (cmdType)
            {
                case ClassBootloaderForCAN.CmdType.EntryBootloader:
                    m_InBootloader = true;
                    BootloaderIndicator(true);
                    TextBlockAddMsg("成功进入Bootloader，"+ datas[1].ToString()+"s内无操作进入用户程序!", System.Windows.Media.Brushes.Blue);
                    break;
                
                case ClassBootloaderForCAN.CmdType.Reset:
                    m_InBootloader = false;
                    BootloaderIndicator(false);
                    TextBlockAddMsg("进入用户程序!", System.Windows.Media.Brushes.Blue);
                    break;
                case ClassBootloaderForCAN.CmdType.Data:
                    m_ReceiveDataCount++;
                    //m_SendHasReponse = true;
                    TextBlockChangeMsg("刷写进度","已刷写指令: "+m_ReceiveDataCount.ToString()+" " , System.Windows.Media.Brushes.Gray);
                    break;
                case ClassBootloaderForCAN.CmdType.DataEnd:
                    //m_SendHasReponse = true;
                    m_ReceiveDataCount++;
                    TextBlockAddMsg("用户程序刷新完毕!", System.Windows.Media.Brushes.Blue);
                                       
                    break;
                case ClassBootloaderForCAN.CmdType.CheckBootloader:
                    m_InBootloader = true;
                    break;
                case ClassBootloaderForCAN.CmdType.Earse:
                    
                    m_McuFlashIsErased = true;
                    
                    break;
                case ClassBootloaderForCAN.CmdType.Other:
                    TextBlockAddMsg("Receive Other: " + BitConverter.ToString(datas), System.Windows.Media.Brushes.DarkOrange);
                    break;
                default: break;
            }
        }

        #endregion

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

            if (m_BOpen)
            {
                DisConnect();
            }

            m_AutoSendTimer.Stop();

            //保存当前配置
            SaveConfig();
            
        }


        /// <summary>
        /// 复位MCU
        /// </summary>
        private void btnResetMcu_Click(object sender, RoutedEventArgs e)
        {
            TextBlockAddMsg("Reset Command!", System.Windows.Media.Brushes.Black);
            m_Mtcl1DevelopCmd.SendNormalBytes(0, m_CanDataParse.g_ResetCmd);
        }

        /// <summary>
        /// 进入BootLoader
        /// </summary>
        private void btnEnterBootloader_Click(object sender, RoutedEventArgs e)
        {
            TextBlockAddMsg("Enter Bootloader Command!", System.Windows.Media.Brushes.Black);
            m_Mtcl1DevelopCmd.SendNormalBytes(0, m_CanDataParse.g_EntryBootloaderCmd);
        }

        /// <summary>
        /// 获取文件路径之后的操作
        /// </summary>
        /// <param name="filePath"></param>
        private void FileSelect(string filePath)
        {
            //读取数据
            string[] allLines = File.ReadAllLines(filePath);

            tbSelectedHexFilePath.Text = filePath;
            tbSelectedHexFilePath.ToolTip = "总行数: " + allLines.Length.ToString();

            TextBlockAddMsg("Hex文件加载完毕，" + tbSelectedHexFilePath.ToolTip, System.Windows.Media.Brushes.Green);

            m_AllAddrData = m_NxpCommonFunction.ConvertStringToDataList(allLines);
            

        }
        /// <summary>
        /// 选择Hex文件并解析
        /// </summary>
        private void btnSelectHexFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog opfDialog = new OpenFileDialog();
            opfDialog.InitialDirectory = Directory.GetCurrentDirectory().ToString();
            opfDialog.Filter = "(*.hex)|*.hex|All files(*.*)|*.*";
            if (opfDialog.ShowDialog().Value)
            {
                FileSelect(opfDialog.FileName);
            }
        }

        #region 拖拽获取文件
        //获取文件名，添加进入
        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            string filePath = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            FileSelect(filePath);
        }
        //图标效果
        private void TextBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }


        #endregion


        //清除信息
        private void btnClearMsg_Click(object sender, RoutedEventArgs e)
        {
            paraMsg.Inlines.Clear();
        }

        //索引变化
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbCanIndex.SelectedIndex)
            {
                case 0:
                case 1:
                    m_CanIndexs = new uint[1] { (uint)cbCanIndex.SelectedIndex }; //保存CAN索引的设置
                    m_Baudrates = new uint[1] { Convert.ToUInt32(cbCanBaudrate.SelectedItem) };
                    break;

                case 2:
                    m_CanIndexs = new uint[2] { 0, 1 }; //保存CAN索引的设置
                    m_Baudrates = new uint[2] { Convert.ToUInt32(cbCanBaudrate.SelectedItem), Convert.ToUInt32(cbCanBaudrate.SelectedItem) };
                    break;
                default: break;
            }
        }
        #endregion


        #region 配置文件
        /// <summary>
        /// 加载上一次配置
        /// </summary>
        private void LoadConfig()
        {
            ClassUser clsUser = new ClassUser();
            //CAN信息
            clsUser.GetNormalDefaultCanType();
            clsUser.GetNormalDefaultDeviceIndex();
            clsUser.GetNormalDefaultCanIndex();
            clsUser.GetNormalDefaultCanBaudRateIndex();

            //发送
            clsUser.GetNormalDefaultSendTimers();
            clsUser.GetNormalDefaultSendGap();

            //自动保存的路径
            clsUser.GetNormalDefaultShowDataSaveInfo();


        }

        /// <summary>
        /// 保存本次配置
        /// </summary>
        private void SaveConfig()
        {
            ClassUser clsUser = new ClassUser();
            //CAN信息
            clsUser.WriteNormalDefaultCanType(cbCanType.SelectedIndex);
            clsUser.WriteNormalDefaultDeviceIndex(cbDeviceIndex.SelectedIndex);
            clsUser.WriteNormalDefaultCanIndex(cbCanIndex.SelectedIndex);
            clsUser.WriteNormalDefaultCanBaudRateIndex(cbCanBaudrate.SelectedIndex);

        }


        #endregion


        #region 其它函数

        /// <summary>
        /// 进度更新
        /// </summary>
        /// <param name="percent"></param>
        private void ProcessUpdate(double percent)
        {
            //判断是否跨线程更新UI
            if (psbProgress.Dispatcher.Thread != Thread.CurrentThread)
            {
                psbProgress.Dispatcher.BeginInvoke(new Action(() =>
                {
                    psbProgress.Value = percent;
                }));
            }
            else
                psbProgress.Value = percent;
        }

        /// <summary>
        /// 是否处于Bootloader中
        /// </summary>
        /// <param name="isOn"></param>
        private void BootloaderIndicator(bool isOn)
        {
            //判断是否跨线程更新UI
            if (sIDstatusOfMcuReset.Dispatcher.Thread != Thread.CurrentThread)
            {
                sIDstatusOfMcuReset.Dispatcher.BeginInvoke(new Action(() =>
                {
                    BootloaderResetIndicator(isOn);
                }));
            }
            else
                BootloaderResetIndicator(isOn);
        }
        /// <summary>
        /// 是否处于Bootloader中
        /// </summary>
        /// <param name="isOn"></param>
        private void BootloaderResetIndicator(bool isOn)
        {
            if (isOn)
            {
                sIDstatusOfMcuReset.IndicatorColor = BeatfanControls.Indicators.StateIndicator1.StateIndicatorColor.Red;
                sIDstatusOfBootloader.IndicatorColor = BeatfanControls.Indicators.StateIndicator1.StateIndicatorColor.Green;
            }
            else
            {
                sIDstatusOfMcuReset.IndicatorColor = BeatfanControls.Indicators.StateIndicator1.StateIndicatorColor.Green;
                sIDstatusOfBootloader.IndicatorColor = BeatfanControls.Indicators.StateIndicator1.StateIndicatorColor.Red;
            }
        }



        private void TextBlockAddMsg(string msg, System.Windows.Media.Brush color, int enterNumBefore = 0, int enterNumAfter = 1)
        {
            //判断是否跨线程更新UI
            if (paraMsg.Dispatcher.Thread != Thread.CurrentThread)
            {
                paraMsg.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ParagraphAddMsg(msg,color,enterNumBefore, enterNumAfter);
                }));
            }
            else
                ParagraphAddMsg(msg, color, enterNumBefore, enterNumAfter);
        }

        /// <summary>
        /// 信息显示
        /// enterNumBefore为信息前方回车数据
        /// </summary>
        private void ParagraphAddMsg(string msg, System.Windows.Media.Brush color, int enterNumBefore = 0, int enterNumAfter = 1)
        {
            System.Windows.Documents.Run line = new System.Windows.Documents.Run();
            string data = "";
            for (int i = 0; i < enterNumBefore; i++)
            {
                data += "\r\n";
            }
            data += msg;
            for (int i = 0; i < enterNumAfter; i++)
            {
                data += "\r\n";
            }
            line.Text = data;
            line.Foreground = color;
            paraMsg.Inlines.Add(line);
            rtbMsg.ScrollToEnd();
        }

        private void TextBlockChangeMsg(string id,string msg, System.Windows.Media.Brush color, int enterNumBefore = 0, int enterNumAfter = 1)
        {
            //判断是否跨线程更新UI
            if (paraMsg.Dispatcher.Thread != Thread.CurrentThread)
            {
                paraMsg.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ParagraphChangeMsg(id,msg, color, enterNumBefore, enterNumAfter);
                }));
            }
            else
                ParagraphChangeMsg(id,msg, color, enterNumBefore, enterNumAfter);
        }

        /// <summary>
        /// 修改信息显示，若不存在，则创建一个
        /// enterNumBefore为信息前方回车数据
        /// </summary>
        private void ParagraphChangeMsg(string id,string msg, System.Windows.Media.Brush color, int enterNumBefore = 0, int enterNumAfter = 1)
        {
            string data = "";
            for (int i = 0; i < enterNumBefore; i++)
            {
                data += "\r\n";
            }
            data += msg;
            for (int i = 0; i < enterNumAfter; i++)
            {
                data += "\r\n";
            }

            int count = 0;
            foreach (Inline ina in paraMsg.Inlines)
            {
                if (count == paraMsg.Inlines.Count - 2 && ina.Name == id)
                {
                    System.Windows.Documents.Run line = ina as System.Windows.Documents.Run;
                    line.Text = data;
                    line.Foreground = color;
                    return;
                }
                count++;
            }
            if (paraMsg.Inlines.LastInline.Name == id)
            {
                
                System.Windows.Documents.Run line = paraMsg.Inlines.LastInline as System.Windows.Documents.Run;
                line.Text = data;
                line.Foreground = color;
            }
            else
            {
                System.Windows.Documents.Run line = new System.Windows.Documents.Run();
                line.Name = id;
                
                line.Text = data;
                line.Foreground = color;
                paraMsg.Inlines.Add(line);
                rtbMsg.ScrollToEnd();
            }
        }


        #endregion

        private void btnShowMsg_Click(object sender, RoutedEventArgs e)
        {
            DataGridTest dgt = new DataGridTest(m_WinLocation, m_WinSize, ref m_CanDataParse.test_AllAddrData);
            dgt.ShowDialog();
        }


        void SendDataCheck_Wait(ref int sendCount,ref int recCount, int timeoutMilliSeconds)
        {
            DateTime startTime = DateTime.Now; //开始计时
            while (sendCount != recCount)
            {
                uint count = 0;
                while (count < 100)
                    count++;
                if (DateTime.Now.Subtract(startTime).TotalMilliseconds > timeoutMilliSeconds) //是否超过10s没有接收到
                    break;
            }
        }
    }
}
