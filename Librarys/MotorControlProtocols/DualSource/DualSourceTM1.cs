using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotorControlProtocols.DualSource
{
    /// <summary>
    /// 双源无轨
    /// 适用于 双源电控，电机2暂时不考虑使用
    /// </summary>
    public class DualSourceTM1 : AbstractMotorControl
    {
        /********************************
         * Intel格式，低位在前，高位在后
         * ******************************/

        #region VCU发送命令

        /// <summary>
        /// 整车控制器1 电机1/2 驱动信息
        /// 电机1 ID:0x0C61D1D0
        /// 电机2 ID:0x0C51D2D0 
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct Scm_EVCU1_Motor1
        {
            /// <summary>
            ///电机1 CAN ID
            /// </summary>
            public const uint motor1CanId = 0x0C61D1D0;

            /// <summary>
            ///电机2 CAN ID
            /// </summary>
            public const uint motor2CanId = 0x0C51D2D0;

            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机使能指令
            /// byte0
            /// bit0，1；2bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 00无效，01清故障，02有效
            /// </summary>
            public byte dualsource_ctrl_ena_dmd_sd;

            /// <summary>
            /// 电机运行方向
            /// byte0
            /// bit2,3,4,5；4bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 00停止，01正转，10反转，从轴往后盖看，顺时针为正转
            /// </summary>
            public byte dualsource_run_dir_dmd_sd;


            /// <summary>
            /// 电机控制模式
            /// byte0
            /// bit6,7；2bits，偏移量0
            /// 范围0-3，默认值1，分辨率1
            /// 00自由转，01扭矩控制，02转速控制
            /// 转速控制时，扭矩限制，扭矩控制时，转速限制
            /// </summary>
            public byte dualsource_ctrl_mod_dmd_sd;

            /// <summary>
            /// 电机需求扭矩/电机扭矩限制
            /// byte1,byte2
            /// 16bits，偏移量-2000
            /// 范围-2000-2000，默认值-2000，分辨率1
            /// </summary>
            public short dualsource_tq_dmd_sd;

            /// <summary>
            /// 电机需求转速/电机转速限制
            /// byte3,byte4
            /// 16bits，偏移量-12000
            /// 范围-12000-12000，默认值-12000，分辨率1
            /// </summary>
            public short dualsource_spd_dmd_sd;

            //byte5/6保留

            /// <summary>
            /// 电机寿命
            /// byte8
            /// </summary>
            public byte dualsource_motor_life;

        }


        #endregion

        #region 电控回复信息


        /// <summary>
        /// 电机控制器1 状态反馈第一帧
        /// 电机1 ID:0x0C62D0D1
        /// 电机2 ID:0x0C52D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU1_Frame1
        {
            /// <summary>
            ///电机1 CAN ID
            /// </summary>
            public const uint motor1CanId = 0x0C62D0D1;

            /// <summary>
            ///电机2 CAN ID
            /// </summary>
            public const uint motor2CanId = 0x0C52D0D2;

            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps


            /// <summary>
            /// 电机实际运行方向
            /// byte0
            /// bit0,1,2,3；,4bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 00自由转，01正转，10反转，11降功率运行，100紧急停止，从轴往后盖看，顺时针为正转
            /// </summary>
            public byte dualsource_run_dir_act;

            /// <summary>
            /// 电机实际工作模式
            /// byte1
            /// bit4,5；2bits，偏移量0
            /// 范围0-8，默认值0，分辨率1
            /// 01电机转矩闭环运行,02电机转速闭环运行
            /// </summary>
            public byte dualsource_ctrl_mod_act;

            /// <summary>
            /// 故障等级
            /// bit6,7,2bits
            /// 00无故障，01一级故障(轻微)，10二级故障(一般)，11三级故障(严重)
            /// </summary>
            public byte dualsource_errorlevel;

            /// <summary>
            /// 电机实际扭矩
            /// byte1,2，偏移量-2000
            /// 范围-2000-2000，默认值-2000，分辨率1
            /// </summary>
            public short dualsource_tq_acu;

            /// <summary>
            /// 电机实际转速
            /// byte3,4，偏移量-12000
            /// 范围-12000-12000，默认值-12000，分辨率1
            /// </summary>
            public short dualsource_spd_acu;

            /// <summary>
            /// 电机在当前状态下能提供的最大扭矩
            /// byte5,6,偏移量-2000
            /// 范围-2000-2000，默认值-2000，分辨率1
            /// </summary>
            public short dualsource_tq_acu_max;


            /// <summary>
            /// 电机基本故障
            /// byte 7
            /// 00正常，01电机母线过流(390A)，02电机绕组电流过大(800A)，03逆变器温度过高(90℃)，04绕组温度过高(151℃)，05模块故障(IGBT)
            /// 06通讯故障(CAN)，07传感器故障，08电机转速超极限(>7700)，09电机转速过高(>7600)，0A电机母线电压过低(350V)，0B电机母线电压过高(750V)，
            /// 12电控低压过高(36V)，13电控低压过低(16V)，14电机过载(Is超过566A)，16电机堵转，17逆变器温度报警(80℃)，18电机温度报警(140℃)，19直流母线欠压(380V)，
            /// 1A直流母线电压过高(705V)
            /// </summary>
            public byte dualsource_warn_sts;
        }


        /// <summary>
        /// 电机控制器1 状态反馈第2帧
        /// 电机1 ID:0x1863D0D1
        /// 电机2 ID:0x1853D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU1_Frame2
        {
            /// <summary>
            ///电机1 CAN ID
            /// </summary>
            public const uint motor1CanId = 0x1863D0D1;

            /// <summary>
            ///电机2 CAN ID
            /// </summary>
            public const uint motor2CanId = 0x1853D0D2;

            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机直流母线电压，V
            /// byte0,1；偏移量0
            /// 范围0-1000，分辨率1
            /// </summary>
            public ushort dualsource_dc_u;

            /// <summary>
            /// 电机直流母线电流，A
            /// byte2,3；偏移量-500
            /// 范围-500-500，分辨率1
            /// </summary>
            public short dualsource_dc_i;

            /// <summary>
            /// 电机最大可用电压，V
            /// byte4,5；偏移量0
            /// 范围0-1000，分辨率1
            /// </summary>
            public ushort dualsource_max_ac_u;

            /// <summary>
            /// 电机三相线电流，A
            /// byte6,7；偏移量-500
            /// 范围-500-500，分辨率1
            /// </summary>
            public short dualsource_ac_i;
        }




        /// <summary>
        /// 电机控制器1 状态反馈第3帧
        /// 电机1 ID:0x1864D0D1
        /// 电机1 ID:0x1854D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU1_Frame3
        {
            /// <summary>
            ///电机1 CAN ID
            /// </summary>
            public const uint motor1CanId = 0x1864D0D1;

            /// <summary>
            ///电机2 CAN ID
            /// </summary>
            public const uint motor2CanId = 0x1854D0D2;

            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机实际电功率，kW
            /// byte0,1；偏移量-300
            /// 范围-300-300，分辨率,0.01
            /// </summary>
            public short dualsource_pwr_acu;

            /// <summary>
            /// 电机消耗电能累计，kWh
            /// byte2,3；偏移量-300
            /// 范围-300-300，分辨率,0.01，默认-300
            /// </summary>
            public short dualsource_totegy_act;

            /// <summary>
            /// 电机绕组温度，℃
            /// byte4，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short dualsource_wind_t;

            /// <summary>
            /// 电逆变器温度，℃
            /// byte5，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short dualsource_inv_t;

            /// <summary>
            /// 电机轴承温度，℃
            /// byte6，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short dualsource_bear_t;


            //byte7保留

        }

        #endregion

        #region 结构体与数组转换

        #region EVCU命令转数组
        /// <summary>
        /// 整车控制器1 驱动信息
        /// 电机1 ID:0x0C61D1D0
        /// 电机2 ID:0x0C51D2D0
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        CanStandardData ScmEvcu1ToBytes(Scm_EVCU1_Motor1 scmEvcu1,uint motorIndex=0)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = motorIndex==0?Scm_EVCU1_Motor1.motor1CanId:Scm_EVCU1_Motor1.motor2CanId;

            scmCanByte.baudrate = 250; //250K
            scmCanByte.datas = new byte[8]; //数据长度为8

            //byte0
            scmCanByte.datas[0] = (byte)(Convert.ToByte(scmEvcu1.dualsource_ctrl_ena_dmd_sd) + //电机使能指令，分辨率1，偏移量0，范围0~2,byte0,bit0,1,2bits,00无效，01有效，10清故障
                        (Convert.ToByte(scmEvcu1.dualsource_run_dir_dmd_sd) << 2) + //电机运行方向，分辨率1，偏移量0，范围0~2,byte0,bit2,3,4,5，4bits 00停止，01正转，10反转，从轴往后盖看，顺时针为正转
                        (Convert.ToByte(scmEvcu1.dualsource_ctrl_mod_dmd_sd) << 6)); //电机控制模式，分辨率1，偏移量0，默认值1，4bits,00自由转，01转速控制，02扭矩控制，11主动放电

            //byte1，byte2,电机需求扭矩/电机扭矩限制,6bits，偏移量-2000,低位在前，范围-2000-2000，默认值-2000，分辨率1
            scmCanByte.datas[1] = Convert.ToByte(scmEvcu1.dualsource_tq_dmd_sd+2000 & 0xff); //低位
            scmCanByte.datas[2] = Convert.ToByte((scmEvcu1.dualsource_tq_dmd_sd+2000>>8) & 0xff); //高位

            //byte3，byte4,电机转速扭矩/电机转速限制,6bits，偏移量-12000,低位在前，范围-12000-12000，默认值-12000，分辨率1
            scmCanByte.datas[3] = Convert.ToByte(scmEvcu1.dualsource_spd_dmd_sd+12000 & 0xff); //低位
            scmCanByte.datas[4] = Convert.ToByte((scmEvcu1.dualsource_spd_dmd_sd+12000 >> 8) & 0xff); //高位


            //byte 6/5 保留

            //byte7
            scmCanByte.datas[7] = scmEvcu1.dualsource_motor_life;

            return scmCanByte;
        }

        #endregion

        #region 数组转MCU命令
        /// <summary>
        /// 电机控制器1 反馈第1帧 使能及控制信息等反馈
        /// 电机1 ID:0x0C62D0D1
        /// 电机1 ID:0x0C52D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        Scm_MCU1_Frame1 ByteToScmMcu1Frame1(CanStandardData scmCanByte1)
        {
            Scm_MCU1_Frame1 scmMcu1Frame1 = new Scm_MCU1_Frame1();

            //byte0
            //bit0,1,2,3
            scmMcu1Frame1.dualsource_run_dir_act = (byte)(scmCanByte1.datas[0] & 0x0f); //电机实际运行方向，00停止，01正转，10反转

            //bit4,5
            scmMcu1Frame1.dualsource_ctrl_mod_act = (byte)((scmCanByte1.datas[0] >> 4) & 0x03); //工作模式，01转速环，10转矩环

            //bit6,7
            scmMcu1Frame1.dualsource_errorlevel = (byte)((scmCanByte1.datas[0] >> 6) & 0x03); //故障等级

            //byte1,2，转矩，偏移量-2000,范围-2000-2000，默认值-2000，分辨率1
            scmMcu1Frame1.dualsource_tq_acu = (short)(scmCanByte1.datas[2] * 256 + scmCanByte1.datas[1] - 2000);

            //byte3,4，速度，偏移量-12000,范围-12000-12000，默认值-12000，分辨率1
            scmMcu1Frame1.dualsource_spd_acu = (short)(scmCanByte1.datas[4] * 256 + scmCanByte1.datas[3] - 12000);

            //byte5,6，最大转矩，偏移量-2000,范围-2000-2000，默认值-2000，分辨率1
            scmMcu1Frame1.dualsource_tq_acu_max = (short)(scmCanByte1.datas[6] * 256 + scmCanByte1.datas[5] - 2000);

            //byte7，基本故障
            scmMcu1Frame1.dualsource_warn_sts = scmCanByte1.datas[7];

            return scmMcu1Frame1;
        }



        /// <summary>
        /// 电机控制器1 状态反馈第2帧 电压电流
        /// 电机1 ID:0x1863D0D1
        /// 电机1 ID:0x1853D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        Scm_MCU1_Frame2 ByteToScmMcu1Frame2(CanStandardData scmCanByte2)
        {
            Scm_MCU1_Frame2 scmMcu1Frame2 = new Scm_MCU1_Frame2();

            //byte0,1，母线电压，0-1000，分辨率1
            scmMcu1Frame2.dualsource_dc_u = (ushort)(scmCanByte2.datas[1]*256 + scmCanByte2.datas[0]);

            //byte2,3，母线电流，-500-500，偏移量-500，分辨率1
            scmMcu1Frame2.dualsource_dc_i = (short)(scmCanByte2.datas[3]*256 + scmCanByte2.datas[2] -500);

            //byte4,5，相电压，0-1000，分辨率1
            scmMcu1Frame2.dualsource_max_ac_u = (ushort)(scmCanByte2.datas[5] * 256 + scmCanByte2.datas[4]);

            //byte6,7，线电流，-500-500，偏移量-500，分辨率1
            scmMcu1Frame2.dualsource_ac_i = (short)(scmCanByte2.datas[7] * 256 + scmCanByte2.datas[6]-500);

            return scmMcu1Frame2;
        }

        /// <summary>
        /// 电机控制器1 状态反馈第3帧 功率及温度
        /// 电机1 ID:0x1864D0D1
        /// 电机1 ID:0x1854D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        /// <param name="scmCanByte3"></param>
        /// <returns></returns>
        Scm_MCU1_Frame3 ByteToScmMcu1Frame3(CanStandardData scmCanByte3)
        {
            Scm_MCU1_Frame3 scmMcu1Frame3 = new Scm_MCU1_Frame3();

            //byte0,1，实际电功率,KW，范围-300-300，分辨率,0.01
            scmMcu1Frame3.dualsource_pwr_acu = (short)((scmCanByte3.datas[1]*256+ scmCanByte3.datas[0])*0.01 - 300);

            //byte2,3，实际电能消耗KWh，范围-300-300，分辨率,0.01
            scmMcu1Frame3.dualsource_totegy_act = (short)((scmCanByte3.datas[3] * 256 + scmCanByte3.datas[2]) * 0.01 - 300);

            //byte4，绕组温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame3.dualsource_wind_t = (short)(scmCanByte3.datas[4] - 40);

            //byte5，逆变器温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame3.dualsource_inv_t = (short)(scmCanByte3.datas[5] - 40);

            //byte6，逆变器温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame3.dualsource_bear_t = (short)(scmCanByte3.datas[6] - 40);

            //byte7 保留

            return scmMcu1Frame3;
        }
        #endregion

        #endregion

        /// <summary>
        /// 清故障
        /// </summary>
        public override AbstractMotorControl.ScmCanSendMsg EcuClearMcuFault()
        {
            ScmCanSendMsg scmSendMsg = new ScmCanSendMsg();

            Scm_EVCU1_Motor1 scmEvcu1 = new Scm_EVCU1_Motor1();
            scmEvcu1.dualsource_ctrl_ena_dmd_sd = 0x01; //清故障
            scmEvcu1.dualsource_ctrl_mod_dmd_sd = 0;
            scmEvcu1.dualsource_motor_life = 0;
            scmEvcu1.dualsource_run_dir_dmd_sd = 0;
            scmEvcu1.dualsource_spd_dmd_sd = 0;
            scmEvcu1.dualsource_tq_dmd_sd = 0;

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
            
            Scm_EVCU1_Motor1 scmEvcu1 = new Scm_EVCU1_Motor1();

            ScmCanSendMsg canSendData1 = new ScmCanSendMsg();
            canSendData1.FeedBkMtclMode = cmdType;

            string strCmdDetail = string.Empty; //命令解析字符串信息

            AbstractMotorControl.Ecm_Gear gear; //0空挡，1前进，2后退
            if (data > 0)
                gear = AbstractMotorControl.Ecm_Gear.DriveGear;
            else if (data < 0)
                gear = AbstractMotorControl.Ecm_Gear.ReverseGear;
            else
                gear = AbstractMotorControl.Ecm_Gear.NeutralGear;

            switch (gear)
            {
                case Ecm_Gear.NeutralGear:
                    scmEvcu1.dualsource_run_dir_dmd_sd = 0x00;
                    strCmdDetail = "空挡" + "\r\n";
                    break;

                case Ecm_Gear.DriveGear:
                    scmEvcu1.dualsource_run_dir_dmd_sd = 0x01;
                    strCmdDetail = "D档" + "\r\n";
                    break;

                case Ecm_Gear.ReverseGear:
                    scmEvcu1.dualsource_run_dir_dmd_sd = 0x02;
                    strCmdDetail = "倒档" + "\r\n";
                    break;


                default: scmEvcu1.dualsource_run_dir_dmd_sd = 0x00; break;
            }

            //电机转速模式设定
            scmEvcu1.dualsource_ctrl_ena_dmd_sd = 0x00; //mcu使能
            

            switch (cmdType)
            {
                case Ecm_WorkMode.None: //备用
                    scmEvcu1.dualsource_ctrl_mod_dmd_sd = 0x00; //控制模式
                    strCmdDetail += "备用" + "\r\n";
                    break;


                case Ecm_WorkMode.TorqueMode: //转矩模式
                    {
                        scmEvcu1.dualsource_tq_dmd_sd = data;//偏移2000，0表示-2000
                        canSendData1.FeedBkMotorTorque = data;
                        scmEvcu1.dualsource_ctrl_ena_dmd_sd = 0x02; //mcu使能
                        scmEvcu1.dualsource_ctrl_mod_dmd_sd = 0x02; //控制模式，跟其他几种协议不一样，01为转速
                        strCmdDetail += "扭矩命令:" + data.ToString() + "\r\n";
                        break;
                    }

                case Ecm_WorkMode.SpeedMode: //转速模式
                    {
                        scmEvcu1.dualsource_spd_dmd_sd = data;  //偏移12000，0表示-12000
                        canSendData1.FeedBkMotorSpeed = data;
                        scmEvcu1.dualsource_ctrl_ena_dmd_sd = 0x02; //mcu使能
                        scmEvcu1.dualsource_ctrl_mod_dmd_sd = 0x02; //控制模式，跟其他几种协议不一样，02为转速
                        strCmdDetail += "转速命令:" + data.ToString() + "\r\n";
                        break;
                    }


                case Ecm_WorkMode.DisCharging: //放电模式
                    scmEvcu1.dualsource_ctrl_ena_dmd_sd = 0x02; //mcu使能
                    scmEvcu1.dualsource_ctrl_mod_dmd_sd = 0x03; //控制模式
                    strCmdDetail += "放电" + "\r\n";
                    break;

                default: scmEvcu1.dualsource_ctrl_mod_dmd_sd = 0x00; break;
            }
            CanStandardData canData = ScmEvcu1ToBytes(scmEvcu1,motorIndex);

            canSendData1.canId = canData.canId;
            canSendData1.baudrate = canData.baudrate;
            canSendData1.datas = canData.datas;
            
            canSendData1.canIndex = canIndex;

            canSendData1.cmdDetail = strCmdDetail;

            canSendDatas.Add(canSendData1);

            return canSendDatas;
        }

        /// <summary>
        /// 故障代码转换
        /// </summary>
        private string ReceiveErrorTransform(byte errorCode)
        {

            string errStr = string.Empty;

            switch (errorCode)
            {

                case 0:
                    return errStr;
                    break;
                case 0x01:
                    errStr = "电机母线电流过流";
                    break;
                case 0x02:
                    errStr = "电机绕组电流过大";
                    break;
                case 0x03:
                    errStr = "逆变器温度过高";
                    break;
                case 0x04:
                    errStr = "绕组温度过高";
                    break;
                case 0x05:
                    errStr = "模块故障";
                    break;
                case 0x06:
                    errStr = "通讯故障";
                    break;
                case 0x07:
                    errStr = "传感器故障";
                    break;
                case 0x08:
                    errStr = "电机转速超限";
                    break;
                case 0x09:
                    errStr = "电机转速过高";
                    break;
                case 0x0A:
                    errStr = "母线电压过低";
                    break;
                case 0x0B:
                    errStr = "母线电压过高";
                    break;
                case 0x12:
                    errStr = "低压电源电压超高极限";
                    break;
                case 0x13:
                    errStr = "低压电源电压低于低极限";
                    break;
                case 0x14:
                    errStr = "电机过载";
                    break;
                case 0x16:
                    errStr = "电机堵转";
                    break;
                case 0x17:
                    errStr = "逆变器温度报警";
                    break;
                case 0x18:
                    errStr = "电机温度报警";
                    break;
                case 0x19:
                    errStr = "母线欠压报警";
                    break;
                case 0x1A:
                    errStr = "母线过压报警";
                    break;

                default: break;
            }

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
                //if(m_receiveAbanCallback!=null)
                //    m_receiveAbanCallback(rec); //推送筛选掉的ID
                return null;
            }

            //ID未被筛选
            recMsg.canId = canRecDatas.canId;
            recMsg.baudrate = canRecDatas.baudrate;
            recMsg.datas = canRecDatas.datas;

            
            switch (canRecDatas.canId)
            {
                #region 电机1
                case Scm_MCU1_Frame1.motor1CanId: //反馈第一帧,使能，方向，扭矩，转速，故障
                    {
                        Scm_MCU1_Frame1 scmMcu1 = ByteToScmMcu1Frame1(canRecDatas);

                        //工作模式
                        recMsg.containFeedBkMtclMode = true;
                        //转速转矩与其他协议的不一样，反的
                        switch (scmMcu1.dualsource_ctrl_mod_act)
                        {
                            case 1: recMsg.FeedBkMtclMode = Ecm_WorkMode.TorqueMode; break; //转矩环
                            case 2: recMsg.FeedBkMtclMode = Ecm_WorkMode.SpeedMode; break; //转速环

                            default: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break;
                        }

                        recMsg.containFeedBkGear = true;
                        switch (scmMcu1.dualsource_run_dir_act)
                        {
                            case 0: recMsg.FeedBkGear = Ecm_Gear.NeutralGear; break;//空档
                            case 1: recMsg.FeedBkGear = Ecm_Gear.DriveGear; break;//前进
                            case 2: recMsg.FeedBkGear = Ecm_Gear.ReverseGear; break;//后退
                            default: recMsg.FeedBkGear = Ecm_Gear.ErrorGear; break;
                        }

                        //扭矩
                        recMsg.containFeedBkMotorTorque = true;
                        recMsg.FeedBkMotorTorque = scmMcu1.dualsource_tq_acu;

                        //转速
                        recMsg.containFeedBkMotorSpeed = true;
                        recMsg.FeedBkMotorSpeed = scmMcu1.dualsource_spd_acu;

                        //最大转矩
                        recMsg.containFeedBkMotorMaxTorque = true;
                        recMsg.FeedbkMotorMaxTorque = scmMcu1.dualsource_tq_acu_max;

                        //故障等级
                        recMsg.containFeedBkErrorLevel = true;
                        recMsg.FeedBkErrorLevel = scmMcu1.dualsource_errorlevel;

                        //故障码
                        recMsg.containFeedBkErrorCode = true;
                        recMsg.FeedBkErrorCode = new byte[1] { scmMcu1.dualsource_warn_sts };

                        ReceiveErrorTransform(recMsg.FeedBkErrorCode[0]);

                        recMsg.motorIndex = 0;//电机1
                    }
                    break;

                case Scm_MCU1_Frame2.motor1CanId: //反馈第二帧,电流电压
                    {
                        Scm_MCU1_Frame2 scmMcu2 = ByteToScmMcu1Frame2(canRecDatas);

                        //直流电压
                        recMsg.containFeedBkDcVoltage = true;
                        recMsg.FeedBkDcVoltage = scmMcu2.dualsource_dc_u;

                        //直流电流
                        recMsg.containFeedBkDcCurrent = true;
                        recMsg.FeedBkDcCurrent = scmMcu2.dualsource_dc_i;


                        //交流电压
                        recMsg.containFeedBkAcVoltage = true;
                        recMsg.FeedBkAcVoltage = scmMcu2.dualsource_max_ac_u;

                        //交流电流
                        recMsg.containFeedBkAcCurrent = true;
                        recMsg.FeedBkAcCurrent = scmMcu2.dualsource_ac_i;

                        recMsg.motorIndex = 0;//电机1
                    }
                    break;

                case Scm_MCU1_Frame3.motor1CanId: //反馈第三帧，功率及温度
                    {
                        Scm_MCU1_Frame3 scmMcu3 = ByteToScmMcu1Frame3(canRecDatas);

                        //功率
                        recMsg.containFeedBkActPower = true;
                        recMsg.FeedBkActPower = scmMcu3.dualsource_pwr_acu;

                        recMsg.containFeedBkMotorWindingTemp = true;
                        recMsg.FeedBkMotorWindingTemp = scmMcu3.dualsource_wind_t;

                        recMsg.containFeedBkMtclInvTemp = true;
                        recMsg.FeedBkMtclInvTemp = scmMcu3.dualsource_inv_t;

                        //dualsource_bear_t;

                        //dualsource_life_cyc;

                        recMsg.motorIndex = 0;//电机1
                    }
                    break;
                #endregion

                #region 电机2
                //反馈第一帧,使能，方向，扭矩，转速，故障
                case Scm_MCU1_Frame1.motor2CanId:
                    {
                        Scm_MCU1_Frame1 scmMcu1 = ByteToScmMcu1Frame1(canRecDatas);

                        //工作模式
                        recMsg.containFeedBkMtclMode = true;
                        //转速转矩与其他协议的不一样，反的
                        switch (scmMcu1.dualsource_ctrl_mod_act)
                        {
                            case 1: recMsg.FeedBkMtclMode = Ecm_WorkMode.TorqueMode; break; //转矩环
                            case 2: recMsg.FeedBkMtclMode = Ecm_WorkMode.SpeedMode; break; //转速环

                            default: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break;
                        }

                        recMsg.containFeedBkGear = true;
                        switch (scmMcu1.dualsource_run_dir_act)
                        {
                            case 0: recMsg.FeedBkGear = Ecm_Gear.NeutralGear; break;//空档
                            case 1: recMsg.FeedBkGear = Ecm_Gear.DriveGear; break;//前进
                            case 2: recMsg.FeedBkGear = Ecm_Gear.ReverseGear; break;//后退
                            default: recMsg.FeedBkGear = Ecm_Gear.ErrorGear; break;
                        }

                        //扭矩
                        recMsg.containFeedBkMotorTorque = true;
                        recMsg.FeedBkMotorTorque = scmMcu1.dualsource_tq_acu;

                        //转速
                        recMsg.containFeedBkMotorSpeed = true;
                        recMsg.FeedBkMotorSpeed = scmMcu1.dualsource_spd_acu;

                        //最大转矩
                        recMsg.containFeedBkMotorMaxTorque = true;
                        recMsg.FeedbkMotorMaxTorque = scmMcu1.dualsource_tq_acu_max;

                        //故障等级
                        recMsg.containFeedBkErrorLevel = true;
                        recMsg.FeedBkErrorLevel = scmMcu1.dualsource_errorlevel;

                        //故障码
                        recMsg.containFeedBkErrorCode = true;
                        recMsg.FeedBkErrorCode = new byte[1] { scmMcu1.dualsource_warn_sts };

                        ReceiveErrorTransform(recMsg.FeedBkErrorCode[0]);

                        recMsg.motorIndex = 1;//电机2
                    }
                    break;

                //反馈第二帧,电流电压
                case Scm_MCU1_Frame2.motor2CanId:
                    {
                        Scm_MCU1_Frame2 scmMcu2 = ByteToScmMcu1Frame2(canRecDatas);

                        //直流电压
                        recMsg.containFeedBkDcVoltage = true;
                        recMsg.FeedBkDcVoltage = scmMcu2.dualsource_dc_u;

                        //直流电流
                        recMsg.containFeedBkDcCurrent = true;
                        recMsg.FeedBkDcCurrent = scmMcu2.dualsource_dc_i;


                        //交流电压
                        recMsg.containFeedBkAcVoltage = true;
                        recMsg.FeedBkAcVoltage = scmMcu2.dualsource_max_ac_u;

                        //交流电流
                        recMsg.containFeedBkAcCurrent = true;
                        recMsg.FeedBkAcCurrent = scmMcu2.dualsource_ac_i;

                        recMsg.motorIndex = 1;//电机2
                    }
                    break;

                //反馈第三帧，功率及温度
                case Scm_MCU1_Frame3.motor2CanId:
                    {
                        Scm_MCU1_Frame3 scmMcu3 = ByteToScmMcu1Frame3(canRecDatas);

                        //功率
                        recMsg.containFeedBkActPower = true;
                        recMsg.FeedBkActPower = scmMcu3.dualsource_pwr_acu;

                        recMsg.containFeedBkMotorWindingTemp = true;
                        recMsg.FeedBkMotorWindingTemp = scmMcu3.dualsource_wind_t;

                        recMsg.containFeedBkMtclInvTemp = true;
                        recMsg.FeedBkMtclInvTemp = scmMcu3.dualsource_inv_t;

                        //dualsource_bear_t;

                        //dualsource_life_cyc;

                        recMsg.motorIndex = 1;//电机2
                    }
                    break;
                #endregion

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
