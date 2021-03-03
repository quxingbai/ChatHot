using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public class MessageContent_Request<T> : MessageContent_Base, ITheMessageContent<T>
    {
        public T GetContent(Object Param = null)
        {
            throw new NotImplementedException();
        }

        public void SetContent(T Source)
        {
            throw new NotImplementedException();
        }
        public override object ToMessageContent(object Arg)
        {
            return GetContent(Arg);
        }
    }
}
