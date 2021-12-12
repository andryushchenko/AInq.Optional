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
using static AInq.Optional.TaskHelper;

namespace AInq.Optional;

/// <summary> Try async extension </summary>
public static class TryAsync
{
#region ResultAsync

    public static async ValueTask<Try<T>> ResultAsync<T>(Task<T> task, CancellationToken cancellation = default)
    {
        _ = task ?? throw new ArgumentNullException(nameof(task));
        try
        {
            return Try.Value(await task.WaitAsync(cancellation).ConfigureAwait(false));
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    public static ValueTask<Try<T>> ResultAsync<T>(ValueTask<T> valueTask, CancellationToken cancellation = default)
        => valueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(Try.Value(valueTask.Result))
            : ResultAsync(valueTask.AsTask(), cancellation);

    public static ValueTask<Try<T>> AsTryAsync<T>(this Task<T> task, CancellationToken cancellation = default)
        => ResultAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    public static ValueTask<Try<T>> AsTryAsync<T>(this ValueTask<T> valueTask, CancellationToken cancellation = default)
        => ResultAsync(valueTask, cancellation);

#endregion

#region Select

    public static ValueTask<Try<TResult>> Select<T, TResult>(this Task<Try<T>> tryTask, Func<T, TResult> selector,
        CancellationToken cancellation = default)
    {
        _ = tryTask ?? throw new ArgumentNullException(nameof(tryTask));
        _ = selector ?? throw new ArgumentNullException(nameof(selector));
        return tryTask.Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<TResult>>(tryTask.Result.Select(selector))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);
                }
                catch (Exception ex)
                {
                    return Try.Error<TResult>(ex);
                }
            });
    }

    public static ValueTask<Try<TResult>> Select<T, TResult>(this Task<Try<T>> tryTask, Func<T, Try<TResult>> selector,
        CancellationToken cancellation = default)
    {
        _ = tryTask ?? throw new ArgumentNullException(nameof(tryTask));
        _ = selector ?? throw new ArgumentNullException(nameof(selector));
        return tryTask.Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<TResult>>(tryTask.Result.Select(selector))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);
                }
                catch (Exception ex)
                {
                    return Try.Error<TResult>(ex);
                }
            });
    }

    public static ValueTask<Try<TResult>> Select<T, TResult>(this ValueTask<Try<T>> tryValueTask, Func<T, TResult> selector,
        CancellationToken cancellation = default)
    {
        _ = selector ?? throw new ArgumentNullException(nameof(selector));
        return tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<TResult>>(tryValueTask.Result.Select(selector))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);
                }
                catch (Exception ex)
                {
                    return Try.Error<TResult>(ex);
                }
            });
    }

    public static ValueTask<Try<TResult>> Select<T, TResult>(this ValueTask<Try<T>> tryValueTask, Func<T, Try<TResult>> selector,
        CancellationToken cancellation = default)
    {
        _ = selector ?? throw new ArgumentNullException(nameof(selector));
        return tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<TResult>>(tryValueTask.Result.Select(selector))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);
                }
                catch (Exception ex)
                {
                    return Try.Error<TResult>(ex);
                }
            });
    }

#endregion

#region SelectAsync

    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        CancellationToken cancellation = default)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(@try.Value, cancellation).AsTryAsync(cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> tryTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion)
            return tryTask.Result.SelectAsync(asyncSelector, cancellation);
        _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
        return FromFunctionAsync(async ()
            =>
        {
            try
            {
                return await (await tryTask.WaitAsync(cancellation).ConfigureAwait(false))
                             .SelectAsync(asyncSelector, cancellation)
                             .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Try.Error<TResult>(ex);
            }
        });
    }

    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
    {
        if (tryValueTask.IsCompletedSuccessfully)
            return tryValueTask.Result.SelectAsync(asyncSelector, cancellation);
        _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
        return FromFunctionAsync(async () =>
        {
            try
            {
                return await (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                             .SelectAsync(asyncSelector, cancellation)
                             .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Try.Error<TResult>(ex);
            }
        });
    }

    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try, Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(@try.Value, cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> tryTask,
        Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector, CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion)
            return tryTask.Result.SelectAsync(asyncSelector, cancellation);
        _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
        return FromFunctionAsync(async ()
            =>
        {
            try
            {
                return await (await tryTask.WaitAsync(cancellation).ConfigureAwait(false))
                             .SelectAsync(asyncSelector, cancellation)
                             .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Try.Error<TResult>(ex);
            }
        });
    }

    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector, CancellationToken cancellation = default)
    {
        if (tryValueTask.IsCompletedSuccessfully)
            return tryValueTask.Result.SelectAsync(asyncSelector, cancellation);
        _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
        return FromFunctionAsync(async () =>
        {
            try
            {
                return await (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                             .SelectAsync(asyncSelector, cancellation)
                             .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return Try.Error<TResult>(ex);
            }
        });
    }

#endregion

#region Utils

    public static ValueTask<Try<T>> Or<T>(this Task<Try<T>> tryTask, Try<T> other, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Or(other))
            : FromFunctionAsync(async () =>
            {
                Try<T> @try;
                try
                {
                    @try = await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    @try = Try.Error<T>(ex);
                }
                return @try.Or(other);
            });

    public static ValueTask<Try<T>> Or<T>(this ValueTask<Try<T>> tryValueTask, Try<T> other, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Or(other))
            : FromFunctionAsync(async () =>
            {
                Try<T> @try;
                try
                {
                    @try = await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    @try = Try.Error<T>(ex);
                }
                return @try.Or(other);
            });

    public static ValueTask<Try<T>> Unwrap<T>(this Task<Try<Try<T>>> tryTask, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Unwrap())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();
                }
                catch (Exception ex)
                {
                    return Try.Error<T>(ex);
                }
            });

    public static ValueTask<Try<T>> Unwrap<T>(this ValueTask<Try<Try<T>>> tryValueTask, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Unwrap())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();
                }
                catch (Exception ex)
                {
                    return Try.Error<T>(ex);
                }
            });

    public static async IAsyncEnumerable<T> Values<T>(this IAsyncEnumerable<Try<T>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var @try in collection.WithCancellation(cancellation).ConfigureAwait(false))
            // ReSharper disable once ConstantConditionalAccessQualifier
            if (@try?.Success ?? false)
                yield return @try.Value;
    }

    public static async IAsyncEnumerable<Exception> Errors<T>(this IAsyncEnumerable<Try<T>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var @try in collection.WithCancellation(cancellation).ConfigureAwait(false))
            // ReSharper disable once ConstantConditionalAccessQualifier
            if (!(@try?.Success ?? true))
                yield return @try.Error!;
    }

#endregion

#region Do

    public static ValueTask Do<T>(this Task<Try<T>> tryTask, Action<T> valueAction, Action<Exception> errorAction,
        CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, errorAction));
        tryTask.Result.Do(valueAction, errorAction);
        return default;
    }

    public static ValueTask Do<T>(this Task<Try<T>> tryTask, Action<T> valueAction, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, throwIfError));
        tryTask.Result.Do(valueAction, throwIfError);
        return default;
    }

    public static ValueTask DoIfError<T>(this Task<Try<T>> tryTask, Action<Exception> errorAction, CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfError(errorAction));
        tryTask.Result.DoIfError(errorAction);
        return default;
    }

    public static ValueTask Do<T>(this ValueTask<Try<T>> tryValueTask, Action<T> valueAction, Action<Exception> errorAction,
        CancellationToken cancellation = default)
    {
        if (tryValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, errorAction));
        tryValueTask.Result.Do(valueAction, errorAction);
        return default;
    }

    public static ValueTask Do<T>(this ValueTask<Try<T>> tryValueTask, Action<T> valueAction, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        if (tryValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, throwIfError));
        tryValueTask.Result.Do(valueAction, throwIfError);
        return default;
    }

    public static ValueTask DoIfError<T>(this ValueTask<Try<T>> tryValueTask, Action<Exception> errorAction, CancellationToken cancellation = default)
    {
        if (tryValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async () => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).DoIfError(errorAction));
        tryValueTask.Result.DoIfError(errorAction);
        return default;
    }

#endregion

#region DoAsync

    public static async Task DoAsync<T>(this Try<T> @try, Func<T, CancellationToken, Task> valueAction,
        Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this Try<T> @try, Func<T, CancellationToken, Task> valueAction, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

    public static async Task DoIfErrorAsync<T>(this Try<T> @try, Func<Exception, CancellationToken, Task> errorAction,
        CancellationToken cancellation = default)
    {
        if (!(@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this Task<Try<T>> tryTask, Func<T, CancellationToken, Task> valueAction,
        Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result
            : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this Task<Try<T>> tryTask, Func<T, CancellationToken, Task> valueAction, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result
            : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

    public static async Task DoIfErrorAsync<T>(this Task<Try<T>> tryTask, Func<Exception, CancellationToken, Task> errorAction,
        CancellationToken cancellation = default)
    {
        var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result
            : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (!@try.Success)
            await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this ValueTask<Try<T>> tryValueTask, Func<T, CancellationToken, Task> valueAction,
        Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        var @try = tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result
            : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this ValueTask<Try<T>> tryValueTask, Func<T, CancellationToken, Task> valueAction, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        var @try = tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result
            : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

    public static async Task DoIfErrorAsync<T>(this ValueTask<Try<T>> tryValueTask, Func<Exception, CancellationToken, Task> errorAction,
        CancellationToken cancellation = default)
    {
        var @try = tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result
            : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (!@try.Success)
            await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

#endregion
}
