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

    /// <summary>
    /// loads a package in another app domain
    /// </summary>
    [Obsolete("testing an idea")]
    internal class SeperateAppDomainLoader : LoaderBase
    {
        private readonly AppDomain _appDomain;

        public SeperateAppDomainLoader(AppDomain appDomain)
        {
            _appDomain = appDomain;
        }

        public override void LoadPackage(Package package)
        {
            CrossAppDomainDelegate loadPackage = () => base.LoadPackage(package);
            _appDomain.DoCallBack(loadPackage);
        }
    }
}