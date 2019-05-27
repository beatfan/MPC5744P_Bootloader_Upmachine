using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialFunctions.Datas
{
    public class BytesColorConvert
    {
        /// <summary>
        /// 比较两个数组，不同的值背景色为Gray，相同值背景色为Transparent
        /// </summary>
        /// <param name="oldBytes"></param>
        /// <param name="newBytes"></param>
        /// <returns></returns>
        public static string[] CompareBytesToColorString(byte[] oldBytes, byte[] newBytes)
        {
            if(oldBytes==null)
                return new string[] { "Transparent", "Transparent", "Transparent", "Transparent", "Transparent", "Transparent", "Transparent", "Transparent" }; //更新颜色

            int length = newBytes.Length;
            string[] strColor = new string[length];
            for (byte i = 0; i < length; i++)
            {
                if (oldBytes[i].Equals(newBytes[i]))
                    strColor[i] = "Transparent";
                else
                    strColor[i] = "Gray";
            }

            return strColor;
        }
    }
}
