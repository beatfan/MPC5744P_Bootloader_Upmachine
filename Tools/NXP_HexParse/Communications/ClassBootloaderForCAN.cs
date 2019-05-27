using NXP_HexParse.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NXP_HexParse.Communications
{
    /// <summary>
    /// CAN Bootloader命令解析封包
    /// </summary>
    public class ClassBootloaderForCAN
    {
        /********************************************************************
        命令类型1byte + 数据长度1byte + 数据6bytes
        *********************************************************************/

        
        /// <summary>
        /// Bootloader帧
        /// FA 55 00 00 00 00 00 00 00 00 56 FB
        /// </summary>
        public readonly byte[] g_EntryBootloaderCmd = new byte[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 复位帧
        /// FA 55 01 00 00 00 00 00 00 00 56 FB
        /// </summary>
        public readonly byte[] g_ResetCmd = new byte[8] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        
        /// <summary>
        /// 数据结束
        /// FA 55 03 00 00 00 00 00 00 00 56 FB
        /// </summary>
        public readonly byte[] g_DataEndCmd = new byte[8] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        /// <summary>
        /// 检查Bootloader帧
        /// FA 55 03 00 00 00 00 00 00 00 56 FB
        /// </summary>
        public readonly byte[] g_CheckBootloaderCmd = new byte[8] { 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

        /// <summary>
        /// 擦除Flash帧
        /// FA 55 04 00 00 00 00 00 00 00 56 FB
        /// </summary>
        public readonly byte[] g_EraseFlashCmd = new byte[8] { 0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };



        /// <summary>
        /// 命令类型
        /// </summary>
        public enum CmdType : byte
        {
            EntryBootloader=0,Reset=1,Data=2,DataEnd,CheckBootloader=4,Earse=5,Other=0xff
        }


        /// <summary>
        /// 一帧数据
        /// </summary>
        public struct OneFrame
        {
            /// <summary>
            /// 本数据所在行
            /// </summary>
            public int lineNum;
            /// <summary>
            /// 帧类型
            /// 命令还是数据
            /// </summary>
            public CmdType cmdType;
            /// <summary>
            /// 包含数据长度
            /// </summary>
            public int dataLength;
            /// <summary>
            /// 一帧数据
            /// 最长4字节
            /// </summary>
            public byte[] data;
        }

        /// <summary>
        /// 打包一行串口数据
        /// data一次不能超过4bytes
        /// </summary>
        public byte[] CanDataPackage(OneFrame oneFrame)
        {
            byte[] dataTmp = new byte[8];

            //具体内容
            dataTmp[0] = (byte)oneFrame.cmdType;
            dataTmp[1] = Convert.ToByte((oneFrame.lineNum >> 8) & 0xff); //行号高位在前
            dataTmp[2] = Convert.ToByte(oneFrame.lineNum & 0xff); //行号低位在后
            dataTmp[3] = Convert.ToByte(oneFrame.dataLength);

            //数据长度超过0xA0，表示一行最后一组数据
            int realDataLength = oneFrame.dataLength >= 0xA0 ? oneFrame.dataLength - 0xA0 : oneFrame.dataLength;

            //6 7 8 9 4bytes为实际数据
            for (int i = 0; i < realDataLength; i++)
                dataTmp[4 + i] = oneFrame.data[i];
            
            return dataTmp;
        }


        /// <summary>
        /// 打包一个指令字数据
        /// 地址为实际地址-0x800000，否则地址会超过24位长度
        /// </summary>
        public byte[] CanDataPackage(DataSource.AddrData oneAddrData)
        {
            byte[] dataTmp = new byte[8];
            
            //具体内容
            dataTmp[0] = (byte)CmdType.Data;
            //1-2为地址，高字节在前，1为高位地址,2为低16位的高位地址，3为低16为的低位地址
            dataTmp[1] = Convert.ToByte(((oneAddrData.RealAddr - 0x800000) >> 16) & 0xff); //页号
            dataTmp[2] = Convert.ToByte(((oneAddrData.RealAddr - 0x800000) >> 8) & 0xff); //高8位
            dataTmp[3] = Convert.ToByte((oneAddrData.RealAddr - 0x800000) & 0xff);
            
            //6 7 8 9 4bytes为实际数据
            //高字节在前,低字节在后
            dataTmp[4] = Convert.ToByte(((oneAddrData.Data) >> 24) & 0xff);
            dataTmp[5] = Convert.ToByte(((oneAddrData.Data) >> 16) & 0xff); //页号
            dataTmp[6] = Convert.ToByte(((oneAddrData.Data) >> 8) & 0xff); //高8位
            dataTmp[7] = Convert.ToByte((oneAddrData.Data) & 0xff);

            return dataTmp;
        }

        /// <summary>
        /// 根据地址打包擦除指令
        /// 地址为实际地址-0x800000，否则地址会超过24位长度
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="endAddr"></param>
        /// <returns></returns>
        public byte[] CanErasePackage(uint startAddr,uint endAddr)
        {
            byte[] dataTmp = new byte[8];

            //具体内容
            dataTmp[0] = (byte)CmdType.Earse;
            //1-3为开始地址，高字节在前，1为高位地址,2为低16位的高位地址，3为低16为的低位地址
            dataTmp[1] = Convert.ToByte(((startAddr - 0x800000) >> 16) & 0xff); //页号
            dataTmp[2] = Convert.ToByte(((startAddr - 0x800000) >> 8) & 0xff); //高8位
            dataTmp[3] = Convert.ToByte((startAddr - 0x800000) & 0xff);

            //4-6为结束地址，高字节在前，4为高位地址,5为低16位的高位地址，6为低16为的低位地址
            dataTmp[4] = Convert.ToByte(((endAddr - 0x800000) >> 16) & 0xff); //页号
            dataTmp[5] = Convert.ToByte(((endAddr - 0x800000) >> 8) & 0xff); //高8位
            dataTmp[6] = Convert.ToByte((endAddr - 0x800000) & 0xff);

            return dataTmp;


        }
        /// <summary>
        /// 判断接收数据
        /// 是否确认对方已经正确接收OK
        /// </summary>
        public CmdType ReceiveDataParse(ref byte[] recData)
        {
            if (recData.Length < 3)
                return CmdType.Other;
            return (CmdType)recData[0];
        }

        /// <summary>
        /// 数组比较
        /// </summary>
        private bool ArrayCompare<T>(T[] data1, T[] data2)
        {
            if (data1.Length!=data2.Length)
                return false;

            for (int i = 0; i < data1.Length; i++)
            {
                if (data1[i].ToString() != data2[i].ToString())
                    return false;
            }

            return true;

        }




        #region 用于测试MCU端接收烧录

        public DataSource.AddrDataListClass test_AllAddrData = new DataSource.AddrDataListClass();
        public ClassBootloaderForCAN()
        {
            test_AllAddrData.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();
        }

        void UserFlash_WriteData(uint addr,uint[] data,uint length)
        {
            uint[] dataTmp = new uint[2];
            
            for (int i = 0; i < length; i += 2)
            {
                //数组偶数位为指令字高16位(高8位为虚拟字节，仅低8位有效)，奇数位为指令字低16位
                //每个指令字存入一个实际16位数据，高8位为0
                dataTmp[0] = data[i];
                dataTmp[1] = data[i + 1];
                //每次写入2个指令字，偏移8个地址，每4个地址表示一个指令字
                DataSource.AddrData oneData1 = new DataSource.AddrData();
                oneData1.No = test_AllAddrData.AddrDataCollection.Count;
                oneData1.RealAddr = addr+(uint)i;
                oneData1.Data = dataTmp[i];
                test_AllAddrData.AddrDataCollection.Add(oneData1);

                DataSource.AddrData oneData2 = new DataSource.AddrData();
                oneData2.No = test_AllAddrData.AddrDataCollection.Count;
                oneData2.RealAddr = addr+(uint)(i+8);
                oneData2.Data = dataTmp[i+1];
                test_AllAddrData.AddrDataCollection.Add(oneData2);
            }
        }

        uint m_LastAddr = 0x000004 + 0x800000; //program every 32 address
        uint[] dataForWrite = new uint[2];

        //解析Hex文件
        //0、1表示开头
        //2表示类型
        //3、4、5表示地址，高字节在前，最高字节无
        //6、7、8、9表示数据，低字节在前
        public void UserFlash_DataParseAddrData(byte[] data, byte length)
        {
            uint addr;
            uint dataTmp;

            addr = Convert.ToUInt32(data[1] * 256 * 256 + data[2] * 256 + data[3] + 0x800000);

            //large-endian
            dataTmp = (uint)(data[4] * 256 * 256 * 256 + data[5] * 256 * 256 + data[6] * 256 + data[7]);


            if (addr % 8 == 0) //本次地址是组内第一个数据
            {
                if (m_LastAddr % 8 == 0) //上一次也是第一个，则先填充上一次第二个指令字，写入flash，然后本次重新开始
                {
                    dataForWrite[1] = 0xFFFFFFFF; //上一组第二个指令字填充
                    UserFlash_WriteData(m_LastAddr, dataForWrite, 2); //上一次数据组合后，写入2指令字
                }
                //上一次是第1个或者第2个，本次都要重新开始
                dataForWrite[0] = dataTmp; //本次为第一个指令字

            }
            else //本次地址是第二个
            {
                if (m_LastAddr % 8 == 0) //上一次是第一个，要继续判断是否连续
                {
                    if ((addr - m_LastAddr) == 4) //与上一次连续，填充到上一次的数据中，并写入2个指令字
                    {
                        dataForWrite[1] = dataTmp; //本次为第二个指令字
                        UserFlash_WriteData(m_LastAddr, dataForWrite, 2); //本次次数据组合后，写入2指令字
                    }
                    else //与上一次不连续，填充上一次的第二个指令字，然后填充本次的第一个指令字，同时两次共写入4个指令字
                    {
                        dataForWrite[1] = 0xFFFFFFFF; //上组第二个指令字填充
                        UserFlash_WriteData(m_LastAddr, dataForWrite, 2); //上一次数据组合后，写入2指令字

                        dataForWrite[0] = 0xFFFFFFFF; //填充本组第一个指令字
                        dataForWrite[1] = dataTmp; //本组第二个指令字
                        UserFlash_WriteData(addr-4, dataForWrite, 2); //本次次数据组合后，写入2指令字
                    }
                }
                else//上一次也是第2个，将本次第一个指令字也填充，写入
                {
                    dataForWrite[0] = 0xFFFFFFFF; //填充本组第一个指令字
                    dataForWrite[1] = dataTmp; //本组第二个指令字
                    UserFlash_WriteData(addr-4, dataForWrite, 2); //本次次数据组合后，写入2指令字，注意地址要减2
                }
            }


            m_LastAddr = addr;

        }

        public void UserFlash_EndCheck()
        {
            if (m_LastAddr % 8 == 0) //上一次是第一个，要继续判断是否连续
            {
                dataForWrite[1] = 0xFFFFFFFF; //上组第二个指令字填充
                UserFlash_WriteData(m_LastAddr, dataForWrite, 2); //上一次数据组合后，写入2指令字
            }
        }
        #endregion
    }
}
