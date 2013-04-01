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
namespace Boxes.FileScanning
{
    using System.Collections.Generic;

    /// <summary>
    /// Scanner used to find files and directories
    /// </summary>
    public interface IScanner
    {
        /// <summary>
        /// find the files
        /// </summary>
        /// <returns>returns all the files</returns>
        IEnumerable<File> FindFiles();

        /// <summary>
        /// find directories
        /// </summary>
        /// <returns>returns the locations of directories</returns>
        IEnumerable<string> FindDirectories();
    }
}