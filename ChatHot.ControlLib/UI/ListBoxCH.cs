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
    public class ListBoxCH : Selector
    {
        static ListBoxCH()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBoxCH), new FrameworkPropertyMetadata(typeof(ListBoxCH)));
        }
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            var SelectedElement = e.OriginalSource as FrameworkElement;
            base.OnSelectionChanged(e);
        }
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var SelectedElement = e.OriginalSource as FrameworkElement;
            //OnSelectedItem(SelectedElement.TemplatedParent);
            base.OnPreviewMouseLeftButtonDown(e);
        }
        /// <summary>
        /// 设置选择项，需要DataContext,IsSelected属性
        /// </summary>
        /// <param name="Item">被选择的项</param>
        /// <returns>结果</returns>
        public bool OnSelectedItem(Object Item)
        {
            var IsSelectePro = Item.GetType().GetProperty("IsSelected");
            var DataContextPro = Item.GetType().GetProperty("DataContext");
            SelectedItem = DataContextPro.GetValue(Item);
            if (SelectedItem == null) return false;
            IsSelectePro.SetValue(Item, true);
            return true;
        }
    }
}
