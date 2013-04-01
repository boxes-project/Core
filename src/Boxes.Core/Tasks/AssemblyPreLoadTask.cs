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
    using System.Reflection;

    /// <summary>
    /// Adds an assembly to the package
    /// </summary>
    public class AssemblyPreLoadTask : FileTaskBase
    {
        public override bool CanHandle(ScanContext item)
        {
            return item.File.Name.EndsWith(".dll");
        }

        public override void Execute(ScanContext context)
        {
            var name = AssemblyName.GetAssemblyName(context.File.FullName);
            context.Package.AddAssembly(name, context.File);
        }
    }
}