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
        public ushort Pos { get; set; }
        public string Sn1 { get; set; }
        public string Sn2 { get; set; }
        public string Sn3 { get; set; }
        public string Sn4 { get; set; }
        public DateTime Dt { get; set; }

        public string ToMessage()
        {
            return $"*#module_number:{ModuleNumber}#batch_number:{BatchNumber}#cmd:{Cmd}#pos:{Pos}#sn1:{Sn1}#sn2:{Sn2}#sn3:{Sn3}#sn4:{Sn4}#dt:{Dt:yyyy-MM-dd HH:mm:ss}&";
        }
    }
}
