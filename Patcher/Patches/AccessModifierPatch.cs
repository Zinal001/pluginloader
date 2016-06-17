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
using System.Linq;
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace
#pragma warning restore CC0065 // Remove trailing whitespace

namespace Patcher.Patches
{
    internal class AccessModifierPatch : Patch
    {
        public AccessModifierPatch(PatcherData patcherData) : base(patcherData)
        {
            allTypesPublic();
            allNotPublicMethodsProtected();
            allPrivateFieldsProtected();
        }

        private void allNotPublicMethodsProtected()
        {
            patcherData.TargetData[PatcherTarget.InterstellarRift].Module.GetTypes().ToList().ForEach(type =>
                type.Methods.ToList().ForEach(method => {
                if ((method.IsPrivate || method.IsAssembly || method.IsFamilyAndAssembly))
                    method.IsFamily = true;
            }));
        }

        private void allPrivateFieldsProtected()
        {
            patcherData.TargetData[PatcherTarget.InterstellarRift].Module.GetTypes().ToList().ForEach(type =>
                type.Fields.ToList().ForEach(field => {
                if (field.IsPrivate)
                    field.IsFamily = true;
            }));
        }

        private void allTypesPublic()
        {
            patcherData.TargetData[PatcherTarget.InterstellarRift].Module.GetTypes().ToList().ForEach(type => {
                if (!type.IsNested)
                    type.IsPublic = true;
                else
                    type.IsNestedPublic = true;
            });
        }
    }
}