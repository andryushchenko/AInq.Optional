// Copyright 2021-2022 Anton Andryushchenko
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

/// <summary> Maybe utils </summary>
public static class Maybe
{
#region Value

    /// <inheritdoc cref="Maybe{T}.None" />
    [PublicAPI]
    public static Maybe<T> None<T>()
        => Maybe<T>.None;

    /// <inheritdoc cref="Maybe{T}.FromValue(T)" />
    [PublicAPI]
    public static Maybe<T> Value<T>(T value)
        => Maybe<T>.FromValue(value);

    /// <summary> Create Maybe from value if not null </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI]
    public static Maybe<T> ValueIfNotNull<T>(T? value)
        where T : class
        => value is not null ? Maybe<T>.FromValue(value) : Maybe<T>.None;

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    [PublicAPI]
    public static Maybe<T> ValueIfNotNull<T>(T? value)
        where T : struct
        => value.HasValue ? Maybe<T>.FromValue(value.Value) : Maybe<T>.None;

    /// <inheritdoc cref="Maybe{T}.FromValue(T)" />
    [PublicAPI, Pure]
    public static Maybe<T> AsMaybe<T>(this T value)
        => Maybe<T>.FromValue(value);

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static Maybe<T> AsMaybeIfNotNull<T>(this T? value)
        where T : class
        => value is not null ? Maybe<T>.FromValue(value) : Maybe<T>.None;

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static Maybe<T> AsMaybeIfNotNull<T>(this T? value)
        where T : struct
        => value.HasValue ? Maybe<T>.FromValue(value.Value) : Maybe<T>.None;

#endregion

#region Select

    /// <summary> Convert to other value type </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    [PublicAPI, Pure]
    public static Maybe<TResult> Select<T, TResult>(this Maybe<T> maybe, [InstantHandle] Func<T, TResult> selector)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? Value((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
            : None<TResult>();

    /// <inheritdoc cref="Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static Maybe<TResult> Select<T, TResult>(this Maybe<T> maybe, [InstantHandle] Func<T, Maybe<TResult>> selector)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : None<TResult>();

#endregion

#region SelectOrDefault

    /// <summary> Convert to other value type or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    [PublicAPI, Pure]
    public static TResult? SelectOrDefault<T, TResult>(this Maybe<T> maybe, [InstantHandle] Func<T, TResult> selector)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : default;

    /// <inheritdoc cref="SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static TResult? SelectOrDefault<T, TResult>(this Maybe<T> maybe, [InstantHandle] Func<T, Maybe<TResult>> selector)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault()
            : default;

    /// <summary> Convert to other value type or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    [PublicAPI, Pure]
    public static TResult SelectOrDefault<T, TResult>(this Maybe<T> maybe, [InstantHandle] Func<T, TResult> selector, TResult defaultValue)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : defaultValue;

    /// <inheritdoc cref="SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult}, TResult)" />
    [PublicAPI, Pure]
    public static TResult SelectOrDefault<T, TResult>(this Maybe<T> maybe, [InstantHandle] Func<T, Maybe<TResult>> selector, TResult defaultValue)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault(defaultValue)
            : defaultValue;

    /// <summary> Convert to other value type or default from generator </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    [PublicAPI, Pure]
    public static TResult SelectOrDefault<T, TResult>(this Maybe<T> maybe, [InstantHandle] Func<T, TResult> selector,
        [InstantHandle] Func<TResult> defaultGenerator)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <inheritdoc cref="SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static TResult SelectOrDefault<T, TResult>(this Maybe<T> maybe, [InstantHandle] Func<T, Maybe<TResult>> selector,
        [InstantHandle] Func<TResult> defaultGenerator)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value)
              .ValueOrDefault((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke())
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

#endregion

#region ValueOrDefault

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static T? ValueOrDefault<T>(this Maybe<T> maybe)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue ? maybe.Value : default;

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static T ValueOrDefault<T>(this Maybe<T> maybe, T defaultValue)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue ? maybe.Value : defaultValue;

    /// <summary> Get value or default from generator </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static T ValueOrDefault<T>(this Maybe<T> maybe, [InstantHandle] Func<T> defaultGenerator)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? maybe.Value
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

#endregion

#region Utils

    /// <summary> Get value form this item or other </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="other"> Other </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static Maybe<T> Or<T>(this Maybe<T> maybe, Maybe<T> other)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? maybe
            : other ?? throw new ArgumentNullException(nameof(other));

    /// <summary> Get value form this item or other </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="otherGenerator"> Other generator </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static Maybe<T> Or<T>(this Maybe<T> maybe, [InstantHandle] Func<Maybe<T>> otherGenerator)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? maybe
            : (otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))).Invoke();

    /// <summary> Filter value </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="filter"> Filter </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static Maybe<T> Filter<T>(this Maybe<T> maybe, [InstantHandle] Func<T, bool> filter)
        => !(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
           || (maybe.HasValue && (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(maybe.Value))
            ? maybe
            : None<T>();

    /// <summary> Unwrap nested Maybe </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static Maybe<T> Unwrap<T>(this Maybe<Maybe<T>> maybe)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue ? maybe.Value : None<T>();

    /// <summary> Select existing values </summary>
    /// <param name="collection"> Maybe collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <returns> Values collection </returns>
    [PublicAPI, LinqTunnel]
    public static IEnumerable<T> Values<T>(this IEnumerable<Maybe<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item is {HasValue: true})
           .Select(item => item.Value);

#endregion

#region Do

    /// <summary> Do action </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="emptyAction"> Action if empty </param>
    /// <typeparam name="T"> Source value type </typeparam>
    [PublicAPI]
    public static void Do<T>(this Maybe<T> maybe, [InstantHandle] Action<T> valueAction, [InstantHandle] Action emptyAction)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value);
        else (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }

    /// <summary> Do action with additional argument </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="emptyAction"> Action if empty </param>
    /// <param name="argument"> Additional action argument </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TArgument"> Additional action argument type </typeparam>
    [PublicAPI]
    public static void Do<T, TArgument>(this Maybe<T> maybe, [InstantHandle] Action<T, TArgument> valueAction, [InstantHandle] Action emptyAction,
        TArgument argument)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, argument);
        else (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }

    /// <summary> Do action with value (if exists) </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <typeparam name="T"> Source value type </typeparam>
    [PublicAPI]
    public static void Do<T>(this Maybe<T> maybe, [InstantHandle] Action<T> valueAction)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value);
    }

    /// <summary> Do action with value (if exists) with additional argument </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="argument"> Additional action argument </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TArgument"> Additional action argument type </typeparam>
    [PublicAPI]
    public static void Do<T, TArgument>(this Maybe<T> maybe, [InstantHandle] Action<T, TArgument> valueAction, TArgument argument)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, argument);
    }

    /// <summary> Do action if empty </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="emptyAction"> Action if empty </param>
    /// <typeparam name="T"> Source value type </typeparam>
    [PublicAPI]
    public static void DoIfEmpty<T>(this Maybe<T> maybe, [InstantHandle] Action emptyAction)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }

#endregion
}
