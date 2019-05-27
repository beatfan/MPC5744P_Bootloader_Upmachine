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
    public class NumbericTextbox_Int : TextBox
    {
        //int m_CurrentValue = 0;

        public NumbericTextbox_Int()
        {
            //DataContext = m_CurrentValue;
            PreviewTextInput += TextBoxInt_PreviewTextInput;
            TextChanged += TextBoxInt_TextChange;
            Text = "0";
            
        }

        public delegate void Dele_TextBoxInt_ValueChange(object sender, TextChangedEventArgs e);
        Dele_TextBoxInt_ValueChange m_TextBoxInt_ValueChange;
        
        int CurrentValueIntProperty;

        

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
        public int CurrentValue
        {
            get
            {
                return CurrentValueIntProperty;
            }
            set
            {
                CurrentValueIntProperty=value;
                CurrentValueChanged(CurrentValueIntProperty);
            }
        }

        /// <summary>
        /// 值改变
        /// </summary>
        public Dele_TextBoxInt_ValueChange ValueChange
        {
            get
            {
                return m_TextBoxInt_ValueChange;
            }
            set
            {
                m_TextBoxInt_ValueChange = value;
            }
        }

        private void CurrentValueChanged(int data)
        {
            TextChanged -= TextBoxInt_TextChange;
            Text = data.ToString();
            TextChanged += TextBoxInt_TextChange;
        }


        private void TextBoxInt_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9-]+"); //带小数点("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void TextBoxInt_TextChange(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (((TextBox)sender).Text == "")
                    return;
                
                CurrentValueIntProperty = Convert.ToInt32(Text);
                

                if (m_TextBoxInt_ValueChange != null)
                    m_TextBoxInt_ValueChange(this, e);
            }
            catch
            { }
        }
    }
}
