using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatHot.ControlLib.UI
{
    public class ImageButton : ButtonBase
    {



        public ImageSource ImageContent
        {
            get { return (ImageSource)GetValue(ImageContentProperty); }
            set { SetValue(ImageContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageContentProperty =
            DependencyProperty.Register("ImageContent", typeof(ImageSource), typeof(ImageButton), new PropertyMetadata(null));



        public Brush IsMouseOverBackgroudn
        {
            get { return (Brush)GetValue(IsMouseOverBackgroudnProperty); }
            set { SetValue(IsMouseOverBackgroudnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsMouseOverBackgroudn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsMouseOverBackgroudnProperty =
            DependencyProperty.Register("IsMouseOverBackgroudn", typeof(Brush), typeof(ImageButton), new PropertyMetadata(null));




        public double ImageWidth
        {
            get { return (double)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageWidthProperty =
            DependencyProperty.Register("ImageWidth", typeof(double), typeof(ImageButton), new PropertyMetadata(50.0));



        public Double ImageHeight
        {
            get { return (Double)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageHeightProperty =
            DependencyProperty.Register("ImageHeight", typeof(Double), typeof(ImageButton), new PropertyMetadata(50.0));



        static ImageButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(typeof(ImageButton)));
        }
    }
}
