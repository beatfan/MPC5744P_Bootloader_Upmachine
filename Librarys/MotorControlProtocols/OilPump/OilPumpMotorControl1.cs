using System;
using System.Collections.Generic;

namespace MotorControlProtocols.OilPump
{
    /// <summary>
    /// 油泵控制
    /// </summary>
    public class OilPumpMotorControl1 : AbstractMotorControl
    {
        /********************************
         * Intel格式，低位在前，高位在后
         * ******************************/

        #region VCU发送命令

        /// <summary>
        /// TCU 驱动信息
        /// ID:0x0C41D503
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct Scm_TCU1
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x0C41D503;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机使能指令
            /// byte0
            /// bit0，1；2bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 00禁能，01清故障，02使能
            /// </summary>
            public byte eop_ctrl_ena_dmd_sd;


            /// <summary>
            /// 电机控制模式
            /// byte0
            /// bit2,3；2bits，偏移量0
            /// 范围0-3，默认值1，分辨率1
            /// 00自由转，01转速控制，02扭矩控制
            /// </summary>
            public byte eop_ctrl_mod_dmd_sd;

            /// <summary>
            /// 电机需求扭矩/电机扭矩限制
            /// byte1
            /// 8bits，偏移量0
            /// 范围0-32，默认值0，分辨率0.125
            /// </summary>
            public byte eop_tq_dmd_sd;

            /// <summary>
            /// 电机需求转速/电机转速限制
            /// byte2,byte3
            /// 16bits，偏移量0
            /// 范围0-5000，默认值0，分辨率1
            /// </summary>
            public short eop_spd_dmd_sd;

            /// <summary>
            /// 清除故障，从0变化到1清故障，持续发1不清故障
            /// byte4
            /// 1bit，偏移量0
            /// 范围0-1，默认值0，分辨率1
            /// </summary>
            public byte eop_clear_fault;

            //byte5、6、7保留

        }

        #endregion

        #region 电控回复信息
        /***************************************
         * 电控接收整车控制器发出的扭矩指令进行输出，电机控制器输出的扭矩应控制在指令范围内
         * 电机控制器具备主动放电功能
         * 电机控制器的预充由整车做
         * 电机控制器连续2s内接收不到整车控制器发送的扭矩命令，电机控制器将输出的扭矩值降为0
         * ************************************/

        /// <summary>
        /// 电机控制器1 状态反馈第一帧
        /// ID:0x0C4203D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct Scm_MCU1_Frame1
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x0C4203D5;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps


            /// <summary>
            /// 电机实际工作模式
            /// byte0
            /// bit0,1；2bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 0：无控制，1：电机转速闭环运行，2：电机转矩闭环运行
            /// </summary>
            public byte eop_ctrl_mod_act;

            /// <summary>
            /// 电机运行电流，A
            /// byte1；偏移量0
            /// 范围0-255，分辨率0.2
            /// </summary>
            public byte eop_act_i;

            /// <summary>
            /// 电机实际转速
            /// byte2,3，偏移量0
            /// 范围0-5000，默认值0，分辨率1
            /// </summary>
            public short eop_spd_acu;

            //byte4,5,6,保留

            public byte eop_error_code;
        }
        

        #endregion

        #region 结构体与数组转换

        #region EVCU命令转数组
        /// <summary>
        /// 整车控制器1 驱动信息
        /// ID:0x0C61D1D0
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        CanStandardData ScmEvcu1ToBytes(Scm_TCU1 scmTcu1)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = Scm_TCU1.canId;
            scmCanByte.baudrate = Scm_TCU1.baudrate; //250K
            scmCanByte.datas = new byte[8]; //数据长度为8

            //byte0
            scmCanByte.datas[0] = (byte)(Convert.ToByte(scmTcu1.eop_ctrl_ena_dmd_sd) + //电机使能指令，分辨率1，偏移量0，范围0~2,byte0,bit0,1,2bits,00无效，01有效，10清故障
                        (Convert.ToByte(scmTcu1.eop_ctrl_mod_dmd_sd) << 2)); //电机控制模式，分辨率1，偏移量0，默认值1，2bits,00自由转，01转速控制，02扭矩控制

            //byte1电机需求扭矩/电机扭矩限制,8bits，偏移量0,范围0-32，默认值0，分辨率0.125
            scmCanByte.datas[1] = (byte)(scmTcu1.eop_tq_dmd_sd/0.125); 

            //byte2，byte3,电机转速扭矩/电机转速限制,6bits，偏移量-0,低位在前，范围0-5000，默认值0，分辨率1
            scmCanByte.datas[2] = Convert.ToByte(scmTcu1.eop_spd_dmd_sd  & 0xff); //低位
            scmCanByte.datas[3] = Convert.ToByte((scmTcu1.eop_spd_dmd_sd  >> 8) & 0xff); //高位

            //byte4，清故障
            scmCanByte.datas[3] = scmTcu1.eop_clear_fault;

            //byte 7/6/5 保留

            return scmCanByte;
        }

        #endregion

        #region 数组转MCU命令
        /// <summary>
        /// 电机控制器1 反馈第1帧 使能及控制信息等反馈
        /// ID:0x0C62D0D1
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        Scm_MCU1_Frame1 ByteToScmMcu1Frame1(CanStandardData scmCanByte1)
        {
            Scm_MCU1_Frame1 scmMcu1Frame1 = new Scm_MCU1_Frame1();

            //byte0
            //bit0,1
            scmMcu1Frame1.eop_ctrl_mod_act = (byte)(scmCanByte1.datas[0]  & 0x03); //工作模式，0：无控制，1：电机转速闭环运行，2：电机转矩闭环运行

            //byte1，电机实际运行电流，分辨率0.2
            scmMcu1Frame1.eop_act_i = (byte)(scmCanByte1.datas[0] * 0.2);

            //byte2,3，转矩，偏移量0,范围0-32，默认值0，分辨率1
            scmMcu1Frame1.eop_spd_acu = (short)(scmCanByte1.datas[3] * 256 + scmCanByte1.datas[2]);

            //byte4、5、6保留

            //byte7故障码
            scmMcu1Frame1.eop_error_code = scmCanByte1.datas[7];

            return scmMcu1Frame1;
        }
        
        #endregion

        #endregion


        public override ScmCanSendMsg EcuClearMcuFault()
        {
            ScmCanSendMsg scmSendMsg = new ScmCanSendMsg();

            Scm_TCU1 scmEvcu1 = new Scm_TCU1();
            scmEvcu1.eop_ctrl_ena_dmd_sd = 0x01; //清故障
            scmEvcu1.eop_clear_fault = 1; //清故障
            scmEvcu1.eop_ctrl_mod_dmd_sd = 0x00;
            scmEvcu1.eop_spd_dmd_sd = 0;
            scmEvcu1.eop_tq_dmd_sd = 0;

            CanStandardData canData = ScmEvcu1ToBytes(scmEvcu1);

            scmSendMsg.canId = canData.canId;
            scmSendMsg.baudrate = canData.baudrate;
            scmSendMsg.datas = canData.datas;

            return scmSendMsg;
        }

        /// <summary>
        /// 发送命令给电控
        /// 00备用，01转速，02转矩，03放电
        /// </summary>
        /// <param name="scmSendData"></param>
        /// <returns></returns>
        public override List<ScmCanSendMsg> TransformEcuSendData(uint canIndex, Ecm_WorkMode cmdType, short data, uint motorIndex = 0)
        {
            List<ScmCanSendMsg> canSendDatas = new List<ScmCanSendMsg>();

            Scm_TCU1 scmTcu1 = new Scm_TCU1();

            ScmCanSendMsg canSendData = new ScmCanSendMsg();
            canSendData.FeedBkMtclMode = cmdType;

            string strCmdDetail = string.Empty; //命令解析字符串信息

            

            //电机转速模式设定
            scmTcu1.eop_ctrl_ena_dmd_sd = 0x00; //mcu使能
            scmTcu1.eop_clear_fault = 0x00; //不清故障

            switch (cmdType)
            {
                case Ecm_WorkMode.None: //备用
                    scmTcu1.eop_ctrl_mod_dmd_sd = 0x00; //控制模式
                    strCmdDetail += "备用" + "\r\n";
                    break;

                case Ecm_WorkMode.SpeedMode: //转速模式
                    {
                        canSendData.FeedBkMotorSpeed = scmTcu1.eop_spd_dmd_sd = Convert.ToInt16(data); 
                        scmTcu1.eop_ctrl_ena_dmd_sd = 0x02; //mcu使能
                        scmTcu1.eop_ctrl_mod_dmd_sd = 0x01; //控制模式
                        strCmdDetail += "转速命令:" + data.ToString() + "\r\n";
                        break;
                    }

                case Ecm_WorkMode.TorqueMode: //转矩模式
                    {
                        canSendData.FeedBkMotorTorque = scmTcu1.eop_tq_dmd_sd = (byte)data;
                        scmTcu1.eop_ctrl_ena_dmd_sd = 0x02; //mcu使能
                        scmTcu1.eop_ctrl_mod_dmd_sd = 0x02; //控制模式
                        strCmdDetail += "扭矩命令:" + data.ToString() + "\r\n";
                        break;
                    }

                default: scmTcu1.eop_ctrl_mod_dmd_sd = 0x00; break;
            }
            CanStandardData canData = ScmEvcu1ToBytes(scmTcu1);

            canSendData.canId = canData.canId;
            canSendData.baudrate = canData.baudrate;
            canSendData.datas = canData.datas;

            canSendData.canIndex = canIndex;

            canSendData.cmdDetail = strCmdDetail;

            canSendDatas.Add(canSendData);

            return canSendDatas;
        }

        /// <summary>
        /// 故障代码转换
        /// </summary>
        private string ReceiveErrorTransform(byte errorCode)
        {
            string errStr = string.Empty;

            if ((errorCode & 0x01) == 0x01) 
                errStr += "过温故障,";
            if (((errorCode >> 1) & 0x01) == 0x01)
                errStr += "堵转故障,";
            if (((errorCode >> 2) & 0x01) == 0x01)
                errStr += "位置传感器故障,";
            if (((errorCode >> 3) & 0x01) == 0x01)
                errStr += "供电电压超限故障,";
            if (((errorCode >> 4) & 0x01) == 0x01)
                errStr += "过流故障,";
            if (((errorCode >> 5) & 0x01) == 0x01)
                errStr += "CAN信号丢失故障,";
            if (((errorCode >> 6) & 0x01) == 0x01)
                errStr += "预驱故障,";
            if (((errorCode >> 7) & 0x01) == 0x01)
                errStr += "其他故障";

            return errStr;

        }


        /// <summary>
        /// 接收数据并处理
        /// </summary>
        /// <param name="canRecDatas"></param>
        public override ScmCanReceiveMsg TransformEcuReceiveData(CanStandardData canRecDatas)
        {
            uint rec = CanDataFilterById(canRecDatas);
            ScmCanReceiveMsg recMsg = new ScmCanReceiveMsg();


            //若是ID被筛选，则推送筛选ID
            if (rec != 0xffffffff) //筛选id
            {
                return null;
            }

            //ID未被筛选
            recMsg.canId = canRecDatas.canId;
            recMsg.baudrate = canRecDatas.baudrate;
            recMsg.datas = canRecDatas.datas;


            switch (canRecDatas.canId)
            {
                case Scm_MCU1_Frame1.canId:  //反馈第一帧,使能，方向，扭矩，转速
                    {
                        Scm_MCU1_Frame1 scmMcu1 = ByteToScmMcu1Frame1(canRecDatas);

                        //工作模式
                        recMsg.containFeedBkMtclMode = true;
                        switch (scmMcu1.eop_ctrl_mod_act)
                        {
                            case 0: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break; //初始化
                            case 1: recMsg.FeedBkMtclMode = Ecm_WorkMode.SpeedMode; break; //转速环
                            case 2: recMsg.FeedBkMtclMode = Ecm_WorkMode.TorqueMode; break; //转矩环
                            default: recMsg.FeedBkMtclMode = Ecm_WorkMode.ErrorProtected;  break;
                        }

                        //实际电流
                        recMsg.containFeedBkDcCurrent = true;
                        recMsg.FeedBkDcCurrent = scmMcu1.eop_act_i;

                        //转速
                        recMsg.containFeedBkMotorSpeed = true;
                        recMsg.FeedBkMotorSpeed = scmMcu1.eop_spd_acu;

                        //故障码
                        recMsg.containFeedBkErrorCode = true;
                        recMsg.FeedBkErrorCode = new byte[1] { scmMcu1.eop_error_code };

                        //故障名称
                        recMsg.containFeedBkErrorStr = true;
                        recMsg.FeedBkErrorStr = ReceiveErrorTransform(scmMcu1.eop_error_code);

                        break;
                    }

                    
                default:
                    //continue;  //其他ID的数据一律不接收
                    break;
            }

            return recMsg; //推送数据
        }

        #region MCU段
        public override List<ScmCanSendMsg> TransformMcuSendData(uint canIndex, AbstractMotorControl.ScmCanSendMsg sendMsg, uint motorIndex = 0) { return null; }

        public override ScmCanReceiveMsg TransformMcuReceiveData(CanStandardData canRecDatas) { return null; }
        #endregion
    }
}
