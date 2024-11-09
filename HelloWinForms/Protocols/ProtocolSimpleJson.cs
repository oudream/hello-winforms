using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.Protocols
{
    public class ProtocolSimpleJson
    {
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

        private uint _serverId = 0;
        private uint _clientId = 0;
        private uint _functionCode = 0;
        private uint _dataLength = 0;
        private byte[] _dataBody = null;
        private uint _checksum = 0;

        public void ParseData(byte[] data)
        {
            // Append new data to buffer
            _buffer.AddRange(data);

            while (_index < _buffer.Count)
            {
                switch (_state)
                {
                    case State.STATE_SYNC:
                        while (_index + 4 <= _buffer.Count)
                        {
                            if (_buffer[_index] == 0xA5 && _buffer[_index + 1] == 0x5A &&
                                _buffer[_index + 2] == 0xA5 && _buffer[_index + 3] == 0x5A)
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
                            // Not enough data or sync not found, remove processed bytes
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
                            // Need more data
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
                            // Need more data
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
                            // Need more data
                            return;
                        }
                        break;

                    case State.STATE_DATA_LENGTH:
                        if (_buffer.Count - _index >= 4)
                        {
                            _dataLength = BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0);
                            if (_dataLength > 1024 * 1024 * 40) // Max 40 MB
                            {
                                // Invalid data length
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
                            // Need more data
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
                            // Need more data
                            return;
                        }
                        break;

                    case State.STATE_CHECKSUM:
                        if (_buffer.Count - _index >= 4)
                        {
                            _checksum = BitConverter.ToUInt32(_buffer.GetRange(_index, 4).ToArray(), 0);
                            _index += 4;

                            // Optionally, verify checksum here
                            // For example: if (!VerifyChecksum()) { /* Handle error */ }

                            // Process the complete message
                            ProcessMessage();

                            // Reset state and remove processed data from buffer
                            int processedBytes = _index;
                            _buffer.RemoveRange(0, processedBytes);
                            _index = 0;
                            ResetState();
                        }
                        else
                        {
                            // Need more data
                            return;
                        }
                        break;

                    default:
                        // Should not reach here
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

        private void ProcessMessage()
        {
            // Implement your message handling logic here
            Console.WriteLine("Message received:");
            Console.WriteLine($"Server ID: {_serverId}");
            Console.WriteLine($"Client ID: {_clientId}");
            Console.WriteLine($"Function Code: {_functionCode}");
            Console.WriteLine($"Data Length: {_dataLength}");
            Console.WriteLine($"Data Body: {BitConverter.ToString(_dataBody)}");
            Console.WriteLine($"Checksum: {_checksum}");
        }

        // Optional: Implement checksum verification if needed
        private bool VerifyChecksum()
        {
            // Calculate checksum over the necessary parts
            // This is a placeholder implementation
            uint calculatedChecksum = 0; // Replace with actual checksum calculation
            return calculatedChecksum == _checksum;
        }
    }
}
