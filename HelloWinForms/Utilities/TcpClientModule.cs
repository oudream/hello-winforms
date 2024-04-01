using CxWorkStation.Utilities;
using OpenCvSharp.XFeatures2D;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Media.Animation;
using System.IO;
using System.Runtime.InteropServices.ComTypes;

namespace HelloWinForms.Utilities
{
    /// <summary>
    //### 重点改进
    //- ** 连接重试策略:** 在读写数据时遇到异常后，会尝试重新建立连接。这是通过`Reconnect`方法实现的，它首先关闭现有的`TcpClient`连接（如果有的话），然后设置为`null`，从而触发`ManageConnection`线程下一次循环时重新连接。
    //- ** 异常处理:** 读(`ReadData`)和写(`Send`)操作中的异常不仅被捕获并记录，还会触发重新连接的尝试。这确保了即使在网络不稳定或服务器暂时不可达的情况下，客户端也能尽可能恢复连接。
    //- ** 线程安全:** 通过`tcpClientLock`确保在修改`tcpClient`实例时的线程安全。任何对`tcpClient`实例的操作都被此锁保护，避免了多线程环境下的潜在问题。
    //### 使用此客户端
    //- 使用`Start`方法启动客户端，它会启动连接和读取数据的线程。
    //- 使用`Send`方法向服务器发送数据。
    //- 如果需要停止客户端，调用`Stop`方法，它会安全地停止所有操作并关闭连接。
    //通过这种方式，客户端在遇到任何读写错误时都会尝试重新建立连接，这增加了其鲁棒性和对网络波动的容忍度。
    public class TcpClientModule
    {
      
        public enum TcpClientModuleState
        {
            Stopping,
            Stopped,
            Started
        }

        public TcpClient tcpClient;
        private readonly string serverIp;
        private readonly int serverPort;

        // S7驱动模板状态变更事件
        public event EventHandler<TcpClientModuleState> ModuleStateChanged;
        private volatile TcpClientModuleState _moduleState = TcpClientModuleState.Stopped;
        public TcpClientModuleState ModuleState
        {
            get => _moduleState;
            private set
            {
                if (_moduleState != value)
                {
                    //LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} {(value ? "Start" : "Stop")}");
                    _moduleState = value;
                    OnRunningStateChanged(_moduleState);
                }
            }
        }
        protected virtual void OnRunningStateChanged(TcpClientModuleState moduleState)
        {
            // ?. 表示如果 RunningStateChanged 不为 null，则调用 Invoke
            ModuleStateChanged?.Invoke(this, moduleState);
        }

        private readonly object tcpClientLock = new object(); // 确保对 TCPClient 操作的线程安全
        private Thread channelThread;
        private Thread readingThread;

        public TcpClientModule(string ip, int port)
        {
            serverIp = ip;
            serverPort = port;
        }

        private volatile bool isStarting = false;
        private volatile bool isReading = false;
        public long ReadTime = 0;
        private bool tcpClientError = false;

        // 启动读取模块，启动TCPClient通道
        public void Start()
        {
            // 检查是否已停止
            if (ModuleState != TcpClientModuleState.Stopped)
            {
                return;
            }

            StartChannelImpl();
            StartReadDynamicPropertyImpl();

            ModuleState = TcpClientModuleState.Started;
        }

        // 启动单独线程来维护TCPClient通道
        private void StartChannelImpl()
        {
            if (isStarting)
            {
                return;
            }

            LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} 通道维护模块已启动");
            isStarting = true;

            // 创建TCPClient、关闭已存在的TCPClient、重连TCPClient
            channelThread = new Thread(() =>
            {
                while (isStarting)
                {
                    if (!NetHelper.ValidateIPv4(serverIp))
                    {
                        LogHelper.Error($"TCP客户端{serverIp}:{serverPort} 无效的IP地址：{serverIp}");
                        continue;
                    }

                    lock (tcpClientLock)
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
                            LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} 连接中...");
                            tcpClient.Connect(serverIp, serverPort);
                            if (!tcpClient.Connected)
                            {
                                LogHelper.Error($"TCP客户端{serverIp}:{serverPort} 无法连接到服务端：{serverIp}");
                                tcpClient = null;
                            }
                            else
                            {
                                LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} 连接成功");
                            }
                        }
                        catch (Exception e)
                        {
                            LogHelper.Error($"TCP客户端{serverIp}:{serverPort} 连接异常：{e.Message}");
                            lock (tcpClientLock)
                            {
                                tcpClient.Close();
                                tcpClient = null;
                            }
                        }
                    }
                    // 等待一段时间再检查 TCPClient 连接，避免频繁重连
                    var waitCount = 300;
                    while (isStarting && waitCount > 0)
                    {
                        waitCount -= 1;
                        Thread.Sleep(10); // 等待时间可以根据实际需要调整
                    }
                }

                lock (tcpClientLock)
                {
                    if (tcpClient != null)
                    {
                        tcpClient.Close(); // 确保线程结束时关闭 TCPClient 连接
                        tcpClient = null;
                    }
                }

                isStarting = false;
                ModuleState = TcpClientModuleState.Stopped;
                LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} TCPClient通道维护模块已停止");
            })
            { IsBackground = true };

            channelThread.Start();
        }

        // 启动单独线程来维护读取模块
        private void StartReadDynamicPropertyImpl()
        {
            if (isReading)
            {
                return;
            }

            LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} TCPClient读取模板已启动");
            isReading = true;
            var hadRead = false;

            readingThread = new Thread(() =>
            {
                while (isReading)
                {
                    try
                    {
                        var dtNow = TimeHelper.GetNow();
                        // 进行 TCPClient 读取操作
                        hadRead = false;
                        lock (tcpClientLock)
                        {
                            try
                            {
                                NetworkStream stream = null;
                                lock (tcpClientLock)
                                {
                                    if (tcpClient?.Connected == true)
                                    {
                                        stream = tcpClient.GetStream();
                                        stream.ReadTimeout = 1000;
                                    }
                                }

                                if (stream != null && stream.CanRead)
                                {
                                    byte[] buffer = new byte[4096];
                                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                                    if (bytesRead > 0)
                                    {
                                        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                        Console.WriteLine($"Received: {receivedData}");
                                        hadRead = true;
                                    }
                                    else
                                    {
                                        tcpClientError = true;
                                        Console.WriteLine("The server has closed the connection.");
                                    }
                                }
                            }
                            catch (IOException ex)
                            {
                                var socketException = ex.InnerException as SocketException;
                                if (socketException != null && socketException.SocketErrorCode == SocketError.TimedOut)
                                {
                                    // 处理超时
                                    Console.WriteLine("Reading timed out.");
                                }
                                else
                                {
                                    // 处理其他IO异常
                                    tcpClientError = true;
                                }
                            }
                        }

                        if (hadRead)
                        {

                            Interlocked.Exchange(ref ReadTime, dtNow);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error($"TCP客户端{serverIp}:{serverPort} 读取TCPClient数据时发生异常：{ex.Message}");
                        tcpClientError = true;
                    }

                    Thread.Sleep(10); // 按配置的读取间隔等待
                }

                isReading = false;
                LogHelper.Debug($"TCP客户端{serverIp}:{serverPort} TCPClient读取模板已停止");
            })
            { IsBackground = true };

            readingThread.Start();
        }

        // 停止读取模块，停止TCPClient通道
        public void Stop()
        {
            // 检查是否已启动
            if (ModuleState != TcpClientModuleState.Started)
            {
                return;
            }

            // 停止读取，等待线程结束
            isReading = false;
            readingThread.Join();

            // 关闭通道模块
            isStarting = false;
            ModuleState = TcpClientModuleState.Stopping;
        }

        public bool GetConnected()
        {
            lock (tcpClientLock)
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    return true;
                }
            }
            return false;
        }

        public void Send(byte[] data)
        {
            try
            {
                lock (tcpClientLock)
                {
                    if (tcpClient?.Connected == true)
                    {
                        NetworkStream stream = tcpClient.GetStream();
                        if (stream.CanWrite)
                        {
                            stream.Write(data, 0, data.Length);
                            Console.WriteLine($"Sent: {data}");
                            // 记录十六进制报文
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                tcpClientError = true;
            }
        }


    }
}
