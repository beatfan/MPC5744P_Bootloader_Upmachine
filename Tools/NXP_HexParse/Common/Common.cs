using NXP_HexParse.Datas;
using SpecialFunctions.HexParse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NXP_HexParse.Common
{
    /// <summary>
    /// NXP的Hex通用处理方法
    /// </summary>
    public class NXPHexCommonFunctions
    {

        /// <summary>
        /// 传入hex文件所有行数据
        /// hexStringList为Hex字符串直接显示的集合
        /// </summary>
        public DataSource.HexStringListClass ConvertStringToHexString(string[] allLines)
        {
            DataSource.HexStringListClass hexStringList = new DataSource.HexStringListClass();
            hexStringList.HexStringCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.HexString>();

            //文字复制
            for (int i = 0; i < allLines.Length; i++)
                hexStringList.HexStringCollection.Add(new DataSource.HexString { No = i, Text = allLines[i] });

            return hexStringList;
        }

        /// <summary>
        /// 传入hex文件所有行数据
        /// 返回字节数组的列表
        /// </summary>
        public List<byte[]> ConvertStringToHexBytes(ref string[] allLines)
        {
            List<byte[]> allLineDatas = new List<byte[]>();

            NXPHexParse microchipHexParse = new NXPHexParse();
            List<NXPHexParse.HexTextOneLine> lsDataList = microchipHexParse.HexStringToBytes(ref allLines);

            for (int i = 0; i < lsDataList.Count; i++)
            {
                allLineDatas.Add(lsDataList[i].allBytes);
            }

            return allLineDatas;
        }

        /// <summary>
        /// 打包一行地址
        /// </summary>
        private DataSource.AddrData PacakOneAddrData(NXPHexParse.HexParseDataOneAddrStruct oneAddr)
        {
            DataSource.AddrData addrDataTmp = new DataSource.AddrData();
            addrDataTmp.notProgramAddr = oneAddr.notProgramFlash; //非程序存储
            addrDataTmp.No = Convert.ToInt32(oneAddr.realAddr/2);
            addrDataTmp.BeforeIsLinearOffset = oneAddr.beforeIsLinearOffset;
            addrDataTmp.BeforeIsSegmentOffset = oneAddr.beforeIsSegmentOffset;
            addrDataTmp.LinearOffset = oneAddr.linearOffset;
            addrDataTmp.SegmentOffset = oneAddr.segmentOffset;
            addrDataTmp.Addr = oneAddr.addr;
            addrDataTmp.RealAddr = oneAddr.realAddr;
            addrDataTmp.Data = oneAddr.data;
            addrDataTmp.LineNum = oneAddr.lineNum;
            addrDataTmp.Remark = "";
            addrDataTmp.IsInsert = false;
            addrDataTmp.WarnningColor = System.Windows.Media.Brushes.Transparent;
            
            return addrDataTmp;
        }


        /// <summary>
        /// 插入一行地址
        /// 本身没有记录这一行
        /// </summary>
        private DataSource.AddrData InsertOneAddrData(uint realaddr)
        {
            DataSource.AddrData addrDataTmp = new DataSource.AddrData();
            addrDataTmp.notProgramAddr = false;
            addrDataTmp.No = Convert.ToInt32(realaddr/2);
            addrDataTmp.RealAddr = realaddr;
            addrDataTmp.Data = 0x00ffffff;
            addrDataTmp.LineNum = -1; //默认不能为0，否则后面可能会被当做第一行的数据
            addrDataTmp.Remark = "Hex无，插入FFFFFF";
            addrDataTmp.IsInsert = true;
            addrDataTmp.WarnningColor = System.Windows.Media.Brushes.LightGray;

            return addrDataTmp;
        }

        /// <summary>
        /// 传入hex文件所有行数据
        /// dataList为解析后的数据的集合
        /// </summary>
        public DataSource.AddrDataListClass ConvertStringToDataList(string[] allLines)
        {
            DataSource.AddrDataListClass dataList = new DataSource.AddrDataListClass();
            dataList.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();

            NXPHexParse nxpHexParse = new NXPHexParse();

            List<NXPHexParse.HexParseDataOneAddrStruct> lsDataList = nxpHexParse.GetDataFromNxpINHX32(ref allLines);

            for (int i = 0; i < lsDataList.Count; i++)
            {
                string remark = string.Empty;
                System.Windows.Media.Brush warnningColor = System.Windows.Media.Brushes.Transparent;

                if (lsDataList[i].checkSumError)
                {
                    remark = "CheckSum Error ";
                    warnningColor = System.Windows.Media.Brushes.LightCyan;
                }
                if (lsDataList[i].lengthError)
                {
                    remark += "Length Error";
                    warnningColor = System.Windows.Media.Brushes.LightPink;
                }
                //lineNum默认不能为0，否则后面可能出错,-1表示没有这行，为插入值
                dataList.AddrDataCollection.Add(PacakOneAddrData(lsDataList[i]));
            }
            
            return dataList;
        }

        /// <summary>
        /// 传入hex文件所有行数据
        /// dataList为解析后的数据的集合
        /// </summary>
        public DataSource.AddrDataListClass HexStringToDataList(DataSource.HexStringListClass hexStringList)
        {
            DataSource.AddrDataListClass dataList = new DataSource.AddrDataListClass();
            dataList.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();

            string[] allLines = new string[hexStringList.HexStringCollection.Count];
            for (int i = 0; i < allLines.Length; i++)
                allLines[i] = hexStringList.HexStringCollection[i].Text;

            NXPHexParse microchipHexParse = new NXPHexParse();

            List<NXPHexParse.HexParseDataOneAddrStruct> lsDataList = microchipHexParse.GetDataFromNxpINHX32(ref allLines);

            for (int i = 0; i < lsDataList.Count; i++)
            {
                string remark = string.Empty;
                System.Windows.Media.Brush warnningColor = System.Windows.Media.Brushes.Transparent;

                if (lsDataList[i].checkSumError)
                {
                    remark = "CheckSum Error ";
                    warnningColor = System.Windows.Media.Brushes.LightCyan;
                }
                if (lsDataList[i].lengthError)
                {
                    remark += "Length Error";
                    warnningColor = System.Windows.Media.Brushes.LightPink;
                }
                //lineNum默认不能为0，否则后面可能出错,-1表示没有这行，为插入值
                dataList.AddrDataCollection.Add(PacakOneAddrData(lsDataList[i]));
            }

            return dataList;
        }

        /// <summary>
        /// 插入跳过的地址
        /// </summary>
        public DataSource.AddrDataListClass InsertSkipToDataList(ref DataSource.AddrDataListClass dataList)
        {
            DataSource.AddrDataListClass insertDataList = new DataSource.AddrDataListClass();
            insertDataList.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();

            int programFlashEndIndex = 0; //程序存储结束

            for (int i = 0; i < dataList.AddrDataCollection.Count; i++)
            {
                //大于最大地址
                if (dataList.AddrDataCollection[i].notProgramAddr)
                {
                    programFlashEndIndex = i - 1;
                    break;
                }
            }

            //查看程序存储最后一个是否有数据，没有则插入，方便下面判断，插入跳过的地址
            if (dataList.AddrDataCollection[programFlashEndIndex].RealAddr < NXPHexParse.HexParseDataOneAddrStruct.maxProgramAddr)
            {
                //第一个非程序地址，往前插入一个
                dataList.AddrDataCollection.Insert(programFlashEndIndex+1,InsertOneAddrData(NXPHexParse.HexParseDataOneAddrStruct.maxProgramAddr));
                programFlashEndIndex++; //添加了一个，增加1
            }
            

            //逐步增加，填充省略的地址
            UInt32 fillAddr = 0;

            //循环查找是否有空缺的地址，然后增加地址进入，查找范围为程序存储
            for (int i = 0; i < programFlashEndIndex+1; i++)
            {
                //其中有跳过的地址，补充进入
                if (fillAddr < dataList.AddrDataCollection[i].RealAddr)
                {
                    //补充跳过的地址为ffffff
                    //实际地址为i*2，从i*2开始，到包含的地址结束，填充地址
                    //地址仅为偶数，所以步长为2
                    for (uint t = fillAddr; t< dataList.AddrDataCollection[i].RealAddr; t += 2)
                    {
                        insertDataList.AddrDataCollection.Add(InsertOneAddrData(t));
                    }
                }
                insertDataList.AddrDataCollection.Add(dataList.AddrDataCollection[i]);
                fillAddr = dataList.AddrDataCollection[i].RealAddr + 2; //地址每次增长2个，2个16bits

            }

            //插入配置字
            for(int i=programFlashEndIndex+1;i< dataList.AddrDataCollection.Count;i++)
                insertDataList.AddrDataCollection.Add(dataList.AddrDataCollection[i]);

            return insertDataList;
        }

        /// <summary>
        /// 删除插入的数据
        /// </summary>
        public DataSource.AddrDataListClass DeleteInsertToDataList(DataSource.AddrDataListClass dataList)
        {
            DataSource.AddrDataListClass dataList2 = new DataSource.AddrDataListClass();
            dataList2.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();
            
            for (int i = 0; i < dataList.AddrDataCollection.Count; i++)
            {
                if (!dataList.AddrDataCollection[i].IsInsert)
                {
                    dataList2.AddrDataCollection.Add(dataList.AddrDataCollection[i]);
                }
            }
            return dataList2;
        }

        /// <summary>
        /// 数据变化，搜索筛选
        /// dataList为搜索源
        /// searchDataList为搜索结果
        /// </summary>
        public void SearchData(string[] no_addr_data, ref DataSource.AddrDataListClass dataList, ref DataSource.AddrDataListClass searchDataList)
        {
            string no = no_addr_data[0];
            string addr = no_addr_data[1];
            string data = no_addr_data[2];

            searchDataList.AddrDataCollection.Clear();

            for (int i = 0; i < dataList.AddrDataCollection.Count; i++)
            {
                bool checkResult1 = false, checkResult2 = false, checkResult3 = false;
                checkResult1 = string.IsNullOrEmpty(no) ||
                    (!string.IsNullOrEmpty(no) && dataList.AddrDataCollection[i].No.ToString("X6").Contains(no.ToUpper()));
                checkResult2 = string.IsNullOrEmpty(addr) ||
                    (!string.IsNullOrEmpty(addr) && dataList.AddrDataCollection[i].RealAddr.ToString("X6").Contains(addr.ToUpper()));
                checkResult3 = string.IsNullOrEmpty(data) ||
                    (!string.IsNullOrEmpty(data) && dataList.AddrDataCollection[i].Data.ToString("X6").Contains(data.ToUpper()));

                if (checkResult1 && checkResult2 && checkResult3)
                {
                    if (dataList.AddrDataCollection[i].WarnningColor == System.Windows.Media.Brushes.Yellow)
                        dataList.AddrDataCollection[i].WarnningColor = System.Windows.Media.Brushes.Transparent;
                    searchDataList.AddrDataCollection.Add(dataList.AddrDataCollection[i]);
                }
            }
        }

        /// <summary>
        /// AddrDataListClass复制
        /// dataList为源
        /// copyDataList为目标，保存复制结果
        /// </summary>
        public void AddrDataListClassCopy(ref DataSource.AddrDataListClass dataList,ref DataSource.AddrDataListClass copyDataList)
        {
            if (dataList == null)
                copyDataList = null;

            if (copyDataList == null)
                copyDataList = new DataSource.AddrDataListClass();
            else
                copyDataList.AddrDataCollection.Clear();

            for (int i = 0; i < dataList.AddrDataCollection.Count; i++)
            {
                DataSource.AddrData oneData = new DataSource.AddrData();
                oneData.No = dataList.AddrDataCollection[i].No;
                oneData.Addr = dataList.AddrDataCollection[i].Addr;
                oneData.BeforeIsLinearOffset = dataList.AddrDataCollection[i].BeforeIsLinearOffset;
                oneData.BeforeIsSegmentOffset = dataList.AddrDataCollection[i].BeforeIsSegmentOffset;
                oneData.LinearOffset = dataList.AddrDataCollection[i].LinearOffset;
                oneData.SegmentOffset = dataList.AddrDataCollection[i].SegmentOffset;
                oneData.RealAddr = dataList.AddrDataCollection[i].RealAddr;
                oneData.Data = dataList.AddrDataCollection[i].Data;
                oneData.LineNum = dataList.AddrDataCollection[i].LineNum;
                oneData.Remark = dataList.AddrDataCollection[i].Remark;
                oneData.IsInsert = dataList.AddrDataCollection[i].IsInsert;
                oneData.WarnningColor = dataList.AddrDataCollection[i].WarnningColor;
                copyDataList.AddrDataCollection.Add(oneData);
            }
        }
    }
}
