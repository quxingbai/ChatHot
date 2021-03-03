using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public class MessageContent_Object : MessageContent_Base, ITheMessageContent<Object>
    {
        public MessageContent_Object(String Source)
        {
            SetContent(Source);
        }
        public Object GetContent(Object Param = null)
        {
            return MessageModel.ToModel<Object>(Content);
            //return Encoding.UTF8.GetString(Content);
        }

        public void SetContent(Object Source)
        {
            Content = MessageModel.ToBytes(Source);
        }
        public override object ToMessageContent(object Arg)
        {
            return (Object)GetContent(Arg);
        }
    }
}
