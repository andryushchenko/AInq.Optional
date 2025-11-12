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

public static partial class EitherAsync
{
    /// <param name="eitherTask"> Either item </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    extension<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask)
    {

#region Convert

        /// <inheritdoc cref="Either.MaybeLeft{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<TLeft>> MaybeLeft(CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Maybe<TLeft>>(eitherTask.Result.MaybeLeft())
                : AwaitMaybeLeft(eitherTask, cancellation);

        /// <inheritdoc cref="Either.MaybeRight{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<TRight>> MaybeRight(CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Maybe<TRight>>(eitherTask.Result.MaybeRight())
                : AwaitMaybeRight(eitherTask, cancellation);

        /// <inheritdoc cref="Either.TryLeft{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<Try<TLeft>> TryLeft(CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Try<TLeft>>(eitherTask.Result.TryLeft())
                : AwaitTryLeft(eitherTask, cancellation);

        /// <inheritdoc cref="Either.TryRight{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<Try<TRight>> TryRight(CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Try<TRight>>(eitherTask.Result.TryRight())
                : AwaitTryRight(eitherTask, cancellation);

#endregion
        
#region SelectLeft

        /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeftResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Either<TLeftResult, TRight>>(
                    eitherTask.Result.SelectLeft(leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))))
                : AwaitSelectLeft(eitherTask, leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)), cancellation);

        /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeftResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRight>> leftSelector, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Either<TLeftResult, TRight>>(
                    eitherTask.Result.SelectLeft(leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))))
                : AwaitSelectLeft(eitherTask, leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)), cancellation);

#endregion

#region SelectRight

        /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeft, TRightResult>> SelectRight<TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Either<TLeft, TRightResult>>(
                    eitherTask.Result.SelectRight(rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
                : AwaitSelectRight(eitherTask, rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)), cancellation);

        /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeft, TRightResult>> SelectRight<TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeft, TRightResult>> rightSelector, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Either<TLeft, TRightResult>>(
                    eitherTask.Result.SelectRight(rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
                : AwaitSelectRight(eitherTask, rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)), cancellation);

#endregion

#region Select

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> Select<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(
                    leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                    rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
                : AwaitSelect(eitherTask,
                    leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                    rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                    cancellation);

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> Select<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(
                    leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                    rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
                : AwaitSelect(eitherTask,
                    leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                    rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                    cancellation);

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> Select<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(
                    leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                    rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
                : AwaitSelect(eitherTask,
                    leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                    rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                    cancellation);

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> Select<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(
                    leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                    rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
                : AwaitSelect(eitherTask,
                    leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                    rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                    cancellation);

#endregion

#region SelectLeftAsync

        /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeftResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.SelectLeftAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation)
                : AwaitSelectLeft(eitherTask, asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation);

        /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeftResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> asyncLeftSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.SelectLeftAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation)
                : AwaitSelectLeft(eitherTask, asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation);

#endregion

#region SelectRightAsync

        /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.SelectRightAsync(asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation)
                : AwaitSelectRight(eitherTask, asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation);

        /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> asyncRightSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.SelectRightAsync(asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation)
                : AwaitSelectRight(eitherTask, asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation);

#endregion

#region SelectAsync

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                    asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                    cancellation)
                : AwaitSelect(eitherTask,
                    asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                    asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                    cancellation);

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                    asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                    cancellation)
                : AwaitSelect(eitherTask,
                    asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                    asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                    cancellation);

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                    asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                    cancellation)
                : AwaitSelect(eitherTask,
                    asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                    asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                    cancellation);

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                    asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                    cancellation)
                : AwaitSelect(eitherTask,
                    asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                    asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                    cancellation);

#endregion

#region ValueOrDefault

        /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
        [PublicAPI, Pure]
        public ValueTask<TLeft?> LeftOrDefault(CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TLeft?>(eitherTask.Result.LeftOrDefault())
                : AwaitLeftOrDefault(eitherTask, cancellation);

        /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
        [PublicAPI, Pure]
        public ValueTask<TLeft> LeftOrDefault([NoEnumeration] TLeft defaultValue, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TLeft>(eitherTask.Result.LeftOrDefault(defaultValue))
                : AwaitLeftOrDefault(eitherTask, defaultValue, cancellation);

        /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
        [PublicAPI, Pure]
        public ValueTask<TLeft> LeftOrDefault([InstantHandle(RequireAwait = true)] Func<TLeft> defaultGenerator,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TLeft>(eitherTask.Result.LeftOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
                : AwaitLeftOrDefault(eitherTask, defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)), cancellation);

        /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
        [PublicAPI, Pure]
        public ValueTask<TRight?> RightOrDefault(CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TRight?>(eitherTask.Result.RightOrDefault())
                : AwaitRightOrDefault(eitherTask, cancellation);

        /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
        [PublicAPI, Pure]
        public ValueTask<TRight> RightOrDefault([NoEnumeration] TRight defaultValue, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TRight>(eitherTask.Result.RightOrDefault(defaultValue))
                : AwaitRightOrDefault(eitherTask, defaultValue, cancellation);

        /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
        [PublicAPI, Pure]
        public ValueTask<TRight> RightOrDefault([InstantHandle(RequireAwait = true)] Func<TRight> defaultGenerator,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TRight>(
                    eitherTask.Result.RightOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
                : AwaitRightOrDefault(eitherTask, defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)), cancellation);

#endregion

#region ValueOrDefaultAsync

        /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
        [PublicAPI, Pure]
        public ValueTask<TLeft> LeftOrDefaultAsync(
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TLeft>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.LeftOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                    cancellation)
                : AwaitLeftOrDefault(eitherTask,
                    asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                    cancellation);

        /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
        [PublicAPI, Pure]
        public ValueTask<TRight> RightOrDefaultAsync(
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.RightOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                    cancellation)
                : AwaitRightOrDefault(eitherTask,
                    asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                    cancellation);

#endregion

#region ToValue

        /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<TLeft> ToLeft([InstantHandle(RequireAwait = true)] Func<TRight, TLeft> rightToLeft, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TLeft>(eitherTask.Result.ToLeft(rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft))))
                : AwaitToLeft(eitherTask, rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft)), cancellation);

        /// <inheritdoc cref="Either.ToRight{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<TRight> ToRight([InstantHandle(RequireAwait = true)] Func<TLeft, TRight> leftToRight,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TRight>(eitherTask.Result.ToRight(leftToRight ?? throw new ArgumentNullException(nameof(leftToRight))))
                : AwaitToRight(eitherTask, leftToRight ?? throw new ArgumentNullException(nameof(leftToRight)), cancellation);

        /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}" />
        [PublicAPI, Pure]
        public ValueTask<TResult> ToValue<TResult>([InstantHandle(RequireAwait = true)] Func<TLeft, TResult> fromLeft,
            [InstantHandle(RequireAwait = true)] Func<TRight, TResult> fromRight, CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TResult>(eitherTask.Result.ToValue(fromLeft ?? throw new ArgumentNullException(nameof(fromLeft)),
                    fromRight ?? throw new ArgumentNullException(nameof(fromRight))))
                : AwaitToValue(eitherTask,
                    fromLeft ?? throw new ArgumentNullException(nameof(fromLeft)),
                    fromRight ?? throw new ArgumentNullException(nameof(fromRight)),
                    cancellation);

#endregion

#region ToValueAsync

        /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<TLeft> ToLeftAsync([InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TLeft>> asyncRightToLeft,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.ToLeftAsync(asyncRightToLeft ?? throw new ArgumentNullException(nameof(asyncRightToLeft)), cancellation)
                : AwaitToLeft(eitherTask, asyncRightToLeft ?? throw new ArgumentNullException(nameof(asyncRightToLeft)), cancellation);

        /// <inheritdoc cref="Either.ToRight{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<TRight> ToRightAsync([InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TRight>> asyncLeftToRight,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.ToRightAsync(asyncLeftToRight ?? throw new ArgumentNullException(nameof(asyncLeftToRight)), cancellation)
                : AwaitToRight(eitherTask, asyncLeftToRight ?? throw new ArgumentNullException(nameof(asyncLeftToRight)), cancellation);

        /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}" />
        [PublicAPI, Pure]
        public ValueTask<TResult> ToValueAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TResult>> asyncFromLeft,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TResult>> asyncFromRight,
            CancellationToken cancellation = default)
            => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result.ToValueAsync(asyncFromLeft ?? throw new ArgumentNullException(nameof(asyncFromLeft)),
                    asyncFromRight ?? throw new ArgumentNullException(nameof(asyncFromRight)),
                    cancellation)
                : AwaitToValue(eitherTask,
                    asyncFromLeft ?? throw new ArgumentNullException(nameof(asyncFromLeft)),
                    asyncFromRight ?? throw new ArgumentNullException(nameof(asyncFromRight)),
                    cancellation);

#endregion

#region Do

        /// <inheritdoc cref="Either.Do{TLeft,TRight}" />
        [PublicAPI]
        public ValueTask Do([InstantHandle(RequireAwait = true)] Action<TLeft> leftAction,
            [InstantHandle(RequireAwait = true)] Action<TRight> rightAction, CancellationToken cancellation = default)
        {
            if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDo(eitherTask,
                    leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                    rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                    cancellation);
            eitherTask.Result.Do(leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                rightAction ?? throw new ArgumentNullException(nameof(rightAction)));
            return default;
        }

        /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}" />
        [PublicAPI]
        public ValueTask DoLeft([InstantHandle(RequireAwait = true)] Action<TLeft> leftAction, CancellationToken cancellation = default)
        {
            if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDoLeft(eitherTask, leftAction ?? throw new ArgumentNullException(nameof(leftAction)), cancellation);
            eitherTask.Result.DoLeft(leftAction ?? throw new ArgumentNullException(nameof(leftAction)));
            return default;
        }

        /// <inheritdoc cref="Either.DoRight{TLeft,TRight}" />
        [PublicAPI]
        public ValueTask DoRight([InstantHandle(RequireAwait = true)] Action<TRight> rightAction, CancellationToken cancellation = default)
        {
            if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDoRight(eitherTask, rightAction ?? throw new ArgumentNullException(nameof(rightAction)), cancellation);
            eitherTask.Result.DoRight(rightAction ?? throw new ArgumentNullException(nameof(rightAction)));
            return default;
        }

#endregion

#region DoWithArgument

        /// <inheritdoc cref="Either.Do{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public ValueTask Do<TArgument>([InstantHandle(RequireAwait = true)] Action<TLeft, TArgument> leftAction,
            [InstantHandle(RequireAwait = true)] Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation = default)
        {
            if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDo(eitherTask,
                    leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                    rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                    argument,
                    cancellation);
            eitherTask.Result.Do(leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                argument);
            return default;
        }

        /// <inheritdoc cref="Either.DoLeft{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public ValueTask DoLeft<TArgument>([InstantHandle(RequireAwait = true)] Action<TLeft, TArgument> leftAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDoLeft(eitherTask, leftAction ?? throw new ArgumentNullException(nameof(leftAction)), argument, cancellation);
            eitherTask.Result.DoLeft(leftAction ?? throw new ArgumentNullException(nameof(leftAction)), argument);
            return default;
        }

        /// <inheritdoc cref="Either.DoRight{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public ValueTask DoRight<TArgument>([InstantHandle(RequireAwait = true)] Action<TRight, TArgument> rightAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDoRight(eitherTask, rightAction ?? throw new ArgumentNullException(nameof(rightAction)), argument, cancellation);
            eitherTask.Result.DoRight(rightAction ?? throw new ArgumentNullException(nameof(rightAction)), argument);
            return default;
        }

#endregion

#region DoAsync

        /// <inheritdoc cref="Either.Do{TLeft,TRight}" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction, CancellationToken cancellation = default)
        {
            var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result
                : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (either.HasLeft)
                await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction))).Invoke(either.Left, cancellation)
                                                                                                   .ConfigureAwait(false);
            else
                await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction))).Invoke(either.Right, cancellation)
                    .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}" />
        [PublicAPI]
        public async Task DoLeftAsync([InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction,
            CancellationToken cancellation = default)
        {
            var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result
                : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (either.HasLeft)
                await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction))).Invoke(either.Left, cancellation)
                                                                                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Either.DoRight{TLeft,TRight}" />
        [PublicAPI]
        public async Task DoRightAsync([InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction,
            CancellationToken cancellation = default)
        {
            var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result
                : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (either.HasRight)
                await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction))).Invoke(either.Right, cancellation)
                    .ConfigureAwait(false);
        }

#endregion

#region DoAsyncWithArgument

        /// <inheritdoc cref="Either.Do{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction,
            [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result
                : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (either.HasLeft)
                await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction))).Invoke(either.Left, argument, cancellation)
                                                                                                   .ConfigureAwait(false);
            else
                await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction))).Invoke(either.Right, argument, cancellation)
                    .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Either.DoLeft{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public async Task DoLeftAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction,
            TArgument argument, CancellationToken cancellation = default)
        {
            var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result
                : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (either.HasLeft)
                await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction))).Invoke(either.Left, argument, cancellation)
                                                                                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Either.DoRight{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public async Task DoRightAsync<TArgument>(
            [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
                ? eitherTask.Result
                : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (either.HasRight)
                await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction))).Invoke(either.Right, argument, cancellation)
                    .ConfigureAwait(false);
        }

#endregion
    }
}
