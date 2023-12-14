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
using System.Xml.Serialization;

namespace HelloWinForms
{
    public partial class HelloXML : Form
    {
        public HelloXML()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 读取XML文件
            string xmlFilePath = "E:\\image\\903-340-32_NDT_SDK_ReleasePackage_4.4.4.10041\\Tools\\iDetector\\x64\\work_dir\\NDT1417LA\\ApplicationMode.xml";
            string xmlContent = File.ReadAllText(xmlFilePath);

            // 使用XmlSerializer反序列化XML内容到数据结构
            HwAcqParamSettingList hwAcqParamSettingList;
            XmlSerializer serializer = new XmlSerializer(typeof(HwAcqParamSettingList));

            using (StringReader reader = new StringReader(xmlContent))
            {
                hwAcqParamSettingList = (HwAcqParamSettingList)serializer.Deserialize(reader);
            }

            // 现在你可以使用 hwAcqParamSettingList 对象来访问XML中的数据
            foreach (var item in hwAcqParamSettingList.Items)
            {
                richTextBox1.AppendText($"Index: {item.Index}, Subset: {item.Subset}, Activity: {item.Activity}, " +
                      $"AcqMode: {item.AcqMode}, Binning: {item.Binning}, Zoom: {item.Zoom}, " +
                      $"ROIColStart: {item.ROIColStart}, ROIColEnd: {item.ROIColEnd}, " +
                      $"ROIRowStart: {item.ROIRowStart}, ROIRowEnd: {item.ROIRowEnd}, " +
                      $"PGA: {item.PGA}, VT: {item.VT}, " +
                      $"SequenceIntervalTime_us: {item.SequenceIntervalTime_us}, " +
                      $"SetDelayTime_ms: {item.SetDelayTime_ms}, " +
                      $"HwPreoffsetDiscardNumberDeforeAcq: {item.HwPreoffsetDiscardNumberDeforeAcq}, " +
                      $"HwPreoffsetAcqNumber: {item.HwPreoffsetAcqNumber}, " +
                      $"HwCorrectionEn: {item.HwCorrectionEn}, " +
                      $"FluroSync: {item.FluroSync}, " +
                      $"NumberOfFramesDiscardedInSeqAcq: {item.NumberOfFramesDiscardedInSeqAcq}, " +
                      $"IntegrationMethod: {item.IntegrationMethod}, " +
                      $"CofPGA: {item.CofPGA}, " +
                      $"OffsetType: {item.OffsetType}\r\n");
                // 在此添加其他属性的输出
            }
        }
    }

    [XmlRoot("HwAcqParamSettingList")]
    public class HwAcqParamSettingList
    {
        [XmlElement("Item")]
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public int Index { get; set; }
        public string Subset { get; set; }
        public string Activity { get; set; }
        public string AcqMode { get; set; }
        public string Binning { get; set; }
        public string Zoom { get; set; }
        public int ROIColStart { get; set; }
        public int ROIColEnd { get; set; }
        public int ROIRowStart { get; set; }
        public int ROIRowEnd { get; set; }
        public string PGA { get; set; }
        public double VT { get; set; }
        public int SequenceIntervalTime_us { get; set; }
        public int SetDelayTime_ms { get; set; }
        public int HwPreoffsetDiscardNumberDeforeAcq { get; set; }
        public int HwPreoffsetAcqNumber { get; set; }
        public string HwCorrectionEn { get; set; }
        public string FluroSync { get; set; }
        public int NumberOfFramesDiscardedInSeqAcq { get; set; }
        public string IntegrationMethod { get; set; }
        public string CofPGA { get; set; }
        public string OffsetType { get; set; }
    }
}
