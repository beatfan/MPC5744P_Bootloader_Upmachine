using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BeatfanControls.DashBoards
{
    /// <summary>
    /// DashBoard1.xaml 的交互逻辑
    /// </summary>
    public partial class DashBoard1 : UserControl
    {

        public DashBoard1()
        {
            InitializeComponent();
            
        }

        #region 依赖属性
        private static readonly DependencyProperty WarnYellowStartValueProperty =
            DependencyProperty.Register("WarnYellowStartValue", typeof(double), typeof(DashBoard1),
                new PropertyMetadata(0.0, new PropertyChangedCallback(DashBoard1.OnWarnYellowStartValueChanged)));

        private static readonly DependencyProperty WarnYellowEndValueProperty =
            DependencyProperty.Register("WarnYellowEndValue", typeof(double), typeof(DashBoard1),
                new PropertyMetadata(0.0, new PropertyChangedCallback(DashBoard1.OnWarnYellowEndValueChanged)));

        private static readonly DependencyProperty UintStrProperty =
            DependencyProperty.Register("UintStr", typeof(string), typeof(DashBoard1),
                new PropertyMetadata("Uint", new PropertyChangedCallback(DashBoard1.OnUintStrChanged)));

        private static readonly DependencyProperty UintColorProperty =
            DependencyProperty.Register("UintColor", typeof(Color), typeof(DashBoard1),
                new PropertyMetadata(Colors.Black, new PropertyChangedCallback(DashBoard1.OnUintColorChanged)));

        private static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(DashBoard1),
                new PropertyMetadata(0.0, new PropertyChangedCallback(DashBoard1.OnCurrentValueChanged)));

        private static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(DashBoard1),
                new PropertyMetadata(0.0, new PropertyChangedCallback(DashBoard1.OnMinValueChanged)));

        private static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(DashBoard1),
                new PropertyMetadata(0.0, new PropertyChangedCallback(DashBoard1.OnMaxValueChanged)));
        #endregion

        #region 设置获取
        /// <summary>
        /// 警告色块起始值
        /// </summary>
        public double WarnYellowStartValue
        {
            set
            {

                SetValue(WarnYellowStartValueProperty, value);
            }
            get
            {
                return (double)GetValue(WarnYellowStartValueProperty);
            }
        }

        /// <summary>
        /// 警告色块结束值
        /// </summary>
        public double WarnYellowEndValue
        {
            set
            {
                SetValue(WarnYellowEndValueProperty, value);
            }
            get
            {
                return (double)GetValue(WarnYellowEndValueProperty);
            }
        }

        /// <summary>
        /// 单位
        /// </summary>
        public string UnitStr
        {
            set
            {
                SetValue(UintStrProperty, value);
            }
            get
            {
                return (string)GetValue(UintStrProperty);
            }
        }

        /// <summary>
        /// 单位颜色
        /// </summary>
        public Color UnitColor
        {
            set
            {
                SetValue(UintColorProperty, value);
            }
            get
            {
                return (Color)GetValue(UintColorProperty);
            }
        }

        /// <summary>
        /// 当前值
        /// </summary>
        public double CurrentValue
        {
            set
            {
                SetValue(CurrentValueProperty, value);
            }
            get
            {
                return (double)GetValue(CurrentValueProperty);
            }
        }

        /// <summary>
        /// 最大值
        /// </summary>
        public double MaxValue
        {
            set
            {
                SetValue(MaxValueProperty, value);
            }
            get
            {
                return (double)GetValue(MaxValueProperty);
            }
        }

        /// <summary>
        /// 最小值
        /// </summary>
        public double MinValue
        {
            set
            {
                SetValue(MinValueProperty, value);
            }
            get
            {
                return (double)GetValue(MinValueProperty);
            }
        }
        #endregion


        #region 通知方法
        private static void OnWarnYellowStartValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DashBoard1 depObj = d as DashBoard1;
            depObj.myDashBoard1.OptimalRangeStartValue = Convert.ToDouble(e.NewValue);
        }

        private static void OnWarnYellowEndValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DashBoard1 depObj = d as DashBoard1;
            depObj.myDashBoard1.OptimalRangeEndValue = Convert.ToDouble(e.NewValue);
        }

        private static void OnUintStrChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DashBoard1 depObj = d as DashBoard1;
            depObj.myDashBoard1.DialText = e.NewValue.ToString();
        }

        private static void OnUintColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DashBoard1 depObj = d as DashBoard1;
            depObj.myDashBoard1.DialTextColor = (Color)e.NewValue;
        }

        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DashBoard1 depObj = d as DashBoard1;
            depObj.myDashBoard1.CurrentValue = Convert.ToDouble(e.NewValue);
        }

        private static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DashBoard1 depObj = d as DashBoard1;
            depObj.myDashBoard1.MinValue = Convert.ToDouble(e.NewValue);
        }

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DashBoard1 depObj = d as DashBoard1;
            depObj.myDashBoard1.MaxValue = Convert.ToDouble(e.NewValue);
        }
        #endregion


    }
}
