using System;
using System.Net.Sockets;
using System.Threading;
using NModbus.Device;
using NModbus;
using CxWorkStation.Utilities;
using HelloWinForms.Utilities;
using System.IO;
using System.Collections.Generic;
using CommonInterfaces;

namespace HelloWinForms.Protocols
{
    public class ModbusTcpClient
    {
        public enum ClientState
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
        private ClientState state = ClientState.Stopped;
        public ClientState ModuleState
        {
            get { return state; }
            private set
            {
                state = value;
                OnStateChanged(state);
            }
        }
        // 状态改变事件
        public event Action<ClientState> StateChanged;

        // 线圈接收事件 slaveAddress, startAddress, coils
        public event Action<byte, ushort, bool[]> CoilsReceived;
        public event Action<byte, ushort, ushort[]> RegistersReceived;

        // modbus
        private IModbusMaster _modbusMaster;
        private ModbusFactory _modbusFactory;

        public ModbusTcpClient()
        {
            _modbusFactory = new ModbusFactory();
        }

        // 启动读取模块，启动TCPClient通道
        public void Start(string ip, ushort port, List<S7ReadIntervalGroup> s7ReadIntervalGroups)
        {
            // 检查是否已停止
            if (ModuleState != ClientState.Stopped)
            {
                return;
            }

            _serverIp = ip;
            _serverPort = port;

            StartClientThread(s7ReadIntervalGroups);

            ModuleState = ClientState.Started;
        }

        private void StartClientThread(List<S7ReadIntervalGroup> readIntervalGroups)
        {
            if (_running)
            {
                return;
            }

            LogHelper.Debug($"ModbusTcp{_serverIp}:{_serverPort} TCPClient读取模板已启动");
            _running = true;

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
                                _modbusMaster = _modbusFactory.CreateMaster(_tcpClient);
                                _modbusMaster.Transport.ReadTimeout = 100;
                                _modbusMaster.Transport.WriteTimeout = 100;
                            }
                        }

                        // 检查TCPClient是否连接，如果没有连接，则尝试重新连接
                        if (!_tcpClient.Connected)
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
                                LogHelper.Debug($"ModbusTcp{_serverIp}:{_serverPort} 连接中...");
                                _tryConnectTime = TimeHelper.GetNow();
                                _tcpClient.Connect(_serverIp, _serverPort);
                                if (_tcpClient.Connected)
                                {
                                    LogHelper.Debug($"ModbusTcp{_serverIp}:{_serverPort} 连接成功");
                                }
                            }
                            catch (Exception e)
                            {
                                lock (_lockObject)
                                {
                                    _tcpClient.Close();
                                    _tcpClient = null;
                                }
                                LogHelper.Error($"ModbusTcp{_serverIp}:{_serverPort} 连接异常：{e.Message}");
                                continue;
                            }
                            if (!_tcpClient.Connected)
                            {
                                lock (_lockObject)
                                {
                                    _tcpClient.Close();
                                    _tcpClient = null;
                                }
                                LogHelper.Error($"ModbusTcp{_serverIp}:{_serverPort} 无法连接到服务端：{_serverIp}");
                            }
                        }

                        if (_tcpClient?.Connected == true)
                        {
                            var dtNow = TimeHelper.GetNow();

                            foreach (var intervalGroup in readIntervalGroups)
                            {
                                if (intervalGroup.ReadInterval > 0 && dtNow - intervalGroup.LastReadTime > intervalGroup.ReadInterval)
                                {
                                    intervalGroup.LastReadTime = dtNow;
                                    foreach (var dbGroup in intervalGroup.DbGroups)
                                    {
                                        switch (dbGroup.FunctionCode)
                                        {
                                            case ModbusFunctionCodes.ReadCoils:
                                                {
                                                    bool[] coils = _modbusMaster.ReadCoils(dbGroup.SlaveAddress, dbGroup.StartAddress, dbGroup.Count);
                                                    CoilsReceived?.Invoke(dbGroup.SlaveAddress, dbGroup.StartAddress, coils);
                                                }
                                                break;
                                            case ModbusFunctionCodes.ReadHoldingRegisters:
                                                {
                                                    ushort[] registers = _modbusMaster.ReadHoldingRegisters(dbGroup.SlaveAddress, dbGroup.StartAddress, dbGroup.Count);
                                                    RegistersReceived?.Invoke(dbGroup.SlaveAddress, dbGroup.StartAddress, registers);
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (IOException ex) when (ex.InnerException is SocketException socketEx && (socketEx.SocketErrorCode == SocketError.TimedOut))
                    {
                        Console.WriteLine("Read timed out.");
                    }
                    // 读取为0时，抛出异常，表示服务器断开连接。
                    // 读取的其它异常
                    catch (Exception ex)
                    {
                        // 除了超时异常，其他都认为是异常，则尝试重新连接
                        _tcpClientError = true;
                        LogHelper.Debug($"ModbusTcp{_serverIp}:{_serverPort} TCPClient通道异常，尝试重新连接：{ex.Message}");
                    }
                }

                _running = false;
                ModuleState = ClientState.Stopped;
                _tcpClient = null;
                LogHelper.Debug($"ModbusTcp{_serverIp}:{_serverPort} TCPClient通道维护模块已停止");
            })
            { IsBackground = true };

            _clientThread.Start();
        }

        public void Send(byte[] data)
        {
            lock (_lockObject)
            {
                if (_tcpClient != null && _tcpClient.Connected)
                {
                    try
                    {
                        NetworkStream stream = _tcpClient.GetStream();
                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception ex)
                    {
                        _tcpClientError = true;
                        LogHelper.Debug($"ModbusTcp{_serverIp}:{_serverPort} 发送异常，尝试重新连接：{ex.Message}");
                    }
                }
            }
        }

        protected virtual void OnStateChanged(ClientState newState)
        {
            StateChanged?.Invoke(newState);
        }

        public void Stop()
        {
            // 检查是否已启动
            if (ModuleState != ClientState.Started)
            {
                return;
            }

            ModuleState = ClientState.Stopping;

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
