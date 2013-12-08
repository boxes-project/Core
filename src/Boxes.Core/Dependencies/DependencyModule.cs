// Copyright 2012 - 2013 dbones.co.uk
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
    using System.Linq;
    using Exceptions;

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

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="requiredModule">the module which is being exposed</param>
        public DependencyModule(Module requiredModule)
        {
            RequiredModule = requiredModule;
        }
        
        /// <summary>
        /// the Module which is being expose by a package, or is being imported by another package
        /// </summary>
        public Module RequiredModule { get; private set; }

        /// <summary>
        /// the packages which require the module
        /// </summary>
        public IEnumerable<Package> RequiredByPackages { get { return _requiredByPackages; } }
        
        /// <summary>
        /// The package which owns/exports the module 
        /// </summary>
        public Package ContainedInPackage
        {
            get { return _containedInPackage; }
            set
            {
                if (_containedInPackage != null)
                {
                    throw new DuplicuteModuleException(new []{ _containedInPackage, value }, RequiredModule);
                }

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
            _containedInPackage.OnReadyToLoad -= OnPackageOnReadyToLoad;
            UpdateDependentPackages();
        }

        private void UpdateDependentPackages()
        {
            //notify all the dependant packages
            foreach (var requiredByPackage in _requiredByPackages)
            {
                Check(requiredByPackage);
                requiredByPackage.DependencyDiscovered(RequiredModule);
            }
        }

        /// <summary>
        /// Add a package which depends on this module
        /// </summary>
        /// <param name="package">the package which depends on the module</param>
        public void AddRequiredByPackage(Package package)
        {
            _requiredByPackages.Add(package);
            if (ContainedInPackage == null) return;
            //its already loaded in, let the dependant know of this.
            Check(package);
            package.DependencyDiscovered(RequiredModule);
        }

        /// <summary>
        /// Check for a circular dependency
        /// </summary>
        /// <param name="package">the depending package</param>
        private void Check(Package package)
        {
            //ContainedInPackage = p1
            //package = p2

            //p1 -> m1 and p1 <- m2 
            //p2 -> m2 and p2 <- m1
            
            //we know that the latter is true.
            //we need to test for the former

            if (package.Manifest.Exports.Any(export => ContainedInPackage.Manifest.Imports.Contains(export)))
            {
                throw new CircularDependencyException(new [] { package, ContainedInPackage });
            }
        }

    }
}