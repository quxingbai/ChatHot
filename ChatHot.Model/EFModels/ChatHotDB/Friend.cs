using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ChatHot.Model.EFModels.ChatHotDB
{
    public partial class Friend
    {
        public int Fid { get; set; }
        public long SendUser { get; set; }
        public long AcceptUser { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? LockDate { get; set; }
    }
}
