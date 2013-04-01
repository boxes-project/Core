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
namespace Boxes.Loading
{
    using System;
    using System.Reflection;
    using Module = Boxes.Module;

    /// <summary>
    /// loads a package's internal dependencies into the same AppDomain
    /// </summary>
    /// <remarks>
    /// this allows 
    ///     for package A to depend on Log4Net 1.2
    ///     for package B to depend on Log4Net 2.0
    ///     for package C to depend on Log4Net 1.2
    /// 
    /// and each Log4Net will be loaded individually into the AppDomain
    /// </remarks>
    public class IsolatedLoader : LoaderBase, IDisposable
    {
        private readonly PackageRegistry _packageRegistry;

        /// <summary>
        /// create a isolated loader, pass in an instance
        /// </summary>
        /// <param name="packageRegistry">the package registry to work with</param>
        public IsolatedLoader(PackageRegistry packageRegistry)
        {
            _packageRegistry = packageRegistry;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainAssemblyResolve;
        }

        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var required = new AssemblyName(args.Name);

            //you can only export a single version of an assembly
            //the internal dependency can have a different version.
            
            //the fastest order should be: importing, internal, exporting
            //the exporting should already be loaded.

            //importing
            Assembly alreadyLoaded;
            if (Loaded.TryGetValue(new Module(required), out alreadyLoaded))
            {
                return alreadyLoaded;
            }
            
            Package package;
            //try internal
            AssemblyName requestor = args.RequestingAssembly.GetName();
            package = _packageRegistry.GetPackageExposing(requestor);
            if (package != null)
            {
                var internalAssembly = package.GetInternalAssembly(required);
                internalAssembly.LoadFromFile();
                return internalAssembly.Assembly;    
            }

            //try exposing (this one should really not need to be run)
            package = _packageRegistry.GetPackageExposing(required);
            if (package != null)
            {
                var assemblyReference = package.GetInternalAssembly(required);
                assemblyReference.LoadFromFile();
                return assemblyReference.Assembly;
            }

            return null;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomainAssemblyResolve;
        }
    }
}