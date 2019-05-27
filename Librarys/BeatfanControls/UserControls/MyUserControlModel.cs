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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatfanControls.UserControls
{
    /// <summary>
    /// 自定义页面控件
    /// InitialControl必须调用，以注册关闭事件
    /// </summary>
    public abstract class MyUserControlModel : System.Windows.Controls.UserControl
    {

        public delegate void UserControlSizeChange_delege(object sender, SizeChangedEventArgs e);
        public delegate void UserControlLocationChange_delege(object sender, EventArgs e);

        /// <summary>
        /// 窗体大小变化委托事件
        /// </summary>
        public UserControlSizeChange_delege UserControlSizeChange;

        /// <summary>
        /// 窗体位置变化委托事件
        /// </summary>
        public UserControlLocationChange_delege UserControlLocationChange;

        //窗体位置和大小
        protected Point m_WinLocation = new Point(0, 0);
        protected Size m_WinSize = new Size(100, 30);

        /// <summary>
        /// 控件自身指针
        /// </summary>
        MyUserControlModel m_MySelf;

        /// <summary>
        /// 父窗体指针
        /// </summary>
        Window m_Root;

        /// <summary>
        /// 传入根窗体指针，用于关闭时通知控件释放资源
        /// 传入子类指针，用于操作子类
        /// 例如关闭窗体时关闭CAN，需要调用子类的方法
        /// </summary>
        /// <param name="self"></param>
        public virtual void InitialControl(Window root, MyUserControlModel self)
        {
            m_Root = root;
            m_MySelf = self;

            root.Closing += self.ControlClosing;
            root.SizeChanged += Root_SizeChanged;
            root.LocationChanged += Root_LocationChanged;
        }

        /// <summary>
        /// 获取窗体大小
        /// </summary>
        /// <returns></returns>
        public virtual Size GetWinSize()
        {
            return m_WinSize;
        }

        /// <summary>
        /// 获取窗体位置
        /// </summary>
        /// <returns></returns>
        public virtual Point GetWinLocation()
        {
            return m_WinLocation;
        }

        /// <summary>
        /// 获取父窗体
        /// </summary>
        /// <returns></returns>
        public virtual Window GetWindow()
        {
            return m_Root;
        }
        
        #region 控件隐藏处理事件
        public static readonly DependencyProperty PageVisibilyProperty =
                    DependencyProperty.Register("PageVisibily", typeof(Visibility), typeof(MyUserControlModel),
                    new PropertyMetadata(Visibility.Collapsed, new PropertyChangedCallback(OnPageVisibilyChanged)));

        public Visibility PageVisibily
        {
            get
            {
                return (Visibility)GetValue(PageVisibilyProperty);
            }
            set
            {
                SetValue(PageVisibilyProperty, value);
            }
        }

        private static void OnPageVisibilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MyUserControlModel p = d as MyUserControlModel;
            //Get access to the instance of CircularGaugeConrol whose property value changed
            switch ((Visibility)e.NewValue)
            {
                case Visibility.Collapsed:
                    if (p.m_MySelf != null)
                        p.m_MySelf.OnClosing();
                    break;
                case Visibility.Visible:
                    if (p.m_MySelf != null)
                        p.m_MySelf.OnVisible();
                    break;

                default: break;
            }
        }
        #endregion

        /// <summary>
        /// 内部控件大小变化
        /// 设置父窗体大小
        /// </summary>
        protected void SetRootSize(double contentWidth, double contentHeight)
        {
            m_Root.Width = contentWidth + 20;
            m_Root.Height = contentHeight + 10 + 60 + 30;
        }

        /// <summary>
        /// 父窗体位置变化
        /// </summary>
        private void Root_LocationChanged(object sender, EventArgs e)
        {
            m_WinLocation = new Point(((Window)sender).Left, ((Window)sender).Top);
            if (UserControlLocationChange != null)
                UserControlLocationChange(sender, e);
        }

        /// <summary>
        /// 父窗体大小变化
        /// </summary>
        private void Root_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_WinSize = new Size(((Window)sender).Width, ((Window)sender).Height);

            if (UserControlSizeChange != null)
                UserControlSizeChange(sender, e);
        }

        /// <summary>
        /// 页面关闭前需要处理的事情
        /// </summary>
        public virtual void ControlClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnClosing();
        }

        /// <summary>
        /// 可见时的操作
        /// </summary>
        protected abstract void OnVisible();

        /// <summary>
        /// 关闭前准备释放资源
        /// </summary>
        protected abstract void OnClosing();
        
    }
}
