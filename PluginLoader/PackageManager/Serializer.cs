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
using Newtonsoft.Json;

namespace PluginLoader
{
    public static class JsonSerializer
    {
        public static JsonConverter[] Converters => new JsonConverter[] {
            new VersionConverter()
        };
        public static T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data, Converters);
        }
        public static string Serialize(object data)
        {
            return JsonConvert.SerializeObject(data, Converters);
        }
    }
}
