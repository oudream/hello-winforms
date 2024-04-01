using CxWorkStation.Utilities;
using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWinForms.Channel
{
    public class TcpClientChannel
    {
        public enum TcpClientModuleState
        {
            Stopping,
            Stopped,
            Started
        }

        private TcpClient tcpClient;
        private Thread clientThread;
        private readonly object lockObject = new object();
        private bool running = false;
        private bool tcpClientError = false;
        private long tryConnectTime = 0;

        // 服务器的IP地址和端口
        private readonly string serverIp;
        private readonly ushort serverPort;

        // 状态属性
        private TcpClientModuleState state = TcpClientModuleState.Stopped;
        public TcpClientModuleState ModuleState
        {
            get { return state; }
            private set
            {
                state = value;
                OnStateChanged(state);
            }
        }
        // 状态改变事件
        public event Action<TcpClientModuleState> StateChanged;

        private byte[] buffer = new byte[8192];

        // 数据接收事件
        public event Action<byte[], int> DataReceived;

        public TcpClientChannel(string ip, ushort port)
        {
            serverIp = ip;
            serverPort = port;
        }

        // 启动读取模块，启动TCPClient通道
        public void Start()
        {
            // 检查是否已停止
            if (ModuleState != TcpClientModuleState.Stopped)
            {
                return;
            }

            StartClientThread();

            ModuleState = TcpClientModuleState.Started;
        }

        private void StartClientThread()
        {
            if (running)
            {
                return;
            }

            LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} TCPClient读取模板已启动");
            running = true;

            NetworkStream networkStream = null;

            clientThread = new Thread(() =>
            {
                while (running)
                {
                    try
                    {
                        lock (lockObject)
                        {
                            // 关闭已存在问题的TCPClient
                            if (tcpClientError)
                            {
                                if (tcpClient != null)
                                {
                                    tcpClient.Close();
                                    tcpClient = null;
                                }
                                tcpClientError = false;
                            }
                            // 创建TCPClient
                            if (tcpClient == null)
                            {
                                tcpClient = new TcpClient();
                            }
                        }

                        // 检查TCPClient是否连接，如果没有连接，则尝试重新连接
                        if (!tcpClient.Connected)
                        {
                            try
                            {
                                // 如果连接时间超过1秒，则尝试重新连接。防止连接过快
                                if (TimeHelper.GetNow() - tryConnectTime < 1000)
                                {
                                    var waitCount = 300;
                                    while (running && waitCount > 0)
                                    {
                                        waitCount -= 1;
                                        Thread.Sleep(10); // 等待时间可以根据实际需要调整
                                    }
                                }
                                LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} 连接中...");
                                tryConnectTime = TimeHelper.GetNow();
                                tcpClient.Connect(serverIp, serverPort);
                                if (tcpClient.Connected)
                                {
                                    LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} 连接成功");
                                }
                            }
                            catch (Exception e)
                            {
                                LogHelper.Error($"TCP客户端{serverIp}:{serverPort} 连接异常：{e.Message}");
                            }
                            if (!tcpClient.Connected)
                            {
                                lock (lockObject)
                                {
                                    tcpClient.Close();
                                    tcpClient = null;
                                }
                                LogHelper.Error($"TCP客户端{serverIp}:{serverPort} 无法连接到服务端：{serverIp}");
                            }
                        }

                        if (tcpClient?.Connected == true)
                        {
                            NetworkStream stream = tcpClient.GetStream();
                            if (stream != networkStream)
                            {
                                networkStream = stream;
                                networkStream.ReadTimeout = 1000; // 1秒
                            }

                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead > 0)
                            {
                                OnDataReceived(buffer, bytesRead);
                            }
                            else if (bytesRead == 0)
                            {
                                lock (lockObject)
                                {
                                    tcpClient.Close();
                                    tcpClient = null;
                                }
                                LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} TCPClient通道已断开");
                            }
                        }
                    }
                    catch (IOException ex) when (ex.InnerException is SocketException socketEx && (socketEx.SocketErrorCode == SocketError.TimedOut))
                    {
                        Console.WriteLine("Read timed out.");
                    }
                    catch (Exception ex)
                    {
                        // 除了超时异常，其他都认为是异常，则尝试重新连接
                        tcpClientError = true;
                        LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} TCPClient通道异常，尝试重新连接：{ex.Message}");
                    }
                }

                running = false;
                ModuleState = TcpClientModuleState.Stopped;
                tcpClient = null;
                LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} TCPClient通道维护模块已停止");
            })
            { IsBackground = true };

            clientThread.Start();
        }

        public void Send(byte[] data)
        {
            lock (lockObject)
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    try
                    {
                        NetworkStream stream = tcpClient.GetStream();
                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        tcpClientError = true;
                        LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} 发送异常，尝试重新连接：{ex.Message}");
                    }
                }
            }
        }

        protected virtual void OnDataReceived(byte[] data, int bytesRead)
        {
            DataReceived?.Invoke(data, bytesRead);
        }

        protected virtual void OnStateChanged(TcpClientModuleState newState)
        {
            StateChanged?.Invoke(newState);
        }

        public void Stop()
        {
            // 检查是否已启动
            if (ModuleState != TcpClientModuleState.Started)
            {
                return;
            }

            ModuleState = TcpClientModuleState.Stopping;

            // 停止读取，等待线程结束
            running = false;
            lock (lockObject)
            {
                if (tcpClient?.Connected == true)
                {
                    tcpClient.Close();
                }
            }
        }

        public bool IsConnected()
        {
            lock (lockObject)
            {
                if (tcpClient?.Connected == true)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
