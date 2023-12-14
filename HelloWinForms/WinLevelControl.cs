using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class HistogramWithAdjustableDiagonalLineControl : Control
{
    public event Action<int, int> NodePositionChanged;

    private int[] histogram;
    private int windowWidth = 128;
    private int windowCenter = 128;
    private Point nodePosition = new Point(128, 128);
    private bool isDragging = false;

    public int WindowWidth
    {
        get { return windowWidth; }
        set
        {
            windowWidth = value;
            UpdateNodePosition();
            Invalidate();
        }
    }

    public int WindowCenter
    {
        get { return windowCenter; }
        set
        {
            windowCenter = value;
            UpdateNodePosition();
            Invalidate();
        }
    }

    public int[] Histogram
    {
        get { return histogram; }
        set
        {
            histogram = value;
            UpdateNodePosition();
            Invalidate();
        }
    }

    public HistogramWithAdjustableDiagonalLineControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.White;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (histogram == null)
            return;

        Graphics g = e.Graphics;
        //using (Graphics g = e.Graphics)
        {
            int barWidth = Width / histogram.Length;

            // 绘制直方图
            for (int i = 0; i < histogram.Length; i++)
            {
                int barHeight = histogram[i] * Height / histogram.Max();
                g.FillRectangle(Brushes.Blue, i * barWidth, Height - barHeight, barWidth, barHeight);
            }

            // 绘制对角线
            Pen linePen = new Pen(Color.Red, 2);

            // 绘制左上角到 nodePosition 的线段
            g.DrawLine(linePen, 0, Height, nodePosition.X, nodePosition.Y);

            // 绘制 nodePosition 到右下角的线段
            g.DrawLine(linePen, nodePosition.X, nodePosition.Y, Width, 0);


            //// 绘制对角线
            //Pen linePen = new Pen(Color.Red, 2);
            //g.DrawLine(linePen, 0, 0, Width, Height);

            // 绘制小圆节点
            int nodeSize = 10;
            g.FillEllipse(Brushes.Green, nodePosition.X - nodeSize / 2, nodePosition.Y - nodeSize / 2, nodeSize, nodeSize);
            linePen.Dispose();
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        // 如果鼠标点击在节点上，则记录初始位置以便进行拖动
        if (Math.Abs(e.X - nodePosition.X) < 10 && Math.Abs(e.Y - nodePosition.Y) < 10)
        {
            isDragging = true;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        // 如果鼠标正在拖动节点，则更新节点的位置
        if (isDragging)
        {
            nodePosition = new Point(e.X, e.Y);
            UpdateWindowParams();
            OnNodePositionChanged(nodePosition.X, nodePosition.Y);
            Invalidate(); // 重新绘制以显示新的节点位置
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        isDragging = false;
    }

    private void UpdateWindowParams()
    {
        // 根据节点的位置更新窗宽窗位等参数
        windowWidth = nodePosition.X * 2;
        windowCenter = nodePosition.Y;
    }

    private void UpdateNodePosition()
    {
        // 根据窗宽窗位等参数更新节点的位置
        nodePosition = new Point(windowCenter, windowWidth / 2);
    }

    protected virtual void OnNodePositionChanged(int x, int y)
    {
        NodePositionChanged?.Invoke(x, y);
    }
}
