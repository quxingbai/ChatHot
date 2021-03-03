using ChatHot.ControlLib.Tool;
using ChatHot.Model;
using ChatHot.Model.EFModels.ChatHotDB;
using ChatHot.Model.MessageContent;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatHot.ControlLib.UI
{
    public delegate void SendedMessageDel(Object Sender, User From, User To, MessageModel Msg, int MessageState);
    public class ChatTextBox : TextBox
    {



        public ListBox MessageReceiveTarget
        {
            get { return (ListBox)GetValue(MessageReceiveTargetProperty); }
            set { SetValue(MessageReceiveTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MessageReceiveTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageReceiveTargetProperty =
            DependencyProperty.Register("MessageReceiveTarget", typeof(ListBox), typeof(ChatTextBox), new PropertyMetadata(null));



        public ICommand SendImageCommand
        {
            get { return (ICommand)GetValue(SendImageCommandProperty); }
            set { SetValue(SendImageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SendImageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SendImageCommandProperty =
            DependencyProperty.Register("SendImageCommand", typeof(ICommand), typeof(ChatTextBox), new PropertyMetadata(null));



        public ICommand SendMessageCommand
        {
            get { return (ICommand)GetValue(SendMessageCommandProperty); }
            set { SetValue(SendMessageCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SendMessageCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SendMessageCommandProperty =
            DependencyProperty.Register("SendMessageCommand", typeof(ICommand), typeof(ChatTextBox), new PropertyMetadata(null));



        public ICommand CloseCommand
        {
            get { return (ICommand)GetValue(CloseCommandProperty); }
            set { SetValue(CloseCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CloseCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(ChatTextBox), new PropertyMetadata(null));



        public User UserFrom
        {
            get { return (User)GetValue(UserFromProperty); }
            set { SetValue(UserFromProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserFrom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserFromProperty =
            DependencyProperty.Register("UserFrom", typeof(User), typeof(ChatTextBox), new PropertyMetadata(null));



        public User UserTo
        {
            get { return (User)GetValue(UserToProperty); }
            set { SetValue(UserToProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UserTo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UserToProperty =
            DependencyProperty.Register("UserTo", typeof(User), typeof(ChatTextBox), new PropertyMetadata(null));

        /// <summary>
        /// 发送了消息
        /// </summary>
        public event SendedMessageDel SendedMessage;

        private SocketTool SocketClient = null;
        static ChatTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChatTextBox), new FrameworkPropertyMetadata(typeof(ChatTextBox)));
        }
        public ChatTextBox()
        {
        }
        /// <param name="From">当前客户端的用户</param>
        /// <param name="To">与当前客户端聊天的用户</param>
        public ChatTextBox(SocketTool socket, User From, User To)
        {
            SocketClient = socket;
            UserFrom = From;
            UserTo = To;
        }
        /// <param name="From">当前客户端的用户</param>
        /// <param name="To">与当前客户端聊天的用户</param>
        public ChatTextBox(User From, User To)
        {
            SocketClient = SocketTool.GetClientUDP();
            UserFrom = From;
            UserTo = To;
        }

        public override void OnApplyTemplate()
        {
            SocketClient = SocketTool.GetClientUDP();
            SendMessageCommand = new ActionCommand(() =>
            {
                MessageContent_String MsgContent = new MessageContent_String(Text);
                MessageModel msg = new MessageModel(MessageType.String, UserFrom.UloginNumber ?? 0, UserTo.UloginNumber ?? 0, SocketTool.IPV4Address.ToString(), MsgContent);
                int MsgState = SocketClient.SendTo(msg, StaticResource.ServerIpaddress);
                SendedMessage?.Invoke(this, UserFrom, UserTo, msg, MsgState);
                Text = "";
            });
            SendImageCommand = new ActionCommand(() =>
              {
                  OpenFileDialog SelectDialog = new OpenFileDialog();
                  SelectDialog.Multiselect = true;
                  SelectDialog.Filter = "*.jpg|*.png";
                  SelectDialog.ShowDialog();
                  String[] files = SelectDialog.FileNames;
                  long FromNum = UserFrom.UloginNumber ?? 0;
                  long ToNum = UserTo.UloginNumber ?? 0;
                  foreach (String file in files)
                  {
                      ThreadPool.QueueUserWorkItem(call =>
                      {
                          String FileName = "";
                          SocketTool.TCPSendFile(file, FromNum, ToNum, DataPacketType.ImageDataPacket,out FileName);
                          MessageModel msg = TheMessageModel.IMAGE(FileName, FromNum, ToNum);
                          int MsgState = SocketTool.GetClientUDP().SendToServer(msg);
                          //将自己选择的文件位置作为图片发送出去
                          MessageContent_Image localImg = MessageModel.ToModel<MessageContent_Image>(msg.MessageContent);
                          localImg.SetContent(file);
                          msg.MessageContent = localImg;
                          Dispatcher.Invoke(() =>
                          {
                              SendedMessage?.Invoke(this, UserFrom, UserTo, msg, MsgState);
                          });
                          Thread.Sleep(1000);
                      });
                  }
              });
            base.OnApplyTemplate();
        }
    }
}
