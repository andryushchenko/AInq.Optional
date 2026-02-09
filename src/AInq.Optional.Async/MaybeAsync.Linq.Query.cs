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

#if NET10_0_OR_GREATER
namespace AInq.Optional;

public static partial class MaybeAsync
{
    /// <param name="collection"> <see cref="Maybe{T}" /> collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<Maybe<T>> collection)
    {
        /// <inheritdoc cref="Maybe.Values{T}(IEnumerable{Maybe{T}})" />
        [PublicAPI, LinqTunnel]
        public IAsyncEnumerable<T> Values()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(maybe => maybe is {HasValue: true}).Select(maybe => maybe.Value);
        }

        /// <inheritdoc cref="Maybe.Values{T}(IEnumerable{Maybe{T}},Func{T,bool})" />
        [PublicAPI, LinqTunnel]
        public IAsyncEnumerable<T> Values([InstantHandle(RequireAwait = true)] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(maybe => maybe is {HasValue: true} && filter.Invoke(maybe.Value)).Select(maybe => maybe.Value);
        }

        /// <inheritdoc cref="Maybe.Values{T}(IEnumerable{Maybe{T}},Func{T,bool})" />
        [PublicAPI, LinqTunnel]
        public IAsyncEnumerable<T> Values([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(async (maybe, ctx) => maybe is {HasValue: true} && await filter.Invoke(maybe.Value, ctx).ConfigureAwait(false))
                             .Select(maybe => maybe.Value);
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T> collection)
    {
        /// <inheritdoc cref="Maybe.MaybeFirst{T}(IEnumerable{T})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeFirstAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe<T>.FromValue).FirstOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeFirst{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeFirstAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter).Select(Maybe<T>.FromValue).FirstOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeFirst{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeFirstAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter).Select(Maybe<T>.FromValue).FirstOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeLast{T}(IEnumerable{T})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeLastAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe<T>.FromValue).LastOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeLast{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeLastAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter).Select(Maybe<T>.FromValue).LastOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeLast{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeLastAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter).Select(Maybe<T>.FromValue).LastOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeSingle{T}(IEnumerable{T})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeSingleAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe<T>.FromValue).SingleOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeSingle{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeSingleAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Select(Maybe<T>.FromValue)
                                   .SingleOrDefaultAsync(maybe => maybe.HasValue && filter.Invoke(maybe.Value), Maybe.None<T>(), cancellation)
                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeSingle{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeSingleAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Select(Maybe<T>.FromValue)
                                   .SingleOrDefaultAsync(async (maybe, ctx)
                                           => maybe.HasValue && await filter.Invoke(maybe.Value, ctx).ConfigureAwait(false),
                                       Maybe.None<T>(),
                                       cancellation)
                                   .ConfigureAwait(false);
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T?> collection)
        where T : class
    {
        /// <inheritdoc cref="Maybe.MaybeFirstNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeFirstNotNullAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .FirstOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeLastNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeLastNotNullAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .LastOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeSingleNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeSingleNotNullAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .SingleOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T?> collection)
        where T : struct
    {
        /// <inheritdoc cref="Maybe.MaybeFirstNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeFirstNotNullAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .FirstOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeLastNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeLastNotNullAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .LastOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.MaybeSingleNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> MaybeSingleNotNullAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .SingleOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }
    }
}

#endif
