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
    public class TitleBox : ContentControl
    {


        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(TitleBox), new PropertyMetadata(null));



        public bool IsDragMove
        {
            get { return (bool)GetValue(IsDragMoveProperty); }
            set { SetValue(IsDragMoveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDragMove.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDragMoveProperty =
            DependencyProperty.Register("IsDragMove", typeof(bool), typeof(TitleBox), new PropertyMetadata(false));



        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(TitleBox), new PropertyMetadata(null));



        public ICommand MaxCommand
        {
            get { return (ICommand)GetValue(MaxCommandProperty); }
            set { SetValue(MaxCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxCommandProperty =
            DependencyProperty.Register("MaxCommand", typeof(ICommand), typeof(TitleBox), new PropertyMetadata(null));



        public ICommand MinCommand
        {
            get { return (ICommand)GetValue(MinCommandProperty); }
            set { SetValue(MinCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinCommandProperty =
            DependencyProperty.Register("MinCommand", typeof(ICommand), typeof(TitleBox), new PropertyMetadata(null));




        //判断父窗体是否为最大化，主要用于切换最大化和返回按钮的
        public bool IsParentMax
        {
            get { return (bool)GetValue(IsParentMaxProperty); }
            set { SetValue(IsParentMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsParentMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsParentMaxProperty =
            DependencyProperty.Register("IsParentMax", typeof(bool), typeof(TitleBox), new PropertyMetadata(false));





        public Visibility MinButtonVisibility
        {
            get { return (Visibility)GetValue(MinButtonVisibilityProperty); }
            set { SetValue(MinButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinButtonVisibilityProperty =
            DependencyProperty.Register("MinButtonVisibility", typeof(Visibility), typeof(TitleBox), new PropertyMetadata(Visibility.Visible));



        public Visibility MaxButtonVisibility
        {
            get { return (Visibility )GetValue(MaxButtonVisibilityProperty); }
            set { SetValue(MaxButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MaxButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxButtonVisibilityProperty =
            DependencyProperty.Register("MaxButtonVisibility", typeof(Visibility ), typeof(TitleBox), new PropertyMetadata(Visibility.Visible));



        public Visibility CloseButtonVisibility
        {
            get { return (Visibility)GetValue(CloseButtonVisibilityProperty); }
            set { SetValue(CloseButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseButtonVisibilityProperty =
            DependencyProperty.Register("CloseButtonVisibility", typeof(Visibility), typeof(TitleBox), new PropertyMetadata(Visibility.Visible));


        static TitleBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBox), new FrameworkPropertyMetadata(typeof(TitleBox)));
        }
        public TitleBox()
        {
        }
        public override void OnApplyTemplate()
        {
            Window par = Window.GetWindow(this);
            MaxCommand = new ActionCommand(() =>
            {
                IsParentMax = ((par.WindowState = par.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized) == WindowState.Maximized);
            });
            MinCommand = new ActionCommand(() =>
              {
                  par.WindowState = WindowState.Minimized;
              });
            CloseCommand = new ActionCommand(() =>
              {
                  par.Close();
              });
            base.OnApplyTemplate();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window.GetWindow(this)?.DragMove();
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            if(MaxButtonVisibility== Visibility.Visible)
                MaxCommand.Execute(null);
            base.OnMouseDoubleClick(e);
        }
    }
}
