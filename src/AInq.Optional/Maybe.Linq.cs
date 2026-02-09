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

public static partial class Maybe
{
    /// <param name="collection"> <see cref="Maybe{T}" /> collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<Maybe<T>> collection)
    {
        /// <summary> Select existing values </summary>
        /// <returns> Values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<T> Values()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(item => item is {HasValue: true}).Select(item => item.Value);
        }

        /// <summary> Select existing matching values </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Values collection </returns>
        [PublicAPI, LinqTunnel]
        public IEnumerable<T> Values([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(item => item is {HasValue: true} && filter.Invoke(item.Value)).Select(item => item.Value);
        }
    }

    /// <param name="collection"> <see cref="Maybe{T}" /> parallel query </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(ParallelQuery<Maybe<T>> collection)
    {
        /// <inheritdoc cref="Values{T}(IEnumerable{Maybe{T}})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<T> Values()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.Where(item => item is {HasValue: true}).Select(item => item.Value);
        }

        /// <inheritdoc cref="Values{T}(IEnumerable{Maybe{T}},Func{T,bool})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<T> Values([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.Where(item => item is {HasValue: true} && filter.Invoke(item.Value)).Select(item => item.Value);
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<T> collection)
    {
        /// <summary> Get first value or none </summary>
        /// <returns> Maybe </returns>
        [PublicAPI]
        public Maybe<T> MaybeFirst()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(Maybe<T>.FromValue).FirstOrDefault() ?? Maybe<T>.None;
#else
            return collection.Select(Maybe<T>.FromValue).FirstOrDefault(Maybe<T>.None);
#endif
        }

        /// <summary> Get first matching value or none </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Maybe </returns>
        [PublicAPI]
        public Maybe<T> MaybeFirst([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
#if NETSTANDARD
            return collection.Where(filter).Select(Maybe<T>.FromValue).FirstOrDefault() ?? Maybe<T>.None;
#else
            return collection.Where(filter).Select(Maybe<T>.FromValue).FirstOrDefault(Maybe<T>.None);
#endif
        }

        /// <summary> Get last value or none </summary>
        /// <returns> Maybe </returns>
        [PublicAPI]
        public Maybe<T> MaybeLast()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(Maybe<T>.FromValue).LastOrDefault() ?? Maybe<T>.None;
#else
            return collection.Select(Maybe<T>.FromValue).LastOrDefault(Maybe<T>.None);
#endif
        }

        /// <summary> Get last matching value or none </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Maybe </returns>
        [PublicAPI]
        public Maybe<T> MaybeLast([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
#if NETSTANDARD
            return collection.Where(filter).Select(Maybe<T>.FromValue).LastOrDefault() ?? Maybe<T>.None;
#else
            return collection.Where(filter).Select(Maybe<T>.FromValue).LastOrDefault(Maybe<T>.None);
#endif
        }

        /// <summary> Get single value or none </summary>
        /// <returns> Maybe </returns>
        /// <exception cref="InvalidOperationException"> Thrown if collection contains more than one element </exception>
        [PublicAPI]
        public Maybe<T> MaybeSingle()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(Maybe<T>.FromValue).SingleOrDefault() ?? Maybe<T>.None;
#else
            return collection.Select(Maybe<T>.FromValue).SingleOrDefault(Maybe<T>.None);
#endif
        }

        /// <summary> Get single matching value or none </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Maybe </returns>
        /// <exception cref="InvalidOperationException"> Thrown if collection contains more than one matching element </exception>
        [PublicAPI]
        public Maybe<T> MaybeSingle([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
#if NETSTANDARD
            return collection.Select(Maybe<T>.FromValue).SingleOrDefault(maybe => maybe.HasValue && filter.Invoke(maybe.Value)) ?? Maybe<T>.None;
#else
            return collection.Select(Maybe<T>.FromValue).SingleOrDefault(maybe => maybe.HasValue && filter.Invoke(maybe.Value), Maybe<T>.None);
#endif
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<T?> collection)
        where T : class
    {
        /// <summary> Get first not null value or none </summary>
        /// <returns> Maybe </returns>
        [PublicAPI]
        public Maybe<T> MaybeFirstNotNull()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(ValueIfNotNull).FirstOrDefault(maybe => maybe.HasValue) ?? Maybe<T>.None;
#else
            return collection.Select(ValueIfNotNull).FirstOrDefault(maybe => maybe.HasValue, Maybe<T>.None);
#endif
        }

        /// <summary> Get last not null value or none </summary>
        /// <returns> Maybe </returns>
        [PublicAPI]
        public Maybe<T> MaybeLastNotNull()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(ValueIfNotNull).LastOrDefault(maybe => maybe.HasValue) ?? Maybe<T>.None;
#else
            return collection.Select(ValueIfNotNull).LastOrDefault(maybe => maybe.HasValue, Maybe<T>.None);
#endif
        }

        /// <summary> Get single not null value or none </summary>
        /// <returns> Maybe </returns>
        /// <exception cref="InvalidOperationException"> Thrown if collection contains more than one not null element </exception>
        [PublicAPI]
        public Maybe<T> MaybeSingleNotNull()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(ValueIfNotNull).SingleOrDefault(maybe => maybe.HasValue) ?? Maybe<T>.None;
#else
            return collection.Select(ValueIfNotNull).SingleOrDefault(maybe => maybe.HasValue, Maybe<T>.None);
#endif
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<T?> collection)
        where T : struct
    {
        /// <inheritdoc cref="MaybeFirstNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public Maybe<T> MaybeFirstNotNull()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(ValueIfNotNull).FirstOrDefault(maybe => maybe.HasValue) ?? Maybe<T>.None;
#else
            return collection.Select(ValueIfNotNull).FirstOrDefault(maybe => maybe.HasValue, Maybe<T>.None);
#endif
        }

        /// <inheritdoc cref="MaybeLastNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public Maybe<T> MaybeLastNotNull()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(ValueIfNotNull).LastOrDefault(maybe => maybe.HasValue) ?? Maybe<T>.None;
#else
            return collection.Select(ValueIfNotNull).LastOrDefault(maybe => maybe.HasValue, Maybe<T>.None);
#endif
        }

        /// <inheritdoc cref="MaybeSingleNotNull{T}(IEnumerable{T?})" />
        [PublicAPI]
        public Maybe<T> MaybeSingleNotNull()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if NETSTANDARD
            return collection.Select(ValueIfNotNull).SingleOrDefault(maybe => maybe.HasValue) ?? Maybe<T>.None;
#else
            return collection.Select(ValueIfNotNull).SingleOrDefault(maybe => maybe.HasValue, Maybe<T>.None);
#endif
        }
    }
}
