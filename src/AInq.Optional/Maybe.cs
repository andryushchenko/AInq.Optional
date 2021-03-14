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

using System;

namespace AInq.Optional
{

/// <summary>
///     Maybe utils
/// </summary>
public static class Maybe
{
    /// <summary>
    ///     Create empty Maybe
    /// </summary>
    /// <typeparam name="T">Value type</typeparam>
    public static Maybe<T> None<T>()
        => new(default!, false);

    /// <summary>
    ///     Create Maybe from value
    /// </summary>
    /// <param name="value">Value</param>
    /// <typeparam name="T">Value type</typeparam>
    public static Maybe<T> Value<T>(T value)
        => new(value, true);

    /// <summary>
    ///     Convert to other value type
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="selector">Converter</param>
    /// <typeparam name="T">Source value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    public static Maybe<TResult> Select<T, TResult>(this Maybe<T> item, Func<T, TResult> selector)
        => item.HasValue ? Value(selector.Invoke(item.Value)) : None<TResult>();

    /// <summary>
    ///     Convert to other value type
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="selector">Converter</param>
    /// <typeparam name="T">Source value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    public static Maybe<TResult> Select<T, TResult>(this Maybe<T> item, Func<T, Maybe<TResult>> selector)
        => item.HasValue ? selector.Invoke(item.Value) : None<TResult>();

    /// <summary>
    ///     Get value or default
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="defaultValue">Default value</param>
    /// <typeparam name="T">Value type</typeparam>
    public static T ValueOrDefault<T>(this Maybe<T> item, T defaultValue)
        => item.HasValue ? item.Value : defaultValue;

    /// <summary>
    ///     Get value or default from generator
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="defaultGenerator">Default value generator</param>
    /// <typeparam name="T">Value type</typeparam>
    public static T ValueOrDefault<T>(this Maybe<T> item, Func<T> defaultGenerator)
        => item.HasValue ? item.Value : defaultGenerator.Invoke();

    /// <summary>
    ///     Get value form this item or other
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="other">Other</param>
    /// <typeparam name="T">Value type</typeparam>
    public static Maybe<T> Or<T>(this Maybe<T> item, Maybe<T> other)
        => item.HasValue ? item : other;

    /// <summary>
    ///     Unwrap nested Maybe
    /// </summary>
    /// <param name="item">Source</param>
    /// <typeparam name="T">Value type</typeparam>
    public static Maybe<T> Unwrap<T>(this Maybe<Maybe<T>> item)
        => item.HasValue ? item.Value : None<T>();
}

}
