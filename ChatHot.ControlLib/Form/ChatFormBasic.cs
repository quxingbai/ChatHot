using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatHot.ControlLib.Form
{
    public class ChatFormBasic : Window
    {



        public Visibility MinButtonVisibility
        {
            get { return (Visibility)GetValue(MinButtonVisibilityProperty); }
            set { SetValue(MinButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinButtonVisibilityProperty =
            DependencyProperty.Register("MinButtonVisibility", typeof(Visibility), typeof(ChatFormBasic), new PropertyMetadata(Visibility.Visible));



        public Visibility MaxButtonVisibility
        {
            get { return (Visibility)GetValue(MaxButtonVisibilityProperty); }
            set { SetValue(MaxButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxButtonVisibilityProperty =
            DependencyProperty.Register("MaxButtonVisibility", typeof(Visibility), typeof(ChatFormBasic), new PropertyMetadata(Visibility.Visible));



        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseButtonVisibilityProperty =
            DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(ChatFormBasic), new PropertyMetadata(Visibility.Visible));

        static ChatFormBasic()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatFormBasic), new FrameworkPropertyMetadata(typeof(ChatFormBasic)));
        }
        public void OnCloseALL()
        {
            Dispatcher.Invoke(() =>
            {
                foreach (Window w in Application.Current.Windows)
                {
                    w.Close();
                }
                Environment.Exit(0);
                Application.Current.Shutdown();
            });
        }
        public void Close(int CloseTime)
        {
            new Thread(()=>
            {
                Dispatcher.Invoke(() =>
                {
                    BeginAnimation(OpacityProperty, new DoubleAnimation(0, new Duration(TimeSpan.Parse("0:0:1.9"))));
                });
                Thread.Sleep(CloseTime);
                Dispatcher.Invoke(Close);
            }).Start();
        }
        public void StartDoubleAnima(DependencyProperty Property,Double To, String Time)
        {
            Dispatcher.Invoke(()=>
            {
                BeginAnimation(Property, new DoubleAnimation(To, new Duration(TimeSpan.Parse(Time))));
            });
        }
    }
}
