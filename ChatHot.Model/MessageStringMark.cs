using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model
{
    //用在返回值类型信息上
    //比如说如果发一个Boolean就发送MessageStringMark.BOOLEAN.True
    public class MessageStringMark
    {
        public enum CodeMessageType
        {
            LoginError_UserOnline = 1,//登陆失败，用户已在线
            LoginSuccess=2,//登陆成功
            Login_OffOnline=3,//将某个用户脱机
        }
        public static Dictionary<CodeMessageType, String> CodeMessages = new Dictionary<CodeMessageType, string>();
        static MessageStringMark()
        {
            AddCodeMessage(CodeMessageType.LoginError_UserOnline, "登陆失败，用户已在线");
            AddCodeMessage(CodeMessageType.LoginSuccess, "登录成功");
            AddCodeMessage(CodeMessageType.Login_OffOnline, "服务器执行了强制离线，有可能是服务器正在关闭");
            void AddCodeMessage(CodeMessageType t,String m)
            {
                CodeMessages.Add(t, m);
            }
        }
        //获取消息码
        public static int GetCodeMsg(CodeMessageType type)
        {
            return (int)type;
        }
        //根据消息码获取消息
        public static String GetCodeMsg (long code)
        {
            return CodeMessages[(CodeMessageType)code];
        }
        public static String GetCodeMsg(String code)
        {
            return CodeMessages[(CodeMessageType)(Convert.ToInt32(code))];
        }

        public class BOOLEAN
        {
            public const String True = "TRUE";
            public const String False = "FALSE";
            public bool Get(String Source)
            {
                return True.Equals(Source);
            }
            public String Get(bool Source)
            {
                return Source ? True : False;
            }
        }
    }
}
