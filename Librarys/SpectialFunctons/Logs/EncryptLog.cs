using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFunctions.EncryptLog
{
    public class EncryptLog
    {
        string m_EncryptLogPath;
        string m_EncryptLogFileName;
        string m_EncryptLogFullName;

        string m_LogContent;

        /// <summary>
        /// 设置日志文件夹，文件名
        /// </summary>
        /// <param name="path"></param>
        private void SetEncryptLogInfo(string path, string logName)
        {
            m_EncryptLogPath = path;
            m_EncryptLogFileName = logName;

            m_EncryptLogFullName = m_EncryptLogPath + m_EncryptLogFileName + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss") + ".eklog";
        }

        /// <summary>
        /// path为EncryptLog文件夹，logName为文件名，例如test，所有加密log的文件后缀统一为.eklog
        /// </summary>
        /// <param name="path"></param>
        public EncryptLog(string path, string logName)
        {
            SetEncryptLogInfo(path,logName);
        }

        public void WriteEncryptLog(string logStr)
        {

        }

        public string ReadEncryptLog()
        {
            return m_LogContent;
        }
    }
}
