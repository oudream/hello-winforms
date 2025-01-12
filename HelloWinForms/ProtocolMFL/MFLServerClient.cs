using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.ProtocolMFL
{
    public class MFLServerClient : ProtocolMFL
    {
        private string _clientIp;
        public string ClientIp { get; }

        private TcpClient _client;
        public TcpClient Client { get; }

        private readonly object _statusLock = new object(); // 锁对象
        private ClientStatus _status = ClientStatus.Idle; // 初始状态为空闲

        // 获取状态的方法
        public ClientStatus GetStatus()
        {
            lock (_statusLock)
            {
                return _status;
            }
        }

        // 设置状态的方法
        public void SetStatus(ClientStatus newStatus)
        {
            lock (_statusLock)
            {
                _status = newStatus;
            }
        }

        // 定义事件来发布解析结果
        public event Action<ClientResponseData> OnClientResponseDataParsed;
        public event Action<ClientRequestSuspendData> OnClientRequestSuspendDataParsed;

        public MFLServerClient(string ip, TcpClient client)
        {
            _clientIp = ip;
            _client = client;
        }

        protected override void ProcessMessage()
        {
            switch (_functionCode)
            {
                case 0x00000102:
                    // 接收结果
                    ParseClientResponseData();
                    break;
                case 0x00000103:
                    // 请求挂起
                    ParseClientRequestSuspendData();
                    break;
                default:
                    LogHelper.Error("Unknown function code");
                    break;
            }
        }

        private void ParseClientResponseData()
        {
            var responseData = ClientResponseData.Parse(_dataBody);
            //LogHelper.Debug("Client Response Parsed: " + responseData);

            SetStatus(ClientStatus.Idle); // 更新状态为空闲
        
            // 触发事件
            OnClientResponseDataParsed?.Invoke(responseData);
        }

        private void ParseClientRequestSuspendData()
        {
            var suspendRequestData = ClientRequestSuspendData.Parse(_dataBody);
            //LogHelper.Debug("Client Request Suspend Parsed: " + suspendRequestData);

            SetStatus(ClientStatus.Suspended); // 更新状态为挂起

            // 触发事件
            OnClientRequestSuspendDataParsed?.Invoke(suspendRequestData);
        }

        // try-catch 捕获异常来判断数据是否发送成功，这种方式只能确认数据是否成功写入网络流，
        // 但无法确定客户端是否成功接收数据。
        public bool SendData(byte[] data)
        {
            try
            {
                NetworkStream stream = _client.GetStream();
                stream.Write(data, 0, data.Length);
                stream.Flush();
                return true;

            }
            catch (Exception ex)
            {
                LogHelper.Error($"数据发送失败: {ex.Message}");
            }
            return false;
        }

        public void SendXRayRequest(byte[] data)
        {
            if (SendData(data))
            {
                SetStatus(ClientStatus.Busy); // 发送请求后设置为忙状态
            }
        }

        public void Close()
        {
            _client.Close();
        }

    }
}
