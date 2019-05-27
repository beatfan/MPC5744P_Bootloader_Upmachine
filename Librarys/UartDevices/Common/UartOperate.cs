using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UartDevices.Common
{

    public class UartOperate
    {
        System.IO.Ports.SerialPort mySerialPort = new System.IO.Ports.SerialPort();

        public delegate void PushReceiveDataDele(byte[] datas);
        public PushReceiveDataDele pushReceiveData;

        public List<string> GetAllComPorts()
        {
            List<string> allPorts  = new CommonUart1().MulGetHardwareInfo(CommonUart1.HardwareEnum.Win32_SerialPort, "Name");
            //allPorts.Add(CommonUart2.GetComName("zebra"));
            return allPorts;
        }

        public bool SerialInitial( PushReceiveDataDele callback)
        {
            mySerialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(mySerialPort_DataReceived);

            pushReceiveData = callback;

            return true;
        }

        public bool Open(string SerialName, int BaudRate, int dataBis, System.IO.Ports.Parity Pority, System.IO.Ports.StopBits StopBits)
        {
            try
            {
                mySerialPort.PortName = SerialName;
                mySerialPort.BaudRate = BaudRate;
                mySerialPort.DataBits = dataBis;
                mySerialPort.Parity = Pority;
                mySerialPort.StopBits = StopBits;

                //mySerialPort.PortName = SerialName.Split('(')[1].Split(')')[0];
                if (!mySerialPort.IsOpen)
                    mySerialPort.Open();

                return true;
            }
            catch (Exception ex)
            {
                //mySerialPort.Close();
                return false;
            }

        }

        public bool Close()
        {
            try
            {
                if (mySerialPort.IsOpen)
                    mySerialPort.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SendBytes(byte[] datas)
        {
            try
            {
                mySerialPort.Write(datas, 0, datas.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void mySerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                System.Threading.Thread.Sleep(5);//等待数据缓冲好

                int count = mySerialPort.BytesToRead;

                byte[] buffer = new byte[count];

                mySerialPort.Read(buffer, 0, count);

                if (pushReceiveData != null)
                    pushReceiveData(buffer);
            }
            catch (Exception ex)
            {

            }
        }
    }
    
}
