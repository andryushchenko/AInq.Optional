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

/// <summary> Try utils </summary>
public static class Try
{
#region Value

    /// <inheritdoc cref="Try{T}.FromValue(T)" />
    [PublicAPI]
    public static Try<T> Value<T>(T value)
        => Try<T>.FromValue(value);

    /// <inheritdoc cref="Try{T}.FromError(Exception)" />
    [PublicAPI]
    public static Try<T> Error<T>(Exception exception)
        => Try<T>.FromError(exception);

    /// <inheritdoc cref="Try{T}.FromValue(T)" />
    [PublicAPI]
    public static Try<T> AsTry<T>(this T value)
        => Try<T>.FromValue(value);

    /// <inheritdoc cref="Try{T}.FromError(Exception)" />
    [PublicAPI]
    public static Try<T> AsTry<T>(this Exception exception)
        => Try<T>.FromError(exception);

    /// <summary> Create Try from value generator </summary>
    /// <param name="generator"> Value generator </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI]
    public static Try<T> Result<T>([InstantHandle] Func<T> generator)
    {
        _ = generator ?? throw new ArgumentNullException(nameof(generator));
        try
        {
            return Value(generator.Invoke());
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Error<T>(ex);
        }
    }

#endregion

#region Select

    /// <summary> Convert to other value type </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    [PublicAPI]
    public static Try<TResult> Select<T, TResult>(this Try<T> @try, [InstantHandle] Func<T, TResult> selector)
    {
        if (!(@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            return Error<TResult>(@try.Error!);
        _ = selector ?? throw new ArgumentNullException(nameof(selector));
        try
        {
            return Value(selector.Invoke(@try.Value));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Error<TResult>(ex);
        }
    }

    /// <inheritdoc cref="Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI]
    public static Try<TResult> Select<T, TResult>(this Try<T> @try, [InstantHandle] Func<T, Try<TResult>> selector)
    {
        if (!(@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            return Error<TResult>(@try.Error!);
        _ = selector ?? throw new ArgumentNullException(nameof(selector));
        try
        {
            return selector.Invoke(@try.Value);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Error<TResult>(ex);
        }
    }

#endregion

#region Utils

    /// <summary> Get value form this item or other </summary>
    /// <param name="try"> Try item </param>
    /// <param name="other"> Other </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI]
    public static Try<T> Or<T>(this Try<T> @try, Try<T> other)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? @try
            : other ?? throw new ArgumentNullException(nameof(other));

    /// <summary> Unwrap nested Try </summary>
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI]
    public static Try<T> Unwrap<T>(this Try<Try<T>> @try)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? @try.Value
            : Error<T>(@try.Error!);

    /// <summary> Select existing values </summary>
    /// <param name="collection"> Try collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <returns> Values collection </returns>
    [PublicAPI]
    [LinqTunnel]
    public static IEnumerable<T> Values<T>(this IEnumerable<Try<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item is {Success: true})
           .Select(item => item.Value);

    /// <summary> Select exceptions </summary>
    /// <param name="collection"> Try collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <returns> Exceptions collection </returns>
    [PublicAPI]
    [LinqTunnel]
    public static IEnumerable<Exception> Errors<T>(this IEnumerable<Try<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item is {Success: false})
           .Select(item => item.Error!);

#endregion

#region Do

    /// <summary> Try do action </summary>
    /// <param name="try"> Try item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="errorAction"> Action if error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    [PublicAPI]
    public static void Do<T>(this Try<T> @try, [InstantHandle] Action<T> valueAction, [InstantHandle] Action<Exception> errorAction)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value);
        else (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!);
    }

    /// <summary> Try do action with value </summary>
    /// <param name="try"> Try item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="throwIfError"> Throw exception if item contains error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    [PublicAPI]
    public static void Do<T>(this Try<T> @try, [InstantHandle] Action<T> valueAction, bool throwIfError = false)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value);
        else if (throwIfError) @try.Throw();
    }

    /// <summary> Try do action with error </summary>
    /// <param name="try"> Try item </param>
    /// <param name="errorAction"> Action if error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    [PublicAPI]
    public static void DoIfError<T>(this Try<T> @try, [InstantHandle] Action<Exception> errorAction)
    {
        if (!(@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!);
    }

#endregion
}
