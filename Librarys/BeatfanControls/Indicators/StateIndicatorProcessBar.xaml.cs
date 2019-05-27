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
    public partial class StateIndicatorProcessBar : UserControl
    {

        public StateIndicatorProcessBar()
        {
            InitializeComponent();
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(StateIndicator1), new FrameworkPropertyMetadata(typeof(StateIndicator1)));
        }

        #region Dependency properties

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty LabelContentProperty =
            DependencyProperty.Register("LabelContent", typeof(string), typeof(StateIndicatorProcessBar),
            new PropertyMetadata("Label", new PropertyChangedCallback(OnLabelContentChanged)));

        private static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(double), typeof(StateIndicatorProcessBar),
            new PropertyMetadata(0.3, OnCurrentValueChanged));


        private static readonly DependencyProperty CurrentValueListProperty = DependencyProperty.Register("CurrentValueList", typeof(List<double>), typeof(StateIndicatorProcessBar),
            new PropertyMetadata(new List<double>() { 50, 100 }, OnCurrentValueListChanged));


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
        /// 设置当前值
        /// 百分比，小于1
        /// </summary>
        public double CurrentValue
        {
            set { SetValue(CurrentValueProperty, value); }
            get { return (double)GetValue(CurrentValueProperty); }
        }

        /// <summary>
        /// 设置当前值
        /// 已完成值和总值
        /// </summary>
        public List<double> CurrentValueList
        {
            set { SetValue(CurrentValueListProperty, value); }
            get { return (List<double>)GetValue(CurrentValueListProperty); }
        }

        
        #endregion


        #region Methods
        private static void OnLabelContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            StateIndicatorProcessBar depObj = d as StateIndicatorProcessBar;
            depObj.lbIndicator.Content = e.NewValue;

        }



        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicatorProcessBar cpb = d as StateIndicatorProcessBar;
            double percentValue = (double)e.NewValue;

            cpb.testRunProcess.CurrentValue = percentValue;
        }

        private static void OnCurrentValueListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            StateIndicatorProcessBar cpb = d as StateIndicatorProcessBar;
            List<double> valueList = (List<double>)e.NewValue;

            cpb.testRunProcess.CurrentValueList = valueList;
        }
        #endregion




    }
}
