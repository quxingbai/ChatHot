using ChatHot.Model.EFModels.ChatHotDB;
using ChatHot.Model.TheInterface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public enum LoginType
    {
        Submit=1,//如果是进行登录
        Result=2, //如果是进行返回登录结果
        ImOnline=3,//用于告诉服务器客户是否离线的
        OffOnLine=4,//告诉某个用户该脱机了
    }
    public class MessageContent_Login : MessageContent_Base,ITheMessageContent<User>
    {
        public LoginType LoginType { get; set; }
        public String RequestDateTime { get; set; } = DateTime.Now.ToString();
        public String WithMessage { get; set; }//可以用于返回消息或者作为携带消息传到服务器
        byte[] ITheMessageContent<User>.Content { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MessageContent_Login() { }
        public MessageContent_Login(LoginType LoginType, User Content,String WithMessage = "") 
        {
            this.LoginType = LoginType;
            SetContent(Content);
            this.WithMessage = WithMessage;
        }

        public override object ToMessageContent(Object Arg)
        {
            return GetContent().Uname+"\n"+ LoginType.ToString();
        }

        public void SetContent(User Source)
        {
            Content = MessageModel.ToBytes(Source);
        }

        public User GetContent(Object Param = null)
        {
            return MessageModel.ToModel<User>(Content);
        }
    }
}
