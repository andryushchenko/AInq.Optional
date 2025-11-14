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

/// <summary> Maybe async extension </summary>
public static partial class MaybeAsync
{
    /// <inheritdoc cref="Maybe.Value{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueAsync<T>(Task<T> task, CancellationToken cancellation = default)
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result))
            : AwaitValue(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.Value{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueAsync<T>(ValueTask<T> valueTask, CancellationToken cancellation = default)
        => valueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result))
            : AwaitValue(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> task, CancellationToken cancellation = default)
        where T : class
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? task.Result is not null ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result)) : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : AwaitValueIfNotNull(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : class
        => valueTask.IsCompletedSuccessfully
            ? valueTask.Result is not null ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result)) : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : AwaitValueIfNotNull(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> task, CancellationToken cancellation = default)
        where T : struct
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? task.Result.HasValue ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result.Value)) : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : AwaitValueIfNotNull(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : struct
        => valueTask.IsCompletedSuccessfully
            ? valueTask.Result.HasValue ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result.Value)) : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : AwaitValueIfNotNull(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="ValueAsync{T}(Task{T},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeAsync<T>(this Task<T> task, CancellationToken cancellation = default)
        => ValueAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueAsync{T}(ValueTask{T},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeAsync<T>(this ValueTask<T> valueTask, CancellationToken cancellation = default)
        => ValueAsync(valueTask, cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> task, CancellationToken cancellation = default)
        where T : class
        => ValueIfNotNullAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(ValueTask{T?},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : class
        => ValueIfNotNullAsync(valueTask, cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> task, CancellationToken cancellation = default)
        where T : struct
        => ValueIfNotNullAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(ValueTask{T?},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : struct
        => ValueIfNotNullAsync(valueTask, cancellation);

    /// <inheritdoc cref="Maybe.Unwrap{T}(Maybe{Maybe{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Unwrap<T>(this Task<Maybe<Maybe<T>>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Unwrap())
            : AwaitUnwrap(maybeTask, cancellation);

    /// <inheritdoc cref="Maybe.Unwrap{T}(Maybe{Maybe{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Unwrap<T>(this ValueTask<Maybe<Maybe<T>>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Unwrap())
            : AwaitUnwrap(maybeValueTask.AsTask(), cancellation);
}
