// Copyright 2012 - 2013 dbones.co.uk (David Rundle)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
namespace Boxes.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class Util
    {
        public static IEnumerable<Assembly> AllLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        public static IEnumerable<Assembly> BoxesLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetName().Name.ToLower().Contains("test.box"));
        }

        /// <summary>
        /// because .Net lazy loads assemblies.
        /// </summary>
        /// <param name="packageRegistry"></param>
        public static void ForceLoadOfInternals(PackageRegistry packageRegistry)
        {
            packageRegistry
                .Packages
                .SelectMany(x => x.LoadedAssemblies)
                .Select(x => x.GetExportedTypes())
                .Force();
        }
    }
}