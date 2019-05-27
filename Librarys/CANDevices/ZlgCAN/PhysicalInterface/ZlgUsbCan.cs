using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

//本类支持周立功的致远CAN
namespace CANDevices.ZlgCAN.PhysicalInterface
{
    /// <summary>
    /// 周立功的CAN
    /// </summary>
    internal class ZlgUsbCan
    {
        #region CAN相关结构体信息
        /// <summary>
        /// 1.ZLGCAN系列接口卡信息的数据类型。
        /// </summary>
        private struct VCI_BOARD_INFO
        {
            internal UInt16 hw_Version;
            internal UInt16 fw_Version;
            internal UInt16 dr_Version;
            internal UInt16 in_Version;
            internal UInt16 irq_Num;
            internal byte can_Num;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            internal byte[] str_Serial_Num;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
            internal byte[] str_hw_Type;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            internal byte[] Reserved;
        }


        /////////////////////////////////////////////////////
        /// <summary>
        /// 2.定义CAN信息帧的数据类型。
        /// </summary>
        unsafe private struct VCI_CAN_OBJ  //使用不安全代码
        {
            internal uint ID;
            internal uint TimeStamp;
            internal byte TimeFlag;
            internal byte SendType; //0-正常发送,1-单次正常发送,2-自发自收,3-单次自发自收
            internal byte RemoteFlag;//是否是远程帧,//0-数据帧,1-远程帧
            internal byte ExternFlag;//是否是扩展帧,//0-标准帧,1-扩展帧
            internal byte DataLen;

            internal fixed byte Data[8];

            internal fixed byte Reserved[3];

        }
        ////2.定义CAN信息帧的数据类型。
        //internal struct VCI_CAN_OBJ 
        //{
        //    internal UInt32 ID;
        //    internal UInt32 TimeStamp;
        //    internal byte TimeFlag;
        //    internal byte SendType;
        //    internal byte RemoteFlag;//是否是远程帧
        //    internal byte ExternFlag;//是否是扩展帧
        //    internal byte DataLen;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        //    internal byte[] Data;
        //    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        //    internal byte[] Reserved;

        //    internal void Init()
        //    {
        //        Data = new byte[8];
        //        Reserved = new byte[3];
        //    }
        //}

        /// <summary>
        /// 3.定义CAN控制器状态的数据类型。
        /// </summary>
        private struct VCI_CAN_STATUS
        {
            internal byte ErrInterrupt;
            internal byte regMode;
            internal byte regStatus;
            internal byte regALCapture;
            internal byte regECCapture;
            internal byte regEWLimit;
            internal byte regRECounter;
            internal byte regTECounter;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            internal byte[] Reserved;
        }

        /// <summary>
        /// 4.定义错误信息的数据类型。
        /// </summary>
        private struct VCI_ERR_INFO
        {
            internal UInt32 ErrCode;
            internal byte Passive_ErrData1;
            internal byte Passive_ErrData2;
            internal byte Passive_ErrData3;
            internal byte ArLost_ErrData;
        }

        /// <summary>
        /// 5.定义初始化CAN的数据类型
        /// </summary>
        private struct VCI_INIT_CONFIG
        {
            internal UInt32 AccCode;
            internal UInt32 AccMask;
            internal UInt32 Reserved;
            internal byte Filter;
            internal byte Timing0;
            internal byte Timing1;
            internal byte Mode;
        }


        ///////// new add struct for filter /////////
        //typedef struct _VCI_FILTER_RECORD{
        //    DWORD ExtFrame;	//是否为扩展帧
        //    DWORD Start;
        //    DWORD End;
        //}VCI_FILTER_RECORD,*PVCI_FILTER_RECORD;
        private struct VCI_FILTER_RECORD
        {
            internal UInt32 ExtFrame;
            internal UInt32 Start;
            internal UInt32 End;
        }


        




        /// <summary>
        /// 一次接收到的多个帧的结构体
        /// </summary>
        internal struct CanReceiveDatas
        {
            internal bool isReceiveSuccess;
            internal List<CanStandardData> receiveCanDatas;
        }
        #endregion
        /// <summary>
        /// CAN类型
        /// </summary>
        private ZlgCanType m_CanType;  //can 类型
        /// <summary>
        /// 设备索引
        /// </summary>
        private UInt32 m_DeviceIndex; //设备索引
        /// <summary>
        /// CAN是否已经打开
        /// </summary>
        private bool m_IsOpen; //已经打开

        //private UInt32 m_canIndex; //第几路CAN

        //private UInt32 m_Baudrate; //can参数，例如波特率

        #region CAN C++ DLL库加载
        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);
        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);
        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_ReadErrInfo(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_ERR_INFO pErrInfo);
        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_ReadCANStatus(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_STATUS pCANStatus);

        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_GetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        [DllImport(@"Librarys\controlcan.dll")]
        //static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        //refType,0设置 PCI和USBCAN-E之类的波特率，1表示设置滤波，2表示启用滤波，3清除滤波，4设置重发超时，默认为4000ms
        unsafe static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, byte* pData);

        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport(@"Librarys\controlcan.dll")]
        static extern UInt32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        //[DllImport(@"Librarys\controlcan.dll")]
        //static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);
        [DllImport(@"Librarys\controlcan.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);
        #endregion

        #region CAN信息初始化
        /// <summary>
        /// 构造函数设置CAN类型及索引
        /// </summary>
        /// <param name="canType"></param>
        /// <param name="deviceIndex"></param>
        /// <param name="baudrate"></param>
        internal ZlgUsbCan(ZlgCanType canType, UInt32 deviceIndex)
        {
            m_CanType = canType;
            m_DeviceIndex = deviceIndex;
            //m_canIndex = canIndex;
            //m_Baudrate = baudrate;
        }

        /// <summary>
        /// 获取CAN类型
        /// </summary>
        /// <returns></returns>
        internal ZlgCanType GetCanType()
        {
            return (ZlgCanType)m_CanType;
        }

        /// <summary>
        /// 获取设备索引
        /// </summary>
        /// <returns></returns>
        internal UInt32 GetDeviceIndex()
        {
            return m_DeviceIndex;
        }

        #endregion

        #region 连接断开CAN

        /// <summary>
        /// 打开CAN
        /// </summary>
        private bool OpenCAN()
        {
            if (m_IsOpen)
                return true;

            try //打开CAN
            {
                if (VCI_OpenDevice((uint)m_CanType, m_DeviceIndex, 0) == 0) //1成功，0失败
                    return false;

            }
            catch
            {
                return false;
            }

            m_IsOpen = true;
            return true;
        }

        /// <summary>
        /// 关闭CAN
        /// </summary>
        private bool CloseCAN()
        {
            try
            {
                //VCI_ResetCAN((uint)m_CanType, m_DeviceIndex, canIndex);

                if (!m_IsOpen)
                    return true;

                if (VCI_CloseDevice((uint)m_CanType, m_DeviceIndex) == 0)//1成功，0失败
                    return false;

                m_IsOpen = false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 连接CAN
        /// </summary>
        /// <param name="canParams">CAN参数结构体</param>
        /// <returns>
        /// </returns>
        internal bool ConnectCan()
        {
            return OpenCAN();
        }

        /// <summary>
        /// 关闭CAN设备
        /// </summary>
        /// <returns></returns>
        internal bool DisConnectCan()
        {
            return CloseCAN();
        }



        #endregion

        #region 配置CAN

        /// <summary>
        /// 设置ID过滤，若不需要过滤，则无需使用设置此函数，使用不安全代码，返回0表示OK
        /// </summary>
        /// <param name="filterMode"></param>
        /// <param name="startId"></param>
        /// <param name="endId"></param>
        /// <returns></returns>
        unsafe private bool SetCanIndexFilter(uint canIndex)
        {
            UInt32 filterMode = 2;//禁用过滤
            UInt32 startId = 0x01; //起始id
            UInt32 endId = 0xff; //结束id

            if (filterMode == 2) //不设置滤波
                return true;

            bool result = true;

            VCI_FILTER_RECORD filterRecord = new VCI_FILTER_RECORD();
            filterRecord.ExtFrame = filterMode; //0标准帧滤波，1扩展帧滤波
            filterRecord.Start = startId;
            filterRecord.End = endId;
            //填充滤波表格
            if (VCI_SetReference((uint)m_CanType, m_DeviceIndex, canIndex, 1, (byte*)&filterRecord) == 0)
            {
                result = false;
                return result;
            }

            //按照表格设置滤波
            if (VCI_SetReference((uint)m_CanType, m_DeviceIndex, canIndex, 2, null) == 0)
            {
                result = false;
                return result;
            }

            return result;
        }



        /// <summary>
        /// 设置波特率
        /// 0 is ok
        /// 设备为PCI-5010-U,PCI-5020-U,USBCAN-E-U,USBCAN-2E-U时，使用以下函数设置设置波特率
        /// </summary>
        /// <param name="baudRate">
        ///  设置波特率，使用不安全代码
        ///  0x060003--1000Kbps
        ///  0x060004--800Kbps
        ///  0x060007--500Kbps
        ///  0x1C0008--250Kbps
        ///  0x1C0011--125Kbps
        ///  0x160023--100Kbps
        ///  0x1C002C--50Kbps
        ///  0x1600B3--20Kbps
        ///  0x1C00E0--10Kbps
        ///  0x1C01C1--5Kbps	
        /// </param>
        /// <returns></returns>
        unsafe private bool SetCanBaudrate(uint canIndex, UInt32 baudRate)
        {
            bool result = true;
            Dictionary<UInt32, UInt32> baudRateTable = new Dictionary<uint, uint>(){
                                                                                {1000,0x060003},  //1000Kbps
                                                                                {800,0x060004},
                                                                                {500,0x060007},
                                                                                {250,0x1C0008},
                                                                                {125,0x1C0011},
                                                                                {100,0x160023},
                                                                                {50,0x1C002C},
                                                                                {20,0x1600B3},
                                                                                {10,0x1C00E0},
                                                                                {5,0x1C01C1}    };  //5Kbps
            UInt32 baud = 0;// baudRateTable[baudRate];
            if (!baudRateTable.TryGetValue(baudRate, out baud))
                return false;

            //清除
            VCI_SetReference((uint)m_CanType, m_DeviceIndex, canIndex, 3, null);

            //refType,0设置 PCI和USBCAN-E之类的波特率，1表示设置滤波，2表示启用滤波，3清除滤波，4设置重发超时，默认为4000ms
            if (VCI_SetReference((uint)m_CanType, m_DeviceIndex, canIndex, 0, (byte*)&baud) == 0) //1成功，0失败
                result = false;
            return result;
        }



        /// <summary>
        /// 初始化各路CAN
        /// 1 - no error
        /// 0x1111 - reset error
        /// 0x2222 - set baudrate error
        /// 0x3333 - initial error
        /// 0x4444 - set filter error
        /// 0x5555 - start CAN error
        /// 0x6666 - clear Buffer error
        /// </summary>
        /// <param name="canIndex"></param>
        internal UInt32 InitialCan(uint canIndex, uint baudrate)
        {

            UInt32 result = 1;


            //VCI_ResetCAN((uint)m_CanType, m_DeviceIndex, canIndex); //复位

            ////清除buffer
            //VCI_ClearBuffer((uint)m_CanType, m_DeviceIndex, canIndex);

            VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();

            //设备为PCI-5010-U,PCI-5020-U,USBCAN-E-U,USBCAN-2E-U时，必须先使用SetRefrence设置波特率
            if (m_CanType == ZlgCanType.PCI5010U || m_CanType == ZlgCanType.PCI5020U || m_CanType == ZlgCanType.USBCAN_E_U || m_CanType == ZlgCanType.USBCAN_2E_U)
            {
                //设置波特率
                if (!SetCanBaudrate(canIndex, baudrate))
                {
                    VCI_CloseDevice((uint)m_CanType, m_DeviceIndex);
                    result = 0x2222;
                    return result;
                }
            }
            else
            {
                //设备不为PCI-5010-U,PCI-5020-U,USBCAN-E-U,USBCAN-2E-U时，使用以下方式设置波特率
                config.AccCode = 0x00000000;  //验收码
                config.AccMask = 0xFFFFFFFF;  //屏蔽码
                #region 波特率换算
                switch (baudrate)
                {
                    case 5:  //5Kbps
                        config.Timing0 = 0xBF;  //定时器0
                        config.Timing1 = 0xFF;  //定时器1
                        break;
                    case 10:  //10Kbps
                        config.Timing0 = 0x31;  //定时器0
                        config.Timing1 = 0x1C;  //定时器1
                        break;
                    case 20:  //20Kbps
                        config.Timing0 = 0x18;  //定时器0
                        config.Timing1 = 0x1C;  //定时器1
                        break;
                    case 40:  //40Kbps
                        config.Timing0 = 0x87;  //定时器0
                        config.Timing1 = 0xFF;  //定时器1
                        break;
                    case 50:  //50Kbps
                        config.Timing0 = 0x09;  //定时器0
                        config.Timing1 = 0x1C;  //定时器1
                        break;
                    case 80:  //80Kbps
                        config.Timing0 = 0x83;  //定时器0
                        config.Timing1 = 0xFF;  //定时器1
                        break;
                    case 100:  //100Kbps
                        config.Timing0 = 0x04;  //定时器0
                        config.Timing1 = 0x1C;  //定时器1
                        break;
                    case 125:  //125Kbps
                        config.Timing0 = 0x03;  //定时器0
                        config.Timing1 = 0x1C;  //定时器1
                        break;
                    case 200:  //200Kbps
                        config.Timing0 = 0x81;  //定时器0
                        config.Timing1 = 0xFA;  //定时器1
                        break;
                    case 250:  //250Kbps
                        config.Timing0 = 0x01;  //定时器0
                        config.Timing1 = 0x1C;  //定时器1
                        break;
                    case 400:  //400Kbps
                        config.Timing0 = 0x80;  //定时器0
                        config.Timing1 = 0xFA;  //定时器1
                        break;
                    case 500:  //500Kbps
                        config.Timing0 = 0x00;  //定时器0
                        config.Timing1 = 0x1C;  //定时器1
                        break;
                    case 666:  //666Kbps
                        config.Timing0 = 0x80;  //定时器0
                        config.Timing1 = 0xB6;  //定时器1
                        break;
                    case 800:  //800Kbps
                        config.Timing0 = 0x00;  //定时器0
                        config.Timing1 = 0x16;  //定时器1
                        break;
                    case 1000:  //5Kbps
                        config.Timing0 = 0x00;  //定时器0
                        config.Timing1 = 0x14;  //定时器1
                        break;
                    default:  //250Kbps
                        config.Timing0 = 0x01;  //定时器0
                        config.Timing1 = 0x1C;  //定时器1
                        break;
                }
                #endregion

                config.Filter = 1;  //0双滤波，1单滤波
            }

            //初始化，对于上述4个设备，滤波及波特率要放到SetReference，而pInitConfig只有Mode需要设置，其他成员忽略

            config.Mode = 0;  //0正常，1只听

            if (VCI_InitCAN((uint)m_CanType, m_DeviceIndex, canIndex, ref config) == 0) //多个通道需要多次调用，1成功，0失败
            {
                VCI_ERR_INFO vciError = new VCI_ERR_INFO();
                VCI_ReadErrInfo((uint)m_CanType, m_DeviceIndex, canIndex, ref vciError);
                VCI_CloseDevice((uint)m_CanType, m_DeviceIndex);
                result = 0x3333;
                return result;
            }


            ////设置滤波
            //if (!SetCanIndexFilter(canIndex))
            //{
            //    DisConnectCan();
            //    result = 0x4444;
            //    return result;
            //}

            //启动CAN
            if (VCI_StartCAN((uint)m_CanType, m_DeviceIndex, canIndex) == 0) //启动CAN，多路则多次调用，1成功，0失败
            {
                VCI_CloseDevice((uint)m_CanType, m_DeviceIndex);
                result = 0x5555;
                return result;
            }

            if (VCI_ClearBuffer((uint)m_CanType, m_DeviceIndex, canIndex) == 0)//1成功，0失败
                result = 0x6666;

            return result;
        }


        #endregion

        #region 发送接收CAN数据

        /// <summary>
        /// 子线程发送CAN数据
        /// </summary>
        unsafe internal UInt32 ASyncSendCanData(uint canIndex, CanStandardData canSendData)
        {
            //CanDataStc canSendData = new CanDataStc();
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            int nTimeOut = 1500;

            sendobj.SendType = 0; //正常发送
            sendobj.RemoteFlag = canSendData.dataFormat;
            sendobj.ExternFlag = canSendData.dataType;
            sendobj.ID = canSendData.canId;
            sendobj.DataLen = (byte)canSendData.datas.Length;

            for (byte i = 0; i < sendobj.DataLen; i++)
                sendobj.Data[i] = canSendData.datas[i];

            //设置发送数据超时时间
            VCI_SetReference((uint)m_CanType, m_DeviceIndex, canIndex, 1, (byte*)&nTimeOut);
            
            if (VCI_Transmit((uint)m_CanType, m_DeviceIndex, canIndex, ref sendobj, 1) == 0)  //error
                return 0;
            return 1;
        }

        /// <summary>
        /// 发送CAN数据
        /// </summary>
        unsafe internal UInt32 SyncSendCanData(uint canIndex, ref CanStandardData canSendData)
        {
            //CanDataStc canSendData = new CanDataStc();
            VCI_CAN_OBJ sendobj = new VCI_CAN_OBJ();
            int nTimeOut = 1500;

            sendobj.SendType = 0; //正常发送
            sendobj.RemoteFlag = canSendData.dataFormat;
            sendobj.ExternFlag = canSendData.dataType;
            sendobj.ID = canSendData.canId;
            sendobj.DataLen = (byte)canSendData.datas.Length;

            for (byte i = 0; i < sendobj.DataLen; i++)
                sendobj.Data[i] = canSendData.datas[i];

            //设置发送数据超时时间
            VCI_SetReference((uint)m_CanType, m_DeviceIndex, canIndex, 1, (byte*)&nTimeOut);

            
            if (VCI_Transmit((uint)m_CanType, m_DeviceIndex, canIndex, ref sendobj, 1) == 0)  //error
                return 0;
            return 1;
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns></returns>
        unsafe internal CanReceiveDatas ReceiveCanData(uint canIndex)
        {
            CanReceiveDatas canRecDatas = new CanReceiveDatas();
            UInt32 res = new UInt32();

            res = VCI_GetReceiveNum((uint)m_CanType, m_DeviceIndex, canIndex); //获取数据组数
            if (res == 0) //没有数据
            {
                canRecDatas.isReceiveSuccess = false;
                Thread.Sleep(10);
                return canRecDatas;
            }


            canRecDatas.receiveCanDatas = new List<CanStandardData>();
            canRecDatas.isReceiveSuccess = true;

            UInt32 con_maxlen = 50;

            //开辟指针
            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)con_maxlen);

            //接收数据
            res = VCI_Receive((uint)m_CanType, m_DeviceIndex, canIndex, pt, con_maxlen, 100);

            //存储数据
            for (byte i = 0; i < res; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));

                CanStandardData csdData = new CanStandardData();
                csdData.timeStamp = obj.TimeStamp;
                csdData.dataType = obj.ExternFlag;
                csdData.dataFormat = obj.RemoteFlag;
                csdData.canId = obj.ID;
                csdData.datas = new byte[obj.DataLen];
                for (byte j = 0; j < (obj.DataLen); j++)
                {
                    csdData.datas[j] = obj.Data[j];
                }
                canRecDatas.receiveCanDatas.Add(csdData);

            }

            //释放指针
            Marshal.FreeHGlobal(pt);

            return canRecDatas;
        }
        #endregion

    }
}
