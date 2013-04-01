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
namespace Boxes.Discovering
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FileScanning;
    using Tasks;

    /// <summary>
    /// Default package scanner.
    /// </summary>
    public class PackageScanner : IPackageScanner
    {
        private readonly string[] _folders;
        private IBoxesTask<ScanContext> _manifestTask;
        private IBoxesTask<ScanContext> _assemblyTask;
        private readonly IList<IBoxesTask<ScanContext>> _boxesTasks;

        /// <summary>
        /// create a package scanner
        /// </summary>
        /// <param name="folders">pass in any root directory which contains packages</param>
        public PackageScanner(params string[] folders)
        {
            _folders = folders;
            var manifestTask = new XmlManifestTask(); //new XmlManifest2012Task();
            manifestTask.AddXmlManifestReader(new XmlManifest2012Reader());
            _manifestTask = manifestTask;
            _assemblyTask = new AssemblyPreLoadTask();
            _boxesTasks = new List<IBoxesTask<ScanContext>>();
        }

        /// <summary>
        /// supply the default manifest reading task (the default one is <see cref="XmlManifest2012Task"/>)
        /// </summary>
        /// <param name="manifestTask">the manifest task to use</param>
        public virtual void SetManifestTask(IBoxesTask<ScanContext> manifestTask)
        {
            if (manifestTask == null) throw new ArgumentNullException("manifestTask");
            _manifestTask = manifestTask;
        }

        /// <summary>
        /// supply the default assembly reading (Pre loading) task (the default one is <see cref="AssemblyPreLoadTask"/>)
        /// </summary>
        /// <param name="assemblyTask">the assembly task to use</param>
        public virtual void SetAssemblyTask(IBoxesTask<ScanContext> assemblyTask)
        {
            _assemblyTask = assemblyTask;
        }

        /// <summary>
        /// add extra scanning tasks, this allows you to handle files as they are discovered
        /// </summary>
        /// <param name="packageTask"></param>
        public virtual void RegisterTask(IBoxesTask<ScanContext> packageTask)
        {
            _boxesTasks.Add(packageTask);
        }

        /// <summary>
        /// returns all the installed packages
        /// </summary>
        public virtual IEnumerable<Package> Scan()
        {
            //create pipeline
            var createPackagePipiline = new PipilineExecutor<ScanContext>();
            createPackagePipiline.Register(_manifestTask);
            createPackagePipiline.Register(_assemblyTask);
            foreach (var boxesTask in _boxesTasks)
            {
                createPackagePipiline.Register(boxesTask);
            }

            //find package folders
            var packageDirectories = FindPackageDirectories();

            //this commented out code shows what the following block will do
            //however it will use yielding to maximise the memory usage
            //List<ScanContext> contexts = new List<ScanContext>();
            //foreach (var packageFolder in packageFolders)
            //{
            //    var package = new Package(packageFolder);
            //    var allFiles = new DirectoryScanner(packageFolder).FindFiles();
            //    contexts.AddRange( allFiles.Select(x => new ScanContext(x, package)));
            //}

            Func<string, IEnumerable<ScanContext>> getContexts =
                packageFolder =>
                {
                    var package = new Package(packageFolder);
                    var allFiles = ScannerToFindPackageFilesWith(packageFolder).FindFiles();
                    var ctxs = allFiles.Select(x => new ScanContext(x, package));
                    return ctxs;
                };

            IEnumerable<ScanContext> contexts = packageDirectories.SelectMany(getContexts);

            //Discover Packages
            IEnumerable<ScanContext> scanContexts = createPackagePipiline.Execute(contexts);

            return scanContexts.Select(x => x.Package).Distinct().ToList();
        }

        protected virtual IEnumerable<string> FindPackageDirectories()
        {
            return _folders.SelectMany(x => ScannerToFindPackageFoldersWith(x).FindDirectories());
        }

        protected virtual IScanner ScannerToFindPackageFilesWith(string packageDirectory)
        {
            return new DirectoryScanner(packageDirectory);
        }
        
        protected virtual IScanner ScannerToFindPackageFoldersWith(string rootDirectory)
        {
            return new OneDeepDiectoryScanner(rootDirectory);
        }
    }
}