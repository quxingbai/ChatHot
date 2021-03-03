using ChatHot.Model;
using ChatHot.Model.MessageContent;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatHot.ControlLib.Tool
{
    public delegate void ByteMsg(MessageModel Msg, EndPoint FromIP);
    public class SocketTool : IDisposable
    {
        private Thread GetMessageThread = null;
        private Thread GetClientThread = null;

        private static SocketTool _ClientUDP = null;
        private static SocketTool _ServerUDP = null;
        private static SocketTool _ClientTCP = null;
        private static SocketTool _ServerTCP = null;
        /// <summary>
        /// 客户端/如果Get就会发起Socket地址申请
        /// </summary>
        public static SocketTool GetClientUDP()
        {
            try
            {
                if (_ClientUDP == null) _ClientUDP = new SocketTool(StaticResource.ClientIpaddress, SocketType.Dgram, ProtocolType.Udp);
            }
            catch
            {
                _ClientUDP = new SocketTool(StaticResource.ClientIpaddressSpare, SocketType.Dgram, ProtocolType.Udp);
            }
            return _ClientUDP;
        }
        /// <summary>
        /// 服务端/如果Get就会发起Socket地址申请
        /// </summary>
        public static SocketTool GetServerUDP()
        {
            if (_ServerUDP == null) _ServerUDP = new SocketTool(StaticResource.ServerIpaddress, SocketType.Dgram, ProtocolType.Udp);
            return _ServerUDP;
        }
        /// <summary>
        /// 别用 尽量用下边的TCPSendMsg
        /// </summary>
        /// <returns></returns>
        public static SocketTool GetClientTCP()
        {
            //try
            //{
            //    if (_ClientTCP == null) _ClientTCP = new SocketTool(StaticResource.ClientIpaddress, SocketType.Stream, ProtocolType.Tcp);
            //}
            //catch
            //{
            //    _ClientTCP = new SocketTool(StaticResource.ClientIpaddressSpare, SocketType.Stream, ProtocolType.Tcp);
            //}
            //return _ClientTCP;
            return _ClientTCP = new SocketTool(StaticResource.ClientIpaddress, SocketType.Stream, ProtocolType.Tcp);
        }

        public static SocketTool GetServerTCP()
        {
            if (_ServerTCP == null) _ServerTCP = new SocketTool(StaticResource.ServerIpaddress, SocketType.Stream, ProtocolType.Tcp);
            _ServerTCP.Listen();
            return _ServerTCP;
        }

        //错误回调
        public Action<Exception> ErrorCallBack;
        public event ByteMsg GetNewMessage;
        public event Action<Socket> GetNewClient;
        public const long DefaultMaxMsgSize = 45000;
        public const long DefaultMaxDataPacketSize = DefaultMaxMsgSize - 20000;
        public const int MaxTCPLinkCount = 10;
        //如果服务器遇到错误就重新开启的最大次数
        public static int MaxRestartCount = 10;
        //获取IPV4
        public static IPAddress IPV4Address
        {
            get
            {
                return Dns.GetHostAddresses(Dns.GetHostName()).Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).Last();
            }
        }
        private Socket Server = new Socket(SocketType.Dgram, ProtocolType.Udp);
        public SocketTool(String Address, int Port, long MaxMsgSize)
        {
            IPAddress address = IPAddress.Parse(Address);
            Server.Bind(new IPEndPoint(address, Port));
            StartGetMessage(MaxMsgSize);
        }
        public SocketTool(int Port)
        {
            Server.Bind(new IPEndPoint(IPV4Address, Port));
            StartGetMessage(DefaultMaxMsgSize);
        }
        public SocketTool(EndPoint Address, long MaxMsgSize)
        {
            Server.Bind(Address);
            StartGetMessage(MaxMsgSize);
        }
        public SocketTool(EndPoint Address, SocketType SocketType, ProtocolType ProtocolType)
        {
            Server = new Socket(SocketType, ProtocolType);
            Server.Bind(Address);
            if (ProtocolType == ProtocolType.Udp)
            {
                StartGetMessage(DefaultMaxMsgSize);

            }
            else if (ProtocolType == ProtocolType.Tcp)
            {
            }
        }
        private void StartGetMessage(long MaxMsgSize)
        {
            GetMessageThread = new Thread(() =>
           {

               EndPoint From = new IPEndPoint(IPAddress.Any, 0);
               Byte[] bs = new byte[MaxMsgSize];
               int Blen = 0;

               try
               {
                   while ((Blen = Server.ReceiveFrom(bs, ref From)) > 0)
                   {
                       MessageModel m = MessageModel.ToModel<MessageModel>(new MemoryStream(bs, 0, Blen).ToArray());
                       GetNewMessage?.Invoke(m, From);
                       bs = new byte[MaxMsgSize];
                   }
               }
               catch (Exception eee)
               {
                   if((--MaxRestartCount)>0)
                   {
                       StartGetMessage(MaxMsgSize);
                   }
                   else
                   {
                       ErrorCallBack?.Invoke(eee);
                   }
               }
           });
            GetMessageThread.Start();
        }
        private void StartGetMessage(long MaxMsgSize, Socket Target)
        {
            GetMessageThread = new Thread(() =>
            {

                EndPoint From = new IPEndPoint(IPAddress.Any, 0);
                Byte[] bs = new byte[MaxMsgSize];
                int Blen = 0;

                try
                {
                    while ((Blen = Target.ReceiveFrom(bs, ref From)) > 0)
                    {
                        MessageModel m = MessageModel.ToModel<MessageModel>(new MemoryStream(bs, 0, Blen).ToArray());
                        GetNewMessage?.Invoke(m, From);
                        bs = new byte[MaxMsgSize];
                    }
                }
                catch (Exception eee)
                {
                    ErrorCallBack?.Invoke(eee);
                }
            });
            GetMessageThread.Start();
        }
        private void StartGetClient()
        {
            Server.Listen(MaxTCPLinkCount);
            GetClientThread = new Thread(() =>
            {
                while (true)
                {
                    Socket Client = Server.Accept();
                    GetNewClient?.Invoke(Client);
                    StartGetMessage(DefaultMaxMsgSize, Client);
                }
            });
            GetClientThread.Start();

        }
        /// <summary>
        /// 是否需要获取用户，如果获取用户那就不能连接
        /// </summary>
        public void Listen()
        {
            StartGetClient();
        }
        public int SendTo(MessageModel Source, EndPoint TargetIP)
        {
            return Server.SendTo(MessageModel.ToBytes(Source), TargetIP);
        }
        public int SendToServer(MessageModel Source)
        {
            return SendTo(Source, StaticResource.ServerIpaddress);
        }
        public int Send(MessageModel Source, EndPoint TargetIP, bool IsCloseConnect)
        {
            Server.Connect(TargetIP);
            int res = Server.Send(MessageModel.ToBytes(Source));
            Server.Close();
            return res;
        }
        public static bool SendFile(String FileName, EndPoint TargetIP)
        {
            Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            client.Connect(TargetIP);
            IoTool.GetFileByteBlockALL(FileName, (int)DefaultMaxDataPacketSize, (bs, str, boo, inti) =>
            {
                client.Send(bs);
                if (boo)
                {
                    client.Close();
                    client.Dispose();
                }
            });
            return true;
        }
        private static Socket SendFrom = null;
        public static int TCPSendMsg(MessageModel msg, bool IsClose, EndPoint To)
        {
            if (SendFrom == null)
            {
                SendFrom = new Socket(SocketType.Stream, ProtocolType.Tcp);
                SendFrom.Connect(To);
            }
            int state = SendFrom.Send(MessageModel.ToBytes(msg));
            if (IsClose)
            {
                SendFrom.Close();
                SendFrom.Dispose();
                SendFrom = null;
            }
            return state;
        }
        /// <summary>
        /// 尝试让服务器转发一个文件，
        /// </summary>
        /// <param name="FileName">文件地址</param>
        /// <param name="FromNum">来自</param>
        /// <param name="ToNum">去往</param>
        /// <param name="PacketType">数据包类型</param>
        /// <param name="PrintFileName">输出后的文件名称</param>
        /// <returns>传过去的大小</returns>
        public static long TCPSendFile(String FileName, long FromNum, long ToNum, DataPacketType PacketType,out String PrintFileName)
        {
            Socket SocketBuffer = new Socket(SocketType.Stream, ProtocolType.Tcp);
            SocketBuffer.Connect(StaticResource.ServerIpaddress);
            long res = 0;
            String DateFileName = DateTime.Now.Ticks+ IoTool.GetFileType(FileName);
            //String DateFileName = StaticResource.DateTimeLoginNumber(FromNum) + IoTool.GetFileType(FileName);
            IoTool.GetFileByteBlockALL(FileName, (int)DefaultMaxDataPacketSize, (content, filenmae, islast, count) =>
            {
                MessageModel msg = new MessageModel()
                {
                    FromIpaddress = IPV4Address.ToString(),
                    FromLoginNumber = FromNum,
                    ToLoginNumber = ToNum,
                    MessageType = MessageType.DataPacket,
                    SendDateTime = DateTime.UtcNow.ToString(),
                    MessageContent = new MessageContent_DataPacket(content, DateFileName, islast, count, PacketType),
                };
                res += SocketBuffer.Send(MessageModel.ToBytes(msg));
                Thread.Sleep(50);
            });
            SocketBuffer.Close();
            SocketBuffer.Dispose();
            PrintFileName = DateFileName;
            return res;
        }
        public void Dispose()
        {
            try
            {
                Server.Dispose();
                GetMessageThread.Abort();
                GetClientThread.Abort();
            }
            catch
            {

            }
        }
    }
}
