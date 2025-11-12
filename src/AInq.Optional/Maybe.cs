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

/// <summary> <see cref="Maybe{T}" /> utils and extensions </summary>
public static partial class Maybe
{
    /// <inheritdoc cref="Maybe{T}.None" />
    [PublicAPI]
    public static Maybe<T> None<T>()
        => Maybe<T>.None;

    /// <inheritdoc cref="Maybe{T}.FromValue(T)" />
    [PublicAPI]
    public static Maybe<T> Value<T>([NoEnumeration] T value)
        => Maybe<T>.FromValue(value);

    /// <summary> Create Maybe from value if not null </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI]
    public static Maybe<T> ValueIfNotNull<T>([NoEnumeration] T? value)
        where T : class
        => value is not null ? Maybe<T>.FromValue(value) : Maybe<T>.None;

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    [PublicAPI]
    public static Maybe<T> ValueIfNotNull<T>([NoEnumeration] T? value)
        where T : struct
        => value.HasValue ? Maybe<T>.FromValue(value.Value) : Maybe<T>.None;

    /// <inheritdoc cref="Maybe{T}.FromValue(T)" />
    [PublicAPI, Pure]
    public static Maybe<T> AsMaybe<T>([NoEnumeration] this T value)
        => Maybe<T>.FromValue(value);

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static Maybe<T> AsMaybeIfNotNull<T>([NoEnumeration] this T? value)
        where T : class
        => value is not null ? Maybe<T>.FromValue(value) : Maybe<T>.None;

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static Maybe<T> AsMaybeIfNotNull<T>([NoEnumeration] this T? value)
        where T : struct
        => value.HasValue ? Maybe<T>.FromValue(value.Value) : Maybe<T>.None;

    /// <summary> Unwrap nested Maybe </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static Maybe<T> Unwrap<T>(this Maybe<Maybe<T>> maybe)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue ? maybe.Value : None<T>();
    
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(Maybe<T>)
    {
        /// <see cref="ValueOrDefault{T}(Maybe{T},T)" />
        public static T operator |(Maybe<T> maybe, T value)
            => maybe.ValueOrDefault(value);

        /// <see cref="Or{T}(Maybe{T},Maybe{T})" />
        public static Maybe<T> operator |(Maybe<T> maybe, Maybe<T> other)
            => maybe.Or(other);
    }
}
