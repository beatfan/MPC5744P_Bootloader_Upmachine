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
using System.Windows.Shapes;
using NXP_HexParse.Datas;
using NXP_HexParse.Config;

namespace NXP_HexParse.Controls.Windows
{
    /// <summary>
    /// AdministratorWindow1.xaml 的交互逻辑
    /// </summary>
    public partial class UserMessageBox : Window
    {
        ClassUser clsUser = new ClassUser();


        public UserMessageBox(Point location, Size size, string headerTitle, string warnningMessage)
        {
            InitializeComponent();


            this.Left = location.X + (size.Width - this.Width) / 2;
            this.Top = location.Y + (size.Height - this.Height) / 2;

            this.Title = headerTitle;
            lbMessage.Content = warnningMessage;
        }


        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(UserMessageBox),
            new PropertyMetadata("提示", new PropertyChangedCallback(OnCaptionChanged)));

        /// <summary>
        /// Dependency property to Get/Set the current value 
        /// </summary>
        public static readonly DependencyProperty UserContentProperty =
            DependencyProperty.Register("UserContent", typeof(string), typeof(UserMessageBox),
            new PropertyMetadata("内容", new PropertyChangedCallback(OnUserContentChanged)));


        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public string UserContent
        {
            get
            {
                return (string)GetValue(UserContentProperty);
            }
            set
            {
                SetValue(UserContentProperty, value);
            }
        }

        private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            UserMessageBox gauge = d as UserMessageBox;
            gauge.Title = e.NewValue.ToString();
        }

        private static void OnUserContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Get access to the instance of CircularGaugeConrol whose property value changed
            UserMessageBox gauge = d as UserMessageBox;
            gauge.lbMessage.Content = e.NewValue.ToString();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
