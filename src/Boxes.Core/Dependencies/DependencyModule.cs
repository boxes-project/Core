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
    /// contains information about a modules dependency (other packages)
    /// </summary>
    /// <remarks>
    /// a package can expose many modules and depend on many modules
    /// 
    /// this makes a module have multiple package dependencies
    /// </remarks>
    public class DependencyModule
    {
        private readonly List<Package> _requiredByPackages = new List<Package>();
        private Package _containedInPackage;

        public DependencyModule(Module requiredModule)
        {
            RequiredModule = requiredModule;
        }
        
        public Module RequiredModule { get; private set; }
        public IEnumerable<Package> RequiredByPackages { get { return _requiredByPackages; } }
        public Package ContainedInPackage
        {
            get { return _containedInPackage; }
            set
            {
                _containedInPackage = value;
                if (_containedInPackage.CanLoad)
                {
                    UpdateDependentPackages();
                }
                else
                {
                    _containedInPackage.OnReadyToLoad += OnPackageOnReadyToLoad;
                }
            }
        }

        private void OnPackageOnReadyToLoad(Package package)
        {
            UpdateDependentPackages();
        }

        private void UpdateDependentPackages()
        {
            //notify all the dependant packages
            foreach (var requiredByPackage in _requiredByPackages)
            {
                requiredByPackage.DependencyDiscovered(RequiredModule);
            }
        }


        public void AddRequiredByPackage(Package package)
        {
            _requiredByPackages.Add(package);
            if (ContainedInPackage == null) return;
            //its already loaded in, let the dependant know of this.
            package.DependencyDiscovered(RequiredModule);
        }
    }
}