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

/// <summary> Try monad utils </summary>
public static class Try
{
    /// <summary> Create Try from value </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> Value<T>(T value)
        => new(value);

    /// <summary> Convert value to Try </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> AsTry<T>(this T value)
        => new(value);

    /// <summary> Create Try from exception </summary>
    /// <param name="exception"> Exception </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> Error<T>(Exception exception)
        => new(exception is AggregateException aggregate && aggregate.InnerExceptions.Count == 1 ? aggregate.InnerExceptions[0] : exception);

    /// <summary> Convert exception to Try </summary>
    /// <param name="exception"> Exception </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> AsTry<T>(this Exception exception)
        => new(exception is AggregateException aggregate && aggregate.InnerExceptions.Count == 1 ? aggregate.InnerExceptions[0] : exception);

    /// <summary> Create Try from value if not null </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> ValueIfNotNull<T>(T? value)
        where T : class
        => value == null ? new Try<T>(new ArgumentNullException(nameof(value))) : new Try<T>(value);

    /// <summary> Convert value to Try if not null </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> AsTryIfNotNull<T>(this T? value)
        where T : class
        => ValueIfNotNull(value);

    /// <inheritdoc cref="ValueIfNotNull{T}(T)" />
    public static Try<T> ValueIfNotNull<T>(T? value)
        where T : struct
        => value == null ? new Try<T>(new ArgumentNullException(nameof(value))) : new Try<T>(value.Value);

    /// <inheritdoc cref="AsTryIfNotNull{T}(T)" />
    public static Try<T> AsTryIfNotNull<T>(this T? value)
        where T : struct
        => ValueIfNotNull(value);

    /// <summary> Create Try from value generator </summary>
    /// <param name="generator"> Value generator </param>
    /// <typeparam name="T"> Value type </typeparam>
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

    /// <summary> Convert to other value type </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static Try<TResult> Select<T, TResult>(this Try<T> @try, Func<T, TResult> selector)
        => @try.Success ? Result(() => selector.Invoke(@try.Value)) : Error<TResult>(@try.Error!);

    /// <summary> Convert to other value type or default </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult? SelectOrDefault<T, TResult>(this Try<T> @try, Func<T, TResult> selector)
        => @try.Select(selector).ValueOrDefault();

    /// <summary> Convert to other value type or default </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Try<T> @try, Func<T, TResult> selector, TResult defaultValue)
        => @try.Select(selector).ValueOrDefault(defaultValue);

    /// <summary> Convert to other value type or default from generator </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Try<T> @try, Func<T, TResult> selector, Func<TResult> defaultGenerator)
        => @try.Success
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Convert to other value type </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static Try<TResult> Select<T, TResult>(this Try<T> @try, Func<T, Try<TResult>> selector)
        => @try.Success
            ? Result(() => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value)).Unwrap()
            : Error<TResult>(@try.Error!);

    /// <summary> Convert to other value type or default </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult? SelectOrDefault<T, TResult>(this Try<T> @try, Func<T, Try<TResult>> selector)
        => @try.Select(selector).ValueOrDefault();

    /// <summary> Convert to other value type or default </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Try<T> @try, Func<T, Try<TResult>> selector, TResult defaultValue)
        => @try.Select(selector).ValueOrDefault(defaultValue);

    /// <summary> Convert to other value type or default from generator </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static TResult SelectOrDefault<T, TResult>(this Try<T> @try, Func<T, Try<TResult>> selector, Func<TResult> defaultGenerator)
        => @try.Success
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(@try.Value)
              .ValueOrDefault((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke())
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Get value or default </summary>
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static T? ValueOrDefault<T>(this Try<T> @try)
        => @try.Success ? @try.Value : default;

    /// <summary> Get value or default </summary>
    /// <param name="try"> Try item </param>
    /// <param name="defaultValue"> Default value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static T ValueOrDefault<T>(this Try<T> @try, T defaultValue)
        => @try.Success ? @try.Value : defaultValue;

    /// <summary> Get value or default from generator </summary>
    /// <param name="try"> Try item </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static T ValueOrDefault<T>(this Try<T> @try, Func<T> defaultGenerator)
        => @try.Success ? @try.Value : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <summary> Get value form this item or other </summary>
    /// <param name="try"> Try item </param>
    /// <param name="other"> Other </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> Or<T>(this Try<T> @try, Try<T> other)
        => @try.Success ? @try : other;

    /// <summary> Unwrap nested Try </summary>
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static Try<T> Unwrap<T>(this Try<Try<T>> @try)
        => @try.Success ? @try.Value : Error<T>(@try.Error!);

    /// <summary> Select existing values </summary>
    /// <param name="collection"> Try collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <returns> Values collection </returns>
    public static IEnumerable<T> Values<T>(this IEnumerable<Try<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => item.Success)
           .Select(item => item.Value);

    /// <summary> Select exceptions </summary>
    /// <param name="collection"> Try collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <returns> Exceptions collection </returns>
    public static IEnumerable<Exception> Errors<T>(this IEnumerable<Try<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection)))
           .Where(item => !item.Success)
           .Select(item => item.Error!);

    /// <summary> Try do action with value </summary>
    /// <param name="try"> Try item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="throwIfError"> Throw exception if item contains error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static void Do<T>(this Try<T> @try, Action<T> valueAction, bool throwIfError = false)
    {
        if (@try.Success) (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value);
        else if (throwIfError) throw @try.Error!;
    }

    /// <summary> Try do action </summary>
    /// <param name="try"> Try item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="errorAction"> Action if error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static void Do<T>(this Try<T> @try, Action<T> valueAction, Action<Exception> errorAction)
    {
        if (@try.Success) (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value);
        else (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!);
    }

    /// <summary> Try do action with error </summary>
    /// <param name="try"> Try item </param>
    /// <param name="errorAction"> Action if error </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static void DoIfError<T>(this Try<T> @try, Action<Exception> errorAction)
    {
        if (!@try.Success) (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!);
    }
}
