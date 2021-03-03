using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public enum DataPacketType
    {
        ImageDataPacket=1
    }
    /// <summary>
    /// 数据包
    /// </summary>
    public class MessageContent_DataPacket : MessageContent_Base, ITheMessageContent<byte[]>
    {
        public MessageContent_DataPacket() { }
        public MessageContent_DataPacket(byte[] Content,String FileName,bool IsLastPacket,int PacketIndex,DataPacketType PacketType)
        {
            this.Content = Content;
            this.FileName = FileName;
            this.IsLastPacket = IsLastPacket;
            this.PacketIndex = PacketIndex;
            this.PacketType = PacketType;
            //this.FileToken = $"{StaticResource.DateTimeString}-{Content.Length}-{PacketIndex}-{FileName}";
        }
        public byte[] GetContent(object Param = null)
        {
            return Content;
        }

        public void SetContent(byte[] Source)
        {
            Content = Source;
        }
        /// <summary>
        /// 文件令牌
        /// 预留 如果用到加密之类的时候再用
        /// 废弃中
        /// </summary>
        public String FileToken { get; set; }
        /// <summary>
        /// 创建文件时使用此Name
        /// </summary>
        public String FileName { get; set; }
        /// <summary>
        /// 表示是否为最后一个包，如果是的话就可以结束传输了
        /// </summary>
        public bool IsLastPacket { get; set; }
        public int PacketIndex { get; set; }
        public DataPacketType PacketType { get; set; }
        public override object ToMessageContent(object Arg)
        {
            return $"数据包{PacketIndex} Size={(Content == null ? 0 : Content.Length)} Token={FileToken} IsLast={IsLastPacket} FileName={FileName}";
        }
    }
}
