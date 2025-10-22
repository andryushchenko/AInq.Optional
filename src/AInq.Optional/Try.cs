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

/// <summary> Try utils </summary>
public static class Try
{
#region Value

    /// <inheritdoc cref="Try{T}.FromValue(T)" />
    [PublicAPI]
    public static Try<T> Value<T>([NoEnumeration] T value)
        => Try<T>.FromValue(value);

    /// <inheritdoc cref="Try{T}.FromError(Exception)" />
    [PublicAPI]
    public static Try<T> Error<T>(Exception exception)
        => Try<T>.FromError(exception);

    /// <inheritdoc cref="Try{T}.FromValue(T)" />
    [PublicAPI, Pure]
    public static Try<T> AsTry<T>([NoEnumeration] this T value)
        => Try<T>.FromValue(value);

    /// <inheritdoc cref="Try{T}.FromError(Exception)" />
    [PublicAPI, Pure]
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
    [PublicAPI, Pure]
    public static Try<TResult> Select<T, TResult>(this Try<T> @try, [InstantHandle] Func<T, TResult> selector)
    {
        if (!(@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            return Try<TResult>.ConvertError(@try);
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
    [PublicAPI, Pure]
    public static Try<TResult> Select<T, TResult>(this Try<T> @try, [InstantHandle] Func<T, Try<TResult>> selector)
    {
        if (!(@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            return Try<TResult>.ConvertError(@try);
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
    [PublicAPI, Pure]
    public static Try<T> Or<T>(this Try<T> @try, Try<T> other)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? @try
            : other ?? throw new ArgumentNullException(nameof(other));

    /// <summary> Get value form this item or other </summary>
    /// <param name="try"> Try item </param>
    /// <param name="otherGenerator"> Other generator </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static Try<T> Or<T>(this Try<T> @try, [InstantHandle] Func<Try<T>> otherGenerator)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? @try
            : (otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))).Invoke();

    /// <summary> Unwrap nested Try </summary>
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static Try<T> Unwrap<T>(this Try<Try<T>> @try)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? @try.Value
            : Try<T>.ConvertError(@try);

#endregion

#region Linq

    /// <summary> Select existing values </summary>
    /// <param name="collection"> Try collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <returns> Values collection </returns>
    [PublicAPI, LinqTunnel]
    public static IEnumerable<T> Values<T>(this IEnumerable<Try<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item is {Success: true})
           .Select(item => item.Value);

    /// <inheritdoc cref="Try.Values{T}(IEnumerable{Try{T}})" />
    [PublicAPI, LinqTunnel]
    public static ParallelQuery<T> Values<T>(this ParallelQuery<Try<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item is {Success: true})
           .Select(item => item.Value);

#endregion

#region Do

    /// <summary> Try to execute action </summary>
    /// <param name="try"> Try item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="errorAction"> Action if error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    [PublicAPI]
    public static void Do<T>(this Try<T> @try, [InstantHandle] Action<T> valueAction, [InstantHandle] Action<Exception> errorAction)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value);
        else
            try
            {
                @try.Throw();
            }
            catch (Exception exception)
            {
                (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(exception);
            }
    }

    /// <summary> Try to execute action with additional argument </summary>
    /// <param name="try"> Try item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="errorAction"> Action if error </param>
    /// <param name="argument"> Additional action argument </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TArgument"> Additional action argument type </typeparam>
    [PublicAPI]
    public static void Do<T, TArgument>(this Try<T> @try, [InstantHandle] Action<T, TArgument> valueAction,
        [InstantHandle] Action<Exception> errorAction, TArgument argument)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument);
        else
            try
            {
                @try.Throw();
            }
            catch (Exception exception)
            {
                (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(exception);
            }
    }

    /// <summary> Try to execute action with value </summary>
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

    /// <summary> Try to execute action with value </summary>
    /// <param name="try"> Try item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="argument"> Additional action argument </param>
    /// <param name="throwIfError"> Throw exception if item contains error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TArgument"> Additional action argument type </typeparam>
    [PublicAPI]
    public static void Do<T, TArgument>(this Try<T> @try, [InstantHandle] Action<T, TArgument> valueAction, TArgument argument,
        bool throwIfError = false)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument);
        else if (throwIfError) @try.Throw();
    }

    /// <summary> Try to execute action with error </summary>
    /// <param name="try"> Try item </param>
    /// <param name="errorAction"> Action if error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    [PublicAPI]
    public static void DoIfError<T>(this Try<T> @try, [InstantHandle] Action<Exception> errorAction)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success) return;
        try
        {
            @try.Throw();
        }
        catch (Exception exception)
        {
            (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(exception);
        }
    }

#endregion
}
