using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public enum MakeFriendType
    {
        SendStart=1,//发起
        Success=2,//如果成功了
        Fail=3,//如果被拒绝
        Delete=4,//删除好友
        DeleteSuccess=5,//删除好友成功
    }
    public class MessageContent_MakeFriend : MessageContent_Base, ITheMessageContent<String>
    {
        public long FromNum { get; set; }
        public long ToNum { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public MakeFriendType MakeFriendType { get; set; }
        public MessageContent_MakeFriend(long From, long To,MakeFriendType type)
        {
            SetContent(From + "-" + To);
            this.FromNum = From;
            this.ToNum = To;
            this.MakeFriendType = type;
        }
        public string GetContent(Object Param = null)
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
            return $"{FromNum}提出向{ToNum}的好友消息 类型{MakeFriendType}";
        }
    }
}
