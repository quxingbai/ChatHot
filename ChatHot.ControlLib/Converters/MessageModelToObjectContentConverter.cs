using ChatHot.ControlLib.Tool;
using ChatHot.Model;
using ChatHot.Model.MessageContent;
using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace ChatHot.ControlLib.Converters
{
    /// <summary>
    /// 这个转换器可以把 MessageModel 转换为xaml中显示的心事
    /// </summary>
    public class MessageModelToObjectContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            MessageModel Msg = value as MessageModel;
            Object Content = null;
            if(Msg.MessageContent is MessageContent_Base)
            {
                MessageContent_Base MsgContent = Msg.MessageContent as MessageContent_Base;
                Content = MsgContent.GetType().GetMethod("ToMessageContent").Invoke(MsgContent, new object[] { parameter });
            }
            else
            {
                MessageContent_Base MsgContent = MessageModel.ToModel<MessageContent_Base>(Msg.MessageContent);
                Content = MsgContent.GetType().GetMethod("ToMessageContent").Invoke(MsgContent, new object[] { parameter });
                Object o = MsgContent.ToMessageContent(null);
            }
            return Content;
            //MessageContent_Base MsgContent = TheMessageModel.ContentConverter(Msg) as MessageContent_Base;
            //return MsgContent.ToMessageContent(null);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
