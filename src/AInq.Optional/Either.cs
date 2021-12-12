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
    /// <param name="left"> Left value </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left)
        => new EitherLeft<TLeft, TRight>(left);

    /// <summary> Create Either from right value </summary>
    /// <param name="right"> Right value </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right)
        => new EitherRight<TLeft, TRight>(right);

    /// <summary> Convert left value to Either </summary>
    /// <param name="left"> Left value </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static Either<TLeft, TRight> AsEither<TLeft, TRight>(this TLeft left)
        => new EitherLeft<TLeft, TRight>(left);

    /// <summary> Convert right value to Either </summary>
    /// <param name="right"> Right value </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static Either<TLeft, TRight> AsEither<TLeft, TRight>(this TRight right)
        => new EitherRight<TLeft, TRight>(right);

    /// <summary> Swap left and right values </summary>
    public static Either<TRight, TLeft> Invert<TLeft, TRight>(this Either<TLeft, TRight> either)
        => either.HasLeft ? Right<TRight, TLeft>(either.Left) : Left<TRight, TLeft>(either.Right);

    /// <summary> Convert to other left value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    public static Either<TLeftResult, TRight> SelectLeft<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> either,
        Func<TLeft, TLeftResult> leftSelector)
        => either.HasLeft
            ? Left<TLeftResult, TRight>((leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left))
            : Right<TLeftResult, TRight>(either.Right);

    /// <inheritdoc cref="SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    public static Either<TLeftResult, TRight> SelectLeft<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Either<TLeftResult, TRight>> leftSelector)
        => item.HasLeft
            ? (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left)
            : Right<TLeftResult, TRight>(item.Right);

    /// <summary> Convert to other right value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    public static Either<TLeft, TRightResult> SelectRight<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either,
        Func<TRight, TRightResult> rightSelector)
        => either.HasRight
            ? Right<TLeft, TRightResult>((rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right))
            : Left<TLeft, TRightResult>(either.Left);

    /// <inheritdoc cref="SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    public static Either<TLeft, TRightResult> SelectRight<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, Either<TLeft, TRightResult>> rightSelector)
        => item.HasRight
            ? (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right)
            : Left<TLeft, TRightResult>(item.Left);

    /// <summary> Convert to other type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> either,
        Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector)
        => either.HasLeft
            ? Left<TLeftResult, TRightResult>((leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left))
            : Right<TLeftResult, TRightResult>((rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right));

    /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> either,
        Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector, Func<TRight, TRightResult> rightSelector)
        => either.HasLeft
            ? (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left)
            : Right<TLeftResult, TRightResult>((rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right));

    /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> either,
        Func<TLeft, TLeftResult> leftSelector, Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        => either.HasLeft
            ? Left<TLeftResult, TRightResult>((leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left))
            : (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right);

    /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> either,
        Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector, Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        => either.HasLeft
            ? (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left)
            : (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right);

    /// <summary> Get left value or default </summary>
    /// <param name="either"> Either item </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TLeft? LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either)
        => either.HasLeft ? either.Left : default;

    /// <summary> Get left value or default </summary>
    /// <param name="either"> Either item </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TLeft LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, TLeft defaultValue)
        => either.HasLeft ? either.Left : defaultValue;

    /// <summary> Get left value or default from generator </summary>
    /// <param name="either"> Either item </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TLeft LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft> defaultGenerator)
        => either.HasLeft ? either.Left : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Get right value or default </summary>
    /// <param name="either"> Either item </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TRight? RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either)
        => either.HasRight ? either.Right : default;

    /// <summary> Get right value or default </summary>
    /// <param name="either"> Either item </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TRight RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, TRight defaultValue)
        => either.HasRight ? either.Right : defaultValue;

    /// <summary> Get right value or default from generator </summary>
    /// <param name="either"> Either item </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TRight RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight> defaultGenerator)
        => either.HasRight ? either.Right : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Convert to left value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="rightToLeft"> Right to left converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TLeft ToLeft<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight, TLeft> rightToLeft)
        => either.HasLeft ? either.Left : (rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft))).Invoke(either.Right);

    /// <summary> Convert to right value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftToRight"> Left to right converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static TRight ToRight<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, TRight> leftToRight)
        => either.HasRight ? either.Right : (leftToRight ?? throw new ArgumentNullException(nameof(leftToRight))).Invoke(either.Left);

    /// <summary> Convert to other value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="fromLeft"> Left value converter </param>
    /// <param name="fromRight"> Right value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TResult"> Left result type </typeparam>
    public static TResult ToValue<TLeft, TRight, TResult>(this Either<TLeft, TRight> either, Func<TLeft, TResult> fromLeft,
        Func<TRight, TResult> fromRight)
        => either.HasLeft
            ? (fromLeft ?? throw new ArgumentNullException(nameof(fromLeft))).Invoke(either.Left)
            : (fromRight ?? throw new ArgumentNullException(nameof(fromRight))).Invoke(either.Right);

    /// <summary> Select existing left values </summary>
    /// <param name="collection"> Either collection </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Left values collection </returns>
    public static IEnumerable<TLeft> LeftValues<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item.HasLeft)
           .Select(item => item.Left);

    /// <summary> Select existing right values </summary>
    /// <param name="collection"> Either collection </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Right values collection </returns>
    public static IEnumerable<TRight> RightValues<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item.HasRight)
           .Select(item => item.Right);

    /// <summary> Do action with left value (if exists) </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftAction"> Left value action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static void DoLeft<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> leftAction)
    {
        if (either.HasLeft) (leftAction ?? throw new ArgumentNullException(nameof(leftAction))).Invoke(either.Left);
    }

    /// <summary> Do action with right value (if exists) </summary>
    /// <param name="either"> Either item </param>
    /// <param name="rightAction"> Right value action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static void DoRight<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TRight> rightAction)
    {
        if (either.HasRight) (rightAction ?? throw new ArgumentNullException(nameof(rightAction))).Invoke(either.Right);
    }

    /// <summary> Do action with left or right value </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftAction"> Left value action </param>
    /// <param name="rightAction"> Right value action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static void Do<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> leftAction, Action<TRight> rightAction)
    {
        if (either.HasLeft) (leftAction ?? throw new ArgumentNullException(nameof(leftAction))).Invoke(either.Left);
        else if (either.HasRight) (rightAction ?? throw new ArgumentNullException(nameof(rightAction))).Invoke(either.Right);
    }
}
