using ChatHot.Model;
using ChatHot.Model.EFModels.ChatHotDB;
using ChatHot.Model.MessageContent;
using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.ControlLib.Tool
{
    /// <summary>
    /// 表示一个消息/对消息的再封装
    /// </summary>
    public class TheMessageModel
    {
        /// <summary>
        /// 一个字符串消息
        /// </summary>
        public static MessageModel STRING(String Msg, long FromNum, long ToNum)
        {
            return new MessageModel()
            {
                FromIpaddress = SocketTool.IPV4Address.ToString(),
                FromLoginNumber = FromNum,
                ToLoginNumber = ToNum,
                MessageContent = new MessageContent_String(Msg),
                MessageType = MessageType.String,
                SendDateTime = DateTime.UtcNow.ToString(),
            };
        }
        public static String STRING(MessageModel Msg)
        {
            return (Msg.MessageContent as MessageContent_String).GetContent();
        }
        /// <summary>
        /// 一个文件请求
        /// </summary>
        public static MessageContent_Get FILE(long LoginNumber)
        {
            return new MessageContent_Get()
            {
                RequestArg = LoginNumber,
                RequestType = Get_Type.File,
            };
        }
        /// <summary>
        /// 一个图片消息
        /// </summary>
        public static MessageModel IMAGE(String FileName, long FromNum, long ToNum)
        {
            return new MessageModel(MessageType.Image, FromNum, ToNum, new MessageContent_Image(FileName));
        }
        /// <summary>
        /// 一个好友邀请
        /// </summary>
        public static MessageModel MAKEFRIEND(long FromNum, long ToNum, MakeFriendType type)
        {
            return new MessageModel(MessageType.MakeFriend, FromNum, ToNum, new MessageContent_MakeFriend(FromNum, ToNum, type));
        }
        /// <summary>
        /// 一个好友邀请
        /// </summary>
        public static MessageModel MAKEFRIEND(long FromNum, long ToNum, long FromNumFriend, long ToNumFriend, MakeFriendType type)
        {
            return new MessageModel(MessageType.MakeFriend, FromNum, ToNum, new MessageContent_MakeFriend(FromNumFriend, ToNumFriend, type));
        }
        /// <summary>
        /// 一个用户更新
        /// </summary>
        public static MessageModel UPDATE_USER(long ToNum, User UserSource,byte[] HeadImageBytes)
        {
            //throw new Exception("因为在犹豫用一起传还是传完文件再传数据 所以放着");
            return new MessageModel( MessageType.UpdateUser,UserSource.UloginNumber??0, ToNum, new MessageContent_UpdateUser(UserSource,HeadImageBytes));
        }

    }
}
