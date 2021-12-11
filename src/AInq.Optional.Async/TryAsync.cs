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
public static class TryAsync
{
#region Result

    /// <summary> Create Try from async value generator </summary>
    /// <param name="generator"> Value generator </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <param name="throwIfCanceled"> Throw exception if cancelled </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Try<T>> ResultAsync<T>(Task<T> generator, CancellationToken cancellation = default, bool throwIfCanceled = true)
    {
        Try<T> result;
        try
        {
            result = new Try<T>(await generator.WaitAsync(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            result = new Try<T>(ex);
        }
        if (throwIfCanceled) result.Throw<OperationCanceledException>();
        return result;
    }

    /// <inheritdoc cref="ResultAsync{T}(Task{T},CancellationToken,bool)" />
    public static async ValueTask<Try<T>> ResultAsync<T>(ValueTask<T> generator, CancellationToken cancellation = default,
        bool throwIfCanceled = true)
    {
        Try<T> result;
        try
        {
            result = new Try<T>(await generator.WaitAsync(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            result = new Try<T>(ex);
        }
        if (throwIfCanceled) result.Throw<OperationCanceledException>();
        return result;
    }

    /// <summary> Create Try from async value generator if not null </summary>
    /// <param name="generator"> Value generator </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <param name="throwIfCanceled"> Throw exception if cancelled </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Try<T>> ResultIfNotNullAsync<T>(Task<T?> generator, CancellationToken cancellation = default,
        bool throwIfCanceled = true)
        where T : class

    {
        Try<T> result;
        try
        {
            var value = await generator.WaitAsync(cancellation).ConfigureAwait(false);
            result = value == null ? new Try<T>(new ArgumentNullException(nameof(value))) : new Try<T>(value);
        }
        catch (Exception ex)
        {
            result = new Try<T>(ex);
        }
        if (throwIfCanceled) result.Throw<OperationCanceledException>();
        return result;
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T?},CancellationToken,bool)" />
    public static async ValueTask<Try<T>> ResultIfNotNullAsync<T>(ValueTask<T?> generator, CancellationToken cancellation = default,
        bool throwIfCanceled = true)
        where T : class

    {
        Try<T> result;
        try
        {
            var value = await generator.WaitAsync(cancellation).ConfigureAwait(false);
            result = value == null ? new Try<T>(new ArgumentNullException(nameof(value))) : new Try<T>(value);
        }
        catch (Exception ex)
        {
            result = new Try<T>(ex);
        }
        if (throwIfCanceled) result.Throw<OperationCanceledException>();
        return result;
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T?},CancellationToken,bool)" />
    public static async ValueTask<Try<T>> ResultIfNotNullAsync<T>(Task<T?> generator, CancellationToken cancellation = default,
        bool throwIfCanceled = true)
        where T : struct

    {
        Try<T> result;
        try
        {
            var value = await generator.WaitAsync(cancellation).ConfigureAwait(false);
            result = value.HasValue ? new Try<T>(value.Value) : new Try<T>(new ArgumentNullException(nameof(value)));
        }
        catch (Exception ex)
        {
            result = new Try<T>(ex);
        }
        if (throwIfCanceled) result.Throw<OperationCanceledException>();
        return result;
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T?},CancellationToken,bool)" />
    public static async ValueTask<Try<T>> ResultIfNotNullAsync<T>(ValueTask<T?> generator, CancellationToken cancellation = default,
        bool throwIfCanceled = true)
        where T : struct

    {
        Try<T> result;
        try
        {
            var value = await generator.WaitAsync(cancellation).ConfigureAwait(false);
            result = value.HasValue ? new Try<T>(value.Value) : new Try<T>(new ArgumentNullException(nameof(value)));
        }
        catch (Exception ex)
        {
            result = new Try<T>(ex);
        }
        if (throwIfCanceled) result.Throw<OperationCanceledException>();
        return result;
    }

#endregion
}
