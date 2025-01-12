using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.ProtocolMFL
{
    // Message File Link Protocol
    public class ProtocolMFL
    {
        private readonly uint _startCode = 0x5AA55AA5;
        private readonly uint _endCode = 0x90EB90EB;

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

        private State _state = State.STATE_SYNC;
        private List<byte> _buffer = new List<byte>();
        private int _index = 0;

        protected uint _serverId = 0;
        protected uint _clientId = 0;
        protected uint _functionCode = 0;
        protected uint _dataLength = 0;
        protected byte[] _dataBody = null;
        protected uint _checksum = 0;

        public void ParseData(byte[] data, int offset, int length)
        {
            // 只添加指定长度的数据到缓冲区
            _buffer.AddRange(new ArraySegment<byte>(data, offset, length));

            while (_index < _buffer.Count)
            {
                switch (_state)
                {
                    case State.STATE_SYNC:
                        while (_index + 4 <= _buffer.Count)
                        {
                            // 检查是否匹配指定的起始码
                            if (BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0) == _startCode)
                            {
                                _index += 4;
                                _state = State.STATE_SERVER_ID;
                                break;
                            }
                            else
                            {
                                _index++;
                            }
                        }
                        if (_state != State.STATE_SERVER_ID)
                        {
                            // 如果没有找到起始码或数据不足，移除已处理的数据
                            _buffer.RemoveRange(0, _index);
                            _index = 0;
                            return;
                        }
                        break;

                    case State.STATE_SERVER_ID:
                        if (_buffer.Count - _index >= 4)
                        {
                            _serverId = BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0);
                            _index += 4;
                            _state = State.STATE_CLIENT_ID;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_CLIENT_ID:
                        if (_buffer.Count - _index >= 4)
                        {
                            _clientId = BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0);
                            _index += 4;
                            _state = State.STATE_FUNCTION_CODE;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_FUNCTION_CODE:
                        if (_buffer.Count - _index >= 4)
                        {
                            _functionCode = BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0);
                            _index += 4;
                            _state = State.STATE_DATA_LENGTH;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_DATA_LENGTH:
                        if (_buffer.Count - _index >= 4)
                        {
                            _dataLength = BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0);
                            if (_dataLength > 1024 * 1024 * 40) // 限制最大数据长度为40MB
                            {
                                ResetState();
                                _buffer.RemoveRange(0, _index);
                                _index = 0;
                                return;
                            }
                            _index += 4;
                            _state = State.STATE_DATA_BODY;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_DATA_BODY:
                        if (_buffer.Count - _index >= _dataLength)
                        {
                            _dataBody = _buffer.GetRange(_index, (int)_dataLength).ToArray();
                            _index += (int)_dataLength;
                            _state = State.STATE_CHECKSUM;
                        }
                        else
                        {
                            return;
                        }
                        break;

                    case State.STATE_CHECKSUM:
                        if (_buffer.Count - _index >= 4)
                        {
                            _checksum = BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0);
                            _index += 4;

                            // 检查是否匹配指定的结束码
                            if (_buffer.Count - _index >= 4 && BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0) == _endCode)
                            {
                                _index += 4;

                                // 处理完整消息
                                ProcessMessage();

                                // 重置状态，移除已处理数据
                                int processedBytes = _index;
                                _buffer.RemoveRange(0, processedBytes);
                                _index = 0;
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
            _state = State.STATE_SYNC;
            _serverId = 0;
            _clientId = 0;
            _functionCode = 0;
            _dataLength = 0;
            _dataBody = null;
            _checksum = 0;
        }

        protected virtual void ProcessMessage()
        {
            if (false)
            {
                var fields = new List<string>();
                fields.Add($"服务器 ID: {_serverId}");
                fields.Add($"客户端 ID: {_clientId}");
                fields.Add($"功能码: {_functionCode}");
                fields.Add($"数据长度: {_dataLength}");
                //fields.Add($"数据内容: {BitConverter.ToString(_dataBody)}");
                fields.Add($"校验码: {_checksum}");
                LogHelper.Debug("接收到完整消息:", string.Join(",", fields));
            }
        }
    }
}
