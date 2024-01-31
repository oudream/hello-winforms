using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWinForms.Channel
{

    public class TcpChannelClient : ChannelClient
    {
        private TcpClient tcpClient;
        private NetworkStream stream;

        private Thread readingThread;
        private volatile bool keepReading;

        private long bytesSent = 0;
        private long bytesReceived = 0;

        public override bool IsConnected => tcpClient != null && tcpClient.Connected;
        public override long BytesSent => bytesSent;
        public override long BytesReceived => bytesReceived;

        public override void Connect(IChannelConfig config)
        {
            if (config is TcpClientConfig tcpConfig)
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(tcpConfig.Host, tcpConfig.Port); // 这里应当处理异常或异步连接
                stream = tcpClient.GetStream();
                keepReading = true;
                StartReading();
            }
            else
            {
                throw new InvalidOperationException("Configuration is not of expected type TcpClientConfig.");
            }
        }

        private void StartReading()
        {
            readingThread = new Thread(() =>
            {
                byte[] buffer = new byte[4096];
                while (keepReading)
                {
                    try
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            bytesReceived += bytesRead; // 更新接收字节数
                            var receivedData = new byte[bytesRead];
                            Array.Copy(buffer, receivedData, bytesRead);
                            OnDataReceived(receivedData);
                        }
                        else
                        {
                            Thread.Sleep(100);
                        }
                    }
                    catch
                    {
                        if (!keepReading) break;
                        OnErrorOccurred("读取数据时发生错误");
                        break;
                    }
                }
            })
            { IsBackground = true };

            readingThread.Start();
        }

        public override void Disconnect()
        {
            if (tcpClient != null)
            {
                try
                {
                    keepReading = false; // 告诉读取循环停止
                    tcpClient.Client.Close();
                    readingThread?.Join(); // 等待读取线程结束
                    stream?.Close(); // 尝试关闭网络流
                    tcpClient.Close(); // 关闭TCP客户端
                }
                catch
                {
                    // 可以在这里记录日志，但不应该因为尝试关闭资源而抛出异常
                }
                finally
                {
                    stream = null;
                    tcpClient = null;
                    readingThread = null;
                }
            }
        }

        public void SendHexString(string hexData)
        {
            if (!string.IsNullOrWhiteSpace(hexData))
            {
                var data = HexStringToByteArray(hexData);
                Send(data);
                bytesSent += data.Length; // 更新发送字节数
            }
        }

        public async Task SendHexStringAsync(string hexData)
        {
            if (!string.IsNullOrWhiteSpace(hexData))
            {
                var data = HexStringToByteArray(hexData);
                await SendAsync(data);
                bytesSent += data.Length; // 更新发送字节数
            }
        }

        public override async Task SendAsync(byte[] data)
        {
            if (!IsConnected)
            {
                return;
            }

            try
            {
                await stream.WriteAsync(data, 0, data.Length);
                bytesSent += data.Length;
            }
            catch (Exception ex)
            {
                // 触发错误事件
                OnErrorOccurred($"发送数据时发生错误: {ex.Message}");
                // 关闭连接
                Disconnect();
            }
        }

        public override void Send(byte[] data)
        {
            if (!IsConnected)
            {
                return;
            }

            try
            {
                stream.Write(data, 0, data.Length);
                bytesSent += data.Length;
            }
            catch (Exception ex)
            {
                // 触发错误事件
                OnErrorOccurred($"发送数据时发生错误: {ex.Message}");
                // 关闭连接
                Disconnect();
            }
        }


        private byte[] HexStringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

    }

    public class TcpClientConfig : IChannelConfig
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }

}
