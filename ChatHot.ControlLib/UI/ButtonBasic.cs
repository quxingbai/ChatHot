﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatHot.ControlLib.UI
{
    public class ButtonBasic : ButtonBase
    {
        static ButtonBasic()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonBasic), new FrameworkPropertyMetadata(typeof(ButtonBasic)));
        }
    }
}
