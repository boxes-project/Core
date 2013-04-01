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
namespace Boxes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Dependencies;
    using Discovering;
    using Exceptions;
    using Loading;

    /// <summary>
    /// This is the entry point of the application. it exposes functions to
    /// Discover and Load packages
    /// </summary>
    /// <code>
    /// <pre>
    ///     PackageRegistry packageRegistry = new PackageRegistry();
    ///     packageRegistry.DiscoverPackages(new PackageScanner("some base dir"));
    ///     using (var loader = new IsolatedLoader(packageRegistry))
    ///     {
    ///         packageRegistry.LoadPackages(loader);    
    ///     }
    /// </pre>
    /// </code>
    public class PackageRegistry
    {
        private readonly ICollection<Package> _packages = new HashSet<Package>();
        private readonly IDictionary<Module, Package> _exposedInPackage = new Dictionary<Module, Package>(); 
        private readonly DependencyMatrix _dependencyMatrix = new DependencyMatrix();

        /// <summary>
        /// The packages which have been found
        /// </summary>
        public virtual IEnumerable<Package> Packages { get { return _packages; } }

        /// <summary>
        /// Find packages, this will update the packages property, with details on if a Package can be loaded
        /// </summary>
        /// <param name="packageScanner">the package scanner to use</param>
        public virtual void DiscoverPackages(IPackageScanner packageScanner)
        {
            var packages = packageScanner.Scan();
            foreach (var package in packages) 
            {
                AddPackage(package);
            }
        }

        /// <summary>
        /// loads all the packages which have their dependences satisfied. 
        /// </summary>
        /// <param name="loader">the loader to apply when loading the package's assemblies into the AppDomain</param>
        public virtual void LoadPackages(ILoader loader)
        {
            var loadablePackages = _packages.Where(x => x.CanLoad && !x.Loaded);
            foreach (var package in loadablePackages)
            {
                loader.LoadPackage(package);
            }
        }

        internal virtual void AddPackage(Package package)
        {
            if (_packages.Contains(package)) return;

            if (package.Manifest == null)
            {
                throw new MissingManifestException(package);
            }
    
            _dependencyMatrix.AddPackage(package);
            _packages.Add(package);
            foreach (var module in package.Manifest.Exports)
            {
                _exposedInPackage.Add(module, package);
            }
        }

        internal virtual Package GetPackageExposing(AssemblyName assemblyName)
        {
            Package package;
            _exposedInPackage.TryGetValue(new Module(assemblyName), out package);
            return package;
        }
    }
}