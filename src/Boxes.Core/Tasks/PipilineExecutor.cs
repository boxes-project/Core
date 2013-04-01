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
    /// creates a pipeline with yielding. This class allows to multiple tasks to
    /// be executed against each item.
    /// </summary>
    /// <typeparam name="T">The type of item which will be yielded over</typeparam>
    public class PipilineExecutor<T>
    {
        readonly List<TaskRunner<T>> _taskRunners = new List<TaskRunner<T>>();

        /// <summary>
        /// provide a task to be applied in this pipeline (you can just register the IBoxesTask)
        /// </summary>
        /// <param name="taskRunner">the taskRunner for a IBoxesTask</param>
        public virtual void Register(TaskRunner<T> taskRunner)
        {
            _taskRunners.Add(taskRunner);
        }

        /// <summary>
        /// provide a task to be applied in this pipeline
        /// </summary>
        /// <typeparam name="TTask">the task type</typeparam>
        /// <param name="task">the task to apply on each item</param>
        public virtual void Register<TTask>(TTask task) where TTask : IBoxesTask<T>
        {
            _taskRunners.Add(new TaskRunner<T>(task));
        }

        public virtual IEnumerable<T> Execute(IEnumerable<T> items)
        {
            foreach (var taskRunner in _taskRunners)
            {
                items = taskRunner.Execute(items);
            }
            return items;
        }
    }
}