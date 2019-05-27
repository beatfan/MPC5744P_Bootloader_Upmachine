using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SpecialFunctions.Encryptions
{
    /// <summary>
    /// 加密，用于密码，哈希值
    /// </summary>
    public class HashEncryptions
    {
        /// <summary>
        /// 将明文密码加密
        /// </summary>
        /// <param name="passwd"></param>
        /// <returns></returns>
        public static string EncryptPasswd(string passwd)
        {
            string str = EncryptHash(SHA1_Hash(passwd));
            return str;
        }

        /// <summary>
        /// 将输入密码与打乱的哈希密码比较
        /// </summary>
        /// <param name="passwd"></param>
        /// <param name="encryptHash"></param>
        /// <returns></returns>
        public static bool ComparePasswd(string inputPasswd, string encryptHash)
        {
            bool result = false;
            string str = EncryptHash(SHA1_Hash(inputPasswd));
            if (str == encryptHash)
                result = true;
            return result;
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="str_md5_in"></param>
        /// <returns></returns>
        private static string MD5_Hash(string str_md5_in)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes_md5_in = UTF8Encoding.Default.GetBytes(str_md5_in);
            byte[] bytes_md5_out = md5.ComputeHash(bytes_md5_in);
            string str_md5_out = BitConverter.ToString(bytes_md5_out).Replace("-","");
            //str_md5_out = str_md5_out.Replace("-", "");
            return str_md5_out;
        }

        /// <summary>
        /// SHA1
        /// </summary>
        /// <param name="str_sha1_in"></param>
        /// <returns></returns>
        private static string SHA1_Hash(string str_sha1_in)
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_sha1_in = UTF8Encoding.Default.GetBytes(str_sha1_in);
            byte[] bytes_sha1_out = sha1.ComputeHash(bytes_sha1_in);
            string str_sha1_out = BitConverter.ToString(bytes_sha1_out).Replace("-", "");
            //str_sha1_out = str_sha1_out.Replace("-", "");
            return str_sha1_out;
        }

        /// <summary>
        /// 将哈希值打乱加密
        /// </summary>
        /// <param name="str_hash"></param>
        /// <returns></returns>
        private static string EncryptHash(string str_hash_fore)
        {
            //将最后5个与前面的交换
            string temp = str_hash_fore.Substring(str_hash_fore.Length - 5,5) + str_hash_fore.Substring(0, str_hash_fore.Length - 5) ;

            return temp;
        }

        /// <summary>
        /// 将打乱的哈希密码还原为正确的哈希值
        /// </summary>
        /// <param name="str_hash_back"></param>
        /// <returns></returns>
        private static string DncryptHash(string str_hash_back)
        {
            //将前面5个与后面的交换
            string temp = str_hash_back.Substring(5, str_hash_back.Length - 5) + str_hash_back.Substring(0, 5);

            return temp;
        }

    }
}
