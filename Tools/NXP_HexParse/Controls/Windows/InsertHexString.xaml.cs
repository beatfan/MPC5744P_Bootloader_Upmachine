using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NXP_HexParse.Datas;
using NXP_HexParse.Config;
using System.Text.RegularExpressions;
using SpecialFunctions.HexParse;

namespace NXP_HexParse.Controls.Windows
{
    /// <summary>
    /// AdministratorWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class InsertHexString : Window
    {
        MicrochipHexParse m_HexParse = new MicrochipHexParse();

        //临时显示添加行的对应数据
        DataSource.AddrDataListClass m_TmpDataList = new DataSource.AddrDataListClass();

        DataSource.AddrDataListClass m_DataList;
        DataSource.HexStringListClass m_HexList;

        public InsertHexString(Point location, Size size, int insertAfterRow, ref DataSource.AddrDataListClass dataList, ref DataSource.HexStringListClass hexList)
        {
            InitializeComponent();


            this.Left = location.X + (size.Width - this.Width) / 2;
            this.Top = location.Y + (size.Height - this.Height) / 2;

            m_DataList = dataList;
            m_HexList = hexList;

            cbDataLength.ItemsSource = new string[] {"4","8","12","16" };
            cbDataLength.SelectedIndex = 3;

            cbDataType.ItemsSource = new string[] { "数据记录(0x00)", "文件记录结束(0x01)", "线性地址(0x04)" };
            cbDataType.SelectedIndex = 0;

            tbInsertAfterRow.CurrentValue = insertAfterRow;

            
            m_TmpDataList.AddrDataCollection = new System.Collections.ObjectModel.ObservableCollection<DataSource.AddrData>();
            dgMsg1.ItemsSource = m_TmpDataList.AddrDataCollection;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            uint lastOffset = SpecialFunctions.Converter.MyStringConverter.HexStringToUint("0x" + m_HexList.HexStringCollection[tbInsertAfterRow.CurrentValue].Text.Substring(3, 4));
            uint dataLength = Convert.ToByte(m_HexList.HexStringCollection[tbInsertAfterRow.CurrentValue].Text.Substring(1, 2),16);
            //每个指令字2个字节，共占用指令字个数为dataLength/2，但每个指令字地址占2个，并且显示数据为2倍，所以实际为dataLength/2/2*2*2
            uint offset = dataLength / 2 * 2 ;

            for (int i = 0; i < m_DataList.AddrDataCollection.Count; i++)
            {
                if (m_DataList.AddrDataCollection[i].LineNum == tbInsertAfterRow.CurrentValue)
                {
                    //实际偏移已经左移16位，将其恢复源数据显示
                    tbLinerOffset.CurrentValue = m_DataList.AddrDataCollection[i].LinearOffset*2>>16;

                    //判断下一个显示地址是否超过0xffff
                    if ((m_DataList.AddrDataCollection[i].LinearOffset*2 & 0xffff) + lastOffset  > (0xffff - offset))
                        tbLinerOffset.CurrentValue++;

                    tbOffset.CurrentValue = (m_DataList.AddrDataCollection[i].LinearOffset & 0xffff) + lastOffset + offset; 
                    ShowRealData();
                    break;
                }
            }

            //显示实际地址，实际地址为一半
            tbReadAddr.CurrentValue = ((tbLinerOffset.CurrentValue << 16) + tbOffset.CurrentValue) / 2;

        }


        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            //插入
            DataSource.HexString hexString1 = new DataSource.HexString();
            hexString1.No = tbInsertAfterRow.CurrentValue + 1;
            hexString1.Text = tbShowLineData.Text.Replace(" ","");
            uint add = Convert.ToUInt32((hexString1.Text.Length - 11) / 2 / 2);
            //在后面插入
            m_HexList.HexStringCollection.Insert(tbInsertAfterRow.CurrentValue+1, hexString1);

            for (int i = tbInsertAfterRow.CurrentValue + 2; i < m_HexList.HexStringCollection.Count; i++)
            {
                m_HexList.HexStringCollection[i].No++;
            }
            

            DialogResult = true;
            //this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region 数据变化，重新生成hex数据
        private void TextBoxHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            //显示实际地址，实际地址为一半
            tbReadAddr.CurrentValue = ((tbLinerOffset.CurrentValue << 16) + tbOffset.CurrentValue)/2;
            ShowRealData();
        }

        private void TextBoxInt_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShowRealData();
        }

        private void cbDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowRealData();
        }

        private void cbDataLength_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowRealData();
        }

        //显示实际数据
        private void ShowRealData()
        {
            if (!IsLoaded)
                return;

            int insertAfter = tbInsertAfterRow.CurrentValue;  //在此行之后插入
            int dataLength = (cbDataLength.SelectedIndex+1)*4; //数据长度
            uint linearOffset = tbLinerOffset.CurrentValue;  //线性地址
            uint offset = tbOffset.CurrentValue; //偏移地址
            uint dataType = cbDataType.SelectedIndex == 2 ? 4 : (uint)cbDataType.SelectedIndex; //数据类型

            //数据
            byte[] data = tbData.CurrentValueBytes;

            
            switch (dataType)
            {
                case 0: //数据
                    if (data == null)
                    {
                        lbDataLengthMsg.Content = "";
                        btnConfirm.IsEnabled = false;
                        return;
                    }
                    if (tbData.Text.Replace(" ","").Length != dataLength*2) //长度不相符
                    {
                        lbDataLengthMsg.Content = "数据长度不一致";
                        btnConfirm.IsEnabled = false;
                    }
                    else
                        DataParse(insertAfter, dataLength, linearOffset, offset, dataType, data);

                    break;
                case 1: //结束
                    FileEndParse();
                    break;
                case 4: //线性地址
                    if (data == null)
                    {
                        lbDataLengthMsg.Content = "";
                        btnConfirm.IsEnabled = false;
                        return;
                    }
                    LinearOffsetParse(data,Convert.ToByte(tbShowLineData.Text.Substring(tbShowLineData.Text.Length-2,2),16));
                    break;
            }
        }

        /// <summary>
        /// 数据记录
        /// </summary>
        private void DataParse(int insertAfter ,int dataLength,uint lineOffset, uint offset,uint dataType,byte[] data)
        {

            lbDataLengthMsg.Content = "";
            byte[] allDataWithoutCheckSum = new byte[4 + dataLength];
            allDataWithoutCheckSum[0] = Convert.ToByte(dataLength);
            allDataWithoutCheckSum[1] = Convert.ToByte((offset >> 8) & 0xff);
            allDataWithoutCheckSum[2] = Convert.ToByte(offset & 0xff);
            allDataWithoutCheckSum[3] = Convert.ToByte(dataType & 0xff);
            for (int i = 4; i < data.Length + 4; i++)
                allDataWithoutCheckSum[i] = data[i - 4];

            //校验和
            byte realCheckSum = 0x00;
            for (int i = 0; i < allDataWithoutCheckSum.Length; i++)
                realCheckSum += allDataWithoutCheckSum[i];
            realCheckSum = (byte)(0x01 + ~realCheckSum);

            string str = string.Empty;

            //普通数据高位在后，只有地址数据才是高位在前
            for (int i = 0; i < data.Length; i++)
            {
                str += data[i].ToString("X2");
            }
            

            //显示总的字符串
            tbShowLineData.Text = ":" + dataLength.ToString("X2") + " " + offset.ToString("X4") + " " + dataType.ToString("X2") + " " 
                + str + " " + realCheckSum.ToString("X2");

            string allText = tbShowLineData.Text.Replace(" ","");

            //解析显示Hex
            #region 解析显示到ListView
            m_TmpDataList.AddrDataCollection.Clear();
            List<MicrochipHexParse.HexParseDataOneAddrStruct> addStructList = m_HexParse.ParseOnRow(allText, insertAfter + 1);

            uint count = 0;
            for (int i = 0; i < addStructList.Count; i++)
            {
                if (addStructList[i].lineNum == insertAfter + 1)
                {
                    DataSource.AddrData addrData = new DataSource.AddrData();
                    addrData.No = m_TmpDataList.AddrDataCollection.Count;
                    addrData.RealAddr = tbReadAddr.CurrentValue + count; //实际地址只有一半
                    count += 2; //每组数据地址加2
                    addrData.Data = addStructList[i].data;
                    addrData.LineNum = addStructList[i].lineNum;
                    if (addStructList[i].checkSumError)
                    {
                        addrData.Remark = "CheckSum Error ";
                        addrData.WarnningColor = System.Windows.Media.Brushes.LightCyan;
                    }
                    if (addStructList[i].lengthError)
                    {
                        addrData.Remark += "Length Error";
                        addrData.WarnningColor = System.Windows.Media.Brushes.LightPink;
                    }

                    m_TmpDataList.AddrDataCollection.Add(addrData);

                }
            }
            #endregion

            btnConfirm.IsEnabled = true;
        }

        /// <summary>
        /// 文件结束
        /// </summary>
        private void FileEndParse()
        {
            //显示总的字符串
            tbShowLineData.Text = ":00 0000 01 FF";
            btnConfirm.IsEnabled = true;
            
        }

        private void LinearOffsetParse(byte[] data,byte checkSum)
        {
            tbShowLineData.Text = ":02 0000 04 "+data[0].ToString("X2")+data[1].ToString("X2")+" "+checkSum;
        }

        #endregion

    }
}
