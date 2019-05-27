using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BeatfanControls.Labels
{
    public class EditLabel : Grid
    {
        
        public EditLabel()
        {
            //DataContext = m_CurrentValue;
            //Content = "0";
            Label lb = new Label();
            TextBox tb = new TextBox();
            
            Children.Add(lb);
            Children.Add(tb);
            lb.HorizontalAlignment = HorizontalAlignment.Stretch;
            tb.HorizontalAlignment = HorizontalAlignment.Stretch;
            tb.Width = ((Grid)tb.Parent).Width;
            tb.Visibility = Visibility.Collapsed;
            lb.MouseDoubleClick += Lb_MouseDoubleClick;
            tb.MouseLeave += Tb_MouseLeave;
        }

        private void Tb_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid g = ((TextBox)sender).Parent as Grid;
            g.Children[1].Visibility = Visibility.Collapsed;
            g.Children[0].Visibility = Visibility.Visible;
            Label lb = g.Children[0] as Label;
            TextBox tb = g.Children[1] as TextBox;

            Content = tb.Text;
        }



        private void Lb_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid g = ((Label)sender).Parent as Grid;
            g.Children[0].Visibility = Visibility.Collapsed;
            g.Children[1].Visibility = Visibility.Visible;
            Label lb = g.Children[0] as Label;
            TextBox tb = g.Children[1] as TextBox;
            tb.Text = lb.Content.ToString();
        }



        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content",
        typeof(string), typeof(EditLabel), new FrameworkPropertyMetadata("", new PropertyChangedCallback(ContentChanged)));


        /// <summary>
        /// 当前显示值
        /// </summary>
        public string Content
        {
            get
            {
                return (string)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }

        private static void ContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            EditLabel gauge = d as EditLabel;
            Label lb = gauge.Children[0] as Label;
            lb.Content = e.NewValue.ToString();
        }


    }
}
