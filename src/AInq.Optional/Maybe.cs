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

/// <summary> Maybe monad utils </summary>
public static class Maybe
{
    /// <summary> Create empty Maybe </summary>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> None<T>()
        => new();

    /// <summary> Create Maybe from value </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> Value<T>(T value)
        => new(value);

    /// <inheritdoc cref="Value{T}"/>
    public static Maybe<T> AsMaybe<T>(this T value)
        => new(value);

    /// <summary> Create Maybe from value if not null </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> ValueIfNotNull<T>(T? value)
        where T : class
        => value == null ? new Maybe<T>() : new Maybe<T>(value);

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    public static Maybe<T> AsMaybeIfNotNull<T>(this T? value)
        where T : class
        => ValueIfNotNull(value);

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    public static Maybe<T> ValueIfNotNull<T>(T? value)
        where T : struct
        => value == null ? new Maybe<T>() : new Maybe<T>(value.Value);

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    public static Maybe<T> AsMaybeIfNotNull<T>(this T? value)
        where T : struct
        => ValueIfNotNull(value);

    /// <summary> Convert to other value type </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static Maybe<TResult> Select<T, TResult>(this Maybe<T> maybe, Func<T, TResult> selector)
        => maybe.HasValue ? Value((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)) : None<TResult>();

    /// <summary> Convert to other value type or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult? SelectOrDefault<T, TResult>(this Maybe<T> maybe, Func<T, TResult> selector)
        => maybe.HasValue ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value) : default;

    /// <summary> Convert to other value type or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Maybe<T> maybe, Func<T, TResult> selector, TResult defaultValue)
        => maybe.HasValue ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value) : defaultValue;

    /// <summary> Convert to other value type or default from generator </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Maybe<T> maybe, Func<T, TResult> selector, Func<TResult> defaultGenerator)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Convert to other value type </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static Maybe<TResult> Select<T, TResult>(this Maybe<T> maybe, Func<T, Maybe<TResult>> selector)
        => maybe.HasValue ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value) : None<TResult>();

    /// <summary> Convert to other value type or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult? SelectOrDefault<T, TResult>(this Maybe<T> maybe, Func<T, Maybe<TResult>> selector)
        => maybe.HasValue ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault() : default;

    /// <summary> Convert to other value type or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Maybe<T> maybe, Func<T, Maybe<TResult>> selector, TResult defaultValue)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault(defaultValue)
            : defaultValue;

    /// <summary> Convert to other value type or default from generator </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Maybe<T> maybe, Func<T, Maybe<TResult>> selector, Func<TResult> defaultGenerator)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value)
              .ValueOrDefault((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke())
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static T? ValueOrDefault<T>(this Maybe<T> maybe)
        => maybe.HasValue ? maybe.Value : default;

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static T ValueOrDefault<T>(this Maybe<T> maybe, T defaultValue)
        => maybe.HasValue ? maybe.Value : defaultValue;

    /// <summary> Get value or default from generator </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static T ValueOrDefault<T>(this Maybe<T> maybe, Func<T> defaultGenerator)
        => maybe.HasValue ? maybe.Value : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Get value form this item or other </summary>
    /// <param name="item"> Maybe item </param>
    /// <param name="other"> Other </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> Or<T>(this Maybe<T> item, Maybe<T> other)
        => item.HasValue ? item : other;

    /// <summary> Filter value </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="filter"> Filter </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> Filter<T>(this Maybe<T> maybe, Func<T, bool> filter)
        => !maybe.HasValue || maybe.HasValue && (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(maybe.Value) ? maybe : None<T>();

    /// <summary> Unwrap nested Maybe </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Maybe<T> Unwrap<T>(this Maybe<Maybe<T>> maybe)
        => maybe.HasValue ? maybe.Value : None<T>();

    /// <summary> Select existing values </summary>
    /// <param name="collection"> Maybe collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <returns> Values collection </returns>
    public static IEnumerable<T> Values<T>(this IEnumerable<Maybe<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item.HasValue)
           .Select(item => item.Value);

    /// <summary> Do action with value (if exists) </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static void Do<T>(this Maybe<T> maybe, Action<T> valueAction)
    {
        if (maybe.HasValue) (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value);
    }

    /// <summary> Do action </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="emptyAction"> Action if empty </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static void Do<T>(this Maybe<T> maybe, Action<T> valueAction, Action emptyAction)
    {
        if (maybe.HasValue) (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value);
        else (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }

    /// <summary> Do action if empty </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="emptyAction"> Action if empty </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static void DoIfEmpty<T>(this Maybe<T> maybe, Action emptyAction)
    {
        if (!maybe.HasValue) (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }
}
