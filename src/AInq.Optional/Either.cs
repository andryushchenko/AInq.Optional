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

using System;

namespace AInq.Optional
{

public static class Either
{
    public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left)
        => new(left);

    public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right)
        => new(right);

    public static Either<TLeftResult, TRight> SelectLeft<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, TLeftResult> leftSelector)
        => item.HasLeft ? Left<TLeftResult, TRight>(leftSelector.Invoke(item.Left)) : Right<TLeftResult, TRight>(item.Right);

    public static Either<TLeft, TRightResult> SelectRight<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, TRightResult> rightSelector)
        => item.HasLeft ? Left<TLeft, TRightResult>(item.Left) : Right<TLeft, TRightResult>(rightSelector.Invoke(item.Right));

    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector)
        => item.HasLeft ? Left<TLeftResult, TRightResult>(leftSelector(item.Left)) : Right<TLeftResult, TRightResult>(rightSelector(item.Right));

    public static TLeft LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item, TLeft defaultValue)
        => item.HasLeft ? item.Left : defaultValue;

    public static TLeft LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft> defaultGenerator)
        => item.HasLeft ? item.Left : defaultGenerator.Invoke();

    public static TRight RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item, TRight defaultValue)
        => item.HasLeft ? defaultValue : item.Right;

    public static TRight RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TRight> defaultGenerator)
        => item.HasLeft ? defaultGenerator.Invoke() : item.Right;

    public static TLeft ToLeft<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TRight, TLeft> rightToLeft)
        => item.HasLeft ? item.Left : rightToLeft.Invoke(item.Right);

    public static TRight ToRight<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft, TRight> leftToRight)
        => item.HasLeft ? leftToRight.Invoke(item.Left) : item.Right;
}

}
