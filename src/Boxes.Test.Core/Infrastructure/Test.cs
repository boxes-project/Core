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
namespace Boxes.Test.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;

    [Serializable]
    [DebuggerStepThrough]
    public abstract class Test : MarshalByRefObject
    {
        public abstract void Execute();
        public List<string> FailedAssertions { get; protected set; }
        public List<string> PassedAssertions { get; protected set; }

        public string GetResult()
        {
            StringBuilder sb = new StringBuilder();
            Action<IEnumerable<string>, string> createOutput =
                (assertions, assertionType) =>
                {
                    if (!assertions.Any()) return;
                    sb.AppendLine(string.Format("Number {0}: {1}", assertionType, assertions.Count()));
                    foreach (var assertion in assertions)
                    {
                        sb.Append(string.Format("{0}: ", assertionType));
                        sb.AppendLine(assertion);
                    }
                };

            sb.AppendLine("Assertions within this test :");
            createOutput(PassedAssertions, "Passed");
            sb.AppendLine("");
            createOutput(FailedAssertions, "Failed");
            return sb.ToString();
        }
    }

    [Serializable]
    [DebuggerStepThrough]
    public class Test<T> : Test where T : class
    {
        private Func<Context<T>> _arrange;
        private Action<Context<T>> _action;
        
        private readonly Dictionary<string,Func<Context<T>, bool>> _asserts;
        private Action<Context<T>> _teardown;
        private int count = 1;

        private Exception exception;

        public Test()
        {
            _asserts = new Dictionary<string, Func<Context<T>, bool>>();
            FailedAssertions = new List<string>();
            PassedAssertions = new List<string>();
        }

        public void Arrange(Func<Context<T>> arrange)
        {
            _arrange = arrange;
        }

        public void Action(Action<Context<T>> action)
        {
            _action = action;
        }

        public void Assert(Func<Context<T>, bool> assert)
        {
            _asserts.Add(string.Format("Assetion: [{0}]", count++), assert);
        }

        public void AssertException<TException>(Func<Context<T>, Exception, bool> assert) where TException : Exception
        {
            Func<Context<T>, bool> exceptionAssert = ctx =>
            {
                if (this.exception == null || !(exception is TException))
                {
                    return false;
                }
                var result = assert(ctx, this.exception);
                //remove the exception if it was expected
                if (result)
                {
                    this.exception = null;
                }
                return result;
            };
            _asserts.Add(string.Format("Exception Assetion: [{0}]", count++), exceptionAssert);
        }

        public void Assert(string name, Func<Context<T>, bool> assert)
        {
            _asserts.Add(string.Format("Assetion: [{0}], {1}", count++, name), assert);
        }

        public void Teardown(Action<Context<T>> teardown)
        {
            _teardown = teardown;
        }


        public override void Execute()
        {
            Debug.WriteLine("executing in " + AppDomain.CurrentDomain.FriendlyName);
            Context<T> context = _arrange();

            try
            {
                _action(context);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            
            foreach (var assert in _asserts)
            {
                var passed = false;
                string exceptionMessage = "";
                try
                {
                    //var expression = SutVisitor.Apply(assert, Context);
                    //PAssert.IsTrue(expression);
                    passed = assert.Value(context);
                }
                catch (Exception ex)
                {
                    passed = false;
                    exceptionMessage = Environment.NewLine + ex;
                }

                if (passed)
                {
                    PassedAssertions.Add(assert.Key);
                }
                else
                {
                    FailedAssertions.Add(assert.Key + exceptionMessage);
                }
            }

            if (exception != null)
            {
                FailedAssertions.Add("Unhandled Exception: " + Environment.NewLine + exception);
            }

            if (_teardown != null)
            {
                _teardown(context);
            }
        }

       
    }
}