using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.Channel
{
    public abstract class ChannelClient
    {
        public abstract bool IsConnected { get; }
        public abstract void Connect(IChannelConfig config);
        public abstract void Disconnect();
        public abstract void Send(byte[] data);
        public abstract Task SendAsync(byte[] data);

        public event Action<byte[]> DataReceived;
        public event Action<string> ErrorOccurred;

        public abstract long BytesSent { get; }

        public abstract long BytesReceived { get; }


        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(data);
        }

        protected virtual void OnErrorOccurred(string errorMessage)
        {
            ErrorOccurred?.Invoke(errorMessage);
        }
    }

    public interface IChannelConfig
    {
        // 这里定义所有通道配置共有的属性和方法
    }

}
