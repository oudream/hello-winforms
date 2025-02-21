using CommonInterfaces;
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

        private TcpClient _tcpClient;
        private Thread _clientThread;
        private readonly object _lockObject = new object();
        private bool _running = false;
        private bool _tcpClientError = false;
        private long _tryConnectTime = 0;

        // 服务器的IP地址和端口
        private string _serverIp;
        private ushort _serverPort;

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

        // 启动读取模块，启动TCPClient通道
        public void Start(string ip, ushort port)
        {
            // 检查是否已停止
            if (ModuleState != TcpClientModuleState.Stopped)
            {
                return;
            }

            _serverIp = ip;
            _serverPort = port;

            StartClientThread();

            ModuleState = TcpClientModuleState.Started;
        }

        private void StartClientThread()
        {
            if (_running)
            {
                return;
            }

            LogHelper.Debug($"TCP客户端{_serverIp}:{_serverPort} TCPClient读取模板已启动");
            _running = true;

            NetworkStream networkStream = null;

            _clientThread = new Thread(() =>
            {
                while (_running)
                {
                    try
                    {
                        lock (_lockObject)
                        {
                            // 关闭已存在问题的TCPClient
                            if (_tcpClientError)
                            {
                                if (_tcpClient != null)
                                {
                                    _tcpClient.Close();
                                    _tcpClient = null;
                                }
                                _tcpClientError = false;
                            }
                            // 创建TCPClient
                            if (_tcpClient == null)
                            {
                                _tcpClient = new TcpClient();
                            }
                        }

                        // 检查TCPClient是否连接，如果没有连接，则尝试重新连接
                        if (_tcpClient.Connected == false)
                        {
                            try
                            {
                                // 如果连接时间超过1秒，则尝试重新连接。防止连接过快
                                if (TimeHelper.GetNow() - _tryConnectTime < 1000)
                                {
                                    var waitCount = 300;
                                    while (_running && waitCount > 0)
                                    {
                                        waitCount -= 1;
                                        Thread.Sleep(10); // 等待时间可以根据实际需要调整
                                    }
                                }
                                LogHelper.Debug($"TCP客户端{_serverIp}:{_serverPort} 连接中...");
                                _tryConnectTime = TimeHelper.GetNow();
                                _tcpClient.Connect(_serverIp, _serverPort);
                                if (_tcpClient.Connected)
                                {
                                    LogHelper.Debug($"TCP客户端{_serverIp}:{_serverPort} 连接成功");
                                }
                            }
                            catch (Exception e)
                            {
                                lock (_lockObject)
                                {
                                    _tcpClient.Close();
                                    _tcpClient = null;
                                }
                                LogHelper.Error($"TCP客户端{_serverIp}:{_serverPort} 连接异常：{e.Message}");
                                continue;
                            }
                            if (!_tcpClient.Connected)
                            {
                                lock (_lockObject)
                                {
                                    _tcpClient.Close();
                                    _tcpClient = null;
                                }
                                LogHelper.Error($"TCP客户端{_serverIp}:{_serverPort} 无法连接到服务端：{_serverIp}");
                            }
                        }

                        if (_tcpClient != null && _tcpClient.Connected == true)
                        {
                            NetworkStream stream = _tcpClient.GetStream();
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
                                lock (_lockObject)
                                {
                                    _tcpClient.Close();
                                    _tcpClient = null;
                                }
                                LogHelper.Debug($"TCP客户端{_serverIp}:{_serverPort} TCPClient通道已断开");
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
                        _tcpClientError = true;
                        LogHelper.Debug($"TCP客户端{_serverIp}:{_serverPort} TCPClient通道异常，尝试重新连接：{ex.Message}");
                    }
                }

                _running = false;
                ModuleState = TcpClientModuleState.Stopped;
                _tcpClient = null;
                LogHelper.Debug($"TCP客户端{_serverIp}:{_serverPort} TCPClient通道维护模块已停止");
            })
            { IsBackground = true };

            _clientThread.Start();
        }

        public bool Send(byte[] data)
        {
            lock (_lockObject)
            {
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    try
                    {
                        NetworkStream stream = _tcpClient.GetStream();
                        stream.Write(data, 0, data.Length);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        _tcpClientError = true;
                        LogHelper.Debug($"TCP客户端{_serverIp}:{_serverPort} 发送异常，尝试重新连接：{ex.Message}");
                    }
                }
            }
            return false;
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
            _running = false;
            lock (_lockObject)
            {
                if (_tcpClient?.Connected == true)
                {
                    _tcpClient.Close();
                }
            }
        }

        public bool IsConnected()
        {
            lock (_lockObject)
            {
                if (_tcpClient?.Connected == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
