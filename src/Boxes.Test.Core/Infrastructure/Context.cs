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
    using System.Collections.Generic;
    using System.Dynamic;

    public class Context<T> : DynamicObject where T : class
    {
        private readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public Context(T subject)
        {
            Sut = subject;
        }

        /// <summary>
        /// Subject under test
        /// </summary>
        public T Sut { get; set; }

        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            string key = GetKey(binder.Name);
            return _dictionary.TryGetValue(key, out result);
        }

        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            var key = GetKey(binder.Name);
            if (_dictionary.ContainsKey(key))
            {
                _dictionary[key] = value;    
            }
            else
            {
                _dictionary.Add(key, value);
            }

            return true;
        }

        private string GetKey(string key)
        {
            return key.ToLower();
        }

        public override string ToString()
        {
            return Sut == null ? "Context - no sut" : string.Format("Context - {0}", Sut);
        }
    }
}