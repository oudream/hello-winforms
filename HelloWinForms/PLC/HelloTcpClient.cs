using CommonInterfaces;
using HelloWinForms.Protocols;
using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.ModelBinding;
using System.Windows.Forms;
using YamlDotNet.Core.Tokens;

namespace HelloWinForms.PLC
{
    public partial class HelloTcpClient : Form
    {
        private const string CS_STOP = "停止";
        private const string CS_STOPPING = "正在停止...";
        private const string CS_START = "启动";

        //TcpClientChannel tcpClient;

        public HelloTcpClient()
        {
            InitializeComponent();
        }

        private void TcClient_DataReceived(byte[] buffer, int bytesRead)
        {
            this.BeginInvoke(new System.Action(() =>
            {
                TcClientDataReceived(buffer, bytesRead);
            }));
            //this.BeginInvoke(new Action<byte[], int>(TcClientDataReceived), new object[] { buffer, bytesRead });
        }

        private void TcClientDataReceived(byte[] buffer, int bytesRead)
        {
            // 转十六进制字符串
            var arg1 = StringHelper.BytesToHexString(buffer, 0, bytesRead);
            richTextBox1.AppendText($"Received: {arg1}\n");
        }

        private void TcClient_StateChanged(object sender, TcpClientChannel.TcpClientModuleState state)
        {
            this.BeginInvoke(new System.Action(() =>
            {
                TcClientStateChanged(state);
            }));
        }
        private void TcClientStateChanged(TcpClientChannel.TcpClientModuleState state)
        {
            switch (state)
            {
                case TcpClientChannel.TcpClientModuleState.Started:
                    connectButton.Enabled = true;
                    connectButton.Text = CS_STOP;
                    break;
                case TcpClientChannel.TcpClientModuleState.Stopping:
                    connectButton.Enabled = false;
                    connectButton.Text = CS_STOPPING;
                    break;
                case TcpClientChannel.TcpClientModuleState.Stopped:
                    connectButton.Enabled = true;
                    connectButton.Text = CS_START;
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var hexString = "01020304ABCDRR";
            var bytes = StringHelper.HexStringToBytes(hexString);
            richTextBox1.AppendText($"Hex: {hexString}\nBytes: {bytes.Length}\n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (connectButton.Text == CS_START)
            {
                if (NetHelper.ValidateIPv4(ipTextBox.Text))
                {
                    PLCTcpDriverHelper.ModuleStateChanged += TcClient_StateChanged;
                    PLCTcpDriverHelper.ReceivedMessage += TcClient_ReceivedMessage;
                    PLCTcpDriverHelper.Start(ipTextBox.Text, (ushort)portNumericUpDown.Value);
                }
                else
                {
                    LogHelper.Error($"无效的PLC IP地址：{ipTextBox.Text}");
                }
            }
            else if (connectButton.Text == CS_STOP)
            {
                PLCTcpDriverHelper.Stop();
                PLCTcpDriverHelper.ReceivedMessage -= TcClient_ReceivedMessage;
                PLCTcpDriverHelper.ModuleStateChanged -= TcClient_StateChanged;
            }

        }

        private void TcClient_ReceivedMessage(object sender, PlcMessage e)
        {
            this.BeginInvoke(new System.Action(() =>
            {
                TcClientReceivedMessage(e);
            }));
        }

        private static volatile int receivedMessageCount = 0;



        private void TcClientReceivedMessage(PlcMessage plcMessage)
        {
            richTextBox1.AppendText($"Received message[{receivedMessageCount++}] {plcMessage.ModuleNumber} {plcMessage.BatchNumber} {plcMessage.Cmd} {plcMessage.Pos} {plcMessage.Sn1} {plcMessage.Sn2} {plcMessage.Sn3} {plcMessage.Sn4} {plcMessage.Dt}\n");
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(sendTextBox.Text))
            {
                // Text 转byte[]
                byte[] bytes = Encoding.ASCII.GetBytes(sendTextBox.Text);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //connectButton_Click(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PlcFeedback feedback = new PlcFeedback();
            feedback.BatchNumber = 1;

            var numberValue = DateTime.Now.Second % 2 == 1 ? (ushort)1 : (ushort)2;

            feedback.Number1 = feedback.Number1 == 2 ? (ushort)2 : numberValue;
        }
    }

}
