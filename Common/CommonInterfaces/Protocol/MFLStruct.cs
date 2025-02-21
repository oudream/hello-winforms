using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    public enum ClientStatusEnum
    {
        Idle,   // 空闲状态
        Busy,   // 忙碌状态
        Suspended, // 挂起状态
        TimeOut // 超时状态
    }

    // X-Ray发起判定请求数据详情（功能码：0x00000101)
    public class XRayRequestData
    {
        public uint LineNumber { get; set; }
        public uint BatchNumber { get; set; }
        public uint Position { get; set; }
        public uint TrayPos { get; set; }
        public string MarkTitlesContext { get; set; } // SN|线号……检测时间|AIResult
        // 图像数据大小
        public uint ImageRows { get; set; }
        public uint ImageCols { get; set; }
        public uint ImageChannels { get; set; }
        // 图像数据
        public byte[] ImageData { get; set; }
        public string AIResultTexts { get; set; }
        public byte[] AIResultBoxes { get; set; }
        public string AIBoxString { get; set; }
        public uint RemoteDisplayAI { get; set; }

        // 静态方法：从字节数组解析数据并创建 XRayRequestData 对象
        public static bool TryParse(byte[] data, uint dataLength, out XRayRequestData request)
        {
            request = null;

            // 检查 data 是否为 null 或长度是否为零
            if (data == null || data.Length == 0)
            {
                return false;
            }

            // 检查 dataLength 是否有效，并且与 data 的长度匹配
            if (dataLength == 0 || dataLength > data.Length)
            {
                return false;
            }

            // 检查数据是否足够解析到 imageLength 字段
            int lengthPosition = (4 * 9);
            if (lengthPosition > dataLength)
            {
                return false;
            }

            // 数据合法性检查通过后，开始解析数据
            request = new XRayRequestData();
            int index = 0;

            request.LineNumber = BitConverter.ToUInt32(data, index); index += 4;
            request.BatchNumber = BitConverter.ToUInt32(data, index); index += 4;
            request.Position = BitConverter.ToUInt32(data, index); index += 4;
            request.TrayPos = BitConverter.ToUInt32(data, index); index += 4;
            request.ImageRows = BitConverter.ToUInt32(data, index); index += 4;
            request.ImageCols = BitConverter.ToUInt32(data, index); index += 4;
            request.ImageChannels = BitConverter.ToUInt32(data, index); index += 4;
            request.RemoteDisplayAI = BitConverter.ToUInt32(data, index); index += 4;

            // MarkTitlesContext
            uint markTitlesContextLength = BitConverter.ToUInt32(data, index); index += 4;
            lengthPosition += (int)markTitlesContextLength;
            if (lengthPosition > dataLength) return false;
            request.MarkTitlesContext = Encoding.UTF8.GetString(data, index, (int)markTitlesContextLength); index += (int)markTitlesContextLength;

            // ImageData
            uint imageLength = BitConverter.ToUInt32(data, index); index += 4;
            lengthPosition += (int)imageLength;
            if (lengthPosition > dataLength) return false;
            request.ImageData = new byte[imageLength];
            Array.Copy(data, index, request.ImageData, 0, imageLength); index += (int)imageLength;

            // AIResultText
            uint aiResultTextsLength = BitConverter.ToUInt32(data, index); index += 4;
            lengthPosition += (int)aiResultTextsLength;
            if (lengthPosition > dataLength) return false;
            request.AIResultTexts = Encoding.UTF8.GetString(data, index, (int)aiResultTextsLength); index += (int)aiResultTextsLength;

            // AIResultBox
            uint aiResultBoxesLength = BitConverter.ToUInt32(data, index); index += 4;
            lengthPosition += (int)aiResultBoxesLength;
            if (lengthPosition > dataLength) return false;
            request.AIResultBoxes = new byte[aiResultBoxesLength];
            Array.Copy(data, index, request.AIResultBoxes, 0, aiResultBoxesLength); index += (int)aiResultBoxesLength;

            return true;
        }

        // 序列化方法：将 XRayRequestData 对象转成字节数组
        public byte[] ToByteArray()
        {
            var markTitlesContestBytes = Encoding.UTF8.GetBytes(MarkTitlesContext ?? string.Empty);
            uint markTitlesContestLength = (uint)markTitlesContestBytes.Length;

            var imageLength = (uint)(ImageData?.Length ?? 0);

            var aiResultTextsBytes = Encoding.UTF8.GetBytes(AIResultTexts ?? string.Empty);
            uint aiResultTextsLength = (uint)aiResultTextsBytes.Length;

            var aiResultBoxesLength = (uint)(AIResultBoxes?.Length ?? 0);

            int totalLength = (4*9) + markTitlesContestBytes.Length + 4 + (int)imageLength + 
                4 + (int)aiResultTextsLength + 4 + (int)aiResultBoxesLength;
            byte[] result = new byte[totalLength];
            int index = 0;

            Array.Copy(BitConverter.GetBytes(LineNumber), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(BatchNumber), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(Position), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(TrayPos), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(ImageRows), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(ImageCols), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(ImageChannels), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(RemoteDisplayAI), 0, result, index, 4); index += 4;

            // MarkTitlesContext
            Array.Copy(BitConverter.GetBytes(markTitlesContestLength), 0, result, index, 4); index += 4;
            Array.Copy(markTitlesContestBytes, 0, result, index, markTitlesContestBytes.Length); index += markTitlesContestBytes.Length;

            // ImageData
            Array.Copy(BitConverter.GetBytes(imageLength), 0, result, index, 4); index += 4;
            if (ImageData != null)
            {
                Array.Copy(ImageData, 0, result, index, ImageData.Length); index += ImageData.Length;
            }

            // AIResultText
            Array.Copy(BitConverter.GetBytes(aiResultTextsLength), 0, result, index, 4); index += 4;
            Array.Copy(aiResultTextsBytes, 0, result, index, aiResultTextsBytes.Length); index += aiResultTextsBytes.Length;

            // AIResultBox
            Array.Copy(BitConverter.GetBytes(aiResultBoxesLength), 0, result, index, 4); index += 4;
            if (AIResultBoxes != null)
            {
                Array.Copy(AIResultBoxes, 0, result, index, AIResultBoxes.Length); index += AIResultBoxes.Length;
            }
            return result;
        }

        public override string ToString()
        {
            return $"Line: {LineNumber}, Batch: {BatchNumber}, Position: {Position}, TrayPos: {TrayPos}, MarkTitlesContext: {MarkTitlesContext}, ImageSize: {ImageData.Length} bytes";
        }
    }

    // client回复判定结果数据详情（功能码：0x00000102)				
    public class ClientResponseJudgeResultData
    {
        public uint Line { get; set; }
        public uint Batch { get; set; }
        public uint Position { get; set; }
        public string Result { get; set; }
        public string UserName { get; set; }
        public uint TrayPos { get; set; }
        // 画框、画圆、尺寸的序列化与反序列化
        public byte[] DrawState { get; set; }

        // 静态方法：从字节数组解析数据并创建 ClientResponseData 对象
        public static bool TryParse(byte[] data, uint dataLength, out ClientResponseJudgeResultData response)
        {
            response = null;

            // 检查 data 是否为 null 或长度是否为零
            if (data == null || data.Length == 0)
            {
                return false;
            }

            // 检查 dataLength 是否有效，并且与 data 的长度匹配
            if (dataLength == 0 || dataLength > data.Length)
            {
                return false;
            }

            // 检查数据是否足够解析到 imageLength 字段
            int lengthPosition = (4 * 7);
            if (lengthPosition > dataLength)
            {
                return false;
            }

            response = new ClientResponseJudgeResultData();
            int index = 0;

            response.Line = BitConverter.ToUInt32(data, index); index += 4;
            response.Batch = BitConverter.ToUInt32(data, index); index += 4;
            response.Position = BitConverter.ToUInt32(data, index); index += 4;

            int resultLength = (int)BitConverter.ToUInt32(data, index); index += 4;
            lengthPosition += resultLength;
            if (lengthPosition > dataLength || resultLength > 20 * 1024 * 1024)
            {
                return false;
            }
            response.Result = Encoding.UTF8.GetString(data, index, resultLength); index += resultLength;

            int userNmaeLength = (int)BitConverter.ToUInt32(data, index); index += 4;
            lengthPosition += userNmaeLength;
            if (lengthPosition > dataLength || userNmaeLength > 64)
            {
                return false;
            }
            response.UserName = Encoding.UTF8.GetString(data, index, userNmaeLength); index += userNmaeLength;

            response.TrayPos = BitConverter.ToUInt32(data, index); index += 4;

            // DrawState
            uint drawStateLength = BitConverter.ToUInt32(data, index); index += 4;
            lengthPosition += (int)drawStateLength;
            if (lengthPosition > dataLength) return false;
            response.DrawState = new byte[drawStateLength];
            Array.Copy(data, index, response.DrawState, 0, drawStateLength); index += (int)drawStateLength;

            return true;
        }

        // 序列化方法：将 ClientResponseData 对象转成字节数组
        public byte[] ToByteArray()
        {
            var resultBytes = Encoding.UTF8.GetBytes(Result ?? string.Empty);
            uint resultLength = (uint)resultBytes.Length;

            var userBytes = Encoding.UTF8.GetBytes(UserName ?? string.Empty);
            uint userLength = (uint)userBytes.Length;

            var drawStateLength = (uint)(DrawState?.Length ?? 0);

            // 计算总长度 Line Batch Position ...
            int totalLength = 4 + 4 + 4 + 4 + resultBytes.Length + 4 + userBytes.Length + 4 + 4 + (int)drawStateLength;
            byte[] result = new byte[totalLength];
            int index = 0;

            Array.Copy(BitConverter.GetBytes(Line), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(Batch), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(Position), 0, result, index, 4); index += 4;

            Array.Copy(BitConverter.GetBytes(resultLength), 0, result, index, 4); index += 4;
            Array.Copy(resultBytes, 0, result, index, resultBytes.Length); index += resultBytes.Length;

            Array.Copy(BitConverter.GetBytes(userLength), 0, result, index, 4); index += 4;
            Array.Copy(userBytes, 0, result, index, userBytes.Length); index += userBytes.Length;

            Array.Copy(BitConverter.GetBytes(TrayPos), 0, result, index, 4); index += 4;

            // DrawState
            Array.Copy(BitConverter.GetBytes(drawStateLength), 0, result, index, 4); index += 4;
            if (DrawState != null)
            {
                Array.Copy(DrawState, 0, result, index, DrawState.Length); index += DrawState.Length;
            }

            return result;
        }

        public override string ToString()
        {
            return $"Line: {Line}, Batch: {Batch}, Position: {Position}, Result: {Result}, UserName: {UserName}, TrayPos: {TrayPos}";
        }
    }

    public class ClientIDResponseData
    {
        // OK类型
        public uint OKLength { get; set; }
        public string OKString { get; set; }
        // NG类型
        public uint NGLength { get; set; }
        public string NGString { get; set; }
        // 放大比
        public double FodFddRatio { get; set; }
        // 图像的单位像素实际大小
        public double ImageUnitPixelActualSize { get; set; }
        // 字符大小
        public int FontMeasureSize { get; set; }
        // 画线宽度
        public int DrawLineWidth { get; set; }

        // 静态方法：从字节数组解析数据并创建 ClientResponseData 对象
        public static bool TryParse(byte[] data, uint dataLength, out ClientIDResponseData response)
        {
            response = null;

            // 检查 data 是否为 null 或长度是否为零
            if (data == null || data.Length == 0)
            {
                return false;
            }

            // 检查 dataLength 是否有效，并且与 data 的长度匹配
            if (dataLength == 0 || dataLength > data.Length)
            {
                return false;
            }

            // 检查数据是否足够解析字段
            if (dataLength < 4 + 4 + 8 + 8 + 4 + 4)
            {
                return false;
            }

            response = new ClientIDResponseData();
            int index = 0;

            try
            {
                // 解析 OK 部分
                response.OKLength = BitConverter.ToUInt32(data, index); index += 4;
                response.OKString = Encoding.UTF8.GetString(data, index, (int)response.OKLength); index += (int)response.OKLength;

                // 解析 NG 部分
                response.NGLength = BitConverter.ToUInt32(data, index); index += 4;
                response.NGString = Encoding.UTF8.GetString(data, index, (int)response.NGLength); index += (int)response.NGLength;

                // 解析新增字段
                response.FodFddRatio = BitConverter.ToDouble(data, index); index += 8;
                response.ImageUnitPixelActualSize = BitConverter.ToDouble(data, index); index += 8;
                response.FontMeasureSize = BitConverter.ToInt32(data, index); index += 4;
                response.DrawLineWidth = BitConverter.ToInt32(data, index); index += 4;

                return true;
            }
            catch
            {
                return false;
            }
        }

        // 序列化方法：将 ClientIDResponseData 对象转成字节数组
        public byte[] ToByteArray()
        {
            var okBytes = Encoding.UTF8.GetBytes(OKString ?? string.Empty);
            OKLength = (uint)okBytes.Length;
            var ngBytes = Encoding.UTF8.GetBytes(NGString ?? string.Empty);
            NGLength = (uint)ngBytes.Length;

            // 计算总长度
            int totalLength = 4 + (int)OKLength + 4 + (int)NGLength + 8 + 8 + 4 + 4;
            byte[] result = new byte[totalLength];
            int index = 0;

            // 序列化 OK 部分
            Array.Copy(BitConverter.GetBytes(OKLength), 0, result, index, 4); index += 4;
            Array.Copy(okBytes, 0, result, index, (int)OKLength); index += (int)OKLength;

            // 序列化 NG 部分
            Array.Copy(BitConverter.GetBytes(NGLength), 0, result, index, 4); index += 4;
            Array.Copy(ngBytes, 0, result, index, (int)NGLength); index += (int)NGLength;

            // 序列化新增字段
            Array.Copy(BitConverter.GetBytes(FodFddRatio), 0, result, index, 8); index += 8;
            Array.Copy(BitConverter.GetBytes(ImageUnitPixelActualSize), 0, result, index, 8); index += 8;
            Array.Copy(BitConverter.GetBytes(FontMeasureSize), 0, result, index, 4); index += 4;
            Array.Copy(BitConverter.GetBytes(DrawLineWidth), 0, result, index, 4); index += 4;

            return result;
        }

        public override string ToString()
        {
            return $"OK: {OKString}, NG: {NGString}, FodFddRatio: {FodFddRatio}, FontMeasureSize: {FontMeasureSize}, " +
                   $"ImageUnitPixelActualSize: {ImageUnitPixelActualSize}, DrawLineWidth: {DrawLineWidth}";
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

    /// <summary>
    /// 一个长度为64字节的string类型内容
    /// </summary>
    public class String64Data
    {
        public string stringData { get; set; }

        // 静态方法：从字节数组解析数据并创建 ClientRequestSuspendData 对象
        public static String64Data Parse(byte[] data)
        {
            var requestData = new String64Data();
            requestData.stringData = Encoding.UTF8.GetString(data).TrimEnd('\0');
            return requestData;
        }

        // 序列化方法：将 ClientRequestSuspendData 对象转成字节数组
        public byte[] ToByteArray()
        {
            var reasonBytes = Encoding.UTF8.GetBytes(stringData);
            Array.Resize(ref reasonBytes, 64); // 固定长度64字节
            return reasonBytes;
        }
    }

}
