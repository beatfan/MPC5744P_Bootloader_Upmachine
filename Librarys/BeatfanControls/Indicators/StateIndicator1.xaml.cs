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

namespace BeatfanControls.Indicators
{
    /// <summary>
    /// StateIndicator1.xaml 的交互逻辑
    /// </summary>
    public partial class StateIndicator1 : UserControl
    {
        /// <summary>
        /// 指示状态颜色
        /// </summary>
        public enum StateIndicatorColor:uint
        {
            Gray = 0,
            Yellow,
            Green,
            Red
        }

        public StateIndicator1()
        {
            InitializeComponent();
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(StateIndicator1), new FrameworkPropertyMetadata(typeof(StateIndicator1)));
        }

        #region Dependency properties

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(string), typeof(StateIndicator1),
            new PropertyMetadata("Label", new PropertyChangedCallback(OnLabelContentChanged)));

        public static readonly DependencyProperty IndicatorColorProperty =
            DependencyProperty.Register("IndicatorColor", typeof(StateIndicatorColor), typeof(StateIndicator1),
            new PropertyMetadata(StateIndicatorColor.Yellow, new PropertyChangedCallback(OnIndicatorColorChanged)));

        public static readonly DependencyProperty IndicatorVisiableProperty =
            DependencyProperty.Register("IndicatorVisiable", typeof(bool), typeof(StateIndicator1),
            new PropertyMetadata(true, new PropertyChangedCallback(OnIndicatorVisiableChanged)));
        #endregion

        #region Wrapper properties

        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public string LabelContent
        {
            get
            {
                return (string)GetValue(LabelContentProperty);
            }
            set
            {
                SetValue(LabelContentProperty, value);
            }
        }

        /// <summary>
        /// 0灰色，1黄色，2绿色
        /// </summary>
        public StateIndicatorColor IndicatorColor
        {
            get
            {
                return (StateIndicatorColor)GetValue(IndicatorColorProperty);
            }
            set
            {
                SetValue(IndicatorColorProperty, value);
            }
        }


        /// <summary>
        /// 指示灯是否显示
        /// </summary>
        public bool IndicatorVisiable
        {
            get
            {
                return (bool)GetValue(IndicatorVisiableProperty);
            }
            set
            {
                SetValue(IndicatorVisiableProperty, value);
            }
        }
        #endregion


        #region Methods
        private static void OnLabelContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            StateIndicator1 depObj = d as StateIndicator1;
            depObj.lbIndicator.Content = e.NewValue;

        }

        private static void OnIndicatorColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            StateIndicator1 depObj = d as StateIndicator1;



            LinearGradientBrush lineBrush = new LinearGradientBrush();
            lineBrush.EndPoint = new Point(0.5, 1);
            lineBrush.StartPoint = new Point(0.5, 0);
            GradientStop gradStop1 = new GradientStop();
            gradStop1.Offset = 0.1;
            GradientStop gradStop2 = new GradientStop();
            gradStop2.Offset = 0.8;

            switch ((StateIndicatorColor)e.NewValue) //on
            {
                case StateIndicatorColor.Gray: //gray
                    {
                        gradStop1.Color = Color.FromArgb(0xFF, 0x9B, 0xAD, 0xA2);
                        gradStop2.Color = Color.FromArgb(0xFF, 0x3F, 0x41, 0x41);
                        break;
                    }
                case StateIndicatorColor.Yellow: //yellow
                    {
                        gradStop1.Color = Color.FromArgb(0xFF, 0xF8, 0xF8, 0x20);
                        gradStop2.Color = Color.FromArgb(0xFF, 0x6C, 0xB7, 0x15);
                        break;
                    }
                case StateIndicatorColor.Green: //green
                    {
                        gradStop1.Color = Color.FromArgb(0xFF, 0x1B, 0xD1, 0x62);
                        gradStop2.Color = Color.FromArgb(0xFF, 0x0C, 0x69, 0x69);
                        break;
                    }

                case StateIndicatorColor.Red: //red
                    {
                        gradStop1.Color = Color.FromArgb(0xFF, 0xF8, 0x28, 0x20);
                        gradStop2.Color = Color.FromArgb(0xFF, 0x9B, 0x2E, 0x15);
                        break;
                    }

                default: break;
            }

            lineBrush.GradientStops.Add(gradStop1);
            lineBrush.GradientStops.Add(gradStop2);
            depObj.elipIndicator.Fill = lineBrush;

        }


        private static void OnIndicatorVisiableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            StateIndicator1 depObj = d as StateIndicator1;
            depObj.elipIndicator.Visibility = ((bool)e.NewValue)?Visibility.Visible:Visibility.Hidden;

        }
        #endregion




    }
}
