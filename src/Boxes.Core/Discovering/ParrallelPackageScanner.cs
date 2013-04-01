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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// scans the packages in parallel.
    /// </summary>
    public class ParrallelPackageScanner : PackageScanner
    {
        private readonly string[] _folders;

        /// <summary>
        /// create a package scanner
        /// </summary>
        /// <param name="folders">pass in any root directory which contains packages</param>
        public ParrallelPackageScanner(params string[] folders) : base(folders)
        {
            _folders = folders;
        }

        protected override IEnumerable<string> FindPackageDirectories()
        {
            return _folders.SelectMany(x => base.ScannerToFindPackageFoldersWith(x).FindDirectories()).AsParallel();
        }
    }
}