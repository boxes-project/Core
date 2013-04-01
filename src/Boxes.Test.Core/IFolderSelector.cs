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
    using System.IO;

    public interface IFolderSelector
    {
        bool Filter(DirectoryInfo dir);
        string GetKey(DirectoryInfo dir);
    }

    public class FolderSelector : IFolderSelector 
    {
        private readonly Func<DirectoryInfo, bool> _filter;
        private readonly Func<DirectoryInfo, string> _getKey;

        public FolderSelector(Func<DirectoryInfo, bool> filter, Func<DirectoryInfo, string> getKey)
        {
            _filter = filter;
            _getKey = getKey;
        }

        public bool Filter(DirectoryInfo dir)
        {
            return _filter(dir);
        }

        public string GetKey(DirectoryInfo dir)
        {
            return _getKey(dir);
        }
    }
}