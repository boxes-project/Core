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
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// xml reader, will read the default format
    /// </summary>
    /// <code>
    /// <pre>
    /// <manifest xmlns="http://DefineANamespaceHere">
    ///     <name>Acme.Test</name>
    ///     <version>1.0</version>
    ///     <description>this is an awesome module which will add cool features</description>
    ///     <exports>
    ///         <assembly name="Acme.Test.Bus" version="3.1.0" />
    ///         <assembly name="Acme.Test.Core" />
    ///     </exports>
    ///     <imports>
    ///         <dependency name="BlueSky.Ioc" version="3.1.0" />
    ///         <dependency name="Acme.Data" />
    ///     </imports>
    /// </manifest>
    /// </pre>
    /// </code>
    public abstract class XmlManifestReader
    {
        /// <summary>
        /// namespace which the reader supports
        /// </summary>
        public abstract XNamespace BoxesNs { get; }

        /// <summary>
        /// reads an xml element into a manifest
        /// </summary>
        /// <param name="manifestXml">the xml element which contains the manifest details</param>
        /// <returns>a fully populated manifest</returns>
        public virtual Manifest ReadManifest(XElement manifestXml)
        {
            var name = GetValue(manifestXml, "name");
            var description = GetValue(manifestXml, "description");

            var temp = GetValue(manifestXml, "version");
            var version = temp == "" ? null : new Version(temp);

            var imports = GetModules(manifestXml, "imports", "dependency");
            var exports = GetModules(manifestXml, "exports", "assembly");

            return CreateManifestInstance(name, version, description, exports, imports);
        }

        protected virtual Manifest CreateManifestInstance(string name, Version version, string description, IEnumerable<Module> exports, IEnumerable<Module> imports)
        {
            //hmmm, trying to make this bit extendable (plus it will not be run in any order of magnitude)
            //however we can override it to provide the Manifest type in any child class
            return (Manifest)Activator.CreateInstance(typeof(Manifest), name, version, description, exports, imports);
        }

        protected string GetValue(XElement xElement, string nodeName)
        {
            var element = xElement.Descendants(BoxesNs + nodeName).FirstOrDefault();
            return element == null ? "" : element.Value;
        }

        protected IEnumerable<Module> GetModules(XElement xElement, string parentNode, string childNodes)
        {
            var element = xElement.Descendants(BoxesNs + parentNode).FirstOrDefault();
            if (element == null) return new List<Module>();

            Func<XElement, Module> createModule =
                node =>
                {
                    var nameAttr = node.Attribute("name");
                    var versionAttr = node.Attribute("version");

                    var name = nameAttr == null ? "" : nameAttr.Value;
                    var version = versionAttr == null ? null : new Version(versionAttr.Value);

                    return new Module(name, version);
                };

            return element.Descendants(BoxesNs + childNodes).Select(createModule);
        }
    }
}