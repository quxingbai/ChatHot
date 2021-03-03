using ChatHot.ControlLib.Form;
using ChatHot.ControlLib.Tool;
using ChatHot.Model;
using ChatHot.Model.EFModels.ChatHotDB;
using ChatHot.Model.MessageContent;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatHot.Server
{
    public partial class MainWindow : ChatFormBasic
    {

        private SocketTool SocketServer = SocketTool.GetServerUDP();
        private SocketTool SocketServerTCP = SocketTool.GetServerTCP();
        private Dictionary<long, UserInfo> OnlineUsers = new Dictionary<long, UserInfo>();
        private Dictionary<long, DateTime> OnlineUsersDateTime = new Dictionary<long, DateTime>();//储存用户的最后一次发送在线判断时间
        private Dictionary<String, WritingFile> WritingFiles = new Dictionary<string, WritingFile>();//正在写入数据的文件
        private ChatHotContext DB = new ChatHotContext();
        private Thread OnlineUsersCheckedThread = null;
        private const int OnlineUsersCheckedTime = 20;//以秒为单位
        private const int OnlineUsersTimeoutTime = 60;//以秒为单位

        public MainWindow()
        {
            InitializeComponent();
            ThreadPool.SetMaxThreads(15, 15);
            SocketServer.ErrorCallBack = eee =>
            {
                try
                {
                    throw eee;
                }
                catch
                {

                }
            };
            SocketServer.GetNewMessage += SocketServer_GetNewMessage;
            SocketServerTCP.GetNewMessage += SocketServer_GetNewMessage;
            OnlineUsersCheckedThread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(OnlineUsersCheckedTime * 1000);
                    foreach (long k in OnlineUsersDateTime.Keys)
                    {
                        DateTime t = new DateTime(DateTime.Now.Ticks - OnlineUsersDateTime[k].Ticks);
                        if (t.Ticks > OnlineUsersTimeoutTime * 10000000)
                        {
                            RemoveOnlineUser(k);
                        }
                    }
                }
            });
            OnlineUsersCheckedThread.Start();
        }

        private void SocketServerTCP_GetNewMessage(MessageModel Msg, EndPoint FromIP)
        {
            if (Msg.MessageType == MessageType.DataPacket)
            {
                MessageContent_DataPacket packet = MessageModel.ToModel<MessageContent_DataPacket>(Msg.MessageContent);
                if (packet.PacketIndex == 1)
                {
                    WritingFile file = new WritingFile(StaticResource.MessageImagePath + packet.FileName);
                    WritingFiles.Add(packet.FileName, file);
                }
                WritingFiles[packet.FileName].Writer(packet.Content);
            }
        }

        private void SocketServer_GetNewMessage(MessageModel Msg, EndPoint From)
        {
            Object b = Msg.MessageContent;
            if (Msg.MessageType == MessageType.Login)
            {
                MessageContent_Login info = MessageModel.ToModel<MessageContent_Login>(Msg.MessageContent);
                //如果是登录消息
                if (info.LoginType == LoginType.Submit)
                {
                    //这里判断是否登录成功，返回一个Result类型的Login信息
                    if (IsOnline(info.GetContent().UloginNumber ?? 0))
                    {
                        SocketServer.SendTo(new MessageModel(MessageType.Login, 0, info.GetContent().UloginNumber ?? 0, SocketTool.IPV4Address.ToString(), new MessageContent_Login(LoginType.Result, info.GetContent(), MessageStringMark.GetCodeMsg(MessageStringMark.CodeMessageType.LoginError_UserOnline).ToString())), From);
                        return;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        AddOnlineUser(new UserInfo()
                        {
                            UserIP = From,
                            User = info.GetContent(),
                        });
                        SocketServer.SendTo(new MessageModel(MessageType.Login, 0, info.GetContent().UloginNumber ?? 0, SocketTool.IPV4Address.ToString(), new MessageContent_Login(LoginType.Result, info.GetContent(), MessageStringMark.GetCodeMsg(MessageStringMark.CodeMessageType.LoginSuccess).ToString())), From);
                    });
                }
                //如果是在线确认消息
                else if (info.LoginType == LoginType.ImOnline)
                {
                    long clientNum = info.GetContent().UloginNumber ?? 0;
                    //如果已存在
                    if (OnlineUsersDateTime.Keys.Where(k => k == clientNum).Count() > 0)
                    {
                        OnlineUsersDateTime[clientNum] = DateTime.Now;
                    }
                    else
                    {
                        //如果不存在就表明已经被下线了
                        // OnlineUsersDateTime.Add(clientNum, DateTime.Now);
                    }
                }
            }
            else
            {
                //!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!
                //这里的消息需要手动添加
                //不然无法显示出来
                MessageContent_Base content_base = MessageModel.ToModel<MessageContent_Base>(Msg.MessageContent);
                //如果是文字消息
                if (Msg.MessageType == MessageType.String)
                {
                    MessageContent_String info = MessageModel.ToModel<MessageContent_String>(Msg.MessageContent);
                    content_base = info;
                    MessageRouter();
                }
                else if (Msg.MessageType == MessageType.Get)
                {
                    MessageContent_Get m = MessageModel.ToModel<MessageContent_Get>(Msg.MessageContent);
                    content_base = m;
                    long lnum = long.Parse(m.RequestArg.ToString());
                    if (m.RequestType == Get_Type.File)
                    {
                        if (m.RequestFileType == File_Type.HeadImagoe)
                        {
                            User u = DB.Users.Where(u => u.UloginNumber == lnum).First();
                            m.RequestArg = u.UheadImage;
                        }
                        if (IoTool.IsExists(m.RequestArg.ToString()))
                        {
                            m.Content = IoTool.GetFileByte(m.RequestArg.ToString());
                        }
                        else
                        {
                            m.Content = IoTool.GetFileByte(StaticResource.DefaultUserHeadImageBasePath);
                        }
                        m.RequestArg = lnum.ToString();
                    }
                    SocketServer.SendTo(new MessageModel(MessageType.Get, 0, Msg.FromLoginNumber, StaticResource.IPV4Address.ToString(),
                     m), From);
                }
                else if (Msg.MessageType == MessageType.DataPacket)
                {
                    MessageContent_DataPacket data = MessageModel.ToModel<MessageContent_DataPacket>(Msg.MessageContent);
                    content_base = data;
                    MessageRouter();
                }
                else if (Msg.MessageType == MessageType.Image)
                {
                    MessageContent_Image img = MessageModel.ToModel<MessageContent_Image>(Msg.MessageContent);
                    content_base = img;
                    MessageRouter();
                }
                //如果是加好友
                else if (Msg.MessageType == MessageType.MakeFriend)
                {
                    MessageContent_MakeFriend friend = MessageModel.ToModel<MessageContent_MakeFriend>(Msg.MessageContent);
                    content_base = friend;
                    if (friend.MakeFriendType == MakeFriendType.SendStart)
                    {
                        MessageRouter();
                    }
                    else if (friend.MakeFriendType == MakeFriendType.Success)
                    {
                        DB.Friends.Add(new Friend()
                        {
                            AcceptUser = friend.ToNum,
                            SendUser = friend.FromNum,
                            CreateDate = friend.CreateDate,
                            LockDate = DateTime.UtcNow,
                        });
                        if (DB.SaveChanges() > 0)
                        {
                            MessageRouter();
                        }
                    }
                    //如果是删好友
                    else if (friend.MakeFriendType == MakeFriendType.Delete)
                    {
                        DB.Entry(DB.Friends.Where(f => f.SendUser == friend.FromNum && f.AcceptUser == friend.ToNum || f.SendUser == friend.ToNum & f.AcceptUser == friend.FromNum).First()).State = EntityState.Deleted;
                        //如果数据库删除成功
                        if (DB.SaveChanges() > 0)
                        {
                            //把消息给两方发过去
                            friend.MakeFriendType = MakeFriendType.DeleteSuccess;
                            Msg.MessageContent = friend;
                            MessageRouter();
                            MessageRouterToFrom();
                        }
                    }
                }
                else if (Msg.MessageType == MessageType.UpdateUser)
                {
                    MessageContent_UpdateUser content = MessageModel.ToModel<MessageContent_UpdateUser>(Msg.MessageContent);
                    Msg.MessageContent = content;
                    User ContentUser = content.GetContent();
                    User EFuser = DB.Users.Where(u => u.Uid == ContentUser.Uid).First();
                    String HeadImageSavePath = null;
                    //如果上传头像了
                    if (content.HeadImageBytes.Length != 0)
                    {
                        //保存图片 把地址放到数据库
                        HeadImageSavePath = StaticResource.UserHeadImageBasePath + ContentUser.UloginNumber + ".png";
                        IoTool.SaveFile(HeadImageSavePath, content.HeadImageBytes);
                        EFuser.UheadImage = HeadImageSavePath;
                    }

                    EFuser.Uname = ContentUser.Uname;
                    EFuser.Upassword = ContentUser.Upassword;
                    EFuser.Utag = ContentUser.Utag;

                    DB.Update(EFuser);
                    if (DB.SaveChanges() > 0)
                    {
                        //成功后再把修改后的User发送回去
                        //如果想要所有人都实时改变就再判断一下分别向好友发送更改后的User
                        content.SetContent(EFuser);
                        Msg.MessageContent = content;
                        MessageRouterToFrom();
                    }

                }


                //将当前消息进行统一处理
                void MessageRouter(MessageContent_Base c = null)
                {
                    if (c != null)
                    {
                        Msg.MessageContent = c;
                    }
                    //如果目标在线
                    if (IsOnline(Msg.ToLoginNumber))
                    {
                        UserInfo UserTo = OnlineUsers[Msg.ToLoginNumber];
                        SocketTool.GetServerUDP().SendTo(Msg, UserTo.UserIP);
                    }
                    //如果目标不在线
                    //输入到数据库------------->
                    else
                    {
                    }
                }
                //将消息返回给发送者
                void MessageRouterToFrom()
                {
                    //如果目标在线
                    if (IsOnline(Msg.FromLoginNumber))
                    {
                        UserInfo UserTo = OnlineUsers[Msg.FromLoginNumber];
                        SocketTool.GetServerUDP().SendTo(Msg, UserTo.UserIP);
                    }
                    //如果目标不在线
                    //输入到数据库------------->
                    else
                    {
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    Msg.MessageContent = content_base;
                    DisplayMessageModel msg = new DisplayMessageModel()
                    {
                        IsFrom = true,
                        Msg = Msg,
                    };
                    LIST_MessageLogs.Items.Add(msg);
                    LIST_MessageLogs.ScrollIntoView(msg);
                });
            }
        }
        /// <summary>
        /// 将消息添加入队列，
        /// </summary>
        /// <param name="Msg">消息</param>
        public void AppendMessageQueue(MessageModel Msg)
        {

        }
        /// <summary>
        /// 添加一个在线用户
        /// </summary>
        public void AddOnlineUser(UserInfo user)
        {
            Dispatcher.Invoke(() =>
            {
                OnlineUsers.Add(user.User.UloginNumber ?? 0, user);
                OnlineUsersDateTime.Add(user.User.UloginNumber ?? 0, DateTime.Now);
                LIST_OnlineUsers.Items.Add(user.User);
                LIST_OnLineUsers.Items.Add(user);
                TABI_OnLineUsers.Header = "在线用户-" + OnlineUsers.Count;
            });
        }
        /// <summary>
        /// 移除一个在线用户
        /// </summary>
        public void RemoveOnlineUser(long LoginNumber)
        {
            Dispatcher.Invoke(() =>
            {
                UserInfo ui = OnlineUsers[LoginNumber];
                User u = ui.User;
                OffOnlineUser(u.UloginNumber??0);
                OnlineUsers.Remove(LoginNumber);
                if (OnlineUsersDateTime.Keys.ToArray().Where(k => k == LoginNumber).Count() > 0)
                    OnlineUsersDateTime.Remove(LoginNumber);
                LIST_OnlineUsers.Items.Remove(u);
                LIST_OnLineUsers.Items.Remove(ui);
                TABI_OnLineUsers.Header = "在线用户-" + OnlineUsers.Count;
            });
        }
        /// <summary>
        /// 使用户脱机
        /// </summary>
        public bool OffOnlineUser(long LoginNumber)
        {
            try
            {
                if (OnlineUsers.Keys.Where(k => k == LoginNumber).Count() > 0)
                {
                    UserInfo ui = OnlineUsers[LoginNumber];
                    //发一条关于被强制退出的消息
                    SocketServer.SendTo(new MessageModel(MessageType.Login, 0, ui.User.UloginNumber ?? 0, StaticResource.IPV4Address.ToString(), new MessageContent_Login(LoginType.OffOnLine, ui.User, MessageStringMark.GetCodeMsg(MessageStringMark.CodeMessageType.Login_OffOnline).ToString())), ui.UserIP);
                    RemoveOnlineUser(ui.User.UloginNumber ?? 0);
                    return true;
                }
            }
            catch
            {

            }
            return false;
        }
        /// <summary>
        /// 判断此用户是否在线
        /// </summary>
        public bool IsOnline(long LoginNumber)
        {
            return OnlineUsers.Where(u => u.Key == LoginNumber).Count() > 0;
        }
        protected override void OnClosed(EventArgs e)
        {
            foreach (UserInfo u in OnlineUsers.Values)
            {
                OffOnlineUser(u.User.UloginNumber ?? 0);
            }
            try
            {
                SocketServer.Dispose();
                OnlineUsersCheckedThread?.Abort();
            }
            catch
            {

            }
            OnCloseALL();
            base.OnClosed(e);
        }

        private void MENUITEM_OnLineUser_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            UserInfo by = LIST_OnLineUsers.SelectedItem as UserInfo;

            switch (item.Tag.ToString().ToUpper())
            {
                case "OFFONLINE": OffOnlineUser(by.User.UloginNumber ?? 0); break;
            }
        }
    }
}
