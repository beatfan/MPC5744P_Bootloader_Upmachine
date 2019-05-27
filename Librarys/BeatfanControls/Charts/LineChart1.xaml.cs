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

namespace BeatfanControls.Charts
{
    /// <summary>
    /// LineChartData1.xaml 的交互逻辑
    /// </summary>
    public partial class LineChart1 : UserControl
    {
        int m_MaxHeight = 0;
        int m_MaxWidth = 0;

        int m_PathWidth = 300;
        int m_PathHeight = 200;

        /// <summary>
        /// 水平坐标
        /// </summary>
        int m_HorizontalPoint;


        PathGeometry m_PathGeometry = new PathGeometry();

        PathFigure m_PathFigure = new PathFigure { IsClosed = false };

        public struct Scm_OneData
        {
            public int transtion;
            public int durationSeconds;
            public int value;
        }

        public LineChart1()
        {
            InitializeComponent();
            
            m_PathGeometry.Figures.Add(m_PathFigure);
            myLineChart1.Data = m_PathGeometry;

            //动画组
            TransformGroup tfg = new TransformGroup();


            //镜像
            ScaleTransform rot = new ScaleTransform();
            rot.CenterX = m_PathWidth/2;
            rot.CenterY = m_PathHeight/2;
            rot.ScaleX = 1;
            rot.ScaleY = -1;

            //dbAscending.RepeatBehavior = RepeatBehavior.Forever; //重复
            tfg.Children.Add(rot);
            //rot.BeginAnimation(RotateTransform.AngleProperty, dbAscendingRotate);

            myLineChart1.RenderTransform = tfg;
        }


        /// <summary>
        /// 新增值
        /// </summary>
        private void AddValue(Scm_OneData oneData)
        {

            if (m_HorizontalPoint == 0)
            {
                m_PathFigure.StartPoint = new Point(m_HorizontalPoint, oneData.value * m_PathHeight / m_MaxHeight);
            }
            else
            {
                m_HorizontalPoint = (int)(m_HorizontalPoint + oneData.transtion * m_PathWidth / m_MaxWidth);

                m_PathFigure.Segments.Add(
                    new LineSegment
                    {
                        Point = new Point(m_HorizontalPoint,oneData.value * m_PathHeight / m_MaxHeight)
                    });
            }

            m_HorizontalPoint = (int)(m_HorizontalPoint + oneData.durationSeconds * m_PathWidth / m_MaxWidth);
            m_PathFigure.Segments.Add(
                new LineSegment
                {
                    Point = new Point(m_HorizontalPoint,oneData.value * m_PathHeight / m_MaxHeight)
                });

        }

        /// <summary>
        /// 新增列表
        /// </summary>
        public void AddList(List<Scm_OneData> dataList)
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                if (m_MaxHeight < dataList[i].value)
                    m_MaxHeight = dataList[i].value;
                m_MaxWidth += dataList[i].durationSeconds + dataList[i].transtion;
            }

            m_MaxHeight += 10;
            m_MaxWidth += 10;

            for (int i = 0; i < dataList.Count; i++)
            {
                AddValue(dataList[i]);
            }
            
        }
    }
}