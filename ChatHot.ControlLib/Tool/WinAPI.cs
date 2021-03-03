using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChatHot.ControlLib.Tool
{
    public class WinAPI
    {
        public enum MouseEventType
        {
            Move=0x0001,
            LeftButtonDown=0x0002,
            LeftButtonUp=0x0004,
            RightButtonDown=0x0008,
            RightButtonUp=0x00010,
            MiddleButtonDown=0x0020,
            MiddleButtonUp=0x0040,
            ABSPosition = 0x8000,
        }
        public enum ShowWindowState
        {
            Hide=0,
            Show = 1,
        }

        public class User32
        {
            public const String DLLName = "User32";
            [DllImport(DLLName,EntryPoint ="mouse_event")]
            public static extern void Mouse_Event(int MouseType, int X, int Y, int Data, int ExtraInfo);
            [DllImport(DLLName, EntryPoint = "mouse_event")]
            public static extern void Mouse_Event(MouseEventType MouseType, int X, int Y, int Data, int ExtraInfo);

            [DllImport(DLLName, EntryPoint = "SendMessageA")]
            public static extern void SendMessage(IntPtr Hwnd, int Msg, IntPtr Wparam, String Param);
            [DllImport(DLLName, EntryPoint = "ShowWindow")]
            public static extern void ShowWindow(IntPtr Hwnd, int State);
            [DllImport(DLLName, EntryPoint = "ShowWindow")]
            public static extern void ShowWindow(IntPtr Hwnd, ShowWindowState State);
            [DllImport(DLLName,EntryPoint ="GetFocus")]
            public static extern IntPtr GetFocus();


        }
    }
}
