using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotorControlProtocols.Hybrid
{
    /// <summary>
    /// 混合动力
    /// 适用于 混动电机控制器1
    /// </summary>
    public class HybridTM1:AbstractMotorControl
    {
        /********************************
         * Intel格式，低位在前，高位在后
         * ******************************/

        #region VCU发送命令

        /// <summary>
        /// 整车控制器1 驱动信息
        /// ID:0x0C51D2D0
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct Scm_EVCU1
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x0C51D2D0;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机使能指令
            /// byte0
            /// bit0，1；2bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 00无效，01有效，10清故障
            /// </summary>
            public byte isg_ctrl_ena_dmd_sd;

            /// <summary>
            /// 电机运行方向
            /// byte0
            /// bit2,3；2bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 00停止，01正转，10反转，从轴往后盖看，顺时针为正转
            /// </summary>
            public byte isg_run_dir_dmd_sd;

            /// <summary>
            /// 电机控制模式
            /// byte0
            /// bit4,5,6,7；4bits，偏移量0
            /// 范围0-3，默认值1，分辨率1
            /// 00自由转，01转速控制，02扭矩控制，11主动放电
            /// </summary>
            public byte isg_ctrl_mod_dmd_sd;

            /// <summary>
            /// 电机需求扭矩/电机扭矩限制
            /// byte1,byte2
            /// 16bits，偏移量-2000
            /// 范围-2000-2000，默认值-2000，分辨率1
            /// </summary>
            public short isg_tq_dmd_sd;

            /// <summary>
            /// 电机需求转速/电机转速限制
            /// byte3,byte4
            /// 16bits，偏移量-12000
            /// 范围-12000-12000，默认值-12000，分辨率1
            /// </summary>
            public short isg_spd_dmd_sd;

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
        /// ID:0x0C52D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU1_Frame1
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x0C52D0D2;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机实际使能状态
            /// byte0
            /// bit0,1；2bits，偏移量0
            /// 范围0-1，默认值0，分辨率1
            /// 00未使能，01使能
            /// </summary>
            public byte isg_ena_act;

            /// <summary>
            /// 电机实际运行方向
            /// byte0
            /// bit2,3；2bits，偏移量0
            /// 范围0-2，默认值0，分辨率1
            /// 00停止，01正转，10反转，从轴往后盖看，顺时针为正转
            /// </summary>
            public byte isg_run_dir_act;

            /// <summary>
            /// 电机实际工作模式
            /// byte0
            /// bit4,5,6,7；4bits，偏移量0
            /// 范围0-8，默认值0，分辨率1
            /// 0：初始化，1：低压上电正常，2：（保留），3：电机允许运行，4：电机转速闭环运行
            /// 5：电机转矩闭环运行，6：下强电（主动放电），7：下弱电（高压低于36V），8：错误（故障保护）
            /// </summary>
            public byte isg_ctrl_mod_act;

            /// <summary>
            /// 电机实际扭矩
            /// byte2,3，偏移量-2000
            /// 范围-2000-2000，默认值-2000，分辨率1
            /// </summary>
            public short isg_tq_acu;

            /// <summary>
            /// 电机实际转速
            /// byte4,5，偏移量-12000
            /// 范围-12000-12000，默认值-12000，分辨率1
            /// </summary>
            public short isg_spd_acu;

            /// <summary>
            /// 电机在当前状态下能提供的最大扭矩
            /// byte6,7,偏移量-2000
            /// 范围-2000-2000，默认值-2000，分辨率1
            /// </summary>
            public short isg_tq_acu_max;
        }


        /// <summary>
        /// 电机控制器1 状态反馈第2帧
        /// ID:0x0C53D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct Scm_MCU1_Frame2
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x0C53D0D2;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机故障等级
            /// byte0，偏移量0
            /// 范围0-4，分辨率1，默认0
            /// 000：正常,001：驱动系统一级故障（报警）,010：驱动系统二级故障（降功率）
            /// 011：驱动系统三级故障（强制停车）,100：驱动系统四级故障（断开接触器）
            /// </summary>
            public byte isg_err_lvl;

            /// <summary>
            /// 电机故障码
            /// byte1
            /// </summary>
            public byte isg_err_cod;

            /// <summary>
            /// 电机故障位，一级故障位
            /// byte2
            /// </summary>
            public byte isg_err_bit1;

            /// <summary>
            /// 电机故障位，二级故障位
            /// byte3
            /// </summary>
            public byte isg_err_bit2;

            /// <summary>
            /// 电机故障位，三级故障位
            /// byte4,5,6
            /// </summary>
            public int isg_err_bit3;

            /// <summary>
            /// 电机故障位，四级故障位
            /// byte7
            /// </summary>
            public byte isg_err_bit4;
        }


        /// <summary>
        /// 电机控制器1 状态反馈第3帧
        /// ID:0x1854D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU1_Frame3
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x1854D0D2;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机直流母线电压，V
            /// byte0,1；偏移量0
            /// 范围0-1000，分辨率1
            /// </summary>
            public ushort isg_dc_u;

            /// <summary>
            /// 电机直流母线电流，A
            /// byte2,3；偏移量-500
            /// 范围-500-500，分辨率1
            /// </summary>
            public short isg_dc_i;

            /// <summary>
            /// 电机三相相电压，V
            /// byte4,5；偏移量0
            /// 范围0-1000，分辨率1
            /// </summary>
            public ushort isg_ac_u;

            /// <summary>
            /// 电机三相线电流，A
            /// byte6,7；偏移量-500
            /// 范围-500-500，分辨率1
            /// </summary>
            public short isg_ac_i;
        }

        /// <summary>
        /// 电机控制器1 状态反馈第4帧
        /// ID:0x1855D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU1_Frame4
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x1855D0D2;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// 电机实际电功率，kW
            /// byte0,1；偏移量-300
            /// 范围-300-300，分辨率,0.01
            /// </summary>
            public short isg_pwr_acu;

            /// <summary>
            /// 电机消耗电能累计，kWh
            /// byte2,3；偏移量-300
            /// 范围-300-300，分辨率,0.01，默认-300
            /// </summary>
            public short isg_totegy_act;

            /// <summary>
            /// 电机绕组温度，℃
            /// byte4，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short isg_wind_t;

            /// <summary>
            /// 电逆变器温度，℃
            /// byte5，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short isg_inv_t;

            /// <summary>
            /// 电机轴承温度，℃
            /// byte6，偏移量-40
            /// 范围-40-210，分辨率1，默认-40
            /// </summary>
            public short isg_bear_t;

            /// <summary>
            /// 电机生命周期
            /// byte7，偏移量0
            /// 范围0-255，分辨率1，默认0
            /// </summary>
            public byte isg_life_cyc;

        }

        /// <summary>
        /// 电机控制器1 状态反馈第5帧
        /// ID:0x1856D0D2
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU1_Frame5
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x1856D0D2;

            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// byte0
            /// 系统类型，偏移量0，分辨率1，范围0-99(HEX)
            /// </summary>
            public byte sys_type_isg;

            /// <summary>
            /// byte1
            /// 控制器类型，偏移量0，分辨率1，范围0-99(HEX)
            /// </summary>
            public byte mcu_type_isg;

            /// <summary>
            /// byte2
            /// 软件版本，偏移量0，分辨率1，范围0-99(HEX)
            /// 软件编码显示方式为16进制，例:软件版本号为02.60.01.01.0001的显示方式为: 02.60.01.01.00.01.00.00（预留位）
            /// </summary>
            public byte soft_rev_isg;

            /// <summary>
            /// byte3
            /// 通用协议版本，偏移量0，分辨率1，范围0-99(HEX)
            /// </summary>
            public byte cmc_pro_ver_isg;

            /// <summary>
            /// byte4,5
            /// 标定量数据，偏移量0，分辨率1，范围0-9999(HEX)
            /// </summary>
            public byte[] rating_data_isg;

            //byte6,7
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
        CanStandardData ScmEvcu1ToBytes(Scm_EVCU1 scmEvcu1)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = Scm_EVCU1.canId;
            scmCanByte.baudrate = Scm_EVCU1.baudrate; //250K
            scmCanByte.datas = new byte[8]; //数据长度为8

            //byte0
            scmCanByte.datas[0] = (byte)(Convert.ToByte(scmEvcu1.isg_ctrl_ena_dmd_sd) + //电机使能指令，分辨率1，偏移量0，范围0~2,byte0,bit0,1,2bits,00无效，01有效，10清故障
                        (Convert.ToByte(scmEvcu1.isg_run_dir_dmd_sd) << 2) + //电机运行方向，分辨率1，偏移量0，范围0~2,byte0,bit2,3,2bits 00停止，01正转，10反转，从轴往后盖看，顺时针为正转
                        (Convert.ToByte(scmEvcu1.isg_ctrl_mod_dmd_sd) << 4)); //电机控制模式，分辨率1，偏移量0，默认值1，4bits,00自由转，01转速控制，02扭矩控制，11主动放电

            //byte1，byte2,电机需求扭矩/电机扭矩限制,6bits，偏移量-2000,低位在前，范围-2000-2000，默认值-2000，分辨率1
            scmCanByte.datas[1] = Convert.ToByte((scmEvcu1.isg_tq_dmd_sd + 2000) & 0xff); //低位
            scmCanByte.datas[2] = Convert.ToByte(((scmEvcu1.isg_tq_dmd_sd + 2000) >> 8) & 0xff); //高位

            //byte3，byte4,电机转速扭矩/电机转速限制,6bits，偏移量-12000,低位在前，范围-12000-12000，默认值-12000，分辨率1
            scmCanByte.datas[3] = Convert.ToByte((scmEvcu1.isg_spd_dmd_sd + 12000) & 0xff); //低位
            scmCanByte.datas[4] = Convert.ToByte(((scmEvcu1.isg_spd_dmd_sd + 12000) >> 8) & 0xff); //高位


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
            scmMcu1Frame1.isg_ena_act = (byte)(scmCanByte1.datas[0] & 0x03); //电机实际使能状况，bit0,bit1,偏移量0，00未使能，01使能

            //bit2,3
            scmMcu1Frame1.isg_run_dir_act = (byte)(scmCanByte1.datas[0] >> 2 & 0x03); //电机实际运行方向，00停止，01正转，10反转

            //bit4,5,6,7
            scmMcu1Frame1.isg_ctrl_mod_act = (byte)(scmCanByte1.datas[0] >> 4 & 0x0f); //工作模式，0初始化，1低压上电正常，2保留，3电机允许运行，4转速环，5转矩环，6放强电，7放弱电，8错误

            //byte1保留

            //byte2,3，转矩，偏移量-2000,范围-2000-2000，默认值-2000，分辨率1
            scmMcu1Frame1.isg_tq_acu = (short)(scmCanByte1.datas[3] * 256 + scmCanByte1.datas[2] - 2000);

            //byte4,5，速度，偏移量-12000,范围-12000-12000，默认值-12000，分辨率1
            scmMcu1Frame1.isg_spd_acu = (short)(scmCanByte1.datas[5] * 256 + scmCanByte1.datas[4] - 12000);

            //byte6,7，最大转矩，偏移量-2000,范围-2000-2000，默认值-2000，分辨率1
            scmMcu1Frame1.isg_tq_acu_max = (short)(scmCanByte1.datas[7] * 256 + scmCanByte1.datas[6] - 2000);

            return scmMcu1Frame1;
        }

        /// <summary>
        /// 电机控制器1 反馈第2帧 电机故障等级及故障码
        /// ID:0x0C63D0D1
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        Scm_MCU1_Frame2 ByteToScmMcu1Frame2(CanStandardData scmCanByte2)
        {
            Scm_MCU1_Frame2 scmMcu1Frame2 = new Scm_MCU1_Frame2();

            //byte0，故障等级，0正常，1一级故障(报警)，2二级故障(降功率),3三级故障(强制停车),4四级故障(断开接触器)
            scmMcu1Frame2.isg_err_lvl = (byte)(scmCanByte2.datas[0] & 0x0f);

            //byte1，故障码
            scmMcu1Frame2.isg_err_cod = (byte)(scmCanByte2.datas[1]);

            //byte2，一级故障位
            scmMcu1Frame2.isg_err_bit1 = scmCanByte2.datas[2];

            //byte3，二级故障位
            scmMcu1Frame2.isg_err_bit2 = scmCanByte2.datas[3];

            //byte4,5,6，三级故障位
            scmMcu1Frame2.isg_err_bit3 = scmCanByte2.datas[4] + scmCanByte2.datas[5] * 256 + scmCanByte2.datas[6] * 256 * 256;

            //byte7，四级故障位
            scmMcu1Frame2.isg_err_bit4 = scmCanByte2.datas[7];

            return scmMcu1Frame2;
        }

        /// <summary>
        /// 电机控制器1 状态反馈第3帧 电压电流
        /// ID:0x1864D0D1
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        Scm_MCU1_Frame3 ByteToScmMcu1Frame3(CanStandardData scmCanByte3)
        {
            Scm_MCU1_Frame3 scmMcu1Frame3 = new Scm_MCU1_Frame3();

            //byte0,1，母线电压，0-1000，分辨率1
            scmMcu1Frame3.isg_dc_u = (ushort)(scmCanByte3.datas[1] * 256 + scmCanByte3.datas[0]);

            //byte2,3，母线电流，-500-500，偏移量-500，分辨率1
            scmMcu1Frame3.isg_dc_i = (short)(scmCanByte3.datas[3] * 256 + scmCanByte3.datas[2] - 500);

            //byte4,5，相电压，0-1000，分辨率1
            scmMcu1Frame3.isg_ac_u = (ushort)(scmCanByte3.datas[5] * 256 + scmCanByte3.datas[4]);

            //byte6,7，线电流，-500-500，偏移量-500，分辨率1
            scmMcu1Frame3.isg_ac_i = (short)(scmCanByte3.datas[7] * 256 + scmCanByte3.datas[6] - 500);

            return scmMcu1Frame3;
        }

        /// <summary>
        /// 电机控制器1 状态反馈第4帧 功率及温度
        /// ID:0x1865D0D1
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        /// <param name="scmCanByte4"></param>
        /// <returns></returns>
        Scm_MCU1_Frame4 ByteToScmMcu1Frame4(CanStandardData scmCanByte4)
        {
            Scm_MCU1_Frame4 scmMcu1Frame4 = new Scm_MCU1_Frame4();

            //byte0,1，实际电功率,KW，范围-300-300，分辨率,0.01
            scmMcu1Frame4.isg_pwr_acu = (short)((scmCanByte4.datas[1] * 256 + scmCanByte4.datas[0]) * 0.01 - 300);

            //byte2,3，实际电能消耗KWh，范围-300-300，分辨率,0.01
            scmMcu1Frame4.isg_totegy_act = (short)((scmCanByte4.datas[3] * 256 + scmCanByte4.datas[2]) * 0.01 - 300);

            //byte4，绕组温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame4.isg_wind_t = (short)(scmCanByte4.datas[4] - 40);

            //byte5，逆变器温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame4.isg_inv_t = (short)(scmCanByte4.datas[5] - 40);

            //byte6，逆变器温度，范围-40-210，分辨率1，默认-40
            scmMcu1Frame4.isg_bear_t = (short)(scmCanByte4.datas[6] - 40);

            //byte7，电机生命周期
            scmMcu1Frame4.isg_life_cyc = scmCanByte4.datas[7];

            return scmMcu1Frame4;
        }

        /// <summary>
        /// 电机控制器1 状态反馈第5帧 版本
        /// ID:0x1856D0D1
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        Scm_MCU1_Frame5 ByteToScmMcu1Frame5(CanStandardData scmCanByte5)
        {
            Scm_MCU1_Frame5 scmMcu5 = new Scm_MCU1_Frame5();
            scmMcu5.sys_type_isg = scmCanByte5.datas[0];
            scmMcu5.mcu_type_isg = scmCanByte5.datas[1];
            scmMcu5.soft_rev_isg = scmCanByte5.datas[2];
            scmMcu5.cmc_pro_ver_isg = scmCanByte5.datas[3];
            scmMcu5.rating_data_isg = new byte[] { scmCanByte5.datas[4], scmCanByte5.datas[5] };
            return scmMcu5;
        }
        #endregion

        #endregion

        

        public override AbstractMotorControl.ScmCanSendMsg EcuClearMcuFault()
        {
            ScmCanSendMsg scmSendMsg = new ScmCanSendMsg();

            Scm_EVCU1 scmEvcu1 = new Scm_EVCU1();
            scmEvcu1.isg_ctrl_ena_dmd_sd = 0x02; //清故障
            scmEvcu1.isg_ctrl_mod_dmd_sd = 0x00;
            scmEvcu1.isg_run_dir_dmd_sd = 0x00;
            scmEvcu1.isg_spd_dmd_sd = 0;
            scmEvcu1.isg_tq_dmd_sd = 0;

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

            Scm_EVCU1 scmEvcu1 = new Scm_EVCU1();

            ScmCanSendMsg canSendData = new ScmCanSendMsg();
            canSendData.FeedBkMtclMode = cmdType;

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
                    scmEvcu1.isg_run_dir_dmd_sd = 0x00;
                    strCmdDetail = "空挡" + "\r\n";
                    break;

                case Ecm_Gear.DriveGear:
                    scmEvcu1.isg_run_dir_dmd_sd = 0x01;
                    strCmdDetail = "D档" + "\r\n";
                    break;

                case Ecm_Gear.ReverseGear:
                    scmEvcu1.isg_run_dir_dmd_sd = 0x02;
                    strCmdDetail = "倒档" + "\r\n";
                    break;

                case Ecm_Gear.ErrorGear:
                    scmEvcu1.isg_run_dir_dmd_sd = 0x03;
                    strCmdDetail = "错误" + "\r\n";
                    break;

                default: scmEvcu1.isg_run_dir_dmd_sd = 0x00; break;
            }

            //电机转速模式设定
            scmEvcu1.isg_ctrl_ena_dmd_sd = 0x00; //mcu使能



            switch (cmdType)
            {
                case Ecm_WorkMode.None: //备用
                    scmEvcu1.isg_ctrl_mod_dmd_sd = 0x00; //控制模式
                    strCmdDetail += "备用" + "\r\n";
                    break;

                case Ecm_WorkMode.SpeedMode: //转速模式
                    {
                        scmEvcu1.isg_spd_dmd_sd = data;  //偏移12000，0表示-12000
                        canSendData.FeedBkMotorSpeed = data;
                        scmEvcu1.isg_ctrl_ena_dmd_sd = 0x01; //mcu使能
                        scmEvcu1.isg_ctrl_mod_dmd_sd = 0x01; //控制模式
                        strCmdDetail += "转速命令:" + data.ToString() + "\r\n";
                        break;
                    }

                case Ecm_WorkMode.TorqueMode: //转矩模式
                    {
                        scmEvcu1.isg_tq_dmd_sd = data;//偏移2000，0表示-2000
                        canSendData.FeedBkMotorTorque = data;
                        scmEvcu1.isg_ctrl_ena_dmd_sd = 0x01; //mcu使能
                        scmEvcu1.isg_ctrl_mod_dmd_sd = 0x02; //控制模式
                        strCmdDetail += "扭矩命令:" + data.ToString() + "\r\n";
                        break;
                    }

                case Ecm_WorkMode.DisCharging: //放电模式
                    scmEvcu1.isg_ctrl_ena_dmd_sd = 0x01; //mcu使能
                    scmEvcu1.isg_ctrl_mod_dmd_sd = 0x03; //控制模式
                    strCmdDetail += "放电" + "\r\n";
                    break;

                default: scmEvcu1.isg_ctrl_mod_dmd_sd = 0x00; break;
            }
            CanStandardData canData = ScmEvcu1ToBytes(scmEvcu1);

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
        private string ReceiveErrorTransform(byte errorLevel, byte errorCode, byte errorBit1, byte errorBit2, int errorBit3, byte errorBit4)
        {
            if (errorLevel == 0)
                return string.Empty;

            string errStr = string.Empty;

            switch (errorLevel)
            {

                case 1: //一级故障
                    #region 一级故障
                    {
                        switch (errorCode)
                        {
                            case 0:
                                {
                                    break;
                                }
                            default: break;
                        }
                        break;
                    }
                #endregion

                case 2: //二级故障
                    #region 二级故障
                    {
                        switch (errorCode)
                        {

                            case 1:
                                {
                                    errStr = "电机过温报警";
                                    break;
                                }

                            case 2:
                                {
                                    errStr = "逆变器过温报警";
                                    break;
                                }

                            case 3:
                                {
                                    switch (errorBit2)
                                    {
                                        case 4:
                                            errStr = "正向过速报警";
                                            break;
                                        case 8:
                                            errStr = "反向过速报警";
                                            break;
                                        default: break;
                                    }

                                    break;
                                }

                            case 4:
                                {
                                    errStr = "直流过压报警";
                                    break;
                                }

                            case 5:
                                {
                                    errStr = "直流欠压报警";
                                    break;
                                }
                            default: break;
                        }
                        break;
                    }
                #endregion

                case 3: //三级故障
                    #region 三级故障
                    {
                        switch (errorCode)
                        {
                            case 1:
                                {
                                    errStr = "CAN通讯故障";
                                    break;
                                }
                            case 2:
                                {
                                    errStr = "直流过压故障";
                                    break;
                                }
                            case 3:
                                {
                                    errStr = "直流欠压故障";
                                    break;
                                }
                            case 4:
                                {
                                    switch (errorBit3)
                                    {
                                        case 8:
                                            errStr = "正向过速故障";
                                            break;
                                        case 16:
                                            errStr = "反向过速故障";
                                            break;
                                        default: break;
                                    }

                                    break;
                                }
                            case 5:
                                {
                                    errStr = "逆变器过温故障";
                                    break;
                                }
                            case 6:
                                {
                                    errStr = "电机过温故障";
                                    break;
                                }
                            case 7:
                                {
                                    errStr = "过流故障";
                                    break;
                                }
                            case 8:
                                {
                                    errStr = "过载故障";
                                    break;
                                }
                            case 9:
                                {
                                    errStr = "IPM故障";
                                    break;
                                }
                            case 0x0A:
                                {
                                    switch (errorBit3)
                                    {
                                        case 0x400:
                                            errStr = "A相传感器故障";
                                            break;
                                        case 0x800:
                                            errStr = "B相传感器故障";
                                            break;
                                        case 0x1000:
                                            errStr = "C相传感器故障";
                                            break;
                                        case 0x2000:
                                            errStr = "缺相故障";
                                            break;
                                        case 0x4000:
                                            errStr = "电机温度传感器故障";
                                            break;
                                        case 0x8000:
                                            errStr = "硬件过压故障";
                                            break;
                                        case 0x10000:
                                            errStr = "IGBT硬件故障";
                                            break;
                                        case 0x20000:
                                            errStr = "力矩反馈故障";
                                            break;
                                        case 0x40000:
                                            errStr = "速度检测故障";
                                            break;
                                        case 0x80000:
                                            errStr = "eep故障";
                                            break;
                                        case 0x100000:
                                            errStr = "电机母线电压检测故障";
                                            break;
                                        case 0x200000:
                                            errStr = "弱电故障";
                                            break;
                                        default: break;
                                    }

                                    break;
                                }
                            default: break;
                        }
                        break;
                    }
                #endregion

                case 4: //四级故障
                    #region 四级故障
                    {
                        switch (errorCode)
                        {
                            case 0:
                                {

                                    break;
                                }
                            default: break;
                        }
                        break;
                    }
                #endregion

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
                return null;
            }

            //ID未被筛选
            recMsg.canId = canRecDatas.canId;
            recMsg.baudrate = canRecDatas.baudrate;
            recMsg.datas = canRecDatas.datas;


            switch (canRecDatas.canId)
            {
                case Scm_MCU1_Frame1.canId: // 0x0C62D0D1: //反馈第一帧,使能，方向，扭矩，转速
                    {
                        Scm_MCU1_Frame1 scmMcu1 = ByteToScmMcu1Frame1(canRecDatas);
                        //电机使能
                        recMsg.containFeedBkMcuEnable = true;
                        recMsg.FeedBkMcuEnable = scmMcu1.isg_ena_act == 1 ? false : true;

                        //工作模式
                        recMsg.containFeedBkMtclMode = true;
                        switch (scmMcu1.isg_ctrl_mod_act)
                        {
                            case 0: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break; //初始化
                            case 1: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break; //低压上电正常
                            case 2: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break; //保留
                            case 3: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break; //电机允许运行
                            case 4: recMsg.FeedBkMtclMode = Ecm_WorkMode.SpeedMode; break; //转速环
                            case 5: recMsg.FeedBkMtclMode = Ecm_WorkMode.TorqueMode; break; //转矩环
                            case 6: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break; //下强电
                            case 7: recMsg.FeedBkMtclMode = Ecm_WorkMode.None; break; //下弱电
                            case 8: recMsg.FeedBkMtclMode = Ecm_WorkMode.ErrorProtected; break; //故障
                            default: break;
                        }

                        recMsg.containFeedBkGear = true;
                        switch (scmMcu1.isg_run_dir_act)
                        {
                            case 0: recMsg.FeedBkGear = Ecm_Gear.NeutralGear; break;//空档
                            case 1: recMsg.FeedBkGear = Ecm_Gear.DriveGear; break;//前进
                            case 2: recMsg.FeedBkGear = Ecm_Gear.ReverseGear; break;//后退
                            default: recMsg.FeedBkGear = Ecm_Gear.ErrorGear; break;
                        }

                        //扭矩
                        recMsg.containFeedBkMotorTorque = true;
                        recMsg.FeedBkMotorTorque = scmMcu1.isg_tq_acu;

                        //转速
                        recMsg.containFeedBkMotorSpeed = true;
                        recMsg.FeedBkMotorSpeed = scmMcu1.isg_spd_acu;

                        //最大转矩
                        recMsg.containFeedBkMotorMaxTorque = true;
                        recMsg.FeedbkMotorMaxTorque = scmMcu1.isg_tq_acu_max;
                        break;
                    }

                case Scm_MCU1_Frame2.canId: // 0x0C63D0D1: //反馈第二帧,故障等级及故障码
                    {
                        Scm_MCU1_Frame2 scmMcu2 = ByteToScmMcu1Frame2(canRecDatas);

                        //故障等级
                        recMsg.containFeedBkErrorLevel = true;
                        recMsg.FeedBkErrorLevel = scmMcu2.isg_err_lvl;

                        //故障码
                        recMsg.containFeedBkErrorCode = true;
                        recMsg.FeedBkErrorCode = new byte[1] { scmMcu2.isg_err_cod };


                        recMsg.containFeedBkErrorBits = true;
                        recMsg.FeedBkErrorBits = new byte[6] { scmMcu2.isg_err_bit1, scmMcu2.isg_err_bit2, (byte)(scmMcu2.isg_err_bit3 & 0xff), (byte)((scmMcu2.isg_err_bit3 >> 8) & 0xff), (byte)((scmMcu2.isg_err_bit3 >> 16) & 0xff), scmMcu2.isg_err_bit4 };

                        //故障名称
                        recMsg.containFeedBkErrorStr = true;
                        recMsg.FeedBkErrorStr = ReceiveErrorTransform(recMsg.FeedBkErrorLevel, scmMcu2.isg_err_cod,
                            scmMcu2.isg_err_bit1, scmMcu2.isg_err_bit2, scmMcu2.isg_err_bit3, scmMcu2.isg_err_bit4);
                        break;
                    }

                case Scm_MCU1_Frame3.canId: // 0x1864D0D1: //反馈第三帧，电流电压
                    {
                        Scm_MCU1_Frame3 scmMcu3 = ByteToScmMcu1Frame3(canRecDatas);

                        //直流电压
                        recMsg.containFeedBkDcVoltage = true;
                        recMsg.FeedBkDcVoltage = scmMcu3.isg_dc_u;

                        //直流电流
                        recMsg.containFeedBkDcCurrent = true;
                        recMsg.FeedBkDcCurrent = scmMcu3.isg_dc_i;


                        //交流电压
                        recMsg.containFeedBkAcVoltage = true;
                        recMsg.FeedBkAcVoltage = scmMcu3.isg_ac_u;

                        //交流电流
                        recMsg.containFeedBkAcCurrent = true;
                        recMsg.FeedBkAcCurrent = scmMcu3.isg_ac_i;

                        break;
                    }

                case Scm_MCU1_Frame4.canId: // 0x1865D0D1: //反馈第四帧，功率、温度
                    {
                        Scm_MCU1_Frame4 scmMcu4 = ByteToScmMcu1Frame4(canRecDatas);

                        //功率
                        recMsg.containFeedBkActPower = true;
                        recMsg.FeedBkActPower = scmMcu4.isg_pwr_acu;

                        recMsg.containFeedBkMotorWindingTemp = true;
                        recMsg.FeedBkMotorWindingTemp = scmMcu4.isg_wind_t;

                        recMsg.containFeedBkMtclInvTemp = true;
                        recMsg.FeedBkMtclInvTemp = scmMcu4.isg_inv_t;

                        //isg_bear_t;

                        //isg_life_cyc;
                    }
                    break;

                case Scm_MCU1_Frame5.canId: //版本
                    {
                        Scm_MCU1_Frame5 scmMcu5 = ByteToScmMcu1Frame5(canRecDatas);
                        recMsg.containFeekBkMtclVersion = true;
                        recMsg.FeekBkMtclVersion = scmMcu5.sys_type_isg.ToString("X2") + "." + scmMcu5.mcu_type_isg.ToString("X2") + "." +
                            scmMcu5.soft_rev_isg.ToString("X2") + "." + scmMcu5.cmc_pro_ver_isg.ToString("X2") + "." +
                            scmMcu5.rating_data_isg[0].ToString("X2") + scmMcu5.rating_data_isg[1].ToString("X2");
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
