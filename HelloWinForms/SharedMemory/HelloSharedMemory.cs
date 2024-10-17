using CxAAC.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.SharedMemory
{
    public partial class HelloSharedMemory : Form
    {
        public HelloSharedMemory()
        {
            InitializeComponent();
            Init();
        }

        private uint _batchNumber = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            // 选择文件
            OpenFileDialog dialog = new OpenFileDialog();
            // 提示信息
            dialog.Title = "请选择文件";
            // 可以设置默认目录，也可以设置文件过滤器等
            // dialog.InitialDirectory = "C:\\"; 
            dialog.Filter = "图片文件|*.tif;*.png";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = dialog.FileName;
                txb_workDir.Text = filePath;

                var newImage = LoadImage(filePath); // 确保这个方法可以处理文件路径
                originalImagePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                originalImagePictureBox.Image = newImage;

                if (sharedMemoryHelper != null)
                {
                    var hImage = HalconHelper.ReadImage(filePath);
                    sharedMemoryHelper.PushImage2SharedMemory(1, _batchNumber++, hImage);
                }
            }
        }

        private Image LoadImage(string filePath)
        {
            // 使用文件流加载图像
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // 创建图像对象
                    Image image = Image.FromStream(fs);
                    // 关闭文件流，释放文件句柄
                    fs.Close();
                    return image;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private const string SharedMemoryName = "Global\\SharedMemoryAQ";
        private SharedMemoryHelper sharedMemoryHelper = null;

        private void Init()
        {
            if (sharedMemoryHelper != null)
            {
                return;
            }

            sharedMemoryHelper = new SharedMemoryHelper(SharedMemoryName);
            sharedMemoryHelper.DetectionResultReleased += SharedMemoryHelper_DetectionResultReleased;
            sharedMemoryHelper.Run();
        }

        private void SharedMemoryHelper_DetectionResultReleased(uint arg1, uint arg2, byte[] arg3)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SharedMemoryHelper_DetectionResultReleased(arg1, arg2, arg3)));
                return;
            }
            string result = Encoding.UTF8.GetString(arg3);
            label3.Text = result;
            label3.Refresh();
        }

        private void HelloSharedMemory_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sharedMemoryHelper != null)
            {
                sharedMemoryHelper.Stop();
                sharedMemoryHelper.Dispose();
            }
        }
    }
}
