using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public class MessageContent_File : MessageContent_Base, ITheMessageContent<byte[]>
    {
        public String FileName { get; set; }

        public byte[] GetContent(Object Param = null)
        {
            return Content;
        }

        public void SetContent(byte[] Source)
        {
            Content = Source;
        }

        public override object ToMessageContent(Object Arg)
        {
            return FileName + "/\nByteSize:"+Content.Length+"";
        }
    }
}
