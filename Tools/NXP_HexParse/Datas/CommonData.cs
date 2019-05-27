using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace NXP_HexParse.Datas
{
    public class CommonData
    {
        #region 系统配置


        /// <summary>
        /// 默认页面
        /// </summary>
        public static int defaultPage = 0;

        /// <summary>
        /// 窗体默认位置
        /// </summary>
        public static Point defaultLocation = new Point(10, 10);

        /// <summary>
        /// 窗体默认大小
        /// </summary>
        public static Size defaultSize = new Size(1000, 550);

        /// <summary>
        /// 默认语言
        /// </summary>
        public static byte defaultLanguage = 0;

        /// <summary>
        /// 配置文件路径
        /// </summary>
        public static string ConfigPath = Directory.GetCurrentDirectory() + "\\Config.ini";


        #endregion


        #region 常规页面

        /// <summary>
        /// 常规通用页面目录
        /// </summary>
        public static string normalPagePath = Directory.GetCurrentDirectory() + "\\NormalPage";

        /// <summary>
        /// 常规页面日志目录
        /// </summary>
        public static string normalPageLogPath = normalPagePath + "\\Logs";


        /// <summary>
        /// ListView数据保存路径
        /// </summary>
        public static string normalRecListSavePath = normalPagePath + "\\ReceiveListDatas_";

        /// <summary>
        /// 接收电控信息保存路径
        /// </summary>
        public static string normalRecDataSavePath = normalPagePath + "\\ShowDatasSave";

        /// <summary>
        /// 电控显示信息保存文件名
        /// </summary>
        public static string normalRecDataSaveFileName = "MotorInfoSave";

        /// <summary>
        /// 电控显示信息保存文件名全路径
        /// </summary>
        public static string normalRecDataSaveFileFullName = normalRecDataSavePath + "\\" + normalRecDataSaveFileName + ".xls";

        /// <summary>
        /// 自动保存数据间隔时间
        /// </summary>
        public static int normalRecDataSaveGapTime = 5000;

        /// <summary>
        /// 机器索引，发电机，电控等
        /// </summary>
        public static int normalMachineIndex = 0;

        /// <summary>
        /// CAN的类型
        /// </summary>
        public static int normalCanCanType = 0;

        /// <summary>
        /// CAN的设备索引
        /// </summary>
        public static int normalCanDeviceIndex = 0;

        /// <summary>
        /// CAN第几路
        /// </summary>
        public static int normalCanCanIndex = 0;

        /// <summary>
        /// CAN波特率
        /// </summary>
        public static int normalCanBaudrateIndex = 3; //250Kbps

        /// <summary>
        /// 发送次数
        /// </summary>
        public static int normalSendTimers = 1;

        /// <summary>
        /// 发送间隔，单位ms
        /// </summary>
        public static int normalSendGap = 1000;



        /// <summary>
        /// 发送速度
        /// </summary>
        public static int normalSendSpeed = 500;
        /// <summary>
        /// 发送扭矩
        /// </summary>
        public static int normalSendTorque = 60;

        /// <summary>
        /// 发送速度下限
        /// </summary>
        public static uint normalSpeedDownLimit = 0;
        /// <summary>
        /// 发送速度上限
        /// </summary>
        public static uint normalSpeedUpLimit = 800;

        /// <summary>
        /// CAN筛选ID启用集合
        /// </summary>
        public static List<bool> normalFiltCanIdActiveList = new List<bool>();

        /// <summary>
        /// CAN筛选ID集合
        /// </summary>
        public static List<UInt32> normalFiltCanIdList = new List<UInt32>();

        /// <summary>
        /// CAN筛选规则，true表示仅屏蔽列表中的ID数据，false表示仅接收列表中的ID的数据
        /// </summary>
        public static bool normalShieldContentId = false;

        #endregion


        #region A4964寄存器
        /// <summary>
        /// 寄存器配置文件路径
        /// </summary>
        public static string registerConfigFilePath = Directory.GetCurrentDirectory() + "\\config.rst";

        /// <summary>
        /// 寄存器值，共33个数据
        /// 每个寄存器16bits
        /// 0-30个为寄存器0-30
        /// 31号为DemandInput
        /// 32号位Npp极对数
        /// 第15-11bits 五位寄存器地址，设置为0
        /// 第0bit 一位 奇偶校验位，设置为0
        /// </summary>
        public static ushort[] registerDataList = new ushort[33]{ 0x04C, 0x000, 0x040, 0x000, 0x000, 0x000, //0-5
            0x0FE, 0x03E, 0x07E, 0x000, 0x000, //6-10
            0x0EE, 0x000, 0x008, 0x002, 0x04A, //11-15
            0x038, 0x04E, 0x0EE, 0x0EE, 0x08E, //16-20
            0x0AA, 0x100, 0x0EE, 0x000, 0x389, //21-25
            0x000, 0x000, 0x000, 0x000, 0x000, //26-30
            269, 0x02 };

        /// <summary>
        /// A4964默认寄存器值
        /// </summary>
        public static ushort[] registerDefaultDataList = new ushort[33] { 0x04C, 0x000, 0x040, 0x000, 0x000, 0x000, //0-5
            0x0FE, 0x03E, 0x07E, 0x000, 0x000, //6-10
            0x0EE, 0x000, 0x008, 0x002, 0x04A, //11-15
            0x038, 0x04E, 0x0EE, 0x0EE, 0x08E, //16-20
            0x0AA, 0x100, 0x0EE, 0x000, 0x389, //21-25
            0x000, 0x000, 0x000, 0x000, 0x000, //26-30
            269, 0x02 };

        #endregion
    }
}
