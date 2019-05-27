using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatfanControls.ProcessBars
{
    /// <summary>
    /// CycleProcessBar2.xaml 的交互逻辑
    /// </summary>
    public partial class CycleProcessBar2 : UserControl
    {
        
        public CycleProcessBar2()
        {
            InitializeComponent();
        }

        private static readonly DependencyProperty CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(double), typeof(CycleProcessBar2),
            new PropertyMetadata(0.3, OnCurrentValueChanged));


        private static readonly DependencyProperty CurrentValueListProperty = DependencyProperty.Register("CurrentValueList", typeof(List<double>), typeof(CycleProcessBar2),
            new PropertyMetadata(new List<double>() { 0, 0 }, OnCurrentValueListChanged));

        /// <summary>
        /// 设置当前值
        /// 百分比，小于1
        /// </summary>
        public double CurrentValue
        {
            set { SetValue(CurrentValueProperty, value); }
            get { return (double)GetValue(CurrentValueProperty); }
        }

        /// <summary>
        /// 设置当前值
        /// 已完成值和总值
        /// </summary>
        public List<double> CurrentValueList
        {
            set { SetValue(CurrentValueListProperty, value); }
            get { return (List<double>)GetValue(CurrentValueListProperty); }
        }

        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CycleProcessBar2 cpb = d as CycleProcessBar2;
            double percentValue = (double)e.NewValue;

            if (percentValue < 0.3)
                cpb.myCycleProcessBar2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF,0xEA,0x6A,0x12));
            else if (percentValue < 0.7)
                cpb.myCycleProcessBar2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF,0xD6,0xBB,0x0F));
            else
                cpb.myCycleProcessBar2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF,0x12,0xCB,0x66));

            cpb.lbProcessContent.Content = (percentValue * 100).ToString() + "%";
            //100%就会归零，此处不归零
            cpb.myCycleProcessBar2.Data = cpb.GetGeometry(percentValue == 1 ? 0.99 : percentValue);
        }

        private static void OnCurrentValueListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CycleProcessBar2 cpb = d as CycleProcessBar2;
            List<double> valueList = (List<double>)e.NewValue;

            double percentValue = valueList[0] / valueList[1];

            if (percentValue < 0.3)
                cpb.myCycleProcessBar2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xEA, 0x6A, 0x12));
            else if (percentValue < 0.7)
                cpb.myCycleProcessBar2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xD6, 0xBB, 0x0F));
            else
                cpb.myCycleProcessBar2.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x12, 0xCB, 0x66));

            cpb.lbProcessContent.Content = valueList[0].ToString()+ "\r\n" + valueList[1].ToString();
            //100%就会归零，此处不让不归零
            cpb.myCycleProcessBar2.Data = cpb.GetGeometry(percentValue >= 1 ? 0.99 : percentValue);
        }

        #region 处理画图
        //根据已保存的大小和文件总大小来计算下载进度百分比
        private Geometry GetGeometry(double percent)
        {
            bool isLargeArc = false;
            double angel = percent * 360D;
            if (angel > 180) isLargeArc = true;
            //double angel = 45;
            double bigR = 16;
            double smallR = 14;
            Point centerPoint = new Point(18, 18);//new Point(100, 300);
            Point firstpoint = GetPointOnCir(centerPoint, bigR, 0);
            Point secondpoint = GetPointOnCir(centerPoint, bigR, angel);
            Point thirdpoint = GetPointOnCir(centerPoint, smallR, 0);
            Point fourpoint = GetPointOnCir(centerPoint, smallR, angel);
            return drawingArc(firstpoint, secondpoint, thirdpoint, fourpoint, bigR, smallR, isLargeArc);
        }

        private Point GetPointOnCir(Point CenterPoint, double r, double angel)
        {
            Point p = new Point();
            p.X = Math.Sin(angel * Math.PI / 180) * r + CenterPoint.X;
            p.Y = CenterPoint.Y - Math.Cos(angel * Math.PI / 180) * r;
            return p;
        }

        private Geometry drawingArc(Point bigCirclefirstPoint, Point bigCirclesecondPoint, Point smallCirclefirstPoint, Point smallCirclesecondPoint, double bigCircleRadius, double smallCircleRadius, bool isLargeArc)
        {
            PathFigure pathFigure = new PathFigure { IsClosed = true };
            pathFigure.StartPoint = bigCirclefirstPoint;
            pathFigure.Segments.Add(
              new ArcSegment
              {
                  Point = bigCirclesecondPoint,
                  IsLargeArc = isLargeArc,
                  Size = new Size(bigCircleRadius, bigCircleRadius),
                  SweepDirection = SweepDirection.Clockwise
              });
            pathFigure.Segments.Add(new LineSegment { Point = smallCirclesecondPoint });
            pathFigure.Segments.Add(
             new ArcSegment
             {
                 Point = smallCirclefirstPoint,
                 IsLargeArc = isLargeArc,
                 Size = new Size(smallCircleRadius, smallCircleRadius),
                 SweepDirection = SweepDirection.Counterclockwise
             });
            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures.Add(pathFigure);
            return pathGeometry;
        }
        #endregion


        #region DrawingVisual
        //public Visual drawShape()
        //{
        //    DrawingVisual drawingWordsVisual = new DrawingVisual();
        //    DrawingContext drawingContext = drawingWordsVisual.RenderOpen();
        //    try
        //    {

        //        if (savedSize != fileSize)
        //        {

        //            drawingContext.DrawEllipse(null, new Pen(Brushes.Gray, 3), vl.StartPoint, 13, 13);
        //            drawingContext.DrawGeometry(vs.VisualBackgroundBrush, vs.VisualFramePen, GetGeometry());
        //            FormattedText formatWords = new FormattedText(PercentString, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(vs.WordsFont.Name), vs.WordsFont.Size, currentStyle.VisualBackgroundBrush);
        //            formatWords.SetFontWeight(FontWeights.Bold);
        //            Point startPoint = new Point(vl.StartPoint.X - formatWords.Width / 2, vl.StartPoint.Y - formatWords.Height / 2);
        //            drawingContext.DrawText(formatWords, startPoint);
        //        }
        //        else
        //        {
        //            drawingContext.DrawEllipse(null, new Pen(Brushes.Green, 3), vl.StartPoint, 16, 16);
        //            FormattedText formatWords = new FormattedText("Open", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface(vs.WordsFont.Name), vs.WordsFont.Size, Brushes.Red);
        //            formatWords.SetFontWeight(FontWeights.Bold);
        //            Point startPoint = new Point(vl.StartPoint.X - formatWords.Width / 2, vl.StartPoint.Y - formatWords.Height / 2);
        //            drawingContext.DrawText(formatWords, startPoint);
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    finally
        //    {
        //        drawingContext.Close();
        //    }
        //    return drawingWordsVisual;
        //}
        #endregion

    }
}
