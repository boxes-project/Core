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
    using Infrastructure;
    using NUnit.Framework;

    [TestFixture]
    public abstract class TestBase<T> where T : class
    {
        /// <summary>
        /// represents the application, allows you to run the test in a seperate app domain
        /// </summary>
        protected App _app;
        protected WorkspaceContext _workspaceContext;

        /// <summary>
        /// the test being run againse the app.
        /// </summary>
        protected Test<T> _test;

        [SetUp]
        public void NUnitSetup()
        {
            _workspaceContext = WorkspaceContext.CreateFromSlnName(SlnName());
            _workspaceContext.Scan(GetPackages());

            _app = new App();
            _app.SetData(GlobalRegistry.WorkingDirKey, _workspaceContext.WorkingDirectory);
            _app.SetData(GlobalRegistry.SlnDirKey, _workspaceContext.SlnDirectory);
            
            _test = _app.CreateTest<T>();

        }

        [TearDown]
        public void NunitTeardown()
        {
            _app.Dispose();
            _workspaceContext.Dispose();
        }

        protected virtual string SlnName()
        {
            return "*.sln";
        }

        protected virtual IFolderSelector GetPackages()
        {
            return new FolderSelector(
                dir => dir.GetFiles().Any(x=>x.Name.ToLower().Contains("manifest")),
                dir =>
                {
                    //var file = dir.GetFiles().First(x => x.Name.ToLower().Contains("manifest"));
                    //var name = Path.GetFileNameWithoutExtension(file.Name);
                    return dir.Name.ToLower();
                } 
            );
        }

        protected void CopyPackage(string key)
        {
            _workspaceContext.CopyFolder(key.ToLower(), GlobalRegistry.ModuleDir);
        }

        protected void CopyPackage(string key, string subdir)
        {
            _workspaceContext.CopyFolder(key.ToLower(), subdir);
        }

        /// <summary>
        /// arrange the test
        /// </summary>
        /// <param name="arrange"></param>
        protected void Arrange(Func<Context<T>> arrange)
        {
            _test.Arrange(arrange);
        }

        /// <summary>
        /// test a ctor, or something which does not require arranging
        /// </summary>
        /// <param name="action"></param>
        protected void Action(Func<Context<T>> action)
        {
            _test.Arrange(action);
            _test.Action(ctx => { }); //meh
        }

        /// <summary>
        /// action which is being tested (Remember to run the <see cref="Execute"/>)
        /// </summary>
        /// <param name="action"></param>
        protected void Action(Action<Context<T>> action)
        {
            _test.Action(action);
        }

        /// <summary>
        /// the assert, to confirm sut is behaving as required (Remember to run the <see cref="Execute"/>)
        /// </summary>
        /// <param name="assert"></param>
        protected void Assert(Func<Context<T>, bool> assert)
        {
            _test.Assert(assert);
        }

        /// <summary>
        /// the assert, to confirm sut is behaving as required (Remember to run the <see cref="Execute"/>) 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assert"></param>
        protected void Assert(string name, Func<Context<T>, bool> assert)
        {
            _test.Assert(name, assert);
        }

        /// <summary>
        /// clean up any thing from the test, inside the other app domain
        /// </summary>
        /// <param name="teardown"></param>
        public void Teardown(Action<Context<T>> teardown)
        {
            _test.Teardown(teardown);
        }

        /// <summary>
        /// execute the test
        /// </summary>
        protected void Execute()
        {
            _app.ExecuteTest(_test);
        }

    }


    /// <summary>
    /// meh
    /// </summary>
    public static class GlobalRegistry
    {
        public static string WorkingDirKey { get { return "WorkingDir"; } }
        public static string SlnDirKey { get { return "SlnDir"; } }
        public static string ModuleDir { get { return "Modules"; } }
        
    }
}