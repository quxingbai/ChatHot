using ChatHot.ControlLib;
using ChatHot.ControlLib.Converters;
using ChatHot.ControlLib.Form;
using ChatHot.ControlLib.Tool;
using ChatHot.ControlLib.UI;
using ChatHot.Model;
using ChatHot.Model.EFModels.ChatHotDB;
using ChatHot.Model.MessageContent;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatHot
{
    /// <summary>
    /// ChatForm.xaml 的交互逻辑
    /// </summary>
    public partial class ChatForm : ChatFormBasic
    {



        public ICommand RegisterMessageTextBox
        {
            get { return (ICommand)GetValue(RegisterMessageTextBoxProperty); }
            set { SetValue(RegisterMessageTextBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RegisterMessageTextBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegisterMessageTextBoxProperty =
            DependencyProperty.Register("RegisterMessageTextBox", typeof(ICommand), typeof(ChatForm), new PropertyMetadata(null));



        public ICommand RegisterMessageListBox
        {
            get { return (ICommand)GetValue(RegisterMessageListBoxProperty); }
            set { SetValue(RegisterMessageListBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RegisterMessageListBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RegisterMessageListBoxProperty =
            DependencyProperty.Register("RegisterMessageListBox", typeof(ICommand), typeof(ChatForm), new PropertyMetadata(null));


        public User UserSource
        {
            get { return (User)GetValue(UserSourceProperty); }
            set { SetValue(UserSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserSourceProperty =
            DependencyProperty.Register("UserSource", typeof(User), typeof(ChatForm), new PropertyMetadata(null));

        //这个作用是表示数据包正在写的文件 FileName 为Key
        private Dictionary<String, WritingFile> DataPacketsFile = new Dictionary<string, WritingFile>();
        /// <summary>
        /// 登录号与对应的Tabitem
        /// </summary>
        public Dictionary<long, TabItem> ChatTabItems = new Dictionary<long, TabItem>();


        public static ChatForm MainChatForm = null;
        public ChatForm()
        {
            InitializeComponent();
        }

        public ChatForm(User u)
        {
            InitializeComponent();
            this.UserSource = u;
        }
        public void OnInitMessageRouter()
        {
            SocketTool.GetClientUDP().GetNewMessage += ClientSocket_GetNewMessage_ChatForm;
        }
        /// <summary>
        /// 设置某个用户的聊天焦点
        /// </summary>
        public void SetUserFocus(long item)
        {
            this.Focus();
            TAB_ChatUsers.SelectedItem = ChatTabItems[item];
        }
        /// <summary>
        /// 添加一个ChatUser
        /// </summary>
        public void AddChatUser(User u)
        {
            ChatPanel chatPanel = new ChatPanel(UserSource, u);
            TabItem item = new TabItem() { DataContext = u, Content = chatPanel };
            TAB_ChatUsers.Items.Add(item);
            ChatTabItems.Add(u.UloginNumber ?? 0, item);
            SetUserFocus(u.UloginNumber ?? 0);
            if (TAB_ChatUsers.Items.Count == 1)
            {
                base.Show();
            }
            chatPanel.CloseCallBack = () =>
            {
                RemoveChatUser(u);
            };

        }
        /// <summary>
        /// 移除一个ChatUser中的一个
        /// </summary>
        public void RemoveChatUser(User u)
        {
            for (int f = 0; f < MainChatForm.TAB_ChatUsers.Items.Count; f++)
            {
                User tu = (MainChatForm.TAB_ChatUsers.Items[f] as TabItem).DataContext as User;
                if (u.UloginNumber == tu.UloginNumber)
                {
                    MainChatForm.TAB_ChatUsers.Items.Remove(MainChatForm.TAB_ChatUsers.Items[f]);
                    ChatTabItems.Remove(u.UloginNumber ?? 0);
                }
            }
            //如果>0就证明还用ChatUser 如果<0就直接Hide窗体
            if (MainChatForm.TAB_ChatUsers.Items.Count == 0)
            {
                MainChatForm.Hide();
            }
        }

        /// <summary>
        /// 判断该用户是否存在于聊天
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public bool HasChatUser(long LoginNumber)
        {
            bool Result = true;
            Dispatcher.Invoke(() =>
            {
                Result = MainChatForm == null;
            });
            if (Result) return false;
            Dispatcher.Invoke(() =>
            {
                foreach (var item in MainChatForm.TAB_ChatUsers.Items)
                {
                    User tu = (item as TabItem).DataContext as User;
                    if (tu.UloginNumber == LoginNumber)
                    {
                        Result = true;
                        break;
                    }
                }
            });
            return Result;
        }
        /// <summary>
        /// 从点开聊天窗体的用户中查找一个
        /// </summary>
        public User GetUserByChatUsersItem(long LoginNumber)
        {
            foreach (var item in MainChatForm.TAB_ChatUsers.Items)
            {
                User Target = (item as TabItem).DataContext as User;
                if (Target.UloginNumber == LoginNumber)
                {
                    return Target;
                }
            }
            return null;
        }
        //------------------消息处理-----------------
        private void ClientSocket_GetNewMessage_ChatForm(MessageModel Msg, System.Net.EndPoint From)
        {
            if (Msg.MessageType == MessageType.String)
            {
                MessageContent_String c = MessageModel.ToModel<MessageContent_String>(Msg.MessageContent);
                Msg.MessageContent = c;
                PrintMessageRouter();
            }
            else if (Msg.MessageType == MessageType.Get)
            {
                MessageContent_Get get = MessageModel.ToModel<MessageContent_Get>(Msg.MessageContent);
                if (get.RequestType == Get_Type.File)
                {
                    if (get.RequestFileType == File_Type.HeadImagoe)
                    {
                        IoTool.CreateDirectory(StaticResource.UserHeadImageBasePath);
                        String headPath = StaticResource.ToUserHeadImagePath(get.RequestArg + ".png");
                        IoTool.SaveFile(headPath, get.Content);
                        Dispatcher.Invoke(() =>
                        {
                            StaticResource.SetHeadImageSource(long.Parse(get.RequestArg.ToString()), ImageTool.GetUserHeadeImage(long.Parse(get.RequestArg.ToString())));
                        });
                    }
                }
            }
            else if (Msg.MessageType == MessageType.Login)
            {
                MessageContent_Login login = MessageModel.ToModel<MessageContent_Login>(Msg.MessageContent);
                if (login.LoginType == LoginType.OffOnLine)
                {
                    MessageBox.Show(MessageStringMark.GetCodeMsg(login.WithMessage));
                    Dispatcher.Invoke(() =>
                    {
                        OnCloseALL();
                    });
                }
            }
            else if (Msg.MessageType == MessageType.DataPacket)
            {
                MessageContent_DataPacket packet = MessageModel.ToModel<MessageContent_DataPacket>(Msg.MessageContent);
                //如果是图片的数据
                if (packet.PacketType == DataPacketType.ImageDataPacket)
                {
                    String fname = packet.FileName;
                    //如果是第一个包
                    if (packet.PacketIndex == 1)
                    {
                        DataPacketsFile.Add(fname, new WritingFile(StaticResource.MessageImagePath + fname)
                        {
                            Tag = packet.FileToken//这条实在没什么用
                        });
                    }
                    DataPacketsFile[fname].Writer(packet.Content);
                    if (packet.IsLastPacket)
                    {
                        DataPacketsFile[fname].WriterEnd();
                    }
                }
            }
            else if (Msg.MessageType == MessageType.Image)
            {
                MessageContent_Image img = MessageModel.ToModel<MessageContent_Image>(Msg.MessageContent);
                Msg.MessageContent = img;
                PrintMessageRouter();
            }
            else if(Msg.MessageType== MessageType.MakeFriend)
            {
                ChatHotContext DB = new ChatHotContext();

                MessageContent_MakeFriend content = MessageModel.ToModel<MessageContent_MakeFriend>(Msg.MessageContent);
                Msg.MessageContent = content;
                if (content.MakeFriendType == MakeFriendType.SendStart)
                {
                    if (MessageBox.Show("好友请求", "来自" + content, MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
                    {
                        //这个是返回消息所以是反着来的
                        //SocketTool.GetClientUDP().SendToServer(TheMessageModel.MAKEFRIEND(content.ToNum, content.FromNum, content.FromNum, content.ToNum, MakeFriendType.Success));
                        Msg.FromIpaddress = StaticResource.IPV4Address.ToString();
                        Msg.FromLoginNumber = content.ToNum;
                        Msg.ToLoginNumber = content.FromNum;
                        content.MakeFriendType = MakeFriendType.Success;
                        Msg.MessageContent = content;
                        SocketTool.GetClientUDP().SendToServer(Msg);
                        Dispatcher.Invoke(() =>
                        {
                            UserListForm.MainUserListForm.AddFriendsItem(DB.Users.Where(u => u.UloginNumber == content.FromNum).First());
                        });
                    }
                }
                else if (content.MakeFriendType == MakeFriendType.Success)
                {
                    MessageBox.Show("对方同意");
                    Dispatcher.Invoke(() =>
                    {
                        UserListForm.MainUserListForm.AddFriendsItem(DB.Users.Where(u => u.UloginNumber == content.ToNum).First());
                    });
                }
                else if (content.MakeFriendType == MakeFriendType.Fail)
                {
                    MessageBox.Show("对方拒绝");
                }
                else if (content.MakeFriendType == MakeFriendType.DeleteSuccess)
                {
                    Dispatcher.Invoke(() => {
                        UserListForm.MainUserListForm.RemoveFriendsItem(UserSource.UloginNumber == content.FromNum ? content.ToNum : content.FromNum);
                    });
                }
                DB.Dispose();
                return;
            }
            else if(Msg.MessageType== MessageType.UpdateUser)
            {
                MessageContent_UpdateUser content = MessageModel.ToModel<MessageContent_UpdateUser>(Msg.MessageContent);
                StaticResource.ALLUserSource = content.GetContent();
                Dispatcher.Invoke(() =>
                {
                    UserInfoChanged(content.GetContent());
                });
                //PrintMessageRouter(content);
            }
            //如果是需要提醒用户的消息就调用一下这个方法
            void PrintMessageRouter(Object content=null)
            {
                if (content != null)
                {
                    Msg.MessageContent = content;
                }
                //如果存在打开的对应登录号的聊天窗体
                if (MainChatForm != null && HasChatUser(Msg.FromLoginNumber))
                {
                    Dispatcher.Invoke(() =>
                    {
                        User UserFrom = GetUserByChatUsersItem(Msg.FromLoginNumber);
                        PrintMessage(Msg, UserFrom, UserSource);
                    });
                }
                //如果不存在
                else
                {
                    //MessageBox.Show("有新的消息--->>" + Msg.FromLoginNumber);
                    Dispatcher.Invoke(() =>
                    {
                        UserListForm.MainUserListForm.AppendToMessageQueue(Msg);
                    });
                }
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
            MainChatForm.TAB_ChatUsers.Items.Clear();
            //之后可以做一下优化，便利每个User并且Remove它
            MainChatForm.ChatTabItems.Clear();
            base.OnClosing(e);
        }

        public new void Show(User UserTo)
        {

            //如果还没有聊天窗体
            if (MainChatForm == null)
            {
                MainChatForm = this;
                MainChatForm.OnInitMessageRouter();
            }
            //一般是第一次在UserList窗体初始化时才会传入Null
            if (UserTo == null) return;
            //如果好友窗体不存在
            if (!HasChatUser(UserTo.UloginNumber ?? 0))
                MainChatForm.AddChatUser(UserTo);

            UserListForm.MainUserListForm.DeQueueMessageBuffers(UserTo.UloginNumber ?? 0, (msg, count) =>
            {
                PrintMessage(msg, UserTo, UserSource);
                //老写法
                //PrintMessage(msg, GetUserByChatUsersItem(msg.FromLoginNumber), UserSource);
            });
        }
        /// <summary>
        /// 将消息输出至List
        /// </summary>
        public void PrintMessage(MessageModel Msg, User From, User To)
        {
            Dispatcher.Invoke(() =>
            {
                long ToNum = 0;
                if (Msg.FromLoginNumber == UserSource.UloginNumber)
                {
                    ToNum = Msg.ToLoginNumber;
                }
                else
                {
                    ToNum = Msg.FromLoginNumber;
                }
                (MainChatForm.ChatTabItems[ToNum].Content as ChatPanel).PrintMessage(Msg, From, To);
            });
        }
        /// <summary>
        /// 手动更改用户信息
        /// </summary>
        public static void UserInfoChanged(User NewUser)
        {
            UserListForm.MainUserListForm.UserSource = NewUser;
            StaticResource.SetHeadImageSource(NewUser.UloginNumber ?? 0, ImageTool.GetUserHeadeImage(NewUser.UloginNumber ?? 0));
            ImageTool.GetUserHeadImage(NewUser.UloginNumber??0);
        }
    }
}
