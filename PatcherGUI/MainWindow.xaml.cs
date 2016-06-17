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
using System.Collections.Generic;
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Patcher;

namespace PatcherGUI
{
    partial class MainWindow : Window
    {
        private static MainWindow instance;
        public static MainWindow Instance => instance;

        private readonly List<IPage> pages = new List<IPage>();
        public List<IPage> Pages => pages;

        private IPage actualPage;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;

            pages.Add(new DescriptionPage());
            pages.Add(new LicencePage());
            pages.Add(new DetectionPage());
            pages.Add(new PatchPage());
            pages.Add(new FinalPage());

            actualPage = pages.First();
            loadPage((Control)actualPage);
        }

        private int getPageIndex(IPage page)
        {
            return pages.FindIndex(p => p == page);
        }

        private bool hasNextPage(IPage page)
        {
            var index = getPageIndex(page);
            if (index < pages.Count - 1)
                return true;
            return false;
        }

        private void loadNextPage()
        {
            if (actualPage.CanGoNext)
            {
                if (hasNextPage(actualPage))
                    loadPage((Control)pages[getPageIndex(actualPage) + 1]);
                else
                    Close();
            }
        }

        private bool hasPreviousPage(IPage page)
        {
            var index = getPageIndex(page);
            if (0 < index)
                return true;
            return false;
        }

        private void loadPreviousPage()
        {
            if (hasPreviousPage(actualPage) && actualPage.CanGoBack)
                loadPage((Control)pages[getPageIndex(actualPage) - 1]);
        }


        public void RevalidateButtons()
        {
            backButton.Visibility = hasPreviousPage((IPage)actualPage) ? Visibility.Visible : Visibility.Hidden;
            backButton.IsEnabled = ((IPage)actualPage).CanGoBack;

            nextButton.IsEnabled = ((IPage)actualPage).CanGoNext;
            nextButton.Content = ((IPage)actualPage).NextButtonLabel;
        }

        private void loadPage(Control page)
        {
            Viewer.Content = page;
            page.HorizontalAlignment = HorizontalAlignment.Stretch;
            page.VerticalAlignment = VerticalAlignment.Stretch;
            actualPage = (IPage)page;

            RevalidateButtons();
            ((IPage)page).PageShowed();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            loadNextPage();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            loadPreviousPage();
        }
    }
}
