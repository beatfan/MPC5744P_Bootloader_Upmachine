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
    /// CycleProcessBar1.xaml 的交互逻辑
    /// </summary>
    public partial class CycleProcessBar1 : UserControl
    {
        public CycleProcessBar1()
        {
            InitializeComponent();
        }

        public double SetCurrentValue1
        {
            set { SetValue(value); }
            get { return 1.0; }
        }

        public void SetValue(double percentValue)
        {
            double angel = percentValue * 360; //角度
            double radius = 14;
            double leftStart = 15;

            double endLeft = 0;
            double endTop = 0;

            ArcSegment arcsegment;

            lbValue.Content = (percentValue*100).ToString() + "%";

            if (angel <= 90) 
            {
                double ra = (90 - angel) * Math.PI / 180;
                endLeft = leftStart + Math.Cos(ra) * radius;
                endTop = radius - Math.Sin(ra) * radius;

                Point arcEndPt = new Point(endLeft, endTop);
                Size arcSize = new Size(14, 14);
                SweepDirection direction = SweepDirection.Clockwise;
                arcsegment = new ArcSegment(arcEndPt, arcSize, 0, false, direction, true);
            }

            else if (angel <= 180)
            {
                double ra = (angel - 90) * Math.PI / 180;
                endLeft = leftStart + Math.Cos(ra) * radius;
                endTop = radius + Math.Sin(ra) * radius;

                Point arcEndPt = new Point(endLeft, endTop);
                Size arcSize = new Size(14, 14);
                SweepDirection direction = SweepDirection.Clockwise;
                arcsegment = new ArcSegment(arcEndPt, arcSize, 0, false, direction, true);
            }

            else if (angel <= 270)
            {
                double ra = (angel - 180) * Math.PI / 180;
                endLeft = leftStart - Math.Sin(ra) * radius;
                endTop = radius + Math.Cos(ra) * radius;
                Point arcEndPt = new Point(endLeft, endTop);
                Size arcSize = new Size(14, 14);
                SweepDirection direction = SweepDirection.Clockwise;
                arcsegment = new ArcSegment(arcEndPt, arcSize, 0, true, direction, true);
            }

            else if(angel < 360)
            {
                double ra = (angel - 270) * Math.PI / 180;
                endLeft = leftStart - Math.Cos(ra) * radius;
                endTop = radius - Math.Sin(ra) * radius;
                Point arcEndPt = new Point(endLeft, endTop);
                Size arcSize = new Size(14, 14);
                SweepDirection direction = SweepDirection.Clockwise;
                arcsegment = new ArcSegment(arcEndPt, arcSize, 0, true, direction, true);
            }
            else
                arcsegment = new ArcSegment(new Point(14,0), new Size(14,14), 0, true, SweepDirection.Clockwise, true);


            PathSegmentCollection pathsegmentCollection = new PathSegmentCollection();
            pathsegmentCollection.Add(arcsegment);

            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(15, 0);
            pathFigure.Segments = pathsegmentCollection;

            PathFigureCollection pathFigureCollection = new PathFigureCollection();
            pathFigureCollection.Add(pathFigure);

            PathGeometry pathGeometry = new PathGeometry();
            pathGeometry.Figures = pathFigureCollection;

            myCycleProcessBar1.Data = pathGeometry;

        }
    }
}
