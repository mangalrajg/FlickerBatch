﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flickr_UI
{
    public static class StaticCommands
    {
        private static readonly RoutedUICommand doSomethingCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));

        public static RoutedUICommand DoSomethingCommand
        {
            get
            {
                return doSomethingCommand;
            }
        }
    }
}
