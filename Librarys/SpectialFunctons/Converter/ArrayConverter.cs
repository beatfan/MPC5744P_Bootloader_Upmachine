using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFunctions.Converter
{
    /// <summary>
    /// 数组转换
    /// </summary>
    public class ArrayConverter
    {
        /// <summary>
        /// 数组复制
        /// 从0开始，长度length
        /// source和dest的长度不小于length
        /// </summary>
        public static bool ArrayCopy<T>(T[] source, ref T[] dest, int length)
        {
            if (source.Length < length || dest.Length < length)
                return false;

            for (int i = 0; i < length; i++)
                dest[i] = source[i];

            return true;
        }

        /// <summary>
        /// 数组复制
        /// dest从0开始，长度length
        /// source从sourceStartIndex开始，长度不小于sourceStartIndex+length
        /// </summary>
        public static bool ArrayCopy<T>(T[] source, ref T[] dest, int sourceStartIndex, int length)
        {
            if (source.Length < length || dest.Length < length)
                return false;

            for (int i = 0; i < length; i++)
                dest[i] = source[i];

            return true;
        }
    }
}
