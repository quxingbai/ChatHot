using ChatHot.Model.EFModels.ChatHotDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatHot.Model
{
    public delegate void SocketFileSuccess(String Path,String Token ,byte[] bs);
    public class StaticResource
    {      
        public StaticResource()
        {
        }
        //获取IPV4
        public static IPAddress IPV4Address
        {
            get
            {
                return Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).Last();
            }
        }
        public static readonly IPEndPoint ServerIpaddress = IPEndPoint.Parse("192.168.0.111:1857");
        //public static readonly IPEndPoint ServerIpaddress = IPEndPoint.Parse(SocketUDP.IPV4Address.ToString() + ":1857");
        public static readonly IPEndPoint ClientIpaddress = IPEndPoint.Parse(IPV4Address.ToString() + ":1858");
        public static readonly IPEndPoint ClientIpaddressSpare = IPEndPoint.Parse(IPV4Address.ToString() + ":1860");//备用端口号
        public static readonly String XmlPath = "./XmlDatas/";
        public static readonly String XmlLoginLog = XmlPath + "LoginLog.xml";
        public static readonly String UserHeadImageBasePath = "./Image/UserHeader/";
        public static readonly String MessageImagePath = "./Image/MessageImage/";
        /// <summary>
        /// 默认用户头像
        /// </summary>
        public static readonly String DefaultUserHeadImageBasePath = @"G:\有用的项目\ChatHot\ChatHot\ChatHot.StaticResource\Icon\HeadImg.jpg";
        /// <summary>
        /// 全局用户
        /// </summary>
        public static User ALLUserSource { get; set; }
        public static String DateTimeString { get => DateTime.UtcNow.ToString("yyyyMMddHHmmss"); }
        public static String DateTimeLoginNumber(long Num)
        {
            return DateTimeString + Num;
        }
        /// <summary>
        /// 从XML文件中获取一个文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static String ToXMLPath(String FileName)
        {
            return XmlPath + FileName;
        }
        /// <summary>
        /// 将Filename转换为ImagePath
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static String ToUserHeadImagePath(String FileName)
        {
            return UserHeadImageBasePath + FileName;
        }

        /// <summary>
        /// 内存缓存
        /// </summary>
        public class MemoryBuffer
        {
            /// <summary>
            /// 用户头像缓存
            /// </summary>
            public BitmapImage UserHeaderImage { get; set; } = null;

        }

        public static event Action<ImageSource, long> HeadImageSourceChanged;
        private static Dictionary<long, ImageSource> headImageSource { get; set; } = new Dictionary<long, ImageSource>();
        public static Dictionary<long, ImageSource> HeadImageSource { get => headImageSource; }
        public static void SetHeadImageSource( long Loginnum, ImageSource img)
        {
            HeadImageSourceChanged?.Invoke(img, Loginnum);
        }

    }
}
