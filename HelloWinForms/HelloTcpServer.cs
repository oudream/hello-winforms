using HelloWinForms.Protocols;
using HelloWinForms.Utilities.Sample.Yaml;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloTcpServer : Form
    {
        private TcpListener listener;
        private volatile bool isRunning;
        private readonly List<Thread> clientThreads = new List<Thread>();
        private ConcurrentDictionary<string, TcpClient> connectedClients = new ConcurrentDictionary<string, TcpClient>();
        private Random random = new Random();

        private Thread broadcastThread = null;

        public HelloTcpServer()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            Start();
        }

        public void Start()
        {
            if (isRunning) return;
            isRunning = true;

            listener = new TcpListener(IPAddress.Parse(ipTextBox.Text), (ushort)portNumericUpDown.Value);
            listener.Start();

            Thread acceptThread = new Thread(() =>
            {
                try
                {
                    while (isRunning)
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        Thread clientThread = new Thread(() => HandleClient(client));
                        clientThread.IsBackground = true;
                        clientThread.Start();
                        clientThreads.Add(clientThread);
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"SocketException: {e.Message}");
                }
                finally
                {
                    listener?.Stop();
                    isRunning = false;
                }
            });
            acceptThread.IsBackground = true;
            acceptThread.Start();

            // Start the broadcast message thread
            broadcastThread = new Thread(BroadcastMessages);
            //broadcastThread.IsBackground = true; // Ensure the thread does not prevent the process from exiting.
            broadcastThread.Start();
        }

        public void Stop()
        {
            isRunning = false;

            if (listener != null)
            {
                listener.Stop();
                listener = null;
            }
            
            if (broadcastThread != null)
            {
                broadcastThread.Join();
            }

            //foreach (var thread in clientThreads)
            //{
            //    if (thread.IsAlive)
            //    {
            //        thread.Join();
            //    }
            //}
            clientThreads.Clear();
            Console.WriteLine("Server stopped.");
        }

        private static volatile int sendMessageCount = 0;

        private void BroadcastMessages()
        {
            while (isRunning)
            {
                var sleep = random.Next(30, 1000);
                Thread.Sleep(sleep); // 延迟1到5秒

                var message = GenerateMessage(); // 生成要发送的消息
                var parts = SplitMessageIntoRandomParts(message); // 将消息随机分割为几个部分

                foreach (var clientPair in connectedClients)
                {
                    TcpClient client = clientPair.Value;
                    try
                    {
                        if (client.Connected)
                        {
                            foreach (var part in parts)
                            {
                                var buffer = Encoding.UTF8.GetBytes(part); // 将消息部分转换为字节数组
                                NetworkStream stream = client.GetStream();
                                stream.Write(buffer, 0, buffer.Length); // 发送消息的一部分给连接的客户端
                            }
                            Console.WriteLine($"Send message[{sendMessageCount++}] to client.");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to send message part to a client: {e.Message}");
                        // 可选择性地从集合中移除不再连接的客户端
                    }
                }
            }
        }

        private List<string> SplitMessageIntoRandomParts(string message)
        {
            List<string> parts = new List<string>();
            int totalLength = message.Length;
            while (totalLength > 0)
            {
                int partLength = random.Next(1, Math.Min(50, totalLength + 1)); // 随机选择部分的长度，假设最大不超过50个字符
                parts.Add(message.Substring(0, partLength)); // 添加消息的一部分到列表
                message = message.Substring(partLength); // 更新消息，移除已经添加的部分
                totalLength -= partLength; // 更新剩余长度
            }
            return parts;
        }

        private void HandleClient(TcpClient client)
        {
            var clientEndpoint = client.Client.RemoteEndPoint.ToString();
            connectedClients.TryAdd(clientEndpoint, client);
            Console.WriteLine("Client connected.");
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[client.ReceiveBufferSize];

                while (isRunning)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        // The client has disconnected
                        break;
                    }

                    // Process received data here...
                    // For now, just echo back the received data to the client.
                    stream.Write(buffer, 0, bytesRead);
                    Console.WriteLine($"Echoed back {bytesRead} bytes to client.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
            finally
            {
                TcpClient removedClient;
                connectedClients.TryRemove(clientEndpoint, out removedClient);
                client.Close();
                Console.WriteLine("Client disconnected.");
            }
        }

        private string GenerateMessage()
        {
            // Generate a message with a random batch number for demonstration
            return $"*#module_number:1#batch_number:{random.Next(100000, 999999)}#cmd:11#pos:11#sn1:123456789qwe#sn2:123456789qwe#sn3:123456789qwe#sn4:123456789qwe#dt:{DateTime.Now:yyyy-MM-dd HH:mm:ss}&";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Stop();
            Thread.Sleep(30);
            Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            for (int i = 10; i < 16; i++)
            {
                var (pos, orientation) = ConvertToImageDetection((ushort)i);
                Console.WriteLine($"Send: {pos} {orientation}");
            }
            for (int i = 20; i <= 26; i++)
            {
                var (pos, orientation) = ConvertToImageDetection((ushort)i);
                Console.WriteLine($"Send: {pos} {orientation}");
            }
        }

        // 位置数据转换
        // 11=1号产品正面取图反馈；12=2号产品正面取图反馈；13=3号产品正面取图反馈；14=4号产品正面取图反馈；
        // 21=1号产品反面取图反馈；22=2号产品反面取图反馈；23=3号产品反面取图反馈；24=4号产品反面取图反馈；
        // 位置数据转换为图像检测任务：11=1号产品正面，12=2号产品正面，13=3号产品正面，14=4号产品正面，
        //                             21=1号产品反面，22=2号产品反面，23=3号产品反面，24=4号产品反面
        public static (int pos, Orientation orientation) ConvertToImageDetection(ushort position)
        {
            if (position < 11 || position > 24) Console.WriteLine($"Invalid Positon: {position}");
            if (position > 14 && position < 21) Console.WriteLine($"Invalid Positon: {position}");

            int productNumber = position % 10;

            Orientation orientation = (position / 10 == 1) ? Orientation.Front : Orientation.Side;

            return (productNumber, orientation);
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
