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

/// <summary> Either monad utils </summary>
public static class Either
{
    /// <summary> Create Either from left value </summary>
    /// <param name="left"> Value </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left)
        => new(left);

    /// <summary> Create Either from right value </summary>
    /// <param name="right"> Value </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right)
        => new(right);

    /// <summary> Convert to other left value type </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    public static Either<TLeftResult, TRight> SelectLeft<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, TLeftResult> leftSelector)
        => item.HasLeft
            ? Left<TLeftResult, TRight>((leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left))
            : Right<TLeftResult, TRight>(item.Right);

    /// <summary> Convert to other right value type </summary>
    /// <param name="item"> Source </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    public static Either<TLeft, TRightResult> SelectRight<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, TRightResult> rightSelector)
        => item.HasLeft
            ? Left<TLeft, TRightResult>(item.Left)
            : Right<TLeft, TRightResult>((rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right));

    /// <summary> Convert to other type </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector)
        => item.HasLeft
            ? Left<TLeftResult, TRightResult>((leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left))
            : Right<TLeftResult, TRightResult>((rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right));

    /// <summary> Get left value or default </summary>
    /// <param name="item"> Source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TLeft? LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item)
        => item.HasLeft ? item.Left : default;

    /// <summary> Get left value or default </summary>
    /// <param name="item"> Source </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TLeft LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item, TLeft defaultValue)
        => item.HasLeft ? item.Left : defaultValue;

    /// <summary> Get left value or default from generator </summary>
    /// <param name="item"> Source </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TLeft LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft> defaultGenerator)
        => item.HasLeft ? item.Left : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Get right value or default </summary>
    /// <param name="item"> Source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TRight? RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item)
        => item.HasRight ? item.Right : default;

    /// <summary> Get right value or default </summary>
    /// <param name="item"> Source </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TRight RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item, TRight defaultValue)
        => item.HasRight ? item.Right : defaultValue;

    /// <summary> Get right value or default from generator </summary>
    /// <param name="item"> Source </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TRight RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TRight> defaultGenerator)
        => item.HasRight ? item.Right : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Convert to left value type </summary>
    /// <param name="item"> Source </param>
    /// <param name="rightToLeft"> Right to left converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TLeft ToLeft<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TRight, TLeft> rightToLeft)
        => item.HasLeft ? item.Left : (rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft))).Invoke(item.Right);

    /// <summary> Convert to right value type </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftToRight"> Left to right converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TRight ToRight<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft, TRight> leftToRight)
        => item.HasRight ? item.Right : (leftToRight ?? throw new ArgumentNullException(nameof(leftToRight))).Invoke(item.Left);

    /// <summary> Convert to other value type </summary>
    /// <param name="item"> Source </param>
    /// <param name="fromLeft"> Left value converter </param>
    /// <param name="fromRight"> Right value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TResult"> Left result type </typeparam>
    public static TResult ToValue<TLeft, TRight, TResult>(this Either<TLeft, TRight> item, Func<TLeft, TResult> fromLeft,
        Func<TRight, TResult> fromRight)
        => item.HasLeft
            ? (fromLeft ?? throw new ArgumentNullException(nameof(fromLeft))).Invoke(item.Left)
            : (fromRight ?? throw new ArgumentNullException(nameof(fromRight))).Invoke(item.Right);
}
