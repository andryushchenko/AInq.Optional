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

#if !NET10_0_OR_GREATER
using System.Runtime.CompilerServices;

namespace AInq.Optional;

public static partial class TryAsync
{
    extension<T>(IAsyncEnumerable<Try<T>> collection)
    {
        /// <inheritdoc cref="Try.Values{T}(IEnumerable{AInq.Optional.Try{T}})" />
        [PublicAPI]
        public async IAsyncEnumerable<T> Values([EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            await foreach (var @try in collection.WithCancellation(cancellation).ConfigureAwait(false))
                if (@try is {Success: true})
                    yield return @try.Value;
        }

        /// <inheritdoc cref="Try.Values{T}(IEnumerable{AInq.Optional.Try{T}},Func{T,bool})" />
        [PublicAPI]
        public async IAsyncEnumerable<T> Values(Func<T, bool> filter, [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            await foreach (var @try in collection.WithCancellation(cancellation).ConfigureAwait(false))
                if (@try is {Success: true} && filter.Invoke(@try.Value))
                    yield return @try.Value;
        }

        /// <inheritdoc cref="Try.Values{T}(IEnumerable{AInq.Optional.Try{T}},Func{T,bool})" />
        [PublicAPI]
        public async IAsyncEnumerable<T> Values(Func<T, CancellationToken, ValueTask<bool>> filter,
            [EnumeratorCancellation] CancellationToken cancellation = default)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            await foreach (var @try in collection.WithCancellation(cancellation).ConfigureAwait(false))
                if (@try is {Success: true} && await filter.Invoke(@try.Value, cancellation).ConfigureAwait(false))
                    yield return @try.Value;
        }
    }
}

#endif
