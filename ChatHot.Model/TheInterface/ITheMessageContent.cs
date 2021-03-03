using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.TheInterface
{
   public  interface ITheMessageContent<T>
    {
        Byte[] Content { get; set; }
        void SetContent(T Source);
        T GetContent(Object Param=null);

    }
}
