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

/// <summary> <see cref="Either{TLeft,TRight}" /> utils and extensions </summary>
public static partial class Either
{
    /// <inheritdoc cref="Either{TLeft,TRight}.FromLeft(TLeft)" />
    [PublicAPI]
    public static Either<TLeft, TRight> Left<TLeft, TRight>([NoEnumeration] TLeft left)
        => Either<TLeft, TRight>.FromLeft(left);

    /// <inheritdoc cref="Either{TLeft,TRight}.FromRight(TRight)" />
    [PublicAPI]
    public static Either<TLeft, TRight> Right<TLeft, TRight>([NoEnumeration] TRight right)
        => Either<TLeft, TRight>.FromRight(right);

    /// <inheritdoc cref="Either{TLeft,TRight}.FromLeft(TLeft)" />
    [PublicAPI, Pure]
    public static Either<TLeft, TRight> AsEither<TLeft, TRight>([NoEnumeration] this TLeft left)
        => Either<TLeft, TRight>.FromLeft(left);

    /// <inheritdoc cref="Either{TLeft,TRight}.FromRight(TRight)" />
    [PublicAPI, Pure]
    public static Either<TLeft, TRight> AsEither<TLeft, TRight>([NoEnumeration] this TRight right)
        => Either<TLeft, TRight>.FromRight(right);

    /// <summary> Swap left and right values </summary>
    [PublicAPI, Pure]
    public static Either<TRight, TLeft> Invert<TLeft, TRight>(this Either<TLeft, TRight> either)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? Right<TRight, TLeft>(either.Left)
            : Left<TRight, TLeft>(either.Right);

    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    extension<TLeft, TRight>(Either<TLeft, TRight>)
    {
        /// <inheritdoc cref="Invert{TLeft,TRight}(Either{TLeft,TRight})" />
        [PublicAPI, Pure]
        public static Either<TRight, TLeft> operator !(Either<TLeft, TRight> either)
            => either.Invert();
    }
}
