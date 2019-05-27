using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFunctions.HexParse
{
    /// <summary>
    /// Microchip的MPLAB X生成的HEX解析
    /// MPLAB可以生成3中不同格式的执行文件
    /// 其中两种是HEX文件，它们分别称为
    /// INHX8M(Intel Hex Format) -- 一般用于8位核心设备编程器
    /// INHX32(Intel Hex 32 Format) --  一般用于16位核心设备编程器
    /// 另外一种INHX8S(Intel Split Hex Format)生成的是HXL和HXH文件，分别保存指令数据的低字节和高字节，这里不做说明
    /// 详情参考MPLAX帮助文件
    /// </summary>
    public class MicrochipHexParse
    {



        #region INHX32格式解析，MPLAB内嵌连接器MPLINK在默认情况下生成INHX32，适用于dsPIC33E/PIC24E
        /*************************************************
        * 1、hex文件以ascii形式，按照行来记录数据
        * 2、每一行从:开始，每至少2个字符表示一组16进制数据，格式为 :BBAAAATTHHHH....HHHCC
        *                   BB -- 16进制，表示此行数据长度字节数，表示HH的数目
        *                 AAAA -- 16进制，表示数据记录的起始地址，若此行是数据记录，则表示偏移地址，其它无意义
        *                   TT -- 16进制，表示记录类型，
        *                                   00-数据记录(Data Record); 
        *                                   01-文件记录结束(End of File record); 
        *                                   02-扩展段地址记录(Extend Segment address record);后面所有数据地址+段地址左移4位
        *                                   04-扩展线性地址记录(Extend Linear address record);后面所有数据地址+线性地址左移16位
        *                   HH...HH -- 16进制，低字节/高字节 结合数据，高字节在后；注意，若是偏移地址，则都是2字节，高字节在前，低字节在后
        *                   CC -- 16进制，校验码，除冒号和自身以外的其他字节数据加起来模除256的余数的补码，例如:10A6B0000000EB00D4FD0700000F78001E007800BA，CC=01+~(00+00+EB+00+D4+FD+07+00+00+0F+78+00+1E+00+78+00)=BA
        *
        *
        * 需要特别注意的是,：
        * 1、对于dsPIC33E/PIC24E，Hex文件地址是乘了一个2，例如一个部分存于0x100，而在Hex中为0x200，详见dsPIC33E/PIC24E编程规范(DS70619B)扩展A中的介绍。
        * 2、dsPIC33E/PIC24E没有扩展段地址，只有扩展线性地址
        * 3、MPLAB X项目属性中的建设里，勾选规范会HEX文件和不勾选生成的HEX文件是不一样的
        * 4、数据记录为little-endian，低端在前
        *
        *
        * 一旦出现段地址或者线性地址，之后所有数据都要加偏移地址，直到出现一个新的段地址或者线性地址，再重新变更偏移地址
        * 对于真实地址，是 线性地址左移16位+段地址左移4位+偏移地址
        *
        * 示例：
        * :020000040108EA           线性偏移地址：0108
        * :0200000212FFBD           段偏移地址：12FF
        * :0401000090FFAA5502       数据地址：0100
        * :00000001FF               文件结束
        * 真实地址为：0108左移16位,为01080000;12FF左移4位,为00012FF0;数据地址为00000100;加起来为010930F0
        * 最终解析出来(8位单片机)：
        * 地址     数据
        * 010930F0 90
        * 010930F1 FF
        * 010930F2 AA
        * 010930F3 55
        * 对于dsPIC33E/PIC24E，16为单片机，2个16位组成32(有效的是低24位)地址要除以2，所以真实地址解析如下：
        * 00849878 55AAFF90
        * 0084987A ......
        * 而2个地址组成一个24位的指令字(32位的高8位为0)，低端在前，所以表示地址应该如下：
        * 00849878 00AAFF90
        * 0084987a xxxxxxxx
        *************************************************/

        /// <summary>
        /// 一行数据解析
        /// </summary>
        public struct HexTextOneLine
        {
            /// <summary>
            /// 数据长度
            /// </summary>
            public byte dataLength;

            /// <summary>
            /// 数据地址
            /// </summary>
            public uint addr;

            /// <summary>
            /// 类型
            /// </summary>
            public ENUM_HexDATA_TYPE dataType;

            /// <summary>
            /// 数据
            /// </summary>
            public byte[] data;

            /// <summary>
            /// 校验和
            /// </summary>
            public byte checkSum;

            /// <summary>
            /// 计算所得校验和
            /// </summary>
            public byte realCheckSum;

            /// <summary>
            /// 所有数据转换成字节
            /// </summary>
            public byte[] allBytes;
        }

        /// <summary>
        /// Hex每一个地址的数据，可能由多行组成
        /// 例如包含线性偏移地址，或者段偏移地址
        /// </summary>
        public struct HexParseDataOneAddrStruct
        {
            /// <summary>
            /// 非程序存储
            /// 超过最大程序地址的都是非程序存储
            /// 例如配置字
            /// </summary>
            public bool notProgramFlash;

            /// <summary>
            /// 线性地址偏移
            /// </summary>
            public uint linearOffset;

            /// <summary>
            /// 段地址偏移
            /// 实际Microchip没有段地址，此处备用
            /// </summary>
            public uint segmentOffset;

            /// <summary>
            /// 前面是线性地址偏移
            /// </summary>
            public bool beforeIsLinearOffset;

            /// <summary>
            /// 前面是段地址偏移
            /// </summary>
            public bool beforeIsSegmentOffset;

            /// <summary>
            /// 数据地址
            /// </summary>
            public uint addr;

            /// <summary>
            /// 实际地址
            /// 线性地址偏移，左移16位与addr相加为实际地址
            /// </summary>
            public uint realAddr;

            /// <summary>
            /// 最大程序存储地址
            /// </summary>
            public const uint maxProgramAddr = 0x2AFEA;

            /// <summary>
            /// 数据
            /// </summary>
            public UInt32 data;
            

            /// <summary>
            /// 属于Hex第几行
            /// </summary>
            public int lineNum;

            
            /// <summary>
            /// 检查校验是否正确
            /// </summary>
            public bool checkSumError;

            /// <summary>
            /// 检查长度是否合法
            /// </summary>
            public bool lengthError;
        }


        /// <summary>
        /// 上一个为线性地址
        /// </summary>
        bool m_LastIsLinearOffset = false;

        /// <summary>
        /// 线性地址，已经左移16位
        /// </summary>
        UInt32 m_INHX32_ExtendLinearAddressRecord = 0;

        /// <summary>
        /// 上一个为段地址
        /// </summary>
        bool m_LastIsSegmentOffset = false;

        /// <summary>
        /// 段地址，已经左移4位
        /// </summary>
        UInt32 m_INHX32_ExtendSegmentAddressRecord = 0;


        /// <summary>
        /// 数据类型
        /// </summary>
        public enum ENUM_HexDATA_TYPE : byte
        {
            DATA=0,END=1, ExtendSegmentAddressRecord=2, ExtendLinearAddressRecord=4
        }

        #region 单行字符串与数据之间转换
        /// <summary>
        /// 行字符串转换为结构体
        /// </summary>
        public HexTextOneLine StringToDataStruct(string str)
        {
            HexTextOneLine oneLine = new HexTextOneLine();

            //所有数据的字节集合
            oneLine.allBytes = Converter.MyStringConverter.HexStringToBytes("0x" + str.Substring(1, str.Length - 1));
            
            //标明数据长度
            oneLine.dataLength = Convert.ToByte("0x" + str.Substring(1, 2),16);

            //偏移地址，为显示地址的一半
            oneLine.addr = Converter.MyStringConverter.HexStringToUint("0x" + str.Substring(3, 4))/2;
            
            //实际数据转换成字节，每四个组成一个数据地址，高字节在后
            oneLine.data = Converter.MyStringConverter.HexStringToBytes("0x" + str.Substring(9, (int)(oneLine.dataLength * 2)));

            //数据类型
            oneLine.dataType = (ENUM_HexDATA_TYPE)Convert.ToInt32(str.Substring(7, 2));

            //校验和
            oneLine.checkSum = oneLine.allBytes[oneLine.allBytes.Length - 1]; 

            //实际计算所得校验和
            oneLine.realCheckSum = 0x00;
            for (int i = 0; i < oneLine.allBytes.Length - 1; i++)
                oneLine.realCheckSum += oneLine.allBytes[i];
            oneLine.realCheckSum = (byte)(0x01 + ~oneLine.realCheckSum);

            return oneLine;
        }

        /// <summary>
        /// 将一行数据转换成string
        /// </summary>
        public string DataStructToString(HexTextOneLine oneLine)
        {
            string str = string.Empty;
            //显示地址是实际地址的2倍
            str = ":" ;
            //数据低位在前
            for (int i = 0; i<oneLine.allBytes.Length; i++)
                str += oneLine.allBytes[i].ToString("X2");
            return str;
        }

        /// <summary>
        /// 将偏移地址转换为字节
        /// 适用于线性地址和段地址
        /// offsetOffset为自身已经偏移的位数
        /// </summary>
        private byte[] GetOffset(uint offset,byte offsetOffset)
        {
            byte[] dataTmp = new byte[7];

            dataTmp[0] = 2; //数据长度
            dataTmp[1] = 0;
            dataTmp[2] = 0; //地址
            dataTmp[3] = 4; //数据类型，线性偏移地址
            //偏移地址，高字节在前
            dataTmp[6] = Convert.ToByte((offset >> offsetOffset) & 0xff); //本身已经偏移了16位，低位
            dataTmp[5] = Convert.ToByte((offset >> (offsetOffset+8)) & 0xff); //本身已经偏移了16位，再偏移8位，总共偏移24位，高位
                                                                                 //实际计算所得校验和
            byte checkSum = 0x00;
            for (int i = 0; i < dataTmp.Length - 1; i++)
                checkSum += dataTmp[i];
            checkSum = (byte)(0x01 + ~checkSum);

            dataTmp[7] = checkSum;

            return dataTmp;
        }

        /// <summary>
        /// 根据线性地址获取对应的数据
        /// linearOffset为左偏移16位之后的数据
        /// </summary>
        public byte[] GetLinearOffset(uint linearOffset)
        {
            return GetOffset(linearOffset, 16); //本身偏移了16位
        }

        /// <summary>
        /// 根据段地址获取对应的数据
        /// segmentOffset为左偏移4位之后的数据
        /// </summary>
        public byte[] GetSegmentOffset(uint segmentOffset)
        {
            return GetOffset(segmentOffset, 4); //本身偏移了4位
        }
        #endregion

        #region 字节数组与Hex字符串之间转换
        /// <summary>
        /// 字符串数组转换成结构体数组
        /// </summary>
        public List<HexTextOneLine> HexStringToBytes(ref string[] allLines)
        {
            List<HexTextOneLine> allLineBytes = new List<HexTextOneLine>();
            for (int i = 0; i < allLines.Length; i++)
            {
                allLineBytes.Add(StringToDataStruct(allLines[i]));
            }
            return allLineBytes;
        }
        #endregion

        #region 地址与字符串之间转换

        /// <summary>
        /// 解析某一行字符串
        /// </summary>
        public List<HexParseDataOneAddrStruct> ParseOnRow(string str, int lineNum)
        {
            List<HexParseDataOneAddrStruct> oneRowData = new List<HexParseDataOneAddrStruct>();

            //:10 0030 00 60030000 24030000 60030000 60030000 70
            //:02 0000 04 0005 F5
            //:00 0000 01 FF

            HexTextOneLine oneLine = StringToDataStruct(str);

            //实际数据长度的2倍，之所以不用除法，避免出现奇数导致问题
            int doubleRealDataLength = (str.Length - 3 - 4 - 2 - 2);

            
            #region 有异常
            //数据长度不一致
            if (oneLine.dataLength * 2 != doubleRealDataLength)
            {
                HexParseDataOneAddrStruct dataOne = new HexParseDataOneAddrStruct();
                dataOne.lineNum = lineNum;
                dataOne.lengthError = true;
                oneRowData.Add(dataOne);
                return oneRowData;
            }

            //校验不通过
            if (oneLine.checkSum != oneLine.realCheckSum)
            {
                HexParseDataOneAddrStruct dataOne = new HexParseDataOneAddrStruct();
                dataOne.lineNum = lineNum;
                dataOne.checkSumError = true;
                oneRowData.Add(dataOne);
                return oneRowData;
            }
            #endregion


            switch (oneLine.dataType)
            {
                case ENUM_HexDATA_TYPE.DATA:
                    {
                        //实际计算所得地址，每一行最多记录16bytes，4组数据;每组占2个地址，4bytes，实际地址要除以2
                        UInt32 realAddr = (m_INHX32_ExtendLinearAddressRecord + m_INHX32_ExtendSegmentAddressRecord + oneLine.addr);

                        //数据低位在前，与其他不同
                        for (int i = 0; i <= oneLine.data.Length - 4; i += 4)
                        {
                            HexParseDataOneAddrStruct oneAddr = new HexParseDataOneAddrStruct();
                            uint tmp = (uint)oneLine.data[i] + (uint)(oneLine.data[i + 1] << 8) + (uint)(oneLine.data[i + 2] << 16) + (uint)(oneLine.data[i + 3] << 24);
                            oneAddr.data = tmp;
                            
                            //线性地址和段地址
                            oneAddr.linearOffset = m_INHX32_ExtendLinearAddressRecord;
                            oneAddr.beforeIsLinearOffset = m_LastIsLinearOffset;
                            oneAddr.segmentOffset = m_INHX32_ExtendSegmentAddressRecord;
                            oneAddr.beforeIsSegmentOffset = m_LastIsSegmentOffset;
                            m_LastIsLinearOffset = m_LastIsSegmentOffset = false; //清空，下次就知道这次不是线性或者段地址

                            //地址
                            oneAddr.addr = Convert.ToUInt32(oneLine.addr + i/2);

                            //实际地址由线性地址和段地址以及地址组成
                            oneAddr.realAddr = Convert.ToUInt32(oneAddr.linearOffset + oneAddr.segmentOffset + oneAddr.addr); //16位单片机，每4个字节地址增长一半

                            //dsPIC33E最大程序地址为0x2AFEA，超过地址可能为配置字或者其它
                            if (oneAddr.realAddr > HexParseDataOneAddrStruct.maxProgramAddr)
                                oneAddr.notProgramFlash = true; //非程序存储

                            oneAddr.lineNum = lineNum;
                            oneRowData.Add(oneAddr);
                        }
                    }
                    break;
                case ENUM_HexDATA_TYPE.END:
                    m_LastIsLinearOffset = m_LastIsSegmentOffset = false; //清空，下次就知道这次不是线性或者段地址
                    break;
                case ENUM_HexDATA_TYPE.ExtendSegmentAddressRecord: //段地址，左移4位
                    {
                        UInt32 segAddr = 0;
                        segAddr = Convert.ToUInt32((oneLine.data[0]<<8)+ oneLine.data[1]); //地址高位在前
                        m_INHX32_ExtendSegmentAddressRecord = (segAddr << 4) / 2;//显示地址为实际地址的2倍

                        m_LastIsSegmentOffset = true; //下次即知此次为段地址
                    }
                    break;
                case ENUM_HexDATA_TYPE.ExtendLinearAddressRecord: //线性地址，左移16位
                    {
                        UInt32 lineAddr = 0;
                        lineAddr = Convert.ToUInt32((oneLine.data[0] << 8) + oneLine.data[1]); //地址高位在前
                        m_INHX32_ExtendLinearAddressRecord =  (lineAddr << 16) / 2; //显示地址为实际地址的2倍
                        m_LastIsLinearOffset = true; //下次即知此次为线性地址
                    }
                    break;
            }

            return oneRowData;
        }

        
        #endregion

        #region 将Hex解析成地址数据


        /// <summary>
        /// Microchip的Hex解析
        /// 输入为Hex的所有行数据
        /// </summary>
        public List<HexParseDataOneAddrStruct> GetDataFromMicrochipINHX32(ref string[] hexDatas)
        {
            List<HexParseDataOneAddrStruct> lsHexParseData = new List<HexParseDataOneAddrStruct>();

            //地址归0
            m_INHX32_ExtendLinearAddressRecord = 0;
            m_INHX32_ExtendSegmentAddressRecord = 0;
            m_LastIsLinearOffset = m_LastIsSegmentOffset = false; //清空，下次就知道这次不是线性或者段地址

            //填充flash数据
            for (int i = 0; i < hexDatas.Length; i++)
            {
                lsHexParseData.AddRange(ParseOnRow(hexDatas[i], i));
            }

            return lsHexParseData;
        }

        #endregion

        #region 将地址数据变回Hex
        
        /// <summary>
        /// 解析属于同一行的地址，将其变回hex字符串
        /// 可能包含偏移地址的多行
        /// </summary>
        private List<string> EncryptOnAddr(List<HexParseDataOneAddrStruct> oneRowDatas)
        {
            List<string> strS = new List<string>();

            //前面是否需要插入线性地址偏移
            if (oneRowDatas[0].beforeIsLinearOffset)
            {
                string linearOffset = ":"+BitConverter.ToString(GetLinearOffset(oneRowDatas[0].linearOffset)).Replace("-", "");
                strS.Add(linearOffset);
            }

            //前面是否需要插入线性地址偏移
            if (oneRowDatas[0].beforeIsSegmentOffset)
            {
                string linearSegment = ":" + BitConverter.ToString(GetSegmentOffset(oneRowDatas[0].segmentOffset)).Replace("-", "");
                strS.Add(linearSegment);
            }

            List<byte> bsTmp = new List<byte>();
            bsTmp.Add(Convert.ToByte(4 * oneRowDatas.Count)); //datalength
            //addr
            bsTmp.Add(Convert.ToByte((oneRowDatas[0].addr >> 24) & 0xff));
            bsTmp.Add(Convert.ToByte((oneRowDatas[0].addr >> 16) & 0xff));
            bsTmp.Add(Convert.ToByte((oneRowDatas[0].addr >> 8) & 0xff));
            bsTmp.Add(Convert.ToByte(oneRowDatas[0].addr & 0xff));
            bsTmp.Add(0); //datatype
            for (int i = 0; i < oneRowDatas.Count; i++)
            {
                //每组地址4个bytes数据，低位在前
                bsTmp.Add(Convert.ToByte(oneRowDatas[i].data & 0xff));
                bsTmp.Add(Convert.ToByte((oneRowDatas[i].data >> 8) & 0xff));
                bsTmp.Add(Convert.ToByte((oneRowDatas[i].data >> 16) & 0xff));
                bsTmp.Add(Convert.ToByte((oneRowDatas[i].data >> 24) & 0xff));
            }
            //实际计算所得校验和
            byte checkSum = 0x00;
            for (int i = 0; i < bsTmp.Count - 1; i++)
                checkSum += bsTmp[i];
            checkSum = (byte)(0x01 + ~checkSum);

            bsTmp.Add(checkSum);

            string strData = BitConverter.ToString(bsTmp.ToArray()).Replace("-", "");
            strS.Add(strData);

            return strS;
        }

        /// <summary>
        /// Microchip的Hex解析
        /// 输入为Hex的所有行数据
        /// </summary>
        public List<string> SetDataToMicrochipINHX32(ref List<HexParseDataOneAddrStruct> lsHexParseData)
        {
            List<string> hexString = new List<string>();

            List<List<HexParseDataOneAddrStruct>> listList = new List<List<HexParseDataOneAddrStruct>>();

            int lineNum = 0;
            int startIndex = 0; //本段起始索引
            //填充flash数据
            for (int i = 0; i < lsHexParseData.Count; i++)
            {
                
                if (lineNum < lsHexParseData[i].lineNum) //边界
                {
                    List<HexParseDataOneAddrStruct> list = new List<HexParseDataOneAddrStruct>();
                    //从边界之间开始复制
                    for (int t = startIndex; t < i; t++)
                    {
                        list.Add(lsHexParseData[t]);
                    }

                    listList.Add(list);

                    startIndex = i;
                    lineNum = lsHexParseData[i].lineNum;
                }
            }

            //解析每一组数据
            for (int i = 0; i < listList.Count; i++)
                hexString.AddRange(EncryptOnAddr(listList[i]));

            //结束
            hexString.Add(":00000001FF");
            return hexString;
        }
        
        #endregion

        #endregion

    }
}
