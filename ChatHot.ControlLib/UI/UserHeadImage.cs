using ChatHot.ControlLib.Tool;
using ChatHot.Model;
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

namespace ChatHot.ControlLib.UI
{
    public class UserHeadImage : ContentControl
    {


        public long LoginNumber
        {
            get { return (long)GetValue(LoginNumberProperty); }
            set { SetValue(LoginNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LoginNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoginNumberProperty =
            DependencyProperty.Register("LoginNumber", typeof(long), typeof(UserHeadImage), new PropertyMetadata(long.Parse("0")));


        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(UserHeadImage), new PropertyMetadata(null));


        static UserHeadImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UserHeadImage), new FrameworkPropertyMetadata(typeof(UserHeadImage)));
        }
        public UserHeadImage()
        {
           StaticResource.HeadImageSourceChanged += UserHeadImage_HeadImageSourceChanged;
        }
        private void UserHeadImage_HeadImageSourceChanged(ImageSource obj, long num)
        {
            Dispatcher.Invoke(() =>
            {
                if (LoginNumber == num)
                    Source = obj;
            });
            if (StaticResource.HeadImageSource.Keys.Where(k => k == num).Count() > 0)
            {
                StaticResource.HeadImageSource[num] = obj;
            }
            else
            {
                StaticResource.HeadImageSource.Add(num,obj);
            }
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == LoginNumberProperty)
            {
                if (StaticResource.HeadImageSource.Keys.Where(k => k == LoginNumber).Count() > 0)
                {
                    Source = StaticResource.HeadImageSource[LoginNumber];
                }
                else
                {
                    try
                    {
                        Source = ImageTool.GetUserHeadeImage(LoginNumber);
                        StaticResource.HeadImageSource.Add(LoginNumber,Source);
                    }
                    catch
                    {
                        Source = null;
                    }
                }
            }
            base.OnPropertyChanged(e);
        }
    }
}
