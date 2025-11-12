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

/// <summary> <see cref="Try{T}" /> utils and extensions </summary>
public static partial class Try
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

    /// <summary> Create Try from value generator with argument </summary>
    /// <param name="generator"> Value generator </param>
    /// <param name="argument"> Generator argument </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <typeparam name="TArgument"> Generator argument type </typeparam>
    [PublicAPI]
    public static Try<T> Result<T, TArgument>([InstantHandle] Func<TArgument, T> generator, TArgument argument)
    {
        _ = generator ?? throw new ArgumentNullException(nameof(generator));
        try
        {
            return Value(generator.Invoke(argument));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Error<T>(ex);
        }
    }

    /// <summary> Unwrap nested Try </summary>
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Value type </typeparam>
    [PublicAPI, Pure]
    public static Try<T> Unwrap<T>(this Try<Try<T>> @try)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success ? @try.Value : Try<T>.ConvertError(@try);

#endregion
}
