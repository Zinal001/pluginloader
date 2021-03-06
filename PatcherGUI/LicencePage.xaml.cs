﻿#region Licence
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
using System.Reflection;

namespace PatcherGUI
{
    public partial class LicencePage : UserControl, IPage
    {
        public bool CanGoBack
        {
            get { return true; }
        }
        public bool CanGoNext
        {
            get { return true; }
        }
        public string NextButtonLabel
        {
            get
            {
                return "Next";
            }
        }
        public LicencePage()
        {
            InitializeComponent();

            var licence = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{nameof(PatcherGUI)}.Content.agpl-3.0.rtf");
            richTextBox.Selection.Load(licence, DataFormats.Rtf);
        }

        public void PageShowed() { }
    }
}
