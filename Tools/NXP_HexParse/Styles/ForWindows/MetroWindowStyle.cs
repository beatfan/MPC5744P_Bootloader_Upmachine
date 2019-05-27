using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Collections.Generic;

namespace NXP_HexParse.Styles.ForWindows
{
    public partial class MetroWindowStyle
    {
        

        private void CloseWinButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;

            if (win.Tag!=null)
            {
                #region 关闭提醒
                Controls.Windows.UserMessageBox myMessageBox = new Controls.Windows.UserMessageBox(
                    new Point(win.Left, win.Top), new Size(win.Width, win.Height),
                    "警告", "确认要关闭本程序吗?");

                myMessageBox.Owner = win;
                if (!myMessageBox.ShowDialog().Value)
                {
                    return;
                }
                #endregion
            }

            //if (!myMessageBox.ShowDialog().Value)
            //    return;

            //if (MessageBox.Show("确定要关闭吗?", "警告", MessageBoxButton.OKCancel, MessageBoxImage.Warning) != MessageBoxResult.OK)
            //    return;

            win.Close();
        }

        private void MinWinButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            win.WindowState = System.Windows.WindowState.Minimized;
        }

        private void MaxWinButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            if (win.WindowState == WindowState.Normal)
                win.WindowState = System.Windows.WindowState.Maximized;
            else
                win.WindowState = WindowState.Normal;
        }


        /// <summary>
        /// 实现窗体移动
        /// </summary>
        /// <param name="e"></param>
        private void TitleGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;

            if (e.ClickCount == 2)
            {
                win.WindowState = win.WindowState==WindowState.Maximized? WindowState.Normal: WindowState.Maximized;
                return;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //若是最大化状态，则先回复正常大小，在将窗体移动到鼠标处
                if (win.WindowState == WindowState.Maximized)
                    return;
                //{
                //    win.WindowState = WindowState.Normal;
                //    //设置窗体位置，保持左边和顶边不变
                //    win.Left = System.Windows.Forms.Cursor.Position.X - win.ActualWidth/2;
                //    win.Top = System.Windows.Forms.Cursor.Position.Y - e.GetPosition(win).Y;

                //}
                //随着鼠标移动
                win.DragMove();
            }
        }

        private void TitleGrid_DragLeave(object sender, MouseButtonEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            //若是最大化状态，则先回复正常大小，在将窗体移动到鼠标处
            if (win.WindowState == WindowState.Maximized)
                return;
            //{
            //    win.WindowState = WindowState.Normal;
            //    //设置窗体位置，保持左边和顶边不变
            //    win.Left = System.Windows.Forms.Cursor.Position.X - win.ActualWidth / 2;
            //    win.Top = System.Windows.Forms.Cursor.Position.Y - e.GetPosition(win).Y;

            //}
            //随着鼠标移动
            win.DragMove();
        }

        private void TitleGrid_MouseEnter(object sender, System.Windows.RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            win.Cursor = Cursors.Hand;
        }

        private void TitleGrid_MouseLeave(object sender, System.Windows.RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            win.Cursor = Cursors.Arrow;
        }


        private void ResizeLeft_MouseEnter(object sender, System.Windows.RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            if (win.WindowState == WindowState.Maximized)
                return;
            win.Cursor = Cursors.SizeNWSE;
        }

        private void ResizeLeft_MouseLeave(object sender, System.Windows.RoutedEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            if (win.WindowState == WindowState.Maximized)
                return;
            win.Cursor = Cursors.Arrow;
        }

        private void ResizeLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Window win = (Window)((FrameworkElement)sender).TemplatedParent;
            if (win.WindowState == WindowState.Maximized)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            Point pit = e.GetPosition(win);
            win.Width = pit.X;
            win.Height = pit.Y;

        }

    }

}
