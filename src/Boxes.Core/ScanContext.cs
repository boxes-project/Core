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
    using FileScanning;

    /// <summary>
    /// this contains information about a file during discovery
    /// </summary>
    public class ScanContext
    {
        public ScanContext(File file, Package package)
        {
            if (file == null) throw new ArgumentNullException("file");
            if (package == null) throw new ArgumentNullException("package");
            
            File = file;
            Package = package;
        }

        /// <summary>
        /// the package the file belongs too
        /// </summary>
        public virtual Package Package { get; private set; }

        /// <summary>
        /// the file in the discovery context
        /// </summary>
        public virtual File File { get; private set; }

        public override string ToString()
        {
            return string.Format("ScanContext for Package: {0}, \tFile: {1}", Package.Name, File.Name);
        }
    }
}