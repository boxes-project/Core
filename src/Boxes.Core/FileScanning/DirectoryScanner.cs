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
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Scans all directories including sub directories
    /// </summary>
    public class DirectoryScanner : IScanner
    {
        private readonly string _baseDirectory;

        public DirectoryScanner(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public virtual IEnumerable<File> FindFiles()
        {
            return FindFiles(_baseDirectory);
        }

        public virtual IEnumerable<string> FindDirectories()
        {
            return FindFolders(_baseDirectory);
        }

        protected virtual IEnumerable<string>  FindFolders(string currentDirectory)
        {
            var dir = new DirectoryInfo(currentDirectory);
            var folders = dir.GetDirectories();
            foreach (var folder in folders)
            {
                yield return folder.FullName;

                foreach (var subFolder in FindFolders(folder.FullName))
                {
                    yield return subFolder;
                }
            }
        }

        protected virtual IEnumerable<File> FindFiles(string currentDirectory)
        {
            var dir = new DirectoryInfo(currentDirectory);

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                yield return CreateFile(file);
            }

            foreach (var subDir in dir.GetDirectories())
            {
                foreach (var subFile in FindFiles(subDir.FullName))
                {
                    yield return subFile;
                }
            }
        }

        protected virtual File CreateFile(FileInfo file)
        {

            Func<string> fileTextContents = () => System.IO.File.ReadAllText(file.FullName);
            Func<byte[]> fileContents = () => System.IO.File.ReadAllBytes(file.FullName);

            return new File(file.FullName, file.Name, file.DirectoryName, fileTextContents, fileContents);
        }
    }
}
