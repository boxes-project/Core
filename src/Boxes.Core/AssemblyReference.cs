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
    using System.Reflection;
    using FileScanning;

    /// <summary>
    /// contains information about an assembly, this is required to load it into the app domain
    /// note this is used to lazy load the assembly.
    /// </summary>
    public class AssemblyReference
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="file">the associated file</param>
        /// <param name="assemblyName">the assemblies name</param>
        public AssemblyReference(File file, AssemblyName assemblyName)
        {
            File = file;
            AssemblyName = assemblyName;
            Module = new Module(assemblyName);
        }

        /// <summary>
        /// the assemblies file
        /// </summary>
        public File File { get; private set; }
        
        /// <summary>
        /// the assemblies name
        /// </summary>
        public AssemblyName AssemblyName { get; private set; }
        
        /// <summary>
        /// the actual assembly, once it has been loaded.
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// the assemblies module information
        /// </summary>
        public Module Module { get; private set; }

        /// <summary>
        /// load the assembly from the file
        /// </summary>
        public void LoadFromFile()
        {
            if (Assembly== null)
            {
                Assembly = Assembly.Load(File.ContentsAsBytes);
            }
        }

        /// <summary>
        /// provide an instance of the assembly to associate with this assemblyReference
        /// </summary>
        /// <param name="assembly">the instance of the assembly</param>
        public void AssociateWithExisting(Assembly assembly)
        {
            Assembly = assembly;
        }

        public override string ToString()
        {
            if (Assembly != null)
            {
                return Assembly.GetName().Name;
            }
            return string.Format("{0} - not loaded", File.Name);
        }
    }
}