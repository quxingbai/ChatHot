using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Runtime.CompilerServices;
using ChatHot.Model.EFModels.ChatHotDB;

namespace ChatHot.Model
{
    public class DisplayMessageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool isfrom { get; set; }
        public bool IsFrom { get { return isfrom; } set { isfrom = value;OnPropertyChanged(); } }
        private User userfrom { get; set; }
        public User UserFrom { get { return userfrom; } set { userfrom = value; OnPropertyChanged(); } }
        private User userto { get; set; }
        public User UserTo { get { return userto; } set { userto = value; OnPropertyChanged(); } }
        private MessageModel msg { get; set; }
        public MessageModel Msg { get { return msg; } set { msg = value; OnPropertyChanged(); } }



        public DisplayMessageModel()
        {

        }
        public void OnPropertyChanged([CallerMemberNameAttribute] String Property="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Property));
        }
    }
}
