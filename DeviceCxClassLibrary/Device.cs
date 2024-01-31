using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceCxClassLibrary
{
    // DeviceFactory.cs
    public static class DeviceFactory
    {
        public static Object CreateDevice()
        {
            return new Device()
            {
                DeviceId = "DEV001",
                DeviceName = "温控器",
            };
        }
    }


    // 代表串口配置的类
    public class SerialPortConfig
    {
        private int baudRate = 9600;
        private Parity parity = Parity.None;
        private int dataBits = 8;
        private StopBits stopBits = StopBits.One;

        [Category("串口设置"), Description("通信的波特率。")]
        public int BaudRate
        {
            get { return baudRate; }
            set { baudRate = value; }
        }

        [Category("串口设置"), Description("通信的奇偶校验位。")]
        [TypeConverter(typeof(ParityConverter))]
        public Parity Parity
        {
            get { return parity; }
            set { parity = value; }
        }

        [Category("串口设置"), Description("通信的数据位。")]
        public int DataBits
        {
            get { return dataBits; }
            set { dataBits = value; }
        }

        [Category("串口设置"), Description("通信的停止位。")]
        public StopBits StopBits
        {
            get { return stopBits; }
            set { stopBits = value; }
        }
    }

    // 设备类
    public class Device
    {
        private string deviceId;
        private string deviceName;
        private SerialPortConfig serialPortConfig = new SerialPortConfig();

        [Category("设备信息"), Description("设备的唯一标识符。")]
        public string DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; }
        }

        [Category("设备信息"), Description("设备的名称。")]
        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; }
        }

        [Category("通信配置"), Description("设备的串口配置。")]
        [TypeConverter(typeof(ExpandableObjectConverter))] // 允许在 PropertyGrid 中展开
        public SerialPortConfig SerialPortConfig
        {
            get { return serialPortConfig; }
            set { serialPortConfig = value; }
        }
    }

    public class ParityConverter : EnumConverter
    {
        public ParityConverter(Type type) : base(type)
        {
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                switch ((Parity)value)
                {
                    case Parity.None:
                        return "无";
                    case Parity.Odd:
                        return "奇校验";
                    case Parity.Even:
                        return "偶校验";
                    case Parity.Mark:
                        return "标记";
                    case Parity.Space:
                        return "空格";
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value.ToString())
            {
                case "无":
                    return Parity.None;
                case "奇校验":
                    return Parity.Odd;
                case "偶校验":
                    return Parity.Even;
                case "标记":
                    return Parity.Mark;
                case "空格":
                    return Parity.Space;
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}
