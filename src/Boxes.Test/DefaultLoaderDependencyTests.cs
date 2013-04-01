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

    public class DefaultLoaderDependencyTests : TestBase<PackageRegistry>
    {
        [Test]
        public void SimpleMissingDependency()
        {
            CopyPackage("test.box3_1");
            Arrange(
                () =>
                {
                    string path = TestInfo.ModulesDirectory;
                    var registry = new PackageRegistry();
                    registry.DiscoverPackages(new PackageScanner(path));
                    return new Context<PackageRegistry>(registry);
                });

            Action(ctx => ctx.Sut.LoadPackages(new DefaultLoader()));
            Assert(ctx => ctx.Sut.Packages.Count() == 1);
            Assert(ctx => ctx.Sut.Packages.Select(x => !x.CanLoad).Count() == 1);
            Assert(ctx => !ctx.Sut.Packages.SelectMany(x => x.LoadedAssemblies).Any());
            Execute();
        }

        [Test]
        public void SimpleDependency()
        {
            CopyPackage("test.box3_1");
            CopyPackage("test.box1");
            CopyPackage("test.box2");
            Arrange(
                () =>
                {
                    string path = TestInfo.ModulesDirectory;
                    var registry = new PackageRegistry();
                    registry.DiscoverPackages(new PackageScanner(path));
                    return new Context<PackageRegistry>(registry);
                });

            Action(ctx =>
                   {
                       ctx.Sut.LoadPackages(new DefaultLoader());
                       Util.ForceLoadOfInternals(ctx.Sut);
                   });

            Assert(ctx => ctx.Sut.Packages.Count() == 3);
            Assert(ctx => ctx.Sut.Packages.SelectMany(x => x.LoadedAssemblies).Count() == 3);
            Assert(ctx => AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name.ToLower().Contains("test.box")) == 3);
            Execute();
        }

        [Test]
        public void CopiedDependency()
        {
            CopyPackage("test.box4_5i"); 
            CopyPackage("test.box5_6_1");
            CopyPackage("test.box6_2_1");
            CopyPackage("test.box1");
            CopyPackage("test.box2");
            Arrange(
                () =>
                {
                    string path = TestInfo.ModulesDirectory;
                    var registry = new PackageRegistry();
                    registry.DiscoverPackages(new PackageScanner(path));
                    return new Context<PackageRegistry>(registry);
                });

            Action(ctx =>
                   {
                       ctx.Sut.LoadPackages(new DefaultLoader());
                       Util.ForceLoadOfInternals(ctx.Sut);
                   });

            Assert(ctx => ctx.Sut.Packages.Count() == 5);
            Assert(ctx => ctx.Sut.Packages.SelectMany(x => x.LoadedAssemblies).Count() == 5);
            Assert(ctx => AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name.ToLower().Contains("test.box")) == 5);
            Execute();
        }

        [Test]
        public void MissingDependency()
        {
            CopyPackage("test.box4_5i"); 
            CopyPackage("test.box5_6_1");
            CopyPackage("test.box6_2_1");   //partially satisfied
            CopyPackage("test.box2");       //Box2 will load
            Arrange(
                () =>
                {
                    string path = TestInfo.ModulesDirectory;
                    var registry = new PackageRegistry();
                    registry.DiscoverPackages(new PackageScanner(path));
                    return new Context<PackageRegistry>(registry);
                });

            Action(ctx =>
                   {
                       ctx.Sut.LoadPackages(new DefaultLoader());
                       Util.ForceLoadOfInternals(ctx.Sut);
                   });

            Assert(ctx => ctx.Sut.Packages.Count() == 4);
            Assert(ctx => ctx.Sut.Packages.SelectMany(x => x.LoadedAssemblies).Count() == 1);
            Assert(ctx => Util.BoxesLoadedAssemblies().Count() == 1);
            Execute();
        }
    }
}