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

/// <summary> Asynchronous utils </summary>
public static class AsyncExtension
{
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
        catch (Exception ex) when (ex is not OperationCanceledException)
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
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return new Try<T>(ex);
        }
    }

    /// <summary> Convert to other value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="selector"> Converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> item, Func<T, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellation = default)
        => item.HasValue ? Maybe.Value(await selector.Invoke(item.Value, cancellation).ConfigureAwait(false)) : Maybe.None<TResult>();

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> item, Func<T, Task<TResult>> selector)
        => await item.SelectAsync((value, _) => selector.Invoke(value)).ConfigureAwait(false);

    /// <summary> Convert to other value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="selector"> Converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static async Task<Try<TResult>> SelectAsync<T, TResult>(this Try<T> item, Func<T, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellation = default)
        => item.Success ? await ResultAsync(selector.Invoke(item.Value, cancellation)).ConfigureAwait(false) : Try.Error<TResult>(item.Error!);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<Try<TResult>> SelectAsync<T, TResult>(this Try<T> item, Func<T, Task<TResult>> selector)
        => await item.SelectAsync((source, _) => selector.Invoke(source)).ConfigureAwait(false);

    /// <summary> Convert to other left value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    public static async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<TLeftResult>> leftSelector, CancellationToken cancellation = default)
        => item.HasLeft
            ? Either.Left<TLeftResult, TRight>(await leftSelector.Invoke(item.Left, cancellation).ConfigureAwait(false))
            : Either.Right<TLeftResult, TRight>(item.Right);

    /// <inheritdoc cref="SelectLeftAsync{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<TLeftResult>> leftSelector)
        => await item.SelectLeftAsync((source, _) => leftSelector.Invoke(source)).ConfigureAwait(false);

    /// <summary> Convert to other right value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    public static async Task<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, CancellationToken, Task<TRightResult>> rightSelector, CancellationToken cancellation = default)
        => item.HasLeft
            ? Either.Left<TLeft, TRightResult>(item.Left)
            : Either.Right<TLeft, TRightResult>(await rightSelector.Invoke(item.Right, cancellation).ConfigureAwait(false));

    /// <inheritdoc
    ///     cref="SelectRightAsync{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, Task<TRightResult>> rightSelector)
        => await item.SelectRightAsync((source, _) => rightSelector.Invoke(source)).ConfigureAwait(false);

    /// <summary> Convert to other type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<TLeftResult>> leftSelector, Func<TRight, CancellationToken, Task<TRightResult>> rightSelector,
        CancellationToken cancellation = default)
        => item.HasLeft
            ? Either.Left<TLeftResult, TRightResult>(await leftSelector.Invoke(item.Left, cancellation).ConfigureAwait(false))
            : Either.Right<TLeftResult, TRightResult>(await rightSelector.Invoke(item.Right, cancellation).ConfigureAwait(false));

    /// <inheritdoc
    ///     cref="SelectAsync{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<TLeftResult>> leftSelector, Func<TRight, Task<TRightResult>> rightSelector)
        => await item.SelectAsync((source, _) => leftSelector.Invoke(source), (source, _) => rightSelector.Invoke(source)).ConfigureAwait(false);
}
