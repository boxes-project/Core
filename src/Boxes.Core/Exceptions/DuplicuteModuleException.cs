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
namespace Boxes.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A module is being exported by 2 packages
    /// </summary>
    public class DuplicuteModuleException : Exception
    {
        /// <summary>
        /// modules which have a circular dependency
        /// </summary>
        public IEnumerable<Package> Packages { get; private set; }

        /// <summary>
        /// Module which is being exported twice
        /// </summary>
        public Module Module { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        public DuplicuteModuleException(IEnumerable<Package> packages, Module module)
        {
            Packages = packages;
            Module = module;
        }

        public override string Message
        {
            get
            {
                return ToString();
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("The following Packages");
            foreach (var packages in Packages)
            {
                sb.AppendLine(packages.ToString());
            }
            sb.AppendLine("Both export: " + Module.Name);
            return sb.ToString();
        }


    }
}