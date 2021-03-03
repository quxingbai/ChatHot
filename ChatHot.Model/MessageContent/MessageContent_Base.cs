using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChatHot.Model.MessageContent
{
    public class MessageContent_Base : IMessageModelConverter
    {
        public byte[] Content { get; set; }
        public virtual object ToMessageContent(object Arg)
        {
            return Arg;
        }

        /// <summary>
        /// 相对转绝对，条件是第一个字符必须为.
        /// </summary>
        /// <param name="RelativeUri">相对路径</param>
        /// <returns></returns>
        public static String RelativeToAbsolute(String RelativeUri)
        {
            if (RelativeUri == null || RelativeUri.Trim() == "") return "";
            if (RelativeUri[0] != '.') throw new Exception("字符串第一位必须为 .");
            return (Directory.GetCurrentDirectory() + RelativeUri.Substring(1, RelativeUri.Length - 1)).Replace("\\", "/");
        }
        /// <summary>
        /// 是否为相对路径 依据是第一位是否为.
        /// </summary>
        /// <param name="Uri">Uri</param>
        /// <returns></returns>
        public static bool IsRelativeUri(String Uri)
        {
            if (Uri == null || Uri.Trim() == "") return false;
            return Uri[0] == '.';
        }
        public static bool IsAbsolute(String Uri)
        {
            if (Uri == null || Uri.Trim() == "") return false;
            Uri = Uri.Replace('\\', '/');
            return !(Uri[0] == '.')&&Uri.IndexOf('/')!=-1;
        }
    }
}
