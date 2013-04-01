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
    using System.Collections.Generic;

    /// <summary>
    /// runs a task, works with a yielding IEnumerable and the PipelineExecutor
    /// </summary>
    /// <typeparam name="T">the type of item the task and task runner will work with</typeparam>
    public class TaskRunner<T>
    {
        private readonly IBoxesTask<T> _task;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="task">the associated Task</param>
        public TaskRunner(IBoxesTask<T> task)
        {
            _task = task;
        }

        /// <summary>
        /// executes the task over an IEnumberable of items
        /// </summary>
        /// <param name="items">the collection to apply the task to</param>
        /// <returns>a yield list, where the task will be applied to each item</returns>
        public virtual IEnumerable<T> Execute(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (_task.CanHandle(item))                  
                    _task.Execute(item);

                yield return item;
            }
        }

        public override string ToString()
        {
            return string.Format("TaskRunner For: {0}. Against: {1}", _task.GetType().Name, typeof(T));
        }
    }
}