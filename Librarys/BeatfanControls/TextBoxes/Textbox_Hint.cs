using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeatfanControls.TextBoxes
{
    /// <summary>
    /// 数字输入控件
    /// </summary>
    public class Textbox_Hint : TextBox
    {
        //int m_CurrentValue = 0;

        public Textbox_Hint()
        {
            //DataContext = m_CurrentValue;
            PreviewTextInput += TextBoxInt_PreviewTextInput;
            TextChanged += TextBoxHint_TextChange;
            Text = "";
        }


        public static readonly DependencyProperty HintProperty = DependencyProperty.Register("CurrentValue_Textbox_Hint_Hint",
        typeof(string), typeof(Textbox_Hint), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(HintChanged)));

        public static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue_Textbox_Hint",
        typeof(int), typeof(Textbox_Hint), new FrameworkPropertyMetadata(0, new PropertyChangedCallback(CurrentValueChanged)));

        #region 屏蔽父类属性
        /// <summary>
        /// 在属性设计器中隐藏文本属性
        /// </summary>
        private new string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }
        #endregion


        /// <summary>
        /// 提示
        /// </summary>
        public string Hint
        {
            get
            {
                return (string)GetValue(HintProperty);
            }
            set
            {
                SetValue(HintProperty, value);
            }
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


        private static void HintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            Textbox_Hint gauge = d as Textbox_Hint;
            gauge.Text = e.NewValue.ToString();
        }

        private static void CurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            Textbox_Hint gauge = d as Textbox_Hint;
            gauge.Text = e.NewValue.ToString();
        }


        private void TextBoxInt_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9-]+"); //带小数点("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void TextBoxHint_TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (((TextBox)sender).Text == "")
                    return;

                CurrentValue = Convert.ToInt32(Text);
                
            }
            catch
            { }
        }
    }
}
