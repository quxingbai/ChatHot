using ChatHot.Model.EFModels.ChatHotDB;
using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public enum UpdateMessageType
    {
        SubmitToServer=1,//向服务器发送的请求,
        Success=2,//成功了
        Fail=3//失败了
    }
    /// <summary>
    /// 更新某项数据 例如User
    /// </summary>
    public class MessageContent_Update_Base<T> : MessageContent_Base, ITheMessageContent<T>
    {
        public MessageContent_Update_Base()
        {

        }
        public MessageContent_Update_Base(T Source)
        {
            SetContent(Source);
        }
        public UpdateMessageType UpdateMessageType { get; set; }
        public T GetContent(object Param = null)
        {
            return MessageModel.ToModel<T>(Content);
        }

        public void SetContent(T Source)
        {
            Content = MessageModel.ToBytes(Source);
        }
        //携带字节
        public override object ToMessageContent(object Arg)
        {
            return $"更新类型={UpdateMessageType} 大小={Content.Length}";
        }
    }
}
