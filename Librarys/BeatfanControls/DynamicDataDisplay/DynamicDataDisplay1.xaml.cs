using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Research.DynamicDataDisplay;

namespace BeatfanControls.DynamicDataDisplay
{
    /// <summary>
    /// DynamicDataDisplay1.xaml 的交互逻辑
    /// </summary>
    public partial class DynamicDataDisplay1 : UserControl
    {
        /// <summary>
        /// 多路源的点集合的集合
        /// </summary>
        List<ObservableDataSource<Point>> m_LineGraphSources = new List<ObservableDataSource<Point>>();

        List<List<Point>> m_AddPoints = new List<List<Point>>();

        System.Windows.Threading.DispatcherTimer m_ShowTimer = new System.Windows.Threading.DispatcherTimer();

        public DynamicDataDisplay1()
        {
            InitializeComponent();

            m_ShowTimer.Tick += M_ShowTimer_Tick;
            m_ShowTimer.Interval = new TimeSpan(0,0,0,0,200);

            m_ShowTimer.Start();
        }

        private void M_ShowTimer_Tick(object sender, EventArgs e)
        {
            lock(this)
            {
                for (int i = 0; i < m_LineGraphSources.Count; i++)
                {
                    if (m_AddPoints[i].Count == 0)
                        continue;
                    m_LineGraphSources[i].AppendMany(m_AddPoints[i]);

                    //超过5千个点则删除前面的
                    while (m_LineGraphSources[i].Collection.Count > 5000)
                        m_LineGraphSources[i].Collection.RemoveAt(0);

                    m_AddPoints[i].Clear();
                }

                
            }
        }

        /// <summary>
        /// sourceNum为线条数量
        /// lineColors为线条颜色，数组长度与sourceNum保持一致
        /// lineNames为线条名称，数组长度与sourceNum保持一致
        /// </summary>
        public void Initial(int sourceNum, Color[] lineColors, string[] lineNames, int legendFontSize=9)
        {
            plotter1.Legend.LegendLeft = 5;
            plotter1.Legend.FontSize = legendFontSize;

            for (int i = 0; i < sourceNum; i++)
            {
                List<Point> addPoint = new List<Point>();
                m_AddPoints.Add(addPoint);

                ObservableDataSource<Point> lineGraphSource = new ObservableDataSource<Point>();
                lineGraphSource.SetXYMapping(p => p);
                plotter1.AddLineGraph(lineGraphSource, lineColors[i], 1, lineNames[i]);
                m_LineGraphSources.Add(lineGraphSource);
            }
        }

        /// <summary>
        /// 新增点
        /// sourceIndex为线条索引，即第几个线条，顺序与初始化传入的颜色和名称保持一致
        /// newPoint为传入的新点
        /// </summary>
        /// <param name="sourceIndex"></param>
        /// <param name="newPoint"></param>
        public void AddPoint(int sourceIndex, Point newPoint)
        {

            lock (this)
            {
                m_AddPoints[sourceIndex].Add(newPoint);
            }
        }

        
    }
}
