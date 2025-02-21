using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    // Message File Link Protocol
    public class ProtocolMFL
    {
        private readonly uint _startCode = 0x5AA55AA5;
        private readonly uint _endCode = 0x90EB90EB;

        public const uint _funCode_s_startJudge = 0x201;    //服务器发起判定
        public const uint _funCode_s_timeout = 0x202;       //服务器向客户端发送超时

        public const uint _funCode_c_endJudge = 0x00000102; //客户端结束判定
        public const uint _funCode_c_suspend = 0x00000103;  //客户端发起挂起请求
        public const uint _funCode_c_continue = 0x00000104; //客户端发起继续请求
        public const uint _funCode_c_sendid = 0x00000105;   //客户端向服务器发送客户端ID
        public const uint _funCode_c_heartbeat = 0x00000106;   //客户端向服务器发送心跳
        public const uint _funCode_c_login = 0x00000107;   //客户端向服务器发送登录信息

        public const int HeartbeatInterval = 3000;
        public static readonly string HeartbeatStr = "Heartbeat...!";

        enum State
        {
            STATE_SYNC,
            STATE_SERVER_ID,
            STATE_CLIENT_ID,
            STATE_FUNCTION_CODE,
            STATE_DATA_LENGTH,
            STATE_DATA_BODY,
            STATE_CHECKSUM
        }

        private State _recState = State.STATE_SYNC;
        private List<byte> _parseBuffer = new List<byte>();
        private int _parseIndex = 0;

        protected uint _recServerId = 0;
        protected uint _recClientId = 0;
        protected uint _recFunctionCode = 0;
        protected uint _recDataLength = 0;
        protected byte[] _recDataBody = null;
        protected uint _recCheckSum = 0;

        public delegate void ProcessDataBodyDelegate(uint serverId, uint clientId, uint functionCode, uint dataLength, byte[] dataBody);
        public ProcessDataBodyDelegate ProcessDataBody = null;

        public void ParseData(byte[] data, int offset, int length)
        {
            // 只添加指定长度的数据到缓冲区
            _parseBuffer.AddRange(new ArraySegment<byte>(data, offset, length));

            while (_parseIndex < _parseBuffer.Count)
            {
                switch (_recState)
                {
                    case State.STATE_SYNC:
                        while (_parseIndex + 4 <= _parseBuffer.Count)
                        {
                            // 检查是否匹配指定的起始码
                            if (BitConverter.ToUInt32(_parseBuffer.GetRange(_parseIndex, 4).ToArray(), 0) == _startCode)
                            {
                                _parseIndex += 4;
                                _recState = State.STATE_SERVER_ID;
                                break;
                            }
                            else
                            {
                                _parseIndex++;
                            }
                        }
                        if (_recState != State.STATE_SERVER_ID)
                        {
                            // 如果没有找到起始码或数据不足，移除已处理的数据
                            _parseBuffer.RemoveRange(0, _parseIndex);
                            _parseIndex = 0;
                            return;
                        }
                        break;

                    case State.STATE_SERVER_ID:
                        if (_parseBuffer.Count - _parseIndex >= 4)
                        {
                            _recServerId = BitConverter.ToUInt32(_parseBuffer.GetRange(_parseIndex, 4).ToArray(), 0);
                            _parseIndex += 4;
                            _recState = State.STATE_CLIENT_ID;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_CLIENT_ID:
                        if (_parseBuffer.Count - _parseIndex >= 4)
                        {
                            _recClientId = BitConverter.ToUInt32(_parseBuffer.GetRange(_parseIndex, 4).ToArray(), 0);
                            _parseIndex += 4;
                            _recState = State.STATE_FUNCTION_CODE;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_FUNCTION_CODE:
                        if (_parseBuffer.Count - _parseIndex >= 4)
                        {
                            _recFunctionCode = BitConverter.ToUInt32(_parseBuffer.GetRange(_parseIndex, 4).ToArray(), 0);
                            _parseIndex += 4;
                            _recState = State.STATE_DATA_LENGTH;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_DATA_LENGTH:
                        if (_parseBuffer.Count - _parseIndex >= 4)
                        {
                            _recDataLength = BitConverter.ToUInt32(_parseBuffer.GetRange(_parseIndex, 4).ToArray(), 0);
                            if (_recDataLength > 1024 * 1024 * 40) // 限制最大数据长度为40MB
                            {
                                ResetState();
                                _parseBuffer.RemoveRange(0, _parseIndex);
                                _parseIndex = 0;
                                return;
                            }
                            _parseIndex += 4;
                            _recState = State.STATE_DATA_BODY;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_DATA_BODY:
                        if (_parseBuffer.Count - _parseIndex >= _recDataLength)
                        {
                            _recDataBody = _parseBuffer.GetRange(_parseIndex, (int)_recDataLength).ToArray();
                            _parseIndex += (int)_recDataLength;
                            _recState = State.STATE_CHECKSUM;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_CHECKSUM:
                        if (_parseBuffer.Count - _parseIndex >= 4)
                        {
                            _recCheckSum = BitConverter.ToUInt32(_parseBuffer.GetRange(_parseIndex, 4).ToArray(), 0);
                            _parseIndex += 4;

                            // 检查是否匹配指定的结束码
                            if (_parseBuffer.Count - _parseIndex >= 4 && BitConverter.ToUInt32(_parseBuffer.GetRange(_parseIndex, 4).ToArray(), 0) == _endCode)
                            {
                                _parseIndex += 4;

                                // 处理完整消息
                                ProcessMessage();

                                // 重置状态，移除已处理数据
                                int processedBytes = _parseIndex;
                                _parseBuffer.RemoveRange(0, processedBytes);
                                _parseIndex = 0;
                                ResetState();
                            }
                            else
                            {
                                // 结束码不匹配，重置状态
                                ResetState();
                            }
                        }
                        else
                        {
                            return;
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void ResetState()
        {
            _recState = State.STATE_SYNC;
            _recServerId = 0;
           // _recClientId = 0;
            _recFunctionCode = 0;
            _recDataLength = 0;
            _recDataBody = null;
            _recCheckSum = 0;
        }

        protected virtual void ProcessMessage()
        {
            if (ProcessDataBody != null)
            {
                ProcessDataBody(_recServerId, _recClientId, _recFunctionCode, _recDataLength, _recDataBody);
            }    
            {
                //var fields = new List<string>();
                //fields.Add($"服务器 ID: {_recServerId}");
                //fields.Add($"客户端 ID: {_recClientId}");
                //fields.Add($"功能码: {_recFunctionCode}");
                //fields.Add($"数据长度: {_recDataLength}");
                ////fields.Add($"数据内容: {BitConverter.ToString(_dataBody)}");
                //fields.Add($"校验码: {_recCheckSum}");
                //LogHelper.Debug("接收到完整消息:", string.Join(",", fields));
            }
        }

        public byte[] AssembleMessage(uint functionCode, byte[] dataBody)
        {
            // 创建一个列表来存储报文内容
            List<byte> message = new List<byte>();

            // 添加起始码
            message.AddRange(BitConverter.GetBytes(_startCode));

            // 添加服务器 ID、客户端 ID 和功能码
            message.AddRange(BitConverter.GetBytes(_recServerId));
            message.AddRange(BitConverter.GetBytes(_recClientId));
            message.AddRange(BitConverter.GetBytes(functionCode));

            // 计算并添加数据长度
            uint dataLength = (dataBody != null) ? (uint)dataBody.Length : 0;
            message.AddRange(BitConverter.GetBytes(dataLength));

            // 添加数据内容
            if (dataBody != null)
            {
                message.AddRange(dataBody);
            }

            // 计算校验码并添加
            uint checksum = 0x00000000;
            message.AddRange(BitConverter.GetBytes(checksum));

            // 添加结束码
            message.AddRange(BitConverter.GetBytes(_endCode));

            // 返回完整的报文字节数组
            return message.ToArray();
        }

        public void SetClientId(uint id)
        {
            _recClientId = id;
        }
    }
}
