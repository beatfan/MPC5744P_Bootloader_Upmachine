using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotorControlProtocols
{
    /// <summary>
    /// 电控抽象基类
    /// </summary>
    public abstract class AbstractMotorControl
    {
        /// <summary>
        /// 电控类型
        /// </summary>
        public enum Ecm_MtclTypes:int
        {
            /// <summary>
            /// 纯电
            /// </summary>
            PureElectricTM1=0,
            /// <summary>
            /// 双源无轨
            /// </summary>
            DualSourceTM1,
            /// <summary>
            /// 混动发电Isg1
            /// </summary>
            HybridIsg1,
            /// <summary>
            /// 混动电动TM1
            /// </summary>
            HybridTM1,
            /// <summary>
            /// 混动电动TM2
            /// </summary>
            HybridTM2,
            
            /// <summary>
            /// 牟特电控自有协议
            /// </summary>
            MuTeOwn1,

            /// <summary>
            /// 行星排电子油泵电控协议
            /// </summary>
            OilPumpMotorControl1,

            /// <summary>
            /// 凯博电子油泵电控协议
            /// </summary>
            EkOilPumpMotorControl1
        }

        /// <summary>
        /// 电控工作模式
        /// 速度环，转矩环
        /// </summary>
        public enum Ecm_WorkMode:int
        {
            /// <summary>
            /// 备用，暂无
            /// </summary>
            None=0,
            /// <summary>
            /// 速度环模式
            /// </summary>
            SpeedMode,
            /// <summary>
            /// 转矩环模式
            /// </summary>
            TorqueMode,
            
            /// <summary>
            /// 放电
            /// </summary>
            DisCharging,

            /// <summary>
            /// 故障保护
            /// </summary>
            ErrorProtected

        }

        /// <summary>
        /// 档位
        /// 空档，前进档，后退档
        /// </summary>
        public enum Ecm_Gear:int
        {
            /// <summary>
            /// 空档
            /// </summary>
            NeutralGear=0, //空档
            /// <summary>
            /// 前进档
            /// </summary>
            DriveGear, //前进
            /// <summary>
            /// 倒档
            /// </summary>
            ReverseGear, //倒档
            /// <summary>
            /// 错误
            /// </summary>
            ErrorGear //错误
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
        /// 一次接收到的多个帧的结构体
        /// </summary>
        public struct CanReceiveDatas
        {
            public bool isReceiveSuccess;
            public List<CanStandardData> receiveCanDatas;
        }
        

        /// <summary>
        /// 电控Can数组结构体
        /// </summary>
        public class ScmCanSendMsg
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public uint canId;
            /// <summary>
            /// 波特率
            /// </summary>
            public uint baudrate;
            /// <summary>
            /// 要发送的数据
            /// </summary>
            public byte[] datas;

            /// <summary>
            /// 其他信息
            /// </summary>
            public string cmdDetail;

            /// <summary>
            /// CAN索引
            /// </summary>
            public uint canIndex;

            /// <summary>
            /// 电机控制模式
            /// 0x00 备用
            /// 0x01 转速模式
            /// 0x02 转矩模式
            /// 0x03 放电模式
            /// </summary>
            public Ecm_WorkMode FeedBkMtclMode;

            /// <summary>
            /// 运行方向，0空挡，1前进，2后退，3错误
            /// </summary>
            public Ecm_Gear FeedBkGear;

            /// <summary>
            /// 电机转速，仅转速控制有效
            /// </summary>
            public short FeedBkMotorSpeed;

            /// <summary>
            /// 电机扭矩，仅转矩控制有效
            /// </summary>
            public short FeedBkMotorTorque;

            /// <summary>
            /// 电机上限速度
            /// </summary>
            public ushort FeedBkMotorSpeedUpLimit;

            /// <summary>
            /// 电机下限速度
            /// </summary>
            public ushort FeedBkMotorSpeedDownLimit;

            /// <summary>
            /// 故障等级
            /// </summary>
            public byte FeedBkErrorLevel;

        }

        /// <summary>
        /// CAN接收数据
        /// </summary>
        public class ScmCanReceiveMsg
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public uint canId;

            /// <summary>
            /// 第几个电控，对于非双源的电控，都是只有一个，为0
            /// </summary>
            public uint motorIndex;

            /// <summary>
            /// 波特率
            /// </summary>
            public uint baudrate;
            /// <summary>
            /// 要发送的数据
            /// </summary>
            public byte[] datas;

            /// <summary>
            /// 时间戳
            /// </summary>
            public uint timeStamp;

            public bool containFeekBkMtclVersion;

            /// <summary>
            /// 程序版本
            /// </summary>
            public string FeekBkMtclVersion;

            public bool containFeedBkMcuEnable;

            /// <summary>
            /// MCU使能状态回馈
            /// </summary>
            public bool FeedBkMcuEnable;

            public bool containFeedBkMtclMode;
            /// <summary>
            /// 电控工作模式
            /// 00备用，01转速控制模式，10扭矩控制模式，11主动放电模式
            /// </summary>
            public Ecm_WorkMode FeedBkMtclMode;

            public bool containFeedBkGear;

            /// <summary>
            /// 转向,0空挡，1前进，2后退
            /// </summary>
            public Ecm_Gear FeedBkGear;

            /// <summary>
            /// 此回复是否包含速度信息
            /// </summary>
            public bool containFeedBkMotorSpeed;
            /// <summary>
            /// 电机速度
            /// </summary>
            public short FeedBkMotorSpeed;

            /// <summary>
            /// 回复是否包含电机反馈扭矩
            /// </summary>

            public bool containFeedBkMotorTorque;
            /// <summary>
            /// 电控回馈扭矩
            /// </summary>
            public short FeedBkMotorTorque;

            public bool containFeedBkMotorMaxTorque;
            /// <summary>
            /// 电控回馈当前状态可用最大扭矩
            /// </summary>
            public short FeedbkMotorMaxTorque;

            public bool containFeedBkActPower;
            /// <summary>
            /// 实际电功率，KW
            /// </summary>
            public short FeedBkActPower;

            public bool containFeedBkDcCurrent;
            /// <summary>
            /// 母线电流
            /// </summary>
            public float FeedBkDcCurrent;

            public bool containFeedBkDcVoltage;
            /// <summary>
            /// 母线电压
            /// </summary>
            public float FeedBkDcVoltage;

            public bool containFeedBkAcCurrent;
            /// <summary>
            /// 三相线电流
            /// </summary>
            public short FeedBkAcCurrent;

            public bool containFeedBkAcVoltage;
            /// <summary>
            /// 三相相电压
            /// </summary>
            public ushort FeedBkAcVoltage;

            public bool containFeedBkErrorLevel;

            public bool containFeedBkMcuVoltage;
            /// <summary>
            /// MCU电压
            /// </summary>
            public ushort FeedBkMcuVoltage;
            
            /// <summary>
            /// 故障等级
            /// </summary>
            public byte FeedBkErrorLevel;

            public bool containFeedBkErrorCode;
            
            /// <summary>
            /// 故障码
            /// </summary>
            public byte[] FeedBkErrorCode;

            public bool containFeedBkErrorBits;
            /// <summary>
            /// 故障位
            /// </summary>
            public byte[] FeedBkErrorBits;

            public bool containFeedBkErrorStr;
            /// <summary>
            /// 故障名称
            /// </summary>
            public string FeedBkErrorStr;

            public bool containFeedBkMotorWindingTemp;
            
            /// <summary>
            /// 电机绕组温度
            /// </summary>
            public short FeedBkMotorWindingTemp;

            public bool containFeedBkMtclInvTemp;
            /// <summary>
            /// 逆变器温度
            /// </summary>
            public short FeedBkMtclInvTemp;

            public bool containFeedBkMtclChipTemp;
            /// <summary>
            /// 控制芯片温度
            /// </summary>
            public short FeedBkMtclChipTemp;

        }
        
        /// <summary>
        /// 推送忽略掉的ID的委托
        /// </summary>
        /// <param name="canId"></param>
        public delegate void Delegate_AbstMtcl1ReceiveAbandonCallback(uint canId);
        /// <summary>
        /// 推送忽略掉的ID
        /// </summary>
        protected Delegate_AbstMtcl1ReceiveAbandonCallback m_receiveAbanCallback;

        /// <summary>
        /// 筛选ID
        /// </summary>
        protected List<UInt32> m_EcuFiltIdList = new List<uint>();

        /// <summary>
        /// 筛选规则
        /// </summary>
        protected bool m_EcuSheildRule;

        /// <summary>
        /// 筛选配置,sheildIds true表示屏蔽列表，false表示允许列表
        /// </summary>
        /// <returns></returns>
        public virtual bool SetCanIdFiltRule(List<UInt32> idList, bool sheildIds)
        {
            m_EcuFiltIdList = idList;
            m_EcuSheildRule = sheildIds;

            return true;
        }


        /// <summary>
        /// 根据ID判断此帧是否可以被忽略
        /// </summary>
        /// <param name="canRecData"></param>
        /// <returns></returns>
        protected virtual UInt32 CanDataFilterById(CanStandardData canRecData)
        {
            uint recData = 0xffffffff;

            if (m_EcuSheildRule) //仅屏蔽列表
            {
                if (m_EcuFiltIdList == null || m_EcuFiltIdList.Count==0)
                    return recData;
                if (m_EcuFiltIdList.Contains(canRecData.canId))
                {
                    recData = canRecData.canId;
                }

            }
            else //仅允许列表
            {
                if (m_EcuFiltIdList == null)
                    return canRecData.canId;

                if (m_EcuFiltIdList.Contains(canRecData.canId))
                {
                    recData = canRecData.canId;
                }
            }

            return recData;
        }

        /// <summary>
        /// 清故障
        /// </summary>
        /// <returns></returns>
        public abstract AbstractMotorControl.ScmCanSendMsg EcuClearMcuFault();

        /// <summary>
        /// 转换发送给电控的命令
        /// 00备用，01转速，02转矩，03放电
        /// motorIndex表示第几个电机，默认都是只有一个的
        /// </summary>
        public abstract List<ScmCanSendMsg> TransformEcuSendData(uint canIndex,Ecm_WorkMode workMode, short data, uint motorIndex=0);
        
        /// <summary>
        /// 将数据转换成对应协议
        /// </summary>
        public abstract ScmCanReceiveMsg TransformEcuReceiveData( CanStandardData canRecDatas);


        #region 电控端
        /// <summary>
        /// MCU发送数据
        /// </summary>
        /// <param name="canIndex"></param>
        /// <param name="workMode"></param>
        /// <param name="data"></param>
        /// <param name="motorIndex"></param>
        /// <returns></returns>
        public abstract List<ScmCanSendMsg> TransformMcuSendData(uint canIndex, AbstractMotorControl.ScmCanSendMsg sendMsg, uint motorIndex = 0);

        public abstract ScmCanReceiveMsg TransformMcuReceiveData(CanStandardData canRecDatas);
        #endregion

    }
    
}
