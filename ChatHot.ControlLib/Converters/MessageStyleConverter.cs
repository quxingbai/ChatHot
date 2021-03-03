using ChatHot.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ChatHot.ControlLib.Converters
{
    public class MessageStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            long QQNum = (long)value;
            ResourceDictionary resource = new ResourceDictionary();
            resource.BeginInit();
            resource.Source = new Uri("/ChatHot.ControlLib;component/Themes/QQStyleResource.xaml", UriKind.Relative);
            resource.EndInit();
            //这里可以通过数据库将所有消息 Style 装进数据库，名称、详细信息、Key、需求等等
            //然后再一个数据库装关系 比如说2017922257 -> 1 表示2017922257用的ID为1的消息Style
            return resource["MESSAGE_Style_ContentStyle_1"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
