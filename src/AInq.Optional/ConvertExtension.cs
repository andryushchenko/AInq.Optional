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

namespace AInq.Optional
{

/// <summary> Monad convert utils </summary>
public static class ConvertExtension
{
    /// <summary> Get left value or none  </summary>
    /// <param name="item"> Source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Maybe with left value </returns>
    public static Maybe<TLeft> LeftOrNone<TLeft, TRight>(this Either<TLeft, TRight> item)
        => item.HasLeft ? Maybe.Value(item.Left) : Maybe.None<TLeft>();

    /// <summary> Get right value or none  </summary>
    /// <param name="item"> Source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Maybe with right value </returns>
    public static Maybe<TRight> RightOrNone<TLeft, TRight>(this Either<TLeft, TRight> item)
        => item.HasLeft ? Maybe.None<TRight>() : Maybe.Value(item.Right);

    /// <summary> Try get left value  </summary>
    /// <param name="item"> Source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Try with left value </returns>
    public static Try<TLeft> TryLeft<TLeft, TRight>(this Either<TLeft, TRight> item)
        => Try.Result(() => item.Left);

    /// <summary> Try get right value  </summary>
    /// <param name="item"> Source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Try with right value </returns>
    public static Try<TRight> TryRight<TLeft, TRight>(this Either<TLeft, TRight> item)
        => Try.Result(() => item.Right);

    /// <summary> Get source value or other if empty  </summary>
    /// <param name="item"> Maybe source </param>
    /// <param name="other"> Other value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Either </returns>
    public static Either<TLeft, TRight> Or<TLeft, TRight>(this Maybe<TLeft> item, TRight other)
        => item.HasValue ? Either.Left<TLeft, TRight>(item.Value) : Either.Right<TLeft, TRight>(other);

    /// <summary> Get source value or other if exception  </summary>
    /// <param name="item"> Maybe source </param>
    /// <param name="other"> Other value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Either </returns>
    public static Either<TLeft, TRight> Or<TLeft, TRight>(this Try<TLeft> item, TRight other)
        => item.Success ? Either.Left<TLeft, TRight>(item.Value) : Either.Right<TLeft, TRight>(other);

    /// <summary> Get value form this item or other  </summary>
    /// <param name="item"> Maybe </param>
    /// <param name="other"> Try </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> Or<T>(this Maybe<T> item, Try<T> other)
        => item.HasValue ? Try.Value(item.Value) : other;

    /// <summary> Get value form this item or other  </summary>
    /// <param name="item"> Try </param>
    /// <param name="other"> Maybe </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> Or<T>(this Try<T> item, Maybe<T> other)
        => item.Success ? Maybe.Value(item.Value) : other;

    /// <summary> Convert <see cref="Maybe{T}" /> to <see cref="Try{T}" />  </summary>
    /// <param name="item"> Maybe </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> AsTry<T>(this Maybe<T> item)
        => Try.Result(() => item.Value);

    /// <summary> Convert <see cref="Try{T}" /> to <see cref="Maybe{T}" />  </summary>
    /// <param name="item"> Try </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> AsMaybe<T>(this Try<T> item)
        => item.Success ? Maybe.Value(item.Value) : Maybe.None<T>();
}

}
