using System;
using System.Collections.Generic;
using MotorControlProtocols;
using MotorControlProtocols.PureElectric;
using MotorControlProtocols.DualSource;
using MotorControlProtocols.Hybrid;

namespace MotorControlProtocols
{

    /// <summary>
    /// 筛选列表的结构体
    /// </summary>
    public struct Scm_SheildRule
    {
        public List<UInt32> idList;  //筛选ID

        /// <summary>
        /// true仅屏蔽列表，false仅允许列表
        /// </summary>
        public bool sheildRule;
    }

    /// <summary>
    /// 详细的发送接收数据命令
    /// </summary>
    public struct Scm_CmdDataDetail
    {
        //public int CanIndex;
        public uint ID;
        public byte[] DataBytes;
        public string CmdDetail;
        public string OtherInfo;
    }

    /// <summary>
    /// 用于测试页面封装CAN命令
    /// </summary>
    public class CommonInterface
    {

        /// <summary>
        /// 电机控制器协议
        /// </summary>
        AbstractMotorControl[] m_AbstractMtcls;



        #region 构造函数
        /// <summary>
        /// 设置协议以及筛选规则
        /// </summary>
        public void MotorControlInitial(AbstractMotorControl.Ecm_MtclTypes[] mtclTypes)
        {
            m_AbstractMtcls = new AbstractMotorControl[mtclTypes.Length];
            for (int i = 0; i < mtclTypes.Length; i++)
            {
                switch (mtclTypes[i])
                {
                    case AbstractMotorControl.Ecm_MtclTypes.PureElectricTM1:  //纯电
                        m_AbstractMtcls[i] = new PureElectricTM1();
                        break;
                    case AbstractMotorControl.Ecm_MtclTypes.DualSourceTM1: //双源无轨
                        m_AbstractMtcls[i] = new DualSourceTM1();
                        break;
                    case AbstractMotorControl.Ecm_MtclTypes.HybridIsg1: //混动发电1
                        m_AbstractMtcls[i] = new HybridIsg1();
                        break;
                    case AbstractMotorControl.Ecm_MtclTypes.HybridTM1: //混动电动1
                        m_AbstractMtcls[i] = new HybridTM1();
                        break;
                    case AbstractMotorControl.Ecm_MtclTypes.HybridTM2: //混动电动2
                        m_AbstractMtcls[i] = new HybridTM2();
                        break;

                    case AbstractMotorControl.Ecm_MtclTypes.MuTeOwn1: //牟特自有协议
                        m_AbstractMtcls[i] = new OthersOwn.MotorThOwn1();
                        break;

                    case AbstractMotorControl.Ecm_MtclTypes.OilPumpMotorControl1: //电子油泵协议
                        m_AbstractMtcls[i] = new OilPump.OilPumpMotorControl1();
                        break;

                    case AbstractMotorControl.Ecm_MtclTypes.EkOilPumpMotorControl1: //Ek电子油泵协议
                        m_AbstractMtcls[i] = new EKOilPump.EkOilPumpMotorControl1();
                        break;

                    default: break;
                }
            }
            
        }

        /// <summary>
        /// 筛选
        /// </summary>
        public void SetSheild(List<uint>[] idList, bool[] sheildRule)
        {
            for (int i = 0; i < sheildRule.Length; i++)
            {
                m_AbstractMtcls[i].SetCanIdFiltRule(idList[i], sheildRule[i]);
            }

        }
        #endregion


        #region Ecu
        #region 发送

        /// <summary>
        /// 解析发送命令
        /// 00备用，01转速，02转矩，03放电
        /// </summary>
        public List<Scm_CmdDataDetail> TransformEcuASyncSendCmd(uint canIndex, AbstractMotorControl.Ecm_WorkMode workMode, short data, uint motorIndex = 0)
        {
            string str = string.Empty;// = SpecialFunctions.Converter.MyStringConverter.BytesToHexString(canSendData.datas);

            List<Scm_CmdDataDetail> cmdDataDetials = new List<Scm_CmdDataDetail>();
            
            List<AbstractMotorControl.ScmCanSendMsg> canSendDatas = m_AbstractMtcls[canIndex].TransformEcuSendData(canIndex, workMode,data, motorIndex);
            
            for(int i=0;i< canSendDatas.Count;i++)
            {

                Scm_CmdDataDetail cmdDataDetial = new Scm_CmdDataDetail();

                cmdDataDetial.OtherInfo = "CAN索引:" + canSendDatas[i].canIndex.ToString();


                cmdDataDetial.ID = canSendDatas[i].canId;
                cmdDataDetial.DataBytes = canSendDatas[i].datas; // str;
                
                cmdDataDetial.CmdDetail = canSendDatas[i].cmdDetail;

                cmdDataDetials.Add(cmdDataDetial);
                
            }
            
            return cmdDataDetials;
        }

        /// <summary>
        /// 解析清故障命令
        /// </summary>
        public AbstractMotorControl.ScmCanSendMsg TransformEcuClearFault(uint canIndex)
        {
            return m_AbstractMtcls[canIndex].EcuClearMcuFault();
        }
        #endregion

        #region 接收
        /// <summary>
        /// 解析接收数据
        /// </summary>
        public AbstractMotorControl.ScmCanReceiveMsg TransformEcuReceiveData(uint canIndex, AbstractMotorControl.CanStandardData canRecDatas)
        {
            return m_AbstractMtcls[canIndex].TransformEcuReceiveData(canRecDatas);
                       
        }
        #endregion

        #endregion

        #region Mcu
        #region 发送

        /// <summary>
        /// 解析发送命令
        /// 00备用，01转速，02转矩，03放电
        /// </summary>
        public List<Scm_CmdDataDetail> TransformMcuASyncSendCmd(uint canIndex, AbstractMotorControl.ScmCanSendMsg sendMsg, uint motorIndex = 0)
        {
            string str = string.Empty;// = SpecialFunctions.Converter.MyStringConverter.BytesToHexString(canSendData.datas);

            List<Scm_CmdDataDetail> cmdDataDetials = new List<Scm_CmdDataDetail>();

            List<AbstractMotorControl.ScmCanSendMsg> canSendDatas = m_AbstractMtcls[canIndex].TransformMcuSendData(canIndex, sendMsg, motorIndex);

            for (int i = 0; i < canSendDatas.Count; i++)
            {

                Scm_CmdDataDetail cmdDataDetial = new Scm_CmdDataDetail();

                cmdDataDetial.OtherInfo = "CAN索引:" + canSendDatas[i].canIndex.ToString();
                
                cmdDataDetial.ID = canSendDatas[i].canId;
                cmdDataDetial.DataBytes = canSendDatas[i].datas; // str;

                cmdDataDetial.CmdDetail = canSendDatas[i].cmdDetail;

                cmdDataDetials.Add(cmdDataDetial);

            }

            return cmdDataDetials;
        }

        #endregion

        #region 接收
        /// <summary>
        /// 解析接收数据
        /// </summary>
        public AbstractMotorControl.ScmCanReceiveMsg TransformMcuReceiveData(uint canIndex, AbstractMotorControl.CanStandardData canRecDatas)
        {
            return m_AbstractMtcls[canIndex].TransformMcuReceiveData(canRecDatas);

        }
        #endregion

        #endregion
    }
}
