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
    /// <summary>
    /// this represents a Boxes task. these are applied normally within a Pipeline
    /// </summary>
    /// <typeparam name="T">what type the task will be executed against</typeparam>
    public interface IBoxesTask<in T>
    {
        /// <summary>
        /// a function to see if the task will support the current instance,
        /// this is normally called before the execute
        /// </summary>
        bool CanHandle(T item); 

        /// <summary>
        /// executes the tasks logic
        /// </summary>
        /// <param name="item">the item on which the task will operate on</param>
        void Execute(T item);
    }
}