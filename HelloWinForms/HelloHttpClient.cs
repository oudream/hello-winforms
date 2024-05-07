using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloHttpClient : Form
    {
        public HelloHttpClient()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync("http://www.contoso.com/"))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);

                    Console.WriteLine(responseBody);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
            }
        }

        // 定义要发送的结构体（替换为实际需要的字段）
        public class DeviceRequest
        {
            [JsonProperty("device_id")]
            public string DeviceId { get; set; }

            [JsonProperty("device_name")]
            public string DeviceName { get; set; }
        }

        // 定义要解析的响应结构体
        public class DeviceResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string Data { get; set; }
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            HttpClientHandler handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) =>

                // 这是为了简化示例，并允许你测试 HTTPS 请求
                true

                // 实际验证服务器证书的合法性，可以替换委托内容为以下逻辑：
                /* 
                {
                    // 允许自签名证书或者你希望信任的特定证书
                    if (sslPolicyErrors == SslPolicyErrors.None)
                        return true;

                    // 在这里可以添加自定义的证书验证逻辑
                    // 例如：你可以检查证书的主题，颁发者等信息
                    // 还可以检查证书是否在信任的 CA 列表中

                    // 例如，允许特定颁发者的证书
                    if (cert.Issuer == "CN=MyTrustedCA")
                        return true;

                    // 默认拒绝
                    return false;
                }
                */
            };
            // 初始化 HttpClient 并配置请求头
            HttpClient httpClient = new HttpClient(handler);
            System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            httpClient.BaseAddress = new Uri("https://localhost:4443/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // 构建要发送的请求数据
            var request = new DeviceRequest
            {
                DeviceId = "123456789",
                DeviceName = "MyDevice"
            };

            // 将请求数据序列化为 JSON 字符串
            //var jsonRequest = JsonSerializer.Serialize(request);
            string jsonRequest = JsonConvert.SerializeObject(request, Formatting.None);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            try
            {
                // 发送 POST 请求并获取响应
                HttpResponseMessage response = await httpClient.PostAsync("device", content);
                //HttpResponseMessage response = await httpClient.GetAsync("device");

                // 确保请求成功
                response.EnsureSuccessStatusCode();

                // 读取并解析响应内容
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var deviceResponse = JsonConvert.DeserializeObject<DeviceResponse>(jsonResponse);

                // 打印解析后的响应内容
                if (deviceResponse != null)
                {
                    Console.WriteLine("Success: " + deviceResponse.Success);
                    Console.WriteLine("Message: " + deviceResponse.Message);
                    Console.WriteLine("Data: " + deviceResponse.Data);
                }
                else
                {
                    Console.WriteLine("无法解析响应内容。");
                }
            }
            catch (HttpRequestException ex)
            {
                // 处理请求异常
                Console.WriteLine($"请求异常: {ex.Message}");
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                using (HttpResponseMessage response = await client.GetAsync("http://localhost:8080/device"))
                {
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Above three lines can be replaced with new helper method below
                    // string responseBody = await client.GetStringAsync(uri);

                    Console.WriteLine(responseBody);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", ex.Message);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer2.Enabled = checkBox1.Checked;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            connectButton_Click(sender, e);
        }
    }
}
