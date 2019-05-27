using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CANDevices.ZlgCAN.PhysicalInterface;
using System.Diagnostics;
using System.Threading;


namespace CANDevices.ZlgCAN
{
    /// <summary>
    /// 周立功CAN的类型
    /// </summary>
    public enum ZlgCanType : uint
    {
        //若修改，则同时需要修改构造函数ClassUsbCan的Can类型加值，本实例为20
        PCI5121 = 1,
        PCI9810 = 2,
        USBCAN1 = 3,
        USBCAN2 = 4,
        USBCAN2A = 4,
        PCI9820 = 5,
        CAN232 = 6,
        PCI5110 = 7,
        CANLITE = 8,
        ISA9620 = 9,
        ISA5420 = 10,
        PC104CAN = 11,
        CANETUDP = 12,
        CANETE = 12,
        DNP9810 = 13,
        PCI9840 = 14,
        PC104CAN2 = 15,
        PCI9820I = 16,
        CANETTCP = 17,
        PEC9920 = 18,
        PCI5010U = 19,
        USBCAN_E_U = 20,
        USBCAN_2E_U = 21,
        PCI5020U = 22,
        EG20T_CAN = 23,
    }

    /// <summary>
    /// 一桢CAN数据结构体
    /// </summary>
    public struct CanStandardData
    {
        public uint timeStamp;
        public UInt32 canId;
        public uint baudrate;
        public byte dataType; //0-标准帧，1-扩展帧
        public byte dataFormat; //0-数据帧，1远程帧
        public byte[] datas;
    }

    /// <summary>
    /// 用户不应直接调用此类，从ClassCmdProcess中调用相应函数
    /// </summary>
    public class ClassAbstractCan
    {
        /// <summary>
        /// 连接并初始化的错误类型
        /// 1 - no error
        /// 0x0000 - connect error
        /// 0x1111 - reset error
        /// 0x2222 - set baudrate error
        /// 0x3333 - initial error
        /// 0x4444 - set filter error
        /// 0x5555 - start CAN error
        /// </summary>
        public enum Ecm_CanInitialErrorTypes:uint
        { 
            OK = 0x01,
            ConnectError = 0x0000,
            ResetError = 0x1111,
            SetBaudrateError = 0x2222,
            InitialError = 0x3333,
            SetFilterError = 0x4444,
            StartCanError = 0x5555
        }

        /// <summary>
        /// 周立功函数封装类，CAN最底层函数
        /// </summary>
        ZlgUsbCan classUsbCan;

        /// <summary>
        /// 回调函数委托类型定义
        /// </summary>
        /// <param name="canRecDatas"></param>
        public delegate void Delegate_ClaAbstCanReceiveCallback(uint canIndex, CanStandardData canRecDatas);
        /// <summary>
        /// 推送接收数据回调
        /// </summary>
        public Delegate_ClaAbstCanReceiveCallback m_receiveCallback;
        
        /// <summary>
        /// 是否已开启连接
        /// </summary>
        bool m_IsOpen;

        /// <summary>
        /// CAN所以集合，只有一路CAN的默认都是0
        /// 含有多路CAN的可以设置
        /// 例如有2路CAN，只打开第2路，new uint[]{1};
        /// 例如有2路CAN，都打开，new uint[2]{0,1}
        /// </summary>
        uint[] m_CanIndexs;

        /// <summary>
        /// 多路CAN的波特率
        /// </summary>
        uint[] m_Baudrates;

        /// <summary>
        /// 构造函数，
        /// </summary>
        public ClassAbstractCan(Delegate_ClaAbstCanReceiveCallback receiveCallback)
        {
            m_receiveCallback = receiveCallback; //接收数据回调函数
        }

        #region 配置、启动、停止
        /// <summary>
        /// 配置CAN参数
        /// devType为CAN类型
        /// devIndex为设备索引
        /// canIndexs为CAN集合，假设有2路CAN，若是只打开第2路，new uint[1]{1}，若是都打开new uint[2]{0,1}
        /// receiveCallback为设置回调函数，若是有多路CAN，通过索引区分
        /// </summary>
        /// <param name="devType"></param>
        /// <param name="devIndex"></param>
        /// <param name="canIndexs"></param>
        /// <param name="baudrate"></param>
        /// <returns></returns>
        public bool SetCanConfig(ZlgCanType devType, UInt32 devIndex)
        {

            classUsbCan = new ZlgUsbCan(devType, devIndex);

            return true;
        }


        /// <summary>
        /// 启动所有设备
        /// </summary>
        /// <returns>返回true表示没问题，返回false表示有问题</returns>
        public Ecm_CanInitialErrorTypes ConnectAndInitial(UInt32[] canIndexs, UInt32[] baudrates)
        {
            Ecm_CanInitialErrorTypes errorType = Ecm_CanInitialErrorTypes.OK;
            //连接CAN
            if (!classUsbCan.ConnectCan())
            {
                errorType = Ecm_CanInitialErrorTypes.ConnectError;
                return errorType;
            }


            m_IsOpen = true;

            m_CanIndexs = canIndexs; //CAN索引
            m_Baudrates = baudrates; //波特率集合

            //循环初始化多路CAN
            for (byte i = 0; i < m_CanIndexs.Length; i++)
            {
                UInt32 rec = classUsbCan.InitialCan(m_CanIndexs[i],m_Baudrates[i]);
                switch ( rec)
                {
                    case 1: break; //ok
                    case 0x1111: errorType = Ecm_CanInitialErrorTypes.ResetError; break;
                    case 0x2222: errorType = Ecm_CanInitialErrorTypes.SetBaudrateError; break;
                    case 0x3333: errorType = Ecm_CanInitialErrorTypes.InitialError; break;
                    case 0x4444: errorType = Ecm_CanInitialErrorTypes.SetFilterError; break;
                    case 0x5555: errorType = Ecm_CanInitialErrorTypes.StartCanError; break;
                    default: break;
                }

            }

            if (errorType == Ecm_CanInitialErrorTypes.OK)
            {
                InitCallback(); //初始化异步回调，使用回调函数接收数据
            }
            

            return errorType;
        }

        /// <summary>
        /// 断开所有设备
        /// </summary>
        /// <returns>返回true表示没问题，返回false表示有问题</returns>
        public bool DisConnect()
        {
            try
            {
                bool result = false;
                m_IsOpen = false;

                if (classUsbCan == null)
                    return true;

                //断开CAN
               result = classUsbCan.DisConnectCan();


                //StopTimer(); //停止定时器

                return result;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 发送数据
        /// <summary>
        /// 从CAN发送数据
        /// </summary>
        /// <param name="myCanId">myId表示发送时所使用的本机CAN的ID</param>
        /// <param name="datas">datas表示byte数组</param>
        /// <returns>返回true表示没问题，返回false表示有问题</returns>
        public bool ASyncSendBytes(ref uint canIndex, UInt32 myCanId, byte[] datas)
        {
            //bool result;

            if (!m_CanIndexs.Contains(canIndex)) //canIndex设置不正确
                canIndex = m_CanIndexs[0]; //默认为第一个

            CanStandardData canSendData = new CanStandardData();

            canSendData.dataFormat = 0; //数据帧
            canSendData.dataType = 1; //扩展帧
            canSendData.canId = myCanId;
            canSendData.datas = datas;

            //Send(canIndex,ref canSendData);
            //return true;

            //if (classUsbCan.SyncSendCanData(canIndex,ref canSendData) == 0)
            //    result = false;
            //else
            //    result = true;
            AutoResetEvent aWait = new AutoResetEvent(false);
            uint canIndexTmp = canIndex;
            Thread a = new Thread(() =>
            {
                classUsbCan.SyncSendCanData(canIndexTmp, ref canSendData);
                aWait.Set();
            });
            a.Start();
            aWait.WaitOne(50);
            a.Abort();

            return true;

        }
        #endregion


        #region 异步回调方法接收数据
        private delegate void ReceiveTakesAWhileDelegate();

        private void InitCallback()
        {
            ReceiveTakesAWhileDelegate receiveAsyncTask = ReceiveCanDatas;

            receiveAsyncTask.BeginInvoke(ReceiveTakesCompletedOnce, receiveAsyncTask);

        }


        /// <summary>
        /// 异步回调一次完成通知
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveTakesCompletedOnce(IAsyncResult ar)
        {
            if (ar == null) throw new ArgumentNullException("ar");

            ReceiveTakesAWhileDelegate d1 = ar.AsyncState as ReceiveTakesAWhileDelegate;

            if (m_IsOpen)
            {
                Trace.Assert(d1 != null, "Invalid object type");
                d1.BeginInvoke(ReceiveTakesCompletedOnce, d1); //再次回调
            }
            else
            {
                d1.EndInvoke(ar);
            }
        }

        
        /// <summary>
        /// 异步回调被回调函数
        /// </summary>
        private void ReceiveCanDatas()
        {
            try
            {
                if (m_IsOpen)
                {
                    //多路CAN接收，分别推送
                    for (byte i = 0; i < m_CanIndexs.Length; i++)
                    {

                        ZlgUsbCan.CanReceiveDatas canRecDatas = classUsbCan.ReceiveCanData(m_CanIndexs[i]);

                        if (!canRecDatas.isReceiveSuccess)
                            continue;

                        foreach (CanStandardData csdData in canRecDatas.receiveCanDatas)
                        {
                            m_receiveCallback(m_CanIndexs[i],csdData); //推送到上一层的回调函数
                        }
                    }
                    
                }
            }
            catch
            {
            
            }
        }
        #endregion

    }
}
