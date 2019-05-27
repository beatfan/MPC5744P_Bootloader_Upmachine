using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BeatfanControls.Shapes
{
    /// <summary>
    /// WarnningShape1.xaml 的交互逻辑
    /// </summary>
    public partial class WarnningShape1 : UserControl
    {
        public enum WarnningStates : uint
        {
            Normal = 0,
            Warnning,
            Dangerous
        }

        public WarnningShape1()
        {
            InitializeComponent();
        }

        private static DependencyProperty WarnningStateProperty = DependencyProperty.Register("WarnningState", typeof(WarnningStates), typeof(WarnningShape1),
            new PropertyMetadata(WarnningStates.Warnning, new PropertyChangedCallback(OnWarnningStateChanged)));


        /// <summary>
        /// 显示警告状态
        /// </summary>
        public WarnningStates WarnningState
        {
            set { SetValue(WarnningStateProperty, value);  }
            get { return (WarnningStates)GetValue(WarnningStateProperty); }
        }

        private static void OnWarnningStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WarnningShape1 ws = d as WarnningShape1;
            switch ((WarnningStates)e.NewValue)
            {
                case WarnningStates.Normal:
                    ws.DangerIco.Fill = Brushes.Green;
                    break;
                case WarnningStates.Warnning:
                    ws.DangerIco.Fill = Brushes.Yellow;
                    break;
                case WarnningStates.Dangerous:
                    ws.DangerIco.Fill = Brushes.Red;
                    break;
                default:
                    ws.DangerIco.Fill = Brushes.Green;
                    break;
            }
        }

    }
}
