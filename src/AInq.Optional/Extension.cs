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

public static class Extension
{
    public static Maybe<TLeft> LeftOrNone<TLeft, TRight>(this Either<TLeft, TRight> item)
        => item.HasLeft ? Maybe.Value(item.Left) : Maybe.None<TLeft>();

    public static Maybe<TRight> RightOrNone<TLeft, TRight>(this Either<TLeft, TRight> item)
        => item.HasLeft ? Maybe.None<TRight>() : Maybe.Value(item.Right);

    public static Try<TLeft> TryLeft<TLeft, TRight>(this Either<TLeft, TRight> item)
        => Try.Result(() => item.Left);

    public static Try<TRight> TryRight<TLeft, TRight>(this Either<TLeft, TRight> item)
        => Try.Result(() => item.Right);

    public static Either<TLeft, TRight> Or<TLeft, TRight>(this Maybe<TLeft> item, TRight other)
        => item.HasValue ? Either.Left<TLeft, TRight>(item.Value) : Either.Right<TLeft, TRight>(other);

    public static Either<TLeft, TRight> Or<TLeft, TRight>(this Try<TLeft> item, TRight other)
        => item.Success ? Either.Left<TLeft, TRight>(item.Value) : Either.Right<TLeft, TRight>(other);

    public static Try<T> Or<T>(this Maybe<T> item, Try<T> other)
        => item.HasValue ? Try.Value(item.Value) : other;

    public static Maybe<T> Or<T>(this Try<T> item, Maybe<T> other)
        => item.Success ? Maybe.Value(item.Value) : other;
}

}
