using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeatfanControls.Labels
{
    public class NumbericLabel_Int : Label
    {
        public NumbericLabel_Int()
        {
            //DataContext = m_CurrentValue;
            Content = "0";
        }


        public static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue",
        typeof(int), typeof(NumbericLabel_Int), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(CurrentValueChanged)));

        private new string Content
        {
            set { base.Content = value; }
            get { return (string)base.Content;  }
        }

        /// <summary>
        /// 当前显示数值
        /// </summary>
        public int CurrentValue
        {
            get
            {
                return (int)GetValue(CurrentValueProperty);
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
            gauge.Content = e.NewValue.ToString();
        }


    }
}
