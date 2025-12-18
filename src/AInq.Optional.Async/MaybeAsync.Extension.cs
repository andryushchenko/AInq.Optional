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

public static partial class MaybeAsync
{
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Source value type </typeparam>
    extension<T>(Maybe<T> maybe)
    {
#region Convert

        /// <inheritdoc cref="Maybe.EitherValue{T,TOther}(Maybe{T},Func{TOther})" />
        [PublicAPI, Pure]
        public ValueTask<Either<T, TOther>> EitherValueAsync<TOther>(
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TOther>> asyncOtherGenerator,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? new ValueTask<Either<T, TOther>>(Optional.Either.Left<T, TOther>(maybe.Value))
                : AwaitEither<T, TOther>(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

#endregion

#region SelectAsync

        /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                                                                                           .AsMaybeAsync(cancellation)
                : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

        /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

#endregion

#region SelectOrDefaultAsync

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult?> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            CancellationToken cancellation = default)
        {
            if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                return new ValueTask<TResult?>(default(TResult));
            var selected = (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation);
            return selected.IsCompletedSuccessfully ? new ValueTask<TResult?>(selected.Result) : AwaitNullable(selected);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, [NoEnumeration] TResult defaultValue,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                : new ValueTask<TResult>(defaultValue);

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

#endregion

#region SelectOrDefaultAsync_Maybe

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<TResult?> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                                                                                           .ValueOrDefault(cancellation)
                : new ValueTask<TResult?>(default(TResult));

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},TResult)" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            [NoEnumeration] TResult defaultValue, CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                                                                                           .ValueOrDefault(defaultValue, cancellation)
                : new ValueTask<TResult>(defaultValue);

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                                                                                           .ValueOrDefault(
                                                                                               defaultGenerator
                                                                                               ?? throw new ArgumentNullException(
                                                                                                   nameof(defaultGenerator)),
                                                                                               cancellation)
                : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                                                                                           .ValueOrDefaultAsync(
                                                                                               asyncDefaultGenerator
                                                                                               ?? throw new ArgumentNullException(
                                                                                                   nameof(asyncDefaultGenerator)),
                                                                                               cancellation)
                : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

#endregion

#region Utils

        /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
        [PublicAPI, Pure]
        public ValueTask<T> ValueOrDefaultAsync([InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? new ValueTask<T>(maybe.Value)
                : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

        /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> OrAsync([InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<Maybe<T>>> asyncOtherGenerator,
            CancellationToken cancellation = default)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? new ValueTask<Maybe<T>>(maybe)
                : (asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator))).Invoke(cancellation);

        /// <inheritdoc cref="Maybe.Filter{T}" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> FilterAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> asyncFilter,
            CancellationToken cancellation = default)
        {
            if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue) return new ValueTask<Maybe<T>>(Maybe.None<T>());
            var result = (asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter))).Invoke(maybe.Value, cancellation);
            return result.IsCompletedSuccessfully ? new ValueTask<Maybe<T>>(result.Result ? maybe : Maybe.None<T>()) : AwaitFilter(maybe, result);
        }

#endregion

#region DoAsync

        /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction, CancellationToken cancellation = default)
        {
            if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(maybe.Value, cancellation)
                    .ConfigureAwait(false);
            else await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
            CancellationToken cancellation = default)
        {
            if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(maybe.Value, cancellation)
                    .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.DoIfEmpty{T}" />
        [PublicAPI]
        public async Task DoIfEmptyAsync([InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction,
            CancellationToken cancellation = default)
        {
            if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(cancellation).ConfigureAwait(false);
        }

#endregion

#region DoAsyncWithArgument

        /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},Action{TArgument},TArgument)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(maybe.Value, argument, cancellation)
                    .ConfigureAwait(false);
            else
                await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(argument, cancellation)
                    .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},TArgument)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            TArgument argument, CancellationToken cancellation = default)
        {
            if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(maybe.Value, argument, cancellation)
                    .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.DoIfEmpty{T,TArgument}" />
        [PublicAPI]
        public async Task DoIfEmptyAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction,
            TArgument argument, CancellationToken cancellation = default)
        {
            if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(argument, cancellation)
                    .ConfigureAwait(false);
        }

#endregion
    }
}
