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
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasValue
            ? Maybe.Value(await (selector ?? throw new ArgumentNullException(nameof(selector)))
                                .Invoke(item.Value, cancellation)
                                .ConfigureAwait(false))
            : Maybe.None<TResult>();
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> item, Func<T, CancellationToken, Task<Maybe<TResult>>> selector,
        CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(item.Value, cancellation)
                    .ConfigureAwait(false)
            : Maybe.None<TResult>();
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> item, Func<T, Task<TResult>> selector)
        => item.HasValue
            ? Maybe.Value(await (selector ?? throw new ArgumentNullException(nameof(selector)))
                                .Invoke(item.Value)
                                .ConfigureAwait(false))
            : Maybe.None<TResult>();

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> item, Func<T, Task<Maybe<TResult>>> selector)
        => item.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(item.Value)
                    .ConfigureAwait(false)
            : Maybe.None<TResult>();

    /// <summary> Convert to other value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="selector"> Converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static async Task<Try<TResult>> SelectAsync<T, TResult>(this Try<T> item, Func<T, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.Success
            ? await ResultAsync((selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(item.Value, cancellation))
                .ConfigureAwait(false)
            : Try.Error<TResult>(item.Error!);
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<Try<TResult>> SelectAsync<T, TResult>(this Try<T> item, Func<T, CancellationToken, Task<Try<TResult>>> selector,
        CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.Success
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(item.Value, cancellation)
                    .ConfigureAwait(false)
            : Try.Error<TResult>(item.Error!);
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<Try<TResult>> SelectAsync<T, TResult>(this Try<T> item, Func<T, Task<TResult>> selector)
        => item.Success
            ? await ResultAsync((selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(item.Value))
                .ConfigureAwait(false)
            : Try.Error<TResult>(item.Error!);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<Try<TResult>> SelectAsync<T, TResult>(this Try<T> item, Func<T, Task<Try<TResult>>> selector)
        => item.Success
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(item.Value)
                    .ConfigureAwait(false)
            : Try.Error<TResult>(item.Error!);

    /// <summary> Convert to other left value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftSelector"> Left value converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    /// <typeparam name="TLeftResult"> Left result type </typeparam>
    public static async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<TLeftResult>> leftSelector, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasLeft
            ? Either.Left<TLeftResult, TRight>(await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)))
                                                     .Invoke(item.Left, cancellation)
                                                     .ConfigureAwait(false))
            : Either.Right<TLeftResult, TRight>(item.Right);
    }

    /// <inheritdoc cref="SelectLeftAsync{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<Either<TLeftResult, TRight>>> leftSelector, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasLeft
            ? await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left, cancellation).ConfigureAwait(false)
            : Either.Right<TLeftResult, TRight>(item.Right);
    }

    /// <inheritdoc cref="SelectLeftAsync{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<TLeftResult>> leftSelector)
        => item.HasLeft
            ? Either.Left<TLeftResult, TRight>(await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)))
                                                     .Invoke(item.Left)
                                                     .ConfigureAwait(false))
            : Either.Right<TLeftResult, TRight>(item.Right);

    /// <inheritdoc cref="SelectLeftAsync{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<Either<TLeftResult, TRight>>> leftSelector)
        => item.HasLeft
            ? await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left).ConfigureAwait(false)
            : Either.Right<TLeftResult, TRight>(item.Right);

    /// <summary> Convert to other right value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="rightSelector"> Right value converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TRightResult"> Right result type </typeparam>
    public static async Task<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, CancellationToken, Task<TRightResult>> rightSelector, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasRight
            ? Either.Right<TLeft, TRightResult>(await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)))
                                                      .Invoke(item.Right, cancellation)
                                                      .ConfigureAwait(false))
            : Either.Left<TLeft, TRightResult>(item.Left);
    }

    /// <inheritdoc cref="SelectRightAsync{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, CancellationToken, Task<Either<TLeft, TRightResult>>> rightSelector, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasRight
            ? await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right, cancellation).ConfigureAwait(false)
            : Either.Left<TLeft, TRightResult>(item.Left);
    }

    /// <inheritdoc cref="SelectRightAsync{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, Task<TRightResult>> rightSelector)
        => item.HasRight
            ? Either.Right<TLeft, TRightResult>(await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)))
                                                      .Invoke(item.Right)
                                                      .ConfigureAwait(false))
            : Either.Left<TLeft, TRightResult>(item.Left);

    /// <inheritdoc cref="SelectRightAsync{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> item,
        Func<TRight, Task<Either<TLeft, TRightResult>>> rightSelector)
        => item.HasRight
            ? await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right).ConfigureAwait(false)
            : Either.Left<TLeft, TRightResult>(item.Left);

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
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasLeft
            ? Either.Left<TLeftResult, TRightResult>(await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)))
                                                           .Invoke(item.Left, cancellation)
                                                           .ConfigureAwait(false))
            : Either.Right<TLeftResult, TRightResult>(await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)))
                                                            .Invoke(item.Right, cancellation)
                                                            .ConfigureAwait(false));
    }

    /// <inheritdoc cref="SelectAsync{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<Either<TLeftResult, TRightResult>>> leftSelector,
        Func<TRight, CancellationToken, Task<TRightResult>> rightSelector, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasLeft
            ? await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left, cancellation).ConfigureAwait(false)
            : Either.Right<TLeftResult, TRightResult>(await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)))
                                                            .Invoke(item.Right, cancellation)
                                                            .ConfigureAwait(false));
    }

    /// <inheritdoc cref="SelectAsync{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<TLeftResult>> leftSelector,
        Func<TRight, CancellationToken, Task<Either<TLeftResult, TRightResult>>> rightSelector, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasLeft
            ? Either.Left<TLeftResult, TRightResult>(await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)))
                                                           .Invoke(item.Left, cancellation)
                                                           .ConfigureAwait(false))
            : await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectAsync{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<Either<TLeftResult, TRightResult>>> leftSelector,
        Func<TRight, CancellationToken, Task<Either<TLeftResult, TRightResult>>> rightSelector, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasLeft
            ? await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left, cancellation).ConfigureAwait(false)
            : await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectAsync{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<TLeftResult>> leftSelector, Func<TRight, Task<TRightResult>> rightSelector)
        => item.HasLeft
            ? Either.Left<TLeftResult, TRightResult>(await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)))
                                                           .Invoke(item.Left)
                                                           .ConfigureAwait(false))
            : Either.Right<TLeftResult, TRightResult>(await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)))
                                                            .Invoke(item.Right)
                                                            .ConfigureAwait(false));

    /// <inheritdoc cref="SelectAsync{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<Either<TLeftResult, TRightResult>>> leftSelector, Func<TRight, Task<TRightResult>> rightSelector)
        => item.HasLeft
            ? await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left).ConfigureAwait(false)
            : Either.Right<TLeftResult, TRightResult>(await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)))
                                                            .Invoke(item.Right)
                                                            .ConfigureAwait(false));

    /// <inheritdoc cref="SelectAsync{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<TLeftResult>> leftSelector, Func<TRight, Task<Either<TLeftResult, TRightResult>>> rightSelector)
        => item.HasLeft
            ? Either.Left<TLeftResult, TRightResult>(await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)))
                                                           .Invoke(item.Left)
                                                           .ConfigureAwait(false))
            : await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TLeftResult}},Func{TRight,CancellationToken,Task{TRightResult}},CancellationToken)" />
    public static async Task<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(this Either<TLeft, TRight> item,
        Func<TLeft, Task<Either<TLeftResult, TRightResult>>> leftSelector, Func<TRight, Task<Either<TLeftResult, TRightResult>>> rightSelector)
        => item.HasLeft
            ? await (leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))).Invoke(item.Left).ConfigureAwait(false)
            : await (rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))).Invoke(item.Right).ConfigureAwait(false);

    /// <summary> Convert to left value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="rightToLeft"> Right to left converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static async Task<TLeft> ToLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> item,
        Func<TRight, CancellationToken, Task<TLeft>> rightToLeft, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasLeft
            ? item.Left
            : await (rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft))).Invoke(item.Right, cancellation).ConfigureAwait(false);
    }

    /// <summary> Convert to right value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftToRight"> Left to right converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    public static async Task<TRight> ToRightAsync<TLeft, TRight>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<TRight>> leftToRight, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasRight
            ? item.Right
            : await (leftToRight ?? throw new ArgumentNullException(nameof(leftToRight))).Invoke(item.Left, cancellation).ConfigureAwait(false);
    }

    /// <summary> Convert to other value type asynchronously </summary>
    /// <param name="item"> Source </param>
    /// <param name="fromLeft"> Left value converter </param>
    /// <param name="fromRight"> Right value converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    /// <typeparam name="TResult"> Left result type </typeparam>
    public static async Task<TResult> ToValueAsync<TLeft, TRight, TResult>(this Either<TLeft, TRight> item,
        Func<TLeft, CancellationToken, Task<TResult>> fromLeft,
        Func<TRight, CancellationToken, Task<TResult>> fromRight, CancellationToken cancellation = default)
    {
        cancellation.ThrowIfCancellationRequested();
        return item.HasLeft
            ? await (fromLeft ?? throw new ArgumentNullException(nameof(fromLeft))).Invoke(item.Left, cancellation).ConfigureAwait(false)
            : await (fromRight ?? throw new ArgumentNullException(nameof(fromRight))).Invoke(item.Right, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ToLeftAsync{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,CancellationToken,Task{TLeft}},CancellationToken)" />
    public static async Task<TLeft> ToLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TRight, Task<TLeft>> rightToLeft)
        => item.HasLeft
            ? item.Left
            : await (rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft))).Invoke(item.Right).ConfigureAwait(false);

    /// <inheritdoc cref="ToRightAsync{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TRight}},CancellationToken)" />
    public static async Task<TRight> ToRightAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft, Task<TRight>> leftToRight)
        => item.HasRight
            ? item.Right
            : await (leftToRight ?? throw new ArgumentNullException(nameof(leftToRight))).Invoke(item.Left).ConfigureAwait(false);

    /// <inheritdoc cref="ToValueAsync{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task{TResult}},Func{TRight,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async Task<TResult> ToValueAsync<TLeft, TRight, TResult>(this Either<TLeft, TRight> item, Func<TLeft, Task<TResult>> fromLeft,
        Func<TRight, Task<TResult>> fromRight)
        => item.HasLeft
            ? await (fromLeft ?? throw new ArgumentNullException(nameof(fromLeft))).Invoke(item.Left).ConfigureAwait(false)
            : await (fromRight ?? throw new ArgumentNullException(nameof(fromRight))).Invoke(item.Right).ConfigureAwait(false);
}
