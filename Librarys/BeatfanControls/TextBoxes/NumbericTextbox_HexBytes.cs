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
    public class NumbericTextbox_HexBytes : TextBox
    {
        //int m_CurrentValue = 0;

        public NumbericTextbox_HexBytes()
        {
            //DataContext = m_CurrentValue;
            PreviewTextInput += TextBoxHex_PreviewTextInput;
            TextChanged += TextBoxHex_TextChange;
            Text = "";
            
        }

        public delegate void Dele_TextBoxHex_ValueChange(object sender, TextChangedEventArgs e);
        Dele_TextBoxHex_ValueChange m_TextBoxHex_ValueChange;

        /// <summary>
        /// 不使用依赖属性
        /// </summary>
        byte[] CurrentBytesValueProperty;

       

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
        public byte[] CurrentValueBytes
        {
            get
            {
                return CurrentBytesValueProperty;
            }
            set
            {
                CurrentBytesValueProperty=value;
                CurrentValueBytesChanged(CurrentBytesValueProperty);
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


        /// <summary>
        /// 值变化
        /// </summary>
        private void CurrentValueBytesChanged(byte[] data)
        {
            TextChanged -= TextBoxHex_TextChange;
            Text = BytesToString(data);
            Text = TextInsertSpace(Text); //插入空格

            TextChanged += TextBoxHex_TextChange;
        }


        private void TextBoxHex_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^A-Fa-f0-9]+$"); //带小数点("[^0-9.-]+");
            e.Handled = re.IsMatch(e.Text);
            //if (((TextBox)sender).Text.Length >= 4) //不允许超过4个Hex数据
            //    e.Handled = true;
        }

        private void TextBoxHex_TextChange(object sender, TextChangedEventArgs e)
        {
            
            try
            {
                if (((TextBox)sender).Text == "")
                    return;

                TextBox tb = sender as TextBox;
                this.TextChanged -= TextBoxHex_TextChange;
                tb.Text = tb.Text.ToUpper();
                tb.Text = tb.Text.Replace(" ", ""); //删除空格
                CurrentBytesValueProperty = StringToBytes(tb.Text); //设置值
                
                tb.Text = TextInsertSpace(tb.Text); //插入空格

                this.TextChanged += TextBoxHex_TextChange;

                tb.SelectionStart = tb.Text.Length;
                
                if (m_TextBoxHex_ValueChange != null)
                    m_TextBoxHex_ValueChange(this,e);
            }
            catch
            {

            }
            
        }

        /// <summary>
        /// 每隔4个字节插入空格
        /// </summary>
        private string TextInsertSpace(string str)
        {
            string tmp = string.Empty;

            //每4个bytes，用空格隔开
            for (int i = 0; i < str.Length; i++)
            {
                tmp += str.Substring(i, 1);
                if ((i + 1) % 8 == 0 && i > 0 && i < str.Length - 1)
                    tmp += " ";
            }
            return tmp;
        }

        /// <summary>
        /// 16进制字符串转bytes
        /// </summary>
        private byte[] StringToBytes(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            //bytes
            List<byte> result2 = new List<byte>();
                            
            if (str.Length % 2 == 1)
                str = "0" + str;

            //0x不计入
            for (int i = 0; i < str.Length - 1; i += 2)
            {
                result2.Add(Convert.ToByte(str.Substring(i, 2), 16));
            }

            return result2.ToArray();
        }

        /// <summary>
        /// bytes转16进制字符串
        /// </summary>
        private string BytesToString(byte[] bs)
        {
            string str = string.Empty;
            for (int i = 0; i < bs.Length; i++)
            {
                str+=bs[i].ToString("X2");
            }
            return str;
        }
    }
}
