using ChatHot.Model;
using ChatHot.Model.MessageContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media.Imaging;

namespace ChatHot.ControlLib.Tool
{
    public class ImageTool
    {
        public byte[] GetBytes(String FileName)
        {
            FileStream file = File.OpenRead(FileName);
            byte[] bs = new byte[file.Length];
            file.Read(bs, 0, bs.Length);
            return bs;
        }
        public ImageInfo GetImage(String FileName)
        {
            return new ImageInfo()
            {
                ImageBytes = GetBytes(FileName),
                ImagePath = FileName,
            };
        }
        public struct ImageInfo
        {
            public byte[] ImageBytes { get; set; }
            public String ImagePath { get; set; }
        }
        /// <summary>
        /// 通过本地
        /// </summary>
        /// <param name="LoginNumber"></param>
        public static BitmapImage GetUserHeadeImage(long LoginNumber)
        {
            String path = StaticResource.UserHeadImageBasePath + "/" + LoginNumber + ".png";
            try
            {
                return GetByteImage(path);
            }
            catch
            {
                return null;
            }
        }
        public static BitmapImage GetByteImage(String Path)
        {
            if (Path == null) return null;
            BitmapImage img = new BitmapImage();
            IoTool.CreateDirectory(StaticResource.UserHeadImageBasePath);
            if (!IoTool.IsExists(Path))
                throw new Exception("无法找到此文件->" + Path);
            MemoryStream imgMemory = new MemoryStream(IoTool.GetFileByte(Path));
            img.BeginInit();
            img.StreamSource = imgMemory;
            img.EndInit();
            return img;
        }
        /// <summary>
        /// 通过服务器下载最新的用户头像
        /// </summary>
        public static void GetUserHeadImage(long LoginNumber)
        {
            //这里是去获取最新的用户头像
            SocketTool.GetClientUDP().SendTo(new MessageModel(MessageType.Get, LoginNumber, 0, StaticResource.IPV4Address.ToString(), new MessageContent_Get()
            {
                RequestArg = LoginNumber,
                RequestType = Get_Type.File,
                RequestFileType = File_Type.HeadImagoe,
            }), StaticResource.ServerIpaddress);
        }
    }
}
