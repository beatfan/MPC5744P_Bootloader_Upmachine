using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecialFunctions.Converter
{
    /// <summary>
    /// 字符串与其它之间的转换
    /// </summary>
    public class MyStringConverter
    {

        /// <summary>
        /// 16进制字符串转Uint32，字符串前含0x，字符串不足偶数则前面补0
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static UInt16 HexStringToUInt16(string hexString)
        {

            UInt16 result = 0;
            if (hexString.Length % 2 == 1)
                hexString = "0x0" + hexString.Substring(2, hexString.Length - 2);

            //0x不计入
            for (int i = 2; i < hexString.Length - 1; i += 2)
            {
                result <<= 8; //左移一个字节，低字节表示高位
                result += Convert.ToUInt16(hexString.Substring(i, 2), 16);
            }
            return result;
        }

        /// <summary>
        /// 16进制字符串转Uint32，字符串前含0x，字符串不足偶数则前面补0
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static UInt32 HexStringToUint(string hexString)
        {

            UInt32 result = 0;
            if (hexString.Length % 2 == 1)
                hexString = "0x0" + hexString.Substring(2, hexString.Length - 2);

            //0x不计入
            for (int i = 2; i < hexString.Length - 1; i += 2)
            {
                result <<= 8; //左移一个字节，低字节表示高位
                result += Convert.ToUInt32(hexString.Substring(i, 2), 16);
            }
            return result;
        }

        /// <summary>
        /// Uint32转16进制字符串，字符串前含有0x
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string UintToHexString(UInt32 num)
        {
            string result = "0x" + num.ToString("X4");
            return result;
        }

        /// <summary>
        /// 16进制字符串转ushort，字符串前含0x，字符串不足偶数则前面补0
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static ushort HexStringToUshort(string hexString)
        {

            ushort result = 0;
            if (hexString.Length % 2 == 1)
                hexString = "0x0" + hexString.Substring(2, hexString.Length - 2);

            //0x不计入
            for (int i = 2; i < hexString.Length - 1; i += 2)
            {
                result <<= 8; //左移一个字节，低字节表示高位
                result += Convert.ToUInt16(hexString.Substring(i, 2), 16);
            }
            return result;
        }

        /// <summary>
        /// ushort转16进制字符串，字符串前含有0x
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string UshortToHexString(ushort num)
        {
            string result = "0x" + num.ToString("X2");
            return result;
        }

        /// <summary>
        /// 16进制字符串转short，字符串前含0x，字符串不足偶数则前面补0
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static short HexStringToShort(string hexString)
        {

            short result = 0;
            if (hexString.Length % 2 == 1)
                hexString = "0x0" + hexString.Substring(2, hexString.Length - 2);

            //0x不计入
            for (int i = 2; i < hexString.Length - 1; i += 2)
            {
                result <<= 8; //左移一个字节，低字节表示高位
                result += Convert.ToInt16(hexString.Substring(i, 2), 16);
            }
            return result;
        }

        /// <summary>
        /// ushort转16进制字符串，字符串前含有0x
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string ShortToHexString(short num)
        {
            string result = "0x" + num.ToString("X2");
            return result;
        }

        /// <summary>
        /// byte数组转16进制字符串，数组下标低的表示高位，字符串前含有0x
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string BytesToHexString(byte[] nums)
        {
            string result = "0x" + MyBitConvert(nums).Replace("-", " ");

            return result;
        }

        /// <summary>
        /// byte数组转16进制字符串，数组下标低的表示高位，字符串前含有0x
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string BytesToHexStringNoX(byte[] nums)
        {
            string result = MyBitConvert(nums).Replace("-", "");

            return result;
        }

        /// <summary>
        /// 字符串转byte数组，数组下标低的表示高位
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToBytes(string hexString)
        {
            List<byte> nums = new List<byte>();
            if (hexString.Length % 2 == 1)
                hexString = "0x0" + hexString.Substring(2, hexString.Length - 2);
            

            //0x不计入
            for (int i = 2; i < hexString.Length - 1; i += 2)
            {
                nums.Add(Convert.ToByte(hexString.Substring(i, 2), 16));
            }

            return nums.ToArray();
        }

        /// <summary>
        /// byte数组转16进制字符串，数组下标低的表示高位，字符串没字节空格隔开
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string BytesToHexSpaceString(byte[] nums)
        {
            string result = MyBitConvert(nums).Replace("-"," ");

            return result;
        }

        /// <summary>
        /// 字符串转byte数组，数组下标低的表示高位，用空隔隔开
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexSpaceStringToBytes(string hexString)
        {
            List<byte> nums = new List<byte>();
            hexString = hexString.Replace(" ", "");
            if (hexString.Length % 2 == 1)
                hexString = "0x0" + hexString;
            else
                hexString = "0x" + hexString;

            //0x不计入
            for (int i = 2; i < hexString.Length - 1; i += 2)
            {
                nums.Add(Convert.ToByte(hexString.Substring(i, 2), 16));
            }

            return nums.ToArray();
        }



        /// <summary>
        /// Uint32数组转字符串数据，不同Uint32之间用，隔开
        /// </summary>
        /// <param name="numList"></param>
        /// <returns></returns>
        public static string UintListToHexString(List<UInt32> numList)
        {
            try
            {
                string result = string.Empty;
                for (int i = 0; i < numList.Count; i++)
                {
                    result += "0x" + numList[i].ToString("X2") + ",";
                }
                result = result.Substring(0, result.Length - 1); //去除最后一个,
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 16进制字符串转Uint32数据，字符串以，隔开，每个隔开的字符串前含0x
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static List<UInt32> HexStringToUintList(string hexString)
        {
            string[] newStrings = hexString.Split(',');
            List<UInt32> result = new List<uint>();
            for (int i = 0; i < newStrings.Length; i++)
            {
                result.Add(HexStringToUint(newStrings[i]));
            }
            return result;
        }

        /// <summary>
        /// Bool数组转字符串数据，不同Bool之间用，隔开
        /// </summary>
        /// <param name="boolList"></param>
        /// <returns></returns>
        public static string BoolListToHexString(List<bool> boolList)
        {
            try
            {
                string result = string.Empty;
                for (int i = 0; i < boolList.Count; i++)
                {
                    result += boolList[i].ToString() + ",";
                }
                result = result.Substring(0, result.Length - 1); //去除最后一个,
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 16进制字符串转Uint32数据，字符串以，隔开，每个隔开的字符串前含0x
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static List<bool> HexStringToBoolList(string hexString)
        {
            string[] newStrings = hexString.Split(',');
            List<bool> result = new List<bool>();
            for (int i = 0; i < newStrings.Length; i++)
            {
                result.Add(Convert.ToBoolean(newStrings[i]));
            }
            return result;
        }

        /// <summary>
        /// byte数组转字符串，带-
        /// 高位在前，低位在后
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string MyBitConvert(byte[] bytes)
        {
            if (bytes == null)
                return string.Empty;
            string str = string.Empty;
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (i != 0)
                {
                    str += bytes[i].ToString("X2") + "-";
                }
                else
                    str += bytes[i].ToString("X2");
            }

            return str;
        }
    }
}
