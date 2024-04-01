using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core.Tokens;

namespace HelloWinForms
{
    public partial class HelloJson : Form
    {
        public HelloJson()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HelloJson1();
        }

        private void HelloJson1()
        {
            // 你的数据对象
            var imageData = new ImageAnnotation
            {
                Version = "5.2.1",
                Flags = new { }, // 确保这里的对象与你的实际需求匹配
                Shapes = new List<ImageShape>
                {
                    new ImageShape
                    {
                        Label = "solder ball",
                        Points = new List<List<double>> { new List<double> { 1202.127, 1643.074 }, new List<double> { 1269.979, 1716.453 } },
                        GroupId = null,
                        Description = "",
                        ShapeType = "rectangle",
                        Flags = new { }
                    },
                    // 根据需要添加其他Shape对象
                },
                ImagePath = "Random_big_3BS.bmp",
                ImageHeight = 2496,
                ImageWidth = 3008
            };

            // 将对象序列化为JSON字符串
            string jsonString = JsonConvert.SerializeObject(imageData, Formatting.Indented);

            // 保存到文件
            File.WriteAllText("imageData.json", jsonString);

            Console.WriteLine("JSON文件已保存。");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            long a = 0;
            if (a is long u)
            {
                MessageBox.Show(a.GetType().ToString());
            }
        }

        /**
        /// <summary>
        /// 灰度图 --> Base64字符串
        /// </summary>
        /// <param name="ho_image"></param>
        /// <param name="bIsBit16">是否16位灰度图</param>
        /// <param name="sBase64"></param>
        public static void GrayImageToBase64String(HObject ho_image, bool bIsBit16, out string sBase64)
        {
            HOperatorSet.CopyImage(ho_image, out var imageCopy);
            HOperatorSet.GetImagePointer1(imageCopy, out var ptrImage, out _, out var img_width, out var img_height);

            int k = bIsBit16 ? 2 : 1;
            byte[] img_data = new byte[img_width.I * img_height.I * k];
            Marshal.Copy(ptrImage, img_data, 0, img_data.Length);

            sBase64 = Convert.ToBase64String(img_data);
            imageCopy.Dispose();
        }


        /// <summary>
        /// Base64字符串 --> 灰度图
        /// </summary>
        /// <param name="sBase64"></param>
        /// <param name="bIsBit16">是否16位灰度图</param>
        /// <param name="imgW"></param>
        /// <param name="imgH"></param>
        /// <param name="ho_image"></param>
        public static void GrayImageFromBase64String(string sBase64, bool bIsBit16, int imgW, int imgH, out HObject ho_image)
        {
            var byteAry = Convert.FromBase64String(sBase64);
            int k = bIsBit16 ? 2 : 1;
            ushort[] img_data = new ushort[byteAry.Length / k];
            Buffer.BlockCopy(byteAry, 0, img_data, 0, byteAry.Length);

            HOperatorSet.GenImageConst(out ho_image, "uint2", imgW, imgH);
            HOperatorSet.GetImagePointer1(ho_image, out var ptrImage, out _, out _, out _);
            Copy(img_data, ptrImage, (uint)img_data.Length);
            HOperatorSet.GenImage1(out ho_image, "uint2", imgW, imgH, ptrImage);
        }

        public static unsafe void Copy(IntPtr ptrSource, ushort[] dest, uint elements)
        {
            fixed (ushort* ptrDest = &dest[0])
            {
                CopyMemory((IntPtr)ptrDest, ptrSource, elements * 2); // 2 bytes per element 
            }
        }

        public static unsafe void Copy(ushort[] source, IntPtr ptrDest, uint elements)
        {
            fixed (ushort* ptrSource = &source[0])
            {
                CopyMemory(ptrDest, (IntPtr)ptrSource, elements * 2); // 2 bytes per element 
            }
        }

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);
        */
    }


    public class ImageAnnotation
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("flags")]
        public object Flags { get; set; } // 确定具体类型或保持为object，根据你的实际情况

        [JsonProperty("shapes")]
        public List<ImageShape> Shapes { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }

        [JsonProperty("imageData")]
        public string ImageData { get; set; }

        [JsonProperty("imageHeight")]
        public int ImageHeight { get; set; }

        [JsonProperty("imageWidth")]
        public int ImageWidth { get; set; }
    }

    public class ImageShape
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("points")]
        public List<List<double>> Points { get; set; }

        [JsonProperty("group_id")]
        public object GroupId { get; set; } // 确定具体类型或保持为object，根据你的实际情况

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("shape_type")]
        public string ShapeType { get; set; }

        [JsonProperty("flags")]
        public object Flags { get; set; } // 确定具体类型或保持为object，根据你的实际情况
    }

}
