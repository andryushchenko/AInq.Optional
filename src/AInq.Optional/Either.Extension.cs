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

public static partial class Either
{
    /// <param name="either"> Either item </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    extension<TLeft, TRight>(Either<TLeft, TRight> either)
    {
#region Convert

        /// <summary> Get left value or none </summary>
        /// <returns> Maybe with left value </returns>
        [PublicAPI, Pure]
        public Maybe<TLeft> MaybeLeft()
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            return either.HasLeft ? Maybe<TLeft>.FromValue(either.Left) : Maybe<TLeft>.None;
        }

        /// <summary> Get right value or none </summary>
        /// <returns> Maybe with right value </returns>
        [PublicAPI, Pure]
        public Maybe<TRight> MaybeRight()
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            return either.HasRight ? Maybe<TRight>.FromValue(either.Right) : Maybe<TRight>.None;
        }

        /// <summary> Try get left value </summary>
        /// <returns> Try with left value </returns>
        [PublicAPI, Pure]
        public Try<TLeft> TryLeft()
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            return either.HasLeft ? Try<TLeft>.FromValue(either.Left) : Try<TLeft>.FromError(new InvalidOperationException("No left value"));
        }

        /// <summary> Try get right value </summary>
        /// <returns> Try with right value </returns>
        [PublicAPI, Pure]
        public Try<TRight> TryRight()
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            return either.HasRight ? Try<TRight>.FromValue(either.Right) : Try<TRight>.FromError(new InvalidOperationException("No right value"));
        }

#endregion

#region Select

        /// <summary> Convert to other left value type </summary>
        /// <param name="leftSelector"> Left value converter </param>
        /// <typeparam name="TLeftResult"> Left result type </typeparam>
        [PublicAPI, Pure]
        public Either<TLeftResult, TRight> SelectLeft<TLeftResult>([InstantHandle] Func<TLeft, TLeftResult> leftSelector)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftSelector ?? throw new ArgumentNullException(nameof(leftSelector));
            return either.HasLeft
                ? Either<TLeftResult, TRight>.FromLeft(leftSelector.Invoke(either.Left))
                : Either<TLeftResult, TRight>.FromRight(either.Right);
        }

        /// <inheritdoc cref="SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
        [PublicAPI, Pure]
        public Either<TLeftResult, TRight> SelectLeft<TLeftResult>([InstantHandle] Func<TLeft, Either<TLeftResult, TRight>> leftSelector)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftSelector ?? throw new ArgumentNullException(nameof(leftSelector));
            return either.HasLeft ? leftSelector.Invoke(either.Left) : Either<TLeftResult, TRight>.FromRight(either.Right);
        }

        /// <summary> Convert to other right value type </summary>
        /// <param name="rightSelector"> Right value converter </param>
        /// <typeparam name="TRightResult"> Right result type </typeparam>
        [PublicAPI, Pure]
        public Either<TLeft, TRightResult> SelectRight<TRightResult>([InstantHandle] Func<TRight, TRightResult> rightSelector)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = rightSelector ?? throw new ArgumentNullException(nameof(rightSelector));
            return either.HasRight
                ? Either<TLeft, TRightResult>.FromRight(rightSelector.Invoke(either.Right))
                : Either<TLeft, TRightResult>.FromLeft(either.Left);
        }

        /// <inheritdoc cref="SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public Either<TLeft, TRightResult> SelectRight<TRightResult>([InstantHandle] Func<TRight, Either<TLeft, TRightResult>> rightSelector)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = rightSelector ?? throw new ArgumentNullException(nameof(rightSelector));
            return either.HasRight ? rightSelector.Invoke(either.Right) : Either<TLeft, TRightResult>.FromLeft(either.Left);
        }

        /// <summary> Convert to other type </summary>
        /// <param name="leftSelector"> Left value converter </param>
        /// <param name="rightSelector"> Right value converter </param>
        /// <typeparam name="TLeftResult"> Left result type </typeparam>
        /// <typeparam name="TRightResult"> Right result type </typeparam>
        [PublicAPI, Pure]
        public Either<TLeftResult, TRightResult> Select<TLeftResult, TRightResult>([InstantHandle] Func<TLeft, TLeftResult> leftSelector,
            [InstantHandle] Func<TRight, TRightResult> rightSelector)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftSelector ?? throw new ArgumentNullException(nameof(leftSelector));
            _ = rightSelector ?? throw new ArgumentNullException(nameof(rightSelector));
            return either.HasLeft
                ? Either<TLeftResult, TRightResult>.FromLeft(leftSelector.Invoke(either.Left))
                : Either<TLeftResult, TRightResult>.FromRight(rightSelector.Invoke(either.Right));
        }

        /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public Either<TLeftResult, TRightResult> Select<TLeftResult, TRightResult>(
            [InstantHandle] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector, [InstantHandle] Func<TRight, TRightResult> rightSelector)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftSelector ?? throw new ArgumentNullException(nameof(leftSelector));
            _ = rightSelector ?? throw new ArgumentNullException(nameof(rightSelector));
            return either.HasLeft
                ? leftSelector.Invoke(either.Left)
                : Either<TLeftResult, TRightResult>.FromRight(rightSelector.Invoke(either.Right));
        }

        /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public Either<TLeftResult, TRightResult> Select<TLeftResult, TRightResult>([InstantHandle] Func<TLeft, TLeftResult> leftSelector,
            [InstantHandle] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftSelector ?? throw new ArgumentNullException(nameof(leftSelector));
            _ = rightSelector ?? throw new ArgumentNullException(nameof(rightSelector));
            return either.HasLeft ? Either<TLeftResult, TRightResult>.FromLeft(leftSelector.Invoke(either.Left)) : rightSelector.Invoke(either.Right);
        }

        /// <inheritdoc cref="Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public Either<TLeftResult, TRightResult> Select<TLeftResult, TRightResult>(
            [InstantHandle] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
            [InstantHandle] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftSelector ?? throw new ArgumentNullException(nameof(leftSelector));
            _ = rightSelector ?? throw new ArgumentNullException(nameof(rightSelector));
            return either.HasLeft ? leftSelector.Invoke(either.Left) : rightSelector.Invoke(either.Right);
        }

#endregion

#region ValueOrDefault

        /// <summary> Get left value or default </summary>
        [PublicAPI, Pure]
        public TLeft? LeftOrDefault()
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            return either.HasLeft ? either.Left : default;
        }

        /// <summary> Get left value or default </summary>
        /// <param name="defaultValue"> Default value </param>
        [PublicAPI, Pure]
        public TLeft LeftOrDefault([NoEnumeration] TLeft defaultValue)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            return either.HasLeft ? either.Left : defaultValue;
        }

        /// <summary> Get left value or default from generator </summary>
        /// <param name="defaultGenerator"> Default value generator </param>
        [PublicAPI, Pure]
        public TLeft LeftOrDefault([InstantHandle] Func<TLeft> defaultGenerator)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator));
            return either.HasLeft ? either.Left : defaultGenerator.Invoke();
        }

        /// <summary> Get right value or default </summary>
        [PublicAPI, Pure]
        public TRight? RightOrDefault()
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            return either.HasRight ? either.Right : default;
        }

        /// <summary> Get right value or default </summary>
        /// <param name="defaultValue"> Default value </param>
        [PublicAPI, Pure]
        public TRight RightOrDefault([NoEnumeration] TRight defaultValue)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            return either.HasRight ? either.Right : defaultValue;
        }

        /// <summary> Get right value or default from generator </summary>
        /// <param name="defaultGenerator"> Default value generator </param>
        [PublicAPI, Pure]
        public TRight RightOrDefault([InstantHandle] Func<TRight> defaultGenerator)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator));
            return either.HasRight ? either.Right : defaultGenerator.Invoke();
        }

#endregion

#region ToValue

        /// <summary> Convert to left value type </summary>
        /// <param name="rightToLeft"> Right to left converter </param>
        [PublicAPI, Pure]
        public TLeft ToLeft([InstantHandle] Func<TRight, TLeft> rightToLeft)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft));
            return either.HasLeft ? either.Left : rightToLeft.Invoke(either.Right);
        }

        /// <summary> Convert to right value type </summary>
        /// <param name="leftToRight"> Left to right converter </param>
        [PublicAPI, Pure]
        public TRight ToRight([InstantHandle] Func<TLeft, TRight> leftToRight)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftToRight ?? throw new ArgumentNullException(nameof(leftToRight));
            return either.HasRight ? either.Right : leftToRight.Invoke(either.Left);
        }

        /// <summary> Convert to other value type </summary>
        /// <param name="fromLeft"> Left value converter </param>
        /// <param name="fromRight"> Right value converter </param>
        /// <typeparam name="TResult"> Left result type </typeparam>
        [PublicAPI, Pure]
        public TResult ToValue<TResult>([InstantHandle] Func<TLeft, TResult> fromLeft, [InstantHandle] Func<TRight, TResult> fromRight)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = fromLeft ?? throw new ArgumentNullException(nameof(fromLeft));
            _ = fromRight ?? throw new ArgumentNullException(nameof(fromRight));
            return either.HasLeft ? fromLeft.Invoke(either.Left) : fromRight.Invoke(either.Right);
        }

#endregion

#region Do

        /// <summary> Do action with left or right value </summary>
        /// <param name="leftAction"> Left value action </param>
        /// <param name="rightAction"> Right value action </param>
        [PublicAPI]
        public void Do([InstantHandle] Action<TLeft> leftAction, [InstantHandle] Action<TRight> rightAction)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftAction ?? throw new ArgumentNullException(nameof(leftAction));
            _ = rightAction ?? throw new ArgumentNullException(nameof(rightAction));
            if (either.HasLeft) leftAction.Invoke(either.Left);
            else rightAction.Invoke(either.Right);
        }

        /// <summary> Do action with left value (if exists) </summary>
        /// <param name="leftAction"> Left value action </param>
        [PublicAPI]
        public void DoLeft([InstantHandle] Action<TLeft> leftAction)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftAction ?? throw new ArgumentNullException(nameof(leftAction));
            if (either.HasLeft) leftAction.Invoke(either.Left);
        }

        /// <summary> Do action with right value (if exists) </summary>
        /// <param name="rightAction"> Right value action </param>
        [PublicAPI]
        public void DoRight([InstantHandle] Action<TRight> rightAction)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = rightAction ?? throw new ArgumentNullException(nameof(rightAction));
            if (either.HasRight) rightAction.Invoke(either.Right);
        }

#endregion

#region DoWithArgument

        /// <summary> Do action with left or right value with additional argument </summary>
        /// <param name="leftAction"> Left value action </param>
        /// <param name="rightAction"> Right value action </param>
        /// <param name="argument"> Additional action argument </param>
        /// <typeparam name="TArgument"> Additional action argument type </typeparam>
        [PublicAPI]
        public void Do<TArgument>([InstantHandle] Action<TLeft, TArgument> leftAction, [InstantHandle] Action<TRight, TArgument> rightAction,
            TArgument argument)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftAction ?? throw new ArgumentNullException(nameof(leftAction));
            _ = rightAction ?? throw new ArgumentNullException(nameof(rightAction));
            if (either.HasLeft) leftAction.Invoke(either.Left, argument);
            else rightAction.Invoke(either.Right, argument);
        }

        /// <summary> Do action with left value (if exists) with additional argument </summary>
        /// <param name="leftAction"> Left value action </param>
        /// <param name="argument"> Additional action argument </param>
        /// <typeparam name="TArgument"> Additional action argument type </typeparam>
        [PublicAPI]
        public void DoLeft<TArgument>([InstantHandle] Action<TLeft, TArgument> leftAction, TArgument argument)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = leftAction ?? throw new ArgumentNullException(nameof(leftAction));
            if (either.HasLeft) leftAction.Invoke(either.Left, argument);
        }

        /// <summary> Do action with right value (if exists) with additional argument </summary>
        /// <param name="rightAction"> Right value action </param>
        /// <param name="argument"> Additional action argument </param>
        /// <typeparam name="TArgument"> Additional action argument type </typeparam>
        [PublicAPI]
        public void DoRight<TArgument>([InstantHandle] Action<TRight, TArgument> rightAction, TArgument argument)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = rightAction ?? throw new ArgumentNullException(nameof(rightAction));
            if (either.HasRight) rightAction.Invoke(either.Right, argument);
        }

#endregion
    }
}
