using ChatHot.Model.TheInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ChatHot.Model.MessageContent
{
    /// <summary>
    /// 并不是携带者内容 而是携带目标的Image
    /// </summary>
    public class MessageContent_Image : MessageContent_Base, ITheMessageContent<String>
    {
        public MessageContent_Image() { }
        /// <summary>
        /// 文件名
        /// </summary>
        /// <param name="FileName">文件名不包括路径</param>
        public MessageContent_Image(String FileName)
        {
            SetContent(FileName);
        }
        public string GetContent(object Param = null)
        {
            return Encoding.UTF8.GetString(Content);
        }

        public void SetContent(string Source)
        {
            Content = MessageModel.ToBytes(Source);
        }

        public override object ToMessageContent(Object Arg)
        {
            try
            {
                String Path = GetContent();
                if (IsRelativeUri(Path))
                {
                    Path = RelativeToAbsolute(Path);
                }
                else if(IsAbsolute(Path))
                {

                }
                //如果是文件名
                else
                {
                    Path = RelativeToAbsolute(StaticResource.MessageImagePath + Path);
                }
                //Image img = new Image()
                //{
                //    Source = new BitmapImage(new Uri(Path)),
                //};
                //return img;
                return Path;
            }
            catch
            {
                return "图片加载失败...";
            }

        }
    }
}
