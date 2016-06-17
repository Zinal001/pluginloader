#region Licence
// Copyright (c) 2016 Tomas Bosek
#pragma warning disable CC0065 // Remove trailing whitespace
// 
// This file is part of PluginLoader.
#pragma warning disable CC0065 // Remove trailing whitespace
// 
// PluginLoader is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
#pragma warning disable CC0065 // Remove trailing whitespace
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
#pragma warning disable CC0065 // Remove trailing whitespace
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
#endregion
using System;
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace PatcherGUI
{
    public partial class DetectionPage : UserControl, IPage
    {
        private static DetectionPage instance;
        public static DetectionPage Instance => instance;

        public static List<string> RequiredFiles => Patcher.PatchRunner.RequiredFiles.ToList();
        public static string DefaultPath => System.IO.Path.Combine(
            Environment.ExpandEnvironmentVariables("%programfiles(x86)%"),
            @"Steam\steamapps\common\Interstellar Rift\Build");
        public static string DefaultPluginsPath => System.IO.Path.Combine(
            Environment.ExpandEnvironmentVariables("%appdata%"), @"InterstellarRift\plugins");

        private string path;
        public string Path => path;

        public bool CanGoBack => true;

        private bool canGoNext;
        public bool CanGoNext => canGoNext;
        public string NextButtonLabel => "Next";
        public DetectionPage()
        {
            InitializeComponent();
            instance = this;

            pathBox.Text = DefaultPath;
            pluginsPathBox.Text = DefaultPluginsPath;
        }

        public void PageShowed()
        {
            filesExist();
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowser = new System.Windows.Forms.FolderBrowserDialog
            {
                SelectedPath = pathBox.Text
            };

            var handle = new WindowInteropHelper(MainWindow.Instance).Handle;
            folderBrowser.ShowDialog(new OldWindow(handle));
            pathBox.Text = folderBrowser.SelectedPath;
            folderBrowser.Dispose();

            filesExist();
        }

        private bool filesExist()
        {
            if (!RequiredFiles.All(file => System.IO.File.Exists(System.IO.Path.Combine(pathBox.Text, file))))
            {
                pathBox.Background = new SolidColorBrush(Color.FromRgb(255,220,220));
                canGoNext = false;
            }
            else
            {
                pathBox.Background = new SolidColorBrush(Color.FromRgb(220, 255, 220));
                path = pathBox.Text;
                canGoNext = true;
            }

            MainWindow.Instance.RevalidateButtons();
            return canGoNext;
        }
    }
}
