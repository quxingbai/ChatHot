using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public enum Get_Type
    {
        File=1,
    }
    public enum File_Type
    {
        HeadImagoe=1,
    }
    public class MessageContent_Get : MessageContent_Base, ITheMessageContent<byte[]>
    {
        /// <summary>
        /// 请求文件地址
        /// </summary>
        public Object RequestArg { get; set; }
        public Get_Type RequestType { get; set; }
        /// <summary>
        /// 如果请求类型是File就会以这个判断文件类型
        /// </summary>
        public File_Type RequestFileType { get; set; }
        public byte[] GetContent(object Param = null)
        {
            return Content;
        }

        public void SetContent(byte[] Source)
        {
            Content = Source;
        }
        public override object ToMessageContent(object Arg)
        {
            return $"请求={RequestType} 请求参数={RequestArg} 请求结果大小{(GetContent()??new byte[0]).Length}";
        }
    }
}
