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
namespace Boxes.Test.Infrastructure
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Security.Policy;

    [DebuggerStepThrough]
    public class App : IDisposable
    {
        private readonly AppDomain _appDomain;
        public App()
        {
            AppDomainSetup domaininfo = new AppDomainSetup();
            domaininfo.ApplicationBase = Environment.CurrentDirectory;
            Evidence evidence = AppDomain.CurrentDomain.Evidence;
            _appDomain = AppDomain.CreateDomain("test", evidence, domaininfo);
        }

        //public void LoadAssemblies(params Assembly[] assemblies)
        //{
        //    var locations = assemblies.Select(x => x.Location).ToList();
        //    var loadProxy = new LoadAssemblyProxy(locations);

        //    _appDomain.DoCallBack(loadProxy.LoadAssemblies);
        //}

        public Test<T> CreateTest<T>() where T : class
        {
            var test = (Test<T>)_appDomain.CreateInstanceAndUnwrap(
                typeof(Test<T>).Assembly.FullName,
                typeof(Test<T>).FullName);
            return test;
        }


        public void SetData(string key, string value)
        {
            _appDomain.SetData(key, value);
        }

        public void ExecuteTest(Test test)
        {
            var executor =
                (BoundaryExecutor)_appDomain.CreateInstanceAndUnwrap(
                    typeof(BoundaryExecutor).Assembly.FullName,
                    typeof(BoundaryExecutor).FullName);

            executor.ExecuteMethod(test);
            string result = test.GetResult();

            if (test.FailedAssertions.Any())
            {
                throw new Exception(result);
            }
            Console.WriteLine(result);
        }


        public void Dispose()
        {
            AppDomain.Unload(_appDomain);
        }
    }
}