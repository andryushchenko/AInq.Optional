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
    /// <param name="maybeValueTask"> Maybe item </param>
    /// <typeparam name="T"> Source value type </typeparam>
    extension<T>(ValueTask<Maybe<T>> maybeValueTask)
    {
#region Convert

        /// <inheritdoc cref="Maybe.EitherValue{T,TOther}(Maybe{T},TOther)" />
        [PublicAPI, Pure]
        public ValueTask<Either<T, TOther>> Either<TOther>([NoEnumeration] TOther other, CancellationToken cancellation = default)
            => maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<Either<T, TOther>>(maybeValueTask.Result.EitherValue(other))
                : AwaitEither(maybeValueTask.AsTask(), other, cancellation);

        /// <inheritdoc cref="Maybe.EitherValue{T,TOther}(Maybe{T},Func{TOther})" />
        [PublicAPI, Pure]
        public ValueTask<Either<T, TOther>> Either<TOther>([InstantHandle(RequireAwait = true)] Func<TOther> otherGenerator,
            CancellationToken cancellation = default)
        {
            _ = otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<Either<T, TOther>>(maybeValueTask.Result.EitherValue(otherGenerator))
                : AwaitEither(maybeValueTask.AsTask(), otherGenerator, cancellation);
        }

        /// <inheritdoc cref="Maybe.EitherValue{T,TOther}(Maybe{T},Func{TOther})" />
        [PublicAPI, Pure]
        public ValueTask<Either<T, TOther>> EitherAsync<TOther>(
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TOther>> asyncOtherGenerator,
            CancellationToken cancellation = default)
        {
            _ = asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.EitherValueAsync(asyncOtherGenerator, cancellation)
                : AwaitEither(maybeValueTask.AsTask(), asyncOtherGenerator, cancellation);
        }

        /// <inheritdoc cref="Maybe.TryValue{T}" />
        [PublicAPI, Pure]
        public ValueTask<Try<T>> TryValue(CancellationToken cancellation = default)
            => maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<Try<T>>(maybeValueTask.Result.TryValue())
                : AwaitTry(maybeValueTask.AsTask(), cancellation);

#endregion

#region Select

        /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<TResult>> Select<TResult>([InstantHandle(RequireAwait = true)] Func<T, TResult> selector,
            CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<Maybe<TResult>>(maybeValueTask.Result.Select(selector))
                : AwaitSelect(maybeValueTask.AsTask(), selector, cancellation);
        }

        /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<TResult>> Select<TResult>([InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector,
            CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<Maybe<TResult>>(maybeValueTask.Result.Select(selector))
                : AwaitSelect(maybeValueTask.AsTask(), selector, cancellation);
        }

#endregion

#region SelectAsync

        /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectAsync(asyncSelector, cancellation)
                : AwaitSelect(maybeValueTask.AsTask(), asyncSelector, cancellation);
        }

        /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectAsync(asyncSelector, cancellation)
                : AwaitSelect(maybeValueTask.AsTask(), asyncSelector, cancellation);
        }

#endregion

#region SelectOrDefault

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult?> SelectOrDefault<TResult>([InstantHandle(RequireAwait = true)] Func<T, TResult> selector,
            CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<TResult?>(maybeValueTask.Result.SelectOrDefault(selector))
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), selector, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefault<TResult>([InstantHandle(RequireAwait = true)] Func<T, TResult> selector,
            [NoEnumeration] TResult defaultValue, CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector, defaultValue))
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), selector, defaultValue, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefault<TResult>([InstantHandle(RequireAwait = true)] Func<T, TResult> selector,
            [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            _ = defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector, defaultGenerator))
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), selector, defaultGenerator, cancellation);
        }

#endregion

#region SelectOrDefault_Maybe

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<TResult?> SelectOrDefault<TResult>([InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector,
            CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<TResult?>(maybeValueTask.Result.SelectOrDefault(selector))
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), selector, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},TResult)" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefault<TResult>([InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector,
            [NoEnumeration] TResult defaultValue, CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector, defaultValue))
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), selector, defaultValue, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefault<TResult>([InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector,
            [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            _ = defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector, defaultGenerator))
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), selector, defaultGenerator, cancellation);
        }

#endregion

#region SelectOrDefaultAsync

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult?> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, cancellation)
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), asyncSelector, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, [NoEnumeration] TResult defaultValue,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), asyncSelector, defaultValue, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            _ = defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), asyncSelector, defaultGenerator, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            _ = asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), asyncSelector, asyncDefaultGenerator, cancellation);
        }

#endregion

#region SelectOrDefaultAsync_Maybe

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<TResult?> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, cancellation)
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), asyncSelector, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},TResult)" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            [NoEnumeration] TResult defaultValue, CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), asyncSelector, defaultValue, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            _ = defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), asyncSelector, defaultGenerator, cancellation);
        }

        /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
        [PublicAPI, Pure]
        public ValueTask<TResult> SelectOrDefaultAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            _ = asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
                : AwaitSelectOrDefault(maybeValueTask.AsTask(), asyncSelector, asyncDefaultGenerator, cancellation);
        }

#endregion

#region ValueOrDefault

        /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T})" />
        [PublicAPI, Pure]
        public ValueTask<T?> ValueOrDefault(CancellationToken cancellation = default)
            => maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<T?>(maybeValueTask.Result.ValueOrDefault())
                : AwaitValueOrDefault(maybeValueTask.AsTask(), cancellation);

        /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
        [PublicAPI, Pure]
        public ValueTask<T> ValueOrDefault([NoEnumeration] T defaultValue, CancellationToken cancellation = default)
            => maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<T>(maybeValueTask.Result.ValueOrDefault(defaultValue))
                : AwaitValueOrDefault(maybeValueTask.AsTask(), defaultValue, cancellation);

        /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
        [PublicAPI, Pure]
        public ValueTask<T> ValueOrDefault([InstantHandle(RequireAwait = true)] Func<T> defaultGenerator, CancellationToken cancellation = default)
        {
            _ = defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<T>(maybeValueTask.Result.ValueOrDefault(defaultGenerator))
                : AwaitValueOrDefault(maybeValueTask.AsTask(), defaultGenerator, cancellation);
        }

        /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
        [PublicAPI, Pure]
        public ValueTask<T> ValueOrDefaultAsync([InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator,
            CancellationToken cancellation = default)
        {
            _ = asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
                : AwaitValueOrDefault(maybeValueTask.AsTask(), asyncDefaultGenerator, cancellation);
        }

#endregion

#region Or

        /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> Or(Maybe<T> other, CancellationToken cancellation = default)
        {
            _ = other ?? throw new ArgumentNullException(nameof(other));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Or(other))
                : AwaitOr(maybeValueTask.AsTask(), other, cancellation);
        }

        /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Func{Maybe{T}})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> Or([InstantHandle(RequireAwait = true)] Func<Maybe<T>> otherGenerator, CancellationToken cancellation = default)
        {
            _ = otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Or(otherGenerator))
                : AwaitOr(maybeValueTask.AsTask(), otherGenerator, cancellation);
        }

        /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> OrAsync([InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<Maybe<T>>> asyncOtherGenerator,
            CancellationToken cancellation = default)
        {
            _ = asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.OrAsync(asyncOtherGenerator, cancellation)
                : AwaitOr(maybeValueTask.AsTask(), asyncOtherGenerator, cancellation);
        }

#endregion

#region Filter

        /// <inheritdoc cref="Maybe.Filter{T}" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> Filter([InstantHandle(RequireAwait = true)] Func<T, bool> filter, CancellationToken cancellation = default)
        {
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return maybeValueTask.IsCompletedSuccessfully
                ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Filter(filter))
                : AwaitFilter(maybeValueTask.AsTask(), filter, cancellation);
        }

        /// <inheritdoc cref="Maybe.Filter{T}" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> FilterAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> asyncFilter,
            CancellationToken cancellation = default)
        {
            _ = asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter));
            return maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result.FilterAsync(asyncFilter, cancellation)
                : AwaitFilter(maybeValueTask.AsTask(), asyncFilter, cancellation);
        }

#endregion

#region Do

        /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
        [PublicAPI]
        public ValueTask Do([InstantHandle(RequireAwait = true)] Action<T> valueAction, [InstantHandle(RequireAwait = true)] Action emptyAction,
            CancellationToken cancellation = default)
        {
            _ = valueAction ?? throw new ArgumentNullException(nameof(valueAction));
            _ = emptyAction ?? throw new ArgumentNullException(nameof(emptyAction));
            if (!maybeValueTask.IsCompletedSuccessfully) return AwaitDo(maybeValueTask.AsTask(), valueAction, emptyAction, cancellation);
            maybeValueTask.Result.Do(valueAction, emptyAction);
            return default;
        }

        /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
        [PublicAPI]
        public ValueTask Do([InstantHandle(RequireAwait = true)] Action<T> valueAction, CancellationToken cancellation = default)
        {
            _ = valueAction ?? throw new ArgumentNullException(nameof(valueAction));
            if (!maybeValueTask.IsCompletedSuccessfully) return AwaitDo(maybeValueTask.AsTask(), valueAction, cancellation);
            maybeValueTask.Result.Do(valueAction);
            return default;
        }

        /// <inheritdoc cref="Maybe.DoIfEmpty{T}" />
        [PublicAPI]
        public ValueTask DoIfEmpty([InstantHandle(RequireAwait = true)] Action emptyAction, CancellationToken cancellation = default)
        {
            _ = emptyAction ?? throw new ArgumentNullException(nameof(emptyAction));
            if (!maybeValueTask.IsCompletedSuccessfully) return AwaitDoIfEmpty(maybeValueTask.AsTask(), emptyAction, cancellation);
            maybeValueTask.Result.DoIfEmpty(emptyAction);
            return default;
        }

#endregion

#region DoWithArgument

        /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},Action{TArgument},TArgument)" />
        [PublicAPI]
        public ValueTask Do<TArgument>([InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction,
            [InstantHandle(RequireAwait = true)] Action<TArgument> emptyAction, TArgument argument, CancellationToken cancellation = default)
        {
            _ = valueAction ?? throw new ArgumentNullException(nameof(valueAction));
            _ = emptyAction ?? throw new ArgumentNullException(nameof(emptyAction));
            if (!maybeValueTask.IsCompletedSuccessfully) return AwaitDo(maybeValueTask.AsTask(), valueAction, emptyAction, argument, cancellation);
            maybeValueTask.Result.Do(valueAction, emptyAction, argument);
            return default;
        }

        /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},TArgument)" />
        [PublicAPI]
        public ValueTask Do<TArgument>([InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            _ = valueAction ?? throw new ArgumentNullException(nameof(valueAction));
            if (!maybeValueTask.IsCompletedSuccessfully) return AwaitDo(maybeValueTask.AsTask(), valueAction, argument, cancellation);
            maybeValueTask.Result.Do(valueAction, argument);
            return default;
        }

        /// <inheritdoc cref="Maybe.DoIfEmpty{T,TArgument}" />
        [PublicAPI]
        public ValueTask DoIfEmpty<TArgument>([InstantHandle(RequireAwait = true)] Action<TArgument> emptyAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            _ = emptyAction ?? throw new ArgumentNullException(nameof(emptyAction));
            if (!maybeValueTask.IsCompletedSuccessfully) return AwaitDoIfEmpty(maybeValueTask.AsTask(), emptyAction, argument, cancellation);
            maybeValueTask.Result.DoIfEmpty(emptyAction, argument);
            return default;
        }

#endregion

#region DoAsync

        /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction, CancellationToken cancellation = default)
        {
            _ = asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction));
            _ = asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction));
            var maybe = maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result
                : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (maybe.HasValue) await asyncValueAction.Invoke(maybe.Value, cancellation).ConfigureAwait(false);
            else await asyncEmptyAction.Invoke(cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
            CancellationToken cancellation = default)
        {
            _ = asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction));
            var maybe = maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result
                : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (maybe.HasValue) await asyncValueAction.Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.DoIfEmpty{T}" />
        [PublicAPI]
        public async Task DoIfEmptyAsync([InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction,
            CancellationToken cancellation = default)
        {
            _ = asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction));
            var maybe = maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result
                : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (!maybe.HasValue) await asyncEmptyAction.Invoke(cancellation).ConfigureAwait(false);
        }

#endregion

#region DoAsyncWithArgument

        /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},Action{TArgument},TArgument)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            _ = asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction));
            _ = asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction));
            var maybe = maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result
                : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (maybe.HasValue) await asyncValueAction.Invoke(maybe.Value, argument, cancellation).ConfigureAwait(false);
            else await asyncEmptyAction.Invoke(argument, cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},TArgument)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            TArgument argument, CancellationToken cancellation = default)
        {
            _ = asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction));
            var maybe = maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result
                : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (maybe.HasValue) await asyncValueAction.Invoke(maybe.Value, argument, cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.DoIfEmpty{T,TArgument}" />
        [PublicAPI]
        public async Task DoIfEmptyAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction,
            TArgument argument, CancellationToken cancellation = default)
        {
            _ = asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction));
            var maybe = maybeValueTask.IsCompletedSuccessfully
                ? maybeValueTask.Result
                : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (!maybe.HasValue) await asyncEmptyAction.Invoke(argument, cancellation).ConfigureAwait(false);
        }

#endregion
    }
}
