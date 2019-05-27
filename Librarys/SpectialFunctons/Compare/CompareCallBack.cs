using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecialFunctions.Compare
{
    /// <summary>
    /// 比较回调，符合条件则回调
    /// </summary>
    public class CompareCallBack
    {
        public delegate void DeletageCallBack();


        /// <summary>
        /// 已操作次数
        /// </summary>
        int m_CompareResultTimers = 0;

        /// <summary>
        /// 时间过渡，增加值
        /// realTime为实际时间
        /// targetTime为总过渡时间
        /// isRise，表示是否上升
        /// gapValue为过渡值
        /// targetValue为实际值
        /// currentValue为当前值
        /// </summary>
        public void CompareAddGapValue(double realTime, double targetTime, double gapValue,  double targetValue, ref double currentValue)
        {
            //时间范围内
            if (realTime < targetTime)
            {
                currentValue += gapValue;

                //值超出范围
                if ( (gapValue>0 && currentValue >targetValue) || ((gapValue < 0) && currentValue < targetValue))
                    currentValue = targetValue;

            }
            //超出过渡时间
            else
                currentValue = targetValue;
        }

        /// <summary>
        /// realValue和targetValue以及tolerance全部为非负值
        /// 比较五次，符合则回调
        /// 第一次是值接近0时
        /// 第二次是四分之一时
        /// 第三次为二分之一时
        /// 第四次为四分之三时
        /// 第五次为接近目标值时
        /// tolerance为比较容差
        /// callback为回调的函数
        /// </summary>
        public void CompareFiveTimesCallBack(double realTime, double targetTime, double toleranceTime, DeletageCallBack callback)
        {
            if (realTime < 0 || targetTime < 0)
                return;
            
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("实际时间:{0},目标时间:{1},容差:{2},记录次数:{3}",realTime,targetTime, toleranceTime, m_CompareResultTimers);
            if (Math.Abs(realTime- toleranceTime) <= toleranceTime && m_CompareResultTimers == 0) //开始记录一次
            {
                m_CompareResultTimers = 1;
                if (callback != null)
                    callback();
            }

            if ((Math.Abs((targetTime / 4) - realTime) <= toleranceTime) && m_CompareResultTimers == 1) // 四分之一记录一次
            {
                m_CompareResultTimers = 2;
                if (callback != null)
                    callback();
            }

            if ((Math.Abs((targetTime / 2) - realTime) <= toleranceTime) && m_CompareResultTimers == 2) //二分之一记录一次
            {
                m_CompareResultTimers = 3;
                if (callback != null)
                    callback();
            }

            if ((Math.Abs((targetTime * 0.75) - realTime) <= toleranceTime) && m_CompareResultTimers == 3) //四分之三记录一次
            {
                m_CompareResultTimers =4;
                if (callback != null)
                    callback();
            }

            if ((Math.Abs(targetTime - realTime) <= toleranceTime) && m_CompareResultTimers == 4)  //最终记录一次
            {
                m_CompareResultTimers = 0;
                if (callback != null)
                    callback();
            }
        }

    }
}
