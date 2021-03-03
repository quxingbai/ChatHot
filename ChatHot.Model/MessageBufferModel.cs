using ChatHot.Model.EFModels.ChatHotDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model
{
    /// <summary>
    /// 消息缓存的实体
    /// </summary>
    public class MessageBufferModel
    {
        public MessageModel Msg { get; set; }
        public User From { get; set; }
        public int MessageCount { get; set; }
        public String CreateDate { get; set; } = DateTime.UtcNow.ToString("HH:mm");
        public String LastMessage { get; set; } = "...";
    }
}
