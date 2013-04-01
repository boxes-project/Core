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
    using System.Xml.Linq;

    /// <summary>
    /// raise this when there there is no xml reader to support the current namespace
    /// </summary>
    public class XmlManifestNotSupportedException : Exception
    {
        private readonly string _fileName;
        private readonly XNamespace _ns;

        public XmlManifestNotSupportedException(string fileName, XNamespace ns)
        {
            _fileName = fileName;
            _ns = ns;
        }

        public override string ToString()
        {
            return string.Format("Cannot read {0}, as there was no reader registered for the following namespace: {1}", _fileName, _ns);
        }
    }
}