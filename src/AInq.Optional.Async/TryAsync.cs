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

/// <summary> Try async extension </summary>
public static partial class TryAsync
{
    /// <inheritdoc cref="Try.Result{T}(Func{T})" />
    [PublicAPI, Pure]
    public static async ValueTask<Try<T>> ResultAsync<T>(Task<T> task, CancellationToken cancellation = default)
    {
        _ = task ?? throw new ArgumentNullException(nameof(task));
        try
        {
            return Try.Value(await task.WaitAsync(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="Try.Result{T}(Func{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> ResultAsync<T>(ValueTask<T> valueTask, CancellationToken cancellation = default)
        => valueTask.IsCompletedSuccessfully ? new ValueTask<Try<T>>(Try.Value(valueTask.Result)) : ResultAsync(valueTask.AsTask(), cancellation);

    /// <inheritdoc cref="ResultAsync{T}(Task{T},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> AsTryAsync<T>(this Task<T> task, CancellationToken cancellation = default)
        => ResultAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ResultAsync{T}(ValueTask{T},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> AsTryAsync<T>(this ValueTask<T> valueTask, CancellationToken cancellation = default)
        => ResultAsync(valueTask, cancellation);

    /// <inheritdoc cref="Try.Unwrap{T}(Try{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> Unwrap<T>(this Task<Try<Try<T>>> tryTask, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Unwrap())
            : AwaitUnwrap(tryTask, cancellation);

    /// <inheritdoc cref="Try.Unwrap{T}(Try{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> Unwrap<T>(this ValueTask<Try<Try<T>>> tryValueTask, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Unwrap())
            : AwaitUnwrap(tryValueTask.AsTask(), cancellation);
}
