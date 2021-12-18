﻿// Copyright 2021 Anton Andryushchenko
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

using System.Runtime.CompilerServices;
using static AInq.Optional.TaskHelper;

namespace AInq.Optional;

/// <summary> Either async extension </summary>
public static class EitherAsync
{
#region ValueAsync

    private static async ValueTask<Either<TLeft, TRight>> FromTaskAsync<TLeft, TRight>(Task<TLeft> leftTask)
        => Either.Left<TLeft, TRight>(await leftTask.ConfigureAwait(false));

    private static async ValueTask<Either<TLeft, TRight>> FromTaskAsync<TLeft, TRight>(Task<TRight> rightTask)
        => Either.Right<TLeft, TRight>(await rightTask.ConfigureAwait(false));

    /// <inheritdoc cref="Either.Left{TLeft,TRight}(TLeft)" />
    public static ValueTask<Either<TLeft, TRight>> LeftAsync<TLeft, TRight>(Task<TLeft> leftTask, CancellationToken cancellation = default)
        => (leftTask ?? throw new ArgumentNullException(nameof(leftTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(Either.Left<TLeft, TRight>(leftTask.Result))
            : FromTaskAsync<TLeft, TRight>(leftTask.WaitAsync(cancellation));

    /// <inheritdoc cref="Either.Left{TLeft,TRight}(TLeft)" />
    public static ValueTask<Either<TLeft, TRight>> LeftAsync<TLeft, TRight>(ValueTask<TLeft> leftValueTask, CancellationToken cancellation = default)
        => leftValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(Either.Left<TLeft, TRight>(leftValueTask.Result))
            : FromTaskAsync<TLeft, TRight>(leftValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Either.Right{TLeft,TRight}(TRight)" />
    public static ValueTask<Either<TLeft, TRight>> RightAsync<TLeft, TRight>(Task<TRight> rightTask, CancellationToken cancellation = default)
        => (rightTask ?? throw new ArgumentNullException(nameof(rightTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(Either.Right<TLeft, TRight>(rightTask.Result))
            : FromTaskAsync<TLeft, TRight>(rightTask.WaitAsync(cancellation));

    /// <inheritdoc cref="Either.Right{TLeft,TRight}(TRight)" />
    public static ValueTask<Either<TLeft, TRight>> RightAsync<TLeft, TRight>(ValueTask<TRight> rightValueTask,
        CancellationToken cancellation = default)
        => rightValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(Either.Right<TLeft, TRight>(rightValueTask.Result))
            : FromTaskAsync<TLeft, TRight>(rightValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="LeftAsync{TLeft,TRight}(Task{TLeft},CancellationToken)" />
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this Task<TLeft> leftTask, CancellationToken cancellation = default)
        => LeftAsync<TLeft, TRight>(leftTask ?? throw new ArgumentNullException(nameof(leftTask)), cancellation);

    /// <inheritdoc cref="LeftAsync{TLeft,TRight}(ValueTask{TLeft},CancellationToken)" />
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this ValueTask<TLeft> leftValueTask,
        CancellationToken cancellation = default)
        => LeftAsync<TLeft, TRight>(leftValueTask, cancellation);

    /// <inheritdoc cref="RightAsync{TLeft,TRight}(Task{TRight},CancellationToken)" />
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this Task<TRight> rightTask, CancellationToken cancellation = default)
        => RightAsync<TLeft, TRight>(rightTask ?? throw new ArgumentNullException(nameof(rightTask)), cancellation);

    /// <inheritdoc cref="RightAsync{TLeft,TRight}(ValueTask{TRight},CancellationToken)" />
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this ValueTask<TRight> rightValueTask,
        CancellationToken cancellation = default)
        => RightAsync<TLeft, TRight>(rightValueTask, cancellation);

#endregion

#region Select

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeft, TRight, TLeftResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TLeftResult> leftSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRight>>(eitherTask.Result.SelectLeft(leftSelector))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeft(leftSelector));

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeft, TRight, TLeftResult>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, TLeftResult> leftSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRight>>(eitherValueTask.Result.SelectLeft(leftSelector))
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectLeft(leftSelector));

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeft, TRight, TLeftResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Either<TLeftResult, TRight>> leftSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRight>>(eitherTask.Result.SelectLeft(leftSelector))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeft(leftSelector));

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeft, TRight, TLeftResult>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, Either<TLeftResult, TRight>> leftSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRight>>(eitherValueTask.Result.SelectLeft(leftSelector))
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectLeft(leftSelector));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRight<TLeft, TRight, TRightResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRightResult>>(eitherTask.Result.SelectRight(rightSelector))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRight(rightSelector));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRight<TLeft, TRight, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRightResult>>(eitherValueTask.Result.SelectRight(rightSelector))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectRight(rightSelector));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRight<TLeft, TRight, TRightResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, Either<TLeft, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRightResult>>(eitherTask.Result.SelectRight(rightSelector))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRight(rightSelector));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRight<TLeft, TRight, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TRight, Either<TLeft, TRightResult>> rightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRightResult>>(eitherValueTask.Result.SelectRight(rightSelector))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectRight(rightSelector));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(leftSelector, rightSelector))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherValueTask.Result.Select(leftSelector, rightSelector))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(leftSelector, rightSelector))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherValueTask.Result.Select(leftSelector, rightSelector))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TLeftResult> leftSelector,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(leftSelector, rightSelector))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, TLeftResult> leftSelector,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherValueTask.Result.Select(leftSelector, rightSelector))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(leftSelector, rightSelector))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherValueTask.Result.Select(leftSelector, rightSelector))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector));

#endregion

#region SelectLeftAsync

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> either,
        Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftAsyncSelector ?? throw new ArgumentNullException(nameof(leftAsyncSelector)))
              .Invoke(either.Left, cancellation)
              .AsEitherAsync<TLeftResult, TRight>(cancellation)
            : new ValueTask<Either<TLeftResult, TRight>>(Either.Right<TLeftResult, TRight>(either.Right));

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> either,
        Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> leftAsyncSelector, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftAsyncSelector ?? throw new ArgumentNullException(nameof(leftAsyncSelector))).Invoke(either.Left, cancellation)
            : new ValueTask<Either<TLeftResult, TRight>>(Either.Right<TLeftResult, TRight>(either.Right));

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectLeftAsync(leftAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectLeftAsync(leftAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> leftAsyncSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectLeftAsync(leftAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectLeftAsync(leftAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectLeftAsync(leftAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectLeftAsync(leftAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> leftAsyncSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectLeftAsync(leftAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectLeftAsync(leftAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region SelectRightAsync

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? (rightAsyncSelector ?? throw new ArgumentNullException(nameof(rightAsyncSelector)))
              .Invoke(either.Right, cancellation)
              .AsEitherAsync<TLeft, TRightResult>(cancellation)
            : new ValueTask<Either<TLeft, TRightResult>>(Either.Left<TLeft, TRightResult>(either.Left));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either,
        Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? (rightAsyncSelector ?? throw new ArgumentNullException(nameof(rightAsyncSelector))).Invoke(either.Right, cancellation)
            : new ValueTask<Either<TLeft, TRightResult>>(Either.Left<TLeft, TRightResult>(either.Left));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectRightAsync(rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectRightAsync(rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectRightAsync(rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectRightAsync(rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectRightAsync(rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectRightAsync(rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectRightAsync(rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectRightAsync(rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region SelectAsync

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftAsyncSelector ?? throw new ArgumentNullException(nameof(leftAsyncSelector)))
              .Invoke(either.Left, cancellation)
              .AsEitherAsync<TLeftResult, TRightResult>(cancellation)
            : (rightAsyncSelector ?? throw new ArgumentNullException(nameof(rightAsyncSelector)))
              .Invoke(either.Right, cancellation)
              .AsEitherAsync<TLeftResult, TRightResult>(cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftAsyncSelector ?? throw new ArgumentNullException(nameof(leftAsyncSelector))).Invoke(either.Left, cancellation)
            : (rightAsyncSelector ?? throw new ArgumentNullException(nameof(rightAsyncSelector)))
              .Invoke(either.Right, cancellation)
              .AsEitherAsync<TLeftResult, TRightResult>(cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftAsyncSelector ?? throw new ArgumentNullException(nameof(leftAsyncSelector)))
              .Invoke(either.Left, cancellation)
              .AsEitherAsync<TLeftResult, TRightResult>(cancellation)
            : (rightAsyncSelector ?? throw new ArgumentNullException(nameof(rightAsyncSelector))).Invoke(either.Right, cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (leftAsyncSelector ?? throw new ArgumentNullException(nameof(leftAsyncSelector))).Invoke(either.Left, cancellation)
            : (rightAsyncSelector ?? throw new ArgumentNullException(nameof(rightAsyncSelector))).Invoke(either.Right, cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> rightAsyncSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> leftAsyncSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> rightAsyncSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(leftAsyncSelector, rightAsyncSelector, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region ValueOrDefault

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<TLeft?> LeftOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TLeft?>(eitherTask.Result.LeftOrDefault())
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault());

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<TLeft?> LeftOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TLeft?>(eitherValueTask.Result.LeftOrDefault())
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault());

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    public static ValueTask<TLeft> LeftOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, TLeft defaultValue,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TLeft>(eitherTask.Result.LeftOrDefault(defaultValue))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault(defaultValue));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    public static ValueTask<TLeft> LeftOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, TLeft defaultValue,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TLeft>(eitherValueTask.Result.LeftOrDefault(defaultValue))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault(defaultValue));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    public static ValueTask<TLeft> LeftOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft> defaultGenerator,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TLeft>(eitherTask.Result.LeftOrDefault(defaultGenerator))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault(defaultGenerator));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    public static ValueTask<TLeft> LeftOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft> defaultGenerator,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TLeft>(eitherValueTask.Result.LeftOrDefault(defaultGenerator))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault(defaultGenerator));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<TRight?> RightOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TRight?>(eitherTask.Result.RightOrDefault())
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault());

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<TRight?> RightOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TRight?>(eitherValueTask.Result.RightOrDefault())
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault());

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    public static ValueTask<TRight> RightOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, TRight defaultValue,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TRight>(eitherTask.Result.RightOrDefault(defaultValue))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault(defaultValue));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    public static ValueTask<TRight> RightOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, TRight defaultValue,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TRight>(eitherValueTask.Result.RightOrDefault(defaultValue))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault(defaultValue));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    public static ValueTask<TRight> RightOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Func<TRight> defaultGenerator,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TRight>(eitherTask.Result.RightOrDefault(defaultGenerator))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault(defaultGenerator));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    public static ValueTask<TRight> RightOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TRight> defaultGenerator, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TRight>(eitherValueTask.Result.RightOrDefault(defaultGenerator))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault(defaultGenerator));

#endregion

#region ValueOrDefaultAsync

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this Either<TLeft, TRight> either, Task<TLeft> defaultTask,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? new ValueTask<TLeft>(either.Left)
            : (defaultTask ?? throw new ArgumentNullException(nameof(defaultTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TLeft>(defaultTask.Result)
                : new ValueTask<TLeft>(defaultTask.WaitAsync(cancellation));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this Either<TLeft, TRight> either, ValueTask<TLeft> defaultValueTask,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? new ValueTask<TLeft>(either.Left)
            : defaultValueTask.IsCompletedSuccessfully
                ? defaultValueTask
                : new ValueTask<TLeft>(defaultValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        Func<CancellationToken, ValueTask<TLeft>> defaultAsyncGenerator, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? new ValueTask<TLeft>(either.Left)
            : (defaultAsyncGenerator ?? throw new ArgumentNullException(nameof(defaultAsyncGenerator))).Invoke(cancellation);

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Task<TLeft> defaultTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.LeftOrDefaultAsync(defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .LeftOrDefaultAsync(defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, ValueTask<TLeft> defaultValueTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.LeftOrDefaultAsync(defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .LeftOrDefaultAsync(defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<CancellationToken, ValueTask<TLeft>> defaultAsyncGenerator, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.LeftOrDefaultAsync(defaultAsyncGenerator, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .LeftOrDefaultAsync(defaultAsyncGenerator, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, Task<TLeft> defaultTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.LeftOrDefaultAsync(defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .LeftOrDefaultAsync(defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        ValueTask<TLeft> defaultValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.LeftOrDefaultAsync(defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .LeftOrDefaultAsync(defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<CancellationToken, ValueTask<TLeft>> defaultAsyncGenerator, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.LeftOrDefaultAsync(defaultAsyncGenerator, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .LeftOrDefaultAsync(defaultAsyncGenerator, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this Either<TLeft, TRight> either, Task<TRight> defaultTask,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? new ValueTask<TRight>(either.Right)
            : (defaultTask ?? throw new ArgumentNullException(nameof(defaultTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TRight>(defaultTask.Result)
                : new ValueTask<TRight>(defaultTask.WaitAsync(cancellation));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this Either<TLeft, TRight> either, ValueTask<TRight> defaultValueTask,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? new ValueTask<TRight>(either.Right)
            : defaultValueTask.IsCompletedSuccessfully
                ? defaultValueTask
                : new ValueTask<TRight>(defaultValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        Func<CancellationToken, ValueTask<TRight>> defaultAsyncGenerator, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? new ValueTask<TRight>(either.Right)
            : (defaultAsyncGenerator ?? throw new ArgumentNullException(nameof(defaultAsyncGenerator))).Invoke(cancellation);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Task<TRight> defaultTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.RightOrDefaultAsync(defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .RightOrDefaultAsync(defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        ValueTask<TRight> defaultValueTask, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.RightOrDefaultAsync(defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .RightOrDefaultAsync(defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<CancellationToken, ValueTask<TRight>> defaultAsyncGenerator, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.RightOrDefaultAsync(defaultAsyncGenerator, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .RightOrDefaultAsync(defaultAsyncGenerator, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Task<TRight> defaultTask, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.RightOrDefaultAsync(defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .RightOrDefaultAsync(defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        ValueTask<TRight> defaultValueTask, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.RightOrDefaultAsync(defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .RightOrDefaultAsync(defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<CancellationToken, ValueTask<TRight>> defaultAsyncGenerator, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.RightOrDefaultAsync(defaultAsyncGenerator, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .RightOrDefaultAsync(defaultAsyncGenerator, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region ToValue

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    public static ValueTask<TLeft> ToLeft<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Func<TRight, TLeft> rightToLeft,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TLeft>(eitherTask.Result.ToLeft(rightToLeft))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToLeft(rightToLeft));

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    public static ValueTask<TLeft> ToLeft<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TRight, TLeft> rightToLeft,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TLeft>(eitherValueTask.Result.ToLeft(rightToLeft))
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ToLeft(rightToLeft));

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    public static ValueTask<TRight> ToRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TRight> leftToRight,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TRight>(eitherTask.Result.ToRight(leftToRight))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToRight(leftToRight));

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    public static ValueTask<TRight> ToRight<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, TRight> leftToRight,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TRight>(eitherValueTask.Result.ToRight(leftToRight))
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ToRight(leftToRight));

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    public static ValueTask<TResult> ToValue<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TResult> fromLeft,
        Func<TRight, TResult> fromRight, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult>(eitherTask.Result.ToValue(fromLeft, fromRight))
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToValue(fromLeft, fromRight));

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    public static ValueTask<TResult> ToValue<TLeft, TRight, TResult>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, TResult> fromLeft, Func<TRight, TResult> fromRight, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult>(eitherValueTask.Result.ToValue(fromLeft, fromRight))
            : FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ToValue(fromLeft, fromRight));

#endregion

#region ToValueAsync

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    public static ValueTask<TLeft> ToLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        Func<TRight, CancellationToken, ValueTask<TLeft>> rightToLeftAsync, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? new ValueTask<TLeft>(either.Left)
            : (rightToLeftAsync ?? throw new ArgumentNullException(nameof(rightToLeftAsync))).Invoke(either.Right, cancellation);

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    public static ValueTask<TLeft> ToLeftAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, CancellationToken, ValueTask<TLeft>> rightToLeftAsync, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.ToLeftAsync(rightToLeftAsync, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ToLeftAsync(rightToLeftAsync, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    public static ValueTask<TLeft> ToLeftAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TRight, CancellationToken, ValueTask<TLeft>> rightToLeftAsync, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.ToLeftAsync(rightToLeftAsync, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ToLeftAsync(rightToLeftAsync, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    public static ValueTask<TRight> ToRightAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        Func<TLeft, CancellationToken, ValueTask<TRight>> leftToRightAsync, CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? new ValueTask<TRight>(either.Right)
            : (leftToRightAsync ?? throw new ArgumentNullException(nameof(leftToRightAsync))).Invoke(either.Left, cancellation);

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    public static ValueTask<TRight> ToRightAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<TRight>> leftToRightAsync, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.ToRightAsync(leftToRightAsync, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ToRightAsync(leftToRightAsync, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    public static ValueTask<TRight> ToRightAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, CancellationToken, ValueTask<TRight>> leftToRightAsync, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.ToRightAsync(leftToRightAsync, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ToRightAsync(leftToRightAsync, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    public static ValueTask<TResult> ToValueAsync<TLeft, TRight, TResult>(this Either<TLeft, TRight> either,
        Func<TLeft, CancellationToken, ValueTask<TResult>> fromLeftAsync, Func<TRight, CancellationToken, ValueTask<TResult>> fromRightAsync,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (fromLeftAsync ?? throw new ArgumentNullException(nameof(fromLeftAsync))).Invoke(either.Left, cancellation)
            : (fromRightAsync ?? throw new ArgumentNullException(nameof(fromRightAsync))).Invoke(either.Right, cancellation);

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    public static ValueTask<TResult> ToValueAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<TResult>> fromLeftAsync, Func<TRight, CancellationToken, ValueTask<TResult>> fromRightAsync,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.ToValueAsync(fromLeftAsync, fromRightAsync, cancellation)
            : FromFunctionAsync(async () => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ToValueAsync(fromLeftAsync, fromRightAsync, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    public static ValueTask<TResult> ToValueAsync<TLeft, TRight, TResult>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, CancellationToken, ValueTask<TResult>> fromLeftAsync, Func<TRight, CancellationToken, ValueTask<TResult>> fromRightAsync,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.ToValueAsync(fromLeftAsync, fromRightAsync, cancellation)
            : FromFunctionAsync(async () => await (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ToValueAsync(fromLeftAsync, fromRightAsync, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region Utils

    /// <inheritdoc cref="Either.Invert{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Either<TRight, TLeft>> Invert<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TRight, TLeft>>(eitherTask.Result.Invert())
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Invert());

    /// <inheritdoc cref="Either.Invert{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Either<TRight, TLeft>> Invert<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TRight, TLeft>>(eitherValueTask.Result.Invert())
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Invert());

    /// <inheritdoc cref="Either.LeftValues{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}})" />
    public static async IAsyncEnumerable<TLeft> LeftValues<TLeft, TRight>(this IAsyncEnumerable<Either<TLeft, TRight>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var either in collection.WithCancellation(cancellation).ConfigureAwait(false))
            // ReSharper disable once ConstantConditionalAccessQualifier
            if (either?.HasLeft ?? false)
                yield return either.Left;
    }

    /// <inheritdoc cref="Either.RightValues{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}})" />
    public static async IAsyncEnumerable<TRight> RightValues<TLeft, TRight>(this IAsyncEnumerable<Either<TLeft, TRight>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var either in collection.WithCancellation(cancellation).ConfigureAwait(false))
            // ReSharper disable once ConstantConditionalAccessQualifier
            if (either?.HasRight ?? false)
                yield return either.Right;
    }

#endregion

#region Do

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    public static ValueTask Do<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Action<TLeft> leftAction, Action<TRight> rightAction,
        CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(leftAction, rightAction));
        eitherTask.Result.Do(leftAction, rightAction);
        return default;
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    public static ValueTask DoLeft<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Action<TLeft> leftAction,
        CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoLeft(leftAction));
        eitherTask.Result.DoLeft(leftAction);
        return default;
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    public static ValueTask DoRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Action<TRight> rightAction,
        CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoRight(rightAction));
        eitherTask.Result.DoRight(rightAction);
        return default;
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    public static ValueTask Do<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, Action<TLeft> leftAction,
        Action<TRight> rightAction, CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(leftAction, rightAction));
        eitherValueTask.Result.Do(leftAction, rightAction);
        return default;
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    public static ValueTask DoLeft<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, Action<TLeft> leftAction,
        CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).DoLeft(leftAction));
        eitherValueTask.Result.DoLeft(leftAction);
        return default;
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    public static ValueTask DoRight<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask, Action<TRight> rightAction,
        CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).DoRight(rightAction));
        eitherValueTask.Result.DoRight(rightAction);
        return default;
    }

#endregion

#region DoAsync

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    public static async Task DoAsync<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, CancellationToken, Task> leftAsyncAction,
        Func<TRight, CancellationToken, Task> rightAsyncAction, CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasLeft)
            await (leftAsyncAction ?? throw new ArgumentNullException(nameof(leftAsyncAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
        else
            await (rightAsyncAction ?? throw new ArgumentNullException(nameof(rightAsyncAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    public static async Task DoLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TLeft, CancellationToken, Task> leftAsyncAction,
        CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasLeft)
            await (leftAsyncAction ?? throw new ArgumentNullException(nameof(leftAsyncAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    public static async Task DoRightAsync<TLeft, TRight>(this Either<TLeft, TRight> either, Func<TRight, CancellationToken, Task> rightAsyncAction,
        CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasRight)
            await (rightAsyncAction ?? throw new ArgumentNullException(nameof(rightAsyncAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    public static async Task DoAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, Task> leftAsyncAction,
        Func<TRight, CancellationToken, Task> rightAsyncAction, CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (leftAsyncAction ?? throw new ArgumentNullException(nameof(leftAsyncAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
        else
            await (rightAsyncAction ?? throw new ArgumentNullException(nameof(rightAsyncAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    public static async Task DoLeftAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, Task> leftAsyncAction, CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (leftAsyncAction ?? throw new ArgumentNullException(nameof(leftAsyncAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    public static async Task DoRightAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, CancellationToken, Task> rightAsyncAction, CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasRight)
            await (rightAsyncAction ?? throw new ArgumentNullException(nameof(rightAsyncAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    public static async Task DoAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, CancellationToken, Task> leftAsyncAction, Func<TRight, CancellationToken, Task> rightAsyncAction,
        CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (leftAsyncAction ?? throw new ArgumentNullException(nameof(leftAsyncAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
        else
            await (rightAsyncAction ?? throw new ArgumentNullException(nameof(rightAsyncAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    public static async Task DoLeftAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TLeft, CancellationToken, Task> leftAsyncAction, CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (leftAsyncAction ?? throw new ArgumentNullException(nameof(leftAsyncAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    public static async Task DoRightAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        Func<TRight, CancellationToken, Task> rightAsyncAction, CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasRight)
            await (rightAsyncAction ?? throw new ArgumentNullException(nameof(rightAsyncAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

#endregion
}
