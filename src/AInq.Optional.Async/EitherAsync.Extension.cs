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
    /// <param name="either"> Either item </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    extension<TLeft, TRight>(Either<TLeft, TRight> either)
    {
#region SelectLeftAsync

        /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeftResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector));
            return either.HasLeft
                ? asyncLeftSelector.Invoke(either.Left, cancellation).AsEitherAsync<TLeftResult, TRight>(cancellation)
                : new ValueTask<Either<TLeftResult, TRight>>(Either.Right<TLeftResult, TRight>(either.Right));
        }

        /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeftResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> asyncLeftSelector,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector));
            return either.HasLeft
                ? asyncLeftSelector.Invoke(either.Left, cancellation)
                : new ValueTask<Either<TLeftResult, TRight>>(Either.Right<TLeftResult, TRight>(either.Right));
        }

#endregion

#region SelectRightAsync

        /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector));
            return either.HasRight
                ? asyncRightSelector.Invoke(either.Right, cancellation).AsEitherAsync<TLeft, TRightResult>(cancellation)
                : new ValueTask<Either<TLeft, TRightResult>>(Either.Left<TLeft, TRightResult>(either.Left));
        }

        /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> asyncRightSelector,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector));
            return either.HasRight
                ? asyncRightSelector.Invoke(either.Right, cancellation)
                : new ValueTask<Either<TLeft, TRightResult>>(Either.Left<TLeft, TRightResult>(either.Left));
        }

#endregion

#region SelectAsync

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector));
            _ = asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector));
            return either.HasLeft
                ? asyncLeftSelector.Invoke(either.Left, cancellation).AsEitherAsync<TLeftResult, TRightResult>(cancellation)
                : asyncRightSelector.Invoke(either.Right, cancellation).AsEitherAsync<TLeftResult, TRightResult>(cancellation);
        }

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector));
            _ = asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector));
            return either.HasLeft
                ? asyncLeftSelector.Invoke(either.Left, cancellation)
                : asyncRightSelector.Invoke(either.Right, cancellation).AsEitherAsync<TLeftResult, TRightResult>(cancellation);
        }

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector));
            _ = asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector));
            return either.HasLeft
                ? asyncLeftSelector.Invoke(either.Left, cancellation).AsEitherAsync<TLeftResult, TRightResult>(cancellation)
                : asyncRightSelector.Invoke(either.Right, cancellation);
        }

        /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeftResult, TRightResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector));
            _ = asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector));
            return either.HasLeft ? asyncLeftSelector.Invoke(either.Left, cancellation) : asyncRightSelector.Invoke(either.Right, cancellation);
        }

#endregion

#region ValueOrDefaultAsync

        /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
        [PublicAPI, Pure]
        public ValueTask<TLeft> LeftOrDefaultAsync(
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TLeft>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator));
            return either.HasLeft ? new ValueTask<TLeft>(either.Left) : asyncDefaultGenerator.Invoke(cancellation);
        }

        /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
        [PublicAPI, Pure]
        public ValueTask<TRight> RightOrDefaultAsync(
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator));
            return either.HasRight ? new ValueTask<TRight>(either.Right) : asyncDefaultGenerator.Invoke(cancellation);
        }

#endregion

#region ToValueAsync

        /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<TLeft> ToLeftAsync([InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TLeft>> asyncRightToLeft,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncRightToLeft ?? throw new ArgumentNullException(nameof(asyncRightToLeft));
            return either.HasLeft ? new ValueTask<TLeft>(either.Left) : asyncRightToLeft.Invoke(either.Right, cancellation);
        }

        /// <inheritdoc cref="Either.ToRight{TLeft,TRight}" />
        [PublicAPI, Pure]
        public ValueTask<TRight> ToRightAsync([InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TRight>> asyncLeftToRight,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftToRight ?? throw new ArgumentNullException(nameof(asyncLeftToRight));
            return either.HasRight ? new ValueTask<TRight>(either.Right) : asyncLeftToRight.Invoke(either.Left, cancellation);
        }

        /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}" />
        [PublicAPI, Pure]
        public ValueTask<TResult> ToValueAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TResult>> asyncFromLeft,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TResult>> asyncFromRight,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncFromLeft ?? throw new ArgumentNullException(nameof(asyncFromLeft));
            _ = asyncFromRight ?? throw new ArgumentNullException(nameof(asyncFromRight));
            return either.HasLeft ? asyncFromLeft.Invoke(either.Left, cancellation) : asyncFromRight.Invoke(either.Right, cancellation);
        }

#endregion

#region DoAsync

        /// <inheritdoc cref="Either.Do{TLeft,TRight}" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction,
            [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction, CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction));
            _ = asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction));
            if (either.HasLeft) await asyncLeftAction.Invoke(either.Left, cancellation).ConfigureAwait(false);
            else await asyncRightAction.Invoke(either.Right, cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}" />
        [PublicAPI]
        public async Task DoLeftAsync([InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction));
            if (either.HasLeft) await asyncLeftAction.Invoke(either.Left, cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Either.DoRight{TLeft,TRight}" />
        [PublicAPI]
        public async Task DoRightAsync([InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction));
            if (either.HasRight) await asyncRightAction.Invoke(either.Right, cancellation).ConfigureAwait(false);
        }

#endregion

#region DoAsyncWithArgument

        /// <inheritdoc cref="Either.Do{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction,
            [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction));
            _ = asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction));
            if (either.HasLeft) await asyncLeftAction.Invoke(either.Left, argument, cancellation).ConfigureAwait(false);
            else await asyncRightAction.Invoke(either.Right, argument, cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Either.DoLeft{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public async Task DoLeftAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction,
            TArgument argument, CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction));
            if (either.HasLeft) await asyncLeftAction.Invoke(either.Left, argument, cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Either.DoRight{TLeft,TRight,TArgument}" />
        [PublicAPI]
        public async Task DoRightAsync<TArgument>(
            [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            _ = either ?? throw new ArgumentNullException(nameof(either));
            _ = asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction));
            if (either.HasRight) await asyncRightAction.Invoke(either.Right, argument, cancellation).ConfigureAwait(false);
        }

#endregion
    }
}
