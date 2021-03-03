using ChatHot.Model;
using System;
using System.Collections.Generic;
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

namespace ChatHot.ControlLib.UI
{
    public class MessageRow : Control
    {


        public MessageModel MessageSource
        {
            get { return (MessageModel)GetValue(MessageSourceProperty); }
            set { SetValue(MessageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageSourceProperty =
            DependencyProperty.Register("MessageSource", typeof(MessageModel), typeof(MessageRow), new PropertyMetadata(null));



        public bool IsFrom
        {
            get { return (bool)GetValue(IsFromProperty); }
            set { SetValue(IsFromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsFrom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsFromProperty =
            DependencyProperty.Register("IsFrom", typeof(bool), typeof(MessageRow), new PropertyMetadata(false));


        static MessageRow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageRow), new FrameworkPropertyMetadata(typeof(MessageRow)));
        }
    }
}
