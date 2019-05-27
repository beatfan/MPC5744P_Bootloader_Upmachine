using System;
using System.Collections.Generic;



namespace MotorControlProtocols.EKOilPump
{
    /// <summary>
    /// 油泵控制
    /// </summary>
    public class EkOilPumpMotorControl1 : AbstractMotorControl
    {
        /********************************
         * Intel格式，低位在前，高位在后
         * ******************************/


        /// <summary>
        /// 整车控制器1 驱动信息
        /// ID:0x0C41D503
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct ElecOilPump_Scm_EVCU1
        {
            public const uint canId = 0x0C41D5D3;
            /// <summary>
            /// 电机使能指令
            /// byte 0
            /// bit0,1；2bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 00停机，01清故障，10启动电机
            /// </summary>
            public byte running_enable;

            /// <summary>
            /// 电机控制模式
            /// byte0
            /// bit2,3；2bits，偏移量0
            /// 范围0-3，默认值1，分辨率1
            /// 00自由转，01扭矩控制，02转速控制
            /// </summary>
            public byte control_mode;

            /// <summary>
            /// 电机需求扭矩/电机扭矩限制
            /// byte 1
            /// 8bits，偏移量0
            /// 范围0-32，默认值0，分辨率0.125
            /// </summary>
            public byte request_torque;

            /// <summary>
            /// 电机需求转速/电机转速限制
            /// byte 2,3
            /// 16bits，偏移量0
            /// 范围0-5000，默认值0，分辨率1
            /// </summary>
            public ushort request_speed;


            //byte5、6、7保留

        }
        


        /***************************************
         * 电控接收整车控制器发出的扭矩指令进行输出，电机控制器输出的扭矩应控制在指令范围内
         * 电机控制器具备主动放电功能
         * 电机控制器的预充由整车做
         * 电机控制器连续2s内接收不到整车控制器发送的扭矩命令，电机控制器将输出的扭矩值降为0
         * ************************************/

        /// <summary>
        /// 电机控制器1 状态反馈第一帧
        /// ID:0x0C42D3D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct ElecOilPump_Scm_MCU1_Frame1
        {
            public const uint canId = 0x0C42D3D5;
            /// <summary>
            /// 电机实际使能状态
            /// byte0
            /// bit0,1；2bits，偏移量0
            /// 范围0-1，默认值0，分辨率1
            /// 00未使能，01使能
            /// </summary>
            public byte actual_running_enable;


            /// <summary>
            /// 电机实际工作模式
            /// byte0
            /// bit2,3；2bits，偏移量0
            /// 范围0-8，默认值0，分辨率1
            /// 00-无控制，01-扭矩，10-转速控制
            /// </summary>
            public byte actual_control_mode;

            /// <summary>
            /// 电机实际扭矩
            /// byte1
            /// 8bits，偏移量0
            /// 范围0-32，默认值0，分辨率0.125
            /// </summary>
            public byte actual_torque;

            /// <summary>
            /// 电机实际转速
            /// byte2,3
            /// 16bits，偏移量0
            /// 范围0-5000，默认值0，分辨率1
            /// </summary>
            public ushort actual_speed;

            //byte4、5保留

            /// <summary>
            /// 故障等级
            /// byte6,偏移量0
            /// 范围0-4，默认值0，分辨率1
            /// </summary>
            public byte error_level;

            /// <summary>
            /// 故障码
            /// byte7,偏移量0
            /// 范围0-255，默认值0，分辨率1
            /// </summary>
            public byte error_code;
        }
        


        /// <summary>
        /// 电机控制器1 状态反馈第2帧
        /// ID:0x0C43D3D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct ElecOilPump_Scm_MCU1_Frame2
        {
            public const uint canId = 0x0C43D3D5;
            /// <summary>
            /// 电机直流母线电流，A
            /// byte0；偏移量0
            /// 范围0-255，分辨率1
            /// </summary>
            public byte dc_current;

            /// <summary>
            /// 电机直流母线电压，V
            /// byte1,2；偏移量0
            /// 范围0-1000，分辨率1
            /// </summary>
            public ushort dc_voltage;

            /// <summary>
            /// 电机三相线电流，A
            /// byte3；偏移量0
            /// 范围0-255，分辨率1
            /// </summary>
            public byte ac_current;

            /// <summary>
            /// 电机三相相电压，V
            /// byte4,5；偏移量0
            /// 范围0-1000，分辨率1
            /// </summary>
            public ushort ac_voltage;

            /// <summary>
            /// MCU电压，mV
            /// byte6,7；偏移量0
            /// 范围0-65535，分辨率1
            /// </summary>
            public ushort mcu_voltage;

        }
        

        /// <summary>
        /// 电机控制器1 状态反馈第3帧
        /// ID:0x0C4403D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct ElecOilPump_Scm_MCU1_Frame3
        {
            public const uint canId = 0x0C44D3D5;
            /// <summary>
            /// 电机实际电功率，kW
            /// byte0,1；偏移量-300
            /// 范围-300-300，分辨率,0.01
            /// </summary>
            public short actual_power;

            /// <summary>
            /// 电机消耗电能累计，kWh
            /// byte2,3；偏移量-300
            /// 范围-300-300，分辨率,0.01，默认-300
            /// </summary>
            public short actual_totalenergy;

            /// <summary>
            /// 电机绕组温度，℃
            /// byte4，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short wind_temperature;

            /// <summary>
            /// 电逆变器温度，℃
            /// byte5，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short inverter_temperature;

            /// <summary>
            /// 控制芯片温度，℃
            /// byte5，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short mtclChip_temperature;

            //byte7保留

        }

        /// <summary>
        /// 电机控制器1 状态反馈第4帧
        /// ID:0x0C4503D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct ElecOilPump_Scm_MCU1_Frame4
        {
            public const uint canId = 0x0C45D3D5;
            /// <summary>
            /// byte0
            /// 年,0~99(2000+))
            /// </summary>
            public byte year;

            /// <summary>
            /// byte1
            /// 月,1~12
            /// </summary>
            public byte month;

            /// <summary>
            /// byte2
            /// 日,1~31
            /// </summary>
            public byte day;

            /// <summary>
            /// byte3
            /// 时,0~23
            /// </summary>
            public byte hour;

            /// <summary>
            /// byte4
            /// 份,0~59
            /// </summary>
            public byte minute;


            //byte7
        }

        #region 结构体与数组转换


        /// <summary>
        /// 整车控制器1 驱动信息
        /// ID:0x0C41D503
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        CanStandardData ElecOilPump_ScmEvcu1ToBytes(ElecOilPump_Scm_EVCU1 scmEvcu1)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = ElecOilPump_Scm_EVCU1.canId;
            scmCanByte.datas = new byte[8];

            //byte0
            scmCanByte.datas[0] = (byte)(scmEvcu1.running_enable + //电机使能指令
                        (scmEvcu1.control_mode << 2)); //电机控制模式

            //byte1,电机需求扭矩/电机扭矩限制,8bits，偏移量0,0范围0-32，默认值0，分辨率0.125
            scmCanByte.datas[1] = (byte)(scmEvcu1.request_torque * 8);

            //byte2，byte3,电机转速扭矩/电机转速限制,16bits，偏移量0,低位在前，范围0-5000，默认值0，分辨率1
            scmCanByte.datas[2] = (byte)((scmEvcu1.request_speed) & 0xff); //低位
            scmCanByte.datas[3] = (byte)((scmEvcu1.request_speed >> 8) & 0xff); //高位


            //uint8_t 7/6/5/4 保留

            return scmCanByte;
        }




        /// <summary>
        /// 电机控制器1 反馈第1帧 使能及控制信息等反馈
        /// ID:0x0C4203D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        ElecOilPump_Scm_MCU1_Frame1 ElecOilPump_ByteToScmMcu1Frame1(CanStandardData scmCanByte1)
        {
            ElecOilPump_Scm_MCU1_Frame1 scmMcu1Frame1 = new ElecOilPump_Scm_MCU1_Frame1();

            //byte0
            //bit0,1
            scmMcu1Frame1.actual_running_enable = (byte)(scmCanByte1.datas[0] & 0x03); //电机实际使能状况，bit0,bit1,偏移量0，00未使能，01使能

            //bit2,3
            scmMcu1Frame1.actual_control_mode = (byte)(scmCanByte1.datas[0] >> 2 & 0x03); //工作模式，00-无控制，01-扭矩，10-转速控制

            //byte1，转矩，范围0-32，默认值0，分辨率0.125
            scmMcu1Frame1.actual_torque = (byte)(scmCanByte1.datas[1] / 8);

            //byte2,3，速度，范围0-5000，默认值0，分辨率1
            scmMcu1Frame1.actual_speed = (ushort)(scmCanByte1.datas[3] * 256 + scmCanByte1.datas[2]);

            //byte6,故障等级
            scmMcu1Frame1.error_level = scmCanByte1.datas[6];

            //byte7,故障代码
            scmMcu1Frame1.error_code = scmCanByte1.datas[7];

            return scmMcu1Frame1;
        }




        /// <summary>
        /// 电机控制器1 状态反馈第2帧 电压电流
        /// ID:0x0C4303D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        ElecOilPump_Scm_MCU1_Frame2 ElecOilPump_ByteToScmMcu1Frame2(CanStandardData scmCanByte2)
        {
            ElecOilPump_Scm_MCU1_Frame2 scmMcu1Frame2 = new ElecOilPump_Scm_MCU1_Frame2();


            //byte0，母线电流，范围0-255，分辨率1
            scmMcu1Frame2.dc_current = scmCanByte2.datas[0];

            //byte1,2，母线电压，0-1000，分辨率1
            scmMcu1Frame2.dc_voltage = (ushort)(scmCanByte2.datas[2] * 256 + scmCanByte2.datas[1]);

            //byte3，线电流，范围0-255，分辨率1
            scmMcu1Frame2.ac_current = scmCanByte2.datas[3];

            //byte4,5，相电压，0-1000，分辨率1
            scmMcu1Frame2.ac_voltage = (ushort)(scmCanByte2.datas[5] * 256 + scmCanByte2.datas[4]);

            //byte6,7，相电压，0-65535，分辨率1,mV
            scmMcu1Frame2.mcu_voltage = (ushort)(scmCanByte2.datas[7] * 256 + scmCanByte2.datas[6]);


            return scmMcu1Frame2;
        }


        /// <summary>
        /// 电机控制器1 状态反馈第3帧 功率及温度
        /// ID:0x0C4403D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        /// <param name="scmCanByte2"></param>
        /// <returns></returns>
        ElecOilPump_Scm_MCU1_Frame3 ElecOilPump_ByteToScmMcu1Frame3(CanStandardData scmCanByte3)
        {
            ElecOilPump_Scm_MCU1_Frame3 scmMcu1Frame3 = new ElecOilPump_Scm_MCU1_Frame3();

            //byte0,1，实际电功率,KW，范围-300-300，分辨率,0.01
            scmMcu1Frame3.actual_power = (short)((scmCanByte3.datas[1] * 256 + scmCanByte3.datas[0]) * 0.01 - 300);

            //byte2,3，实际电能消耗KWh，范围-300-300，分辨率,0.01
            scmMcu1Frame3.actual_totalenergy = (short)((scmCanByte3.datas[3] * 256 + scmCanByte3.datas[2]) * 0.01 - 300);

            //byte4，绕组温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame3.wind_temperature = (short)(scmCanByte3.datas[4] - 40);

            //byte5，逆变器温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame3.inverter_temperature = (short)(scmCanByte3.datas[5] - 40);

            //byte6，控制芯片温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame3.mtclChip_temperature = (short)(scmCanByte3.datas[6] - 40);

            return scmMcu1Frame3;
        }

        /// <summary>
        /// 电机控制器1 状态反馈第4帧 版本
        /// ID:0x0C4503D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        /// <param name="scmCanByte4"></param>
        /// <returns></returns>
        ElecOilPump_Scm_MCU1_Frame4 ElecOilPump_ByteToScmMcu1Frame4(CanStandardData scmCanByte4)
        {
            ElecOilPump_Scm_MCU1_Frame4 scmMcu1Frame4 = new ElecOilPump_Scm_MCU1_Frame4();

            scmMcu1Frame4.year = scmCanByte4.datas[0];
            scmMcu1Frame4.month = scmCanByte4.datas[1];
            scmMcu1Frame4.day = scmCanByte4.datas[2];

            scmMcu1Frame4.hour = scmCanByte4.datas[3];
            scmMcu1Frame4.minute = scmCanByte4.datas[4];

            return scmMcu1Frame4;
        }
        #endregion


        public override ScmCanSendMsg EcuClearMcuFault()
        {
            ScmCanSendMsg scmSendMsg = new ScmCanSendMsg();

            ElecOilPump_Scm_EVCU1 scmEvcu1 = new ElecOilPump_Scm_EVCU1();
            scmEvcu1.control_mode = 0x01; //清故障
            scmEvcu1.request_speed = 0;
            scmEvcu1.request_torque = 0x00;
            scmEvcu1.running_enable = 0;

            CanStandardData canData = ElecOilPump_ScmEvcu1ToBytes(scmEvcu1);

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

            ElecOilPump_Scm_EVCU1 scmTcu1 = new ElecOilPump_Scm_EVCU1();

            ScmCanSendMsg canSendData = new ScmCanSendMsg();
            canSendData.FeedBkMtclMode = cmdType;

            string strCmdDetail = string.Empty; //命令解析字符串信息

            

            //电机转速模式设定
            scmTcu1.running_enable = 0x00; //mcu使能
            scmTcu1.control_mode = 0x00; //不清故障

            switch (cmdType)
            {
                case Ecm_WorkMode.None: //备用
                    scmTcu1.control_mode = 0x00; //控制模式
                    strCmdDetail += "备用" + "\r\n";
                    break;

                case Ecm_WorkMode.TorqueMode: //转矩模式
                    {
                        canSendData.FeedBkMotorTorque = scmTcu1.request_torque = (byte)data;
                        scmTcu1.running_enable = 0x02; //mcu使能
                        scmTcu1.control_mode = 0x01; //控制模式
                        strCmdDetail += "扭矩命令:" + data.ToString() + "\r\n";
                        break;
                    }

                case Ecm_WorkMode.SpeedMode: //转速模式
                    {
                        canSendData.FeedBkMotorSpeed = (short)(scmTcu1.request_speed = Convert.ToUInt16(data)); 
                        scmTcu1.running_enable = 0x02; //mcu使能
                        scmTcu1.control_mode = 0x02; //控制模式
                        strCmdDetail += "转速命令:" + data.ToString() + "\r\n";
                        break;
                    }



                default: scmTcu1.control_mode = 0x00; break;
            }
            CanStandardData canData = ElecOilPump_ScmEvcu1ToBytes(scmTcu1);

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
                case ElecOilPump_Scm_MCU1_Frame1.canId:  //反馈第一帧,使能，方向，扭矩，转速
                    {
                        ElecOilPump_Scm_MCU1_Frame1 scmMcu1 = ElecOilPump_ByteToScmMcu1Frame1(canRecDatas);

                        //工作模式
                        recMsg.containFeedBkMtclMode = true;
                        switch (scmMcu1.actual_control_mode)
                        {
                            case 0: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break; //初始化
                            case 1: recMsg.FeedBkMtclMode = Ecm_WorkMode.TorqueMode; break; //转矩环
                            case 2: recMsg.FeedBkMtclMode = Ecm_WorkMode.SpeedMode; break; //转速环
                            default: recMsg.FeedBkMtclMode = Ecm_WorkMode.None;  break;
                        }
                        
                        //转速
                        recMsg.containFeedBkMotorSpeed = true;
                        recMsg.FeedBkMotorSpeed = (short)scmMcu1.actual_speed;

                        //转矩
                        recMsg.containFeedBkMotorTorque = true;
                        recMsg.FeedBkMotorTorque = (short)scmMcu1.actual_torque;

                        //故障等级
                        recMsg.containFeedBkErrorLevel = true;
                        recMsg.FeedBkErrorLevel = scmMcu1.error_level;
                         
                        //故障码
                        recMsg.containFeedBkErrorCode = true;
                        recMsg.FeedBkErrorCode = new byte[1] { scmMcu1.error_code };

                        //故障名称
                        recMsg.containFeedBkErrorStr = true;
                        recMsg.FeedBkErrorStr = ReceiveErrorTransform(scmMcu1.error_code);

                        break;
                    }
                case ElecOilPump_Scm_MCU1_Frame2.canId:  //电流电压
                    {
                        ElecOilPump_Scm_MCU1_Frame2 scmMcu2 = ElecOilPump_ByteToScmMcu1Frame2(canRecDatas);
                        //实际电流
                        recMsg.containFeedBkDcCurrent = true;
                        recMsg.FeedBkDcCurrent = scmMcu2.dc_current;
                        //相电压
                        recMsg.containFeedBkDcVoltage = true;
                        recMsg.FeedBkDcVoltage = scmMcu2.dc_voltage;
                        //实际电流
                        recMsg.containFeedBkAcCurrent = true;
                        recMsg.FeedBkAcCurrent = scmMcu2.ac_current;
                        //相电压
                        recMsg.containFeedBkAcVoltage = true;
                        recMsg.FeedBkAcVoltage = scmMcu2.ac_voltage;
                        //MCU电压
                        recMsg.containFeedBkMcuVoltage = true;
                        recMsg.FeedBkMcuVoltage = scmMcu2.mcu_voltage;

                        break;
                    }
                case ElecOilPump_Scm_MCU1_Frame3.canId:  //功率、温度
                    {
                        ElecOilPump_Scm_MCU1_Frame3 scmMcu3 = ElecOilPump_ByteToScmMcu1Frame3(canRecDatas);
                        recMsg.containFeedBkActPower = true;
                        recMsg.FeedBkActPower = scmMcu3.actual_power;

                        recMsg.containFeedBkMtclInvTemp = true;
                        recMsg.FeedBkMtclInvTemp = scmMcu3.inverter_temperature;

                        recMsg.containFeedBkMotorWindingTemp = true;
                        recMsg.FeedBkMotorWindingTemp = scmMcu3.wind_temperature;

                        recMsg.containFeedBkMtclChipTemp = true;
                        recMsg.FeedBkMtclChipTemp = scmMcu3.mtclChip_temperature;
                        break;
                    }
                case ElecOilPump_Scm_MCU1_Frame4.canId:  //软件版本
                    {
                        ElecOilPump_Scm_MCU1_Frame4 scmMcu4 = ElecOilPump_ByteToScmMcu1Frame4(canRecDatas);
                        recMsg.containFeekBkMtclVersion = true;
                        recMsg.FeekBkMtclVersion = scmMcu4.year.ToString("X2") + "." + scmMcu4.month.ToString("X2") + "." + scmMcu4.day.ToString("X2") + "." 
                            + scmMcu4.hour.ToString("X2") + "." + scmMcu4.minute.ToString("X2");
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
