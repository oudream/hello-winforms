using OpenCvSharp.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HelloWinForms.Controls
{
    public partial class PagerControl : UserControl
    {
        public delegate void PageControlEventHandler(object currentSource);

        public PagerControl()
        {
            InitializeComponent();

            txtPage.Controls[0].Visible = false;

            for (int i = 0; i < 9; i++)
            {
                Button c = (Button)this.tableLayoutPanel1.Controls.Find("p" + (i + 1), false)[0];
                c.Click += page_BtnClick;
            }

            btnFirst.Click += btnFirst_BtnClick;
            btnPrevious.Click += btnPrevious_BtnClick;
            btnNext.Click += btnNext_BtnClick;
            btnEnd.Click += btnEnd_BtnClick;
            btnToPage.Click += btnToPage_BtnClick;
            txtPage.KeyDown += txtInput_KeyDown;
        }

        private void PagerControl_Load(object sender, EventArgs e)
        {
            if (DataSource == null)
                ShowBtn(false, false);
            else
            {
                ShowBtn(false, DataSource.Count > PageSize);
            }
        }

        private int startIndex = 0;
        // 开始的下标
        public virtual int StartIndex
        {
            get { return startIndex; }
            set
            {
                startIndex = value;
                if (startIndex <= 0)
                    startIndex = 0;
            }
        }

        // 获取当前页数据
        public virtual List<object> GetCurrentSource()
        {
            if (DataSource == null || DataSource.Count <= 0)
                return null;
            int intShowCount = m_pageSize;
            if (intShowCount + startIndex > DataSource.Count)
                intShowCount = DataSource.Count - startIndex;
            object[] objs = new object[intShowCount];
            DataSource.CopyTo(startIndex, objs, 0, intShowCount);
            var lst = objs.ToList();

            bool blnLeft = false;
            bool blnRight = false;
            if (lst.Count > 0)
            {
                if (DataSource.IndexOf(lst[0]) > 0)
                {
                    blnLeft = true;
                }
                else
                {
                    blnLeft = false;
                }
                if (DataSource.IndexOf(lst[lst.Count - 1]) >= DataSource.Count - 1)
                {
                    blnRight = false;
                }
                else
                {
                    blnRight = true;
                }
            }
            ShowBtn(blnLeft, blnRight);
            return lst;
        }

        void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnToPage_BtnClick(null, null);
            }
        }

        // 数据源改变时发生
        public event PageControlEventHandler ShowSourceChanged;


        private List<object> dataSource;
        // 关联的数据源
        public List<object> DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                if (value == null)
                {
                    dataSource = new List<object>();
                }
                else
                {
                    dataSource = value;

                }
                PageIndex = 1;
                ResetPageCount();
                var s = GetCurrentSource();
                if (ShowSourceChanged != null)
                {
                    ShowSourceChanged(s);
                }
            }
        }

        private int m_pageSize = 10;
        // 每页显示数量
        public int PageSize
        {
            get
            {
                return m_pageSize;
            }
            set
            {
                m_pageSize = value;
                ResetPageCount();
                var s = GetCurrentSource();
                if (ShowSourceChanged != null)
                {
                    ShowSourceChanged(s);
                }
            }
        }

        private int m_pageCount = 0;
        // 总页数
        public int PageCount
        {
            get
            {
                return m_pageCount;
            }
            set
            {
                m_pageCount = value;
                ReloadPage();
            }
        }

        private int m_pageIndex = 1;
        // 当前页
        public int PageIndex
        {
            get
            {
                return m_pageIndex;
            }
            set
            {
                m_pageIndex = value;
                ReloadPage();
            }
        }

        // 第一页
        public void FirstPage()
        {
            if (PageIndex == 1)
                return;
            PageIndex = 1;
            ReloadPage();
            StartIndex = (PageIndex - 1) * PageSize;
            var s = GetCurrentSource();
            if (ShowSourceChanged != null)
            {
                ShowSourceChanged(s);
            }
        }

        // 上一页
        public void PreviousPage()
        {
            if (PageIndex <= 1)
            {
                return;
            }
            PageIndex--;
            ReloadPage();
            StartIndex = (PageIndex - 1) * PageSize;
            var s = GetCurrentSource();
            if (ShowSourceChanged != null)
            {
                ShowSourceChanged(s);
            }
        }

        // 下一页
        public void NextPage()
        {
            if (PageIndex >= PageCount)
            {
                return;
            }
            PageIndex++;
            ReloadPage();
            StartIndex = (PageIndex - 1) * PageSize;
            var s = GetCurrentSource();
            if (ShowSourceChanged != null)
            {
                ShowSourceChanged(s);
            }
        }

        // 最后一页
        public void EndPage()
        {
            if (PageIndex == PageCount)
                return;
            PageIndex = PageCount;
            ReloadPage();
            StartIndex = (PageIndex - 1) * PageSize;
            var s = GetCurrentSource();
            if (ShowSourceChanged != null)
            {
                ShowSourceChanged(s);
            }
        }

        /// <summary>
        /// Resets the page count.
        /// </summary>
        private void ResetPageCount()
        {
            if (PageSize > 0)
            {
                if (dataSource != null)
                    PageCount = dataSource.Count / m_pageSize + (dataSource.Count % m_pageSize > 0 ? 1 : 0);
            }
            txtPage.Maximum = PageCount;
            txtPage.Minimum = 1;
            ReloadPage();
        }

        /// <summary>
        /// Reloads the page.
        /// </summary>
        private void ReloadPage()
        {
            try
            {
                //ControlHelper.FreezeControl(tableLayoutPanel1, true);
                tableLayoutPanel1.SuspendLayout();
                List<int> lst = new List<int>();

                if (PageCount <= 9)
                {
                    for (var i = 1; i <= PageCount; i++)
                    {
                        lst.Add(i);
                    }
                }
                else
                {
                    if (this.PageIndex <= 6)
                    {
                        for (var i = 1; i <= 7; i++)
                        {
                            lst.Add(i);
                        }
                        lst.Add(-1);
                        lst.Add(PageCount);
                    }
                    else if (this.PageIndex > PageCount - 6)
                    {
                        lst.Add(1);
                        lst.Add(-1);
                        for (var i = PageCount - 6; i <= PageCount; i++)
                        {
                            lst.Add(i);
                        }
                    }
                    else
                    {
                        lst.Add(1);
                        lst.Add(-1);
                        var begin = PageIndex - 2;
                        var end = PageIndex + 2;
                        if (end > PageCount)
                        {
                            end = PageCount;
                            begin = end - 4;
                            if (PageIndex - begin < 2)
                            {
                                begin = begin - 1;
                            }
                        }
                        else if (end + 1 == PageCount)
                        {
                            end = PageCount;
                        }
                        for (var i = begin; i <= end; i++)
                        {
                            lst.Add(i);
                        }
                        if (end != PageCount)
                        {
                            lst.Add(-1);
                            lst.Add(PageCount);
                        }
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    Button c = (Button)this.tableLayoutPanel1.Controls.Find("p" + (i + 1), false)[0];
                    if (i >= lst.Count)
                    {
                        c.Visible = false;
                    }
                    else
                    {
                        if (lst[i] == -1)
                        {
                            c.Text = "...";
                            c.Enabled = false;
                        }
                        else
                        {
                            c.Text = lst[i].ToString();
                            c.Enabled = true;
                        }
                        c.Visible = true;
                        if (lst[i] == PageIndex)
                        {
                            c.BackColor = Color.FromArgb(255, 77, 59);
                        }
                        else
                        {
                            c.BackColor = Color.FromArgb(223, 223, 223);
                        }
                    }
                }
                ShowBtn(PageIndex > 1, PageIndex < PageCount);
            }
            finally
            {
                //ControlHelper.FreezeControl(tableLayoutPanel1, false);
                tableLayoutPanel1.ResumeLayout(true); // 参数为true表示重新计算布局
            }
        }

        /// <summary>
        /// Handles the BtnClick event of the page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void page_BtnClick(object sender, EventArgs e)
        {
            PageIndex = int.Parse((sender as Button).Text);
            StartIndex = (PageIndex - 1) * PageSize;
            ReloadPage();
            var s = GetCurrentSource();

            if (ShowSourceChanged != null)
            {
                ShowSourceChanged(s);
            }
        }

        /// <summary>
        /// 控制按钮显示
        /// </summary>
        /// <param name="blnLeftBtn">是否显示上一页，第一页</param>
        /// <param name="blnRightBtn">是否显示下一页，最后一页</param>
        protected void ShowBtn(bool blnLeftBtn, bool blnRightBtn)
        {
            btnFirst.Enabled = btnPrevious.Enabled = blnLeftBtn;
            btnNext.Enabled = btnEnd.Enabled = blnRightBtn;
        }

        /// <summary>
        /// Handles the BtnClick event of the btnFirst control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnFirst_BtnClick(object sender, EventArgs e)
        {
            FirstPage();
        }

        /// <summary>
        /// Handles the BtnClick event of the btnPrevious control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnPrevious_BtnClick(object sender, EventArgs e)
        {
            PreviousPage();
        }

        /// <summary>
        /// Handles the BtnClick event of the btnNext control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnNext_BtnClick(object sender, EventArgs e)
        {
            NextPage();
        }

        /// <summary>
        /// Handles the BtnClick event of the btnEnd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void btnEnd_BtnClick(object sender, EventArgs e)
        {
            EndPage();
        }

        private void btnToPage_BtnClick(object sender, EventArgs e)
        {
            PageIndex = (int)txtPage.Value;
            StartIndex = (PageIndex - 1) * PageSize;
            ReloadPage();
            var s = GetCurrentSource();
            if (ShowSourceChanged != null)
            {
                ShowSourceChanged(s);
            }
        }

    }
}
