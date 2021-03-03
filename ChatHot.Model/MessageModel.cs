using ChatHot.Model.MessageContent;
using ChatHot.Model.TheInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model
{
    public enum MessageType
    {
        Image=1,
        String=2,
        File=3,
        Request=4,//例如请求控制电脑等
        Login=5,
        Get=6,//本地文件访问方式
        DataPacket=7,//数据包
        MakeFriend=8,//加好友
        UpdateUser,//更新数据
    }
    public class MessageModel
    {
        public MessageType MessageType { get; set; }
        public long FromLoginNumber { get; set; }
        public long ToLoginNumber { get; set; }
        public String FromIpaddress { get; set; }
        public Object MessageContent { get; set; }
        public String SendDateTime { get; set; } = DateTime.Now.ToLongDateString();

        public MessageModel() { }
        public MessageModel(MessageType MsgType,long FromNum,long ToNum,String FromIP, Object Content)
        {
            this.MessageType = MsgType;
            this.FromLoginNumber = FromNum;
            this.ToLoginNumber = ToNum;
            this.MessageContent = Content;
            this.FromIpaddress = FromIP;
        }
        public MessageModel(MessageType MsgType, long FromNum, long ToNum, MessageContent_Base Content)
        {
            this.MessageType = MsgType;
            this.FromLoginNumber = FromNum;
            this.ToLoginNumber = ToNum;
            this.MessageContent = Content;
            this.FromIpaddress = StaticResource.IPV4Address.ToString();
        }
        public static T ToModel<T>(Byte[] Source)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(Source));
        }
        public static byte[] ToBytes(Object Source)
        {
            if(Source is string)
            {
                return Encoding.UTF8.GetBytes(Source.ToString());
            }
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Source));
        }
        public static T ToModel<T>(Object Source)
        {
            return ToModel<T>(ToBytes(Source));
        }
    }
}
