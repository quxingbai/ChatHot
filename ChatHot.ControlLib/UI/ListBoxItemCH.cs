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
    public class ListBoxItemCH : ListBoxItem
    {
        static ListBoxItemCH()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBoxItemCH), new FrameworkPropertyMetadata(typeof(ListBoxItemCH)));
        }
        public void InitItemAction()
        {
            Selector p = Parent as Selector;
            p.SelectionChanged += (sss, eee) =>
            {
                this.IsSelected = p.SelectedItem == this;
            };
        }
        public override void OnApplyTemplate()
        {
            var a = this;
            var b = a.TemplatedParent;
            var c = a.VisualParent;

            base.OnApplyTemplate();
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }
    }
}
