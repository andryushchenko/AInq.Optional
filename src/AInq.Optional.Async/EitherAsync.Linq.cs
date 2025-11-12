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

using System.Runtime.CompilerServices;

namespace AInq.Optional;

public static partial class EitherAsync
{
    /// <param name="collection"> <see cref="Either{TLeft,TRight}" /> collection </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    extension<TLeft, TRight>(IAsyncEnumerable<Either<TLeft, TRight>> collection)
    {
        /// <inheritdoc cref="Either.LeftValues{TLeft,TRight}(System.Collections.Generic.IEnumerable{AInq.Optional.Either{TLeft,TRight}})" />
        [PublicAPI]
        public async IAsyncEnumerable<TLeft> LeftValues([EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await foreach (var either in collection.WithCancellation(cancellation).ConfigureAwait(false))
                if (either is {HasLeft: true})
                    yield return either.Left;
        }

        /// <inheritdoc cref="Either.RightValues{TLeft,TRight}(System.Collections.Generic.IEnumerable{AInq.Optional.Either{TLeft,TRight}})" />
        [PublicAPI]
        public async IAsyncEnumerable<TRight> RightValues([EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await foreach (var either in collection.WithCancellation(cancellation).ConfigureAwait(false))
                if (either is {HasRight: true})
                    yield return either.Right;
        }
    }
}
