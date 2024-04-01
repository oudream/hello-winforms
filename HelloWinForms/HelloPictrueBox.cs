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

namespace HelloWinForms
{
    public partial class HelloPictrueBox : Form
    {
        public HelloPictrueBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 图片加文字水印
        /// </summary>
        public static void PictureTextWatermark(string sourceFile, string text, int x, int y, Font font, Color fontColor, string targetFile = null)
        {
            /*
             （1）SourceFile   --  原始图片文件路径
             （2）WaterText  --水印文字
             （3）WaterTextFont    --水印文字字体
             （4）WaterTextBrush    --水印文字笔触
             （5）x    --水印图像的起始X坐标
             （6）y    --水印图像的起始Y坐标
             （7）TargetFile        --新生成的目标图片文件路径
             */

            if (File.Exists(sourceFile) == false) return;
            if (targetFile == null) targetFile = sourceFile;

            var uti = new DeveloperSharp.Framework.CoreUtility.Utility();
            Brush brush = new SolidBrush(fontColor);
            uti.PictureTextWatermark(sourceFile, text, font, brush, x, y, targetFile);
        }

        /// <summary>
        /// 图片加图片水印
        /// </summary>
        public static void PictureImageWatermark(string sourceFile, string watermarkFile, int x, int y, string targetFile = null)
        {
            /*
             （1）string SourceFile   --  原始图片文件路径
             （2）string WatermarkFile  --水印图像文件路径
             （3）int x    --水印图像的起始X坐标
             （4）int y    --水印图像的起始Y坐标
             （5）string TargetFile        --新生成的目标图片文件路径
             */

            if (File.Exists(sourceFile) == false) return;
            if (targetFile == null) targetFile = sourceFile;

            var uti = new DeveloperSharp.Framework.CoreUtility.Utility();
            uti.PictureImageWatermark(sourceFile, watermarkFile, x, y, targetFile);
        }
    }
}
