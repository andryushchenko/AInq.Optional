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
        /// <inheritdoc cref="Maybe.FirstOrNone{T}(IEnumerable{T})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe<T>.FromValue).FirstOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.FirstOrNone{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter).Select(Maybe<T>.FromValue).FirstOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.FirstOrNone{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter).Select(Maybe<T>.FromValue).FirstOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }
        
        /// <inheritdoc cref="Maybe.LastOrNone{T}(IEnumerable{T})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> LastOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe<T>.FromValue).LastOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.LastOrNone{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> LastOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter).Select(Maybe<T>.FromValue).LastOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.LastOrNone{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> LastOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter).Select(Maybe<T>.FromValue).LastOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.SingleOrNone{T}(IEnumerable{T})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe<T>.FromValue).SingleOrDefaultAsync(Maybe.None<T>(), cancellation).ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.SingleOrNone{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter)
                                   .Select(Maybe<T>.FromValue)
                                   .SingleOrDefaultAsync(Maybe.None<T>(), cancellation)
                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.SingleOrNone{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return await collection.Where(filter)
                                   .Select(Maybe<T>.FromValue)
                                   .SingleOrDefaultAsync(Maybe.None<T>(), cancellation)
                                   .ConfigureAwait(false);
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T?> collection)
        where T : class
    {
        /// <inheritdoc cref="Maybe.FirstNotNullOrNone{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .FirstOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }
        
        /// <inheritdoc cref="Maybe.LastNotNullOrNone{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> LastNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .LastOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.SingleNotNullOrNone{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleNotNullOrNoneAsync(CancellationToken cancellation = default)
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
        /// <inheritdoc cref="Maybe.FirstNotNullOrNone{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> FirstNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .FirstOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }
        
        /// <inheritdoc cref="Maybe.LastNotNullOrNone{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> LastNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .LastOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }

        /// <inheritdoc cref="Maybe.SingleNotNullOrNone{T}(IEnumerable{T?})" />
        [PublicAPI]
        public async ValueTask<Maybe<T>> SingleNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return await collection.Select(Maybe.ValueIfNotNull)
                                   .SingleOrDefaultAsync(maybe => maybe.HasValue, Maybe<T>.None, cancellation)
                                   .ConfigureAwait(false);
        }
    }
}

#endif
