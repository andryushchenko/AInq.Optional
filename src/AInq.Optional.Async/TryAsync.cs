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
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Try<T>> ResultAsync<T>(Task<T> generator, CancellationToken cancellation = default)
    {
        try
        {
            return Try.Value(await (generator ?? throw new ArgumentNullException(nameof(generator))).WaitAsync(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> ResultAsync<T>(ValueTask<T> generator, CancellationToken cancellation = default)
    {
        try
        {
            return Try.Value(await generator.WaitAsync(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> AsTryAsync<T>(this Task<T> generator, CancellationToken cancellation = default)
    {
        try
        {
            return Try.Value(await (generator ?? throw new ArgumentNullException(nameof(generator))).WaitAsync(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> AsTryAsync<T>(this ValueTask<T> generator, CancellationToken cancellation = default)
    {
        try
        {
            return Try.Value(await generator.WaitAsync(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <summary> Create Try from async value generator if not null </summary>
    /// <param name="generator"> Value generator </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Try<T>> ResultIfNotNullAsync<T>(Task<T?> generator, CancellationToken cancellation = default)
        where T : class
    {
        try
        {
            var value = await (generator ?? throw new ArgumentNullException(nameof(generator))).WaitAsync(cancellation).ConfigureAwait(false);
            return value == null ? Try.Error<T>(new ArgumentException("No value specified")) : Try.Value(value);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> ResultIfNotNullAsync<T>(ValueTask<T?> generator, CancellationToken cancellation = default)
        where T : class
    {
        try
        {
            var value = await generator.WaitAsync(cancellation).ConfigureAwait(false);
            return value == null ? Try.Error<T>(new ArgumentException("No value specified")) : Try.Value(value);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> ResultIfNotNullAsync<T>(Task<T?> generator, CancellationToken cancellation = default)
        where T : struct
    {
        try
        {
            var value = await (generator ?? throw new ArgumentNullException(nameof(generator))).WaitAsync(cancellation).ConfigureAwait(false);
            return value.HasValue ? Try.Value(value.Value) : Try.Error<T>(new ArgumentException("No value specified"));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> ResultIfNotNullAsync<T>(ValueTask<T?> generator, CancellationToken cancellation = default)
        where T : struct
    {
        try
        {
            var value = await generator.WaitAsync(cancellation).ConfigureAwait(false);
            return value.HasValue ? Try.Value(value.Value) : Try.Error<T>(new ArgumentException("No value specified"));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> AsTryIfNotNullAsync<T>(this Task<T?> generator, CancellationToken cancellation = default)
        where T : class
    {
        try
        {
            var value = await (generator ?? throw new ArgumentNullException(nameof(generator))).WaitAsync(cancellation).ConfigureAwait(false);
            return value == null ? Try.Error<T>(new ArgumentException("No value specified")) : Try.Value(value);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> AsTryIfNotNullAsync<T>(this ValueTask<T?> generator, CancellationToken cancellation = default)
        where T : class
    {
        try
        {
            var value = await generator.WaitAsync(cancellation).ConfigureAwait(false);
            return value == null ? Try.Error<T>(new ArgumentException("No value specified")) : Try.Value(value);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> AsTryIfNotNullAsync<T>(this Task<T?> generator, CancellationToken cancellation = default)
        where T : struct
    {
        try
        {
            var value = await (generator ?? throw new ArgumentNullException(nameof(generator))).WaitAsync(cancellation).ConfigureAwait(false);
            return value.HasValue ? Try.Value(value.Value) : Try.Error<T>(new ArgumentException("No value specified"));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="ResultIfNotNullAsync{T}(Task{T},CancellationToken)" />
    public static async ValueTask<Try<T>> AsTryIfNotNullAsync<T>(this ValueTask<T?> generator, CancellationToken cancellation = default)
        where T : struct
    {
        try
        {
            var value = await generator.WaitAsync(cancellation).ConfigureAwait(false);
            return value.HasValue ? Try.Value(value.Value) : Try.Error<T>(new ArgumentException("No value specified"));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

#endregion

#region Result_Helper

    private static async ValueTask<Try<T>> ResultAsync<T>(Func<CancellationToken, Task<T>> generator, CancellationToken cancellation = default)
    {
        try
        {
            return Try.Value(await (generator ?? throw new ArgumentNullException(nameof(generator))).Invoke(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    private static async ValueTask<Try<T>> ResultAsync<T>(Func<CancellationToken, Task<Try<T>>> generator, CancellationToken cancellation = default)
    {
        try
        {
            return await (generator ?? throw new ArgumentNullException(nameof(generator))).Invoke(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    private static async ValueTask<Try<T>> ResultAsync<T>(Func<CancellationToken, ValueTask<T>> generator, CancellationToken cancellation = default)
    {
        try
        {
            return Try.Value(await (generator ?? throw new ArgumentNullException(nameof(generator))).Invoke(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    private static async ValueTask<Try<T>> ResultAsync<T>(Func<CancellationToken, ValueTask<Try<T>>> generator,
        CancellationToken cancellation = default)
    {
        try
        {
            return await (generator ?? throw new ArgumentNullException(nameof(generator))).Invoke(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

#endregion

#region Select

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> Select<T, TResult>(this Task<Try<T>> @try, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (await ResultAsync(@try, cancellation).ConfigureAwait(false)).Unwrap().Select(selector);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> Select<T, TResult>(this ValueTask<Try<T>> @try, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (await ResultAsync(@try, cancellation).ConfigureAwait(false)).Unwrap().Select(selector);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> Select<T, TResult>(this Task<Try<T>> @try, Func<T, Try<TResult>> selector,
        CancellationToken cancellation = default)
        => (await ResultAsync(@try, cancellation).ConfigureAwait(false)).Unwrap().Select(selector);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> Select<T, TResult>(this ValueTask<Try<T>> @try, Func<T, Try<TResult>> selector,
        CancellationToken cancellation = default)
        => (await ResultAsync(@try, cancellation).ConfigureAwait(false)).Unwrap().Select(selector);

#endregion

#region SelectAsync

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, Task<TResult>> selector)
        => @try.Success
            ? ResultAsync(_ => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value))
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, ValueTask<TResult>> selector)
        => @try.Success
            ? ResultAsync(_ => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value))
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <summary> Convert to other value type asynchronously </summary>
    /// <param name="try"> Try item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellation = default)
        => @try.Success
            ? ResultAsync(cancel => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value, cancel), cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, CancellationToken, ValueTask<TResult>> selector,
        CancellationToken cancellation = default)
        => @try.Success
            ? ResultAsync(cancel => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value, cancel), cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> @try, Func<T, Task<TResult>> selector)
    {
        var result = (await @try.AsTryAsync().ConfigureAwait(false)).Unwrap();
        return result.Success
            ? await ResultAsync(_ => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value)).ConfigureAwait(false)
            : Try.Error<TResult>(result.Error!);
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> @try, Func<T, ValueTask<TResult>> selector)
    {
        var result = (await @try.AsTryAsync().ConfigureAwait(false)).Unwrap();
        return result.Success
            ? await ResultAsync(_ => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value)).ConfigureAwait(false)
            : Try.Error<TResult>(result.Error!);
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> @try, Func<T, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellation = default)
    {
        var result = (await @try.AsTryAsync(cancellation).ConfigureAwait(false)).Unwrap();
        return result.Success
            ? await ResultAsync(cancel => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancel), cancellation)
                .ConfigureAwait(false)
            : Try.Error<TResult>(result.Error!);
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> @try,
        Func<T, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellation = default)
    {
        var result = (await @try.AsTryAsync(cancellation).ConfigureAwait(false)).Unwrap();
        return result.Success
            ? await ResultAsync(cancel => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancel), cancellation)
                .ConfigureAwait(false)
            : Try.Error<TResult>(result.Error!);
    }

#endregion

#region SelectAsync_Try

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, Task<Try<TResult>>> selector)
        => @try.Success
            ? ResultAsync(_ => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value))
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, ValueTask<Try<TResult>>> selector)
        => @try.Success
            ? ResultAsync(_ => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value))
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, CancellationToken, Task<Try<TResult>>> selector,
        CancellationToken cancellation = default)
        => @try.Success
            ? ResultAsync(cancel => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value, cancel), cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, CancellationToken, ValueTask<Try<TResult>>> selector,
        CancellationToken cancellation = default)
        => @try.Success
            ? ResultAsync(cancel => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(@try.Value, cancel), cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> @try, Func<T, Task<Try<TResult>>> selector)
    {
        var result = (await @try.AsTryAsync().ConfigureAwait(false)).Unwrap();
        return result.Success
            ? await ResultAsync(_ => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value))
                .ConfigureAwait(false)
            : Try.Error<TResult>(result.Error!);
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> @try, Func<T, ValueTask<Try<TResult>>> selector)
    {
        var result = (await @try.AsTryAsync().ConfigureAwait(false)).Unwrap();
        return result.Success
            ? await ResultAsync(_ => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value))
                .ConfigureAwait(false)
            : Try.Error<TResult>(result.Error!);
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> @try,
        Func<T, CancellationToken, Task<Try<TResult>>> selector, CancellationToken cancellation = default)
    {
        var result = (await @try.AsTryAsync(cancellation).ConfigureAwait(false)).Unwrap();
        return result.Success
            ? await ResultAsync(cancel => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancel), cancellation)
                .ConfigureAwait(false)
            : Try.Error<TResult>(result.Error!);
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Try{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> @try,
        Func<T, CancellationToken, ValueTask<Try<TResult>>> selector, CancellationToken cancellation = default)
    {
        var result = (await @try.AsTryAsync(cancellation).ConfigureAwait(false)).Unwrap();
        return result.Success
            ? await ResultAsync(cancel => (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancel), cancellation)
                .ConfigureAwait(false)
            : Try.Error<TResult>(result.Error!);
    }

#endregion
}
