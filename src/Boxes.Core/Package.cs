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
    using Exceptions;
    using FileScanning;

    /// <summary>
    /// raised when a package is ready to load
    /// </summary>
    /// <param name="package"></param>
    public delegate void PackageReadyToLoad(Package package);

    /// <summary>
    /// Package contains all the elements to load an extension into the application
    /// </summary>
    public class Package 
    {
        private readonly IDictionary<Module, AssemblyReference> _assemblies;
        private readonly ICollection<Module> _dependenciesNotPresent;
        private readonly int _hash;

        public Package(Manifest manifest, string location) : this(location)
        {
            Manifest = manifest;
            Name = manifest.Name;
            
        }

        public Package(string location)
        {
            _assemblies = new Dictionary<Module, AssemblyReference>();
            _dependenciesNotPresent = new HashSet<Module>();
            Location = location;
            Loaded = false;
            CanLoad = false;
            _hash = location.GetHashCode();
            OnReadyToLoad += delegate {  };
        }

        /// <summary>
        /// Raised when this package is ready to load.
        /// </summary>
        public event PackageReadyToLoad OnReadyToLoad;


        /// <summary>
        /// name of the package
        /// </summary>
        public virtual string Name { get; private set; }
        
        /// <summary>
        /// the manifest for the package
        /// </summary>
        public virtual Manifest Manifest { get; private set; }

        /// <summary>
        /// location of the package
        /// </summary>
        public virtual string Location { get; private set; }

        /// <summary>
        /// if the modules have been loaded
        /// </summary>
        public virtual bool Loaded { get; internal set; }
        
        /// <summary>
        /// false if there are dependencies which are not found
        /// </summary>
        public virtual bool CanLoad { get; private set; }

        /// <summary>
        /// exposed assemblies which have been loaded
        /// </summary>
        public virtual IEnumerable<Assembly> LoadedAssemblies
        {
            get
            {
                if (!Loaded)
                {
                    return new List<Assembly>();
                }

                //TODO: do we want to return all the loaded assemblies.
                return _assemblies.Where(
                            assemblies => Manifest.Exports.Any(exported => exported.Equals(assemblies.Key)))
                        .Select(assemblyEntry => assemblyEntry.Value)
                        .Select(assemblyReference => assemblyReference.Assembly);
            }
        }

        /// <summary>
        /// load this modules in this package
        /// </summary>
        public virtual IEnumerable<AssemblyReference> AssembliesToLoad()
        {
            if (!CanLoad) throw new MissingImportsException(_dependenciesNotPresent);

            return _assemblies.Where(
                    assemblies => Manifest.Exports.Any(exported => exported.Equals(assemblies.Key)))
                .Select(assemblyEntry=>assemblyEntry.Value);
        }

        /// <summary>
        /// try and get an internal reference of an assembly in this package
        /// </summary>
        /// <param name="name">assembly name to try an retrieve</param>
        /// <returns>the instance of the assembly</returns>
        public virtual AssemblyReference GetInternalAssembly(AssemblyName name)
        {
            AssemblyReference result;
            _assemblies.TryGetValue(new Module(name), out result);
            return result;
        }

        /// <summary>
        /// try and get an internal reference of an assembly in this package
        /// </summary>
        /// <param name="module">the module</param>
        /// <returns></returns>
        public virtual AssemblyReference GetInternalAssembly(Module module)
        {
            AssemblyReference result;
            _assemblies.TryGetValue(module, out result);
            return result;
        }

        /// <summary>
        /// sets the manifest for this package
        /// </summary>
        /// <param name="manifest">manifest</param>
        internal virtual void SetManifest(Manifest manifest)
        {
            Manifest = manifest;
            Name = manifest.Name;
            foreach (var dependency in Manifest.Imports)
            {
                _dependenciesNotPresent.Add(dependency);
            }
            
            if (_dependenciesNotPresent.Count == 0)
            {
                CanLoad = true;
                //OnReadyToLoad(this);
            }
        }

        /// <summary>
        /// update this package when one of its dependencies have been discovered
        /// </summary>
        /// <param name="dependency"></param>
        internal virtual void DependencyDiscovered(Module dependency)
        {
            _dependenciesNotPresent.Remove(dependency);
            if (_dependenciesNotPresent.Count == 0)
            {
                CanLoad = true;
                OnReadyToLoad(this);
            }
        }

        /// <summary>
        /// add an assembly to the package, this can be an exposed or internal (if the assembly is ready)
        /// </summary>
        /// <param name="name">name of the assembly</param>
        /// <param name="file">the assembly</param>
        internal virtual void AddAssembly(AssemblyName name, File file)
        {
            _assemblies.Add(new Module(name), new AssemblyReference(file, name));
        }

        public override bool Equals(object obj)
        {
            Package package = obj as Package;
            if (package == null) return false;
            return GetHashCode() == package.GetHashCode();
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override string ToString()
        {
            if (Manifest == null)
            {
                return base.ToString();
            }

            return _dependenciesNotPresent.Count > 0
                ? string.Format("Package: {0}, Loaded: {1}, Missing {2} dependencies", Name, Loaded, _dependenciesNotPresent.Count)
                : string.Format("Package: {0}, Loaded: {1}", Name, Loaded);
        }
    }
}