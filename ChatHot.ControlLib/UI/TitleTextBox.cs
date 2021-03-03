﻿using System;
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
    public class TitleTextBox : TextBox
    {


        public Brush TitleForegroudn
        {
            get { return (Brush)GetValue(TitleForegroudnProperty); }
            set { SetValue(TitleForegroudnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleForegroudn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleForegroudnProperty =
            DependencyProperty.Register("TitleForegroudn", typeof(Brush), typeof(TitleTextBox), new PropertyMetadata(Brushes.Gray));



        public String Title
        {
            get { return (String)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(String), typeof(TitleTextBox), new PropertyMetadata(null));


        static TitleTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleTextBox), new FrameworkPropertyMetadata(typeof(TitleTextBox)));
        }
        public TitleTextBox()
        {

        }
    }
}
