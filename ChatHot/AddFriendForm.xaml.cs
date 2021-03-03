using ChatHot.ControlLib.Form;
using ChatHot.ControlLib.Tool;
using ChatHot.ControlLib.UI;
using ChatHot.Model.EFModels.ChatHotDB;
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
    /// AddFriendForm.xaml 的交互逻辑
    /// </summary>
    public partial class AddFriendForm : ChatFormBasic
    {

        ChatHotContext DB = new ChatHotContext();
        public User UserSource
        {
            get { return (User)GetValue(UserSourceProperty); }
            set { SetValue(UserSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserSourceProperty =
            DependencyProperty.Register("UserSource", typeof(User), typeof(AddFriendForm), new PropertyMetadata(null));


        public AddFriendForm()
        {
            InitializeComponent();
        }
        public AddFriendForm(User user)
        {
            InitializeComponent();
            this.UserSource = user;
        }

        private void BT_SelectUserButton_Click(object sender, RoutedEventArgs e)
        {
            String KeyWord = TEXT_SelectUserText.Text;
            
            var dbResult = DB.Users.Where(u => u.UloginNumber.ToString() == KeyWord || u.Uname == KeyWord);
            if (dbResult.Count() > 0)
            {
                LIST_SelectUserList.ItemsSource = dbResult.ToArray();
            }
        }

        private void BT_SendAddFriendMessage_Click(object sender, RoutedEventArgs e)
        {
            ContentControl bt = sender as ContentControl;
            long num = Convert.ToInt64(bt.Tag);
            SocketTool.GetClientUDP().SendToServer(TheMessageModel.MAKEFRIEND(UserSource.UloginNumber??0, num, Model.MessageContent.MakeFriendType.SendStart));
        }
    }
}
