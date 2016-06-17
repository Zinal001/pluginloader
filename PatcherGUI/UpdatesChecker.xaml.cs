#region Licence
// Copyright (c) 2016 Tomas Bosek
// 
// This file is part of PluginLoader.
// 
// PluginLoader is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as
// published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
#endregion
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;

namespace PatcherGUI
{
    public partial class UpdatesChecker : Window
    {
        private readonly string gitUser = "Bosek";
        private readonly string gitRepo = "pluginloader";

        public UpdatesChecker()
        {
            InitializeComponent();
            PluginLoaderVersion.Text = PatchPage.Instance.PatchRunner.GetPLVersion();

            GitHubVersion.Text = "-checking-";
            new Thread(() => Dispatcher.Invoke(() =>
            {
                var plversion = Version.Parse(PluginLoaderVersion.Text);
                var gitversion = getLastGitReleaseVersion(gitUser, gitRepo);
                GitHubVersion.Text = gitversion != null ? gitversion.ToString() : PluginLoaderVersion.Text;
                gitversion = Version.Parse(GitHubVersion.Text);

                if (plversion >= gitversion)
                {
                    Close();
                }
                else
                {
                    InfoText.Text = "New update available.";
                    okButton.Content = "Close patcher";
                    okButton.Click += (sender, args) => MainWindow.Instance.Close();
                    okButton.Click += (sender, args) => Close();
                    okButton.IsEnabled = true;
                }
            })).Start();
        }

        private static Version getLastGitReleaseVersion(string owner, string repo)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create($"https://api.github.com/repos/{owner}/{repo}/releases/latest");
                request.UserAgent = owner;
                request.Timeout = 1000;
                var response = (HttpWebResponse)request.GetResponse();

                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic data = serializer.DeserializeObject(reader.ReadToEnd());
                    return Version.Parse((string)data["tag_name"]);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
