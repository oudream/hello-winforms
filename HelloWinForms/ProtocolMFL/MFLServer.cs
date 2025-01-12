using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog.Core;
using HelloWinForms.Utilities;

namespace HelloWinForms.ProtocolMFL
{
    public class MFLServer
    {
        private readonly int _port;
        private TcpListener _listener;
        private volatile bool _isRunning = false;

        private readonly object _clientsLock = new object();
        private Dictionary<string, MFLServerClient> _clients = new Dictionary<string, MFLServerClient>(); // key: clientIp>

        public bool IsRunning => _isRunning;

        public MFLServer(int port)
        {
            _port = port;
        }

        public void Start()
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                _isRunning = true;
                LogHelper.Debug($"MFL服务 端口[{_port}]已成功启动");

                new Thread(() =>
                {
                    while (_isRunning)
                    {
                        try
                        {
                            var client = _listener.AcceptTcpClient();
                            IPEndPoint remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
                            if (remoteEndPoint != null)
                            {
                                string clientIp = remoteEndPoint.Address.ToString();
                                LogHelper.Debug($"MFL服务 客户端[{clientIp}]已连接");

                                MFLServerClient clientObj = new MFLServerClient(clientIp, client);
                                lock (_clientsLock)
                                {
                                    _clients.Add(clientIp, clientObj);
                                }

                                new Thread(() => HandleClient(clientObj)).Start();
                            }
                        }
                        catch (SocketException ex)
                        {
                            LogHelper.Debug($"MFL服务 Socket 异常: {ex.Message}");
                            Stop();
                        }
                    }
                })
                { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                LogHelper.Debug($"MFL服务 启动失败: {ex.Message}");
            }
        }

        // 处理客户端请求.loop
        private void HandleClient(MFLServerClient clientObj)
        {
            try
            {
                using (NetworkStream stream = clientObj.Client.GetStream())
                {
                    byte[] buffer = new byte[1024 * 1024 * 10];
                    int bytesRead;
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        clientObj.ParseData(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MFL服务 客户端处理错误: {ex.Message}");
            }
            finally
            {
                lock (_clientsLock)
                {
                    clientObj.Close();
                    _clients.Remove(clientObj.ClientIp);
                }
                LogHelper.Debug($"MFL服务 客户端[{clientObj.ClientIp}]已断开");
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener?.Stop();
            LogHelper.Debug($"MFL服务 端口[{_port}]已停止。");
        }

        public void SendXRayRequest(XRayRequestData requestData)
        {
            MFLServerClient idleClient = null;
            lock (_clientsLock)
            {
                // 查找一个空闲的客户端
                idleClient = _clients.Values.FirstOrDefault(c => c.GetStatus() == ClientStatus.Idle);
            }

            if (idleClient != null)
            {
                // 序列化 XRayRequestData
                byte[] data = requestData.ToByteArray();

                // 创建功能码字节数组 (0x00000101)
                byte[] functionCode = BitConverter.GetBytes(0x00000101);

                // 合并功能码和序列化数据
                byte[] payload = new byte[functionCode.Length + data.Length];
                Array.Copy(functionCode, 0, payload, 0, functionCode.Length);
                Array.Copy(data, 0, payload, functionCode.Length, data.Length);

                // 使用空闲客户端发送请求
                idleClient.SendXRayRequest(payload);
                LogHelper.Debug($"MFL服务 发送图像检测请求到客户端 {idleClient.ClientIp}");
            }
            else
            {
                LogHelper.Error("MFL服务 没有空闲的客户端可用，无法发送图像检测请求");
            }
        }
    }
}
