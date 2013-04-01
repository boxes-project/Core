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
namespace Boxes.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class WorkspaceContext : IDisposable
    {
        public string WorkingDirectory { get; private set; }
        public string SlnDirectory { get; private set; }
        private readonly IDictionary<string, DirectoryInfo> _registeredDirectories = new Dictionary<string, DirectoryInfo>();
        private static Random rnd = new Random();


        private WorkspaceContext()
        {
                        
        }

        public static WorkspaceContext CreateFromSlnName(string slnName)
        {
            var location = Assembly.GetExecutingAssembly().CodeBase;
            location = location.Replace("file:///", "");
            location = location.Replace("/", "\\");

            location = Path.GetDirectoryName(location);

            DirectoryInfo dir = new DirectoryInfo(location);
            while (!dir.GetFiles(slnName).Any())
            {
                dir = dir.Parent;
            }
            int postfix = rnd.Next(100, 999);
            var workingDir = Path.Combine(Path.GetTempPath(), "boxes_" + Path.GetRandomFileName() + postfix);
            Directory.CreateDirectory(workingDir);

            return CreateFromSlnAndWorkingDirectories(dir.FullName, workingDir);

        }

        public static WorkspaceContext CreateFromSlnAndWorkingDirectories(string slnDir, string workingDir)
        {
            return new WorkspaceContext {SlnDirectory = slnDir, WorkingDirectory = workingDir};
        }

        public void Scan(IFolderSelector selector)
        {
            var directories = Directory.GetDirectories(SlnDirectory).Select(x => new DirectoryInfo(x)).ToList();
            foreach (var directoryInfo in directories)
            {
                if (!selector.Filter(directoryInfo))
                    continue;

                var binDirectory = BinDirectory(directoryInfo);
                if (binDirectory == null)
                    continue;


                _registeredDirectories.Add(selector.GetKey(directoryInfo), binDirectory);
            }
        }

        public DirectoryInfo BinDirectory(DirectoryInfo projectDir)
        {
            string binFolder;
#if DEBUG
            binFolder = "Debug";
#endif
#if RELEASE
            binFolder = "Release";
#endif

            var bin = projectDir.GetDirectories("bin").FirstOrDefault();
            if (bin == null) return null;

            return bin.GetDirectories(binFolder).FirstOrDefault();

        }

        public void CopyFolder(string key, string subDir)
        {
            var registeredDirectory = _registeredDirectories[key];
            var dest = string.IsNullOrWhiteSpace(subDir)
                ? Path.Combine(WorkingDirectory, key)
                : Path.Combine(WorkingDirectory, subDir, key);
            DirectoryCopy(registeredDirectory.FullName, dest, true);
        }


        public void Dispose()
        {
            if (Directory.Exists(WorkingDirectory))
            {
                Directory.Delete(WorkingDirectory, true);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}