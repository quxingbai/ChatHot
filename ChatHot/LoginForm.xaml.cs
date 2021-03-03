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
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static ChatHot.Model.MessageStringMark;

namespace ChatHot
{
    /// <summary>
    /// LoginForm.xaml 的交互逻辑
    /// </summary>
    public partial class LoginForm : ChatFormBasic
    {






        public Double HeadImageOffset
        {
            get { return (Double)GetValue(HeadImageOffsetProperty); }
            set { SetValue(HeadImageOffsetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeadImageOffset.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeadImageOffsetProperty =
            DependencyProperty.Register("HeadImageOffset", typeof(Double), typeof(LoginForm), new PropertyMetadata(HeadImageDefaultOffset));


        public double TitleBoxHeight
        {
            get { return (double)GetValue(TitleBoxHeightProperty); }
            set { SetValue(TitleBoxHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleBoxHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleBoxHeightProperty =
            DependencyProperty.Register("TitleBoxHeight", typeof(double), typeof(LoginForm), new PropertyMetadata(TitleBoxDefautlHeight));



        private XmlController XMLLoginLog = new XmlController(StaticResource.XmlLoginLog);
        private ChatHotContext DB = new ChatHotContext();
        private const Double TitleBoxDefautlHeight = 127;
        private const Double HeadImageDefaultOffset = -30.0;
        private const Double HeadImageLoggingOffset = -240;
        private const int OnLineTokenSendTime = 19;//秒
        private Thread OnlineSendThread = null;//用于告诉服务器是否在线的
        private Thread LoginSubmitThread = null;

        public static LoginForm MainLoginForm = null;
        public static SocketTool SocketClient = SocketTool.GetClientUDP();
        public LoginForm()
        {
            //if (MainLoginForm != null) throw new Exception("不可以生成第二个登录窗体，因为会导致一些不可控的错误");
            InitializeComponent();
            MainLoginForm = this;
            SocketClient.ErrorCallBack = (error) =>
              {
                  OnLoginError(error.Message+"\n");
                  Close(1900);
              };
            ReloadUserImageBuffer();
            SocketClient.GetNewMessage += SocketClient_GetNewMessage;
            TEXT_LoginNumber.SelectedValuePath = "UloginNumber";
            List<User> LoginLogUsers = XMLLoginLog.GetBodyElements<User>();
            TEXT_LoginNumber.ItemsSource = LoginLogUsers;
            TEXT_LoginNumber.SelectedIndex = 0;
        }

        public static void ReloadUserImageBuffer()
        {
            //加载用户头像缓存
            foreach (String ui in IoTool.GetFiles(StaticResource.UserHeadImageBasePath))
            {
                long loginnum = 0;
                String fn = IoTool.GetFileName(ui);
                if (long.TryParse(fn, out loginnum))
                {
                    StaticResource.SetHeadImageSource(loginnum, ImageTool.GetUserHeadeImage(loginnum));
                }
            }
        }
        private void SocketClient_GetNewMessage(MessageModel Msg, EndPoint From)
        {
            if (Msg.MessageType == MessageType.Login)
            {
                MessageContent_Login msg = MessageModel.ToModel<MessageContent_Login>(Msg.MessageContent);
                bool IsLoginSuccess = msg.LoginType == LoginType.Result && msg.WithMessage == MessageStringMark.GetCodeMsg(CodeMessageType.LoginSuccess).ToString();
                if (!IsLoginSuccess)
                {
                    OnLoginError(MessageStringMark.GetCodeMsg(Convert.ToInt64(msg.WithMessage)));
                    ViewChange(0);
                }
                else
                {
                    OnLoginSuccess(msg.GetContent());
                }
            }
            else if (Msg.MessageType == MessageType.File)
            {

            }
            //else if (Msg.MessageType == MessageType.Get)
            //{
            //    MessageContent_Get get = MessageModel.ToModel<MessageContent_Get>(Msg.MessageContent);
            //    if (get.RequestType == Get_Type.File)
            //    {
            //        if (get.RequestFileType == File_Type.HeadImagoe)
            //        {
            //            IoTool.CreateDirectory(StaticResource.UserHeadImageBasePath);
            //            String headPath = StaticResource.ToUserHeadImagePath(get.RequestArg + ".png");
            //            IoTool.SaveFile(headPath, get.Content);
            //            Dispatcher.Invoke(() =>
            //            {
            //                StaticResource.SetHeadImageSource(long.Parse(get.RequestArg.ToString()), ImageTool.GetUserHeadeImage(long.Parse(get.RequestArg.ToString())));
            //            });
            //        }
            //    }
            //}

        }

        private void BT_Login_Click(object sender, RoutedEventArgs e)
        {

            if (TEXT_LoginNumber.Text.Trim() == "" || TEXT_Password.Password.Trim() == "")
            {
                return;
            }
            long number = Convert.ToInt64(TEXT_LoginNumber.Text);
            String password = TEXT_Password.Password;
            LoginSubmitThread = new Thread(() =>
            {
                ViewChange(1);
                var user = DB.Users.Where(u => u.UloginNumber == number && u.Upassword == password);
                if (user.Count() == 0)
                {
                    OnLoginError();
                    ViewChange(0);
                    return;
                }
                else
                {
                    User u = user.First();
                    int result = SocketClient.SendTo(new MessageModel()
                    {
                        FromIpaddress = SocketTool.IPV4Address.ToString(),
                        FromLoginNumber = u.UloginNumber ?? 0,
                        MessageContent = new MessageContent_Login() { Content = MessageModel.ToBytes(u), RequestDateTime = DateTime.Now.ToString(), LoginType = LoginType.Submit, WithMessage = "" },
                        MessageType = MessageType.Login,
                        SendDateTime = DateTime.Now.ToString(),
                        ToLoginNumber = 0,
                    },
                         StaticResource.ServerIpaddress);

                }
            });
            LoginSubmitThread.Start();
        }
        /// <summary>
        /// 切换界面
        /// 0登录界面，1登陆中界面
        /// </summary>
        public void ViewChange(int ViewCount)
        {
            Dispatcher.Invoke(() =>
            {
                switch (ViewCount)
                {
                    case 0: GRID_LoginSubmit.Visibility = Visibility.Visible; BT_LoginCancel.Visibility = Visibility.Collapsed; TitleBoxHeight = TitleBoxDefautlHeight; HeadImageOffset = HeadImageDefaultOffset; TEXT_LoggingText.Visibility = Visibility.Collapsed; break;
                    case 1: GRID_LoginSubmit.Visibility = Visibility.Collapsed; BT_LoginCancel.Visibility = Visibility.Visible; TitleBoxHeight = ActualHeight; HeadImageOffset = HeadImageLoggingOffset; TEXT_LoggingText.Visibility = Visibility.Visible; break;
                }
            });
        }
        //如果登录失败
        public void OnLoginError(String Msg = "你输入的账户名或密码不正确，你要找回密码吗？别想了。")
        {
            Dispatcher.Invoke(() =>
            {
                GRID_LoginError.Visibility = Visibility.Visible;
                TEXT_LoginErrorMsg.Text = Msg;
            });
        }
        //如果登录成功
        public void OnLoginSuccess(User u)
        {
            StaticResource.ALLUserSource = u;
            StartDoubleAnima(WidthProperty, 0, "0:0:1");
            Thread.Sleep(700);
            ImageTool.GetUserHeadImage(u.UloginNumber ?? 0);
            u.UheadImage = StaticResource.ToUserHeadImagePath(u.UloginNumber.ToString() + ".png");
            Dispatcher.Invoke(() =>
            {
                new UserListForm(StaticResource.ALLUserSource).Show();
                StaticResource.SetHeadImageSource(u.UloginNumber??0, ImageTool.GetUserHeadeImage(u.UloginNumber ?? 0));
                Hide();
                OnlineSendThread = new Thread(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(OnLineTokenSendTime * 1000);
                        SocketClient.SendTo(new MessageModel(MessageType.Login, u.UloginNumber ?? 0, 0, SocketTool.IPV4Address.ToString(), new MessageContent_Login()
                        {
                            Content = MessageModel.ToBytes(u),
                            LoginType = LoginType.ImOnline,
                            RequestDateTime = DateTime.Now.ToString(),
                            WithMessage = ""
                        }), StaticResource.ServerIpaddress);
                    }
                });
                OnlineSendThread.Start();
                SocketClient.GetNewMessage -= SocketClient_GetNewMessage;
                //添加一项登录记录
                if (!CB_IsSavePassword.IsChecked ?? false)
                {
                    u.Upassword = "";
                }
                foreach (User tu in (List<User>)TEXT_LoginNumber.ItemsSource)
                {
                    XMLLoginLog.SelectALL(sn =>
                    {
                        if (sn["Uid"].InnerText == u.Uid.ToString())
                        {
                            XMLLoginLog.Body.RemoveChild(sn);
                            return;
                        }
                    });
                }
                XMLLoginLog.Body.AppendChild(XMLLoginLog.ModelToNode(u));
                XMLLoginLog.Save();

            });
        }
        private void TEXT_LoginNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            User user = TEXT_LoginNumber.SelectedItem as User;
            if (user != null)
            {
                //IMAGE_UserImage.Source = new BitmapImage(new Uri(user.UheadImage));
                TEXT_Password.Password = user.Upassword;
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            SocketClient.Dispose();
            base.OnClosed(e);
        }

        private void BT_BackToLoginSubmit_Click(object sender, RoutedEventArgs e)
        {
            GRID_LoginError.Visibility = Visibility.Collapsed;
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                OnlineSendThread?.Abort();
            }
            catch
            {

            }
            base.OnClosing(e);
        }

        private void BT_LoginCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoginSubmitThread?.Abort();
            }
            catch
            {

            }
            ViewChange(0);
        }
        /// <summary>
        /// Show 并且将获取消息处理权
        /// </summary>
        public void ShowWithMessageRouter()
        {
            SocketClient.GetNewMessage += SocketClient_GetNewMessage;
            Show();
            StartDoubleAnima(WidthProperty, 428, "0:0:1");
        }

        private void LINK_RegisterCode_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
