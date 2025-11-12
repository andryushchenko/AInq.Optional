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
            => (collection ?? throw new ArgumentNullException(nameof(collection))).Where(item => item is {HasLeft: true}).Select(item => item.Left);

        /// <summary> Select existing right values </summary>
        /// <returns> Right values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<TRight> RightValues()
            => (collection ?? throw new ArgumentNullException(nameof(collection))).Where(item => item is {HasRight: true}).Select(item => item.Right);
    }

    /// <param name="collection"> <see cref="Either{TLeft,TRight}" /> parallel query </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    extension<TLeft, TRight>(ParallelQuery<Either<TLeft, TRight>> collection)
    {
        /// <inheritdoc cref="LeftValues{TLeft,TRight}(System.Collections.Generic.IEnumerable{AInq.Optional.Either{TLeft,TRight}})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<TLeft> LeftValues()
            => (collection ?? throw new ArgumentNullException(nameof(collection))).Where(item => item is {HasLeft: true}).Select(item => item.Left);

        /// <inheritdoc cref="RightValues{TLeft,TRight}(System.Collections.Generic.IEnumerable{AInq.Optional.Either{TLeft,TRight}})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<TRight> RightValues()
            => (collection ?? throw new ArgumentNullException(nameof(collection))).Where(item => item is {HasRight: true}).Select(item => item.Right);
    }
}
