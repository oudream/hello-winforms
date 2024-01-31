using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public class Coordinate
    {

        public static Point RotatePoint(Point point, double angle)
        {
            double angleInRadians = angle * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);

            double rotatedX = point.X * cosTheta - point.Y * sinTheta;
            double rotatedY = point.X * sinTheta + point.Y * cosTheta;

            return new Point(rotatedX, rotatedY);
        }

        public static Point TransformToA(Point pointB, Point originA, double angle)
        {
            Point rotatedPoint = RotatePoint(pointB, angle);
            return new Point(rotatedPoint.X + originA.X, rotatedPoint.Y + originA.Y);
        }

        // originA_B: 坐标系B中的原点在坐标系A中值
        // angle: 旋转角度
        // l: 边长
        public static (Point, Point) GetOriginA(Point originA_B, double angle, double l)
        {
            // 假设A点坐标、边长l和旋转角度ga已知
            Point pointA_B = new Point(0, 0); // A点在坐标系B中的坐标

            // 计算C点和D点在坐标系B中的坐标
            Point pointC_B = new Point(0, 0 + l);
            Point pointD_B = new Point(0 - l, 0 + l);

            // 计算C点和D点在坐标系A中的坐标
            Point pointC_A = TransformToA(pointC_B, originA_B, angle + 180);
            Point pointD_A = TransformToA(pointD_B, originA_B, angle + 180);

            return (pointC_A, pointD_A);
        }

        public static void TestTransformToA()
        {
            // 定义坐标系A的原点和坐标系B中的点
            Point originA = new Point(2, 3);
            Point pointB = new Point(5, -4);
            double angle = 30; // 旋转角度

            // 执行坐标变换
            Point transformedPoint = TransformToA(pointB, originA, angle);

            // 输出变换后的坐标
            Console.WriteLine($"Transformed Point: ({transformedPoint.X:0.000}, {transformedPoint.Y:0.000})");
        }

        public static void TestGetOriginA()
        {
            // 定义坐标系A的原点和坐标系B中的点
            Point originA_B = new Point(1, 4);
            // 旋转角度
            double angle = 30; // 旋转角度
            // 边长
            double l = 4;

            Point pointC_A;
            Point pointD_A;

            (pointC_A, pointD_A) = GetOriginA(originA_B, angle, l);

            // 输出变换后的坐标
            Console.WriteLine($"pointC_A: ({pointC_A.X:0.000}, {pointC_A.Y:0.000}), pointC_A: ({pointD_A.X:0.000}, {pointD_A.Y:0.000})");
        }






        // 以上的是坐标变换




        // 以下的是直接运算




        // 注意：倾斜角度分正负（目前测试过+90 ~ -90）
        public static void TestStep2()
        {
            //double xA = 200;  // 示例坐标，需要替换为实际值
            //double yA = 200;  // 示例坐标，需要替换为实际值
            //double xC = 77;  // 示例坐标，需要替换为实际值
            //double yC = 387;  // 示例坐标，需要替换为实际值
            //double g = 30;   // 倾斜角度（度），需要替换为实际值

            double xA = 300;  // 示例坐标，需要替换为实际值
            double yA = 387;  // 示例坐标，需要替换为实际值
            double xC = 77;  // 示例坐标，需要替换为实际值
            double yC = 387;  // 示例坐标，需要替换为实际值
            double g = -30;   // 倾斜角度（度），需要替换为实际值

            double xB;
            double yB;
            double xD;
            double yD;
            (xB, yB, xD, yD) = CalculateCoordinates(xA, yA, xC, yC, g);

            // Output the results
            Console.WriteLine($"Coordinates of B: ({xB}, {yB})");
            Console.WriteLine($"Coordinates of D: ({xD}, {yD})");
        }


        // Method to calculate the distance between two points A and C
        public static double CalculateDiagonalLength(double xA, double yA, double xC, double yC)
        {
            double deltaX = xA - xC;
            double deltaY = yA - yC;
            double diagonalLength = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
            return diagonalLength;
        }

        public static double CalculateAngleACtoCD(double xA, double yA, double xC, double yC, double gDegrees)
        {
            // Calculate the slope (m) of the line AC
            double m = (yC - yA) / (xC - xA);

            // Convert the slope to an angle in radians
            double thetaRadians = Math.Atan(m);

            // Convert theta to degrees
            double thetaDegrees = thetaRadians * (180 / Math.PI);

            // Calculate the difference between the angles
            double angleDifference = Math.Abs(Math.Abs(thetaDegrees) - gDegrees);

            return angleDifference;
        }

        public static (double, double) CalculateSidesLengths(double xA, double yA, double xC, double yC, double gDegrees)
        {
            double diagonalLength = CalculateDiagonalLength(xA, yA, xC, yC);
            double angleACtoCD = CalculateAngleACtoCD(xA, yA, xC, yC, gDegrees);

            // Convert angle from degrees to radians
            double angleRadians = angleACtoCD * (Math.PI / 180);

            // Calculate lengths of AB and BC
            double AB = diagonalLength * Math.Cos(angleRadians);
            double BC = diagonalLength * Math.Sin(angleRadians);

            return (AB, BC);
        }

        // Method to calculate the coordinates of point B
        public static (double xB, double yB) CalculatePointB(double xC, double yC, double BC, double gDegrees)
        {
            double gRadians = Math.PI * gDegrees / 180.0;
            double xB = xC - BC * Math.Sin(gRadians);
            double yB = yC - BC * Math.Cos(gRadians);
            return (xB, yB);
        }

        // Method to calculate the coordinates of point D
        public static (double xD, double yD) CalculatePointD(double xC, double yC, double AB, double gDegrees)
        {
            double gRadians = Math.PI * gDegrees / 180.0;
            double xD = xC + AB * Math.Cos(gRadians);
            double yD = yC - AB * Math.Sin(gRadians);
            return (xD, yD);
        }

        public static (double xB, double yB, double xD, double yD) CalculateCoordinates(double xA, double yA, double xC, double yC, double gDegrees)
        {
            // Calculate the lengths of sides AB and BC
            (double AB, double BC) = CalculateSidesLengths(xA, yA, xC, yC, gDegrees);

            // Calculate the coordinates for B
            (double xB, double yB) = CalculatePointB(xC, yC, BC, gDegrees);

            // Calculate the coordinates for D
            (double xD, double yD) = CalculatePointD(xC, yC, AB, gDegrees);

            return (xB, yB, xD, yD);
        }
    }


    public partial class HelloCoordinate : Form
    {
        public HelloCoordinate()
        {
            InitializeComponent();
        }

        private void HelloCoordinate_Load(object sender, EventArgs e)
        {

        }
    }
}
