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

namespace Patcher
{
    public static class Logger
    {
        public static event Action<string> OnError;
        public static event Action<string> OnInfo;
        public static event Action<string> OnSuccess;

        public static void Info(string text = "")
        {
            OnInfo?.Invoke(text);
        }
        public static void InfoLine(string text = "")
        {
            Info(text + Environment.NewLine);
        }
        public static void Error(string text = "")
        {
            OnError?.Invoke(text);
        }
        public static void ErrorLine(string text = "")
        {
            Error(text + Environment.NewLine);
        }
        public static void Success(string text = "")
        {
            OnSuccess?.Invoke(text);
        }
        public static void SuccessLine(string text = "")
        {
            Success(text + Environment.NewLine);
        }
    }
}
