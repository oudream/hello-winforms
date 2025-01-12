using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.ProtocolMFL
{
    public enum ClientStatus
    {
        Idle,   // 空闲状态
        Busy,   // 忙碌状态
        Suspended, // 挂起状态
        TimeOut // 超时状态
    }

    // X-Ray发起判定请求数据详情（功能码：0x00000101)
    public class XRayRequestData
    {
        public uint Line { get; private set; }
        public uint Batch { get; private set; }
        public uint Position { get; private set; }
        public uint TrayPos { get; private set; }
        public string SerialNumber { get; private set; }
        public byte[] ImageData { get; private set; }

        // 静态方法：从字节数组解析数据并创建 XRayRequestData 对象
        public static XRayRequestData Parse(byte[] data)
        {
            var requestData = new XRayRequestData();
            int index = 0;

            requestData.Line = BitConverter.ToUInt32(data, index); index += 4;
            requestData.Batch = BitConverter.ToUInt32(data, index); index += 4;
            requestData.Position = BitConverter.ToUInt32(data, index); index += 4;
            requestData.TrayPos = BitConverter.ToUInt32(data, index); index += 4;

            uint snLength = BitConverter.ToUInt32(data, index); index += 4;
            requestData.SerialNumber = Encoding.UTF8.GetString(data, index, (int)snLength);
            index += (int)snLength;

            uint imageLength = BitConverter.ToUInt32(data, index); index += 4;
            requestData.ImageData = new byte[imageLength];
            Array.Copy(data, index, requestData.ImageData, 0, imageLength);

            return requestData;
        }

        // 序列化方法：将 XRayRequestData 对象转成字节数组
        public byte[] ToByteArray()
        {
            var snBytes = Encoding.UTF8.GetBytes(SerialNumber ?? string.Empty);
            uint snLength = (uint)snBytes.Length;

            var imageLength = (uint)(ImageData?.Length ?? 0);

            int totalLength = 4 + 4 + 4 + 4 + 4 + snBytes.Length + 4 + (int)imageLength;
            byte[] result = new byte[totalLength];
            int index = 0;

            Array.Copy(BitConverter.GetBytes(Line), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(Batch), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(Position), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(TrayPos), 0, result, index, 4); index += 4;

            Array.Copy(BitConverter.GetBytes(snLength), 0, result, index, 4); index += 4;
            Array.Copy(snBytes, 0, result, index, snBytes.Length); index += snBytes.Length;

            Array.Copy(BitConverter.GetBytes(imageLength), 0, result, index, 4); index += 4;
            if (ImageData != null)
            {
                Array.Copy(ImageData, 0, result, index, ImageData.Length);
            }

            return result;
        }

        public override string ToString()
        {
            return $"Line: {Line}, Batch: {Batch}, Position: {Position}, TrayPos: {TrayPos}, SN: {SerialNumber}, ImageSize: {ImageData.Length} bytes";
        }
    }

    // client回复判定结果数据详情（功能码：0x00000102)				
    public class ClientResponseData : ProtocolMFL
    {
        public uint Line { get; set; }
        public uint Batch { get; set; }
        public uint Position { get; set; }
        public uint ResultLength { get; set; }
        public string Result { get; set; }
        public uint TrayPos { get; set; }

        // 静态方法：从字节数组解析数据并创建 ClientResponseData 对象
        public static ClientResponseData Parse(byte[] data)
        {
            var responseData = new ClientResponseData();
            int index = 0;

            responseData.Line = BitConverter.ToUInt32(data, index); index += 4;
            responseData.Batch = BitConverter.ToUInt32(data, index); index += 4;
            responseData.Position = BitConverter.ToUInt32(data, index); index += 4;

            responseData.ResultLength = BitConverter.ToUInt32(data, index); index += 4;
            responseData.Result = Encoding.UTF8.GetString(data, index, (int)responseData.ResultLength);
            index += (int)responseData.ResultLength;

            responseData.TrayPos = BitConverter.ToUInt32(data, index);

            return responseData;
        }

        // 序列化方法：将 ClientResponseData 对象转成字节数组
        public byte[] ToByteArray()
        {
            var resultBytes = Encoding.UTF8.GetBytes(Result ?? string.Empty);
            uint resultLength = (uint)resultBytes.Length;

            int totalLength = 4 + 4 + 4 + 4 + 4 + resultBytes.Length + 4;
            byte[] result = new byte[totalLength];
            int index = 0;

            Array.Copy(BitConverter.GetBytes(Line), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(Batch), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(Position), 0, result, index, 4); index += 4;

            Array.Copy(BitConverter.GetBytes(resultLength), 0, result, index, 4); index += 4;
            Array.Copy(resultBytes, 0, result, index, resultBytes.Length); index += resultBytes.Length;

            Array.Copy(BitConverter.GetBytes(TrayPos), 0, result, index, 4);

            return result;
        }

        public override string ToString()
        {
            return $"Line: {Line}, Batch: {Batch}, Position: {Position}, Result: {Result}, TrayPos: {TrayPos}";
        }
    }

    public class ClientResponseSuspendData
    {
        public string Response { get; set; }

        // 静态方法：从字节数组解析数据并创建 ClientResponseSuspendData 对象
        public static ClientResponseSuspendData Parse(byte[] data)
        {
            var responseData = new ClientResponseSuspendData();
            responseData.Response = Encoding.UTF8.GetString(data).TrimEnd('\0');
            return responseData;
        }

        // 序列化方法：将 ClientResponseSuspendData 对象转成字节数组
        public byte[] ToByteArray()
        {
            var responseBytes = Encoding.UTF8.GetBytes(Response);
            Array.Resize(ref responseBytes, 64); // 固定长度64字节
            return responseBytes;
        }
    }

    public class ClientRequestSuspendData
    {
        public string Reason { get; set; }

        // 静态方法：从字节数组解析数据并创建 ClientRequestSuspendData 对象
        public static ClientRequestSuspendData Parse(byte[] data)
        {
            var requestData = new ClientRequestSuspendData();
            requestData.Reason = Encoding.UTF8.GetString(data).TrimEnd('\0');
            return requestData;
        }

        // 序列化方法：将 ClientRequestSuspendData 对象转成字节数组
        public byte[] ToByteArray()
        {
            var reasonBytes = Encoding.UTF8.GetBytes(Reason);
            Array.Resize(ref reasonBytes, 64); // 固定长度64字节
            return reasonBytes;
        }
    }

}
