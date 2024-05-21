using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.Protocols
{
    public class PlcMessage
    {
        // 模组
        public uint ModuleNumber { get; set; }
        // 批次号
        public uint BatchNumber { get; set; }
        // 命令
        public ushort Cmd { get; set; }
        // 穴位
        // 11=1号产品正面取图请求；12=2号产品正面取图请求；13=3号产品正面取图请求；14=4号产品正面取图请求；
        // 21=1号产品反面取图请求；22=2号产品反面取图请求；23=3号产品反面取图请求；24=4号产品反面取图请求；
        // 11=扫码请求；21=Xray采图请求；100=请求最终结果；200=开光源请求；
        public ushort Pos { get; set; }
        public string Sn1 { get; set; }
        public string Sn2 { get; set; }
        public string Sn3 { get; set; }
        public string Sn4 { get; set; }
        public DateTime Dt { get; set; }

        public long SendImageAcquisitionTime { get; set; }
        public long ReceivedQRCodeTime { get; set; }
        public string ReceivedQRCodes { get; set; }
        public long ReceivedImageTime { get; set; }
        public long SendResultTime { get; set; }
        public string ReceivedResult { get; set; }
        public long ReceivedResultTime { get; set; }

        public string ToMessage()
        {
            return $"*#module_number:{ModuleNumber}#batch_number:{BatchNumber}#cmd:{Cmd}#pos:{Pos}#sn1:{Sn1}#sn2:{Sn2}#sn3:{Sn3}#sn4:{Sn4}#dt:{Dt:yyyy-MM-dd HH:mm:ss}&";
        }


    }

    public class PlcFeedback
    {
        // 模组号
        // 1=1号模组反馈，2=2号模组反馈，
        public uint ModuleNumber { get; set; }
        // 反馈的结果
        // 1=ok;2=busy;3=fault;
        public ushort Result { get; set; }
        // 位置
        // 11=1号产品正面取图反馈；12=2号产品正面取图反馈；13=3号产品正面取图反馈；14=4号产品正面取图反馈；
        // 21=1号产品反面取图反馈；22=2号产品反面取图反馈；23=3号产品反面取图反馈；24=4号产品反面取图反馈；
        public ushort Pos { get; set; }
        // 产品检测批次号反馈
        public uint BatchNumber { get; set; }
        // 1=1号穴位产品OK;2=1号穴位产品NG;
        public ushort? Number1 { get; set; }
        public ushort? Number2 { get; set; }
        public ushort? Number3 { get; set; }
        public ushort? Number4 { get; set; }
        // 1号穴位SN码反馈;
        public string Sn1 { get; set; }
        public string Sn2 { get; set; }
        public string Sn3 { get; set; }
        public string Sn4 { get; set; }
        public DateTime Dt { get; set; }

        public override string ToString()
        {
            return $"Module Number: {ModuleNumber}, Result: {Result}, Position: {Pos}, Batch Number: {BatchNumber}, " +
                   $"Number1: {Number1}, Number2: {Number2}, Number3: {Number3}, Number4: {Number4}, " +
                   $"SN1: {Sn1}, SN2: {Sn2}, SN3: {Sn3}, SN4: {Sn4}, Date: {Dt.ToString("yyyy-MM-dd HH:mm:ss")}";
        }
    }
}
