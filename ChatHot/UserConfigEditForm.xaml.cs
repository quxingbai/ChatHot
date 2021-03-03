using ChatHot.ControlLib.Form;
using ChatHot.ControlLib.Tool;
using ChatHot.Model.EFModels.ChatHotDB;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// UserConfigEditForm.xaml 的交互逻辑
    /// </summary>
    public partial class UserConfigEditForm : ChatFormBasic
    {

        public User UserSource
        {
            get { return (User)GetValue(UserSourceProperty); }
            set { SetValue(UserSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserSourceProperty =
            DependencyProperty.Register("UserSource", typeof(User), typeof(UserConfigEditForm), new PropertyMetadata(null));

        private ChatHotContext DB = new ChatHotContext();
        private byte[] HeadImageBytes = new byte[0];
        public UserConfigEditForm()
        {
            InitializeComponent();
            UserSource = DB.Users.First();
        }
        public UserConfigEditForm(User u)
        {
            InitializeComponent();
            UserSource = u;
        }
        private void BT_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BT_Save_Click(object sender, RoutedEventArgs e)
        {
            UserSource.Uname = TEXT_Name.Text;
            UserSource.Utag = TEXT_Tag.Text;
            UserSource.Upassword = TEXT_Pwd.Text;
            SocketTool.GetClientUDP().SendToServer(TheMessageModel.UPDATE_USER(0, UserSource, HeadImageBytes));
        }

        private void BT_HeadImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "*.png|*.png";
            fileDialog.ShowDialog();
            FileInfo file = new FileInfo(fileDialog.FileName);
            if (file.Length > 6000)
            {
                MessageBox.Show("选择文件过大 最大6KB");
            }
            else
            {
                HeadImageBytes = IoTool.GetFileByte(fileDialog.FileName);
                IMG_UserHeadImage.Source = ImageTool.GetByteImage(fileDialog.FileName);
            }
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (UserSourceProperty == e.Property)
            {
                TEXT_Name.Text = UserSource.Uname;
                TEXT_Tag.Text = UserSource.Utag;
                TEXT_Pwd.Text= UserSource.Upassword;
            }
            base.OnPropertyChanged(e);
        }
    }
}
