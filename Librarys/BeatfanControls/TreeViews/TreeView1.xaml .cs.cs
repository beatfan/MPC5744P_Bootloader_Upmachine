using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BeatfanControls.TreeViews
{
    /// <summary>
    /// TreeView1.xaml 的交互逻辑
    /// </summary>
    public partial class TreeView1 : UserControl
    {
        public TreeView1()
        {
            InitializeComponent();
            SetRootLevelContextMenu();
        }

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty TreeViewTitleProperty =
            DependencyProperty.Register("TreeViewTitle", typeof(string), typeof(TreeView1),
            new PropertyMetadata("TreeViewTitle", new PropertyChangedCallback(TreeView1.OnTreeViewTitleChanged)));

        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public string TreeViewTitle
        {
            get
            {
                return (string)GetValue(TreeViewTitleProperty);
            }
            set
            {
                SetValue(TreeViewTitleProperty, value);
            }
        }

        private static void OnTreeViewTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            TreeView1 gauge = d as TreeView1;
            gauge.myTreeViewTitle.Content = e.NewValue.ToString();
        }


        #region 右键菜单选项
        #region 右键选中并弹出菜单

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        /// <summary>
        /// TreeView中右键
        /// </summary>
        private void TreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem1 = VisualUpwardSearch<UserTreeViewItem1>(e.OriginalSource as DependencyObject) as UserTreeViewItem1;
            if (treeViewItem1 != null)
            {
                //父节点是TreeView，设置一级菜单
                ChangeToOneLevelContextMenu();
                treeViewItem1.IsSelected = true;
                e.Handled = true;
                return;
                
            }
            var treeViewItem2 = VisualUpwardSearch<UserTreeViewItem2>(e.OriginalSource as DependencyObject) as UserTreeViewItem2;
            if (treeViewItem2 != null)
            {
                //父节点不是TreeView，则属于二级节点，设置为二级菜单
                ChangeToTwoLevelContextMenu();
                treeViewItem2.IsSelected = true;
                e.Handled = true;
                return;
            }
            
            myTreeView.ContextMenu = null; //未选中任何节点，不弹出菜单
        }

        #endregion

        #region 根菜单
        /// <summary>
        /// 设置根菜单
        /// </summary>
        private void SetRootLevelContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi1 = new MenuItem();
            mi1.Header = "新增字节";
            mi1.Click += TreeViewAddOneByteChild_Click;
            cm.Items.Add(mi1);

            MenuItem mi2 = new MenuItem();
            mi2.Header = "新增双字节";
            mi2.Click += TreeViewAddTwoBytesChild_Click;
            cm.Items.Add(mi2);

            MenuItem mi3 = new MenuItem();
            mi3.Header = "新增三字节";
            mi3.Click += TreeViewAddThreeBytesChild_Click;
            cm.Items.Add(mi3);

            MenuItem mi4 = new MenuItem();
            mi4.Header = "新增四字节";
            mi4.Click += TreeViewAddFourBytesChild_Click;
            cm.Items.Add(mi4);

            myTreeViewTitle.ContextMenu = cm;
        }

        /// <summary>
        /// 根节点右键菜单点击，新增一级节点，一个字节的节点
        /// </summary>
        private void TreeViewAddOneByteChild_Click(object sender, RoutedEventArgs e)
        {
            UserTreeViewItem1 tviTmp = new UserTreeViewItem1();
            int lastEndIndex = -1;
            if(myTreeView.Items.Count>0)
                lastEndIndex = ((UserTreeViewItem1)myTreeView.Items[myTreeView.Items.Count - 1]).TreeViewItemEndByte;
            if (lastEndIndex >= 7)
                return;
            tviTmp.TreeViewItemStartByte = lastEndIndex + 1;
            tviTmp.TreeViewItemEndByte = lastEndIndex + 1;            

            tviTmp.PreviewMouseRightButtonDown += TreeViewItem_PreviewMouseRightButtonDown;
            myTreeView.Items.Add(tviTmp); 
        }

        /// <summary>
        /// 根节点右键菜单点击，新增一级节点，2个字节的节点
        /// </summary>
        private void TreeViewAddTwoBytesChild_Click(object sender, RoutedEventArgs e)
        {
            UserTreeViewItem1 tviTmp = new UserTreeViewItem1();
            int lastEndIndex = -1;
            if (myTreeView.Items.Count > 0)
                lastEndIndex = ((UserTreeViewItem1)myTreeView.Items[myTreeView.Items.Count - 1]).TreeViewItemEndByte;

            if (lastEndIndex >= 6)
                return;
            tviTmp.TreeViewItemStartByte = lastEndIndex + 1;
            tviTmp.TreeViewItemEndByte = lastEndIndex + 2;

            tviTmp.PreviewMouseRightButtonDown += TreeViewItem_PreviewMouseRightButtonDown;
            myTreeView.Items.Add(tviTmp);
        }

        /// <summary>
        /// 根节点右键菜单点击，新增一级节点，3个字节的节点
        /// </summary>
        private void TreeViewAddThreeBytesChild_Click(object sender, RoutedEventArgs e)
        {
            UserTreeViewItem1 tviTmp = new UserTreeViewItem1();
            int lastEndIndex = -1;
            if (myTreeView.Items.Count > 0)
                lastEndIndex = ((UserTreeViewItem1)myTreeView.Items[myTreeView.Items.Count - 1]).TreeViewItemEndByte;

            if (lastEndIndex >= 5)
                return;
            tviTmp.TreeViewItemStartByte = lastEndIndex + 1;
            tviTmp.TreeViewItemEndByte = lastEndIndex + 3;

            tviTmp.PreviewMouseRightButtonDown += TreeViewItem_PreviewMouseRightButtonDown;
            myTreeView.Items.Add(tviTmp);
        }

        /// <summary>
        /// 根节点右键菜单点击，新增一级节点，4个字节的节点
        /// </summary>
        private void TreeViewAddFourBytesChild_Click(object sender, RoutedEventArgs e)
        {
            UserTreeViewItem1 tviTmp = new UserTreeViewItem1();
            int lastEndIndex = -1;
            if (myTreeView.Items.Count > 0)
                lastEndIndex = ((UserTreeViewItem1)myTreeView.Items[myTreeView.Items.Count - 1]).TreeViewItemEndByte;

            if (lastEndIndex >= 4)
                return;
            tviTmp.TreeViewItemStartByte = lastEndIndex + 1;
            tviTmp.TreeViewItemEndByte = lastEndIndex + 4;

            tviTmp.PreviewMouseRightButtonDown += TreeViewItem_PreviewMouseRightButtonDown;
            myTreeView.Items.Add(tviTmp);
        }


        #endregion

        #region 一级子菜单
        /// <summary>
        /// 切换到一级菜单
        /// </summary>
        private void ChangeToOneLevelContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi1 = new MenuItem();
            mi1.Header = "新增位";
            mi1.Click += TreeViewMenuItemAddChild_Click;
            cm.Items.Add(mi1);

            MenuItem mi2 = new MenuItem();
            mi2.Header = "删除字节";
            mi2.Click += TreeViewMenuItemDeleteMe_Click;
            cm.Items.Add(mi2);

            MenuItem mi3 = new MenuItem();
            mi3.Header = "上移";
            mi3.Click += TreeViewMenuItemUpMe_Click;
            cm.Items.Add(mi3);

            MenuItem mi4 = new MenuItem();
            mi4.Header = "上移";
            mi4.Click += TreeViewMenuItemDownMe_Click;
            cm.Items.Add(mi4);

            myTreeView.ContextMenu = cm;
        }

        #region 新增子节点
        /// <summary>
        /// 一级子节点右键菜单点击，新增子二级节点
        /// </summary>
        private void TreeViewMenuItemAddChild_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget).SelectedItem as TreeViewItem;

            if (node == null)
                return;

            UserTreeViewItem2 tviTmp = new UserTreeViewItem2();
            //tviTmp.SetValue(TreeViewItem.StyleProperty, Application.Current.Resources["myTreeViewItemStyle2"]);
            int lastTag = -1;
            if (node.Items.Count != 0)
            {
                lastTag = ((UserTreeViewItem2)node.Items[node.Items.Count - 1]).TreeViewItemEndBit;
                if (lastTag >= 7)
                    return;
            }

            tviTmp.Tag = lastTag + 1;



            tviTmp.PreviewMouseRightButtonDown += TreeViewItem_PreviewMouseRightButtonDown;
            node.Items.Add(tviTmp); ;
        }


        #endregion

        #region 删除字节
        /// <summary>
        /// 删除自身节点
        /// </summary>
        private void TreeViewMenuItemDeleteMe_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget).SelectedItem as TreeViewItem;

            if (node == null)
                return;

            //从父节点删除自身
            ((TreeView)node.Parent).Items.Remove(node);

        }
        #endregion

        #region 上移
        /// <summary>
        /// 节点上移
        /// </summary>
        private void TreeViewMenuItemUpMe_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #region 下移
        /// <summary>
        /// 节点上移
        /// </summary>
        private void TreeViewMenuItemDownMe_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion

        #endregion

        #region 二级子菜单

        /// <summary>
        /// 切换到二级菜单
        /// </summary>
        private void ChangeToTwoLevelContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi1 = new MenuItem();
            mi1.Header = "删除";
            cm.Items.Add(mi1);

            myTreeView.ContextMenu = cm;
        }

        /// <summary>
        /// 二级子节点右键菜单点击，删除此子节点
        /// </summary>
        private void TreeViewItemMenuItemDeleteChild_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((TreeViewItem)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget).Parent).SelectedItem as TreeViewItem;
            if (node == null)
                return;

            ((TreeViewItem)node.Parent).Items.Remove(node);

        }

        /// <summary>
        /// 二级子节点右键菜单点击，清除信息
        /// </summary>
        private void TreeViewItemMenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            //获取到对应的TreeViewItem
            var node = ((TreeView)((TreeViewItem)((System.Windows.Controls.Primitives.Popup)((ContextMenu)((MenuItem)sender).Parent).Parent).PlacementTarget).Parent).SelectedItem as TreeViewItem;
            string str = string.Empty;
            int num = VisualTreeHelper.GetChildrenCount(node);
            for (int i = 0; i < num; i++)
            {
                DependencyObject v = (DependencyObject)VisualTreeHelper.GetChild(node, i);
                TextBox child = v as TextBox;
                if (child != null)
                {
                    str += child.Text + ";";
                }

            }
            MessageBox.Show(str);
        }
        #endregion

        #endregion

        #region 自定义一级节点TreeViewItem
        /// <summary>
        /// 自定义TreeViewItem，一级位节点
        /// </summary>
        internal class UserTreeViewItem1 : TreeViewItem
        {
            


            public UserTreeViewItem1()
            {

                #region template
                //ControlTemplate ct = new ControlTemplate();

                //FrameworkElementFactory feL1 = new FrameworkElementFactory(typeof(Label));
                //feL1.SetValue(Label.ContentProperty, "byte "+ TreeViewItemStartByteProperty.ToString() + ":"+ TreeViewItemEndByteProperty.ToString());
                //feL1.SetValue(Label.NameProperty, "feL1");


                //ct.VisualTree = feL1;

                //this.Template = ct;
                #endregion

                //SetValue(StyleProperty, Application.Current.Resources["myTreeViewItemStyle2"]);

            }

            #region 属性

            //起始字节
            static readonly DependencyProperty TreeViewItemStartByteProperty =
                DependencyProperty.Register("TreeViewItemStartByte", typeof(int), typeof(UserTreeViewItem1),
                new PropertyMetadata(-1, new PropertyChangedCallback(OnTreeViewItemStartByteChanged)));

            //结束字节
            static readonly DependencyProperty TreeViewItemEndByteProperty =
                DependencyProperty.Register("TreeViewItemEndByte", typeof(int), typeof(UserTreeViewItem1),
                new PropertyMetadata(-1, new PropertyChangedCallback(OnTreeViewItemEndByteChanged)));

            //长度
            static readonly DependencyProperty TreeViewItemByteLengthProperty =
                DependencyProperty.Register("TreeViewItemByteLength", typeof(int), typeof(UserTreeViewItem1),
                new PropertyMetadata(0, null));


            #endregion

            #region set/get


            /// <summary>
            /// 起始字节
            /// </summary>
            public int TreeViewItemStartByte
            {
                get
                {
                    return (int)GetValue(TreeViewItemStartByteProperty);
                }
                set
                {
                    SetValue(TreeViewItemStartByteProperty, value);
                }
            }

            /// <summary>
            /// 结束字节
            /// </summary>
            public int TreeViewItemEndByte
            {
                get
                {
                    return (int)GetValue(TreeViewItemEndByteProperty);
                }
                set
                {
                    SetValue(TreeViewItemEndByteProperty, value);
                }
            }

            /// <summary>
            /// 长度
            /// </summary>
            public int TreeViewItemByteLength
            {
                get
                {
                    return (int)GetValue(TreeViewItemByteLengthProperty);
                }
            }
            
            #endregion

            #region 事件


            //起始位
            private static void OnTreeViewItemStartByteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                int b = Convert.ToInt32(e.NewValue);
                if (b < 0 || b > 7)
                    return;

                UserTreeViewItem1 gauge = d as UserTreeViewItem1;
                //Label lb = gauge.GetTemplateChild("feL1") as Label;
                //Label lb = LogicalTreeHelper.FindLogicalNode(VisualTreeHelper.GetChild(gauge, 0), "feL1") as Label;
                //lb.Content = "byte " + gauge.TreeViewItemStartByte.ToString() + ":" + e.NewValue.ToString();

                gauge.Header = "byte " + gauge.TreeViewItemStartByte.ToString() + ":" + e.NewValue.ToString();
                //长度
                gauge.SetValue(TreeViewItemByteLengthProperty, gauge.TreeViewItemEndByte - gauge.TreeViewItemStartByte);
            }

            //结束位
            private static void OnTreeViewItemEndByteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                int b = Convert.ToInt32(e.NewValue);
                if (b < 0 || b > 7)
                    return;

                UserTreeViewItem1 gauge = d as UserTreeViewItem1;

                //Label lb = gauge.GetTemplateChild("feL1") as Label;
                //Label lb = gauge.Template.FindName("feL1",gauge) as Label;
                //lb.Content = "byte " + gauge.TreeViewItemStartByte.ToString() + ":" + e.NewValue.ToString();

                gauge.Header = "byte " + gauge.TreeViewItemStartByte.ToString() + ":" + e.NewValue.ToString();
                //长度
                gauge.SetValue(TreeViewItemByteLengthProperty, gauge.TreeViewItemEndByte - gauge.TreeViewItemStartByte);
            }




            #endregion
        }
        #endregion

        #region 自定义二级节点TreeViewItem
        /// <summary>
        /// 自定义TreeViewItem，二级位节点
        /// </summary>
        internal class UserTreeViewItem2 : TreeViewItem
        {
            //FrameworkElementFactory feL1;
            ///// <summary>
            ///// 开始位
            ///// </summary>
            //FrameworkElementFactory feCbStartBit;
            
            ///// <summary>
            ///// 结束位
            ///// </summary>
            //FrameworkElementFactory feCbEndBit;
            ///// <summary>
            ///// 名称
            ///// </summary>
            //FrameworkElementFactory feTbName;
            ///// <summary>
            ///// 实际值
            ///// </summary>
            //FrameworkElementFactory feTbRealValue;


            public UserTreeViewItem2()
            {
                SetValue(StyleProperty, Application.Current.Resources["myTreeViewItemStyle2"]);
                this.PreviewMouseRightButtonDown += feL1.PreviewMouseRightButtonDown;
                
                //#region template
                //ControlTemplate ct = new ControlTemplate();

                //feL1 = new FrameworkElementFactory(typeof(Label));
                //feL1.SetValue(Label.ContentProperty, "bit");

                //feCbStartBit = new FrameworkElementFactory(typeof(ComboBox));
                //feCbStartBit.SetValue(ComboBox.ItemsSourceProperty, new string[] { "0", "1", "2", "3", "4", "5", "6", "7" });
                //feCbStartBit.SetValue(ComboBox.SelectedIndexProperty, 0);
                //feCbStartBit.SetValue(ComboBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //feCbStartBit.SetValue(ComboBox.VerticalAlignmentProperty, VerticalAlignment.Center);

                //FrameworkElementFactory feL3 = new FrameworkElementFactory(typeof(Label));
                //feL3.SetValue(Label.ContentProperty, "-");
                //feL3.SetValue(Label.MarginProperty, new Thickness { Left = -5, Right = -5, Top = 0, Bottom = 0 });
                //feL3.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //feL3.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

                //feCbEndBit = new FrameworkElementFactory(typeof(ComboBox));
                //feCbEndBit.SetValue(ComboBox.ItemsSourceProperty, new string[] { "0", "1", "2", "3", "4", "5", "6", "7" });
                //feCbEndBit.SetValue(ComboBox.SelectedIndexProperty, 0);
                //feCbEndBit.SetValue(ComboBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //feCbEndBit.SetValue(ComboBox.VerticalAlignmentProperty, VerticalAlignment.Center);

                //feTbName = new FrameworkElementFactory(typeof(TextBox));
                //feTbName.SetValue(TextBox.TextProperty, "名称");
                //feTbName.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //feTbName.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

                //FrameworkElementFactory feL5 = new FrameworkElementFactory(typeof(Label));
                //feL5.SetValue(Label.ContentProperty, ":");
                //feL5.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //feL5.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);

                //feTbRealValue = new FrameworkElementFactory(typeof(Label));
                //feTbRealValue.SetValue(Label.ContentProperty, "0");
                //feTbRealValue.SetValue(Label.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //feTbRealValue.SetValue(Label.VerticalAlignmentProperty, VerticalAlignment.Center);

                //FrameworkElementFactory feSP = new FrameworkElementFactory(typeof(StackPanel));
                //feSP.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                //feSP.SetValue(Label.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                //feSP.SetValue(Label.VerticalAlignmentProperty, VerticalAlignment.Center);

                //feSP.AppendChild(feL1);
                //feSP.AppendChild(feCbStartBit);
                //feSP.AppendChild(feL3);
                //feSP.AppendChild(feCbEndBit);
                //feSP.AppendChild(feTbName);
                //feSP.AppendChild(feL5);
                //feSP.AppendChild(feTbRealValue);

                //ct.VisualTree = feSP;

                //this.Template = ct;
                //#endregion

            }

            #region 属性

            //起始位
            static readonly DependencyProperty TreeViewItemStartBitProperty =
                DependencyProperty.Register("TreeViewItemStartBit", typeof(int), typeof(UserTreeViewItem2),
                new PropertyMetadata(0, new PropertyChangedCallback(OnTreeViewItemStartBitChanged)));

            //结束位
            static readonly DependencyProperty TreeViewItemEndBitProperty =
                DependencyProperty.Register("TreeViewItemEndBit", typeof(int), typeof(UserTreeViewItem2),
                new PropertyMetadata(0, new PropertyChangedCallback(OnTreeViewItemEndBitChanged)));

            //长度
            static readonly DependencyProperty TreeViewItemBitLengthProperty =
                DependencyProperty.Register("TreeViewItemBitLength", typeof(int), typeof(UserTreeViewItem2),
                new PropertyMetadata(0, null));

            //名称
            static readonly DependencyProperty TreeViewItemValueNameProperty =
                DependencyProperty.Register("TreeViewItemValueName", typeof(string), typeof(UserTreeViewItem2),
                new PropertyMetadata("TreeViewItemValueName", new PropertyChangedCallback(OnTreeViewItemValueNameChanged)));

            //实际值
            static readonly DependencyProperty TreeViewItemRealValueProperty =
                DependencyProperty.Register("TreeViewItemRealValue", typeof(int), typeof(UserTreeViewItem2),
                new PropertyMetadata(0, new PropertyChangedCallback(OnTreeViewItemRealValueChanged)));

            #endregion

            #region set/get


            /// <summary>
            /// 起始位
            /// </summary>
            public int TreeViewItemStartBit
            {
                get
                {
                    return (int)GetValue(TreeViewItemStartBitProperty);
                }
                set
                {
                    SetValue(TreeViewItemStartBitProperty, value);
                }
            }

            /// <summary>
            /// 结束位
            /// </summary>
            public int TreeViewItemEndBit
            {
                get
                {
                    return (int)GetValue(TreeViewItemEndBitProperty);
                }
                set
                {
                    SetValue(TreeViewItemEndBitProperty, value);
                }
            }

            /// <summary>
            /// 长度
            /// </summary>
            public int TreeViewItemBitLength
            {
                get
                {
                    return (int)GetValue(TreeViewItemBitLengthProperty);
                }

            }

            /// <summary>
            /// 值名称
            /// </summary>
            public string TreeViewItemValueName
            {
                get
                {
                    return (string)GetValue(TreeViewItemValueNameProperty);
                }
                set
                {
                    SetValue(TreeViewItemValueNameProperty, value);
                }
            }

            /// <summary>
            /// 实际值
            /// </summary>
            public int TreeViewItemRealValue
            {
                get
                {
                    return (int)GetValue(TreeViewItemRealValueProperty);
                }
                set
                {
                    SetValue(TreeViewItemRealValueProperty, value);
                }
            }

            #endregion


            #region 事件


            //起始位
            private static void OnTreeViewItemStartBitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                int b = Convert.ToInt32(e.NewValue);
                if (b < 0 || b > 7)
                    return;

                UserTreeViewItem2 gauge = d as UserTreeViewItem2;
                (gauge.feCbStartBit).Text = e.NewValue.ToString();

                //长度
                gauge.SetValue(TreeViewItemBitLengthProperty, gauge.TreeViewItemEndBit - gauge.TreeViewItemStartBit);
            }

            //结束位
            private static void OnTreeViewItemEndBitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                int b = Convert.ToInt32(e.NewValue);
                if (b < 0 || b > 7)
                    return;

                UserTreeViewItem2 gauge = d as UserTreeViewItem2;
                (gauge.feCbEndBit).Text = e.NewValue.ToString();

                //长度
                gauge.SetValue(TreeViewItemBitLengthProperty, gauge.TreeViewItemEndBit - gauge.TreeViewItemStartBit);
            }

            //值名称
            private static void OnTreeViewItemValueNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                UserTreeViewItem2 gauge = d as UserTreeViewItem2;
                (gauge.feTbName).Text = e.NewValue.ToString();
            }

            //实际值
            private static void OnTreeViewItemRealValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            {
                UserTreeViewItem2 gauge = d as UserTreeViewItem2;
                (gauge.feTbRealValue).Text = e.NewValue.ToString();
            }
            #endregion
        }
        #endregion
    }



}
