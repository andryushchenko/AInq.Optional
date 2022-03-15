// Copyright 2021-2022 Anton Andryushchenko
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

/// <summary> Either utils </summary>
public static class Either
{
#region Value

    /// <inheritdoc cref="Either{TLeft,TRight}.FromLeft(TLeft)" />
    [PublicAPI]
    public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left)
        => Either<TLeft, TRight>.FromLeft(left);

    /// <inheritdoc cref="Either{TLeft,TRight}.FromRight(TRight)" />
    [PublicAPI]
    public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right)
        => Either<TLeft, TRight>.FromRight(right);

    /// <inheritdoc cref="Either{TLeft,TRight}.FromLeft(TLeft)" />
    [PublicAPI]
    public static Either<TLeft, TRight> AsEither<TLeft, TRight>(this TLeft left)
        => Either<TLeft, TRight>.FromLeft(left);

    /// <inheritdoc cref="Either{TLeft,TRight}.FromRight(TRight)" />
    [PublicAPI]
    public static Either<TLeft, TRight> AsEither<TLeft, TRight>(this TRight right)
        => Either<TLeft, TRight>.FromRight(right);

#endregion

#region Select

    /// <summary> Convert to other left value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    [PublicAPI]
    public static Either<TLeftResult, TRight> SelectLeft<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> either,
        [InstantHandle] Func<TLeft, TLeftResult> leftSelector)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? Left<TLeftResult, TRight>((leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left))
            : Right<TLeftResult, TRight>(either.Right);

    /// <inheritdoc cref="SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    [PublicAPI]
    public static Either<TLeftResult, TRight> SelectLeft<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> either,
        [InstantHandle] Func<TLeft, Either<TLeftResult, TRight>> leftSelector)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left)
            : Right<TLeftResult, TRight>(either.Right);

    /// <summary> Convert to other right value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    [PublicAPI]
    public static Either<TLeft, TRightResult> SelectRight<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either,
        [InstantHandle] Func<TRight, TRightResult> rightSelector)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? Right<TLeft, TRightResult>((rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right))
            : Left<TLeft, TRightResult>(either.Left);

    /// <inheritdoc cref="SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    [PublicAPI]
    public static Either<TLeft, TRightResult> SelectRight<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either,
        [InstantHandle] Func<TRight, Either<TLeft, TRightResult>> rightSelector)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right)
            : Left<TLeft, TRightResult>(either.Left);

    /// <summary> Convert to other type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    [PublicAPI]
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> either,
        [InstantHandle] Func<TLeft, TLeftResult> leftSelector, [InstantHandle] Func<TRight, TRightResult> rightSelector)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? Left<TLeftResult, TRightResult>((leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left))
            : Right<TLeftResult, TRightResult>((rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right));

    /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    [PublicAPI]
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> either,
        [InstantHandle] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector, [InstantHandle] Func<TRight, TRightResult> rightSelector)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left)
            : Right<TLeftResult, TRightResult>((rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right));

    /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    [PublicAPI]
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> either,
        [InstantHandle] Func<TLeft, TLeftResult> leftSelector, [InstantHandle] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? Left<TLeftResult, TRightResult>((leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left))
            : (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right);

    /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    [PublicAPI]
    public static Either<TLeftResult, TRightResult> Select<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> either,
        [InstantHandle] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        [InstantHandle] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(either.Left)
            : (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(either.Right);

#endregion

#region ValueOrDefault

    /// <summary> Get left value or default </summary>
    /// <param name="either"> Either item </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    [PublicAPI]
    public static TLeft? LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft ? either.Left : default;

    /// <summary> Get left value or default </summary>
    /// <param name="either"> Either item </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    [PublicAPI]
    public static TLeft LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, TLeft defaultValue)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft ? either.Left : defaultValue;

    /// <summary> Get left value or default from generator </summary>
    /// <param name="either"> Either item </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    [PublicAPI]
    public static TLeft LeftOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, [InstantHandle] Func<TLeft> defaultGenerator)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? either.Left
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Get right value or default </summary>
    /// <param name="either"> Either item </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    [PublicAPI]
    public static TRight? RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight ? either.Right : default;

    /// <summary> Get right value or default </summary>
    /// <param name="either"> Either item </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    [PublicAPI]
    public static TRight RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, TRight defaultValue)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight ? either.Right : defaultValue;

    /// <summary> Get right value or default from generator </summary>
    /// <param name="either"> Either item </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    [PublicAPI]
    public static TRight RightOrDefault<TLeft, TRight>(this Either<TLeft, TRight> either, [InstantHandle] Func<TRight> defaultGenerator)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? either.Right
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

#endregion

#region ToValue

    /// <summary> Convert to left value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="rightToLeft"> Right to left converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    [PublicAPI]
    public static TLeft ToLeft<TLeft, TRight>(this Either<TLeft, TRight> either, [InstantHandle] Func<TRight, TLeft> rightToLeft)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? either.Left
            : (rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft))).Invoke(either.Right);

    /// <summary> Convert to right value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftToRight"> Left to right converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    [PublicAPI]
    public static TRight ToRight<TLeft, TRight>(this Either<TLeft, TRight> either, [InstantHandle] Func<TLeft, TRight> leftToRight)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? either.Right
            : (leftToRight ?? throw new ArgumentNullException(nameof(leftToRight))).Invoke(either.Left);

    /// <summary> Convert to other value type </summary>
    /// <param name="either"> Either item </param>
    /// <param name="fromLeft"> Left value converter </param>
    /// <param name="fromRight"> Right value converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TResult"> Left result type </typeparam>
    [PublicAPI]
    public static TResult ToValue<TLeft, TRight, TResult>(this Either<TLeft, TRight> either, [InstantHandle] Func<TLeft, TResult> fromLeft,
        [InstantHandle] Func<TRight, TResult> fromRight)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (fromLeft ?? throw new ArgumentNullException(nameof(fromLeft))).Invoke(either.Left)
            : (fromRight ?? throw new ArgumentNullException(nameof(fromRight))).Invoke(either.Right);

#endregion

#region Utils

    /// <summary> Swap left and right values </summary>
    [PublicAPI]
    public static Either<TRight, TLeft> Invert<TLeft, TRight>(this Either<TLeft, TRight> either)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? Right<TRight, TLeft>(either.Left)
            : Left<TRight, TLeft>(either.Right);

    /// <summary> Select existing left values </summary>
    /// <param name="collection"> Either collection </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Left values collection </returns>
    [PublicAPI]
    [LinqTunnel]
    public static IEnumerable<TLeft> LeftValues<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item is {HasLeft: true})
           .Select(item => item.Left);

    /// <summary> Select existing right values </summary>
    /// <param name="collection"> Either collection </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <returns> Right values collection </returns>
    [PublicAPI]
    [LinqTunnel]
    public static IEnumerable<TRight> RightValues<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item is {HasRight: true})
           .Select(item => item.Right);

#endregion

#region Do

    /// <summary> Do action with left or right value </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftAction"> Left value action </param>
    /// <param name="rightAction"> Right value action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    [PublicAPI]
    public static void Do<TLeft, TRight>(this Either<TLeft, TRight> either, [InstantHandle] Action<TLeft> leftAction,
        [InstantHandle] Action<TRight> rightAction)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasLeft)
            (leftAction ?? throw new ArgumentNullException(nameof(leftAction))).Invoke(either.Left);
        else if (either.HasRight) (rightAction ?? throw new ArgumentNullException(nameof(rightAction))).Invoke(either.Right);
    }

    /// <summary> Do action with left value (if exists) </summary>
    /// <param name="either"> Either item </param>
    /// <param name="leftAction"> Left value action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    [PublicAPI]
    public static void DoLeft<TLeft, TRight>(this Either<TLeft, TRight> either, [InstantHandle] Action<TLeft> leftAction)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasLeft)
            (leftAction ?? throw new ArgumentNullException(nameof(leftAction))).Invoke(either.Left);
    }

    /// <summary> Do action with right value (if exists) </summary>
    /// <param name="either"> Either item </param>
    /// <param name="rightAction"> Right value action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    [PublicAPI]
    public static void DoRight<TLeft, TRight>(this Either<TLeft, TRight> either, [InstantHandle] Action<TRight> rightAction)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasRight)
            (rightAction ?? throw new ArgumentNullException(nameof(rightAction))).Invoke(either.Right);
    }

#endregion
}
