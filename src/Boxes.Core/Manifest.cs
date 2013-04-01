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
    using System.Collections.Generic;

    /// <summary>
    /// contains information about a Package
    /// </summary>
    public class Manifest
    {
        private readonly ICollection<Module> _exports = new HashSet<Module>();
        private readonly ICollection<Module> _imports = new HashSet<Module>();

        public Manifest(string name, Version version, string description, 
            IEnumerable<Module> exports, IEnumerable<Module> imports)
        {
            Name = name;
            Version = version;
            Description = description;

            _exports.AddRange(exports);
            _imports.AddRange(imports);
        }

        /// <summary>
        /// Name of the package, recommend a full name such as [CompanyName or Product].[Package] i.e. Dbones.ApplicationBus
        /// </summary>
        public virtual string Name { get; private set; }

        /// <summary>
        /// the version of the package
        /// </summary>
        public virtual Version Version { get; private set; }

        /// <summary>
        /// a description of the package, what does it do
        /// </summary>
        public virtual string Description { get; private set; }

        /// <summary>
        /// the modules exposed in this package
        /// </summary>
        public virtual IEnumerable<Module> Exports { get { return _exports; } }

        /// <summary>
        /// the modules this package requires to run
        /// </summary>
        public virtual IEnumerable<Module> Imports { get { return _imports; } }
    }
}