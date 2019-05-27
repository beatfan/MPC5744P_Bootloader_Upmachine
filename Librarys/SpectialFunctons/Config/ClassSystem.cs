using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace SpecialFunctions.Config
{
    /// <summary>
    /// 系统相关的操作，如获取服务程序所在路径，获取IP等等
    /// </summary>
    public class ClassSystem
    {
        string m_ConfigPath;
        public ClassSystem(string configPath)
        {
            m_ConfigPath = configPath;
        }

        #region INI配置文件操作
        /// <summary>
        /// ini配置文件相关
        /// </summary>

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 往section中的key写值
        /// </summary>
        public void WriteConfig(string section, string key, string val)
        {
            WritePrivateProfileString(section, key, val, m_ConfigPath);
        }

        /// <summary>
        /// 读section中的key的值
        /// </summary>
        public string GetConfig(string section, string key)
        {
            StringBuilder sb = new StringBuilder();
            GetPrivateProfileString(section, key, "No", sb, 255, m_ConfigPath);
            return sb.ToString();
        }
        #endregion
    }
}
