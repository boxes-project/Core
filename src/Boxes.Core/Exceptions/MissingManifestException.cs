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
namespace Boxes.Exceptions
{
    using System;

    /// <summary>
    /// Have you copied the manifest file into your package directory?
    /// 
    /// raise this when the manifest file is missing.
    /// </summary>
    public class MissingManifestException : Exception
    {
        /// <summary>
        /// This package is missing its manifest file.
        /// </summary>
        public Package Package { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="package">the package with a missing manifest</param>
        public MissingManifestException(Package package)
        {
            Package = package;
        }

        public override string ToString()
        {
            return
                string.Format(
                    "{0} - has no defined manifest, ensure when compiling the package you always copy the manifest file",
                    Package);
        }
    }
}