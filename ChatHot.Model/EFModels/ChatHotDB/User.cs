using System;
using System.Collections.Generic;

#nullable disable

namespace ChatHot.Model.EFModels.ChatHotDB
{
    public partial class User
    {
        public long Uid { get; set; }
        public string Uname { get; set; }
        public string Utag { get; set; }
        public string UheadImage { get; set; }
        public long? UloginNumber { get; set; }
        public string Upassword { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
