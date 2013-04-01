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
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// raise this when there are missing import dependencies
    /// </summary>
    public class MissingImportsException : Exception
    {
        public IEnumerable<Module> DependenciesNotPresent { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="dependenciesNotPresent">the missing dependencies</param>
        public MissingImportsException(IEnumerable<Module> dependenciesNotPresent)
        {
            DependenciesNotPresent = dependenciesNotPresent;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("missing the following import dependencies");
            foreach (var module in DependenciesNotPresent)
            {
                sb.AppendLine(module.ToString());
            }

            return sb.ToString();
        }
    }
}