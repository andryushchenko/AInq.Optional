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
///     Try utils
/// </summary>
public static class Try
{
    /// <summary>
    ///     Create Try from value
    /// </summary>
    /// <param name="value">Value</param>
    /// <typeparam name="T">Value type</typeparam>
    public static Try<T> Value<T>(T value)
        => new(value);

    /// <summary>
    ///     Create Try from exception
    /// </summary>
    /// <param name="exception">Exception</param>
    /// <typeparam name="T">Value type</typeparam>
    public static Try<T> Error<T>(Exception exception)
        => new(exception is AggregateException aggregate && aggregate.InnerExceptions.Count == 1 ? aggregate.InnerExceptions[0] : exception);

    /// <summary>
    ///     Create Try from value generator
    /// </summary>
    /// <param name="generator">Value generator</param>
    /// <typeparam name="T">Value type</typeparam>
    public static Try<T> Result<T>(Func<T> generator)
    {
        try
        {
            return Value(generator.Invoke());
        }
        catch (Exception ex)
        {
            return Error<T>(ex);
        }
    }

    /// <summary>
    ///     Convert to other value type
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="selector">Converter</param>
    /// <typeparam name="T">Source value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    public static Try<TResult> Select<T, TResult>(this Try<T> item, Func<T, TResult> selector)
        => item.Success ? Result(() => selector.Invoke(item.Value)) : Error<TResult>(item.Error!);

    /// <summary>
    ///     Convert to other value type or default
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="selector">Converter</param>
    /// <param name="defaultValue">Default value</param>
    /// <typeparam name="T">Source value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Try<T> item, Func<T, TResult> selector, TResult defaultValue)
        => item.Select(selector).ValueOrDefault(defaultValue);

    /// <summary>
    ///     Convert to other value type or default from generator
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="selector">Converter</param>
    /// <param name="defaultGenerator">Default value generator</param>
    /// <typeparam name="T">Source value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Try<T> item, Func<T, TResult> selector, Func<TResult> defaultGenerator)
        => item.Select(selector).ValueOrDefault(defaultGenerator);

    /// <summary>
    ///     Convert to other value type
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="selector">Converter</param>
    /// <typeparam name="T">Source value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    public static Try<TResult> Select<T, TResult>(this Try<T> item, Func<T, Try<TResult>> selector)
        => item.Success ? selector.Invoke(item.Value) : Error<TResult>(item.Error!);

    /// <summary>
    ///     Convert to other value type or default
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="selector">Converter</param>
    /// <param name="defaultValue">Default value</param>
    /// <typeparam name="T">Source value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Try<T> item, Func<T, Try<TResult>> selector, TResult defaultValue)
        => item.Select(selector).ValueOrDefault(defaultValue);

    /// <summary>
    ///     Convert to other value type or default from generator
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="selector">Converter</param>
    /// <param name="defaultGenerator">Default value generator</param>
    /// <typeparam name="T">Source value type</typeparam>
    /// <typeparam name="TResult">Result value type</typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Try<T> item, Func<T, Try<TResult>> selector, Func<TResult> defaultGenerator)
        => item.Select(selector).ValueOrDefault(defaultGenerator);

    /// <summary>
    ///     Get value or default
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="defaultValue">Default value</param>
    /// <typeparam name="T">Value type</typeparam>
    public static T ValueOrDefault<T>(this Try<T> item, T defaultValue)
        => item.Success ? item.Value : defaultValue;

    /// <summary>
    ///     Get value or default from generator
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="defaultGenerator">Default value generator</param>
    /// <typeparam name="T">Value type</typeparam>
    public static T ValueOrDefault<T>(this Try<T> item, Func<T> defaultGenerator)
        => item.Success ? item.Value : defaultGenerator.Invoke();

    /// <summary>
    ///     Get value form this item or other
    /// </summary>
    /// <param name="item">Source</param>
    /// <param name="other">Other</param>
    /// <typeparam name="T">Value type</typeparam>
    public static Try<T> Or<T>(this Try<T> item, Try<T> other)
        => item.Success ? item : other;

    /// <summary>
    ///     Unwrap nested Try
    /// </summary>
    /// <param name="item">Source</param>
    /// <typeparam name="T">Value type</typeparam>
    public static Try<T> Unwrap<T>(this Try<Try<T>> item)
        => item.Success ? item.Value : Error<T>(item.Error!);
}

}
