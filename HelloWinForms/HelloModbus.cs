using NModbus;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloModbus : Form
    {
        public HelloModbus()
        {
            InitializeComponent();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            // 创建TCP客户端
            var client = new TcpClient("127.0.0.1", 502);  // IP地址和端口
            var modbusFactory = new ModbusFactory();
            var modbusMaster = modbusFactory.CreateMaster(client);

            try
            {
                // 读取从站数据
                ushort startAddress = 100;  // 起始地址
                ushort numInputs = 5;       // 读取数量
                byte slaveId = 1;         // 从站ID

                // 读取保持寄存器
                ushort[] registers = await modbusMaster.ReadHoldingRegistersAsync(slaveId, startAddress, numInputs);
                Console.WriteLine("Read data: ");
                foreach (var register in registers)
                {
                    Console.WriteLine($"Register value: {register}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}
