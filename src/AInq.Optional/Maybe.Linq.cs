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
            => (collection ?? throw new ArgumentNullException(nameof(collection))).Where(item => item is {HasValue: true}).Select(item => item.Value);

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
        /// <inheritdoc cref="Values{T}(System.Collections.Generic.IEnumerable{AInq.Optional.Maybe{T}})" />
        [PublicAPI, LinqTunnel]
        public ParallelQuery<T> Values()
            => (collection ?? throw new ArgumentNullException(nameof(collection))).Where(item => item is {HasValue: true}).Select(item => item.Value);

        /// <inheritdoc cref="Values{T}(System.Collections.Generic.IEnumerable{AInq.Optional.Maybe{T}},System.Func{T,bool})" />
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
        public Maybe<T> FirstOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            switch (collection)
            {
                case IList<T> list:
                    return list.Count == 0 ? Maybe<T>.None : list[0];
                case IReadOnlyList<T> readOnlyList:
                    return readOnlyList.Count == 0 ? Maybe<T>.None : readOnlyList[0];
            }
#if !NETSTANDARD
            if (collection.TryGetNonEnumeratedCount(out var count))
                return count == 0 ? Maybe<T>.None : collection.ElementAt(0);
#endif
            using var enumerator = collection.GetEnumerator();
            return enumerator.MoveNext() ? enumerator.Current : Maybe<T>.None;
        }

        /// <summary> Get first matching value or none </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Maybe </returns>
        [PublicAPI]
        public Maybe<T> FirstOrNone([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
#if !NETSTANDARD
            if (collection.TryGetNonEnumeratedCount(out var count) && count == 0)
                return Maybe<T>.None;
#endif
            using var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
                if (filter.Invoke(enumerator.Current))
                    return enumerator.Current;
            return Maybe<T>.None;
        }

        /// <summary> Get single value or none </summary>
        /// <returns> Maybe </returns>
        /// <exception cref="InvalidOperationException"> Thrown if collection contains more than one element </exception>
        [PublicAPI]
        public Maybe<T> SingleOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            switch (collection)
            {
                case IList<T> list:
                    return list.Count switch
                    {
                        0 => Maybe<T>.None,
                        1 => list[0],
                        _ => throw new InvalidOperationException("Collection contains more than one element")
                    };
                case IReadOnlyList<T> readOnlyList:
                    return readOnlyList.Count switch
                    {
                        0 => Maybe<T>.None,
                        1 => readOnlyList[0],
                        _ => throw new InvalidOperationException("Collection contains more than one element")
                    };
            }
#if !NETSTANDARD
            if (collection.TryGetNonEnumeratedCount(out var count))
                return count switch
                {
                    0 => Maybe<T>.None,
                    1 => collection.ElementAt(0),
                    _ => throw new InvalidOperationException("Collection contains more than one element")
                };
#endif
            using var enumerator = collection.GetEnumerator();
            if (!enumerator.MoveNext()) return Maybe<T>.None;
            var result = enumerator.Current;
            return enumerator.MoveNext() ? throw new InvalidOperationException("Collection contains more than one element") : result;
        }

        /// <summary> Get single matching value or none </summary>
        /// <param name="filter"> Filter </param>
        /// <returns> Maybe </returns>
        /// <exception cref="InvalidOperationException"> Thrown if collection contains more than one matching element </exception>
        [PublicAPI]
        public Maybe<T> SingleOrNone([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
#if !NETSTANDARD
            if (collection.TryGetNonEnumeratedCount(out var count) && count == 0)
                return Maybe<T>.None;
#endif
            using var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (!filter.Invoke(result)) continue;
                while (enumerator.MoveNext())
                    if (filter.Invoke(enumerator.Current))
                        throw new InvalidOperationException("Collection contains more than one matching element");
                return result;
            }
            return Maybe<T>.None;
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
        public Maybe<T> FirstNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if !NETSTANDARD
            if (collection.TryGetNonEnumeratedCount(out var count) && count == 0)
                return Maybe<T>.None;
#endif
            using var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current is not null)
                    return enumerator.Current;
            return Maybe<T>.None;
        }

        /// <summary> Get single not null value or none </summary>
        /// <returns> Maybe </returns>
        /// <exception cref="InvalidOperationException"> Thrown if collection contains more than one not null element </exception>
        [PublicAPI]
        public Maybe<T> SingleNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if !NETSTANDARD
            if (collection.TryGetNonEnumeratedCount(out var count) && count == 0)
                return Maybe<T>.None;
#endif
            using var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (result is null) continue;
                while (enumerator.MoveNext())
                    if (enumerator.Current is not null)
                        throw new InvalidOperationException("Collection contains more than one not null element");
                return result;
            }
            return Maybe<T>.None;
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<T?> collection)
        where T : struct
    {
        /// <inheritdoc cref="FirstNotNullOrNone{T}(IEnumerable{T})" />
        [PublicAPI]
        public Maybe<T> FirstNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if !NETSTANDARD
            if (collection.TryGetNonEnumeratedCount(out var count) && count == 0)
                return Maybe<T>.None;
#endif
            using var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
                if (enumerator.Current.HasValue)
                    return enumerator.Current.Value;
            return Maybe<T>.None;
        }

        /// <inheritdoc cref="SingleNotNullOrNone{T}(IEnumerable{T})" />
        [PublicAPI]
        public Maybe<T> SingleNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
#if !NETSTANDARD
            if (collection.TryGetNonEnumeratedCount(out var count) && count == 0)
                return Maybe<T>.None;
#endif
            using var enumerator = collection.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var result = enumerator.Current;
                if (!result.HasValue) continue;
                while (enumerator.MoveNext())
                    if (enumerator.Current.HasValue)
                        throw new InvalidOperationException("Collection contains more than one not null element");
                return result.Value;
            }
            return Maybe<T>.None;
        }
    }
}
