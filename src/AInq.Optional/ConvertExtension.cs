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

/// <summary> Monad convert utils </summary>
public static class ConvertExtension
{
    /// <summary> Get left value or none </summary>
    /// <param name="either"> Either source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Maybe with left value </returns>
    public static Maybe<TLeft> MaybeLeft<TLeft, TRight>(this Either<TLeft, TRight> either)
        => either.HasLeft ? Maybe.Value(either.Left) : Maybe.None<TLeft>();

    /// <summary> Get right value or none </summary>
    /// <param name="either"> Either source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Maybe with right value </returns>
    public static Maybe<TRight> MaybeRight<TLeft, TRight>(this Either<TLeft, TRight> either)
        => either.HasLeft ? Maybe.None<TRight>() : Maybe.Value(either.Right);

    /// <summary> Try get left value </summary>
    /// <param name="either"> Either source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Try with left value </returns>
    public static Try<TLeft> TryLeft<TLeft, TRight>(this Either<TLeft, TRight> either)
        => Try.Result(() => either.Left);

    /// <summary> Try get right value </summary>
    /// <param name="either"> Either source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Try with right value </returns>
    public static Try<TRight> TryRight<TLeft, TRight>(this Either<TLeft, TRight> either)
        => Try.Result(() => either.Right);

    /// <summary> Get source value or other if empty </summary>
    /// <param name="maybe"> Maybe source </param>
    /// <param name="other"> Other value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Either </returns>
    public static Either<TLeft, TRight> Or<TLeft, TRight>(this Maybe<TLeft> maybe, TRight other)
        => maybe.HasValue ? Either.Left<TLeft, TRight>(maybe.Value) : Either.Right<TLeft, TRight>(other);

    /// <summary> Get source value or other if exception </summary>
    /// <param name="try"> Try source </param>
    /// <param name="other"> Other value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Either </returns>
    public static Either<TLeft, TRight> Or<TLeft, TRight>(this Try<TLeft> @try, TRight other)
        => @try.Success ? Either.Left<TLeft, TRight>(@try.Value) : Either.Right<TLeft, TRight>(other);

    /// <summary> Get value form this item or other </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> Or<T>(this Maybe<T> maybe, Try<T> @try)
        => maybe.HasValue ? Try.Value(maybe.Value) : @try;

    /// <summary> Get value form this item or other </summary>
    /// <param name="try"> Try item </param>
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> Or<T>(this Try<T> @try, Maybe<T> maybe)
        => @try.Success ? Maybe.Value(@try.Value) : maybe;

    /// <summary> Convert <see cref="Maybe{T}" /> to <see cref="Try{T}" /> </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> AsTry<T>(this Maybe<T> maybe)
        => Try.Result(() => maybe.Value);

    /// <summary> Convert <see cref="Try{T}" /> to <see cref="Maybe{T}" /> </summary>
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> AsMaybe<T>(this Try<T> @try)
        => @try.Success ? Maybe.Value(@try.Value) : Maybe.None<T>();
}
