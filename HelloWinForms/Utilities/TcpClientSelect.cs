using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloWinForms.Utilities
{
    class TcpClientSelect
    {
        private Socket socket;
        private readonly string serverIP;
        private readonly int serverPort;
        private readonly Thread workerThread;
        private volatile bool isRunning;

        public TcpClientSelect(string ip, int port)
        {
            serverIP = ip;
            serverPort = port;
            workerThread = new Thread(new ThreadStart(Run));
        }

        public void Start()
        {
            isRunning = true;
            workerThread.Start();
        }

        public void Stop()
        {
            isRunning = false;
            workerThread.Join();
            socket?.Close();
        }

        private void Run()
        {
            while (isRunning)
            {
                try
                {
                    // Establish connection
                    Connect();

                    while (isRunning && socket.Connected)
                    {
                        var readSockets = new List<Socket> { socket };
                        Socket.Select(readSockets, null, null, 1000000); // 1 second timeout

                        if (readSockets.Count > 0)
                        {
                            // Data available to read
                            byte[] buffer = new byte[1024];
                            int bytesRead = socket.Receive(buffer);
                            if (bytesRead > 0)
                            {
                                string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                Console.WriteLine($"Received: {receivedData}");
                            }
                            else
                            {
                                // Connection closed
                                Console.WriteLine("Connection closed by the server.");
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                // Attempt to reconnect
                if (isRunning)
                {
                    Console.WriteLine("Attempting to reconnect...");
                    Thread.Sleep(5000); // Wait before reconnecting
                }
            }
        }

        private void Connect()
        {
            socket?.Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var ipAddress = IPAddress.Parse(serverIP);
            var remoteEP = new IPEndPoint(ipAddress, serverPort);

            socket.Connect(remoteEP);
            Console.WriteLine("Connected to the server.");
        }
    }
}
