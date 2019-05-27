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
    public class NumbericTextbox_HexUInt : TextBox
    {
        //int m_CurrentValue = 0;

        public NumbericTextbox_HexUInt()
        {
            //DataContext = m_CurrentValue;
            PreviewTextInput += TextBoxHex_PreviewTextInput;
            TextChanged += TextBoxHex_TextChange;
            Text = "";
        }

        public delegate void Dele_TextBoxHex_ValueChange(object sender, TextChangedEventArgs e);
        Dele_TextBoxHex_ValueChange m_TextBoxHex_ValueChange;


        uint CurrentValueHexProperty;


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
        public uint CurrentValue
        {
            get
            {
                return CurrentValueHexProperty;
            }
            set
            {
                CurrentValueHexProperty=value;
                CurrentValueChanged(CurrentValueHexProperty);
            }
        }



        /// <summary>
        /// 值改变
        /// </summary>
        public Dele_TextBoxHex_ValueChange ValueChange
        {
            get
            {
                return m_TextBoxHex_ValueChange;
            }
            set
            {
                m_TextBoxHex_ValueChange = value;
                
            }
        }



        private void CurrentValueChanged(uint data)
        {
            TextChanged -= TextBoxHex_TextChange;
            Text = data.ToString("X4");
            TextChanged += TextBoxHex_TextChange;
        }



        private void TextBoxHex_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^A-Fa-f0-9]+$"); //带小数点("[^0-9.-]+");
            TextBox tb = sender as TextBox;
            e.Handled = re.IsMatch(e.Text) || (tb.Text.Length >= 4);//不符合正则表达式或大于2个字节则表示
                
        }

        private void TextBoxHex_TextChange(object sender, TextChangedEventArgs e)
        {
            
            try
            {
                if (((TextBox)sender).Text == "")
                    return;


               this.TextChanged -= TextBoxHex_TextChange;
                ((TextBox)sender).Text = ((TextBox)sender).Text.ToUpper();
                ((TextBox)sender).SelectionStart = ((TextBox)sender).Text.Length;
                this.TextChanged += TextBoxHex_TextChange;

                //uint
                uint result1 = 0;
                string str = ((TextBox)sender).Text;
                
                if (str.Length % 2 == 1)
                    str = "0"+str;
                
                //0x不计入
                for (int i = 0; i < str.Length - 1; i += 2)
                {
                    result1 <<= 8; //左移一个字节，低字节表示高位
                    result1 += Convert.ToUInt16(str.Substring(i, 2), 16);
                }
                
                CurrentValueHexProperty = result1;
                
                if (m_TextBoxHex_ValueChange != null)
                    m_TextBoxHex_ValueChange(this,e);
            }
            catch
            {

            }
            
        }
    }
}
