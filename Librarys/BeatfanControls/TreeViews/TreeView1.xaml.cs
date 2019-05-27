using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace BeatfanControls.TreeViews
{
    

    /// <summary>
    /// TreeView1.xaml 的交互逻辑
    /// </summary>
    public partial class TreeView1 : UserControl
    {

        /// <summary>
        /// 所有一级子节点集合，一级子节点中包含二级子节点
        /// </summary>
        public NodeX0 m_NodeX0 = new NodeX0();

        public TreeView1()
        {
            InitializeComponent();

            //NodeX1 nodex1 = new NodeX1();
            //nodex1.ItemStartValue = 1;
            //nodex1.ItemEndValue = 2;

            //NodeX1 nodex11 = new NodeX1();
            //nodex11.ItemStartValue = 3;
            //nodex11.ItemEndValue = 5;


            //NodeX2 nodex2 = new NodeX2();
            //nodex2.StartBitSources = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
            //nodex2.EndBitSources = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
            //nodex2.StartBitIndex = 0;
            //nodex2.EndBitIndex = 0;
            //nodex2.ValueName = "速度";
            //nodex2.Value = 109;

            //NodeX2 nodex21 = new NodeX2();
            //nodex21.StartBitSources = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
            //nodex21.EndBitSources = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
            //nodex21.StartBitIndex = 0;
            //nodex21.EndBitIndex = 0;
            //nodex21.ValueName = "速度";
            //nodex21.Value = 109;

            //nodex1.NodeX2s.Add(nodex2);
            //nodex11.NodeX2s.Add(nodex21);

            //TreeViewNodeX0.NodeX1s.Add(nodex1);
            //TreeViewNodeX0.NodeX1s.Add(nodex11);

            //m_NodeX0 = nodeX0;

            myTreeViewTitle.DataContext = m_NodeX0;
            myTreeView.DataContext = m_NodeX0;
            myTreeViewCanID.DataContext = m_NodeX0;
        }






        #region 右键菜单

        #region 右键点击自动获取对应菜单
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem1 = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem1 != null)
            {
                var treeView = treeViewItem1.DataContext.ToString();
                if (treeView.Contains("NodeX1"))
                {
                    //父节点是TreeView，设置一级菜单
                    ChangeToOneLevelContextMenu();

                }
                else
                {
                    //父节点不是TreeView，则属于二级节点，设置为二级菜单
                    ChangeToTwoLevelContextMenu();

                }

                treeViewItem1.IsSelected = true;
                e.Handled = true;
                return;

            }


            myTreeView.ContextMenu = null; //未选中任何节点，不弹出菜单
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }



        
        #endregion

        #region 根菜单
        /// <summary>
        /// 标题菜单新增字节
        /// </summary>
        private void TitleMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            int length = Convert.ToInt32(mi.Tag);

            if (length == 99)
            {
                for (int i = 0; i < m_NodeX0.NodeX1s.Count; i++)
                {
                    m_NodeX0.NodeX1s[i].IsExpanded = true;
                }
                return;
            }

            //前一个结束索引索引
            int lastEndIndex=-1;

            if(m_NodeX0.NodeX1s.Count>0)
                lastEndIndex = m_NodeX0.NodeX1s[m_NodeX0.NodeX1s.Count - 1].ItemEndValue;
            
            if (lastEndIndex + length > 7)
                return;

            lastEndIndex += 1;

            NodeX1 nodex1 = new NodeX1();
            nodex1.StartByteSources = nodex1.EndByteSources = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
            nodex1.ItemStartValue = lastEndIndex;
            nodex1.ItemEndValue = lastEndIndex+length-1;


            m_NodeX0.NodeX1s.Add(nodex1);
            

        }
        #endregion

        #region 一级菜单
        /// <summary>
        /// 切换到一级菜单
        /// </summary>
        private void ChangeToOneLevelContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi1 = new MenuItem();
            mi1.Header = "新增位";
            mi1.Click += TreeViewOneLevelMenuItemAddChild_Click;
            cm.Items.Add(mi1);

            MenuItem mi2 = new MenuItem();
            mi2.Header = "删除字节";
            mi2.Click += TreeViewOneLevelMenuItemDeleteMe_Click;
            cm.Items.Add(mi2);

            MenuItem mi3 = new MenuItem();
            mi3.Header = "上移";
            mi3.Click += TreeViewOneLevelMenuItemUpMe_Click;
            cm.Items.Add(mi3);

            MenuItem mi4 = new MenuItem();
            mi4.Header = "下移";
            mi4.Click += TreeViewOneLevelMenuItemDownMe_Click;
            cm.Items.Add(mi4);

            myTreeView.ContextMenu = cm;
        }

        /// <summary>
        /// 新增位二级节点
        /// </summary>
        private void TreeViewOneLevelMenuItemAddChild_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget) as TreeView;
            int indexX1=-1;

            if (node == null)
                return;

            for (int i = 0; i < node.Items.Count; i++)
            {
                if (node.Items[i] == node.SelectedItem)
                    indexX1 = i;
            }
            if (indexX1 == -1)
                return;

            

            //前一个结束索引索引
            int lastEndIndex = -1;

            if (m_NodeX0.NodeX1s[indexX1].NodeX2s.Count > 0)
                lastEndIndex = m_NodeX0.NodeX1s[indexX1].NodeX2s[m_NodeX0.NodeX1s[indexX1].NodeX2s.Count - 1].EndBitIndex;

            if (lastEndIndex + 1 > 7)
                return;

            lastEndIndex += 1;

            NodeX2 nodex2 = new NodeX2();
            string[] bitIndexSource = null;
            switch (m_NodeX0.NodeX1s[indexX1].ItemLengthValue)
            {
                case 1:
                    bitIndexSource = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
                    break;
                case 2:
                    bitIndexSource = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
                    break;
                case 3:
                    bitIndexSource = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                    "16", "17", "18", "19", "20", "21", "22", "23"};
                    break;
                case 4:
                    bitIndexSource = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                    "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"};
                    break;
                default: break;
            }
            nodex2.StartBitSources = nodex2.EndBitSources = bitIndexSource;
            nodex2.StartBitIndex = lastEndIndex;
            nodex2.EndBitIndex = lastEndIndex;
            nodex2.Resolution = 1;
            nodex2.Offset = 0;
            nodex2.ValueName = "名称";
            nodex2.Value = 0;

            m_NodeX0.NodeX1s[indexX1].NodeX2s.Add(nodex2);
        }

        /// <summary>
        /// 删除自身
        /// </summary>
        private void TreeViewOneLevelMenuItemDeleteMe_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget) as TreeView;
            int indexX1 = -1;

            if (node == null)
                return;

            for (int i = 0; i < node.Items.Count; i++)
            {
                if (node.Items[i] == node.SelectedItem)
                    indexX1 = i;
            }
            if (indexX1 == -1)
                return;

            m_NodeX0.NodeX1s.RemoveAt(indexX1);
        }

        /// <summary>
        /// 上移
        /// </summary>
        private void TreeViewOneLevelMenuItemUpMe_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget) as TreeView;
            int indexX1 = -1;

            if (node == null)
                return;

            for (int i = 0; i < node.Items.Count; i++)
            {
                if (node.Items[i] == node.SelectedItem)
                    indexX1 = i;
            }
            if (indexX1 < 1)
                return;

            //与上方交换索引
            NodeX1 nodex1Tmp = new NodeX1();
            nodex1Tmp.ItemStartValue = m_NodeX0.NodeX1s[indexX1].ItemStartValue;
            nodex1Tmp.ItemEndValue = m_NodeX0.NodeX1s[indexX1].ItemEndValue;
            nodex1Tmp.NodeX2s = m_NodeX0.NodeX1s[indexX1].NodeX2s;

            m_NodeX0.NodeX1s[indexX1].ItemStartValue = m_NodeX0.NodeX1s[indexX1 - 1].ItemStartValue;
            m_NodeX0.NodeX1s[indexX1].ItemEndValue = m_NodeX0.NodeX1s[indexX1 - 1].ItemEndValue;
            m_NodeX0.NodeX1s[indexX1].NodeX2s = m_NodeX0.NodeX1s[indexX1 - 1].NodeX2s;

            m_NodeX0.NodeX1s[indexX1-1].ItemStartValue = nodex1Tmp.ItemStartValue;
            m_NodeX0.NodeX1s[indexX1-1].ItemEndValue = nodex1Tmp.ItemEndValue;
            m_NodeX0.NodeX1s[indexX1-1].NodeX2s = nodex1Tmp.NodeX2s;
        }

        /// <summary>
        /// 下移
        /// </summary>
        private void TreeViewOneLevelMenuItemDownMe_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget) as TreeView;
            int indexX1 = -1;

            if (node == null)
                return;

            for (int i = 0; i < node.Items.Count; i++)
            {
                if (node.Items[i] == node.SelectedItem)
                    indexX1 = i;
            }
            if (indexX1 > 6)
                return;

            //与上方交换索引
            NodeX1 nodex1Tmp = new NodeX1();
            nodex1Tmp.ItemStartValue = m_NodeX0.NodeX1s[indexX1].ItemStartValue;
            nodex1Tmp.ItemEndValue = m_NodeX0.NodeX1s[indexX1].ItemEndValue;
            nodex1Tmp.NodeX2s = m_NodeX0.NodeX1s[indexX1].NodeX2s;

            m_NodeX0.NodeX1s[indexX1].ItemStartValue = m_NodeX0.NodeX1s[indexX1 + 1].ItemStartValue;
            m_NodeX0.NodeX1s[indexX1].ItemEndValue = m_NodeX0.NodeX1s[indexX1 + 1].ItemEndValue;
            m_NodeX0.NodeX1s[indexX1].NodeX2s = m_NodeX0.NodeX1s[indexX1 + 1].NodeX2s;

            m_NodeX0.NodeX1s[indexX1 + 1].ItemStartValue = nodex1Tmp.ItemStartValue;
            m_NodeX0.NodeX1s[indexX1 + 1].ItemEndValue = nodex1Tmp.ItemEndValue;
            m_NodeX0.NodeX1s[indexX1 + 1].NodeX2s = nodex1Tmp.NodeX2s;
        }

        #endregion

        #region 二级菜单
        /// <summary>
        /// 切换到二级菜单
        /// </summary>
        private void ChangeToTwoLevelContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi1 = new MenuItem();
            mi1.Header = "删除";
            mi1.Click += TreeViewTwoLevelMenuItemDeleteMe_Click;
            cm.Items.Add(mi1);

            myTreeView.ContextMenu = cm;
        }

        /// <summary>
        /// 删除自身
        /// </summary>
        private void TreeViewTwoLevelMenuItemDeleteMe_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget) as TreeView;
            int indexX1 = -1;
            int indexX2 = -1;

            if (node == null)
                return;
            

            for (int i = 0; i < node.Items.Count; i++)
            {
                for (int j = 0; j < ((NodeX1)node.Items[i]).NodeX2s.Count; j++)
                    if (((NodeX1)node.Items[i]).NodeX2s[j] == node.SelectedItem)
                    {
                        indexX1 = i;
                        indexX2 = j;
                    }
            }

            
            if (indexX1 == -1 || indexX2 == -1)
                return;

            m_NodeX0.NodeX1s[indexX1].NodeX2s.RemoveAt(indexX2);
        }





        #endregion

        #endregion

        #region 禁止非数字输入
        private void myTreeViewCanID_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9abcdefABCEDF-]+");
            e.Handled = re.IsMatch(e.Text);
        }
        #endregion
    }


    #region 节点绑定数据
    public class NodeX0 : INotifyPropertyChanged
    {
        string _title;
        uint _uintCanID;

        ObservableCollection<NodeX1> _nodex1s = new ObservableCollection<NodeX1>();

        public event PropertyChangedEventHandler PropertyChanged;

        

        /// <summary>
        /// Title
        /// </summary>
        public string Title
        {
            set
            {
                _title = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
            }
            get
            {
                return _title;
            }
        }



        /// <summary>
        /// CAN ID 数值
        /// </summary>
        public uint UINTCANID
        {
            set
            {
                _uintCanID = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("UINTCANID"));
                
            }
            get
            {
                return _uintCanID;
            }
        }

        /// <summary>
        /// 一级子节点集合
        /// </summary>
        public ObservableCollection<NodeX1> NodeX1s
        {
            set
            {
                _nodex1s = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("NodeX1s"));
            }
            get
            {
                return _nodex1s;
            }
        }
    }

    /// <summary>
    /// 一级节点绑定数据
    /// </summary>
    public class NodeX1 : INotifyPropertyChanged
    {
        int _itemstartvalue;
        int _itemendvalue;
        string[] _itemStartByteSource;
        string[] _itemEndByteSource;
        bool _isExpanded;

        ObservableCollection<NodeX2> _nodex2s = new ObservableCollection<NodeX2>();

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            set
            {
                _isExpanded = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
            }
            get { return _isExpanded; }
        }

        /// <summary>
        /// 起始字节数据源
        /// </summary>
        public string[] StartByteSources
        {
            set
            {
                _itemStartByteSource = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("StartByteSources"));
            }
            get
            {
                return _itemStartByteSource;
            }
        }

        /// <summary>
        /// 结束字节数据源
        /// </summary>
        public string[] EndByteSources
        {
            set
            {
                _itemEndByteSource = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("EndByteSources"));
            }
            get
            {
                return _itemEndByteSource;
            }
        }

        /// <summary>
        /// 开始索引
        /// </summary>
        public int ItemStartValue
        {
            set
            {
                _itemstartvalue = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ItemStartValue"));
            }
            get
            {
                return _itemstartvalue;
            }
        }

        /// <summary>
        /// 结束索引
        /// </summary>
        public int ItemEndValue
        {
            set
            {
                _itemendvalue = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ItemEndValue"));
            }
            get
            {
                return _itemendvalue;
            }
        }

        /// <summary>
        /// 长度
        /// </summary>
        public int ItemLengthValue
        {

            get
            {
                return _itemendvalue - _itemstartvalue + 1;
            }
        }

        /// <summary>
        /// 二级子节点集合
        /// </summary>
        public ObservableCollection<NodeX2> NodeX2s
        {
            set
            {
                _nodex2s = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("NodeX2s"));
            }
            get
            {
                return _nodex2s;
            }
        }
    }

    /// <summary>
    /// 二级节点绑定数据
    /// </summary>
    public class NodeX2 : INotifyPropertyChanged
    {
        string[] _startbitSources;
        string[] _endbitSources;
        int _startbitIndex;
        int _endbitIndex;
        double _resolution;
        double _offset;
        string _valuename;
        int _value;

        bool _isExpanded;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            set
            {
                _isExpanded = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("IsExpanded"));
            }
            get { return _isExpanded; }
        }

        /// <summary>
        /// 起始位数据源
        /// </summary>
        public string[] StartBitSources
        {
            set
            {
                _startbitSources = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("StartBitSources"));
            }
            get
            {
                return _startbitSources;
            }
        }

        /// <summary>
        /// 结束位数据源
        /// </summary>
        public string[] EndBitSources
        {
            set
            {
                _endbitSources = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("EndBitSources"));
            }
            get
            {
                return _endbitSources;
            }
        }

        /// <summary>
        /// 起始位索引
        /// </summary>
        public int StartBitIndex
        {
            set
            {
                _startbitIndex = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("StartBitIndex"));
            }
            get
            {
                return _startbitIndex;
            }
        }

        /// <summary>
        /// 结束位索引
        /// </summary>
        public int EndBitIndex
        {
            set
            {
                _endbitIndex = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("EndBitIndex"));
            }
            get
            {
                return _endbitIndex;
            }
        }

        /// <summary>
        /// 位长度
        /// </summary>
        public int BitLength
        {
            get
            {
                return _endbitIndex - _startbitIndex + 1;
            }
        }

        /// <summary>
        /// 分辨率
        /// </summary>
        public double Resolution
        {
            set
            {
                _resolution = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Resolution"));
            }
            get
            {
                return _resolution;
            }
        }

        /// <summary>
        /// 偏移
        /// </summary>
        public double Offset
        {
            set
            {
                _offset = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Offset"));
            }
            get
            {
                return _offset;
            }
        }

        /// <summary>
        /// 值名称
        /// </summary>
        public string ValueName
        {
            set
            {
                _valuename = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ValueName"));
            }
            get
            {
                return _valuename;
            }
        }

        /// <summary>
        /// 值
        /// </summary>
        public int Value
        {
            set
            {
                _value = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
            get
            {
                return _value;
            }
        }
    }
    #endregion


    #region Node数据与xml转换
    public class DataAndXml
    {

        XmlDocument xml = new XmlDocument();

        //string m_XmlDir = "UserDifinedProtocol";


        public void LoadXml(string filePath, ref List<NodeX0> sendNodeX0s, ref List<NodeX0> recNodeX0s)
        {
           
            xml.Load(filePath);

            XmlElement xRoot = xml.DocumentElement; //根元素,"用户自定义协议版本"

            if (xRoot != null)
            {
                //帧数量
                XmlNodeList x0Nlist = xRoot.ChildNodes;  //发送帧或者接收帧

                foreach (XmlElement eCur0 in x0Nlist)
                {

                    List<NodeX0> tmpNodeX0s = new List<NodeX0>();
                    XmlNodeList x00Nlist = eCur0.ChildNodes;//多个帧节点，"发送帧1","发送帧2","发送帧3"。。。
                    foreach (XmlElement eCur00 in x00Nlist) //每个帧的内容，"Title","CANID"。。。
                    {
                        NodeX0 nodeX0 = new NodeX0();
                        foreach (XmlNode eCur01 in eCur00.ChildNodes) //0级节点封装节点，每一个都是一帧
                        {

                            switch (eCur01.Name)
                            {
                                case "Title":
                                    nodeX0.Title = eCur01.InnerText; //要读取节点的属性
                                    break;
                                case "CANID":
                                    nodeX0.UINTCANID = SpecialFunctions.HexStringToUint(eCur01.InnerText);
                                    break;

                                case "NodeX1":
                                    {
                                        #region 一级节点

                                        foreach (XmlNode eCur10 in eCur01.ChildNodes) //1级节点内所有内容遍历
                                        {
                                            NodeX1 nodeX1 = new NodeX1();
                                            nodeX1.StartByteSources = nodeX1.EndByteSources = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
                                            XmlNodeList eCur10List = eCur10.ChildNodes;
                                            foreach (XmlNode eCur1 in eCur10List)
                                            {
                                                switch (eCur1.Name)
                                                {
                                                    case "ItemStartValue":
                                                        nodeX1.ItemStartValue = Convert.ToInt32(eCur1.InnerText); //要读取节点的属性
                                                        break;
                                                    case "ItemEndValue":
                                                        nodeX1.ItemEndValue = Convert.ToInt32(eCur1.InnerText);
                                                        break;

                                                    case "NodeX2":
                                                        {
                                                            #region 二级节点

                                                            foreach (XmlNode eCur20 in eCur1.ChildNodes) //1级节点内所有内容遍历
                                                            {
                                                                XmlNodeList eCur20List = eCur20.ChildNodes;
                                                                NodeX2 nodeX2 = new NodeX2();
                                                                string[] bitIndexSource = null;
                                                                switch (nodeX1.ItemLengthValue)
                                                                {
                                                                    case 1:
                                                                        bitIndexSource = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
                                                                        break;
                                                                    case 2:
                                                                        bitIndexSource = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
                                                                        break;
                                                                    case 3:
                                                                        bitIndexSource = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                                                                            "16", "17", "18", "19", "20", "21", "22", "23"};
                                                                        break;
                                                                    case 4:
                                                                        bitIndexSource = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                                                                            "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"};
                                                                        break;
                                                                    default: break;
                                                                }
                                                                nodeX2.StartBitSources = nodeX2.EndBitSources = bitIndexSource;
                                                                //nodeX2.StartBitSources = nodeX2.EndBitSources = new string[] { "0", "1", "2", "3", "4", "5", "6", "7" };
                                                                nodeX2.Value = 0;
                                                                
                                                                
                                                                foreach (XmlNode eCur2 in eCur20List)
                                                                {
                                                                    string a = eCur2.ParentNode.Name;
                                                                    
                                                                    switch (eCur2.Name)
                                                                    {
                                                                        case "StartIndex":
                                                                            nodeX2.StartBitIndex = Convert.ToInt32(eCur2.InnerText); //要读取节点的属性
                                                                            break;
                                                                        case "EndIndex":
                                                                            nodeX2.EndBitIndex = Convert.ToInt32(eCur2.InnerText);
                                                                            break;
                                                                        case "Resolution":
                                                                            nodeX2.Resolution = Convert.ToDouble(eCur2.InnerText);
                                                                            break;
                                                                        case "Offset":
                                                                            nodeX2.Offset = Convert.ToDouble(eCur2.InnerText);
                                                                            break;
                                                                        case "ValueName":
                                                                            nodeX2.ValueName = eCur2.InnerText;
                                                                            
                                                                            break;
                                                                        case "Value":
                                                                            nodeX2.Value = Convert.ToInt32(eCur2.InnerText);
                                                                            
                                                                            break;
                                                                        default: break;
                                                                    }
                                                                }
                                                                nodeX1.NodeX2s.Add(nodeX2);

                                                                
                                                            }

                                                            #endregion
                                                        }
                                                        break;
                                                    default: break;
                                                }
                                            }
                                            nodeX0.NodeX1s.Add(nodeX1);
                                        }
                                        #endregion
                                    }
                                    break;
                                default: break;
                            }
                        }
                        tmpNodeX0s.Add(nodeX0);

                    }

                    if (eCur0.Name == "SendRoot")
                        sendNodeX0s = tmpNodeX0s;
                    else
                        recNodeX0s = tmpNodeX0s;
                }
            }

        }

        public void CreateXml(List<NodeX0> sendNodeX0s, List<NodeX0> recNodeX0s, string fileName)
        {
            //声明XML文件
            xml.CreateXmlDeclaration("1.0", "utf-8", null);
            XmlElement root = xml.CreateElement("UserDefineProtocol"); //创建根节点
            XmlAttribute bookStoreTitle = xml.CreateAttribute("用户自定义协议版本");//创建节点属性  
            //bookStoreTitle.InnerText = "v1.0";
            root.SetAttributeNode(bookStoreTitle);//将节点与属性相关联  
            xml.AppendChild(root);//将根节点保存入文档中

            //发送帧节点集合
            XmlElement rootSend = xml.CreateElement("SendRoot"); //创建节点
            rootSend.SetAttribute("发送帧根节点", null);//节点属性  
            root.AppendChild(rootSend);//将节点保存入根节点中

            //接收帧节点集合
            XmlElement rootRec = xml.CreateElement("ReceiveRoot"); //创建节点
            rootRec.SetAttribute("接收帧根节点", null);//节点属性  
            root.AppendChild(rootRec);//将节点保存入根节点中

            List<NodeX0> tmpNodeX0s = null;
            XmlElement tmpSecondRoot = null;

            for (int t = 0; t < 2; t++)
            {
                if (t == 0)
                {
                    tmpNodeX0s = sendNodeX0s;
                    tmpSecondRoot = rootSend;
                }
                else
                {
                    tmpNodeX0s = recNodeX0s;
                    tmpSecondRoot = rootRec;
                }

                for (int i = 0; i < tmpNodeX0s.Count; i++)
                {
                    //帧节点
                    XmlElement child00_FrameNode = xml.CreateElement("帧节点"); //创建帧子节点
                    child00_FrameNode.SetAttribute("节点名称", tmpNodeX0s[i].Title);
                    tmpSecondRoot.AppendChild(child00_FrameNode);

                    //NodeX0
                    XmlElement child01_Title = xml.CreateElement("Title"); //创建标题子节点
                    child01_Title.InnerText = tmpNodeX0s[i].Title;
                    child00_FrameNode.AppendChild(child01_Title);

                    XmlElement child02_CanId = xml.CreateElement("CANID"); //创建CAN ID子节点
                    child02_CanId.InnerText = tmpNodeX0s[i].UINTCANID.ToString("X8");
                    child00_FrameNode.AppendChild(child02_CanId);

                    XmlElement child03_NodeX1 = xml.CreateElement("NodeX1"); //创建NodeX1子节点
                    child03_NodeX1.SetAttribute("节点集合", "字节节点"); //节点属性
                    child00_FrameNode.AppendChild(child03_NodeX1);

                    for (int j = 0; j < tmpNodeX0s[i].NodeX1s.Count; j++)
                    {
                        //字节节点
                        XmlElement child10_ByteNode = xml.CreateElement("ByteNode"); //创建帧子节点
                        child10_ByteNode.SetAttribute("节点集合", "字节");
                        child03_NodeX1.AppendChild(child10_ByteNode);

                        //NodeX1
                        XmlElement child11_ItemStartValue = xml.CreateElement("ItemStartValue"); //创建起始字节索引子节点
                        child11_ItemStartValue.InnerText = tmpNodeX0s[i].NodeX1s[j].ItemStartValue.ToString();
                        child10_ByteNode.AppendChild(child11_ItemStartValue);

                        XmlElement child12_ItemEndValue = xml.CreateElement("ItemEndValue"); //创建结束字节索引子节点
                        child12_ItemEndValue.InnerText = tmpNodeX0s[i].NodeX1s[j].ItemEndValue.ToString();
                        child10_ByteNode.AppendChild(child12_ItemEndValue);

                        XmlElement child13_NodeX2 = xml.CreateElement("NodeX2"); //创建NodeX2子节点
                        child13_NodeX2.SetAttribute("节点集合", "位节点");
                        child10_ByteNode.AppendChild(child13_NodeX2);

                        for (int k = 0; k < tmpNodeX0s[i].NodeX1s[j].NodeX2s.Count; k++)
                        {
                            //字节节点
                            XmlElement child20_ByteNode = xml.CreateElement("BitNode"); //创建帧子节点
                            child20_ByteNode.SetAttribute("节点集合", "位");
                            child13_NodeX2.AppendChild(child20_ByteNode);

                            //NodeX2

                            XmlElement child21_ValueName = xml.CreateElement("ValueName"); //创建ValueName子节点
                            child21_ValueName.InnerText = tmpNodeX0s[i].NodeX1s[j].NodeX2s[k].ValueName.ToString();
                            child20_ByteNode.AppendChild(child21_ValueName);

                            XmlElement child22_Value = xml.CreateElement("Value"); //创建Value子节点
                            child22_Value.InnerText = tmpNodeX0s[i].NodeX1s[j].NodeX2s[k].Value.ToString();
                            child20_ByteNode.AppendChild(child22_Value);

                            XmlElement child23_Resolution = xml.CreateElement("Resolution"); //创建分辨率子节点
                            child23_Resolution.InnerText = tmpNodeX0s[i].NodeX1s[j].NodeX2s[k].Resolution.ToString();
                            child20_ByteNode.AppendChild(child23_Resolution);

                            XmlElement child24_Offset = xml.CreateElement("Offset"); //创建偏移子节点
                            child24_Offset.InnerText = tmpNodeX0s[i].NodeX1s[j].NodeX2s[k].Offset.ToString();
                            child20_ByteNode.AppendChild(child24_Offset);

                            XmlElement child25_StartIndex = xml.CreateElement("StartIndex"); //创建起始字节索引子节点
                            child25_StartIndex.InnerText = tmpNodeX0s[i].NodeX1s[j].NodeX2s[k].StartBitIndex.ToString();
                            child20_ByteNode.AppendChild(child25_StartIndex);

                            XmlElement child26_EndIndex = xml.CreateElement("EndIndex"); //创建结束字节索引子节点
                            child26_EndIndex.InnerText = tmpNodeX0s[i].NodeX1s[j].NodeX2s[k].EndBitIndex.ToString();
                            child20_ByteNode.AppendChild(child26_EndIndex);

                        }
                    }

                }
            }


            //if (!Directory.Exists(m_XmlDir))
            //    Directory.CreateDirectory(m_XmlDir);

            xml.Save(fileName);
        }


    }
    #endregion

    #region Node数据与Bytes之间转换
    public class NodeXAndBytes
    {
        public struct SendFrame
        {
            public uint canId;
            public byte[] datas;
        }

        /// <summary>
        /// 发送数据转换
        /// </summary>
        public List<SendFrame> NodeX0sToBytes(List<NodeX0> nodeX0s)
        {
            List<SendFrame> sendBytesList = new List<SendFrame>();

            for (int i = 0; i < nodeX0s.Count; i++)
            {
                SendFrame sf = new SendFrame();
                sf.canId = nodeX0s[i].UINTCANID;
                sf.datas = new byte[8];
                for (int j = 0; j < nodeX0s[i].NodeX1s.Count; j++)
                {
                    switch (nodeX0s[i].NodeX1s[j].ItemLengthValue) //第一个可用字节长度
                    {
                        case 1: //一个字节
                            for (int k = 0; k < nodeX0s[i].NodeX1s[j].NodeX2s.Count; k++)
                            {
                                int a = nodeX0s[i].NodeX1s[j].NodeX2s[k].Value;
                                int offset = nodeX0s[i].NodeX1s[j].NodeX2s[k].StartBitIndex;
                                int and = (int)Math.Pow(2, nodeX0s[i].NodeX1s[j].NodeX2s[k].BitLength) - 1;
                                byte value = (byte)((a & and) << offset);
                                value = (byte)((value - nodeX0s[i].NodeX1s[j].NodeX2s[k].Offset) / nodeX0s[i].NodeX1s[j].NodeX2s[k].Resolution);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue] += value;
                            }
                            break;
                        case 2: //二个字节
                            {
                                int value = (int)((nodeX0s[i].NodeX1s[j].NodeX2s[0].Value - nodeX0s[i].NodeX1s[j].NodeX2s[0].Offset) / nodeX0s[i].NodeX1s[j].NodeX2s[0].Resolution);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue] = (byte)(value & 0xff);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemEndValue] = (byte)((value >> 8) & 0xff);
                            }
                            break;
                        case 3: //三个字节
                            {
                                int value = (int)((nodeX0s[i].NodeX1s[j].NodeX2s[0].Value - nodeX0s[i].NodeX1s[j].NodeX2s[0].Offset) / nodeX0s[i].NodeX1s[j].NodeX2s[0].Resolution);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue] = (byte)(value & 0xff);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue + 1] = (byte)((value >> 8) & 0xff);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemEndValue] = (byte)((value >> 16) & 0xff);
                            }
                            break;
                        case 4: //四个字节
                            {
                                int value = (int)((nodeX0s[i].NodeX1s[j].NodeX2s[0].Value - nodeX0s[i].NodeX1s[j].NodeX2s[0].Offset) / nodeX0s[i].NodeX1s[j].NodeX2s[0].Resolution);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue] = (byte)(value & 0xff);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue + 1] = (byte)((value >> 8) & 0xff);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue + 2] = (byte)((value >> 16) & 0xff);
                                sf.datas[nodeX0s[i].NodeX1s[j].ItemEndValue] = (byte)((value >> 24) & 0xff);
                            }
                            break;
                        default:
                            break;
                    }

                }
                sendBytesList.Add(sf);
            }
            return sendBytesList;
        }

        /// <summary>
        /// 接收数据转换
        /// </summary>
        public void BytesToFrame(SendFrame sf, ref List<NodeX0> nodeX0s)
        {
            
            for (int i = 0; i < nodeX0s.Count; i++)
            {
                if (sf.canId == nodeX0s[i].UINTCANID)
                {
                    for (int j = 0; j < nodeX0s[i].NodeX1s.Count; j++)
                    {
                        #region 字节值转换
                        switch (nodeX0s[i].NodeX1s[j].ItemLengthValue) //第一个可用字节长度
                        {
                            case 1: //一个字节
                                for (int k = 0; k < nodeX0s[i].NodeX1s[j].NodeX2s.Count; k++)
                                {
                                    #region 位域转换
                                    int a = sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue];
                                    int offset = nodeX0s[i].NodeX1s[j].NodeX2s[k].StartBitIndex;
                                    int and = (int)Math.Pow(2, nodeX0s[i].NodeX1s[j].NodeX2s[k].BitLength) - 1;

                                    byte value = (byte)((a >>offset)&and);
                                    
                                    value = (byte)(value*nodeX0s[i].NodeX1s[j].NodeX2s[k].Resolution + nodeX0s[i].NodeX1s[j].NodeX2s[k].Offset);
                                        nodeX0s[i].NodeX1s[j].NodeX2s[k].Value = value;

                                    #endregion
                                }
                                break;
                            case 2: //二个字节
                                {
                                    nodeX0s[i].NodeX1s[j].NodeX2s[0].Value = (int)((sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue] +
                                    sf.datas[nodeX0s[i].NodeX1s[j].ItemEndValue] * 256) * nodeX0s[i].NodeX1s[j].NodeX2s[0].Resolution+nodeX0s[i].NodeX1s[j].NodeX2s[0].Offset);
                                    
                                }
                                break;
                            case 3: //三个字节
                                {
                                    nodeX0s[i].NodeX1s[j].NodeX2s[0].Value = (int)((sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue] +
                                    sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue + 1] * 256 +
                                    sf.datas[nodeX0s[i].NodeX1s[j].ItemEndValue] * 256 * 256) * nodeX0s[i].NodeX1s[j].NodeX2s[0].Resolution + nodeX0s[i].NodeX1s[j].NodeX2s[0].Offset);
     
                                }

                                break;
                            case 4: //四个字节
                                {
                                    nodeX0s[i].NodeX1s[j].NodeX2s[0].Value = (int)((sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue] +
                                    sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue + 1] * 256 +
                                    sf.datas[nodeX0s[i].NodeX1s[j].ItemStartValue + 1] * 256 * 256 +
                                    sf.datas[nodeX0s[i].NodeX1s[j].ItemEndValue] * 256 * 256 * 256) * nodeX0s[i].NodeX1s[j].NodeX2s[0].Resolution + nodeX0s[i].NodeX1s[j].NodeX2s[0].Offset);
      
                                }
                                break;
                            default:
                                break;
                        }
                        #endregion
                    }

                }
            }
        }
    }
    #endregion

    #region 值转换器
    internal class SpecialFunctions
    {
        /// <summary>
        /// 16进制字符串转Uint32，字符串前含0x，字符串不足偶数则前面补0
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        internal static UInt32 HexStringToUint(string hexString)
        {

            UInt32 result = 0;
            if (hexString.Length % 2 == 1)
                hexString = "0" + hexString.Substring(2, hexString.Length - 2);

            //0x不计入
            for (int i = 0; i < hexString.Length - 1; i += 2)
            {
                result <<= 8; //左移一个字节，低字节表示高位
                result += Convert.ToUInt32(hexString.Substring(i, 2), 16);
            }
            return result;
        }

        /// <summary>
        /// Uint32转16进制字符串，字符串前含有0x
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        internal static string UintToHexString(UInt32 num)
        {
            string result = num.ToString("X8");
            //while (result.Length < 8)
            //{
            //    result = "0" + result;
            //}
            return result;
        }
    }
    /// <summary>
    /// 自定义字符串转换事件
    /// </summary>
    public class StringUintConverter : IValueConverter
    {


        /// <summary>
        /// 当值从绑定源传播给绑定目标时，调用方法Convert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return DependencyProperty.UnsetValue;
            string str = SpecialFunctions.UintToHexString((UInt32)value);
            return str;
        }



        /// <summary>
        /// 当值从绑定目标传播给绑定源时，调用此方法ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            uint txtDate = SpecialFunctions.HexStringToUint(str);

            return txtDate;
        }
    }
    #endregion



}
