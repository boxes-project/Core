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

    public class TestsActInIsolationTests : TestBase<PackageRegistry>
    {
        [Test]
        public void Ensure_tests_are_isolated_pt1()
        {
            CopyPackage("test.box1");
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
            Assert(ctx => ctx.Sut.Packages.SelectMany(x=> x.LoadedAssemblies).Count() == 1);
            Assert(ctx => AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name.ToLower().Contains("test.box")) == 1);
            Execute();
        }

        [Test]
        public void Ensure_tests_are_isolated_pt2()
        {
            CopyPackage("test.box2");
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
            Assert(ctx => ctx.Sut.Packages.SelectMany(x => x.LoadedAssemblies).Count() == 1);
            Assert(ctx => AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name.ToLower().Contains("test.box")) == 1);
            Execute();
        }


        [Test]
        [Ignore]
        public void Test2()
        {
            Arrange(
                () =>
                {
                    string path = @"C:\Users\Dave\Documents\Visual Studio 2012\Projects\Boxes\build\packages";
                    var registry = new PackageRegistry();
                    var isolatedLoader = new IsolatedLoader(registry);

                    registry.DiscoverPackages(new PackageScanner(path));
                    dynamic sut = new Context<PackageRegistry>(registry);
                    sut.IsolatedLoader = isolatedLoader;
                    return sut;
                });

            Action(
                sut =>
                {
                    IsolatedLoader loader = ((dynamic) sut).IsolatedLoader;
                    sut.Sut.LoadPackages(loader);
                });
            
            Assert("loaded assemblies should be 5", sut => sut.Sut.Packages.SelectMany(x => x.LoadedAssemblies).Count() == 5);

            Teardown(
                sut=>
                {
                    IsolatedLoader loader = ((dynamic)sut).IsolatedLoader;
                    loader.Dispose();
                });
            Execute();
        }
    }
}