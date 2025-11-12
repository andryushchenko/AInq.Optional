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

using System.Runtime.CompilerServices;

namespace AInq.Optional;

public static partial class MaybeAsync
{
    /// <param name="collection"> <see cref="Maybe{T}" /> collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<Maybe<T>> collection)
    {
        /// <inheritdoc cref="Maybe.Values{T}(System.Collections.Generic.IEnumerable{AInq.Optional.Maybe{T}})" />
        [PublicAPI]
        public async IAsyncEnumerable<T> Values([EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await foreach (var maybe in collection.WithCancellation(cancellation).ConfigureAwait(false))
                if (maybe is {HasValue: true})
                    yield return maybe.Value;
        }

        /// <inheritdoc cref="Maybe.Values{T}(System.Collections.Generic.IEnumerable{AInq.Optional.Maybe{T}},System.Func{T,bool})" />
        [PublicAPI]
        public async IAsyncEnumerable<T> Values([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            await foreach (var maybe in collection.WithCancellation(cancellation).ConfigureAwait(false))
                if (maybe is {HasValue: true} && filter.Invoke(maybe.Value))
                    yield return maybe.Value;
        }

        /// <inheritdoc cref="Maybe.Values{T}(System.Collections.Generic.IEnumerable{AInq.Optional.Maybe{T}},System.Func{T,bool})" />
        [PublicAPI]
        public async IAsyncEnumerable<T> Values([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            await foreach (var maybe in collection.WithCancellation(cancellation).ConfigureAwait(false))
                if (maybe is {HasValue: true} && await filter.Invoke(maybe.Value, cancellation).ConfigureAwait(false))
                    yield return maybe.Value;
        }

        /// <inheritdoc cref="Maybe.FirstOrNone{T}(System.Collections.Generic.IEnumerable{T})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            return await enumerator.MoveNextAsync().ConfigureAwait(false) ? enumerator.Current : Maybe.None<T>();
        }

        /// <inheritdoc cref="Maybe.SingleOrNone{T}(System.Collections.Generic.IEnumerable{T})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            if (!await enumerator.MoveNextAsync().ConfigureAwait(false)) return Maybe.None<T>();
            var result = enumerator.Current;
            return await enumerator.MoveNextAsync().ConfigureAwait(false)
                ? throw new InvalidOperationException("Collection contains more than one element")
                : result;
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T> collection)
    {
        /// <inheritdoc cref="Maybe.FirstOrNone{T}(System.Collections.Generic.IEnumerable{T},System.Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                if (filter.Invoke(enumerator.Current))
                    return enumerator.Current;
            return Maybe.None<T>();
        }

        /// <inheritdoc cref="Maybe.FirstOrNone{T}(System.Collections.Generic.IEnumerable{T},System.Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                if (await filter.Invoke(enumerator.Current, cancellation).ConfigureAwait(false))
                    return enumerator.Current;
            return Maybe.None<T>();
        }

        /// <inheritdoc cref="Maybe.SingleOrNone{T}(System.Collections.Generic.IEnumerable{T},System.Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                var result = enumerator.Current;
                if (!filter.Invoke(result)) continue;
                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                    if (filter.Invoke(enumerator.Current))
                        throw new InvalidOperationException("Collection contains more than one matching element");
                return result;
            }
            return Maybe.None<T>();
        }

        /// <inheritdoc cref="Maybe.SingleOrNone{T}(System.Collections.Generic.IEnumerable{T},System.Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                var result = enumerator.Current;
                if (!await filter.Invoke(result, cancellation).ConfigureAwait(false)) continue;
                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                    if (await filter.Invoke(enumerator.Current, cancellation).ConfigureAwait(false))
                        throw new InvalidOperationException("Collection contains more than one matching element");
                return result;
            }
            return Maybe.None<T>();
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T?> collection)
        where T : class
    {
        /// <inheritdoc cref="Maybe.FirstNotNullOrNone{T}(System.Collections.Generic.IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                if (enumerator.Current is not null)
                    return enumerator.Current;
            return Maybe.None<T>();
        }

        /// <inheritdoc cref="Maybe.SingleNotNullOrNone{T}(System.Collections.Generic.IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                var result = enumerator.Current;
                if (result is null) continue;
                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                    if (enumerator.Current is not null)
                        throw new InvalidOperationException("Collection contains more than one not null element");
                return result;
            }
            return Maybe.None<T>();
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T?> collection)
        where T : struct
    {
        /// <inheritdoc cref="Maybe.FirstNotNullOrNone{T}(System.Collections.Generic.IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                if (enumerator.Current.HasValue)
                    return enumerator.Current.Value;
            return Maybe.None<T>();
        }

        /// <inheritdoc cref="Maybe.SingleNotNullOrNone{T}(System.Collections.Generic.IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await using var enumerator = collection.GetAsyncEnumerator(cancellation);
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            {
                var result = enumerator.Current;
                if (!result.HasValue) continue;
                while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                    if (enumerator.Current.HasValue)
                        throw new InvalidOperationException("Collection contains more than one not null element");
                return result.Value;
            }
            return Maybe.None<T>();
        }
    }
}
