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

namespace BeatfanControls.Thermometers
{
    /// <summary>
    /// Thermometer.xaml 的交互逻辑
    /// </summary>
    public partial class Thermometer1 : UserControl
    {
        const double defaultFontSize = 12;  //默认字体大小
        const double defaultWidth = 50; //控件默认宽度
        const double defaultHeight = 150; //控件默认高度
        const double maxHeight = 100; //最大高度为100

        public Thermometer1()
        {
            InitializeComponent();
            valueStr.FontSize = defaultFontSize;
            unitStr.FontSize = defaultFontSize;
        }

        #region 依赖属性

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        private static readonly DependencyProperty UintStrProperty =
            DependencyProperty.Register("UintStr", typeof(string), typeof(Thermometer1),
                new PropertyMetadata("Uint", new PropertyChangedCallback(OnUintStrChanged)));

        private static readonly DependencyProperty UintColorProperty =
            DependencyProperty.Register("UintColor", typeof(Brush), typeof(Thermometer1),
                new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnUintColorChanged)));

        private static readonly DependencyProperty ValueColorProperty =
            DependencyProperty.Register("ValueColor", typeof(Brush), typeof(Thermometer1),
                new PropertyMetadata(Brushes.Black, new PropertyChangedCallback(OnValueColorChanged)));

        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(Thermometer1),
            new PropertyMetadata(100.0, new PropertyChangedCallback(Thermometer1.OnCurrentValue1Changed)));

        private static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(Thermometer1),
        new PropertyMetadata(0.0, new PropertyChangedCallback(Thermometer1.OnMinValueChanged)));

        private static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(Thermometer1),
                new PropertyMetadata(0.0, new PropertyChangedCallback(Thermometer1.OnMaxValueChanged)));
        #endregion

        #region 设定获取
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
        public Brush UnitColor
        {
            set
            {
                SetValue(UintColorProperty, value);
            }
            get
            {
                return (Brush)GetValue(UintColorProperty);
            }
        }

        /// <summary>
        /// 值颜色
        /// </summary>
        public Brush ValueColor
        {
            set
            {
                SetValue(ValueColorProperty, value);
            }
            get
            {
                return (Brush)GetValue(ValueColorProperty);
            }
        }

        public double CurrentValue
        {
            get
            {
                return (double)GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
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

        #region 值变化通知
        private static void OnUintStrChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thermometer1 depObj = d as Thermometer1;
            depObj.unitStr.Content = e.NewValue.ToString();
        }

        private static void OnUintColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thermometer1 depObj = d as Thermometer1;
            depObj.unitStr.Foreground = (Brush)e.NewValue;
        }

        private static void OnValueColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thermometer1 depObj = d as Thermometer1;
            depObj.valueStr.Foreground = (Brush)e.NewValue;
        }

        private static void OnCurrentValue1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            Thermometer1 depObj = d as Thermometer1;

            double heig = ((double)(depObj.CurrentValue - depObj.MinValue) / (depObj.MaxValue - depObj.MinValue) * maxHeight);
            heig = heig > 100 ? 100 : heig; //最大不能超过100
            depObj.valueStr.Content = e.NewValue.ToString();
            depObj.ThermoFore.Data = Geometry.Parse("m 14,100 a 10,10 0 1 0 12,0 l 0," + "-" + heig + " l -12,0 z");

        }
        
        private static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thermometer1 depObj = d as Thermometer1;
            
        }

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Thermometer1 depObj = d as Thermometer1;
            
        }
        #endregion
        
    }
}
