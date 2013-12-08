// Copyright 2012 - 2013 dbones.co.uk
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
    using Discovering;
    using Exceptions;
    using Infrastructure;
    using NUnit.Framework;

    public class DependencyExceptionTests : TestBase<PackageRegistry>
    {
        [Test]
        public void SimpleCircularDependency()
        {
            CopyPackage("test.box9_10");
            CopyPackage("test.box10_9");
            Arrange(
                () =>
                {
                    var registry = new PackageRegistry();
                    var sut = (dynamic)new Context<PackageRegistry>(registry);
                    sut.Scanner = new PackageScanner(TestInfo.ModulesDirectory);
                    return sut;
                });
            Action(ctx => ctx.Sut.DiscoverPackages(((dynamic)ctx).Scanner));
            AssertException<CircularDependencyException>();
            Execute();
        }

        [Test]
        public void SelfCircularDependency()
        {
            CopyPackage("test.box12_12");
            Arrange(
                () =>
                {
                    var registry = new PackageRegistry();
                    var sut = (dynamic)new Context<PackageRegistry>(registry);
                    sut.Scanner = new PackageScanner(TestInfo.ModulesDirectory);
                    return sut;
                });
            Action(ctx => ctx.Sut.DiscoverPackages(((dynamic)ctx).Scanner));
            AssertException<CircularDependencyException>();
            Execute();
        }

        [Test]
        public void DuplicuteModuleDependency()
        {
            CopyPackage("test.box1");
            CopyPackage("test.box11_1e");
            Arrange(
                () =>
                {
                    var registry = new PackageRegistry();
                    var sut = (dynamic)new Context<PackageRegistry>(registry);
                    sut.Scanner = new PackageScanner(TestInfo.ModulesDirectory);
                    return sut;
                });
            Action(ctx => ctx.Sut.DiscoverPackages(((dynamic)ctx).Scanner));
            AssertException<DuplicuteModuleException>();
            Execute();
        }
    }
}