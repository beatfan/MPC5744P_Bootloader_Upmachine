using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFunctions.Logs
{

    /// <summary>
    /// 日志相关的类，一般是写日志
    /// </summary>
    public class NormalLog
    {
        string m_DefaultErrorLogDir;
        static string m_CreatingDate = DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");
        static string m_DefaultErrorLogPath;

        bool m_IsHide = false;

        public NormalLog(string fatherDirectory)
        {
            if (!Directory.Exists(fatherDirectory))
                Directory.CreateDirectory(fatherDirectory);

            m_DefaultErrorLogDir = fatherDirectory + "\\ErrorLog";
            m_DefaultErrorLogPath = m_DefaultErrorLogDir + "\\ErrorLog_" + m_CreatingDate + ".log";
        }

        /// <summary>
        /// 隐藏日志文件夹
        /// </summary>
        /// <param name="hide"></param>
        public void HideErrorLog(bool hide)
        {
            if (hide == m_IsHide)
                return;

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            
            p.StartInfo.CreateNoWindow = true;         // 不创建新窗口    
            p.StartInfo.UseShellExecute = false;       //不启用shell启动进程  
            p.StartInfo.RedirectStandardInput = true;  // 重定向输入    
            p.StartInfo.RedirectStandardOutput = true; // 重定向标准输出    
            p.StartInfo.RedirectStandardError = true;  // 重定向错误输出

            p.Start();

            string cmdInfo;
            
            if (hide)
            {
                cmdInfo = "attrib +h +s " + m_DefaultErrorLogDir;
                m_IsHide = true;
            }
            else
            {
                cmdInfo = "attrib -h -s " + m_DefaultErrorLogDir;
                m_IsHide = false;
            }
            p.StandardInput.WriteLine(cmdInfo + "&exit");
            p.StandardInput.AutoFlush = true;

        }

        /// <summary>
        /// 错误日志
        /// errorLevel 故障等级
        /// errorStr 故障信息
        /// </summary>
        public void WriteErrorLog(byte errorLevel, string errorStr)
        {

            if (!Directory.Exists(m_DefaultErrorLogDir))
                Directory.CreateDirectory(m_DefaultErrorLogDir);

            string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                "  【错误等级:" + errorLevel.ToString() + "】 " + "错误信息:" + errorStr + "\r\n\r\n";
            File.AppendAllText(m_DefaultErrorLogPath, str);

        }

        /// <summary>
        /// 错误日志
        /// errorStr 故障代码信息
        /// </summary>
        public void WriteErrorLog(string errorStr)
        {
            if (!Directory.Exists(m_DefaultErrorLogDir))
                Directory.CreateDirectory(m_DefaultErrorLogDir);

            string str = "【" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "】    " + 
                errorStr + "\r\n\r\n";
            File.AppendAllText(m_DefaultErrorLogPath, str);
        }

        
    }
}
