using ChatHot.ControlLib.Form;
using ChatHot.Model;
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
    /// RegisterUserForm.xaml 的交互逻辑
    /// </summary>
    public partial class RegisterUserForm : ChatFormBasic
    {
        public RegisterUserForm()
        {
            InitializeComponent();
        }

        private void BT_Register_Click(object sender, RoutedEventArgs e)
        {
            User u = new User()
            {
                UheadImage = StaticResource.DefaultUserHeadImageBasePath,
                UloginNumber = RandomLoginNumber(),
                Uname = TEXT_Name.Text,
                Upassword = TEXT_Pwd.Text,
            };
            using (ChatHotContext db = new ChatHotContext())
            {
                db.Users.Add(u);
                int result = db.SaveChanges();
                Content = new TextBox() { Text = result > 0 ? "注册成功 登录号为\n" + u.UloginNumber : "注册失败" ,FontSize=40};
            }
        }
        public long RandomLoginNumber()
        {
            Random rd = new Random();
            using (ChatHotContext db = new ChatHotContext())
            {
                while (true)
                {
                    int LeftCode = rd.Next(10000, 90000);
                    int RightCode = rd.Next(10000, 90000);
                    long code = Convert.ToInt64(LeftCode.ToString() + RightCode);
                    if (db.Users.Where(u => u.UloginNumber == code).Count() == 0)
                    {
                        return code;
                    }
                }
            }
        }
    }
}
