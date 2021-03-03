using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public class MessageContent_String : MessageContent_Base, ITheMessageContent<String>
    {
        public MessageContent_String(String Source)
        {
            SetContent(Source);
        }
        public string GetContent(Object Param=null)
        {
            //return MessageModel.ToModel<String>(Content);
            return Encoding.UTF8.GetString(Content);
        }

        public void SetContent(string Source)
        {
            Content = MessageModel.ToBytes(Source);
        }
        public override object ToMessageContent(object Arg)
        {
            return (Object)GetContent(Arg);
        }
    }
}
