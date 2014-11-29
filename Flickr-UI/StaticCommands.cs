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
        private static readonly RoutedUICommand _MoveImagesLocalCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand MoveImagesLocalCommand
        {
            get
            {
                return _MoveImagesLocalCommand;
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

        private static readonly RoutedUICommand _FindImagesToUploadCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand FindImagesToUploadCommand
        {
            get
            {
                return _FindImagesToUploadCommand;
            }
        }

        private static readonly RoutedUICommand _FindImagesToRenameCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand FindImagesToRenameCommand
        {
            get
            {
                return _FindImagesToRenameCommand;
            }
        }

    }
}
