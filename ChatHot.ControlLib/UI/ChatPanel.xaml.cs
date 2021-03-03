using ChatHot.ControlLib.Tool;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatHot.ControlLib.UI
{
    /// <summary>
    /// ChatPanel.xaml 的交互逻辑
    /// </summary>
    public partial class ChatPanel : UserControl
    {
        private ChatHotContext DB = new ChatHotContext();
        private User UserFrom { get => TEXT_MessageSendBox.UserFrom; }
        private User UserTo { get => TEXT_MessageSendBox.UserTo; }
        public Action CloseCallBack
        {
            set
            {
                TEXT_MessageSendBox.CloseCommand = new ActionCommand(() =>
                  {
                      value();
                  });
            }
            get
            {
                return () => { TEXT_MessageSendBox.CloseCommand.Execute(null); };
            }
        }
        public ChatPanel()
        {
            InitializeComponent();
        }
        public ChatPanel(User From, User To)
        {
            InitializeComponent();

            this.TEXT_MessageSendBox.UserFrom = From;
            this.TEXT_MessageSendBox.UserTo = To;
        }
        /// <summary>
        /// 基PrintMessage
        /// </summary>
        public void PrintMessage(MessageModel Msg, User From, User To)
        {
            #region MessageRouter
            if (Msg.MessageType == MessageType.DataPacket)
            {
                return;
            }
            else if (Msg.MessageType == MessageType.Image)
            {
                String path = MessageModel.ToModel<MessageContent_Image>(Msg.MessageContent).GetContent();
            }
            #endregion
            //MessageRow msg = new MessageRow() { MessageSource = Msg };
            //LIST_Message.Items.Add(msg);
            DisplayMessageModel msg = new DisplayMessageModel()
            {
                IsFrom = UserFrom.UloginNumber == From.UloginNumber,
                UserFrom = From,
                UserTo = To,
                Msg = Msg,
            };
            LIST_Message.Items.Add(msg);
            LIST_Message.ScrollIntoView(msg);
        }

        private void TEXT_MessageSendBox_SendedMessage(object Sender, User From, User To, MessageModel Msg, int MessageState)
        {
            PrintMessage(Msg, From, To);
        }

        private void TEXT_MessageSendBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!Keyboard.IsKeyDown(Key.LeftShift))
                {
                    TEXT_MessageSendBox.SendMessageCommand.Execute(null);
                }
                else
                {
                    TEXT_MessageSendBox.Text += "\n";
                    TEXT_MessageSendBox.Select(TEXT_MessageSendBox.Text.Length, 0);
                }
            }

        }
    }
}
