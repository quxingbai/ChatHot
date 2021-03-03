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
    public class ListTextBox : ComboBox
    {


        public ImageSource LeftIcon
        {
            get { return (ImageSource)GetValue(LeftIconProperty); }
            set { SetValue(LeftIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftIconProperty =
            DependencyProperty.Register("LeftIcon", typeof(ImageSource), typeof(ListTextBox), new PropertyMetadata(null));


        static ListTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListTextBox), new FrameworkPropertyMetadata(typeof(ListTextBox)));
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            if(SelectedItem!=null)
            Text = SelectedItem.GetType().GetProperty(SelectedValuePath).GetValue(SelectedItem).ToString(); ;
        }
    }
}
