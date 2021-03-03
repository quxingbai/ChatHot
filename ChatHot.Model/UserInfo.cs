using ChatHot.Model.EFModels.ChatHotDB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ChatHot.Model
{
    public class UserInfo
    {
        public EndPoint UserIP { get; set; }
        public User User { get; set; }
    }
}
