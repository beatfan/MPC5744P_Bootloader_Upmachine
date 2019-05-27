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
    public class NumbericTextbox_Double : TextBox
    {
        //int m_CurrentValue = 0;

        public NumbericTextbox_Double()
        {
            //DataContext = m_CurrentValue;
            PreviewTextInput += TextBoxDouble_PreviewTextInput;
            TextChanged += TextBoxDouble_TextChange;
            Text = "0.0";
            
        }
        double CurrentValueDoubleProperty;
        

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
        /// 当前显示数值
        /// </summary>
        public double CurrentValue
        {
            get
            {
                return CurrentValueDoubleProperty;
            }
            set
            {
                CurrentValueDoubleProperty=value;
                CurrentValueChanged(CurrentValueDoubleProperty);
            }
        }


        private void CurrentValueChanged(double data)
        {
            TextChanged -= TextBoxDouble_TextChange;
            Text = data.ToString();
            TextChanged += TextBoxDouble_TextChange;
        }


        private void TextBoxDouble_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9.-]+"); //带小数点("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void TextBoxDouble_TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                CurrentValueDoubleProperty = Convert.ToDouble(Text);
            }
            catch { }
        }

       
    }
}
