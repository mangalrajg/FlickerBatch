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
        public static RoutedUICommand MoveImagesLocalCommand { get { return _MoveImagesLocalCommand; } }

        private static readonly RoutedUICommand _MoveImagesRemoteCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand MoveImagesRemoteCommand { get { return _MoveImagesRemoteCommand; } }

        private static readonly RoutedUICommand _FindImagesToUploadCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand UploadCommand { get { return _FindImagesToUploadCommand; } }

        private static readonly RoutedUICommand _FindImagesToRenameCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand FindImagesToRenameCommand { get { return _FindImagesToRenameCommand; } }

        private static readonly RoutedUICommand _LoadFromDBCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand LoadFromDBCommand { get { return _LoadFromDBCommand; } }

        private static readonly RoutedUICommand _LoadFromFileCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand LoadFromFileCommand { get { return _LoadFromFileCommand; } }

        private static readonly RoutedUICommand _LoadBaseDirFromFileCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand LoadBaseDirFromFileCommand { get { return _LoadBaseDirFromFileCommand; } }

        private static readonly RoutedUICommand _LoadAllAlbumsFromFlicker = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand LoadAllAlbumsFromFlicker { get { return _LoadAllAlbumsFromFlicker; } }

        private static readonly RoutedUICommand _LoadAlbumsFromFlicker = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand LoadAlbumFromFlicker { get { return _LoadAlbumsFromFlicker; } }

        private static readonly RoutedUICommand _SyncCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand SyncCommand { get { return _SyncCommand; } }

        private static readonly RoutedUICommand _ClearDataGrid = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand ClearDataGrid { get { return _ClearDataGrid; } }

        private static readonly RoutedUICommand _Cancel = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand CancelCommand { get { return _Cancel; } }

        private static readonly RoutedUICommand _MergeCommand = new RoutedUICommand("description", "DoSomethingCommand", typeof(StaticCommands));
        public static RoutedUICommand MergeCommand { get { return _MergeCommand; } }

    }
}
