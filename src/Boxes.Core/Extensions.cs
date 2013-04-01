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
namespace Boxes
{
    using System.Collections.Generic;
    using Tasks;

    /// <summary>
    /// Extension methods go here :)
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// add a range of items to a collection
        /// </summary>
        /// <typeparam name="T">the item type</typeparam>
        /// <param name="collection">the collection to add too</param>
        /// <param name="collectionToAdd">the items to add</param>
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> collectionToAdd)
        {
            foreach (var item in collectionToAdd)
            {
                collection.Add(item);
            }
        }

        /// <summary>
        /// force the iteration over a list (use this to force the <see cref="PipilineExecutor{T}.Execute"/> to run)
        /// </summary>
        /// <typeparam name="T">the type in the list</typeparam>
        /// <param name="collection">the list to iterate over</param>
        public static void Force<T>(this IEnumerable<T> collection )
        {
            IEnumerator<T> enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext()){ }
        }

        /// <summary>
        /// creates a <see cref="PipilineExecutor{T}"/> with tasks to run
        /// </summary>
        /// <typeparam name="T">the type to work with</typeparam>
        /// <param name="tasks">tasks to add to the pipeline</param>
        /// <returns>a configured pipeline</returns>
        public static PipilineExecutor<T> CreatePipeline<T>(this IEnumerable<IBoxesTask<T>> tasks)
        {
            var pipeline = new PipilineExecutor<T>();
            foreach (var registration in tasks)
            {
                pipeline.Register(registration);
            }
            return pipeline;
        }
    }
}
