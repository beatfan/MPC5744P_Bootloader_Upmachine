using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXP_HexParse.Datas;
using System.IO;
using System.Windows;

namespace NXP_HexParse.Config
{
    /// <summary>
    /// 与用户操作相关的类，例如写特定ini，比较列表、数组
    /// </summary>
    public class ClassUser
    {

        #region 读写相关数值函数
        //系统函数所在类，例如获取服务安装文件所在路径
        ClassSystem cstem = new ClassSystem();
        

        #region 主窗体

        

        /// <summary>
        /// 写默认页面
        /// </summary>
        public void WriteDefaultPage(int pageIndex)
        {

            string str = pageIndex.ToString();
            CommonData.defaultPage = pageIndex;  //更新CommomData

            cstem.WriteConfig("系统", "默认页面", str); //写配置文件
        }

        /// <summary>
        /// 获取默认页面索引，如果默认页面索引不存在则写入括号内默认页面索引，并返回false,默认页面索引赋值给CommonData.defaultPage
        /// </summary>
        public bool GetDefaultPage()
        {
            string s = cstem.GetConfig("系统", "默认页面"); //从配置文件中获取ip
            if (s == "No")  //不存在此节点
            {
                WriteDefaultPage(CommonData.defaultPage); //写默认
                 
                return false;
            }
            else
            {
                CommonData.defaultPage = Convert.ToInt32(s);
                return true;
            }
        }

        /// <summary>
        /// 写默认位置
        /// </summary>
        public void WriteDefaultLocation(Point defaultLocation)
        {

            string str = defaultLocation.ToString();
            CommonData.defaultLocation = defaultLocation;  //更新CommomData

            cstem.WriteConfig("系统", "默认位置", str); //写配置文件
        }

        /// <summary>
        /// 获取默认页面索引，如果默认页面索引不存在则写入括号内默认页面索引，并返回false,默认页面索引赋值给CommonData.defaultPage
        /// </summary>
        public bool GetDefaultLocation()
        {
            string s = cstem.GetConfig("系统", "默认位置"); //从配置文件中获取ip

            if (s == "No")  //不存在此节点
            {

                WriteDefaultLocation(CommonData.defaultLocation); //写默认
                return false;
            }
            else
            {
                Point loc = Point.Parse(s);
                CommonData.defaultLocation = loc;
                return true;
            }
        }

        /// <summary>
        /// 写默认大小
        /// </summary>
        public void WriteDefaultSize(Size defaultSize)
        {

            string str = defaultSize.ToString();
            CommonData.defaultSize = defaultSize;  //更新CommomData

            cstem.WriteConfig("系统", "默认大小", str); //写配置文件
        }

        /// <summary>
        /// 获取默认大小索引，如果默认大小索引不存在则写入括号内默认大小索引，并返回false,默认大小索引赋值给CommonData.defaultSize
        /// </summary>
        public bool GetDefaultSize()
        {
            string s = cstem.GetConfig("系统", "默认大小"); //从配置文件中获取ip

            if (s == "No")  //不存在此节点
            {

                WriteDefaultSize(CommonData.defaultSize); //写默认
                return false;
            }
            else
            {
                Size loc = Size.Parse(s);
                CommonData.defaultSize = loc;
                return true;
            }
        }

        /// <summary>
        /// 写默认语言索引
        /// </summary>
        /// <param name="defaultLanguage"></param>
        public void WriteDefaultLanguage(byte defaultLanguage)
        {

            string str = defaultLanguage.ToString();
            CommonData.defaultLanguage = defaultLanguage;  //更新CommomData

            cstem.WriteConfig("系统", "默认语言", str); //写配置文件
        }

        /// <summary>
        /// 获取默认语言索引，如果默认大小索引不存在则写入括号内默认大小索引，并返回false,默认大小索引赋值给CommonData.defaultSize
        /// </summary>
        public bool GetDefaultLanguage()
        {
            string s = cstem.GetConfig("系统", "默认语言"); //从配置文件中获取

            if (s == "No")  //不存在此节点
            {

                WriteDefaultLanguage(CommonData.defaultLanguage); //写默认
                return false;
            }
            else
            {
                CommonData.defaultLanguage = Convert.ToByte(s);
                return true;
            }
        }
        #endregion

        
        #region 通用页面
        #region 配置CAN筛选ID
        /// <summary>
        /// 写默认筛选
        /// </summary>
        public void WriteNormalDefaultCanFilterActive(List<bool> normalDefaultFiltCanIdActiveList)
        {

            string str = SpecialFunctions.Converter.MyStringConverter.BoolListToHexString(normalDefaultFiltCanIdActiveList);
            CommonData.normalFiltCanIdActiveList = normalDefaultFiltCanIdActiveList;  //更新CommomData

            cstem.WriteConfig("通用页面", "CAN忽略ID启用", str); //写配置文件
        }

        /// <summary>
        /// 获取默认CAN ID启用，如果默认页面索引不存在则写入括号内默认页面索引，并返回false,默认页面索引赋值给CommonData.defaultPage
        /// </summary>
        public bool GetNormalDefaultCanFilterActive()
        {
            string s = cstem.GetConfig("通用页面", "CAN忽略ID启用"); //从配置文件中获取

            if (s == "No")  //不存在此节点
            {

                WriteNormalDefaultCanFilterActive(CommonData.normalFiltCanIdActiveList); //写默认
                return false;
            }
            else
            {
                CommonData.normalFiltCanIdActiveList = SpecialFunctions.Converter.MyStringConverter.HexStringToBoolList(s);
                return true;
            }
        }

        /// <summary>
        /// 写默认筛选
        /// </summary>
        public void WriteNormalDefaultCanFilter(List<UInt32> normalDefaultFiltCanIdList)
        {

            string str = SpecialFunctions.Converter.MyStringConverter.UintListToHexString(normalDefaultFiltCanIdList);
            CommonData.normalFiltCanIdList = normalDefaultFiltCanIdList;  //更新CommomData

            cstem.WriteConfig("通用页面", "CAN忽略ID", str); //写配置文件
        }

        /// <summary>
        /// 获取默认页面索引，如果默认页面索引不存在则写入括号内默认页面索引，并返回false,默认页面索引赋值给CommonData.defaultPage
        /// </summary>
        public bool GetNormalDefaultCanFilter()
        {
            string s = cstem.GetConfig("通用页面", "CAN忽略ID"); //从配置文件中获取

            if (s == "No")  //不存在此节点
            {

                WriteNormalDefaultCanFilter(CommonData.normalFiltCanIdList); //写默认
                return false;
            }
            else
            {
                CommonData.normalFiltCanIdList = SpecialFunctions.Converter.MyStringConverter.HexStringToUintList(s);
                return true;
            }
        }

        /// <summary>
        /// 写筛选规则,true仅屏蔽，false仅接收
        /// </summary>
        public void WriteNormalDefaultShieldContentId(bool defaultShieldContentId)
        {

            string str = defaultShieldContentId.ToString();
            CommonData.normalShieldContentId = defaultShieldContentId;  //更新CommomData

            cstem.WriteConfig("通用页面", "CAN屏蔽", str); //写配置文件
        }

        /// <summary>
        /// 读筛选规则
        /// </summary>
        public bool GetNormalDefaultShieldContentId()
        {
            string s = cstem.GetConfig("通用页面", "CAN屏蔽"); //从配置文件中获取

            if (s == "No")  //不存在此节点
            {

                WriteNormalDefaultShieldContentId(CommonData.normalShieldContentId); //写默认
                return false;
            }
            else
            {
                CommonData.normalShieldContentId = Convert.ToBoolean(s);
                return true;
            }
        }
        #endregion

        /// <summary>
        /// 写默认保存显示数据路径
        /// </summary>
        public void WriteNormalDefaultShowDataSaveInfo(string devRecDataSaveFileName, int devRecDataSaveGapTime)
        {
            //直接保存反斜杠会有问题，先将其转换
            CommonData.normalRecDataSaveFileName = devRecDataSaveFileName;  //更新CommomData
            CommonData.normalRecDataSaveGapTime = devRecDataSaveGapTime; //间隔时间

            cstem.WriteConfig("通用页面", "默认保存文件名", devRecDataSaveFileName); //写配置文件
            cstem.WriteConfig("通用页面", "默认保存间隔", devRecDataSaveGapTime.ToString()); //写配置文件
        }

        /// <summary>
        /// 获取默认保存显示数据路径
        /// </summary>
        public bool GetNormalDefaultShowDataSaveInfo()
        {
            //直接保存反斜杠会有问题，先将其转换
            string s1 = cstem.GetConfig("通用页面", "默认保存文件名"); ; //从配置文件中获取
            string s2 = cstem.GetConfig("通用页面", "默认保存间隔"); //从配置文件中获取

            if (s1 == "No" || s2 == "No")  //不存在此节点
            {
                WriteNormalDefaultShowDataSaveInfo(CommonData.normalRecDataSaveFileName, CommonData.normalRecDataSaveGapTime); //写默认

                return false;
            }
            else
            {
                CommonData.normalRecDataSaveFileName = s1;  //更新CommomData
                CommonData.normalRecDataSaveGapTime = Convert.ToInt32(s2); //间隔时间
                return true;
            }
        }

        /// <summary>
        /// 写默认CAN类型
        /// </summary>
        public void WriteNormalDefaultCanType(int canType)
        {

            string str = canType.ToString();
            CommonData.normalCanCanType = canType;  //更新CommomData

            cstem.WriteConfig("通用页面", "默认CAN类型", str); //写配置文件
        }

        /// <summary>
        /// 获取默认Can类型，如果默认设备索引不存在则写入括号内默认设备索引，并返回false,默认设备索引赋值给CommonData.canDeviceIndex
        /// </summary>
        public bool GetNormalDefaultCanType()
        {
            string s = cstem.GetConfig("通用页面", "默认CAN类型"); //从配置文件中获取ip
            if (s == "No")  //不存在此节点
            {
                WriteNormalDefaultCanType(CommonData.normalCanCanType); //写默认

                return false;
            }
            else
            {
                CommonData.normalCanCanType = Convert.ToInt32(s);
                return true;
            }
        }

        /// <summary>
        /// 写默认CAN设备索引
        /// </summary>
        public void WriteNormalDefaultDeviceIndex(int deviceIndex)
        {

            string str = deviceIndex.ToString();
            CommonData.normalCanDeviceIndex = deviceIndex;  //更新CommomData

            cstem.WriteConfig("通用页面", "默认设备索引", str); //写配置文件
        }

        /// <summary>
        /// 获取默认设备索引，如果默认设备索引不存在则写入括号内默认设备索引，并返回false,默认设备索引赋值给CommonData.canDeviceIndex
        /// </summary>
        public bool GetNormalDefaultDeviceIndex()
        {
            string s = cstem.GetConfig("通用页面", "默认设备索引"); //从配置文件中获取ip
            if (s == "No")  //不存在此节点
            {
                WriteNormalDefaultDeviceIndex(CommonData.normalCanDeviceIndex); //写默认

                return false;
            }
            else
            {
                CommonData.normalCanDeviceIndex = Convert.ToInt32(s);
                return true;
            }
        }

        /// <summary>
        /// 写默认CAN CAN索引
        /// </summary>
        public void WriteNormalDefaultCanIndex(int canIndex)
        {

            string str = canIndex.ToString();
            CommonData.normalCanCanIndex = canIndex;  //更新CommomData

            cstem.WriteConfig("通用页面", "默认CAN索引", str); //写配置文件
        }

        /// <summary>
        /// 获取默认CAN索引，如果默认CAN索引不存在则写入括号内默认CAN索引，并返回false,默认CAN索引赋值给CommonData.canDeviceIndex
        /// </summary>
        public bool GetNormalDefaultCanIndex()
        {
            string s = cstem.GetConfig("通用页面", "默认CAN索引"); //从配置文件中获取ip
            if (s == "No")  //不存在此节点
            {
                WriteNormalDefaultCanIndex(CommonData.normalCanCanIndex); //写默认

                return false;
            }
            else
            {
                CommonData.normalCanCanIndex = Convert.ToInt32(s);
                return true;
            }
        }

        /// <summary>
        /// 写默认CAN波特率索引
        /// </summary>
        public void WriteNormalDefaultCanBaudRateIndex(int baudrateIndex)
        {

            string str = baudrateIndex.ToString();
            CommonData.normalCanBaudrateIndex = baudrateIndex;  //更新CommomData

            cstem.WriteConfig("通用页面", "默认波特率索引", str); //写配置文件
        }

        /// <summary>
        /// 获取默认波特率索引，如果默认波特率索引不存在则写入括号内默认波特率索引，并返回false,默认波特率索引赋值给CommonData.canDeviceIndex
        /// </summary>
        public bool GetNormalDefaultCanBaudRateIndex()
        {
            string s = cstem.GetConfig("通用页面", "默认波特率索引"); //从配置文件中获取波特率
            if (s == "No")  //不存在此节点
            {
                WriteNormalDefaultCanBaudRateIndex(CommonData.normalCanBaudrateIndex); //写默认

                return false;
            }
            else
            {
                CommonData.normalCanBaudrateIndex = Convert.ToInt32(s);
                return true;
            }
        }

        /// <summary>
        /// 写默认CAN发送次数
        /// </summary>
        public void WriteNormalDefaultSendTimers(int sendTimers)
        {

            string str = sendTimers.ToString();
            CommonData.normalSendTimers = sendTimers;  //更新CommomData

            cstem.WriteConfig("通用页面", "发送次数", str); //写配置文件
        }

        /// <summary>
        /// 获取默认发送次数
        /// </summary>
        public bool GetNormalDefaultSendTimers()
        {
            string s = cstem.GetConfig("通用页面", "发送次数"); //从配置文件中获取
            if (s == "No")  //不存在此节点
            {
                WriteNormalDefaultSendTimers(CommonData.normalSendTimers); //写默认

                return false;
            }
            else
            {
                CommonData.normalSendTimers = Convert.ToInt32(s);
                return true;
            }
        }

        /// <summary>
        /// 写默认CAN发送间隔
        /// </summary>
        public void WriteNormalDefaultSendGap(int sendGap)
        {

            string str = sendGap.ToString();
            CommonData.normalSendGap = sendGap;  //更新CommomData

            cstem.WriteConfig("通用页面", "发送间隔", str); //写配置文件
        }

        /// <summary>
        /// 获取默认发送间隔
        /// </summary>
        public bool GetNormalDefaultSendGap()
        {
            string s = cstem.GetConfig("通用页面", "发送间隔"); //从配置文件中获取
            if (s == "No")  //不存在此节点
            {
                WriteNormalDefaultSendGap(CommonData.normalSendGap); //写默认

                return false;
            }
            else
            {
                CommonData.normalSendGap = Convert.ToInt32(s);
                return true;
            }
        }

        

        /// <summary>
        /// 写默认机器列表索引
        /// </summary>
        public void WriteNormalDefaultMachineIndex(int machineIndex)
        {

            string str = machineIndex.ToString();
            CommonData.normalMachineIndex = machineIndex;  //更新CommomData

            cstem.WriteConfig("通用页面", "默认电控索引", str); //写配置文件
        }

        /// <summary>
        /// 获取默认厂商列表索引，如果默认波特率索引不存在则写入括号内默认波特率索引，并返回false,默认波特率索引赋值给CommonData.canDeviceIndex
        /// </summary>
        public bool GetNormalDefaultMachineIndex()
        {
            string s = cstem.GetConfig("通用页面", "默认电控索引"); //从配置文件中获取波特率
            if (s == "No")  //不存在此节点
            {
                WriteNormalDefaultMachineIndex(CommonData.normalMachineIndex); //写默认

                return false;
            }
            else
            {
                CommonData.normalMachineIndex = Convert.ToInt32(s);
                return true;
            }
        }

        /// <summary>
        /// 写发送转速
        /// </summary>
        public void WriteNormalDefaultSendSpeed(int sendSpeed)
        {

            string str = sendSpeed.ToString();
            CommonData.normalSendSpeed = sendSpeed;  //更新CommomData

            cstem.WriteConfig("通用页面", "发送转速", str); //写配置文件

        }

        /// <summary>
        /// 获取发送转速，如果默认设备索引不存在则写入括号内默认设备索引，并返回false,默认设备索引赋值给CommonData.canDeviceIndex
        /// </summary>
        public bool GetNormalDefaultSendSpeed()
        {
            string s1 = cstem.GetConfig("通用页面", "发送转速"); //从配置文件中获取
            if (s1 == "No")  //不存在此节点
            {
                WriteNormalDefaultSendSpeed(CommonData.normalSendSpeed); //写默认

                return false;
            }
            else
            {
                CommonData.normalSendSpeed = Convert.ToInt32(s1);
                return true;
            }
        }

        /// <summary>
        /// 写默认发送扭矩
        /// </summary>
        public void WriteNormalDefaultSendTorque(int sendTorque)
        {

            string str = sendTorque.ToString();
            CommonData.normalSendTorque = sendTorque;  //更新CommomData

            cstem.WriteConfig("通用页面", "发送扭矩", str); //写配置文件

        }

        /// <summary>
        /// 获取发送扭矩，如果默认设备索引不存在则写入括号内默认设备索引，并返回false,默认设备索引赋值给CommonData.canDeviceIndex
        /// </summary>
        public bool GetNormalDefaultSendTorque()
        {
            string s1 = cstem.GetConfig("通用页面", "发送扭矩"); //从配置文件中获取
            if (s1 == "No")  //不存在此节点
            {
                WriteNormalDefaultSendTorque(CommonData.normalSendTorque); //写默认

                return false;
            }
            else
            {
                CommonData.normalSendTorque = Convert.ToInt32(s1);
                return true;
            }
        }

        /// <summary>
        /// 写默认发送速度上下限
        /// </summary>
        public void WriteNormalDefaultSpeedUpDownLimit(uint downLimit, uint upLimit)
        {

            string str = downLimit.ToString();
            CommonData.normalSpeedDownLimit = downLimit;  //更新CommomData

            cstem.WriteConfig("通用页面", "发送速度下限", str); //写配置文件

            str = upLimit.ToString();
            CommonData.normalSpeedUpLimit = upLimit;  //更新CommomData

            cstem.WriteConfig("通用页面", "发送速度上限", str); //写配置文件

        }

        /// <summary>
        /// 获取发送速度上下限，如果默认CAN索引不存在则写入括号内默认CAN索引，并返回false,默认CAN索引赋值给CommonData.canDeviceIndex
        /// </summary>
        public bool GetNormalDefaultSpeedUpDownLimit()
        {
            string s1 = cstem.GetConfig("通用页面", "发送速度下限"); //从配置文件中获取
            string s2 = cstem.GetConfig("通用页面", "发送速度上限"); //从配置文件中获取

            if (s1 == "No" || s2 == "No")  //不存在此节点
            {
                WriteNormalDefaultSpeedUpDownLimit(CommonData.normalSpeedDownLimit, CommonData.normalSpeedUpLimit); //写默认

                return false;
            }
            else
            {
                CommonData.normalSpeedDownLimit = Convert.ToUInt32(s1);
                CommonData.normalSpeedUpLimit = Convert.ToUInt32(s2);
                return true;
            }
        }

        #endregion


        #endregion

    }
}
