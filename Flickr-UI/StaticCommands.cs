using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Flickr_UI
{
    public static class StaticCommands
    {
        private static readonly RoutedUICommand _MoveImagesCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));

        public static RoutedUICommand MoveImagesCommand
        {
            get
            {
                return _MoveImagesCommand;
            }
        }
        private static readonly RoutedUICommand _MoveImagesRemoteCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));

        public static RoutedUICommand MoveImagesRemoteCommand
        {
            get
            {
                return _MoveImagesRemoteCommand;
            }
        }

    }
}
