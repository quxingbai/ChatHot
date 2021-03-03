using ChatHot.ControlLib.Tool;
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
    /// <summary>
    /// 读到内存的图片 
    /// </summary>
    public class ByteImage : Control
    {

        public String Source
        {
            get { return (String)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(String), typeof(ByteImage), new PropertyMetadata(null));



        public BitmapImage ContentImage
        {
            get { return (BitmapImage)GetValue(ContentImageProperty); }
            set { SetValue(ContentImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentImageProperty =
            DependencyProperty.Register("ContentImage", typeof(BitmapImage), typeof(ByteImage), new PropertyMetadata(null));



        static ByteImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ByteImage), new FrameworkPropertyMetadata(typeof(ByteImage)));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == SourceProperty)
            {
                ContentImage = ImageTool.GetByteImage(Source);
            }
            base.OnPropertyChanged(e);
        }

    }
}
