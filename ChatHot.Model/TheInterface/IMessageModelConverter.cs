using System;
using System.Collections.Generic;
using System.Text;

namespace ChatHot.Model.TheInterface
{
    /// <summary>
    /// 表示这个 MessageModel 可以通过一个 Arg 进行 Xaml Content的转化
    /// </summary>
    public interface IMessageModelConverter
    {
        Object ToMessageContent(Object Arg);
    }
}
