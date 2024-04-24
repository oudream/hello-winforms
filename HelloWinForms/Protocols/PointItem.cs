using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.Protocols
{
    // 设备点表，设备点表包含本设备的点表项，点表项由设备名、设备属性名组成。
    // 点表项是一个抽象类，为其它驱动模块来具体化，比如（板卡点表项、IO模板点表项、S7点表项）
    public abstract class PointItem
    {
        [DisplayName("设备名")] // DataGridView 显示名
        public string DeviceName { get; private set; }

        [DisplayName("属性名")] // DataGridView 属性名
        public string VariableName { get; private set; }

        [DisplayName("描述")] // DataGridView 描述
        public string Description { get; set; }

        [DisplayName("可读")]
        public bool CanRead { get; set; } = true;

        [DisplayName("可写")]
        public bool CanWrite { get; set; } = false;

        [DisplayName("可控")]
        public bool CanControl { get; set; } = false;

        [DisplayName("可事件")]
        public bool CanEvent { get; set; } = false;

        [DisplayName("可告警")]
        public bool CanAlarm { get; set; } = false;

        protected PointItem(string deviceName, string variableName)
        {
            DeviceName = deviceName;
            VariableName = variableName;
        }

    }

}
