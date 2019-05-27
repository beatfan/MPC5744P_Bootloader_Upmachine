using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NXP_HexParse.Datas
{
    public class DataSource
    {
        #region DataGrid数据源
        /// <summary>
        /// 地址、值显示一行DataGrid
        /// </summary>
        public class AddrData : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            bool _isInsert;
            int _no;
            UInt32 _realAddr;
            UInt32 _data;
            int _lineNum;
            string _remark;
            System.Windows.Media.Brush _warnningColor = System.Windows.Media.Brushes.Transparent;

            /// <summary>
            /// 非程序存储
            /// 超过程序存储最大地址即为非程序存储，例如配置字
            /// </summary>
            public bool notProgramAddr;

            /// <summary>
            /// 线性地址偏移
            /// </summary>
            public uint LinearOffset;

            /// <summary>
            /// 段地址偏移
            /// 实际Microchip没有段地址，此处备用
            /// </summary>
            public uint SegmentOffset;

            /// <summary>
            /// 前面是线性地址偏移
            /// </summary>
            public bool BeforeIsLinearOffset;

            /// <summary>
            /// 前面是段地址偏移
            /// </summary>
            public bool BeforeIsSegmentOffset;

            /// <summary>
            /// 数据地址，不包含段偏移和线偏移
            /// </summary>
            public uint Addr;

            /// <summary>
            /// 最大地址
            /// </summary>
            public const uint MaxAddr = 0x2AFEA;


            /// <summary>
            /// 是否是插入数据
            /// </summary>
            public bool IsInsert
            {
                set
                {
                    _isInsert = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("IsInsert"));
                }
                get
                {
                    return _isInsert;
                }
            }

            /// <summary>
            /// 地址
            /// </summary>
            public int No
            {
                set
                {
                    _no = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("No"));
                }
                get
                {
                    return _no;
                }
            }

            /// <summary>
            /// 实际地址
            /// </summary>
            public UInt32 RealAddr
            {
                set
                {
                    _realAddr = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("RealAddr"));
                }
                get
                {
                    return _realAddr;
                }
            }

            /// <summary>
            /// 数据
            /// </summary>
            public UInt32 Data
            {
                set
                {
                    _data = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Data"));
                }
                get
                {
                    return _data;
                }
            }

            /// <summary>
            /// hex所在行
            /// </summary>
            public int LineNum
            {
                set
                {
                    _lineNum = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("LineNum"));
                }
                get
                {
                    return _lineNum;
                }
            }

            /// <summary>
            /// 警告背景颜色
            /// </summary>
            public System.Windows.Media.Brush WarnningColor
            {
                set
                {
                    _warnningColor = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("WarnningColor"));
                }
                get
                {
                    return _warnningColor;
                }
            }

            /// <summary>
            /// 备注
            /// </summary>
            public string Remark
            {
                set
                {
                    _remark = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Remark"));
                }
                get
                {
                    return _remark;
                }
            }
        }

        /// <summary>
        /// 所有行数据集合
        /// </summary>
        public class AddrDataListClass : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private ObservableCollection<AddrData> _AddrDataCollection;

            /// <summary>
            /// 集合
            /// </summary>
            public ObservableCollection<AddrData> AddrDataCollection
            {
                set
                {
                    if (this._AddrDataCollection != value)
                    {
                        _AddrDataCollection = value;
                        if (PropertyChanged != null)
                            PropertyChanged(this, new PropertyChangedEventArgs("AddrDataCollection"));
                    }
                }
                get
                {
                    return _AddrDataCollection;
                }
            }
        }
        #endregion

        #region Text数据源
        /// <summary>
        /// 显示一行HexString
        /// </summary>
        public class HexString : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            int _no;
            string _text;

            /// <summary>
            /// No
            /// </summary>
            public int No
            {
                set
                {
                    _no = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("No"));
                }
                get
                {
                    return _no;
                }
            }

            /// <summary>
            /// HexString
            /// </summary>
            public string Text
            {
                set
                {
                    _text = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }
                get
                {
                    return _text;
                }
            }
        }

        /// <summary>
        /// 所有行数据集合
        /// </summary>
        public class HexStringListClass : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private ObservableCollection<HexString> _HexStringCollection;

            /// <summary>
            /// 集合
            /// </summary>
            public ObservableCollection<HexString> HexStringCollection
            {
                set
                {
                    if (this._HexStringCollection != value)
                    {
                        _HexStringCollection = value;
                        if (PropertyChanged != null)
                            PropertyChanged(this, new PropertyChangedEventArgs("HexStringCollection"));
                    }
                }
                get
                {
                    return _HexStringCollection;
                }
            }
        }
        #endregion

        #region ListView数据绑定源
        /// <summary>
        /// ListView源行定义，包含属性变化通知
        /// </summary>
        public class LvItemStruct : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            int _index;
            uint _canId;
            byte[] _dataBytes;
            string[] _dataBytesChangeColor;
            string _cmdDetail;
            string _otherInfo;

            public int Index
            {
                set
                {
                    _index = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("Index"));
                }
                get { return _index; }
            }
            public UInt32 ID
            {
                set
                {
                    _canId = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("ID"));
                }
                get { return _canId; }
            }

            public byte[] DataBytes
            {
                set
                {
                    _dataBytes = value; //获取新数据
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("DataBytes"));
                }
                get { return _dataBytes; }
            }


            /// <summary>
            /// 数据有变化
            /// </summary>
            public string[] DataBytesChangeColor
            {
                set
                {
                    _dataBytesChangeColor = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("DataBytesChangeColor"));
                }
                get { return _dataBytesChangeColor; }
            }

            public string CmdDetail
            {
                set
                {
                    _cmdDetail = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("CmdDetail"));
                }
                get { return _cmdDetail; }
            }
            public string OtherInfo
            {
                set
                {
                    _otherInfo = value;
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs("OtherInfo"));
                }
                get { return _otherInfo; }
            }

        }

        /// <summary>
        /// 集合增删变化通知
        /// </summary>
        public class ListViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private ObservableCollection<LvItemStruct> _listViewCollection;
            public ObservableCollection<LvItemStruct> ListViewCollection
            {
                get
                {
                    return this._listViewCollection;
                }
                set
                {
                    if (this._listViewCollection != value)
                    {
                        this._listViewCollection = value;
                        if (PropertyChanged != null)
                            PropertyChanged(this, new PropertyChangedEventArgs("ListViewCollection"));
                    }
                }
            }

        }

        public class ListViewMethods
        {
            /// <summary>
            /// Listview集合转结构体
            /// </summary>
            /// <param name="lvList"></param>
            /// <returns></returns>
            public static DataTable ListToDt(ObservableCollection<LvItemStruct> lvList)
            {
                DataTable dtTmp = new DataTable();
                //增加列
                dtTmp.Columns.Add("序号", typeof(int));
                dtTmp.Columns.Add("ID", typeof(string));
                dtTmp.Columns.Add("数据", typeof(string));
                dtTmp.Columns.Add("命令解析", typeof(string));
                dtTmp.Columns.Add("其他信息", typeof(string));
                for (int i = 0; i < lvList.Count; i++)
                {
                    DataRow dr = dtTmp.NewRow();
                    dr["序号"] = lvList[i].Index;
                    dr["ID"] = SpecialFunctions.Converter.MyStringConverter.UintToHexString(lvList[i].ID);
                    dr["数据"] = SpecialFunctions.Converter.MyStringConverter.BytesToHexSpaceString(lvList[i].DataBytes);
                    dr["命令解析"] = lvList[i].CmdDetail;
                    dr["其他信息"] = lvList[i].OtherInfo;
                    dtTmp.Rows.Add(dr);
                }

                return dtTmp;
            }

            /// <summary>
            /// ListView调整列宽
            /// </summary>
            /// <param name="lv"></param>
            public static void ListViewAutoWith(ref ListView lv)
            {
                GridView gv = lv.View as GridView;
                if (gv != null)
                {
                    foreach (GridViewColumn gvc in gv.Columns)
                    {
                        gvc.Width = gvc.ActualWidth;
                        gvc.Width = Double.NaN;
                    }
                }
            }
        }

        #endregion
    }
}
