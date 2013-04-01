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
    using System;
    using System.Reflection;

    /// <summary>
    /// a module contains functionality or UI which the host application or another module can implement.
    /// 
    /// in most cases a module will be an assembly (dll).
    /// 
    /// each package will contain modules (dll's), such as import and expose modules.
    /// </summary>
    public class Module
    {
        private readonly int _hash;

        public Module(string name, Version version)
        {
            if (name == null) throw new ArgumentNullException("name");
            Name = name;
            Version = version == null ? new Version(0, 0, 0, 0) : version;
            
            //cache the hash
            var nameHash = Name.GetHashCode();
            //var versionHash = Version.GetHashCode();
            _hash = nameHash; // +versionHash;
        }

        public Module(AssemblyName assemblyName) :this(assemblyName.Name, assemblyName.Version)
        {
            if (assemblyName == null) throw new ArgumentNullException("assemblyName");
            AssemblyName = assemblyName;
        }
        
        /// <summary>
        /// the assemblies name, this is not guaranteed to have a value
        /// </summary>
        public virtual AssemblyName AssemblyName { get; private set; }

        /// <summary>
        /// The assembly version, this is used to provide extra meta data, but not to identify a package
        /// </summary>
        public virtual Version Version { get; private set; }
        
        /// <summary>
        /// The modules name, this is to uniquely identify the module
        /// </summary>
        public virtual string Name { get; private set; }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override bool Equals(object obj)
        {
            var module = obj as Module;
            if (module == null)
                return false;

            return GetHashCode() == module.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Module: {0}, Version {1}", Name, Version);
        }
    }
}