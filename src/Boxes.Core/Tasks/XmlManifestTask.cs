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
namespace Boxes.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Linq;
    using Exceptions;

    /// <summary>
    /// task to read an xml manifest file.
    /// </summary>
    public class XmlManifestTask : FileTaskBase
    {
        private readonly Func<ScanContext, bool> _isLike;
        private readonly IDictionary<XNamespace, XmlManifestReader> _xmlManifestReaders 
            = new Dictionary<XNamespace, XmlManifestReader>();

        /// <summary>
        /// add a reader to run during the loading of a XML manifest file
        /// </summary>
        /// <param name="reader"></param>
        public void AddXmlManifestReader(XmlManifestReader reader)
        {
            _xmlManifestReaders.Add(reader.BoxesNs, reader);
        }
        
        public override bool CanHandle(ScanContext item)
        {
            return _isLike(item); 
        }

        public override void Execute(ScanContext context)
        {
            var content = context.File.ContentsAsText;
            XElement manifestXml = XElement.Parse(content);
            var defaultNamespace = manifestXml.GetDefaultNamespace();

            XmlManifestReader reader;
            if(!_xmlManifestReaders.TryGetValue(defaultNamespace, out reader))
            {
                throw new XmlManifestNotSupportedException(context.File.FullName, defaultNamespace);
            }

            var manifest = reader.ReadManifest(manifestXml);
            context.Package.SetManifest(manifest);
        }

        public XmlManifestTask()
        {
            _isLike = IsLike("manifest.xml");
        }
    }
}