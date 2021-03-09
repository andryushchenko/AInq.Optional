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
    public static Optional<TLeft> LeftOrNone<TLeft, TRight>(this Either<TLeft, TRight> item)
        => item.HasLeft ? Optional.Value(item.Left) : Optional.None<TLeft>();

    public static Optional<TRight> RightOrNone<TLeft, TRight>(this Either<TLeft, TRight> item)
        => item.HasLeft ? Optional.None<TRight>() : Optional.Value(item.Right);

    public static Try<TLeft> TryLeft<TLeft, TRight>(this Either<TLeft, TRight> item)
        => Try.Result(() => item.Left);

    public static Try<TRight> TryRight<TLeft, TRight>(this Either<TLeft, TRight> item)
        => Try.Result(() => item.Right);

    public static Either<TLeft, TRight> Or<TLeft, TRight>(this Optional<TLeft> left, TRight right)
        => left.HasValue ? Either.Left<TLeft, TRight>(left.Value) : Either.Right<TLeft, TRight>(right);

    public static Either<TLeft, TRight> Or<TLeft, TRight>(this Try<TLeft> left, TRight right)
        => left.Success ? Either.Left<TLeft, TRight>(left.Value) : Either.Right<TLeft, TRight>(right);
}

}
