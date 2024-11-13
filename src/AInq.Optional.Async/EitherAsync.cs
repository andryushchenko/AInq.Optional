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

using System.Runtime.CompilerServices;

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
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> LeftAsync<TLeft, TRight>(Task<TLeft> leftTask, CancellationToken cancellation = default)
        => (leftTask ?? throw new ArgumentNullException(nameof(leftTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(Either.Left<TLeft, TRight>(leftTask.Result))
            : FromTaskAsync<TLeft, TRight>(leftTask.WaitAsync(cancellation));

    /// <inheritdoc cref="Either.Left{TLeft,TRight}(TLeft)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> LeftAsync<TLeft, TRight>(ValueTask<TLeft> leftValueTask, CancellationToken cancellation = default)
        => leftValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(Either.Left<TLeft, TRight>(leftValueTask.Result))
            : FromTaskAsync<TLeft, TRight>(leftValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Either.Right{TLeft,TRight}(TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> RightAsync<TLeft, TRight>(Task<TRight> rightTask, CancellationToken cancellation = default)
        => (rightTask ?? throw new ArgumentNullException(nameof(rightTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(Either.Right<TLeft, TRight>(rightTask.Result))
            : FromTaskAsync<TLeft, TRight>(rightTask.WaitAsync(cancellation));

    /// <inheritdoc cref="Either.Right{TLeft,TRight}(TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> RightAsync<TLeft, TRight>(ValueTask<TRight> rightValueTask,
        CancellationToken cancellation = default)
        => rightValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(Either.Right<TLeft, TRight>(rightValueTask.Result))
            : FromTaskAsync<TLeft, TRight>(rightValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="LeftAsync{TLeft,TRight}(Task{TLeft},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this Task<TLeft> leftTask, CancellationToken cancellation = default)
        => LeftAsync<TLeft, TRight>(leftTask ?? throw new ArgumentNullException(nameof(leftTask)), cancellation);

    /// <inheritdoc cref="LeftAsync{TLeft,TRight}(ValueTask{TLeft},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this ValueTask<TLeft> leftValueTask,
        CancellationToken cancellation = default)
        => LeftAsync<TLeft, TRight>(leftValueTask, cancellation);

    /// <inheritdoc cref="RightAsync{TLeft,TRight}(Task{TRight},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this Task<TRight> rightTask, CancellationToken cancellation = default)
        => RightAsync<TLeft, TRight>(rightTask ?? throw new ArgumentNullException(nameof(rightTask)), cancellation);

    /// <inheritdoc cref="RightAsync{TLeft,TRight}(ValueTask{TRight},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this ValueTask<TRight> rightValueTask,
        CancellationToken cancellation = default)
        => RightAsync<TLeft, TRight>(rightValueTask, cancellation);

#endregion

#region SelectLeft

    private static async ValueTask<Either<TLeftResult, TRight>> AwaitSelectLeft<TLeft, TRight, TLeftResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TLeftResult> leftSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeft(leftSelector);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeft, TRight, TLeftResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRight>>(
                eitherTask.Result.SelectLeft(leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))))
            : AwaitSelectLeft(eitherTask, leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)), cancellation);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeft, TRight, TLeftResult>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRight>>(
                eitherValueTask.Result.SelectLeft(leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))))
            : AwaitSelectLeft(eitherValueTask.AsTask(), leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)), cancellation);

    private static async ValueTask<Either<TLeftResult, TRight>> AwaitSelectLeft<TLeft, TRight, TLeftResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Either<TLeftResult, TRight>> leftSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeft(leftSelector);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeft, TRight, TLeftResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRight>> leftSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRight>>(
                eitherTask.Result.SelectLeft(leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))))
            : AwaitSelectLeft(eitherTask, leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)), cancellation);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeft<TLeft, TRight, TLeftResult>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRight>> leftSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRight>>(
                eitherValueTask.Result.SelectLeft(leftSelector ?? throw new ArgumentNullException(nameof(leftSelector))))
            : AwaitSelectLeft(eitherValueTask.AsTask(), leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)), cancellation);

#endregion

#region SelectRight

    private static async ValueTask<Either<TLeft, TRightResult>> AwaitSelectRight<TLeft, TRight, TRightResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, TRightResult> rightSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRight(rightSelector);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRight<TLeft, TRight, TRightResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRightResult>>(
                eitherTask.Result.SelectRight(rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelectRight(eitherTask, rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)), cancellation);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRight<TLeft, TRight, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRightResult>>(
                eitherValueTask.Result.SelectRight(rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelectRight(eitherValueTask.AsTask(), rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)), cancellation);

    private static async ValueTask<Either<TLeft, TRightResult>> AwaitSelectRight<TLeft, TRight, TRightResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, Either<TLeft, TRightResult>> rightSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRight(rightSelector);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRight<TLeft, TRight, TRightResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeft, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRightResult>>(
                eitherTask.Result.SelectRight(rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelectRight(eitherTask, rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)), cancellation);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRight<TLeft, TRight, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeft, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRightResult>>(
                eitherValueTask.Result.SelectRight(rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelectRight(eitherValueTask.AsTask(), rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)), cancellation);

#endregion

#region Select

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelect(eitherTask,
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherValueTask.Result.Select(
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelect(eitherValueTask.AsTask(),
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                cancellation);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector, Func<TRight, TRightResult> rightSelector,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelect(eitherTask,
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, TRightResult> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherValueTask.Result.Select(
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelect(eitherValueTask.AsTask(),
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                cancellation);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TLeftResult> leftSelector, Func<TRight, Either<TLeftResult, TRightResult>> rightSelector,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelect(eitherTask,
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, [InstantHandle(RequireAwait = true)] Func<TLeft, TLeftResult> leftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherValueTask.Result.Select(
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelect(eitherValueTask.AsTask(),
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                cancellation);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask, [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherTask.Result.Select(
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelect(eitherTask,
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> Select<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeftResult, TRightResult>>(eitherValueTask.Result.Select(
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector))))
            : AwaitSelect(eitherValueTask.AsTask(),
                leftSelector ?? throw new ArgumentNullException(nameof(leftSelector)),
                rightSelector ?? throw new ArgumentNullException(nameof(rightSelector)),
                cancellation);

#endregion

#region SelectLeftAsync

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)))
              .Invoke(either.Left, cancellation)
              .AsEitherAsync<TLeftResult, TRight>(cancellation)
            : new ValueTask<Either<TLeftResult, TRight>>(Either.Right<TLeftResult, TRight>(either.Right));

    private static async ValueTask<Either<TLeftResult, TRight>> AwaitSelectLeftAsync<TLeft, TRight, TLeftResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeftAsync(asyncLeftSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectLeftAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation)
            : AwaitSelectLeftAsync(eitherTask, asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectLeftAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation)
            : AwaitSelectLeftAsync(eitherValueTask.AsTask(),
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                cancellation);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> asyncLeftSelector,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector))).Invoke(either.Left, cancellation)
            : new ValueTask<Either<TLeftResult, TRight>>(Either.Right<TLeftResult, TRight>(either.Right));

    private static async ValueTask<Either<TLeftResult, TRight>> AwaitSelectLeftAsync<TLeft, TRight, TLeftResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> asyncLeftSelector,
        CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeftAsync(asyncLeftSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> asyncLeftSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectLeftAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation)
            : AwaitSelectLeftAsync(eitherTask, asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation);

    /// <inheritdoc cref="Either.SelectLeft{TLeft,TRight,TLeftResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRight}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRight>> SelectLeftAsync<TLeft, TRight, TLeftResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> asyncLeftSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectLeftAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)), cancellation)
            : AwaitSelectLeftAsync(eitherValueTask.AsTask(),
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                cancellation);

#endregion

#region SelectRightAsync

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? (asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)))
              .Invoke(either.Right, cancellation)
              .AsEitherAsync<TLeft, TRightResult>(cancellation)
            : new ValueTask<Either<TLeft, TRightResult>>(Either.Left<TLeft, TRightResult>(either.Left));

    private static async ValueTask<Either<TLeft, TRightResult>> AwaitSelectRightAsync<TLeft, TRight, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRightAsync(asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectRightAsync(asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation)
            : AwaitSelectRightAsync(eitherTask, asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectRightAsync(asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation)
            : AwaitSelectRightAsync(eitherValueTask.AsTask(),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? (asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector))).Invoke(either.Right, cancellation)
            : new ValueTask<Either<TLeft, TRightResult>>(Either.Left<TLeft, TRightResult>(either.Left));

    private static async ValueTask<Either<TLeft, TRightResult>> AwaitSelectRightAsync<TLeft, TRight, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRightAsync(asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectRightAsync(asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation)
            : AwaitSelectRightAsync(eitherTask, asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation);

    /// <inheritdoc cref="Either.SelectRight{TLeft,TRight,TRightResult}(Either{TLeft,TRight},Func{TRight,Either{TLeft,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRightResult>> SelectRightAsync<TLeft, TRight, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectRightAsync(asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)), cancellation)
            : AwaitSelectRightAsync(eitherValueTask.AsTask(),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

#endregion

#region SelectAsync

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)))
              .Invoke(either.Left, cancellation)
              .AsEitherAsync<TLeftResult, TRightResult>(cancellation)
            : (asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)))
              .Invoke(either.Right, cancellation)
              .AsEitherAsync<TLeftResult, TRightResult>(cancellation);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncLeftSelector, asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation)
            : AwaitSelectAsync(eitherTask,
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation)
            : AwaitSelectAsync(eitherValueTask.AsTask(),
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector))).Invoke(either.Left, cancellation)
            : (asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)))
              .Invoke(either.Right, cancellation)
              .AsEitherAsync<TLeftResult, TRightResult>(cancellation);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncLeftSelector, asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation)
            : AwaitSelectAsync(eitherTask,
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,TRightResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation)
            : AwaitSelectAsync(eitherValueTask.AsTask(),
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)))
              .Invoke(either.Left, cancellation)
              .AsEitherAsync<TLeftResult, TRightResult>(cancellation)
            : (asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector))).Invoke(either.Right, cancellation);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncLeftSelector, asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation)
            : AwaitSelectAsync(eitherTask,
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,TLeftResult},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation)
            : AwaitSelectAsync(eitherValueTask.AsTask(),
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector))).Invoke(either.Left, cancellation)
            : (asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector))).Invoke(either.Right, cancellation);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncLeftSelector, asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation)
            : AwaitSelectAsync(eitherTask,
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

    /// <inheritdoc cref="Either.Select{TLeft,TRight,TLeftResult,TRightResult}(Either{TLeft,TRight},Func{TLeft,Either{TLeftResult,TRightResult}},Func{TRight,Either{TLeftResult,TRightResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeftResult, TRightResult>> SelectAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.SelectAsync(asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation)
            : AwaitSelectAsync(eitherValueTask.AsTask(),
                asyncLeftSelector ?? throw new ArgumentNullException(nameof(asyncLeftSelector)),
                asyncRightSelector ?? throw new ArgumentNullException(nameof(asyncRightSelector)),
                cancellation);

#endregion

#region ValueOrDefault

    private static async ValueTask<TLeft?> AwaitLeftOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault();

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft?> LeftOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TLeft?>(eitherTask.Result.LeftOrDefault())
            : AwaitLeftOrDefault(eitherTask, cancellation);

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft?> LeftOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TLeft?>(eitherValueTask.Result.LeftOrDefault())
            : AwaitLeftOrDefault(eitherValueTask.AsTask(), cancellation);

    private static async ValueTask<TLeft> AwaitLeftOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, TLeft defaultValue,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault(defaultValue);

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> LeftOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, [NoEnumeration] TLeft defaultValue,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TLeft>(eitherTask.Result.LeftOrDefault(defaultValue))
            : AwaitLeftOrDefault(eitherTask, defaultValue, cancellation);

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},TLeft)" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> LeftOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [NoEnumeration] TLeft defaultValue, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TLeft>(eitherValueTask.Result.LeftOrDefault(defaultValue))
            : AwaitLeftOrDefault(eitherValueTask.AsTask(), defaultValue, cancellation);

    private static async ValueTask<TLeft> AwaitLeftOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Func<TLeft> defaultGenerator,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault(defaultGenerator);

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> LeftOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft> defaultGenerator, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TLeft>(eitherTask.Result.LeftOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitLeftOrDefault(eitherTask, defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)), cancellation);

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> LeftOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft> defaultGenerator, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TLeft>(
                eitherValueTask.Result.LeftOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitLeftOrDefault(eitherValueTask.AsTask(),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

    private static async ValueTask<TRight?> AwaitRightOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault();

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight?> RightOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TRight?>(eitherTask.Result.RightOrDefault())
            : AwaitRightOrDefault(eitherTask, cancellation);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight?> RightOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TRight?>(eitherValueTask.Result.RightOrDefault())
            : AwaitRightOrDefault(eitherValueTask.AsTask(), cancellation);

    private static async ValueTask<TRight> AwaitRightOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, TRight defaultValue,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault(defaultValue);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> RightOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, [NoEnumeration] TRight defaultValue,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TRight>(eitherTask.Result.RightOrDefault(defaultValue))
            : AwaitRightOrDefault(eitherTask, defaultValue, cancellation);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> RightOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [NoEnumeration] TRight defaultValue, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TRight>(eitherValueTask.Result.RightOrDefault(defaultValue))
            : AwaitRightOrDefault(eitherValueTask.AsTask(), defaultValue, cancellation);

    private static async ValueTask<TRight> AwaitRightOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Func<TRight> defaultGenerator,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault(defaultGenerator);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> RightOrDefault<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight> defaultGenerator, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TRight>(eitherTask.Result.RightOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitRightOrDefault(eitherTask, defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)), cancellation);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> RightOrDefault<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight> defaultGenerator, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TRight>(
                eitherValueTask.Result.RightOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitRightOrDefault(eitherValueTask.AsTask(),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

#endregion

#region ValueOrDefaultAsync

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TLeft>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? new ValueTask<TLeft>(either.Left)
            : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

    private static async ValueTask<TLeft> AwaitLeftOrDefaultAsync<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        Func<CancellationToken, ValueTask<TLeft>> asyncDefaultGenerator, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TLeft>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.LeftOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitLeftOrDefaultAsync(eitherTask,
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Either.LeftOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> LeftOrDefaultAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TLeft>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.LeftOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitLeftOrDefaultAsync(eitherValueTask.AsTask(),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? new ValueTask<TRight>(either.Right)
            : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

    private static async ValueTask<TRight> AwaitRightOrDefaultAsync<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        Func<CancellationToken, ValueTask<TRight>> asyncDefaultGenerator, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.RightOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitRightOrDefaultAsync(eitherTask,
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Either.RightOrDefault{TLeft,TRight}(Either{TLeft,TRight},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> RightOrDefaultAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.RightOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitRightOrDefaultAsync(eitherValueTask.AsTask(),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

#endregion

#region ToValue

    private static async ValueTask<TLeft> AwaitToLeft<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Func<TRight, TLeft> rightToLeft,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToLeft(rightToLeft);

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> ToLeft<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, TLeft> rightToLeft, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TLeft>(eitherTask.Result.ToLeft(rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft))))
            : AwaitToLeft(eitherTask, rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft)), cancellation);

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> ToLeft<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, TLeft> rightToLeft, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TLeft>(eitherValueTask.Result.ToLeft(rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft))))
            : AwaitToLeft(eitherValueTask.AsTask(), rightToLeft ?? throw new ArgumentNullException(nameof(rightToLeft)), cancellation);

    private static async ValueTask<TRight> AwaitToRight<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TRight> leftToRight,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToRight(leftToRight);

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> ToRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TRight> leftToRight, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TRight>(eitherTask.Result.ToRight(leftToRight ?? throw new ArgumentNullException(nameof(leftToRight))))
            : AwaitToRight(eitherTask, leftToRight ?? throw new ArgumentNullException(nameof(leftToRight)), cancellation);

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> ToRight<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TRight> leftToRight, CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TRight>(eitherValueTask.Result.ToRight(leftToRight ?? throw new ArgumentNullException(nameof(leftToRight))))
            : AwaitToRight(eitherValueTask.AsTask(), leftToRight ?? throw new ArgumentNullException(nameof(leftToRight)), cancellation);

    private static async ValueTask<TResult> AwaitToValue<TLeft, TRight, TResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TResult> fromLeft, Func<TRight, TResult> fromRight, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToValue(fromLeft, fromRight);

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> ToValue<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TResult> fromLeft, [InstantHandle(RequireAwait = true)] Func<TRight, TResult> fromRight,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult>(eitherTask.Result.ToValue(fromLeft ?? throw new ArgumentNullException(nameof(fromLeft)),
                fromRight ?? throw new ArgumentNullException(nameof(fromRight))))
            : AwaitToValue(eitherTask,
                fromLeft ?? throw new ArgumentNullException(nameof(fromLeft)),
                fromRight ?? throw new ArgumentNullException(nameof(fromRight)),
                cancellation);

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> ToValue<TLeft, TRight, TResult>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TResult> fromLeft, [InstantHandle(RequireAwait = true)] Func<TRight, TResult> fromRight,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult>(eitherValueTask.Result.ToValue(fromLeft ?? throw new ArgumentNullException(nameof(fromLeft)),
                fromRight ?? throw new ArgumentNullException(nameof(fromRight))))
            : AwaitToValue(eitherValueTask.AsTask(),
                fromLeft ?? throw new ArgumentNullException(nameof(fromLeft)),
                fromRight ?? throw new ArgumentNullException(nameof(fromRight)),
                cancellation);

#endregion

#region ToValueAsync

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> ToLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TLeft>> asyncRightToLeft,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? new ValueTask<TLeft>(either.Left)
            : (asyncRightToLeft ?? throw new ArgumentNullException(nameof(asyncRightToLeft))).Invoke(either.Right, cancellation);

    private static async ValueTask<TLeft> AwaitToLeftAsync<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, CancellationToken, ValueTask<TLeft>> asyncRightToLeft, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToLeftAsync(asyncRightToLeft, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> ToLeftAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TLeft>> asyncRightToLeft,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.ToLeftAsync(asyncRightToLeft ?? throw new ArgumentNullException(nameof(asyncRightToLeft)), cancellation)
            : AwaitToLeftAsync(eitherTask, asyncRightToLeft ?? throw new ArgumentNullException(nameof(asyncRightToLeft)), cancellation);

    /// <inheritdoc cref="Either.ToLeft{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,TLeft})" />
    [PublicAPI, Pure]
    public static ValueTask<TLeft> ToLeftAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TLeft>> asyncRightToLeft,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.ToLeftAsync(asyncRightToLeft ?? throw new ArgumentNullException(nameof(asyncRightToLeft)), cancellation)
            : AwaitToLeftAsync(eitherValueTask.AsTask(), asyncRightToLeft ?? throw new ArgumentNullException(nameof(asyncRightToLeft)), cancellation);

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> ToRightAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TRight>> asyncLeftToRight,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
            ? new ValueTask<TRight>(either.Right)
            : (asyncLeftToRight ?? throw new ArgumentNullException(nameof(asyncLeftToRight))).Invoke(either.Left, cancellation);

    private static async ValueTask<TRight> AwaitToRightAsync<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<TRight>> asyncLeftToRight, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToRightAsync(asyncLeftToRight, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> ToRightAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TRight>> asyncLeftToRight,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.ToRightAsync(asyncLeftToRight ?? throw new ArgumentNullException(nameof(asyncLeftToRight)), cancellation)
            : AwaitToRightAsync(eitherTask, asyncLeftToRight ?? throw new ArgumentNullException(nameof(asyncLeftToRight)), cancellation);

    /// <inheritdoc cref="Either.ToRight{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<TRight> ToRightAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TRight>> asyncLeftToRight,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.ToRightAsync(asyncLeftToRight ?? throw new ArgumentNullException(nameof(asyncLeftToRight)), cancellation)
            : AwaitToRightAsync(eitherValueTask.AsTask(),
                asyncLeftToRight ?? throw new ArgumentNullException(nameof(asyncLeftToRight)),
                cancellation);

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> ToValueAsync<TLeft, TRight, TResult>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TResult>> asyncFromLeft,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TResult>> asyncFromRight,
        CancellationToken cancellation = default)
        => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
            ? (asyncFromLeft ?? throw new ArgumentNullException(nameof(asyncFromLeft))).Invoke(either.Left, cancellation)
            : (asyncFromRight ?? throw new ArgumentNullException(nameof(asyncFromRight))).Invoke(either.Right, cancellation);

    private static async ValueTask<TResult> AwaitToValueAsync<TLeft, TRight, TResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<TResult>> asyncFromLeft, Func<TRight, CancellationToken, ValueTask<TResult>> asyncFromRight,
        CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToValueAsync(asyncFromLeft, asyncFromRight, cancellation)
                                                                                 .ConfigureAwait(false);

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> ToValueAsync<TLeft, TRight, TResult>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TResult>> asyncFromLeft,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TResult>> asyncFromRight,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result.ToValueAsync(asyncFromLeft ?? throw new ArgumentNullException(nameof(asyncFromLeft)),
                asyncFromRight ?? throw new ArgumentNullException(nameof(asyncFromRight)),
                cancellation)
            : AwaitToValueAsync(eitherTask,
                asyncFromLeft ?? throw new ArgumentNullException(nameof(asyncFromLeft)),
                asyncFromRight ?? throw new ArgumentNullException(nameof(asyncFromRight)),
                cancellation);

    /// <inheritdoc cref="Either.ToValue{TLeft,TRight,TResult}(Either{TLeft,TRight},Func{TLeft,TResult},Func{TRight,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> ToValueAsync<TLeft, TRight, TResult>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, ValueTask<TResult>> asyncFromLeft,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, ValueTask<TResult>> asyncFromRight,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result.ToValueAsync(asyncFromLeft ?? throw new ArgumentNullException(nameof(asyncFromLeft)),
                asyncFromRight ?? throw new ArgumentNullException(nameof(asyncFromRight)),
                cancellation)
            : AwaitToValueAsync(eitherValueTask.AsTask(),
                asyncFromLeft ?? throw new ArgumentNullException(nameof(asyncFromLeft)),
                asyncFromRight ?? throw new ArgumentNullException(nameof(asyncFromRight)),
                cancellation);

#endregion

#region Utils

    private static async ValueTask<Either<TRight, TLeft>> AwaitInvert<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Invert();

    /// <inheritdoc cref="Either.Invert{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TRight, TLeft>> Invert<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TRight, TLeft>>(eitherTask.Result.Invert())
            : AwaitInvert(eitherTask, cancellation);

    /// <inheritdoc cref="Either.Invert{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TRight, TLeft>> Invert<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TRight, TLeft>>(eitherValueTask.Result.Invert())
            : AwaitInvert(eitherValueTask.AsTask(), cancellation);

#endregion

#region Linq

    /// <inheritdoc cref="Either.LeftValues{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}})" />
    [PublicAPI]
    public static async IAsyncEnumerable<TLeft> LeftValues<TLeft, TRight>(this IAsyncEnumerable<Either<TLeft, TRight>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var either in collection.WithCancellation(cancellation).ConfigureAwait(false))
            if (either is {HasLeft: true})
                yield return either.Left;
    }

    /// <inheritdoc cref="Either.RightValues{TLeft,TRight}(IEnumerable{Either{TLeft,TRight}})" />
    [PublicAPI]
    public static async IAsyncEnumerable<TRight> RightValues<TLeft, TRight>(this IAsyncEnumerable<Either<TLeft, TRight>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var either in collection.WithCancellation(cancellation).ConfigureAwait(false))
            if (either is {HasRight: true})
                yield return either.Right;
    }

#endregion

#region Do

    private static async ValueTask AwaitDo<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Action<TLeft> leftAction,
        Action<TRight> rightAction, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(leftAction, rightAction);

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    [PublicAPI]
    public static ValueTask Do<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Action<TLeft> leftAction, [InstantHandle(RequireAwait = true)] Action<TRight> rightAction,
        CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDo(eitherTask,
                leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                cancellation);
        eitherTask.Result.Do(leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
            rightAction ?? throw new ArgumentNullException(nameof(rightAction)));
        return default;
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    [PublicAPI]
    public static ValueTask Do<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Action<TLeft> leftAction, [InstantHandle(RequireAwait = true)] Action<TRight> rightAction,
        CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return AwaitDo(eitherValueTask.AsTask(),
                leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                cancellation);
        eitherValueTask.Result.Do(leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
            rightAction ?? throw new ArgumentNullException(nameof(rightAction)));
        return default;
    }

    private static async ValueTask AwaitDoLeft<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Action<TLeft> leftAction,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoLeft(leftAction);

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    [PublicAPI]
    public static ValueTask DoLeft<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Action<TLeft> leftAction, CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDoLeft(eitherTask, leftAction ?? throw new ArgumentNullException(nameof(leftAction)), cancellation);
        eitherTask.Result.DoLeft(leftAction ?? throw new ArgumentNullException(nameof(leftAction)));
        return default;
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    [PublicAPI]
    public static ValueTask DoLeft<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Action<TLeft> leftAction, CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return AwaitDoLeft(eitherValueTask.AsTask(), leftAction ?? throw new ArgumentNullException(nameof(leftAction)), cancellation);
        eitherValueTask.Result.DoLeft(leftAction ?? throw new ArgumentNullException(nameof(leftAction)));
        return default;
    }

    private static async ValueTask AwaitDoRight<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Action<TRight> rightAction,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoRight(rightAction);

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    [PublicAPI]
    public static ValueTask DoRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Action<TRight> rightAction, CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDoRight(eitherTask, rightAction ?? throw new ArgumentNullException(nameof(rightAction)), cancellation);
        eitherTask.Result.DoRight(rightAction ?? throw new ArgumentNullException(nameof(rightAction)));
        return default;
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    [PublicAPI]
    public static ValueTask DoRight<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Action<TRight> rightAction, CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return AwaitDoRight(eitherValueTask.AsTask(), rightAction ?? throw new ArgumentNullException(nameof(rightAction)), cancellation);
        eitherValueTask.Result.DoRight(rightAction ?? throw new ArgumentNullException(nameof(rightAction)));
        return default;
    }

#endregion

#region DoWithArgument

    private static async ValueTask AwaitDo<TLeft, TRight, TArgument>(Task<Either<TLeft, TRight>> eitherTask, Action<TLeft, TArgument> leftAction,
        Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(leftAction, rightAction, argument);

    /// <inheritdoc cref="Either.Do{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask Do<TLeft, TRight, TArgument>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Action<TLeft, TArgument> leftAction,
        [InstantHandle(RequireAwait = true)] Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDo(eitherTask,
                leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                argument,
                cancellation);
        eitherTask.Result.Do(leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
            rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
            argument);
        return default;
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask Do<TLeft, TRight, TArgument>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Action<TLeft, TArgument> leftAction,
        [InstantHandle(RequireAwait = true)] Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return AwaitDo(eitherValueTask.AsTask(),
                leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                argument,
                cancellation);
        eitherValueTask.Result.Do(leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
            rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
            argument);
        return default;
    }

    private static async ValueTask AwaitDoLeft<TLeft, TRight, TArgument>(Task<Either<TLeft, TRight>> eitherTask, Action<TLeft, TArgument> leftAction,
        TArgument argument, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoLeft(leftAction, argument);

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask DoLeft<TLeft, TRight, TArgument>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Action<TLeft, TArgument> leftAction, TArgument argument, CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDoLeft(eitherTask,
                leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                argument,
                cancellation);
        eitherTask.Result.DoLeft(leftAction ?? throw new ArgumentNullException(nameof(leftAction)), argument);
        return default;
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask DoLeft<TLeft, TRight, TArgument>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Action<TLeft, TArgument> leftAction, TArgument argument, CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return AwaitDoLeft(eitherValueTask.AsTask(),
                leftAction ?? throw new ArgumentNullException(nameof(leftAction)),
                argument,
                cancellation);
        eitherValueTask.Result.DoLeft(leftAction ?? throw new ArgumentNullException(nameof(leftAction)), argument);
        return default;
    }

    private static async ValueTask AwaitDoRight<TLeft, TRight, TArgument>(Task<Either<TLeft, TRight>> eitherTask,
        Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoRight(rightAction, argument);

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask DoRight<TLeft, TRight, TArgument>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation = default)
    {
        if ((eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDoRight(eitherTask,
                rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                argument,
                cancellation);
        eitherTask.Result.DoRight(rightAction ?? throw new ArgumentNullException(nameof(rightAction)), argument);
        return default;
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask DoRight<TLeft, TRight, TArgument>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation = default)
    {
        if (!eitherValueTask.IsCompletedSuccessfully)
            return AwaitDoRight(eitherValueTask.AsTask(),
                rightAction ?? throw new ArgumentNullException(nameof(rightAction)),
                argument,
                cancellation);
        eitherValueTask.Result.DoRight(rightAction ?? throw new ArgumentNullException(nameof(rightAction)), argument);
        return default;
    }

#endregion

#region DoAsync

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    [PublicAPI]
    public static async Task DoAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction, CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    [PublicAPI]
    public static async Task DoLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction, CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    [PublicAPI]
    public static async Task DoRightAsync<TLeft, TRight>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction, CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasRight)
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    [PublicAPI]
    public static async Task DoAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction, CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    [PublicAPI]
    public static async Task DoLeftAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction, CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    [PublicAPI]
    public static async Task DoRightAsync<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction, CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasRight)
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft},Action{TRight})" />
    [PublicAPI]
    public static async Task DoAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction, CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight}(Either{TLeft,TRight},Action{TLeft})" />
    [PublicAPI]
    public static async Task DoLeftAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, CancellationToken, Task> asyncLeftAction, CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight}(Either{TLeft,TRight},Action{TRight})" />
    [PublicAPI]
    public static async Task DoRightAsync<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, CancellationToken, Task> asyncRightAction, CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasRight)
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, cancellation)
                  .ConfigureAwait(false);
    }

#endregion

#region DoAsyncWithArgument

    /// <inheritdoc cref="Either.Do{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<TLeft, TRight, TArgument>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction,
        [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, argument, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoLeftAsync<TLeft, TRight, TArgument>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoRightAsync<TLeft, TRight, TArgument>(this Either<TLeft, TRight> either,
        [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        if ((either ?? throw new ArgumentNullException(nameof(either))).HasRight)
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<TLeft, TRight, TArgument>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction,
        [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, argument, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoLeftAsync<TLeft, TRight, TArgument>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoRightAsync<TLeft, TRight, TArgument>(this Task<Either<TLeft, TRight>> eitherTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var either = (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? eitherTask.Result
            : await eitherTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasRight)
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.Do{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<TLeft, TRight, TArgument>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction,
        [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, argument, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoLeft{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TLeft,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoLeftAsync<TLeft, TRight, TArgument>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TLeft, TArgument, CancellationToken, Task> asyncLeftAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasLeft)
            await (asyncLeftAction ?? throw new ArgumentNullException(nameof(asyncLeftAction)))
                  .Invoke(either.Left, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Either.DoRight{TLeft,TRight,TArgument}(Either{TLeft,TRight},Action{TRight,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoRightAsync<TLeft, TRight, TArgument>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight, TArgument, CancellationToken, Task> asyncRightAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var either = eitherValueTask.IsCompletedSuccessfully
            ? eitherValueTask.Result
            : await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (either.HasRight)
            await (asyncRightAction ?? throw new ArgumentNullException(nameof(asyncRightAction)))
                  .Invoke(either.Right, argument, cancellation)
                  .ConfigureAwait(false);
    }

#endregion
}
