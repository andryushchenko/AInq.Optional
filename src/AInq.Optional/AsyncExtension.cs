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
using System.Threading.Tasks;

namespace AInq.Optional
{

/// <summary> Asynchronous utils </summary>
public static class AsyncExtension
{
    /// <summary> Create Try from async value generator </summary>
    /// <param name="generator"> Value generator </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async Task<Try<T>> ResultAsync<T>(Func<Task<T>> generator)
    {
        try
        {
            var result = await generator.Invoke().ConfigureAwait(false);
            return new Try<T>(result);
        }
        catch (Exception ex)
        {
            return new Try<T>(ex);
        }
    }

    /// <summary> Create Try from value task </summary>
    /// <param name="task"> Value task </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async Task<Try<T>> ResultAsync<T>(Task<T> task)
    {
        try
        {
            var result = await task.ConfigureAwait(false);
            return new Try<T>(result);
        }
        catch (Exception ex)
        {
            return new Try<T>(ex);
        }
    }

    /// <summary> Unwrap to Task </summary>
    /// <param name="task"> Try with task </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async Task<Try<T>> UnwrapAsync<T>(this Try<Task<T>> task)
    {
        if (!task.Success) return Try.Error<T>(task.Error!);
        try
        {
            var result = await task.Value.ConfigureAwait(false);
            return new Try<T>(result);
        }
        catch (Exception ex)
        {
            return new Try<T>(ex);
        }
    }

    /// <summary> Convert to other value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> item, Func<T, Task<TResult>> selector)
        => item.HasValue ? Maybe.Value(await selector.Invoke(item.Value).ConfigureAwait(false)) : Maybe.None<TResult>();

    /// <summary> Convert to other value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="selector"> Converter </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static async Task<Try<TResult>> SelectAsync<T, TResult>(this Try<T> item, Func<T, Task<TResult>> selector)
        => item.Success ? await ResultAsync(selector.Invoke(item.Value)).ConfigureAwait(false) : Try.Error<TResult>(item.Error!);

    /// <summary> Convert to other left value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftSelector"> Converter </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    public static async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<TLeftResult>> leftSelector)
        => item.HasLeft
            ? Either.Left<TLeftResult, TRight>(await leftSelector.Invoke(item.Left).ConfigureAwait(false))
            : Either.Right<TLeftResult, TRight>(item.Right);

    /// <summary> Convert to other right value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="rightSelector"> Converter </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    public static async Task<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, Task<TRightResult>> rightSelector)
        => item.HasLeft
            ? Either.Left<TLeft, TRightResult>(item.Left)
            : Either.Right<TLeft, TRightResult>(await rightSelector.Invoke(item.Right).ConfigureAwait(false));
}

}
