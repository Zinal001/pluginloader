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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Security.Principal;
using System.Threading;
using Patcher;

namespace PatcherGUI
{
    public partial class PatchPage : UserControl, IPage
    {
        private static PatchPage instance;
        public static PatchPage Instance => instance;

        private PatchRunner patchRunner;
        public PatchRunner PatchRunner => patchRunner;

        private readonly static ColorAnimation greenAnimation = new ColorAnimation(Color.FromRgb(220,255,220), new Duration(TimeSpan.FromMilliseconds(300)));
        private readonly static ColorAnimation redAnimation = new ColorAnimation(Color.FromRgb(255, 220, 220), new Duration(TimeSpan.FromMilliseconds(300)));

        private readonly bool canGoBack;
        public bool CanGoBack => canGoBack;
        private bool canGoNext;
        public bool CanGoNext => canGoNext;
        public string NextButtonLabel => "Next";

        private ListBoxItem lastListBoxItem;
        public PatchPage()
        {
            InitializeComponent();
            instance = this;

            Logger.OnInfo += (string text) => this.Dispatcher.Invoke(() =>
            {
                var endsLine = text.EndsWith(Environment.NewLine);
                text = text.Replace(Environment.NewLine, String.Empty);

                if (lastListBoxItem != null)
                    lastListBoxItem.Content += text;
                else if (lastListBoxItem == null)
                    lastListBoxItem = CreateNewItem(text);

                if (endsLine)
                    lastListBoxItem = null;
            });

            Logger.OnError += (string text) => this.Dispatcher.Invoke(() =>
            {
                var endsLine = text.EndsWith(Environment.NewLine);
                text = text.Replace(Environment.NewLine, String.Empty);

                if (lastListBoxItem != null)
                    lastListBoxItem.Content += text;
                else if (lastListBoxItem == null)
                    lastListBoxItem = CreateNewItem(text);

                FailAnimation(lastListBoxItem);
                if (endsLine)
                    lastListBoxItem = null;
            });

            Logger.OnSuccess += (string text) => this.Dispatcher.Invoke(() =>
            {
                var endsLine = text.EndsWith(Environment.NewLine);
                text = text.Replace(Environment.NewLine, String.Empty);

                if (lastListBoxItem != null)
                    lastListBoxItem.Content += text;
                else if (lastListBoxItem == null)
                    lastListBoxItem = CreateNewItem(text);

                SuccessAnimation(lastListBoxItem);
                if (endsLine)
                    lastListBoxItem = null;
            });
        }

        public ListBoxItem CreateNewItem(string text = "")
        {
            var item = new ListBoxItem
            {
                Background = new SolidColorBrush(Colors.Transparent),
                Content = text
            };
            listBox.Items.Add(item);
            return item;
        }

        public void PageShowed()
        {
            if (patchRunner == null)
            {
                patchRunner = new PatchRunner(DetectionPage.Instance.Path);
                patchRunner.LoadData();

                var updatesChecker = new UpdatesChecker();
                updatesChecker.ShowDialog();

                var thread = new Thread(() =>
                {
                    patchRunner.Patch();
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();

                canGoNext = true;
                MainWindow.Instance.RevalidateButtons();
            }
        }

        public static void SuccessAnimation(ListBoxItem item)
        {
            item.Background.BeginAnimation(SolidColorBrush.ColorProperty, greenAnimation);
        }
        public static void FailAnimation(ListBoxItem item)
        {
            item.Background.BeginAnimation(SolidColorBrush.ColorProperty, redAnimation);
        }
    }
}
