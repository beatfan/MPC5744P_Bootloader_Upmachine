using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MotorControlProtocols.PureElectric
{
    /// <summary>
    /// 纯电动控制器
    /// 适用于 大郡电控1,900NM
    /// 已完成，待检查
    /// </summary>
    public class PureElectricTM1:AbstractMotorControl
    {
        #region VCU发送命令

        /// <summary>
        /// 整车控制器1 驱动信息
        /// ID:0x1000EFD0
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_EVCU1
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x1000EFD0;

            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// byte1
            /// 加速踏板行程信号，分辨率0.4%，偏移量0，范围0~100%
            /// </summary>
            public byte accPedalDepth; 

            /// <summary>
            /// byte2
            /// 制动踏板行程型号，分辨率0.4%，偏移量0，范围0~100%
            /// </summary>
            public byte brakePedalDepth; 

            /// <summary>
            /// byte3，bit8,0无电刹，1有电刹
            /// </summary>
            public byte eBrakeState;
            /// <summary>
            /// byte3
            /// 0制动踏板无效，1制动踏板有效，bit7
            /// </summary>
            public byte brakePedalEff; 
            /// <summary>
            /// byte3
            /// 0加速踏板无效，1加速踏板有效，bit6
            /// </summary>
            public byte accPedalEff; //
            /// <summary>
            /// byte3
            /// 0DC/AC不使能，1DC/AC使能，bit5
            /// </summary>
            public byte dcacEnable; 
            /// <summary>
            /// byte3
            /// 请求空调降功率运行，0未请求，1有请求，bit4
            /// </summary>
            public byte askAirDec; //
            /// <summary>
            /// byte3
            /// 请求空调停机，0未请求，1，有请求，bit3
            /// </summary>
            public byte askAirStop; //
            /// <summary>
            /// byte3
            /// DC/DC使能状况，0未使能，1使能，bit2
            /// </summary>
            public byte dcdcEnable; //
            /// <summary>
            /// byte3
            /// 充电放电状态，0充电，1放电，bit1
            /// </summary>
            public byte genElecOrElecState; //

            //byte4
            /// <summary>
            /// byte4
            /// 车辆互锁，0互锁无效，1互锁有效，byte4，bit8
            /// </summary>
            public byte vehLockOther; 
            /// <summary>
            /// byte4
            /// 三级故障，强制停车模式，0非强制停车，1强制停车，bit7
            /// </summary>
            public byte forceParkMode; //
            /// <summary>
            /// byte4
            /// 二级故障，强制降功率模式，0未降功率，1降功率，bit6
            /// </summary>
            public byte forceDecPowerMode; //
            /// <summary>
            /// byte4
            /// 四级故障，强制断开主接触器，0未断开，1断开，bit5
            /// </summary>
            public byte forceDisCMainContactor; //
            //bit4/3保留
            /// <summary>
            /// byte4
            /// 档位，00空挡(N)，01前进挡(D)，10后退档(R)，11错误，bit2/1
            /// </summary>
            public byte gear; //

            //byte5
            /// <summary>
            /// byte5
            /// 整车系统故障，0无效，1有效，byte5，bit8
            /// </summary>
            public byte vehSystemError; //
            /// <summary>
            /// byte5
            /// 预充超时，0未超时，1超时，bit7
            /// </summary>
            public byte preChargeTimeout; //
            /// <summary>
            /// byte5
            /// 电机控制器节点掉线，0未掉线，1掉线，bit6
            /// </summary>
            public byte mtclOffLine; //
            
            //bit5 保留
            /// <summary>
            /// byte5
            /// 电池管理系统主控节点掉线，0未掉线，1掉线，bit4
            /// </summary>
            public byte bmsOffline; //
            /// <summary>
            /// byte5
            /// READY有效，0无效，1有效，bit3
            /// </summary>
            public byte ready; //
            //bit2/1 保留

            //byte 7/6 保留

            /// <summary>
            /// byte8
            /// 整车控制器寿命，分辨率1/bit，偏移量0，范围0~255
            /// </summary>
            public byte vcuLife; 

        }

        /// <summary>
        /// 整车控制器2 故障检查及MCU使能、电机控制模式、扭矩转速设定
        /// ID:0x0CFA00D0
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_EVCU2
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x0CFA00D0;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            //byte1
            //bit8/7保留
            /// <summary>
            /// byte1
            /// 主负接触器状态反馈，0断开，1闭合，bit6
            /// </summary>
            public byte mNegContactorStateFeedbk; //
            /// <summary>
            /// byte1
            /// Key Start，0无效，1有效，bit5
            /// </summary>
            public byte keyStart; 
            /// <summary>
            /// byte1
            /// 清除故障（一直发0），0无效，1有效，bit4
            /// </summary>
            public byte clearFaults; 
            /// <summary>
            /// byte1
            /// 预充主接触器状态反馈，0断开，1闭合，bit3
            /// </summary>
            public byte preChrCtSFeedBk;   
            /// <summary>
            /// byte1
            /// 主正接触器状态反馈，0断开，1闭合，bit2
            /// </summary>
            public byte mPosContactorStateFeedbk; 
            /// <summary>
            /// byte1
            /// MCU使能，0无效，1有效，bit1
            /// </summary>
            public byte enMcu; 

            
            //bit8-3保留
            /// <summary>
            /// byte2
            /// 电机控制模式，00备用，01转速控制模式，10扭矩控制模式，11主动放电模式，bit2/1
            /// </summary>
            public byte mtclMode;

            //byte3保留

            /// <summary>
            /// byte5/4
            /// 请求扭矩，分辨率0.5NM/bit，偏移量:-10000NM，范围-10000~10000NM，16bits
            /// </summary>
            public short askTorque;

            //byte6保留

            /// <summary>
            /// byte8/7
            /// 请求转速，分辨率1rpm/bit，偏移量-15000rpm，范围-15000rpm~15000rpm
            /// </summary>
            public short askSpeed;
        }
        

        /// <summary>
        /// 整车控制器3 转速上下限
        /// ID:0x0CF101D0
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_EVCU3
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x0CF101D0;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// byte2/1
            /// 转速指令上限
            /// 例如限速67Km/h，转速为2512rpm，限制倒车速度20Km/h，转速为380rpm            
            /// </summary>
            public ushort speedUpLimit;
            
            /// <summary>
            /// byte4/3
            /// 转速指令下限
            /// </summary>
            public ushort speedDownLimit;

            //byte8-byte3保留
        }

        /// <summary>
        /// 整车控制器4 故障汇报及强制限制、强制停车、强制断电
        /// ID:0x0CFB00D0
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_EVCU4
        {
            /// <summary>
            /// CAN ID
            /// </summary>
            public const uint canId = 0x0CFB00D0;
            /// <summary>
            /// 波特率
            /// </summary>
            public const uint baudrate = 250; //250Kbps

            /// <summary>
            /// byte1，bit8
            /// 电机及MCU系统故障，0无效，1有效
            /// </summary>
            public byte mtclSysFault;
            /// <summary>
            /// byte1，bit7
            /// MCU节点组在线
            /// </summary>
            public byte mcuNodeOffLine;    //
            /// <summary>
            /// byte1
            /// BMS系统故障，bit6
            /// </summary>
            public byte bmsSysFault;   //
            /// <summary>
            /// byte1
            /// BMS节点不在线，bit5
            /// </summary>
            public byte bmsNodeOffLine;    //
            /// <summary>
            /// byte1
            /// DC/DC三级故障，bit4
            /// </summary>
            public byte dcDcThirdFault;    //
            /// <summary>
            /// byte1
            /// DC/DC节点不在线，bit3
            /// </summary>
            public byte dcDcNodeOffLine;   //
            /// <summary>
            /// byte1
            /// 油泵DC/AC三级故障，bit2
            /// </summary>
            public byte oilPumpDcACThirdFault; //
            /// <summary>
            /// byte1
            /// 油泵DC/AC节点不在线，bit1
            /// </summary>
            public byte oilPumpDcACNodeOffLine;    //
            
            
            //byte2
            /// <summary>
            /// byte2
            /// 气泵DC/AC三级故障，bit8
            /// </summary>
            public byte airPumpDcACThirdFault; //
            /// <summary>
            /// byte2
            /// 气泵DC/AC节点不在线，bit7
            /// </summary>
            public byte airPumpDcACNodeOffLine;    //
            /// <summary>
            /// byte2
            /// 绝缘故障，bit6
            /// </summary>
            public byte insulationFault;   //
            /// <summary>
            /// byte2
            /// 绝缘监测仪节点不在线，bit5
            /// </summary>
            public byte insulationDetecterOffLine;    //
            /// <summary>
            /// byte2
            /// 表系统故障，bit4
            /// </summary>
            public byte dashboardSysFault; //仪
            /// <summary>
            /// byte2
            /// 仪表节点不在线，bit3
            /// </summary>
            public byte dashboardNodeOffLine;  //
            /// <summary>
            /// byte2
            /// 低气压报警，bit2
            /// </summary>
            public byte lowGasPressAlarm;  //
            /// <summary>
            /// byte2
            /// 24V蓄电池欠压报警，bit1
            /// </summary>
            public byte batteryLowerAlarm; //

            //byte3
            /// <summary>
            /// byte3
            /// </summary>
            public byte forceLimitPower;   //强制限功率模式，0无效，1有效，bit8
            /// <summary>
            /// byte3
            /// </summary>
            public byte forcePark; //强制停车模式，bit7
            /// <summary>
            /// byte3
            /// 
            /// </summary>
            public byte forceHVDiconnect;  //强制高压断电，bit6
            //bit5~1保留

            //byte4保留

            //byte5
            //bit8-6保留
            /// <summary>
            /// byte5
            /// 
            /// </summary>
            public byte backWareDoorLock;  //后舱门开互锁，0无效，1有效，bit5
            /// <summary>
            /// byte5
            /// 
            /// </summary>
            public byte chargeWareDoorLock;    //充电舱门开互锁，bit4
            /// <summary>
            /// byte5
            /// 
            /// </summary>
            public byte chargePlugConnectLock; //充电插头开互锁，bit3
            /// <summary>
            /// byte5
            /// 气压低开互锁，气压过低报警（前后桥气压<0.55Mpar）bit2
            /// </summary>
            public byte lowGapPressLock;   //
            /// <summary>
            /// byte5，bit1
            /// 取消回馈充电
            /// </summary>
            public byte cancelFeedBackCharge;  //

            //byte6~8保留
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
        /// 电机控制器1 寿命、故障、扭矩反馈、档位超温信息等反馈
        /// ID:0x0CFFEBEF
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct Scm_MCU1
        {
            public const uint canId = 0x0CFFEBEF;

            /// <summary>
            /// byte1
            /// 电机控制器life值，分辨率1/bit，偏移量0，范围：0~255
            /// </summary>
            public byte mtclLife;

            //byte2
            //bit8~4保留
            /// <summary>
            /// byte2,bit3-1
            /// 电机及电控故障等级，000正常，001驱动系统一级故障（报警），010驱动系统二级故障（降功率），011驱动系统三级故障（强制停车），100，驱动系统四级故障（断开接触器）
            /// </summary>
            public byte mtclFaultLevel; 
            
            //byte3
            //bit8/7保留
            /// <summary>
            /// byte3，bit6
            /// 预充电完成，电机控制器母线电压达到动力电池电压95%以上表示预充电完成，0未完成，1完成
            /// </summary>
            public byte preChargeCompleted; 
            /// <summary>
            /// byte3，bit5
            /// 驱动有效
            /// </summary>
            public byte accEffect; //
            /// <summary>
            /// byte3，bit4
            /// 制动有效
            /// </summary>
            public byte brakeEffect;   //
            /// <summary>
            /// byte3，bit3
            /// 空挡
            /// </summary>
            public byte neuralGear;    //
            /// <summary>
            /// byte3，bit2
            /// 前进挡
            /// </summary>
            public byte driveGear; //
            /// <summary>
            /// byte3，bit1
            /// 倒档
            /// </summary>
            public byte reverseGear;   //

            //bit8-3保留
            /// <summary>
            /// byte4，bit2
            /// 电机超温
            /// </summary>
            public byte motorOverTemperature;  //
            /// <summary>
            /// byte4，bit1
            /// 电控超温
            /// </summary>
            public byte mtclOverTemperature;

            //byte6/5保留

            /// <summary>
            /// byte8/7
            /// 电机扭矩反馈，分辨率1NM/bit，偏移量：-32000；范围-32000~32000NM
            /// </summary>
            public short motorTorqueFeedbk;
        }

        /// <summary>
        /// 电机控制器2 母线电压、电流
        /// ID:0x18FFECEF
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct Scm_MCU2
        {
            public const uint canId = 0x18FFECEF;

            /// <summary>
            /// byte2/1
            /// 电控母线电压，分辨率0.1V/bit，偏移量0，范围0~1000V
            /// </summary>
            public float mtclMotherVoltage;

            //byte4/3保留

            /// <summary>
            /// byte6/5
            /// 电控母线电流，分辨率0.1A/bit，偏移量-3200A，范围-3200A~3353.5A
            /// </summary>
            public float mtclMotherCurrent;

            //byte8/7保留
        }

        /// <summary>
        /// 电机控制器3 电机转速回馈，电控电机温度
        /// ID:0x0CFFEDEF
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:10ms
        /// </summary>
        struct Scm_MCU3
        {
            public const uint canId = 0x0CFFEDEF;

            /// <summary>
            /// byte2/1
            /// 电机转速，分辨率1rpm/bit，偏移量-20000rpm，范围-20000~20000rpm
            /// </summary>
            public short motorSpeed;

            //byte3保留

            /// <summary>
            /// byte4
            /// 驱动系统冷却请求，分辨率0.4%/bit，偏移量0，范围0-100%。请求比为冷却风扇转速
            /// </summary>
            public float mtclColdRequest;

            
            /// <summary>
            /// byte5
            /// 电控温度，分辨率1℃/bit，偏移量-40℃，范围-40~210℃
            /// </summary>
            public short mtclTemperature;

            //byte6保留

            /// <summary>
            /// byte7
            /// 电机温度，分辨率1℃/bit，偏移量-40℃，范围-40~210℃
            /// </summary>
            public short motorTemperature;

            //byte8保留
        }

        /// <summary>
        /// 电机控制器4 直流侧功率及功耗
        /// ID:0x18FFEEEF
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU4
        {
            public const uint canId = 0x18FFEEEF;

            /// <summary>
            /// byte2/1
            /// 控制器直流侧输入功率Pin，分辨率1Kw/bit，偏移量0，范围0-400Kwh
            /// </summary>
            public short mtclDcSideInputPower;

            /// <summary>
            /// byte3/2
            /// 驱动系统耗电量Ein，分辨率0.1Kwh/bit，偏移量0，范围0-1000Kwh
            /// </summary>
            public float mtclTotalElecConsumpt; 

            //byte8~5保留
        }

        /// <summary>
        /// 电机控制器5 电机故障
        /// ID:0x18FFEFEF
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:50ms
        /// </summary>
        struct Scm_MCU5
        {
            public const uint canId = 0x18FFEFEF;

            /// <summary>
            /// byte1
            /// 电机故障4，代码0-9，bit8/7/6/5
            /// </summary>
            public byte motorFaultCode4;

            /// <summary>
            /// byte1,bit4/3/2/1
            /// 电机故障3，代码0-9
            /// </summary>
            public byte motorFaultCode3; 

            /// <summary>
            /// byte2,bit8/7/6/5
            /// 电机故障2，代码0-9
            /// </summary>
            public byte motorFaultCode2;

            /// <summary>
            ///  byte2,bit4/3/2/1
            ///  电机故障1，代码0-9
            /// </summary>
            public byte motorFaultCode1;

            //byte8/7/6/5/4/3保留
        }

        /// <summary>
        /// 电机控制器程序版本
        /// ID:0x18FF24EF
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:500ms
        /// </summary>
        struct Scm_MCU_Version
        {
            public const uint canId = 0x18FF24EF;
            
            /// <summary>
            /// byte1
            /// 年BCD码
            /// </summary>
            public byte year;  //

            /// <summary>
            /// byte2
            /// 月BCD码
            /// </summary>
            public byte month; //

            /// <summary>
            /// byte3
            /// 日BCD码
            /// </summary>
            public byte day;   //

            /// <summary>
            /// byte4
            /// 小时BCD码
            /// </summary>
            public byte hour;  //

            /// <summary>
            /// byte5
            /// 分钟BCD码
            /// </summary>
            public byte minute;    //

            //byte6保留
                        
            /// <summary>
            /// byte7/8
            /// 版本号，分辨率0.1，偏移量0，范围0-10，只发送数字，字母V由仪表显示时自行增加
            /// </summary>
            public ushort version;
        }
        #endregion

        #region 结构体与数组转换

        #region EVCU命令转数组
        /// <summary>
        /// 整车控制器1 驱动信息
        /// </summary>
        /// <param name="scmEvcu1"></param>
        /// <returns></returns>
        CanStandardData ScmEvcu1ToBytes(Scm_EVCU1 scmEvcu1)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = Scm_EVCU1.canId;
            scmCanByte.baudrate = Scm_EVCU1.baudrate; //250K
            scmCanByte.datas = new byte[8]; //数据长度为8

            //byte1
            scmCanByte.datas[0] = (byte)(scmEvcu1.accPedalDepth / 0.4); //加速踏板行程信号，分辨率0.4%，偏移量0，范围0~100%,byte1

            //byte2
            scmCanByte.datas[1] = (byte)(scmEvcu1.brakePedalDepth / 0.4); //制动踏板行程型号，分辨率0.4%，偏移量0，范围0~100%,byte2

            //byte3
            scmCanByte.datas[2] = (byte)(Convert.ToByte(scmEvcu1.eBrakeState<<7) + //0无电刹，1有电刹，byte3，bit8
                        Convert.ToByte(scmEvcu1.brakePedalEff<<6) + //0制动踏板无效，1制动踏板有效，bit7
                        Convert.ToByte(scmEvcu1.accPedalEff<<5) + //0加速踏板无效，1加速踏板有效，bit6
                        Convert.ToByte(scmEvcu1.dcacEnable<<4) + //0DC/AD不使能，1DC/AC使能，bit5
                        Convert.ToByte(scmEvcu1.askAirDec<<3) + //请求空调降功率运行，0未请求，1有请求，bit4
                        Convert.ToByte(scmEvcu1.askAirStop<<2) + //请求空调停机，0未请求，1，有请求，bit3
                        Convert.ToByte(scmEvcu1.dcdcEnable<<1) + //DC/DC使能状况，0未使能，1使能，bit2
                        Convert.ToByte(scmEvcu1.genElecOrElecState)); //充电放电状态，0充电，1放电，bit1

            //byte4
            scmCanByte.datas[3] = (byte)(Convert.ToByte(scmEvcu1.vehLockOther<<7) + //车辆互锁，0互锁无效，1互锁有效，byte4，bit8
                        Convert.ToByte(scmEvcu1.forceParkMode<<6) + //三级故障，强制停车模式，0非强制停车，1强制停车，bit7
                        Convert.ToByte(scmEvcu1.forceDecPowerMode<<5) + //二级故障，强制降功率模式，0未降功率，1降功率，bit6
                        Convert.ToByte(scmEvcu1.forceDisCMainContactor<<4) + //四级故障，强制断开主接触器，0未断开，1断开，bit5
                        //bit4/3保留
                        Convert.ToByte(scmEvcu1.gear)); //DNR状态，00空挡(N)，01前进挡(D)，10后退档(R)，11错误，bit2/1

            //byte5
            scmCanByte.datas[4] = (byte)(Convert.ToByte(scmEvcu1.vehSystemError<<7) + //整车系统故障，0无效，1有效，byte5，bit8
                        Convert.ToByte(scmEvcu1.preChargeTimeout<<6) + //预充超时，0未超时，1超时，bit7
                        Convert.ToByte(scmEvcu1.mtclOffLine<<5) + //电机控制器节点掉线，0未掉线，1掉线，bit6
                        Convert.ToByte(scmEvcu1.bmsOffline<<4) + //电池管理系统主控节点掉线，0未掉线，1掉线，bit4
                        Convert.ToByte(scmEvcu1.ready)<<3); //READY有效，0无效，1有效，bit3
                        //bit2/1 保留

            //byte 7/6 保留

            //byte8
            scmCanByte.datas[7] = scmEvcu1.vcuLife; //整车控制器寿命，分辨率1/bit，偏移量0，范围0~255

            return scmCanByte;
        }

        /// <summary>
        /// 整车控制器2 故障检查及MCU使能、电机控制模式、扭矩转速设定
        /// </summary>
        /// <param name="scmEvcu2"></param>
        /// <returns></returns>
        CanStandardData ScmEvcu2ToBytes(Scm_EVCU2 scmEvcu2)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = Scm_EVCU2.canId;
            scmCanByte.baudrate = Scm_EVCU2.baudrate; //250K
            scmCanByte.datas = new byte[8]; //数据长度为8

            //byte1
            //bit8/7保留
            scmCanByte.datas[0] = (byte)(Convert.ToByte(scmEvcu2.mNegContactorStateFeedbk<<5) + //主负接触器状态反馈，0断开，1闭合，bit6
                        Convert.ToByte(scmEvcu2.keyStart<<4) + //Key Start，0无效，1有效，bit5
                        Convert.ToByte(scmEvcu2.clearFaults<<3) + //清除故障（一直发0），0无效，1有效，bit4
                        Convert.ToByte(scmEvcu2.preChrCtSFeedBk<<2) +   //预充主接触器状态反馈，0断开，1闭合，bit3
                        Convert.ToByte(scmEvcu2.mPosContactorStateFeedbk<<1) + //主正接触器状态反馈，0断开，1闭合，bit2
                        Convert.ToByte(scmEvcu2.enMcu)); //MCU使能，0无效，1有效，bit1

            //byte2
            //bit8-bit3保留
            scmCanByte.datas[1] = Convert.ToByte(scmEvcu2.mtclMode);  //电机控制模式，00备用，01转速控制模式，10扭矩控制模式，11主动放电模式，bit2/1

            //byte3保留

            //byte5/4
            ushort askTorqueBits = Convert.ToUInt16(scmEvcu2.askTorque*2 + 10000*2);
            scmCanByte.datas[3] = (byte)(askTorqueBits & 0xff); //请求扭矩，分辨率0.5NM/bit，偏移量:-10000NM，范围-10000~10000NM，16bits
            scmCanByte.datas[4] = (byte)((askTorqueBits >> 8) & 0xff);

            //byte6保留

            //byte8/7
            ushort askSpeedBits = Convert.ToUInt16(scmEvcu2.askSpeed+15000); //转换为无符号数值，加上偏移量
            scmCanByte.datas[6] = Convert.ToByte(askSpeedBits & 0xff); //请求转速，分辨率1rpm/bit，偏移量-15000rpm，范围-15000rpm~15000rpm
            scmCanByte.datas[7] = Convert.ToByte((askSpeedBits >> 8) & 0xff);

            return scmCanByte;
        }

        /// <summary>
        /// 整车控制器3 转速上下限
        /// </summary>
        /// <param name="scmEvcu3"></param>
        /// <returns></returns>
        CanStandardData ScmEvcu3ToBytes(Scm_EVCU3 scmEvcu3)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = Scm_EVCU3.canId;
            scmCanByte.baudrate = Scm_EVCU3.baudrate; //250K
            scmCanByte.datas = new byte[8]; //数据长度为8

            //byte2/1
            scmCanByte.datas[0] = Convert.ToByte(scmEvcu3.speedUpLimit & 0xff); //转速指令上限，例如限速67Km/h，转速为2512rpm，限制倒车速度20Km/h，转速为380rpm            
            scmCanByte.datas[1] = Convert.ToByte(scmEvcu3.speedUpLimit>>8 & 0xff);

            //byte4/3
            scmCanByte.datas[2] = Convert.ToByte(scmEvcu3.speedDownLimit & 0xff);  //转速指令下限
            scmCanByte.datas[3] = Convert.ToByte(scmEvcu3.speedDownLimit>>8 & 0xff);

            //byte8-byte3保留

            return scmCanByte;
        }

        /// <summary>
        /// 整车控制器4 故障汇报及强制限制、强制停车、强制断电
        /// </summary>
        /// <param name="scmEvcu4"></param>
        /// <returns></returns>
        CanStandardData ScmEvcu4ToBytes(Scm_EVCU4 scmEvcu4)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = Scm_EVCU4.canId;
            scmCanByte.baudrate = Scm_EVCU4.baudrate; //250K
            scmCanByte.datas = new byte[8]; //数据长度为8

            //byte1
            scmCanByte.datas[0] = (byte)(Convert.ToByte(scmEvcu4.mtclSysFault<<7) + //电机及MCU系统故障，0无效，1有效，bit8
                        Convert.ToByte(scmEvcu4.mcuNodeOffLine<<6) +    //MCU节点组在线，bit7
                        Convert.ToByte(scmEvcu4.bmsSysFault<<5) +   //BMS系统故障，bit6
                        Convert.ToByte(scmEvcu4.bmsNodeOffLine<<4) +    //BMS节点不在线，bit5
                        Convert.ToByte(scmEvcu4.dcDcThirdFault<<3) +    //DC/DC三级故障，bit4
                        Convert.ToByte(scmEvcu4.dcDcNodeOffLine<<2) +  //DC/DC节点不在线，bit3
                        Convert.ToByte(scmEvcu4.oilPumpDcACThirdFault<<1) + //油泵DC/AC三级故障，bit2
                        Convert.ToByte(scmEvcu4.oilPumpDcACNodeOffLine));    //油泵DC/AC节点不在线，bit1
            
            //byte2
            scmCanByte.datas[1] = (byte)(Convert.ToByte(scmEvcu4.airPumpDcACThirdFault<<7) + //气泵DC/AC三级故障，bit8
                        Convert.ToByte(scmEvcu4.airPumpDcACNodeOffLine<<6) +    //气泵DC/AC节点不在线，bit7
                        Convert.ToByte(scmEvcu4.insulationFault<<5) +  //绝缘故障，bit6
                        Convert.ToByte(scmEvcu4.insulationDetecterOffLine<<4) +    //绝缘监测仪节点不在线，bit5
                        Convert.ToByte(scmEvcu4.dashboardSysFault<<3) + //仪表系统故障，bit4
                        Convert.ToByte(scmEvcu4.dashboardNodeOffLine<<2) +  //仪表节点不在线，bit3
                        Convert.ToByte(scmEvcu4.lowGasPressAlarm<<1) +  //低气压报警，bit2
                        Convert.ToByte(scmEvcu4.batteryLowerAlarm)); //24V蓄电池欠压报警，bit1

            //byte3
            scmCanByte.datas[2] = (byte)(Convert.ToByte(scmEvcu4.forceLimitPower<<7) +   //强制限功率模式，0无效，1有效，bit8
                        Convert.ToByte(scmEvcu4.forcePark<<6) + //强制停车模式，bit7
                        Convert.ToByte(scmEvcu4.forceHVDiconnect<<5));  //强制高压断电，bit6
            //bit5~1保留

            //byte4保留

            //byte5
            scmCanByte.datas[4] = (byte)(Convert.ToByte(scmEvcu4.backWareDoorLock<<4) +  //后舱门开互锁，0无效，1有效，bit5
                        Convert.ToByte(scmEvcu4.chargeWareDoorLock<<3) +    //充电舱门开互锁，bit4
                        Convert.ToByte(scmEvcu4.chargePlugConnectLock<<2) + //充电插头开互锁，bit3
                        Convert.ToByte(scmEvcu4.lowGapPressLock<<1) +   //气压低开互锁，气压过低报警（前后桥气压<0.55Mpar）bit2
                        Convert.ToByte(scmEvcu4.cancelFeedBackCharge));  //取消回馈充电，bit1

            //byte8~6保留

            return scmCanByte;
        }
        #endregion

        #region 数组转EVCU
        /// <summary>
        /// 整车控制器1 驱动信息
        /// </summary>
        /// <param name="scmEvcu1"></param>
        /// <returns></returns>
        Scm_EVCU1 BytesToScmEvcu1(CanStandardData scmCanByte1)
        {
            Scm_EVCU1 scmEvcu1 = new Scm_EVCU1();

            //byte1
            scmEvcu1.accPedalDepth = Convert.ToByte(scmCanByte1.datas[0] * 0.4); //加速踏板行程信号，分辨率0.4%，偏移量0，范围0~100%,byte1
            //byte2
            scmEvcu1.brakePedalDepth = Convert.ToByte(scmCanByte1.datas[1] * 0.4);//制动踏板行程型号，分辨率0.4%，偏移量0，范围0~100%,byte2
            //byte3
            scmEvcu1.eBrakeState = (byte)((scmCanByte1.datas[2] >> 7) & 0x01);//0无电刹，1有电刹，byte3，bit8
            scmEvcu1.brakePedalEff = (byte)((scmCanByte1.datas[2] >> 6) & 0x01); //0制动踏板无效，1制动踏板有效，bit7
            scmEvcu1.accPedalEff = (byte)((scmCanByte1.datas[2] >> 5) & 0x01);//0加速踏板无效，1加速踏板有效，bit6
            scmEvcu1.dcacEnable = (byte)((scmCanByte1.datas[2] >> 4) & 0x01);//0DC/AD不使能，1DC/AC使能，bit5
            scmEvcu1.askAirDec = (byte)((scmCanByte1.datas[2] >> 3) & 0x01);//请求空调降功率运行，0未请求，1有请求，bit4
            scmEvcu1.askAirStop = (byte)((scmCanByte1.datas[2] >> 2) & 0x01);//请求空调停机，0未请求，1，有请求，bit3
            scmEvcu1.dcdcEnable = (byte)((scmCanByte1.datas[2] >> 1) & 0x01);//DC/DC使能状况，0未使能，1使能，bit2
            scmEvcu1.genElecOrElecState = (byte)(scmCanByte1.datas[2] & 0x01);//充电放电状态，0充电，1放电，bit1


            //byte4
            scmEvcu1.vehLockOther = (byte)((scmCanByte1.datas[3] >> 7) & 0x01);//车辆互锁，0互锁无效，1互锁有效，byte4，bit8
            scmEvcu1.forceParkMode = (byte)((scmCanByte1.datas[3] >> 6) & 0x01);//三级故障，强制停车模式，0非强制停车，1强制停车，bit7
            scmEvcu1.forceDecPowerMode = (byte)((scmCanByte1.datas[3] >> 5) & 0x01);//二级故障，强制降功率模式，0未降功率，1降功率，bit6
            scmEvcu1.forceDisCMainContactor = (byte)((scmCanByte1.datas[3] >> 4) & 0x01);//四级故障，强制断开主接触器，0未断开，1断开，bit5
            //bit4/3保留
            scmEvcu1.gear = Convert.ToByte((scmCanByte1.datas[3]) & 0x03);//DNR状态，00空挡(N)，01前进挡(D)，10后退档(R)，11错误，bit2/1


            //byte5
            scmEvcu1.vehSystemError = (byte)((scmCanByte1.datas[4] >> 7) & 0x01);//整车系统故障，0无效，1有效，byte5，bit8
            scmEvcu1.preChargeTimeout = (byte)((scmCanByte1.datas[4] >> 6) & 0x01); //预充超时，0未超时，1超时，bit7
            scmEvcu1.mtclOffLine = (byte)((scmCanByte1.datas[4] >> 5) & 0x01); //电机控制器节点掉线，0未掉线，1掉线，bit6
            scmEvcu1.bmsOffline = (byte)((scmCanByte1.datas[4] >> 4) & 0x01); //电池管理系统主控节点掉线，0未掉线，1掉线，bit4
            scmEvcu1.ready = (byte)((scmCanByte1.datas[4] >> 3) & 0x01); //READY有效，0无效，1有效，bit3

            //bit2/1 保留

            //byte 7/6 保留

            //byte8
            scmEvcu1.vcuLife = scmCanByte1.datas[7]; //整车控制器寿命，分辨率1/bit，偏移量0，范围0~255

            return scmEvcu1;
        }

        /// <summary>
        /// 整车控制器2 故障检查及MCU使能、电机控制模式、扭矩转速设定
        /// </summary>
        /// <param name="scmEvcu2"></param>
        /// <returns></returns>
        Scm_EVCU2 BytesToScmEvcu2(CanStandardData scmCanByte2)
        {
            Scm_EVCU2 scmEvcu2 = new Scm_EVCU2();

            //byte1
            //bit8/7保留
            scmEvcu2.mNegContactorStateFeedbk = (byte)((scmCanByte2.datas[0] >> 5) & 0x01);//主负接触器状态反馈，0断开，1闭合，bit6
            scmEvcu2.keyStart = (byte)((scmCanByte2.datas[0] >> 4) & 0x01);//Key Start，0无效，1有效，bit5
            scmEvcu2.clearFaults = (byte)((scmCanByte2.datas[0] >> 3) & 0x01) ;//清除故障（一直发0），0无效，1有效，bit4
            scmEvcu2.preChrCtSFeedBk = (byte)((scmCanByte2.datas[0] >> 2) & 0x01); //预充主接触器状态反馈，0断开，1闭合，bit3
            scmEvcu2.mPosContactorStateFeedbk = (byte)((scmCanByte2.datas[0] >> 1) & 0x01); //主正接触器状态反馈，0断开，1闭合，bit2
            scmEvcu2.enMcu = (byte)(scmCanByte2.datas[0] & 0x01); //MCU使能，0无效，1有效，bit1


            //byte2
            //bit8-bit3保留
            scmEvcu2.mtclMode = Convert.ToByte(scmCanByte2.datas[1] & 0x03);  //电机控制模式，00备用，01转速控制模式，10扭矩控制模式，11主动放电模式，bit2/1

            //byte3保留

            //byte5/4
            scmEvcu2.askTorque = Convert.ToInt16((scmCanByte2.datas[4] * 256 + scmCanByte2.datas[3]) * 0.5 - 10000);//请求扭矩，分辨率0.5NM/bit，偏移量:-10000NM，范围-10000~10000NM，16bits

            //byte6保留

            //byte8/7
            scmEvcu2.askSpeed = Convert.ToInt16(scmCanByte2.datas[7] * 256 + scmCanByte2.datas[6] - 15000); //请求转速，分辨率1rpm/bit，偏移量-15000rpm，范围-15000rpm~15000rpm

            return scmEvcu2;
        }

        /// <summary>
        /// 整车控制器3 转速上下限
        /// </summary>
        /// <param name="scmEvcu3"></param>
        /// <returns></returns>
        Scm_EVCU3 BytesToScmEvcu3(CanStandardData scmCanByte3)
        {
            Scm_EVCU3 scmEvcu3 = new Scm_EVCU3();
            //byte2/1
            scmEvcu3.speedUpLimit = Convert.ToUInt16(scmCanByte3.datas[1] * 256 + scmCanByte3.datas[0]);//转速指令上限，例如限速67Km/h，转速为2512rpm，限制倒车速度20Km/h，转速为380rpm            


            //byte4/3
            scmEvcu3.speedDownLimit = Convert.ToUInt16(scmCanByte3.datas[3] * 256 + scmCanByte3.datas[2]); //转速指令下限

            //byte8-byte3保留

            return scmEvcu3;
        }

        /// <summary>
        /// 整车控制器4 故障汇报及强制限制、强制停车、强制断电
        /// </summary>
        /// <param name="scmEvcu4"></param>
        /// <returns></returns>
        Scm_EVCU4 BytesToScmEvcu4(CanStandardData scmCanByte4)
        {
            Scm_EVCU4 scmEvcu4 = new Scm_EVCU4();
            //byte1
            scmEvcu4.mtclSysFault = (byte)((scmCanByte4.datas[0] >> 7) & 0x01);//电机及MCU系统故障，0无效，1有效，bit8
            scmEvcu4.mcuNodeOffLine = (byte)((scmCanByte4.datas[0] >> 6) & 0x01); //MCU节点组在线，bit7
            scmEvcu4.bmsSysFault = (byte)((scmCanByte4.datas[0] >> 5) & 0x01);//BMS系统故障，bit6
            scmEvcu4.bmsNodeOffLine = (byte)((scmCanByte4.datas[0] >> 4) & 0x01);//BMS节点不在线，bit5
            scmEvcu4.dcDcThirdFault = (byte)((scmCanByte4.datas[0] >> 3) & 0x01);//DC/DC三级故障，bit4
            scmEvcu4.dcDcNodeOffLine = (byte)((scmCanByte4.datas[0] >> 2) & 0x01);//DC/DC节点不在线，bit3
            scmEvcu4.oilPumpDcACThirdFault = (byte)((scmCanByte4.datas[0] >> 1) & 0x01);//油泵DC/AC三级故障，bit2
            scmEvcu4.oilPumpDcACNodeOffLine = (byte)(scmCanByte4.datas[0] & 0x01);//油泵DC/AC节点不在线，bit1


            //byte2
            scmEvcu4.airPumpDcACThirdFault = (byte)((scmCanByte4.datas[1] >> 7) & 0x01);//气泵DC/AC三级故障，bit8
            scmEvcu4.airPumpDcACNodeOffLine = (byte)((scmCanByte4.datas[1] >> 6) & 0x01); //气泵DC/AC节点不在线，bit7
            scmEvcu4.insulationFault = (byte)((scmCanByte4.datas[1] >> 5) & 0x01);//绝缘故障，bit6
            scmEvcu4.insulationDetecterOffLine = (byte)((scmCanByte4.datas[1] >> 4) & 0x01);//绝缘监测仪节点不在线，bit5
            scmEvcu4.dashboardSysFault = (byte)((scmCanByte4.datas[1] >> 3) & 0x01);//仪表系统故障，bit4
            scmEvcu4.dashboardNodeOffLine = (byte)((scmCanByte4.datas[1] >> 2) & 0x01);//仪表节点不在线，bit3
            scmEvcu4.lowGasPressAlarm = (byte)((scmCanByte4.datas[1] >> 1) & 0x01);//低气压报警，bit2
            scmEvcu4.batteryLowerAlarm = (byte)(scmCanByte4.datas[1] & 0x01);//24V蓄电池欠压报警，bit1


            //byte3
            scmEvcu4.forceLimitPower = (byte)((scmCanByte4.datas[2] >> 7) & 0x01);//强制限功率模式，0无效，1有效，bit8
            scmEvcu4.forcePark = (byte)((scmCanByte4.datas[2] >> 6) & 0x01); //强制停车模式，bit7
            scmEvcu4.forceHVDiconnect = (byte)((scmCanByte4.datas[2] >> 5) & 0x01);//强制高压断电，bit6

            //bit5~1保留

            //byte4保留

            //byte5
            scmEvcu4.backWareDoorLock = (byte)((scmCanByte4.datas[4] >> 4) & 0x01);//后舱门开互锁，0无效，1有效，bit5
            scmEvcu4.chargeWareDoorLock = (byte)((scmCanByte4.datas[4] >> 3) & 0x01);//充电舱门开互锁，bit4
            scmEvcu4.chargePlugConnectLock = (byte)((scmCanByte4.datas[4] >> 2) & 0x01);//充电插头开互锁，bit3
            scmEvcu4.lowGapPressLock = (byte)((scmCanByte4.datas[4] >> 1) & 0x01);//气压低开互锁，气压过低报警（前后桥气压<0.55Mpar）bit2
            scmEvcu4.cancelFeedBackCharge = (byte)(scmCanByte4.datas[4] & 0x01); //取消回馈充电，bit1

            //byte8~6保留

            return scmEvcu4;
        }
        #endregion

        #region 数组转MCU命令
        /// <summary>
        /// 电机控制器1 寿命、故障、扭矩反馈、档位超温信息等反馈
        /// ID:0x0CFFEBEF
        /// </summary>
        /// <param name="scmCanByte1"></param>
        /// <returns></returns>
        Scm_MCU1 ByteToScmMcu1(CanStandardData scmCanByte1)
        {
            Scm_MCU1 scmMcu1 = new Scm_MCU1();

            //byte1
            scmMcu1.mtclLife = scmCanByte1.datas[0]; //电机控制器life值，分辨率1/bit，偏移量0，范围：0~255

            //byte2
            //bit8~4保留
            scmMcu1.mtclFaultLevel = Convert.ToByte(scmCanByte1.datas[1] & 0x0f);    //电机及电控故障等级，000正常，001驱动系统一级故障（报警），010驱动系统二级故障（降功率），011驱动系统三级故障（强制停车），100，驱动系统四级故障（断开接触器）
            
            //byte3
            scmMcu1.preChargeCompleted = (byte)((scmCanByte1.datas[2]>>5)&0x01);    //预充电完成，电机控制器母线电压达到动力电池电压95%以上表示预充电完成，0未完成，1完成，bit6
            scmMcu1.accEffect = (byte)((scmCanByte1.datas[2] >> 4)&0x01); //驱动有效，bit5
            scmMcu1.brakeEffect = (byte)((scmCanByte1.datas[2] >> 3)&0x01);   //制动有效，bit4
            scmMcu1.neuralGear = (byte)((scmCanByte1.datas[2] >> 2)&0x01);    //空挡，bit3
            scmMcu1.driveGear = (byte)((scmCanByte1.datas[2] >> 1)&0x01); //前进挡，bit2
            scmMcu1.reverseGear = (byte)((scmCanByte1.datas[2]) & 0x01);   //倒档，bit1

            //byte4
            scmMcu1.motorOverTemperature = (byte)((scmCanByte1.datas[3] >> 1) & 0x01);  //电机超温，bit2
            scmMcu1.mtclOverTemperature = (byte)((scmCanByte1.datas[3]) & 0x01);   //电控超温，bit1

            //byte6/5保留

            //byte8/7
            short motorTorqueFeedbk = Convert.ToInt16(scmCanByte1.datas[7] * 256 + scmCanByte1.datas[6] - 32000); //转换为有符号数值，减上偏移量
            scmMcu1.motorTorqueFeedbk = motorTorqueFeedbk; //电机扭矩反馈，分辨率1NM/bit，偏移量：-32000；范围-32000~32000NM

            return scmMcu1;
        }

        /// <summary>
        /// 电机控制器2 母线电压、电流
        /// ID:0x18FFECEF
        /// </summary>
        /// <param name="scmCanByte2"></param>
        /// <returns></returns>
        Scm_MCU2 ByteToScmMcu2(CanStandardData scmCanByte2)
        {
            Scm_MCU2 scmMcu2 = new Scm_MCU2();

            scmMcu2.mtclMotherVoltage = Convert.ToSingle((scmCanByte2.datas[1] * 256 + scmCanByte2.datas[0])*0.1); //0.1V/bit，偏移量0，范围0~1000V
            scmMcu2.mtclMotherCurrent = Convert.ToSingle((scmCanByte2.datas[5] * 256 + scmCanByte2.datas[4])*0.1 - 3200); // 电控母线电流，分辨率0.1A/bit，偏移量-3200A，范围-3200A~3353.5A
            
            return scmMcu2;
        }

        /// <summary>
        /// 电机控制器3 电机转速回馈，电控电机温度
        /// ID:0x0CFFEDEF
        /// </summary>
        /// <param name="scmCanByte3"></param>
        /// <returns></returns>
        Scm_MCU3 ByteToScmMcu3(CanStandardData scmCanByte3)
        {
            Scm_MCU3 scmMcu3 = new Scm_MCU3();

            //byte2/1
            short motorSpeed = Convert.ToInt16(scmCanByte3.datas[1]*256 + scmCanByte3.datas[0] -20000); //转化为有符号，减去偏移量
            scmMcu3.motorSpeed = motorSpeed;  //电机转速，分辨率1rpm/bit，偏移量-20000rpm，范围-20000~20000rpm

            //byte3保留

            //byte4
            scmMcu3.mtclColdRequest = Convert.ToSingle(scmCanByte3.datas[3] * 0.4);   //驱动系统冷却请求，分辨率0.4%/bit，偏移量0，范围0-100%。请求比为冷却风扇转速

            //byte5
            short mtclTemperature = Convert.ToInt16(scmCanByte3.datas[4] - 40); //转换为有符号，减去偏移量
            scmMcu3.mtclTemperature = mtclTemperature;   //电控温度，分辨率1℃/bit，偏移量-40℃，范围-40~210℃

            //byte6保留

            //byte7
            short motorTemperature = Convert.ToInt16(scmCanByte3.datas[6] - 40); //转换为有符号，减去偏移量
            scmMcu3.motorTemperature = motorTemperature;  //电机温度，分辨率1℃/bit，偏移量-40℃，范围-40~210℃

            //byte8保留

            return scmMcu3;
        }

        /// <summary>
        /// 电机控制器4 直流侧功率及功耗
        /// ID:0x18FFEEEF
        /// </summary>
        /// <returns></returns>
        Scm_MCU4 ByteToScmMcu4(CanStandardData scmCanByte4)
        { 
            Scm_MCU4 scmMcu4 = new Scm_MCU4();

            scmMcu4.mtclDcSideInputPower = Convert.ToInt16(scmCanByte4.datas[1] * 256 + scmCanByte4.datas[0]);  //直流侧功率，1Kw/bit，偏移量0，范围0-400Kw

            scmMcu4.mtclTotalElecConsumpt = Convert.ToSingle((scmCanByte4.datas[3] * 256 + scmCanByte4.datas[2])*0.1); //总耗能，0.1Kwh/bit，范围0-1000kwh
            return scmMcu4;
        }

        /// <summary>
        /// 电机控制器5 电机故障
        /// ID:0x18FFEFEF
        /// </summary>
        /// <param name="scmCanByte5"></param>
        /// <returns></returns>
        Scm_MCU5 ByteToScmMcu5(CanStandardData scmCanByte5)
        {
            Scm_MCU5 scmMcu5 = new Scm_MCU5();

            //byte1
            scmMcu5.motorFaultCode4 = (byte)((scmCanByte5.datas[0] >> 4 & 0x0f));   //电机故障4，代码0-9，bit8/7/6/5
            scmMcu5.motorFaultCode3 = (byte)(scmCanByte5.datas[0]&0x0f);   //电机故障3，代码0-9，bit4/3/2/1

            //byte2
            scmMcu5.motorFaultCode2 = (byte)((scmCanByte5.datas[1] >> 4 & 0x0f));   //电机故障2，代码0-9，bit8/7/6/5
            scmMcu5.motorFaultCode1 = (byte)(scmCanByte5.datas[1] & 0x0f);   //电机故障1，代码0-9，bit4/3/2/1

            //byte8/7/6/5/4/3保留

            return scmMcu5;
        }

        byte BcdToByte(byte bcd)
        {
            return (byte)((0xff & (bcd >> 4)) * 10 + (0xf & bcd));
        }

        Scm_MCU_Version ByteToScmMcuVersion(CanStandardData scmCanByteVersion)
        {
            Scm_MCU_Version scmVersion = new Scm_MCU_Version();
            scmVersion.year = BcdToByte(scmCanByteVersion.datas[0]);
            scmVersion.month = BcdToByte(scmCanByteVersion.datas[1]);
            scmVersion.day = BcdToByte(scmCanByteVersion.datas[2]);
            scmVersion.hour = BcdToByte(scmCanByteVersion.datas[3]);
            scmVersion.minute = BcdToByte(scmCanByteVersion.datas[4]);
            scmVersion.version = (ushort)((scmCanByteVersion.datas[7] * 256 + scmCanByteVersion.datas[6])/10);
            return scmVersion;
        }
        #endregion

        #region MCU命令转数组
        /// <summary>
        /// 电机控制器1 寿命、故障、扭矩反馈、档位超温信息等反馈
        /// ID:0x0CFFEBEF
        /// </summary>
        /// <param name="scmCanByte1"></param>
        /// <returns></returns>
        CanStandardData ScmMcu1ToBytes(Scm_MCU1 scmMcu1)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = 0x0CFFEBEF;
            scmCanByte.baudrate = 250;
            scmCanByte.datas = new byte[8];

            //byte1
            scmCanByte.datas[0] = scmMcu1.mtclLife; //电机控制器life值，分辨率1/bit，偏移量0，范围：0~255

            //byte2
            //bit8~4保留
            scmCanByte.datas[1] = scmMcu1.mtclFaultLevel;    //电机及电控故障等级，000正常，001驱动系统一级故障（报警），010驱动系统二级故障（降功率），011驱动系统三级故障（强制停车），100，驱动系统四级故障（断开接触器）

            //byte3
            scmCanByte.datas[2] = Convert.ToByte((scmMcu1.preChargeCompleted  << 5) + (scmMcu1.accEffect << 4) +
                (scmMcu1.brakeEffect  << 3) + (scmMcu1.neuralGear << 2) + (scmMcu1.driveGear  << 1) +
                scmMcu1.reverseGear);
            //预充电完成，电机控制器母线电压达到动力电池电压95%以上表示预充电完成，0未完成，1完成，bit6
            //驱动有效，bit5
            //制动有效，bit4
            //空挡，bit3
            //前进挡，bit2
            //倒档，bit1

            //byte4
            scmCanByte.datas[3] = Convert.ToByte((scmMcu1.motorOverTemperature  << 1) + scmMcu1.mtclOverTemperature );
            //电机超温，bit2
            //电控超温，bit1

            //byte6/5保留

            //byte8/7
            scmCanByte.datas[6] = Convert.ToByte((scmMcu1.motorTorqueFeedbk + 32000) & 0xff);
            scmCanByte.datas[7] = Convert.ToByte(((scmMcu1.motorTorqueFeedbk + 32000) >> 8) & 0xff);

            //电机扭矩反馈，分辨率1NM/bit，偏移量：-32000；范围-32000~32000NM

            return scmCanByte;
        }

        /// <summary>
        /// 电机控制器2 母线电压、电流
        /// ID:0x18FFECEF
        /// </summary>
        /// <param name="scmCanByte2"></param>
        /// <returns></returns>
        CanStandardData ScmMcu2ToBytes(Scm_MCU2 scmMcu2)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = 0x18FFECEF;
            scmCanByte.baudrate = 250;
            scmCanByte.datas = new byte[8];

            //0.1V/bit，偏移量0，范围0~1000V
            scmCanByte.datas[0] = Convert.ToByte(Convert.ToInt32(scmMcu2.mtclMotherVoltage * 10) & 0xff);
            scmCanByte.datas[1] = Convert.ToByte(Convert.ToInt32(scmMcu2.mtclMotherVoltage * 10) >> 8 & 0xff);

            // 电控母线电流，分辨率0.1A/bit，偏移量-3200A，范围-3200A~3353.5A
            scmCanByte.datas[4] = Convert.ToByte(Convert.ToInt32((scmMcu2.mtclMotherCurrent + 3200) * 10) & 0xff);
            scmCanByte.datas[5] = Convert.ToByte(Convert.ToInt32((scmMcu2.mtclMotherCurrent + 3200) * 10) >> 8 & 0xff);

            return scmCanByte;
        }

        /// <summary>
        /// 电机控制器3 电机转速回馈，电控电机温度
        /// ID:0x0CFFEDEF
        /// </summary>
        /// <param name="scmCanByte3"></param>
        /// <returns></returns>
        CanStandardData ScmMcu3ToBytes(Scm_MCU3 scmMcu3)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = 0x0CFFEDEF;
            scmCanByte.baudrate = 250;
            scmCanByte.datas = new byte[8];

            //byte2/1
            scmCanByte.datas[0] = Convert.ToByte(Convert.ToUInt16(scmMcu3.motorSpeed + 20000) & 0xff);
            scmCanByte.datas[1] = Convert.ToByte((Convert.ToUInt16(scmMcu3.motorSpeed + 20000) >> 8) & 0xff);
            //电机转速，分辨率1rpm/bit，偏移量-20000rpm，范围-20000~20000rpm

            //byte3保留

            //byte4
            scmCanByte.datas[3] = Convert.ToByte(scmMcu3.mtclColdRequest / 0.4);
            //驱动系统冷却请求，分辨率0.4%/bit，偏移量0，范围0-100%。请求比为冷却风扇转速

            //byte5
            scmCanByte.datas[4] = Convert.ToByte(scmMcu3.mtclTemperature + 40);
            //电控温度，分辨率1℃/bit，偏移量-40℃，范围-40~210℃

            //byte6保留

            //byte7
            scmCanByte.datas[6] = Convert.ToByte(scmMcu3.motorTemperature + 40);
            //电机温度，分辨率1℃/bit，偏移量-40℃，范围-40~210℃

            //byte8保留

            return scmCanByte;
        }

        /// <summary>
        /// 电机控制器4 直流侧功率及功耗
        /// ID:0x18FFEEEF
        /// </summary>
        /// <returns></returns>
        CanStandardData ScmMcu4ToBytes(Scm_MCU4 scmMcu4)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = 0x18FFEEEF;
            scmCanByte.baudrate = 250;
            scmCanByte.datas = new byte[8];

            scmCanByte.datas[0] = Convert.ToByte(scmMcu4.mtclDcSideInputPower & 0xff);
            scmCanByte.datas[1] = Convert.ToByte((scmMcu4.mtclDcSideInputPower >> 8) & 0xff);
            //直流侧功率，1Kw/bit，偏移量0，范围0-400Kw

            scmCanByte.datas[2] = Convert.ToByte(Convert.ToUInt16(scmMcu4.mtclTotalElecConsumpt / 0.1) & 0xff);
            scmCanByte.datas[3] = Convert.ToByte(Convert.ToUInt16(scmMcu4.mtclTotalElecConsumpt / 0.1) >> 8 & 0xff);

            return scmCanByte;
        }

        /// <summary>
        /// 电机控制器5 电机故障
        /// ID:0x18FFEFEF
        /// </summary>
        /// <param name="scmCanByte5"></param>
        /// <returns></returns>
        CanStandardData ScmMcu5ToBytes(Scm_MCU5 scmMcu5)
        {
            CanStandardData scmCanByte = new CanStandardData();
            scmCanByte.canId = 0x18FFEFEF;
            scmCanByte.baudrate = 250;
            scmCanByte.datas = new byte[8];

            //byte1
            scmCanByte.datas[0] = Convert.ToByte(scmMcu5.motorFaultCode4 << 4 + scmMcu5.motorFaultCode3);
            //电机故障4，代码0-9，bit8/7/6/5
            //电机故障3，代码0-9，bit4/3/2/1

            //byte2
            scmCanByte.datas[1] = Convert.ToByte(scmMcu5.motorFaultCode2 << 4 + scmMcu5.motorFaultCode1);
            //电机故障2，代码0-9，bit8/7/6/5
            //电机故障1，代码0-9，bit4/3/2/1

            //byte8/7/6/5/4/3保留

            return scmCanByte;
        }

        /// <summary>
        /// 电机控制器程序版本
        /// ID:0x18FF24EF
        /// </summary>
        /// <param name="scmMcuVer"></param>
        /// <returns></returns>
        CanStandardData ScmMcuVerToBytes(Scm_MCU_Version scmMcuVer)
        {
            CanStandardData scmCanByte = new CanStandardData();

            scmCanByte.canId = 0x18FF24EF;
            scmCanByte.baudrate = 250;
            scmCanByte.datas = new byte[8];

            scmCanByte.datas[0] = scmMcuVer.year;
            scmCanByte.datas[1] = scmMcuVer.month;
            scmCanByte.datas[2] = scmMcuVer.day;
            scmCanByte.datas[3] = scmMcuVer.hour;
            scmCanByte.datas[4] = scmMcuVer.minute;
            scmCanByte.datas[6] = Convert.ToByte(scmMcuVer.version & 0xff);
            scmCanByte.datas[7] = Convert.ToByte((scmMcuVer.version >> 8) & 0xff);
            return scmCanByte;
        }
        #endregion

        #endregion


        #region 作为VCU
        #region 发送
        public override AbstractMotorControl.ScmCanSendMsg EcuClearMcuFault()
        {
            ScmCanSendMsg scmSendMsg = new ScmCanSendMsg();

            Scm_EVCU2 scmEvcu2 = new Scm_EVCU2();
            scmEvcu2.askSpeed = 0;
            scmEvcu2.askTorque = 0;
            scmEvcu2.clearFaults = 0; //清故障
            scmEvcu2.enMcu = 0;
            scmEvcu2.keyStart = 1;
            scmEvcu2.mNegContactorStateFeedbk = 1;
            scmEvcu2.mPosContactorStateFeedbk = 1;
            scmEvcu2.mtclMode = 0;
            scmEvcu2.preChrCtSFeedBk = 0;

            CanStandardData canData = ScmEvcu2ToBytes(scmEvcu2);

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

            
            AbstractMotorControl.Ecm_Gear gear; //0空挡，1前进，2后退
            if (data > 0)
                gear = AbstractMotorControl.Ecm_Gear.DriveGear;
            else if (data < 0)
                gear = AbstractMotorControl.Ecm_Gear.ReverseGear;
            else
                gear = AbstractMotorControl.Ecm_Gear.NeutralGear;

            //档位
            ScmCanSendMsg canSendData1 = TransformGear(canIndex, gear);
            canSendDatas.Add(canSendData1);

            //转速转矩
            ScmCanSendMsg canSendData2 = TransformSpeedTorque(canIndex, cmdType, data);
            canSendDatas.Add(canSendData2);

            ////转速限制
            //ScmCanSendMsg canSendData3 = TransformSpeedLimit(canIndex, 0, 5000);
            //canSendDatas.Add(canSendData3);

            ////故障发送
            //ScmCanSendMsg canSendData4 = TransformErrorInfo(canIndex);
            //canSendDatas.Add(canSendData4);

            return canSendDatas;
        }

        /// <summary>
        /// 发送方向到电控，0空挡，1前进，2后退，3错误
        /// </summary>
        protected ScmCanSendMsg TransformGear(uint canIndex, Ecm_Gear gear)
        {
            ScmCanSendMsg canSendData = new ScmCanSendMsg();

            Scm_EVCU1 scmEvcu1 = new Scm_EVCU1();

            string strCmdDetail = string.Empty; //命令解析字符串信息

            canSendData.FeedBkGear = gear;

            switch (gear)
            {
                case Ecm_Gear.NeutralGear:
                    scmEvcu1.gear = 0x00;
                    strCmdDetail = "空挡" + "\r\n";
                    break;

                case Ecm_Gear.DriveGear:
                    scmEvcu1.gear = 0x01;
                    strCmdDetail = "D档" + "\r\n";
                    break;

                case Ecm_Gear.ReverseGear:
                    scmEvcu1.gear = 0x02;
                    strCmdDetail = "倒档" + "\r\n";
                    break;

                case Ecm_Gear.ErrorGear:
                    scmEvcu1.gear = 0x03;
                    strCmdDetail = "错误" + "\r\n";
                    break;

                default: scmEvcu1.gear = 0x00; break;
            }
            
            CanStandardData canData = ScmEvcu1ToBytes(scmEvcu1);
            canSendData.canId = canData.canId;
            canSendData.baudrate = canData.baudrate;
            canSendData.datas = canData.datas;

            //if (!SendNormalBytes(ref canIndex, canData.canId, ref canData.datas))
            //    return null;

            canSendData.canIndex = canIndex;

            canSendData.cmdDetail = strCmdDetail;

            return canSendData;
        }

        /// <summary>
        /// 转换转速转矩
        /// </summary>
        protected ScmCanSendMsg TransformSpeedTorque(uint canIndex, Ecm_WorkMode cmdType, short data)
        {
            Scm_EVCU2 scmEvcu2 = new Scm_EVCU2();
            string strCmdDetail = string.Empty; //命令解析字符串信息

            ScmCanSendMsg canSendData = new ScmCanSendMsg();
            canSendData.FeedBkMtclMode = cmdType;

            //电机转速模式设定
            scmEvcu2.clearFaults = 0; //清故障
            scmEvcu2.enMcu = 0; //mcu使能
            scmEvcu2.keyStart = 1;

            //主接触器是否闭合
            scmEvcu2.mNegContactorStateFeedbk = 1;
            scmEvcu2.mPosContactorStateFeedbk = 1;
            scmEvcu2.preChrCtSFeedBk = 0;
            

            switch (cmdType)
            {
                case Ecm_WorkMode.None: //备用
                    scmEvcu2.mtclMode = 0x00; //控制模式
                    strCmdDetail += "备用" + "\r\n";
                    break;

                case Ecm_WorkMode.SpeedMode: //转速模式
                    {
                        canSendData.FeedBkMotorSpeed = scmEvcu2.askSpeed = data;
                        scmEvcu2.enMcu = 1; //mcu使能
                        scmEvcu2.mtclMode = 0x01; //控制模式
                        strCmdDetail += "转速命令:" + data.ToString() + "\r\n";
                        break;
                    }

                case Ecm_WorkMode.TorqueMode: //转矩模式
                    {
                        canSendData.FeedBkMotorTorque = scmEvcu2.askTorque = data;
                        scmEvcu2.enMcu = 1; //mcu使能
                        scmEvcu2.mtclMode = 0x02; //控制模式
                        strCmdDetail += "扭矩命令:" + data.ToString() + "\r\n";
                        break;
                    }

                case Ecm_WorkMode.DisCharging: //放电模式
                    scmEvcu2.enMcu = 1; //mcu使能
                    scmEvcu2.mtclMode = 0x03; //控制模式
                    strCmdDetail += "放电" + "\r\n";
                    break;

                default: scmEvcu2.mtclMode = 0x00; break;
            }
            CanStandardData canData = ScmEvcu2ToBytes(scmEvcu2);

            canSendData.canId = canData.canId;
            canSendData.baudrate = canData.baudrate;
            canSendData.datas = canData.datas;

            canSendData.canIndex = canIndex;
            canSendData.cmdDetail = strCmdDetail;

            return canSendData;
        }

        /// <summary>
        /// 转速限制
        /// </summary>
        /// <param name="downSpeed"></param>
        /// <param name="upSpeed"></param>
        /// <returns></returns>
        protected ScmCanSendMsg TransformSpeedLimit(uint canIndex, ushort downSpeed, ushort upSpeed)
        {
            ScmCanSendMsg canSendData = new ScmCanSendMsg();

            Scm_EVCU3 scmEvcu3 = new Scm_EVCU3();

            //电机转速限制
            canSendData.FeedBkMotorSpeedDownLimit = scmEvcu3.speedDownLimit = downSpeed;
            canSendData.FeedBkMotorSpeedUpLimit = scmEvcu3.speedUpLimit = upSpeed; //默认转速限制12000
            
            CanStandardData canData = ScmEvcu3ToBytes(scmEvcu3);
            canSendData.canId = canData.canId;
            canSendData.baudrate = canData.baudrate;
            canSendData.datas = canData.datas;

            canSendData.canIndex = canIndex;

            return canSendData;
        }

        /// <summary>
        /// 故障问题转换
        /// </summary>
        protected ScmCanSendMsg TransformErrorInfo(uint canIndex)
        {
            ScmCanSendMsg canSendData = new ScmCanSendMsg();

            Scm_EVCU4 scmEvcu4 = new Scm_EVCU4();
            scmEvcu4.airPumpDcACNodeOffLine = 0;
            scmEvcu4.airPumpDcACThirdFault = 0;
            scmEvcu4.backWareDoorLock = 0;
            scmEvcu4.batteryLowerAlarm = 0;
            scmEvcu4.bmsNodeOffLine = 0;
            scmEvcu4.bmsSysFault = 0;
            scmEvcu4.cancelFeedBackCharge = 0;
            scmEvcu4.chargePlugConnectLock = 0;
            scmEvcu4.chargeWareDoorLock = 0;
            scmEvcu4.dashboardNodeOffLine = 0;
            scmEvcu4.dashboardSysFault = 0;
            scmEvcu4.dcDcNodeOffLine = 0;
            scmEvcu4.dcDcThirdFault = 0;
            scmEvcu4.forceHVDiconnect = 0;
            scmEvcu4.forceLimitPower = 0;
            scmEvcu4.forcePark = 0;
            scmEvcu4.insulationDetecterOffLine = 0;
            scmEvcu4.insulationFault = 0;
            scmEvcu4.lowGapPressLock = 0;
            scmEvcu4.lowGasPressAlarm = 0;
            scmEvcu4.mcuNodeOffLine = 0;
            scmEvcu4.mtclSysFault = 0;
            scmEvcu4.oilPumpDcACNodeOffLine = 0;
            scmEvcu4.oilPumpDcACThirdFault = 0;

            CanStandardData canData = ScmEvcu4ToBytes(scmEvcu4);
            canSendData.canId = canData.canId;
            canSendData.baudrate = canData.baudrate;
            canSendData.datas = canData.datas;

            canSendData.canIndex = canIndex;

            return canSendData;
        }

        #endregion

        #region 接收
        /// <summary>
        /// 故障代码转换
        /// </summary>
        private string ReceiveErrorTransform(byte code1, byte code2, byte code3, byte code4)
        {
            string errStr = string.Empty;

            if (code1 != 0)
            {
                switch (code1)
                {
                    case 1: errStr = "电机温度传感器故障"; break;
                    default: break;
                }
            }

            if (code2 != 0)
            {
                switch (code2)
                {
                    case 1: errStr = "母线过压报警"; break;
                    case 2: errStr = "母线欠压报警"; break;
                    case 3: errStr = "模块过温报警"; break;
                    case 4: errStr = "电机过温报警"; break;
                    default: break;
                }
            }

            if (code3 != 0)
            {
                switch (code3)
                {
                    case 1: errStr = "过流故障"; break;
                    case 2: errStr = "直流母线过压故障"; break;
                    case 3: errStr = "直流母线欠压故障"; break;
                    case 4: errStr = "直流母线过流故障"; break;
                    case 5: errStr = "过载故障"; break;
                    case 6: errStr = "电机超速故障"; break;
                    case 7: errStr = "模块过温故障"; break;
                    case 8: errStr = "电机过温故障"; break;
                    case 9: errStr = "CAN通讯故障"; break;
                    case 10: errStr = "控制电过压"; break;
                    case 11: errStr = "控制电欠压"; break;
                    default: break;
                }
            }

            if (code4 != 0)
            {
                switch (code4)
                {
                    case 1: errStr = "IPM故障"; break;
                    case 2: errStr = "自检故障"; break;
                    case 3: errStr = "电机堵转故障"; break;
                    default: break;
                }
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
            recMsg.timeStamp = canRecDatas.timeStamp / 10; //时间戳除10为毫秒
            
            
            switch (canRecDatas.canId)
            {
               

                case Scm_MCU1.canId: //驱动信息及超温反馈,mcu1
                    {
                        Scm_MCU1 scmMcu1 = ByteToScmMcu1(canRecDatas);

                        //扭矩
                        recMsg.containFeedBkMotorTorque = true;
                        recMsg.FeedBkMotorTorque = scmMcu1.motorTorqueFeedbk;

                        //档位
                        recMsg.containFeedBkGear = true;
                        if (scmMcu1.neuralGear==0x01)
                            recMsg.FeedBkGear = Ecm_Gear.NeutralGear; //空档
                        if (scmMcu1.driveGear==0x01)
                            recMsg.FeedBkGear = Ecm_Gear.DriveGear; //前进
                        if (scmMcu1.reverseGear==0x01)
                            recMsg.FeedBkGear = Ecm_Gear.ReverseGear; //后退

                        //故障等级
                        recMsg.containFeedBkErrorLevel = true;
                        recMsg.FeedBkErrorLevel = scmMcu1.mtclFaultLevel;
                        
                        break;
                    }

                case Scm_MCU2.canId: //母线电压母线电流,mcu2
                    {
                        Scm_MCU2 scmMcu2 = ByteToScmMcu2(canRecDatas);
                        //电流电压
                        recMsg.containFeedBkDcCurrent = true;
                        recMsg.FeedBkDcCurrent = scmMcu2.mtclMotherCurrent;
                        recMsg.containFeedBkDcVoltage = true;
                        recMsg.FeedBkDcVoltage = scmMcu2.mtclMotherVoltage;

                        break;
                    }

                case 0x0CFFEDEF: //转速及温度，mcu3
                    {
                        Scm_MCU3 scmMcu3 = ByteToScmMcu3(canRecDatas);
                        //电机温度
                        recMsg.containFeedBkMotorWindingTemp = true;
                        recMsg.FeedBkMotorWindingTemp = scmMcu3.motorTemperature;
                        //电控温度
                        recMsg.containFeedBkMtclInvTemp = true;
                        recMsg.FeedBkMtclInvTemp = scmMcu3.mtclTemperature;
                        //速度
                        recMsg.containFeedBkMotorSpeed = true;
                        recMsg.FeedBkMotorSpeed = scmMcu3.motorSpeed;
                        break;
                    }

                case Scm_MCU4.canId: //控制器直流侧电流以及总耗电,mcu4
                    {
                        Scm_MCU4 scmMcu4 = ByteToScmMcu4(canRecDatas);
                        //功率
                        recMsg.containFeedBkActPower = true;
                        recMsg.FeedBkActPower = scmMcu4.mtclDcSideInputPower;

                        break;
                    }

                case Scm_MCU5.canId: //电机故障,mcu5
                    {
                        Scm_MCU5 scmMcu5 = ByteToScmMcu5(canRecDatas);

                        //错误码
                        recMsg.containFeedBkErrorCode = true;
                        byte[] errorCode = new byte[2] { (byte)(scmMcu5.motorFaultCode4 * 0x0f + scmMcu5.motorFaultCode3), (byte)(scmMcu5.motorFaultCode2 * 0x0f + scmMcu5.motorFaultCode1) };
                        recMsg.FeedBkErrorCode = errorCode;

                        //故障说明
                        recMsg.containFeedBkErrorStr = true;
                        recMsg.FeedBkErrorStr = ReceiveErrorTransform(scmMcu5.motorFaultCode1, scmMcu5.motorFaultCode2, scmMcu5.motorFaultCode3, scmMcu5.motorFaultCode4);

                        break;
                    }

                case Scm_MCU_Version.canId:  //程序版本暂不需要
                    {
                        Scm_MCU_Version scmVersion = ByteToScmMcuVersion(canRecDatas);
                        recMsg.containFeekBkMtclVersion = true;
                        recMsg.FeekBkMtclVersion = string.Format("{0}-{1}-{2} {3}:{4},V{5}",
                            scmVersion.year.ToString(), scmVersion.month.ToString(),
                            scmVersion.day.ToString(),scmVersion.hour.ToString(), scmVersion.minute.ToString(),
                            scmVersion.version.ToString());
                    }
                    break;

                default:
                    //continue;  //其他ID的数据一律不接收
                    break;
            }

            return recMsg; //推送数据
        }
        #endregion

        #endregion

        #region MCU段发送接收
        /// <summary>
        /// 发送回馈给上位机
        /// 00备用，01转速，02转矩，03放电
        /// </summary>
        /// <param name="scmSendData"></param>
        /// <returns></returns>
        public override List<ScmCanSendMsg> TransformMcuSendData(uint canIndex, AbstractMotorControl.ScmCanSendMsg sendMsg, uint motorIndex = 0)
        {
            List<ScmCanSendMsg> results = new List<ScmCanSendMsg>();

            //档位，工作模式，扭矩
            Scm_MCU1 scmMcu1 = new Scm_MCU1();
            scmMcu1.accEffect = 1;
            scmMcu1.brakeEffect = 1;
            scmMcu1.preChargeCompleted = 1;

            scmMcu1.neuralGear = (byte)(sendMsg.FeedBkGear == Ecm_Gear.NeutralGear ? 0x01 : 0x00); //空挡
            scmMcu1.driveGear = (byte)(sendMsg.FeedBkGear == Ecm_Gear.DriveGear ? 0x01 : 0x00); //前进挡
            scmMcu1.reverseGear = (byte)(sendMsg.FeedBkGear == Ecm_Gear.ReverseGear ? 0x01 : 0x00); //后退挡

            scmMcu1.motorTorqueFeedbk = sendMsg.FeedBkMotorTorque; //反馈扭矩
            scmMcu1.mtclFaultLevel = sendMsg.FeedBkErrorLevel;


            CanStandardData canData1 = ScmMcu1ToBytes(scmMcu1);

            ScmCanSendMsg result1 = new ScmCanSendMsg();
            result1.canId = Scm_MCU1.canId;
            result1.canIndex = canIndex;
            result1.datas = canData1.datas;
            result1.cmdDetail = "方向:" + (scmMcu1.neuralGear==1 ? "空挡" : "") + (scmMcu1.driveGear==1 ? "前进" : "") + (scmMcu1.reverseGear==1 ? "后退" : "") + "\r\n" +
                "扭矩:" + scmMcu1.motorTorqueFeedbk.ToString() + "故障等级:" + scmMcu1.mtclFaultLevel.ToString();
            results.Add(result1);


            //母线电流电压
            Scm_MCU2 scmMcu2 = new Scm_MCU2();
            scmMcu2.mtclMotherCurrent = 55;
            scmMcu2.mtclMotherVoltage = 499;

            CanStandardData canData2 = ScmMcu2ToBytes(scmMcu2);
            
            ScmCanSendMsg result2 = new ScmCanSendMsg();
            result2.canId = Scm_MCU2.canId;
            result2.canIndex = canIndex;
            result2.datas = canData2.datas;
            result2.cmdDetail = "母线电压:" + scmMcu2.mtclMotherVoltage.ToString() + "\r\n" + "母线电流:" + scmMcu2.mtclMotherCurrent.ToString();
            results.Add(result2);


            //速度与温度
            Scm_MCU3 scmMcu3 = new Scm_MCU3();
            scmMcu3.motorSpeed = sendMsg.FeedBkMotorSpeed;
            scmMcu3.motorTemperature = 118;
            scmMcu3.mtclTemperature = 76;

            CanStandardData canData3 = ScmMcu3ToBytes(scmMcu3);

            ScmCanSendMsg result3 = new ScmCanSendMsg();
            result3.canId = Scm_MCU3.canId;
            result3.canIndex = canIndex;
            result3.datas = canData3.datas;
            result3.cmdDetail = "速度:" + scmMcu3.motorSpeed.ToString() + "\r\n" + "电机温度:" + scmMcu3.motorTemperature.ToString() + "\r\n" + "电控温度:" + scmMcu3.mtclTemperature.ToString();
            results.Add(result3);


            //功率
            Scm_MCU4 scmMcu4 = new Scm_MCU4();
            scmMcu4.mtclDcSideInputPower = 160;

            CanStandardData canData4 = ScmMcu4ToBytes(scmMcu4);
            
            ScmCanSendMsg result4 = new ScmCanSendMsg();
            result4.canId = Scm_MCU4.canId;
            result4.canIndex = canIndex;
            result4.datas = canData4.datas;
            result4.cmdDetail = "功率:" + scmMcu4.mtclDcSideInputPower.ToString();
            results.Add(result4);
            

            //故障码
            Scm_MCU5 scmMcu5 = new Scm_MCU5();
            scmMcu5.motorFaultCode1 = 0;
            scmMcu5.motorFaultCode2 = 0;
            scmMcu5.motorFaultCode3 = 0;
            scmMcu5.motorFaultCode4 = 0;

            CanStandardData canData5 = ScmMcu5ToBytes(scmMcu5);

            ScmCanSendMsg result5 = new ScmCanSendMsg();
            result5.canId = Scm_MCU5.canId;
            result5.canIndex = canIndex;
            result5.datas = canData5.datas;
            result5.cmdDetail = "故障码:" + scmMcu5.motorFaultCode4.ToString("X2") + " " + scmMcu5.motorFaultCode3.ToString("X2") + " " + scmMcu5.motorFaultCode2.ToString("X2") + " " + scmMcu5.motorFaultCode1.ToString("X2");
            results.Add(result5);
            

            //程序版本
            Scm_MCU_Version scmVersion = new Scm_MCU_Version();
            scmVersion.year = 18;
            scmVersion.month = 03;
            scmVersion.day = 12;
            scmVersion.hour = 15;
            scmVersion.minute = 23;
            scmVersion.version = 334;

            CanStandardData canDataVersion = ScmMcuVerToBytes(scmVersion);
            
            ScmCanSendMsg resultVer = new ScmCanSendMsg();
            resultVer.canId = Scm_MCU_Version.canId;
            resultVer.canIndex = canIndex;
            resultVer.datas = canDataVersion.datas;
            resultVer.cmdDetail = "软件版本";
            results.Add(resultVer);
            

            return results;
        }


        /// <summary>
        /// 接收数据并处理
        /// 0x1000EFD0  //整车控制器1 驱动信息
        /// 0x0CFA00D0  //整车控制器2 故障检查及MCU使能、电机控制模式、扭矩转速设定
        /// 0x0CF101D0  //整车控制器3 转速上下限
        /// 0x0CFB00D0  //整车控制器4 故障汇报及强制限制、强制停车、强制断电
        /// </summary>
        /// <param name="canRecDatas"></param>
        public override ScmCanReceiveMsg TransformMcuReceiveData(CanStandardData canRecDatas)
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
            recMsg.timeStamp = canRecDatas.timeStamp / 10; //时间戳除10为毫秒

            switch (canRecDatas.canId)
            {
                case Scm_EVCU1.canId:  //整车控制器1 驱动信息
                    {
                        Scm_EVCU1 scmEvcu1 = BytesToScmEvcu1(canRecDatas);
                        recMsg.containFeedBkGear = true;
                        recMsg.FeedBkGear = (Ecm_Gear)scmEvcu1.gear;
                        break;
                    }

                case Scm_EVCU2.canId:  //整车控制器2 故障检查及MCU使能、电机控制模式、扭矩转速设定
                    {
                        Scm_EVCU2 scmEvcu2 = BytesToScmEvcu2(canRecDatas);
                        recMsg.containFeedBkMtclMode = true;
                        recMsg.FeedBkMtclMode = (Ecm_WorkMode)scmEvcu2.mtclMode; //工作模式
                        switch (recMsg.FeedBkMtclMode)
                        {
                            case Ecm_WorkMode.None: //备用
                                break;
                            case Ecm_WorkMode.SpeedMode: //转速
                                recMsg.containFeedBkMotorSpeed = true;
                                break;
                            case Ecm_WorkMode.TorqueMode: //转矩
                                recMsg.containFeedBkMotorTorque = true;
                                break;
                            default: break;
                        }

                        recMsg.FeedBkMotorSpeed = scmEvcu2.askSpeed; //转速

                        recMsg.FeedBkMotorTorque = scmEvcu2.askTorque; //扭矩

                        break;
                    }

                case Scm_EVCU3.canId:  //整车控制器3 转速上下限
                    {
                        Scm_EVCU3 scmEvcu3 = BytesToScmEvcu3(canRecDatas);
                        //recMsg.FeedBkMotorSpeedDownLimit = scmEvcu3.speedDownLimit;
                        //recMsg.MotorSpeedUpLimit = scmEvcu3.speedUpLimit;
                        break;
                    }

                case Scm_EVCU4.canId:  //整车控制器4 故障汇报及强制限制、强制停车、强制断电
                    {
                        Scm_EVCU4 scmEvcu4 = BytesToScmEvcu4(canRecDatas);
                        break;
                    }

                default:
                    //continue;  //
                    break;
            }

            return recMsg; //推送数据
        }
        #endregion
    }
}
