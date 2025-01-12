using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.ProtocolMFL
{
    public class MFLClient : ProtocolMFL
    {
        private string _clientIp;
        public string ClientIp { get; }

        private TcpClient _client;
        public TcpClient Client { get; }

        public ClientResponseData ResponseData { get; private set; }
        public ClientRequestSuspendData SuspendRequestData { get; private set; }
        public ClientResponseSuspendData SuspendResponseData { get; private set; }

        // 定义事件来发布解析结果
        public event Action<ClientResponseData> OnClientResponseDataParsed;
        public event Action<ClientRequestSuspendData> OnClientRequestSuspendDataParsed;
        public event Action<ClientResponseSuspendData> OnClientResponseSuspendDataParsed;

        public MFLClient(string ip, TcpClient client)
        {
            _clientIp = ip;
            _client = client;
        }

        protected override void ProcessMessage()
        {
            switch (_functionCode)
            {
                case 0x00000101:
                    ParseXRayRequestData();
                    break;
                case 0x00000104:
                    ParseClientResponseSuspendData();
                    break;
                default:
                    Console.WriteLine("Unknown function code");
                    break;
            }
        }

        private void ParseXRayRequestData()
        {
            // 假设已实现的 XRayRequestData 类
            var requestData = XRayRequestData.Parse(_dataBody);
            Console.WriteLine("X-Ray Request Parsed: " + requestData);
        }

        private void ParseClientResponseSuspendData()
        {
            SuspendResponseData = ClientResponseSuspendData.Parse(_dataBody);
            Console.WriteLine("Client Response Suspend Parsed: " + SuspendResponseData);

            // 触发事件
            OnClientResponseSuspendDataParsed?.Invoke(SuspendResponseData);
        }
    }
}
