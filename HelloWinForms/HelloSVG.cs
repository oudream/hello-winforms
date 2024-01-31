using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloSVG : Form
    {
        private SvgDocument svgDoc;

        public HelloSVG()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (openSvgFile.ShowDialog() == DialogResult.OK)
                {
                    svgDoc = SvgDocument.Open(openSvgFile.FileName);
                    RenderSvg(svgDoc);
                }
            }
            catch
            {
            }
        }

        private void RenderSvg(SvgDocument svgDoc)
        {
            svgImage.Image?.Dispose();
            //using (var render = new DebugRenderer())
            //    svgDoc.Draw(render);
            svgImage.Image = svgDoc.Draw();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int millisecond = DateTime.Now.Millisecond;
            // 使用模运算确定颜色
            Color color;
            switch (millisecond % 3)
            {
                case 0: // 注意这里是0，因为0, 1, 2是模3的可能结果
                    color = Color.Red;
                    break;
                case 1:
                    color = Color.White;
                    break;
                case 2:
                    color = Color.Green;
                    break;
                default:
                    throw new InvalidOperationException("Unexpected modulo result");
            }
            // SvgUse
            var ele = svgDoc.GetElementById<SvgRectangle>("svg_65");
            if (ele != null)
            {
                ele.Fill = new SvgColourServer(color);
            }
            ele = svgDoc.GetElementById<SvgRectangle>("svg_5");
            if (ele != null)
            {
                ele.Fill = new SvgColourServer(color);
            }
            RenderSvg(svgDoc);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            button2_Click(sender, e);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }
    }
}
