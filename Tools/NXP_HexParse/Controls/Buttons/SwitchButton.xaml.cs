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
using System.Windows.Media.Animation;

namespace MotorOffLineTester.Controls.Buttons
{
    /// <summary>
    /// SwitchButton.xaml 的交互逻辑
    /// </summary>
    public partial class SwitchButton : UserControl
    {

        /// <summary>
        /// 0统统表示第一个的为绿色背景，绿色内容
        /// 1表示第二个，红色背景，红色内容
        /// 打开后变红，表示可关闭
        /// 关闭后变绿，表示可开启
        /// </summary>
        public SwitchButton()
        {
            InitializeComponent();

            m_BtnBackgroundImgs[0] = "MotorOffLineTester;component/Pictures/打开按钮.png";
            m_BtnBackgroundImgs[1] = "MotorOffLineTester;component/Pictures/关闭按钮.png";
            
            m_BtnBackgroundColor[0] = Brushes.Green;
            m_BtnBackgroundColor[1] = Brushes.Red;


            //默认
            btnStart.DataContext = m_BtnBackgroundImgs[0];
            lbStart.Foreground = m_BtnBackgroundColor[0];
        }

        /// <summary>
        /// 点击事件保存
        /// </summary>
        RoutedEventHandler m_BtnClick;

        /// <summary>
        /// 开启关闭的标签内容,0绿色内容
        /// </summary>
        private string[] m_BtnOnOffContents = new string[2];

        /// <summary>
        /// 开启关闭的背景图片，0绿色图片
        /// </summary>
        private string[] m_BtnBackgroundImgs = new string[2];

        /// <summary>
        /// 开启关闭的背景颜色片，0绿色
        /// </summary>
        private Brush[] m_BtnBackgroundColor = new Brush[2];

        #region Dependency properties

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty LabelGreenContentProperty =
            DependencyProperty.Register("LabelGreenContent", typeof(string), typeof(SwitchButton),
            new PropertyMetadata("LabelGreen", new PropertyChangedCallback(SwitchButton.OnLabelGreenContentChanged)));

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty LabelRedContentProperty =
            DependencyProperty.Register("LabelRedContent", typeof(string), typeof(SwitchButton),
            new PropertyMetadata("LabelRed", new PropertyChangedCallback(SwitchButton.OnLabelRedContentChanged)));

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty IsEnableProperty = DependencyProperty.Register("IsEnable",
        typeof(bool), typeof(SwitchButton), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(OnIsEnableChanged)));


        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty IsButtonGreenProperty = DependencyProperty.Register("IsButtonGreen",
        typeof(bool), typeof(SwitchButton), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsButtonGreenChanged)));


        #endregion



        // A property changed callback (optional)   

        


        #region Wrapper properties

        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public string LabelGreenContent
        {
            get
            {
                return (string)GetValue(LabelGreenContentProperty);
            }
            set
            {
                SetValue(LabelGreenContentProperty, value);
            }
        }

        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public string LabelRedContent
        {
            get
            {
                return (string)GetValue(LabelRedContentProperty);
            }
            set
            {
                SetValue(LabelRedContentProperty, value);
            }
        }


        ///// <summary>
        ///// 是否使能
        ///// </summary>
        public bool IsEnable
        {
            get { return (bool)GetValue(SwitchButton.IsEnableProperty); }

            set { SetValue(SwitchButton.IsEnableProperty, value); }

        }

        /// <summary>
        /// 按钮是否绿色
        /// </summary>
        public bool IsButtonGreen
        {
            get { return (bool)GetValue(SwitchButton.IsButtonGreenProperty); }

            set { SetValue(SwitchButton.IsButtonGreenProperty, value); }

        }

        /// <summary>
        /// 是否默认按钮
        /// </summary>
        public bool IsDefault
        {
            set { btnStart.IsDefault = value; }
            get { return false; }
        }

        public RoutedEventHandler BtnClick
        {
            set
            {
                m_BtnClick = value;
            }
            get
            {
                return m_BtnClick;
            }

        }
        #endregion


        #region Methods
        private static void OnLabelGreenContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            SwitchButton gauge = d as SwitchButton;
            gauge.m_BtnOnOffContents[0] = e.NewValue.ToString();
        }

        private static void OnLabelRedContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            SwitchButton gauge = d as SwitchButton;
            gauge.m_BtnOnOffContents[1] = e.NewValue.ToString();
        }
        
        private static void OnIsEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            SwitchButton gauge = d as SwitchButton;
            gauge.btnStart.IsEnabled = (bool)e.NewValue;
            gauge.btnStart.Opacity = gauge.btnStart.IsEnabled ? 1 : 0.3;

        }


        private static void OnIsButtonGreenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SwitchButton gauge = d as SwitchButton;

            //状态与实际颜色相反，表示点击可进行的操作，例如开启状态下是红色，表示点击可关闭
            int index = Convert.ToInt32(!(bool)e.NewValue);

            gauge.lbStart.Content = gauge.m_BtnOnOffContents[index];
            //修改按钮图片
            gauge.btnStart.DataContext = gauge.m_BtnBackgroundImgs[index];
            gauge.lbStart.Foreground = gauge.m_BtnBackgroundColor[index];

        }

        #endregion

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            m_BtnClick(sender, e);
        }

        #region 开始按钮的鼠标放上去变换
        private void btnStart_MouseEnter(object sender, MouseEventArgs e)
        {
            
            Button my = (Button)sender as Button;
            double centerX = my.ActualWidth / 2;
            double centerY = my.ActualHeight / 2;
            // my.BorderBrush = isStarted ? Brushes.Red : Brushes.Green;
            // my.BorderThickness = new Thickness(2.0);
            //my.Width -= 2;
            //my.Height -= 2;
            

            my.Opacity = 0.7;

            //动画组
            TransformGroup tfg = new TransformGroup();
            

            //缩放
            ScaleTransform stf = new ScaleTransform();
            stf.CenterX = centerX;
            stf.CenterY = centerY;
            stf.ScaleX = 0.9;
            stf.ScaleY = 0.9;
            
            tfg.Children.Add(stf);
            //stf.BeginAnimation(ScaleTransform.ScaleXProperty, dbAscendingScale);
            //stf.BeginAnimation(ScaleTransform.ScaleYProperty, dbAscendingScale);

            //旋转180度
            RotateTransform rot = new RotateTransform();
            rot.CenterX = centerX;
            rot.CenterY = centerY;
            rot.Angle = 180;
            
            //dbAscending.RepeatBehavior = RepeatBehavior.Forever; //重复
            tfg.Children.Add(rot);
            //rot.BeginAnimation(RotateTransform.AngleProperty, dbAscendingRotate);

            my.RenderTransform = tfg;

            ////缩放动画，从1,缩放到0.9
            //DoubleAnimation dbAscendingScale = new DoubleAnimation(1, 0.9, new Duration(TimeSpan.FromSeconds(0.1)));
            ////旋转动画，旋转0.2s，旋转属性为角度，值为180
            //DoubleAnimation dbAscendingRotate = new DoubleAnimation(0, 180, new Duration(TimeSpan.FromSeconds(0.1)));

            ////执行动画
            //tfg.Children[0].BeginAnimation(ScaleTransform.ScaleXProperty, dbAscendingScale);
            //tfg.Children[0].BeginAnimation(ScaleTransform.ScaleYProperty, dbAscendingScale);
            //tfg.Children[1].BeginAnimation(RotateTransform.AngleProperty, dbAscendingRotate);

            lbStart.Visibility = Visibility.Visible;
            //ss++;
        }

        private void btnStart_MouseLeave(object sender, MouseEventArgs e)
        {
            Button my = (Button)sender as Button;
            double centerX = my.ActualWidth / 2;
            double centerY = my.ActualHeight / 2;
            //my.BorderBrush = null;
            //my.BorderThickness = new Thickness(0.0);
            

            //动画组
            TransformGroup tfg = new TransformGroup();

            //缩放
            ScaleTransform stf = new ScaleTransform();
            stf.CenterX = centerX;
            stf.CenterY = centerY;
            stf.ScaleX = 1;
            stf.ScaleY = 1;
            
            tfg.Children.Add( stf);
            //stf.BeginAnimation(ScaleTransform.ScaleXProperty, dbAscendingScale);
            //stf.BeginAnimation(ScaleTransform.ScaleYProperty, dbAscendingScale);


            //旋转返回
            RotateTransform rot = new RotateTransform();
            rot.CenterX = centerX;
            rot.CenterY = centerY;
            rot.Angle = 0;
            
            tfg.Children.Add(rot);
            //dbAscending.RepeatBehavior = RepeatBehavior.Forever; //重复
            //rot.BeginAnimation(RotateTransform.AngleProperty, dbAscending);

            my.RenderTransform = tfg;

            ////缩放动画，从0.9放大到1
            //DoubleAnimation dbAscendingScale = new DoubleAnimation(0.9, 1, new Duration(TimeSpan.FromSeconds(0.1)));
            ////旋转动画，旋转0.2s，旋转属性为角度，值为0
            //DoubleAnimation dbAscendingRotate = new DoubleAnimation(180, 360, new Duration(TimeSpan.FromSeconds(0.1)));

            ////执行动画
            //tfg.Children[0].BeginAnimation(ScaleTransform.ScaleXProperty, dbAscendingScale);
            //tfg.Children[0].BeginAnimation(ScaleTransform.ScaleYProperty, dbAscendingScale);
            //tfg.Children[1].BeginAnimation(RotateTransform.AngleProperty, dbAscendingRotate);

            
            my.Opacity = 1;

            lbStart.Visibility = Visibility.Hidden;
        }
        #endregion

        
    }
}
