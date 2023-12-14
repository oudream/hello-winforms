using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class HistogramWithAdjustableDiagonalLineControl : Control
{
    public event Action<string, int, int, int, int, int, int> NodePositionsChanged;

    private int[] histogram;
    private int windowWidth = 128;
    private int windowCenter = 128;

    private Point _minNodePosition;
    private Point _middleNodePosition;
    private Point _maxNodePosition;

    private bool isDraggingMinNode = false;
    private bool isDraggingMiddleNode = false;
    private bool isDraggingMaxNode = false;

    public int WindowWidth
    {
        get { return windowWidth; }
        set
        {
            windowWidth = value;
            UpdateNodePositions();
            Invalidate();
        }
    }

    public int WindowCenter
    {
        get { return windowCenter; }
        set
        {
            windowCenter = value;
            UpdateNodePositions();
            Invalidate();
        }
    }

    public Point MinNodePosition
    {
        get { return _minNodePosition; }
        set
        {
            _minNodePosition = value;
            UpdateWindowParams();
            OnNodePositionsChanged("min", _minNodePosition.X, _minNodePosition.Y, _middleNodePosition.X, _middleNodePosition.Y, _maxNodePosition.X, _maxNodePosition.Y);
            Invalidate();
        }
    }

    public Point MiddleNodePosition
    {
        get { return _middleNodePosition; }
        set
        {
            _middleNodePosition = value;
            UpdateWindowParams();
            OnNodePositionsChanged("middle", _minNodePosition.X, _minNodePosition.Y, _middleNodePosition.X, _middleNodePosition.Y, _maxNodePosition.X, _maxNodePosition.Y);
            Invalidate();
        }
    }

    public Point MaxNodePosition
    {
        get { return _maxNodePosition; }
        set
        {
            _maxNodePosition = value;
            UpdateWindowParams();
            OnNodePositionsChanged("max", _minNodePosition.X, _minNodePosition.Y, _middleNodePosition.X, _middleNodePosition.Y, _maxNodePosition.X, _maxNodePosition.Y);
            Invalidate();
        }
    }

    public int[] Histogram
    {
        get { return histogram; }
        set
        {
            histogram = value;
            UpdateNodePositions();
            Invalidate();
        }
    }

    public HistogramWithAdjustableDiagonalLineControl()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        SetStyle(ControlStyles.ResizeRedraw, true);
        SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        BackColor = Color.White;
        //
        _minNodePosition = new Point(0, 0);
        _middleNodePosition = new Point(128, 128);
        _maxNodePosition = new Point(Width, Height);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (histogram == null)
            return;

        Graphics g = e.Graphics;

        int barWidth = Width / histogram.Length;

        // 绘制直方图
        for (int i = 0; i < histogram.Length; i++)
        {
            int barHeight = histogram[i] * Height / histogram.Max();
            Rectangle barRect = new Rectangle(i * barWidth, Height - barHeight, barWidth, barHeight);
            g.FillRectangle(Brushes.Blue, barRect);
            g.DrawRectangle(Pens.Black, barRect); // Optional: Draw borders for each bar
        }

        // 绘制折线路径
        Pen linePen = new Pen(Color.Red, 2);

        // 左下角到 minNodePosition
        // g.DrawLine(linePen, 0, Height, minNodePosition.X, minNodePosition.Y);

        // minNodePosition 到 middleNodePosition
        g.DrawLine(linePen, _minNodePosition.X, _minNodePosition.Y, _middleNodePosition.X, _middleNodePosition.Y);

        // middleNodePosition 到 maxNodePosition
        g.DrawLine(linePen, _middleNodePosition.X, _middleNodePosition.Y, _maxNodePosition.X, _maxNodePosition.Y);

        // maxNodePosition 到 右下角
        // g.DrawLine(linePen, maxNodePosition.X, maxNodePosition.Y, Width, Height);

        // 绘制小圆节点
        int nodeSize = 10;
        g.FillEllipse(Brushes.Green, _minNodePosition.X - nodeSize / 2, _minNodePosition.Y - nodeSize / 2, nodeSize, nodeSize);
        g.FillEllipse(Brushes.Green, _middleNodePosition.X - nodeSize / 2, _middleNodePosition.Y - nodeSize / 2, nodeSize, nodeSize);
        g.FillEllipse(Brushes.Green, _maxNodePosition.X - nodeSize / 2, _maxNodePosition.Y - nodeSize / 2, nodeSize, nodeSize);

        linePen.Dispose();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        // 判断鼠标点击在哪个节点上
        if (Math.Abs(e.X - _minNodePosition.X) < 10 && Math.Abs(e.Y - _minNodePosition.Y) < 10)
        {
            isDraggingMinNode = true;
        }
        else if (Math.Abs(e.X - _middleNodePosition.X) < 10 && Math.Abs(e.Y - _middleNodePosition.Y) < 10)
        {
            isDraggingMiddleNode = true;
        }
        else if (Math.Abs(e.X - _maxNodePosition.X) < 10 && Math.Abs(e.Y - _maxNodePosition.Y) < 10)
        {
            isDraggingMaxNode = true;
        }
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        // 如果鼠标正在拖动节点，则更新节点的位置
        if (isDraggingMinNode)
        {
            _minNodePosition = new Point(e.X, e.Y);
            UpdateWindowParams();
            OnNodePositionsChanged("min", _minNodePosition.X, _minNodePosition.Y, _middleNodePosition.X, _middleNodePosition.Y, _maxNodePosition.X, _maxNodePosition.Y);
            Invalidate(); // 重新绘制以显示新的节点位置
        }
        else if (isDraggingMiddleNode)
        {
            _middleNodePosition = new Point(e.X, e.Y);
            UpdateWindowParams();
            OnNodePositionsChanged("middle", _minNodePosition.X, _minNodePosition.Y, _middleNodePosition.X, _middleNodePosition.Y, _maxNodePosition.X, _maxNodePosition.Y);
            Invalidate();
        }
        else if (isDraggingMaxNode)
        {
            _maxNodePosition = new Point(e.X, e.Y);
            UpdateWindowParams();
            OnNodePositionsChanged("max", _minNodePosition.X, _minNodePosition.Y, _middleNodePosition.X, _middleNodePosition.Y, _maxNodePosition.X, _maxNodePosition.Y);
            Invalidate();
        }
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        isDraggingMinNode = false;
        isDraggingMiddleNode = false;
        isDraggingMaxNode = false;
    }

    private void UpdateWindowParams()
    {
        // 根据节点的位置更新窗宽窗位等参数
        windowWidth = _middleNodePosition.X * 2;
        windowCenter = _middleNodePosition.Y;
    }

    private void UpdateNodePositions()
    {
        // 根据窗宽窗位等参数更新节点的位置
        _minNodePosition = new Point(0, Height);
        _middleNodePosition = new Point(windowCenter, windowWidth / 2);
        _maxNodePosition = new Point(Width, 0);
    }

    protected virtual void OnNodePositionsChanged(string name, int minX, int minY, int middleX, int middleY, int maxX, int maxY)
    {
        NodePositionsChanged?.Invoke(name, minX, minY, middleX, middleY, maxX, maxY);
    }
}
