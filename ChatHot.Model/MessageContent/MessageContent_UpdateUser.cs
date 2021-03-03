using ChatHot.Model.EFModels.ChatHotDB;
using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public class MessageContent_UpdateUser : MessageContent_Update_Base<User>
    {
        public MessageContent_UpdateUser(User UserSource,Byte[] HeadImageBytes)
        {
            SetContent(UserSource);
            this.HeadImageBytes = HeadImageBytes;
        }
        public byte[] HeadImageBytes { get; set; }
    }
}
