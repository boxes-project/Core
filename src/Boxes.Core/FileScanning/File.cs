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
    using System;

    /// <summary>
    /// contains information about a file
    /// </summary>
    public class File
    {
        private readonly Func<string> _getTextContents;
        private readonly Func<byte[]> _getByteContents;

        public File(string fullName, string name, string location,
                    Func<string> getTextContents, Func<byte[]> getByteContents)
        {
            _getTextContents = getTextContents;
            _getByteContents = getByteContents;
            FullName = fullName;
            Name = name;
            Location = location;
        }

        /// <summary>
        /// the full name, directory + name
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// the name of the file
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// the directory of the file
        /// </summary>
        public string Location { get; private set; }
        
        /// <summary>
        /// the file contents as a string
        /// </summary>
        public string ContentsAsText
        {
            get
            {
                return _getTextContents();
            }
        }

        /// <summary>
        /// the file contents in a byte[]
        /// </summary>
        public byte[] ContentsAsBytes
        {
            get
            {
                return _getByteContents();
            }
        }
    }
}