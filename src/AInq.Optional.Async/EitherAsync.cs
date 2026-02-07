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

/// <summary> Either async extension </summary>
public static partial class EitherAsync
{
    /// <inheritdoc cref="Either.Left{TLeft,TRight}(TLeft)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> LeftAsync<TLeft, TRight>(Task<TLeft> leftTask, CancellationToken cancellation = default)
    {
        _ = leftTask ?? throw new ArgumentNullException(nameof(leftTask));
        return leftTask.Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(Either.Left<TLeft, TRight>(leftTask.Result))
            : AwaitLeft<TLeft, TRight>(leftTask.WaitAsync(cancellation));
    }

    /// <inheritdoc cref="Either.Left{TLeft,TRight}(TLeft)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> LeftAsync<TLeft, TRight>(ValueTask<TLeft> leftValueTask, CancellationToken cancellation = default)
        => leftValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(Either.Left<TLeft, TRight>(leftValueTask.Result))
            : AwaitLeft<TLeft, TRight>(leftValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Either.Right{TLeft,TRight}(TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> RightAsync<TLeft, TRight>(Task<TRight> rightTask, CancellationToken cancellation = default)
    {
        _ = rightTask ?? throw new ArgumentNullException(nameof(rightTask));
        return rightTask.Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(Either.Right<TLeft, TRight>(rightTask.Result))
            : AwaitRight<TLeft, TRight>(rightTask.WaitAsync(cancellation));
    }

    /// <inheritdoc cref="Either.Right{TLeft,TRight}(TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> RightAsync<TLeft, TRight>(ValueTask<TRight> rightValueTask,
        CancellationToken cancellation = default)
        => rightValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(Either.Right<TLeft, TRight>(rightValueTask.Result))
            : AwaitRight<TLeft, TRight>(rightValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="LeftAsync{TLeft,TRight}(Task{TLeft},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this Task<TLeft> leftTask, CancellationToken cancellation = default)
    {
        _ = leftTask ?? throw new ArgumentNullException(nameof(leftTask));
        return LeftAsync<TLeft, TRight>(leftTask, cancellation);
    }

    /// <inheritdoc cref="LeftAsync{TLeft,TRight}(ValueTask{TLeft},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this ValueTask<TLeft> leftValueTask,
        CancellationToken cancellation = default)
        => LeftAsync<TLeft, TRight>(leftValueTask, cancellation);

    /// <inheritdoc cref="RightAsync{TLeft,TRight}(Task{TRight},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this Task<TRight> rightTask, CancellationToken cancellation = default)
    {
        _ = rightTask ?? throw new ArgumentNullException(nameof(rightTask));
        return RightAsync<TLeft, TRight>(rightTask, cancellation);
    }

    /// <inheritdoc cref="RightAsync{TLeft,TRight}(ValueTask{TRight},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> AsEitherAsync<TLeft, TRight>(this ValueTask<TRight> rightValueTask,
        CancellationToken cancellation = default)
        => RightAsync<TLeft, TRight>(rightValueTask, cancellation);

    /// <inheritdoc cref="Either.Invert{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TRight, TLeft>> Invert<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
    {
        _ = eitherTask ?? throw new ArgumentNullException(nameof(eitherTask));
        return eitherTask.Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TRight, TLeft>>(eitherTask.Result.Invert())
            : AwaitInvert(eitherTask, cancellation);
    }

    /// <inheritdoc cref="Either.Invert{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TRight, TLeft>> Invert<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TRight, TLeft>>(eitherValueTask.Result.Invert())
            : AwaitInvert(eitherValueTask.AsTask(), cancellation);
}
