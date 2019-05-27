using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeatfanControls.Labels
{
    /// <summary>
    /// 布尔颜色标签
    /// True为绿色
    /// False为红色
    /// </summary>
    public class BoolLabel : Label
    {
        public BoolLabel()
        {
            //DataContext = m_CurrentValue;
            Foreground = System.Windows.Media.Brushes.Green;
        }


        public static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue",
        typeof(bool), typeof(BoolLabel), new FrameworkPropertyMetadata(true, new PropertyChangedCallback(CurrentValueChanged)));

        private new string Content
        {
            set { base.Content = value; }
            get { return (string)base.Content;  }
        }

        /// <summary>
        /// 当前值
        /// </summary>
        public bool CurrentValue
        {
            get
            {
                return (bool)GetValue(CurrentValueProperty);
            }
            set
            {
                SetValue(CurrentValueProperty, value);
            }
        }

        private static void CurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            NumbericLabel_Int gauge = d as NumbericLabel_Int;
            gauge.Foreground = ((bool)e.NewValue)?System.Windows.Media.Brushes.Green: System.Windows.Media.Brushes.Red;
        }


    }
}
