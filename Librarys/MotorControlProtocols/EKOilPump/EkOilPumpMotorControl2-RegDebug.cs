using System;
using System.Collections.Generic;



namespace MotorControlProtocols.EKOilPump
{
    /// <summary>
    /// 油泵控制
    /// </summary>
    public class EkOilPumpMotorControl2_RegDebug
    {
        /********************************
         * Intel格式，低位在前，高位在后
         * ******************************/


        /// <summary>
        /// EVCU1 Config 0~2
        /// ID:0x0C61D503
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:按需发送
        /// </summary>
        public struct ElecOilPump_Scm_EVCU1
        {
            public const uint canId = 0x0C61D5D3;
            /// <summary>
            /// 禁止电控命令，仅用于调试A4964寄存器
            /// byte 0
            /// bit0；1bits，偏移量0
            /// 范围0-1，默认值0，分辨率1
            /// 0不禁止，1禁止
            /// </summary>
            public byte forbidMotorControlCmd;
            
            /// <summary>
            /// A4964寄存器索引
            /// byte1
            /// 16bits，偏移量0
            /// 范围0-255，默认值0，分辨率1
            /// </summary>
            public byte N;

            /// <summary>
            /// A4964寄存器N
            /// byte2
            /// 16bits，偏移量0
            /// 范围0-2047，默认值0，分辨率1
            /// </summary>
            public UInt16 ConfigRegN;

        }

        

        /***************************************
         * 电控接收整车控制器发出的扭矩指令进行输出，电机控制器输出的扭矩应控制在指令范围内
         * 电机控制器具备主动放电功能
         * 电机控制器的预充由整车做
         * 电机控制器连续2s内接收不到整车控制器发送的扭矩命令，电机控制器将输出的扭矩值降为0
         * ************************************/

        /// <summary>
        /// MCU1 Config 29~N
        /// ID:0x0C91D3D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:按需发送
        /// </summary>
        public struct  ElecOilPump_Scm_MCU1_Frame1
        {
            public const uint canId = 0x0C91D3D5;


            /// <summary>
            /// A4964寄存器N的数字，属于返回第几个寄存器数值
            /// byte0
            /// 8bits，偏移量0
            /// 范围0-31，默认值0，分辨率1
            /// </summary>
            public byte N;

            /// <summary>
            /// A4964寄存器N
            /// byte1、2
            /// 16bits，偏移量0
            /// 范围0-2047，默认值0，分辨率1
            /// </summary>
            public UInt16 ConfigRegN;


            /// <summary>
            /// A4964寄存器29
            /// byte3、4
            /// 16bits，偏移量0
            /// 范围0-2047，默认值0，分辨率1
            /// </summary>
            public UInt16 ConfigReg29;
        }



        #region 结构体与数组转换


        /// <summary>
        /// 整车控制器1
        /// ID:0x0C61D5D3
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:按需发送
        /// </summary>
        AbstractMotorControl.CanStandardData ElecOilPump_ScmEvcu1ToBytes(ElecOilPump_Scm_EVCU1 scmEvcu1)
        {
            AbstractMotorControl.CanStandardData scmCanByte = new AbstractMotorControl.CanStandardData();
            scmCanByte.canId = ElecOilPump_Scm_EVCU1.canId;
            scmCanByte.datas = new byte[8];

            //byte0
            scmCanByte.datas[0] = scmEvcu1.forbidMotorControlCmd; //电机命令禁止

            //byte1，寄存器索引N，默认值0，分辨率1
            scmCanByte.datas[1] = (byte)((scmEvcu1.N) & 0xff); //低位

            //byte2，byte3,Config[N]，默认值0，分辨率1
            scmCanByte.datas[2] = (byte)((scmEvcu1.ConfigRegN) & 0xff); //低位
            scmCanByte.datas[3] = (byte)((scmEvcu1.ConfigRegN >> 8) & 0xff); //高位

            //byte4、5、6、7保留

            return scmCanByte;
        }

        /// <summary>
        /// 整车控制器1 数据反解析回结构体
        /// ID:0x0C61D5D3
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:按需发送
        /// </summary>
        public ElecOilPump_Scm_EVCU1 ElecOilPump_ScmBytesToEvcu1(byte[] canData)
        {
            ElecOilPump_Scm_EVCU1 scmEvcu1 = new ElecOilPump_Scm_EVCU1();

            //byte0
            scmEvcu1.forbidMotorControlCmd = (byte)(canData[0]&0x01); //电机命令禁止

            //byte1，寄存器索引N，默认值0，分辨率1
            scmEvcu1.N = canData[1]; //低位

            //byte2，byte3,Config[N]，默认值0，分辨率1
            scmEvcu1.ConfigRegN = (ushort)(canData[3]*256 + canData[2]); //高位

            //byte4、5、6、7保留

            return scmEvcu1;
        }



        /// <summary>
        /// 电机控制器反馈帧1 
        /// ID:0x0C7103D5
        /// 波特率:250Kbps
        /// 数据长度:8bytes
        /// 周期:按需发送
        /// </summary>
        public ElecOilPump_Scm_MCU1_Frame1 ElecOilPump_ByteToScmMcu1Frame1(byte[] scmCanByte1)
        {
            ElecOilPump_Scm_MCU1_Frame1 scmMcu1Frame1 = new ElecOilPump_Scm_MCU1_Frame1();
            
            //byte0，N
            scmMcu1Frame1.N = (byte)(scmCanByte1[0]);

            //byte1,2,Config[N]
            scmMcu1Frame1.ConfigRegN = (ushort)(scmCanByte1[2] * 256 + scmCanByte1[1]);
            
            //byte3,4,Config[29]
            scmMcu1Frame1.ConfigReg29 = (ushort)(scmCanByte1[4] * 256 + scmCanByte1[3]);

            return scmMcu1Frame1;
        }



        #endregion
        

        /// <summary>
        /// 转换发送数据
        /// forbidMotorCmd表示是否禁用汽车帧，采用一问一答模式
        /// 0~28
        /// </summary>
        public List<AbstractMotorControl.CanStandardData> TransformEcuSendData0To26(bool forbidMotorCmd, UInt16[] regList)
        {
            List<AbstractMotorControl.CanStandardData> canSendDatas = new List<AbstractMotorControl.CanStandardData>();

            //config 0~26
            for (byte i = 0; i <= 26; i++)
            {
                ElecOilPump_Scm_EVCU1 scmTcu1 = new ElecOilPump_Scm_EVCU1();
                scmTcu1.forbidMotorControlCmd = Convert.ToByte(forbidMotorCmd);
                scmTcu1.N = i;
                scmTcu1.ConfigRegN = regList[i];
                AbstractMotorControl.CanStandardData canData1 = ElecOilPump_ScmEvcu1ToBytes(scmTcu1);
                canSendDatas.Add(canData1);
            }
            
            

            return canSendDatas;
        }

        /// <summary>
        /// 转换发送数据
        /// 27
        /// </summary>
        public List<AbstractMotorControl.CanStandardData> TransformEcuSendData27(bool forbidMotorCmd, UInt16[] regList)
        {
            List<AbstractMotorControl.CanStandardData> canSendDatas = new List<AbstractMotorControl.CanStandardData>();

            //config 27
            ElecOilPump_Scm_EVCU1 scmTcu1 = new ElecOilPump_Scm_EVCU1();
            scmTcu1.forbidMotorControlCmd = Convert.ToByte(forbidMotorCmd);
            scmTcu1.N = 27;
            scmTcu1.ConfigRegN = regList[27];
            AbstractMotorControl.CanStandardData canData1 = ElecOilPump_ScmEvcu1ToBytes(scmTcu1);
            canSendDatas.Add(canData1);

            return canSendDatas;
        }

        /// <summary>
        /// 转换发送数据
        /// 28
        /// </summary>
        public List<AbstractMotorControl.CanStandardData> TransformEcuSendData28(bool forbidMotorCmd, UInt16[] regList)
        {
            List<AbstractMotorControl.CanStandardData> canSendDatas = new List<AbstractMotorControl.CanStandardData>();

            //config 28
            ElecOilPump_Scm_EVCU1 scmTcu1 = new ElecOilPump_Scm_EVCU1();
            scmTcu1.forbidMotorControlCmd = Convert.ToByte(forbidMotorCmd);
            scmTcu1.N = 28;
            scmTcu1.ConfigRegN = regList[28];
            AbstractMotorControl.CanStandardData canData1 = ElecOilPump_ScmEvcu1ToBytes(scmTcu1);
            canSendDatas.Add(canData1);

            return canSendDatas;
        }

        /// <summary>
        /// 转换发送数据
        /// 29
        /// </summary>
        public List<AbstractMotorControl.CanStandardData> TransformEcuSendData29(bool forbidMotorCmd, UInt16[] regList)
        {
            List<AbstractMotorControl.CanStandardData> canSendDatas = new List<AbstractMotorControl.CanStandardData>();

            //config 29
            ElecOilPump_Scm_EVCU1 scmTcu1 = new ElecOilPump_Scm_EVCU1();
            scmTcu1.forbidMotorControlCmd = Convert.ToByte(forbidMotorCmd);
            scmTcu1.N = 29;
            scmTcu1.ConfigRegN = regList[29];
            AbstractMotorControl.CanStandardData canData1 = ElecOilPump_ScmEvcu1ToBytes(scmTcu1);
            canSendDatas.Add(canData1);

            return canSendDatas;
        }


        /// <summary>
        /// 转换发送数据
        /// 30
        /// </summary>
        public List<AbstractMotorControl.CanStandardData> TransformEcuSendData30(bool forbidMotorCmd, UInt16[] regList)
        {
            List<AbstractMotorControl.CanStandardData> canSendDatas = new List<AbstractMotorControl.CanStandardData>();

            //config 30
            ElecOilPump_Scm_EVCU1 scmTcu1 = new ElecOilPump_Scm_EVCU1();
            scmTcu1.forbidMotorControlCmd = Convert.ToByte(forbidMotorCmd);
            scmTcu1.N = 30;
            scmTcu1.ConfigRegN = regList[30];
            AbstractMotorControl.CanStandardData canData1 = ElecOilPump_ScmEvcu1ToBytes(scmTcu1);
            canSendDatas.Add(canData1);

            return canSendDatas;
        }


        /// <summary>
        /// 接收数据并处理
        /// </summary>
        public List<ElecOilPump_Scm_MCU1_Frame1> TransformEcuReceiveData(AbstractMotorControl.CanStandardData canRecDatas)
        {

            if (canRecDatas.canId != ElecOilPump_Scm_MCU1_Frame1.canId)
                return null;

            List<ElecOilPump_Scm_MCU1_Frame1> recMcus = new List<ElecOilPump_Scm_MCU1_Frame1>();

            ElecOilPump_Scm_MCU1_Frame1 recMcu1 = ElecOilPump_ByteToScmMcu1Frame1(canRecDatas.datas);
            recMcus.Add(recMcu1);

            return recMcus;
        }


    }
}
