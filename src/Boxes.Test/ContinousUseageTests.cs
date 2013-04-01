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
    using System.Linq;
    using Discovering;
    using Infrastructure;
    using Loading;
    using NUnit.Framework;

    public class ContinousUseageTests : TestBase<PackageRegistry>
    {
        [Test]
        public void RunLoadPackagesTwice()
        {
            CopyPackage("test.box1");
            CopyPackage("test.box2");
            Arrange(
                () =>
                {
                    string path = TestInfo.ModulesDirectory;
                    var registry = new PackageRegistry();
                    registry.DiscoverPackages(new PackageScanner(path));
                    ILoader loader = new DefaultLoader();

                    dynamic ctx = new Context<PackageRegistry>(registry);
                    ctx.Loader = loader;
                    
                    ctx.Sut.LoadPackages(loader);

                    return ctx;

                });

            Action(ctx => ctx.Sut.LoadPackages(((dynamic)ctx).Loader));
            Assert(ctx => ctx.Sut.Packages.Count() == 2);
            Assert(ctx => ctx.Sut.Packages.SelectMany(x => x.LoadedAssemblies).Count() == 2);
            Assert(ctx => AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name.ToLower().Contains("test.box")) == 2);
            Execute();
        }

        [Test]
        public void RunDiscoverPackagesTwice()
        {
            CopyPackage("test.box1");
            CopyPackage("test.box2");
            Arrange(
                () =>
                {
                    string path = TestInfo.ModulesDirectory;
                    var registry = new PackageRegistry();
                    registry.DiscoverPackages(new PackageScanner(path));
                    registry.DiscoverPackages(new PackageScanner(path));
                    return new Context<PackageRegistry>(registry);
                });

            Action(ctx => ctx.Sut.LoadPackages(new DefaultLoader()));
            Assert(ctx => ctx.Sut.Packages.Count() == 2);
            Assert(ctx => ctx.Sut.Packages.SelectMany(x => x.LoadedAssemblies).Count() == 2);
            Assert(ctx => AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name.ToLower().Contains("test.box")) == 2);
            Execute();
        }

    }
}