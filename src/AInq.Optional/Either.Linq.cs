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

public static partial class Either
{
    /// <param name="collection"> <see cref="Either{TLeft,TRight}" /> collection </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    extension<TLeft, TRight>(IEnumerable<Either<TLeft, TRight>> collection)
    {
        /// <summary> Select existing left values </summary>
        /// <returns> Left values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<TLeft> LeftValues()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(either => either is {HasLeft: true}).Select(either => either.Left);
        }

        /// <summary> Select matching left values </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Left values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<TLeft> LeftValues(Func<TLeft, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(either => either is {HasLeft: true} && filter.Invoke(either.Left)).Select(either => either.Left);
        }

        /// <summary> Select existing right values </summary>
        /// <returns> Right values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<TRight> RightValues()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(either => either is {HasRight: true}).Select(either => either.Right);
        }

        /// <summary> Select matching right values </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Right values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<TRight> RightValues(Func<TRight, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(either => either is {HasRight: true} && filter.Invoke(either.Right)).Select(either => either.Right);
        }
    }

    /// <param name="collection"> <see cref="Either{TLeft,TRight}" /> parallel query </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    extension<TLeft, TRight>(ParallelQuery<Either<TLeft, TRight>> collection)
    {
        /// <inheritdoc cref="LeftValues{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<TLeft> LeftValues()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(either => either is {HasLeft: true}).Select(either => either.Left);
        }

        /// <inheritdoc cref="LeftValues{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}},Func{TLeft,bool})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<TLeft> LeftValues(Func<TLeft, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(either => either is {HasLeft: true} && filter.Invoke(either.Left)).Select(either => either.Left);
        }

        /// <inheritdoc cref="RightValues{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<TRight> RightValues()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(either => either is {HasRight: true}).Select(either => either.Right);
        }

        /// <inheritdoc cref="RightValues{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}},Func{TRight,bool})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<TRight> RightValues(Func<TRight, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(either => either is {HasRight: true} && filter.Invoke(either.Right)).Select(either => either.Right);
        }
    }
}
