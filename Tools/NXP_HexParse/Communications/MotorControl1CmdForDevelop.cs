using System;
using System.Collections.Generic;
using NXP_HexParse.Datas;


namespace NXP_HexParse.Communications
{
    /// <summary>
    /// 用于测试页面封装CAN命令
    /// </summary>
    public class MotorControl1CmdForDevelop
    {
        /// <summary>
        /// 发送ID
        /// </summary>
        public const uint m_SendCANID = 0x00c18064;
        /// <summary>
        /// 接收ID
        /// </summary>
        public const uint m_ReceiveCANID = 0x00a180c8;

        /// <summary>
        /// 开发模式下的Can配置筛选列表的结构体
        /// </summary>
        public struct Scm_DevelopCanConfig
        {
            public CANDevices.ZlgCAN.ZlgCanType canType;
            public UInt32 devIndex;
            public UInt32[] canIndexs;
            public UInt32[] baudrates;
            public List<UInt32>[] idList;  //筛选ID

            /// <summary>
            /// true仅屏蔽列表，false仅允许列表
            /// </summary>
            public bool[] sheildRule;
        }

        Scm_DevelopCanConfig m_DevelopCanConfig;

        /// <summary>
        /// CAN配置
        /// </summary>
        public Scm_DevelopCanConfig DevelopCanConfig
        {
            set { m_DevelopCanConfig = value; }
            get { return m_DevelopCanConfig; }
        }
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
        public enum Ecm_CanInitialErrorTypes : uint
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
        /// CAN通讯抽象类
        /// </summary>
        CANDevices.ZlgCAN.ClassAbstractCan m_ClsAbsCan;


        /// <summary>
        /// 推送普通byte数据
        /// </summary>
        public delegate void Delegate_DevCmdReceiveNormalBytesCallback(uint canIndex, byte[] data);
        Delegate_DevCmdReceiveNormalBytesCallback m_receiveNormalBytesCallback;


        bool m_IsConnected = false;

        #region 构造函数

        /// <summary>
        /// 构造函数，设置回调函数
        /// 不解析，仅接受数据
        /// </summary>
        public MotorControl1CmdForDevelop(Delegate_DevCmdReceiveNormalBytesCallback receiveNormalBytesCallback)
        {
            m_ClsAbsCan = new CANDevices.ZlgCAN.ClassAbstractCan(ReceiveData);

            //委托赋值
            m_receiveNormalBytesCallback = receiveNormalBytesCallback;
        }


        #endregion

        #region 连接断开
        /// <summary>
        /// CAN是否已连接
        /// </summary>
        public bool CanIsConnected
        {
            get { return m_IsConnected;  }
        }

        /// <summary>
        /// 启动所有设备，例如CAN
        /// mtclTypes为两个设备的协议类型，支持不同协议
        /// mtclTypes[0]为被测电机，mtclTypes[1]为测功机
        /// </summary>
        /// <returns>返回true表示没问题，返回false表示有问题</returns>
        public Ecm_CanInitialErrorTypes Connect( Scm_DevelopCanConfig scm_DevCanConfig) //默认250Kbps
        {
            if (m_IsConnected)
                return Ecm_CanInitialErrorTypes.ConnectError;


            m_ClsAbsCan.SetCanConfig(scm_DevCanConfig.canType,
                 scm_DevCanConfig.devIndex);
            m_DevelopCanConfig = scm_DevCanConfig; //保存CAN配置

            Ecm_CanInitialErrorTypes result = (Ecm_CanInitialErrorTypes)m_ClsAbsCan.ConnectAndInitial(scm_DevCanConfig.canIndexs, scm_DevCanConfig.baudrates);
            m_IsConnected = result == Ecm_CanInitialErrorTypes.OK ? true : false;
            return result;
        }

        /// <summary>
        /// 停止所有设备，例如CAN
        /// </summary>
        /// <returns>返回true表示没问题，返回false表示有问题</returns>
        public bool DisConnect()
        {
            if (!m_IsConnected)
                return true; 

            m_IsConnected = false;
            return m_ClsAbsCan.DisConnect();
        }
        #endregion

        #region 发送
        /// <summary>
        /// 发送测试命令
        /// </summary>
        /// <returns>返回true表示没问题，返回false表示有问题</returns>
        public DataSource.LvItemStruct SendNormalBytes(uint canIndex, byte[] datas)
        {
            uint canId = m_SendCANID;
            string str = string.Empty;
            // 发送显示
            DataSource.LvItemStruct lvItem = new DataSource.LvItemStruct();
            lvItem.ID = canId;
            lvItem.DataBytes = datas; // str;


            str = "";
            str += "扩展帧:数据帧";
            lvItem.OtherInfo = str;

            uint canIndexTemp = canIndex;
            bool sendResult = m_ClsAbsCan.ASyncSendBytes(ref canIndex, canId, datas);
            if(!sendResult)
                m_ClsAbsCan.ASyncSendBytes(ref canIndex, canId, datas);

            lvItem.OtherInfo = "CAN索引:" + canIndex.ToString();
            if (canIndex != canIndexTemp)
                lvItem.OtherInfo += " 纠正";

            return lvItem;
        }

        

        #endregion

        #region 接收
        /// <summary>
        /// 接收数据，被委托调用
        /// </summary>
        private void ReceiveData(uint canIndex, CANDevices.ZlgCAN.CanStandardData canRecDatas)
        {
            //id必须为0x00a180c8
            if (canRecDatas.canId != m_ReceiveCANID)
                return;

            if (m_receiveNormalBytesCallback != null)
                m_receiveNormalBytesCallback(canIndex, canRecDatas.datas);

            
        }
        #endregion
    }
}
