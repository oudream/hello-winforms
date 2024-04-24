using CxWorkStation.Utilities;
using DevExpress.Services.Internal;
using DevExpress.Utils;
using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.XtraEditors.Filtering.DataItemsExtension;

namespace HelloWinForms.Protocols
{
    public class S7PointItem : PointItem
    {
        // PLC的DB区，如"DB1"
        // 合成整个地址："DB1.DBW10"等
        [DisplayName("DB区号")]
        public int DBArea { get; set; }

        // 数据类型，如"Byte", "Word", "DWord"等
        [DisplayName("变量类型")]
        public VariableType VariableType { get; set; }

        // Address是"0.0"，解释后要点前部分
        private string address;
        [DisplayName("变量地址")]
        public string Address
        {
            get => address;
            set
            {
                address = value;
                SetByteOffset();
                SetBitOffset();
            }
        }

        [DisplayName("字符长度")]
        public int StringLength { get; set; }

        [DisplayName("读取周期间隔")]
        public int ReadInterval { get; set; }

        [Browsable(false)]
        public int ByteOffset { get; private set; }

        [Browsable(false)]
        public byte BitOffset { get; private set; }

        public S7PointItem(string deviceName, string variableName) : base(deviceName, variableName)
        {
        }



        private void SetByteOffset()
        {
            // Address是"0.0"，解释后要点前部分
            // Assuming the Address format could be "DB1.DBW10" or "I0.0"
            var parts = Address.Split(new char[] { '.', 'W' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1)
            {
                // Try to parse the part that represents the byte offset
                if (int.TryParse(parts[0], out int byteOffset))
                {
                    ByteOffset = byteOffset;
                    return;
                }
            }
            ByteOffset = 0; // Indicate failure to parse
        }

        private void SetBitOffset()
        {
            // Address是"0.0"，解释后要点后部分
            var parts = Address.Split('.');
            if (parts.Length >= 2)
            {
                // The bit offset is after the period
                if (byte.TryParse(parts[1], out byte bitOffset))
                {
                    BitOffset = bitOffset;
                    return;
                }
            }
            BitOffset = 0; // Indicate failure to parse
        }

        public int GetLength()
        {
            // if string return Length
            // else switch VarType
            switch (VariableType)
            {
                case VariableType.Bit: return 1;
                case VariableType.Byte: return 1;
                case VariableType.Int16: return 2;
                case VariableType.Int32: return 4;
                case VariableType.Float32: return 4;
                case VariableType.Float64: return 8;
                case VariableType.DateTimeLong: return 12;
            }
            return StringLength;
        }

        public static string CheckS7Point(S7PointItem pt)
        {
            if (pt.DBArea < 0 || pt.DBArea > 255)
            {
                return "DB区号[{pt.DBArea}]不合法";
            }
            if (pt.ByteOffset < 0 || pt.ByteOffset > 65535)
            {
                return "字节偏移[{pt.ByteOffset}]不合法";
            }
            if (pt.BitOffset < 0 || pt.BitOffset > 7)
            {
                return "位偏移[{pt.BitOffset}]不合法";
            }
            if (pt.StringLength < 0 || pt.StringLength > 254)
            {
                return "字符串长度[{pt.StringLength}]不合法";
            }
            return null;
        }

    }

    public enum VariableType
    {
        [Description("Bit (bool)")]
        Bit,

        [Description("Byte (1 byte)")]
        Byte,

        [Description("Int16 (2 bytes)")]
        Int16,

        [Description("Int32 (4 bytes)")]
        Int32,

        [Description("Float32 (4 bytes)")]
        Float32,

        [Description("Float64 (8 bytes)")]
        Float64,

        [Description("String (N bytes < 254)")]
        S7String,

        [Description("DateTimeLong (12 bytes)")]
        DateTimeLong,
    }
}
