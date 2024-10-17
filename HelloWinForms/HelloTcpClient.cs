using CxWorkStation.Utilities;
using HelloWinForms.Channel;
using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Utils.HashCodeHelper.Primitives;

namespace HelloWinForms
{
    public partial class HelloTcpClient : Form
    {
        public HelloTcpClient()
        {
            InitializeComponent();
        }

        TcpClientChannel _tcpClientChannel = new TcpClientChannel();

        private void button1_Click(object sender, EventArgs e)
        {
            if (!_tcpClientChannel.IsConnected())
            {
                _tcpClientChannel.Start(ipTextBox.Text, (ushort)portNumericUpDown.Value);
                _tcpClientChannel.DataReceived += TcpClientChannel_DataReceived;
            }
        }

        private void TcpClientChannel_DataReceived(byte[] arg1, int arg2)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<byte[], int>(TcpClientChannel_DataReceived), new object[] { arg1, arg2 });
                return;
            }
            string message = Encoding.UTF8.GetString(arg1, 0, arg2);
            richTextBox1.AppendText(message + "\n");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                timer1.Interval = (int)numericUpDown1.Value;
            }
            timer1.Enabled = checkBox1.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Send();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            _tcpClientChannel.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //this.richTextBox1.Clear();
            var byteArray = new byte[255];
            richTextBox1.AppendText(BitConverter.ToString(byteArray));
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void Send()
        {
            if (checkBox2.Checked)
            {
                byte[] buffer = HexStringToByteArray(sendTextBox.Text);
                _tcpClientChannel.Send(buffer);
            }
            else
            {
                byte[] buffer = Encoding.UTF8.GetBytes(sendTextBox.Text);
                _tcpClientChannel.Send(buffer);
            }
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            // 去除字符串中的连字符
            hexString = hexString.Replace("-", string.Empty);

            if (hexString.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length");

            byte[] byteArray = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                byteArray[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return byteArray;
        }

    }

   
}
