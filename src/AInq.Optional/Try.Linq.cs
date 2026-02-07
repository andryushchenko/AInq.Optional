// Copyright 2021 Anton Andryushchenko
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace AInq.Optional;

public static partial class Try
{
    /// <param name="collection"> Try collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<Try<T>> collection)
    {
        /// <summary> Select existing values </summary>
        /// <returns> Values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<T> Values()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(@try => @try is {Success: true}).Select(@try => @try.Value);
        }

        /// <summary> Select existing matching values </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<T> Values([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(@try => @try is {Success: true} && filter.Invoke(@try.Value)).Select(@try => @try.Value);
        }
    }

    extension<T>(ParallelQuery<Try<T>> collection)
    {
        /// <inheritdoc cref="Values{T}(IEnumerable{Try{T}})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<T> Values()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(@try => @try is {Success: true}).Select(@try => @try.Value);
        }

        /// <inheritdoc cref="Values{T}(IEnumerable{Try{T}},Func{T,bool})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<T> Values([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(@try => @try is {Success: true} && filter.Invoke(@try.Value)).Select(@try => @try.Value);
        }
    }
}
