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
    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T> collection)
    {
        /// <inheritdoc cref="MaybeFirstAsync{T}(IAsyncEnumerable{T},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeFirstAsync instead")]
        public ValueTask<Maybe<T>> FirstOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeFirstAsync(cancellation);
        }

        /// <inheritdoc cref="MaybeFirstAsync{T}(IAsyncEnumerable{T},Func{T,bool},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeFirstAsync instead")]
        public ValueTask<Maybe<T>> FirstOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeFirstAsync(filter, cancellation);
        }

        /// <inheritdoc cref="MaybeFirstAsync{T}(IAsyncEnumerable{T},Func{T,CancellationToken,ValueTask{bool}},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeFirstAsync instead")]
        public ValueTask<Maybe<T>> FirstOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeFirstAsync(filter, cancellation);
        }

        /// <inheritdoc cref="MaybeLastAsync{T}(IAsyncEnumerable{T},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeLastAsync instead")]
        public ValueTask<Maybe<T>> LastOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeLastAsync(cancellation);
        }

        /// <inheritdoc cref="MaybeLastAsync{T}(IAsyncEnumerable{T},Func{T,bool},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeLastAsync instead")]
        public ValueTask<Maybe<T>> LastOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeLastAsync(filter, cancellation);
        }

        /// <inheritdoc cref="MaybeLastAsync{T}(IAsyncEnumerable{T},Func{T,CancellationToken,ValueTask{bool}},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeLastAsync instead")]
        public ValueTask<Maybe<T>> LastOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeLastAsync(filter, cancellation);
        }

        /// <inheritdoc cref="MaybeSingleAsync{T}(IAsyncEnumerable{T},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeSingleAsync instead")]
        public ValueTask<Maybe<T>> SingleOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeSingleAsync(cancellation);
        }

        /// <inheritdoc cref="MaybeSingleAsync{T}(IAsyncEnumerable{T},Func{T,bool},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeSingleAsync instead")]
        public ValueTask<Maybe<T>> SingleOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, bool> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeSingleAsync(filter, cancellation);
        }

        /// <inheritdoc cref="MaybeSingleAsync{T}(IAsyncEnumerable{T},Func{T,CancellationToken,ValueTask{bool}},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeSingleAsync instead")]
        public ValueTask<Maybe<T>> SingleOrNoneAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter,
            CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeSingleAsync(filter, cancellation);
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T?> collection)
        where T : class
    {
        /// <inheritdoc cref="MaybeFirstNotNullAsync{T}(IAsyncEnumerable{T?},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeFirstNotNullAsync instead")]
        public ValueTask<Maybe<T>> FirstNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeFirstNotNullAsync(cancellation);
        }

        /// <inheritdoc cref="MaybeLastNotNullAsync{T}(IAsyncEnumerable{T?},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeLastNotNullAsync instead")]
        public ValueTask<Maybe<T>> LastNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeLastNotNullAsync(cancellation);
        }

        /// <inheritdoc cref="MaybeSingleNotNullAsync{T}(IAsyncEnumerable{T?},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeSingleNotNullAsync instead")]
        public ValueTask<Maybe<T>> SingleNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeSingleNotNullAsync(cancellation);
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IAsyncEnumerable<T?> collection)
        where T : struct
    {
        /// <inheritdoc cref="MaybeFirstNotNullAsync{T}(IAsyncEnumerable{T?},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeFirstNotNullAsync instead")]
        public ValueTask<Maybe<T>> FirstNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeFirstNotNullAsync(cancellation);
        }

        /// <inheritdoc cref="MaybeLastNotNullAsync{T}(IAsyncEnumerable{T?},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeLastNotNullAsync instead")]
        public ValueTask<Maybe<T>> LastNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeLastNotNullAsync(cancellation);
        }

        /// <inheritdoc cref="MaybeSingleNotNullAsync{T}(IAsyncEnumerable{T?},CancellationToken)" />
        [PublicAPI, Obsolete("Use MaybeSingleNotNullAsync instead")]
        public ValueTask<Maybe<T>> SingleNotNullOrNoneAsync(CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeSingleNotNullAsync(cancellation);
        }
    }
}
