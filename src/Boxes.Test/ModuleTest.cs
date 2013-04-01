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
    using System.Reflection;
    using Infrastructure;
    using NUnit.Framework;
    using Module = Boxes.Module;

    public class ModuleTest : TestBase<Module>
    {
        [Test]
        public void Set_version_and_name_manually()
        {
            Action(()=> new Context<Module>(new Module("test", new Version(1,0,0,0))));
            Assert("Name", ctx => ctx.Sut.Name == "test");
            Assert("Version", ctx => ctx.Sut.Version == new Version(1,0,0,0));
            Execute();
        }

        [Test]
        public void Set_version_and_name_via_AssemblyName()
        {
            //mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
            Action(() => new Context<Module>(new Module(new AssemblyName("mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"))));
            Assert("Name", ctx => ctx.Sut.Name == "mscorlib");
            Assert("Version", ctx => ctx.Sut.Version == new Version(2, 0, 0, 0));
            Execute();
        }

        [Test]
        public void Equality_ver1_ver1()
        {
            Action(() =>
                   {
                       dynamic ctx = new Context<Module>(new Module("test", new Version(1, 0, 0, 0)));
                       ctx.Module2 = new Module("test", new Version(1, 0, 0, 0));
                       return ctx;
                   }
                );
            Assert("equals", ctx => ctx.Sut.Equals(((dynamic)ctx).Module2));
            Execute();
        }

        [Test]
        [Ignore]
        public void Ver1_lessThan_ver2()
        {
            Action(() =>
                   {
                       dynamic ctx = new Context<Module>(new Module("test", new Version(1, 0, 0, 0)));
                       ctx.Module2 = new Module("test", new Version(2, 0, 0, 0));
                       return ctx;
                   }
                );
            Assert(ctx => ctx.Sut < ((dynamic)ctx).Module2);
            Execute();
        }

        [Test]
        [Ignore]
        public void Ver2_moreThan_ver1()
        {
            Action(() =>
                   {
                       dynamic ctx = new Context<Module>(new Module("test", new Version(2, 0, 0, 0)));
                       ctx.Module2 = new Module("test", new Version(1, 0, 0, 0));
                       return ctx;
                   }
                );
            Assert(ctx => ctx.Sut > ((dynamic)ctx).Module2);
            Execute();
        }


        [Test]
        [Ignore]
        public void Equality_ver2_moreThan_verStar()
        {
            Action(() =>
                   {
                       dynamic ctx = new Context<Module>(new Module("test", new Version(2, 0, 0, 0)));
                       ctx.Module2 = new Module("test", new Version(0,0,0,0));
                       return ctx;
                   }
                );
            Assert(ctx => ctx.Sut == (Module)((dynamic)ctx).Module2);
            Execute();
        }
    }
}