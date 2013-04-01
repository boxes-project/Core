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
namespace Boxes.Dependencies
{
    using System.Collections.Generic;

    /// <summary>
    /// this tries to see if a module (or even a package) can be loaded.
    /// </summary>
    /// <remarks>
    /// as dependencies are detailed in a manifest file, however these are at package level.
    /// </remarks>
    public class DependencyMatrix
    {
        readonly IDictionary<Module, DependencyModule> _dependencies = new Dictionary<Module, DependencyModule>();

        public void AddPackage(Package package)
        {
            //add exported first
            foreach (var export in package.Manifest.Exports)
            {
                DependencyModule dependencyModule = null;
                if(!_dependencies.TryGetValue(export, out dependencyModule))
                {
                    dependencyModule = new DependencyModule(export);
                    _dependencies.Add(export, dependencyModule);
                }
                dependencyModule.ContainedInPackage = package;
            }

            //imported
            foreach (var import in package.Manifest.Imports)
            {
                DependencyModule dependencyModule = null;
                if (!_dependencies.TryGetValue(import, out dependencyModule))
                {
                    dependencyModule = new DependencyModule(import);
                    _dependencies.Add(import, dependencyModule);
                }
                dependencyModule.AddRequiredByPackage(package);
            }
        }
    }
}