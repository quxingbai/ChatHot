using ChatHot.ControlLib;
using ChatHot.ControlLib.Form;
using ChatHot.ControlLib.Tool;
using ChatHot.ControlLib.UI;
using ChatHot.Model;
using ChatHot.Model.EFModels.ChatHotDB;
using ChatHot.Model.MessageContent;
using System;
using System.Collections.Generic;
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
    /// UserListForm.xaml 的交互逻辑
    /// </summary>
    public partial class UserListForm : ChatFormBasic
    {

        public User UserSource
        {
            get { return (User)GetValue(UserSourceProperty); }
            set { SetValue(UserSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserSourceProperty =
            DependencyProperty.Register("UserSource", typeof(User), typeof(UserListForm), new PropertyMetadata(null));



        public ICommand SelectBoxAutoFocusedCommand
        {
            get { return (ICommand)GetValue(SelectBoxAutoFocusedCommandProperty); }
            set { SetValue(SelectBoxAutoFocusedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectBoxAutoFocusedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectBoxAutoFocusedCommandProperty =
            DependencyProperty.Register("SelectBoxAutoFocusedCommand", typeof(ICommand), typeof(UserListForm), new PropertyMetadata(null));


        private ChatHotContext DB = new ChatHotContext();

        public static UserListForm MainUserListForm = null;
        /// <summary>
        /// 消息缓存，如果没有打开聊天窗体就直接放这里
        /// </summary>
        public static Dictionary<long, Queue<MessageModel>> MessageQueueBuffer = new Dictionary<long, Queue<MessageModel>>();
        /// <summary>
        /// 消息队列的显示关系
        /// </summary>
        public Dictionary<long, ListBoxItem> MessageQueueListItems = new Dictionary<long, ListBoxItem>();
        public UserListForm(User u)
        {
            InitializeComponent();
            MainUserListForm = this;
            this.UserSource = u;
            ADDRangeFriendsItem(GetFriends());
        }
        public void AddFriendsItem(User f)
        {
            LIST_Friends.Items.Add(f);
        }
        public void ADDRangeFriendsItem(User[] fs)
        {
            foreach(User f in fs)
            {
                AddFriendsItem(f);
            }
        }
        public override void OnApplyTemplate()
        {
            SelectBoxAutoFocusedCommand = new ActionCommand(arg =>
            {
                TextBox t = arg as TextBox;
                t.Text = "";
            });
            new ChatForm(UserSource).Show(null);
            base.OnApplyTemplate();
        }
        //检索
        private void TEXT_UserSelectBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = sender as TextBox;
        }
        public User[] GetFriends()
        {
            long[] FriendNums = DB.Friends.Where(u => u.SendUser == UserSource.UloginNumber || u.AcceptUser == UserSource.UloginNumber).Select(u => u.AcceptUser != UserSource.UloginNumber ? u.AcceptUser : u.SendUser).ToArray();
            var FriendModels = DB.Users.ToArray().Where(u => FriendNums.Where(f => f == u.UloginNumber).Count() > 0).ToArray();
            foreach (User u in FriendModels)
            {
                ImageTool.GetUserHeadImage(u.UloginNumber ?? 0);
            }
            return FriendModels.ToArray();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            OnCloseALL();
        }

        private void ITEM_FriendsItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            User SelectedUser = LIST_Friends.SelectedItem as User;
            if (SelectedUser == null) return;
            new ChatForm(UserSource).Show(SelectedUser);
        }
        private void ITEM_MessageBufferItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MessageBufferModel msg = LIST_Messages.SelectedItem as MessageBufferModel;
            if (msg == null) return;
            if (GetUserByFriends(msg.From.UloginNumber ?? 0) != null)
            {
                new ChatForm(UserSource).Show(msg.From);
            }
            else
            {
                MessageBox.Show("未成为好友，无法交流");
            }
        }
        /// <summary>
        /// 将消息出列
        /// </summary>
        /// <param name="FromNum">谁发的消息</param>
        /// <param name="MessageCallBack">出列后执行的操作 参数[消息，消息计数]</param>
        /// <returns>一共执行力几次</returns>
        public int DeQueueMessageBuffers(long FromNum, Action<MessageModel, int> MessageCallBack)
        {
            int MessageCount = 0;
            if (MessageQueueBuffer.Keys.Where(k => k == FromNum).Count() > 0)
            {
                while (MessageQueueBuffer[FromNum].Count > 0)
                {
                    MessageModel msg = MessageQueueBuffer[FromNum].Dequeue();
                    MessageCallBack?.Invoke(msg, ++MessageCount);
                }
            }
            RemoveMessageBufferItem(FromNum);
            return MessageCount;
        }
        /// <summary>
        /// 添加一条消息进入消息等待队列
        /// </summary>
        public void AppendToMessageQueue(MessageModel Msg)
        {
            //如果不存在此用户的消息队列
            if (MessageQueueBuffer.Keys.Where(k => k == Msg.FromLoginNumber).Count() == 0)
            {
                MessageQueueBuffer.Add(Msg.FromLoginNumber, new Queue<MessageModel>());
                //MessageQueueListItems.Add(Msg.FromLoginNumber,)
            }
            MessageQueueBuffer[Msg.FromLoginNumber].Enqueue(Msg);
            UpdateMessageBufferItem(Msg);
        }
        /// <summary>
        /// 删除一条显示的消息缓存
        /// </summary>
        public bool RemoveMessageBufferItem(long FromNum)
        {
            foreach (var item in LIST_Messages.Items)
            {
                MessageBufferModel msg = item as MessageBufferModel;
                if (msg.Msg.FromLoginNumber == FromNum)
                {
                    LIST_Messages.Items.Remove(msg);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 更新一个消息缓存的显示,包括添加
        /// </summary>
        /// <param name="Msg">消息</param>
        public void UpdateMessageBufferItem(MessageModel Msg)
        {
            MessageBufferModel UpdateItem = null;
            if (HasMessageBufferItem(Msg.FromLoginNumber, out UpdateItem))
            {
                LIST_Messages.Items.Remove(UpdateItem);
            }
            String LastMsg = (Msg.MessageContent as MessageContent_Base).ToMessageContent(null).ToString();
            if (Msg.MessageContent is MessageContent_Image)
            {
                LastMsg = "[图片]";
            }
            LIST_Messages.Items.Insert(0, new MessageBufferModel()
            {
                MessageCount = MessageQueueBuffer[Msg.FromLoginNumber].Count,
                Msg = Msg,
                From = GetUserByFriends(Msg.FromLoginNumber),
                LastMessage = LastMsg,
            });
        }
        /// <summary>
        /// 判断是否有某个人的消息缓存
        /// </summary>
        /// <param name="FromNum">发消息的人</param>
        /// <param name="Msg">返回的消息</param>
        /// <returns>是否有此人的消息</returns>
        public bool HasMessageBufferItem(long FromNum, out MessageBufferModel Msg)
        {
            foreach (var item in LIST_Messages.Items)
            {
                MessageBufferModel msg = item as MessageBufferModel;
                if (msg.Msg.FromLoginNumber == FromNum)
                {
                    Msg = msg;
                    return true;
                }
            }
            Msg = null;
            return false;
        }
        /// <summary>
        /// 从好友中找到一个用户
        /// </summary>
        public User GetUserByFriends(long LoginNumber)
        {
            return GetFriends().Where(u => u.UloginNumber == LoginNumber).First();
        }


        private void LIST_MultADDList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void BT_UsersMultAdd_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            switch (bt.Tag.ToString().ToUpper())
            {
                case "ADDFRIEND":new AddFriendForm(UserSource).Show(); break;
                case "CREATEGROUPCHAT":MessageBox.Show("创建个Der"); break;
            }
            TBT_AddFriendButton.IsChecked = false;
        }


        private void MENUITEM_Friends_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (sender as MenuItem);
            User SelectedUser = item.DataContext as User;
            switch (item.Tag.ToString().ToUpper())
            {
                case "DELETEFRIEND":
                    SocketTool.GetClientUDP().SendToServer(TheMessageModel.MAKEFRIEND(UserSource.UloginNumber??0,SelectedUser.UloginNumber??0,MakeFriendType.Delete))
                    ; break;
            }
        }
        public bool RemoveFriendsItem(long Num)
        {
            foreach(var item in LIST_Friends.Items)
            {
                User u = item as User;
                if (u.UloginNumber == Num)
                {
                    LIST_Friends.Items.Remove(u);
                    return true;
                }
            }
            return false;
        }

        private void MENUITEM_MessageBuffer_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (sender as MenuItem);
            MessageBufferModel Selected = item.DataContext as MessageBufferModel;
            switch (item.Tag.ToString().ToUpper())
            {
                case "MESSAGEBUFFERREMOVE":
                    LIST_Messages.Items.Remove(Selected);
                    ; break;
            }
            
        }

        private void BT_UserHeadSetting_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            UserHeadImage img = bt.Content as UserHeadImage;
            new UserConfigEditForm(StaticResource.ALLUserSource).Show();
        }
    }
}
