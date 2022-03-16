// Copyright 2021-2022 Anton Andryushchenko
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
        => valueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(Try.Value(valueTask.Result))
            : ResultAsync(valueTask.AsTask(), cancellation);

    /// <inheritdoc cref="ResultAsync{T}(Task{T},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> AsTryAsync<T>(this Task<T> task, CancellationToken cancellation = default)
        => ResultAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ResultAsync{T}(ValueTask{T},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> AsTryAsync<T>(this ValueTask<T> valueTask, CancellationToken cancellation = default)
        => ResultAsync(valueTask, cancellation);

#endregion

#region Select

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> Select<T, TResult>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
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
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<TResult>(ex);
                }
            });
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> Select<T, TResult>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, Try<TResult>> selector, CancellationToken cancellation = default)
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
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<TResult>(ex);
                }
            });
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> Select<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
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
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<TResult>(ex);
                }
            });
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> Select<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, Try<TResult>> selector, CancellationToken cancellation = default)
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
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<TResult>(ex);
                }
            });
    }

#endregion

#region SelectAsync

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(@try.Value, cancellation).AsTryAsync(cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion)
            return tryTask.Result.SelectAsync(asyncSelector, cancellation);
        _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
        return FromFunctionAsync(async () =>
        {
            try
            {
                return await (await tryTask.WaitAsync(cancellation).ConfigureAwait(false))
                             .SelectAsync(asyncSelector, cancellation)
                             .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Try.Error<TResult>(ex);
            }
        });
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Try.Error<TResult>(ex);
            }
        });
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(@try.Value, cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion)
            return tryTask.Result.SelectAsync(asyncSelector, cancellation);
        _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
        return FromFunctionAsync(async () =>
        {
            try
            {
                return await (await tryTask.WaitAsync(cancellation).ConfigureAwait(false))
                             .SelectAsync(asyncSelector, cancellation)
                             .ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Try.Error<TResult>(ex);
            }
        });
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
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
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                return Try.Error<TResult>(ex);
            }
        });
    }

#endregion

#region Utils

    /// <inheritdoc cref="Try.Or{T}(Try{T},Try{T})" />
    [PublicAPI, Pure]
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
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    @try = Try.Error<T>(ex);
                }
                return @try.Or(other);
            });

    /// <inheritdoc cref="Try.Or{T}(Try{T},Try{T})" />
    [PublicAPI, Pure]
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
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    @try = Try.Error<T>(ex);
                }
                return @try.Or(other);
            });

    /// <inheritdoc cref="Try.Unwrap{T}(Try{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> Unwrap<T>(this Task<Try<Try<T>>> tryTask, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Unwrap())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<T>(ex);
                }
            });

    /// <inheritdoc cref="Try.Unwrap{T}(Try{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> Unwrap<T>(this ValueTask<Try<Try<T>>> tryValueTask, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Unwrap())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<T>(ex);
                }
            });

    /// <inheritdoc cref="Try.Values{T}(IEnumerable{Try{T}})" />
    [PublicAPI]
    public static async IAsyncEnumerable<T> Values<T>(this IAsyncEnumerable<Try<T>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var @try in collection.WithCancellation(cancellation).ConfigureAwait(false))
            if (@try is {Success: true})
                yield return @try.Value;
    }

    /// <inheritdoc cref="Try.Errors{T}(IEnumerable{Try{T}})" />
    [PublicAPI]
    public static async IAsyncEnumerable<Exception> Errors<T>(this IAsyncEnumerable<Try<T>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var @try in collection.WithCancellation(cancellation).ConfigureAwait(false))
            if (@try is {Success: false})
                yield return @try.Error!;
    }

    /// <inheritdoc cref="Try{T}.Throw()" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T>(this Task<Try<T>> tryTask, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Throw())
            : FromFunctionAsync(async () => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Throw());

    /// <inheritdoc cref="Try{T}.Throw()" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T>(this ValueTask<Try<T>> tryValueTask, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Throw())
            : FromFunctionAsync(async () => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Throw());

    /// <inheritdoc cref="Try{T}.Throw{TException}()" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T, TException>(this Task<Try<T>> tryTask, CancellationToken cancellation = default)
        where TException : Exception
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Throw<TException>())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Throw<TException>();
                }
                catch (Exception ex) when (ex is not OperationCanceledException && ex is not TException)
                {
                    return Try.Error<T>(ex);
                }
            });

    /// <inheritdoc cref="Try{T}.Throw{TException}()" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T, TException>(this ValueTask<Try<T>> tryValueTask, CancellationToken cancellation = default)
        where TException : Exception
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Throw<TException>())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Throw<TException>();
                }
                catch (Exception ex) when (ex is not OperationCanceledException && ex is not TException)
                {
                    return Try.Error<T>(ex);
                }
            });

    /// <inheritdoc cref="Try{T}.Throw(Type)" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T>(this Task<Try<T>> tryTask, Type exceptionType, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Throw(exceptionType))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Throw(exceptionType);
                }
                catch (Exception ex) when (ex is not OperationCanceledException && ex.GetType() != exceptionType)
                {
                    return Try.Error<T>(ex);
                }
            });

    /// <inheritdoc cref="Try{T}.Throw(Type)" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T>(this ValueTask<Try<T>> tryValueTask, Type exceptionType, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Throw(exceptionType))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Throw(exceptionType);
                }
                catch (Exception ex) when (ex is not OperationCanceledException && ex.GetType() != exceptionType)
                {
                    return Try.Error<T>(ex);
                }
            });

#endregion

#region Do

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
    [PublicAPI]
    public static ValueTask Do<T>(this Task<Try<T>> tryTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        [InstantHandle(RequireAwait = true)] Action<Exception> errorAction, CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, errorAction));
        tryTask.Result.Do(valueAction, errorAction);
        return default;
    }

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
    [PublicAPI]
    public static ValueTask Do<T>(this Task<Try<T>> tryTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, throwIfError));
        tryTask.Result.Do(valueAction, throwIfError);
        return default;
    }

    /// <inheritdoc cref="Try.DoIfError{T}(Try{T},Action{Exception})" />
    [PublicAPI]
    public static ValueTask DoIfError<T>(this Task<Try<T>> tryTask, [InstantHandle(RequireAwait = true)] Action<Exception> errorAction,
        CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfError(errorAction));
        tryTask.Result.DoIfError(errorAction);
        return default;
    }

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
    [PublicAPI]
    public static ValueTask Do<T>(this ValueTask<Try<T>> tryValueTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        [InstantHandle(RequireAwait = true)] Action<Exception> errorAction, CancellationToken cancellation = default)
    {
        if (!tryValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, errorAction));
        tryValueTask.Result.Do(valueAction, errorAction);
        return default;
    }

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
    [PublicAPI]
    public static ValueTask Do<T>(this ValueTask<Try<T>> tryValueTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        bool throwIfError = false, CancellationToken cancellation = default)
    {
        if (!tryValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, throwIfError));
        tryValueTask.Result.Do(valueAction, throwIfError);
        return default;
    }

    /// <inheritdoc cref="Try.DoIfError{T}(Try{T},Action{Exception})" />
    [PublicAPI]
    public static ValueTask DoIfError<T>(this ValueTask<Try<T>> tryValueTask, [InstantHandle(RequireAwait = true)] Action<Exception> errorAction,
        CancellationToken cancellation = default)
    {
        if (!tryValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async () => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).DoIfError(errorAction));
        tryValueTask.Result.DoIfError(errorAction);
        return default;
    }

#endregion

#region DoWithArgument

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument},Action{Exception}, TArgument)" />
    [PublicAPI]
    public static ValueTask Do<T, TArgument>(this Task<Try<T>> tryTask, [InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction,
        [InstantHandle(RequireAwait = true)] Action<Exception> errorAction, TArgument argument, CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async ()
                => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, errorAction, argument));
        tryTask.Result.Do(valueAction, errorAction, argument);
        return default;
    }

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument}, TArgument,bool)" />
    [PublicAPI]
    public static ValueTask Do<T, TArgument>(this Task<Try<T>> tryTask, [InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction,
        TArgument argument, bool throwIfError = false, CancellationToken cancellation = default)
    {
        if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async ()
                => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, argument, throwIfError));
        tryTask.Result.Do(valueAction, argument, throwIfError);
        return default;
    }

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument},Action{Exception}, TArgument)" />
    [PublicAPI]
    public static ValueTask Do<T, TArgument>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction, [InstantHandle(RequireAwait = true)] Action<Exception> errorAction,
        TArgument argument, CancellationToken cancellation = default)
    {
        if (!tryValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, errorAction, argument));
        tryValueTask.Result.Do(valueAction, errorAction, argument);
        return default;
    }

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument}, TArgument,bool)" />
    [PublicAPI]
    public static ValueTask Do<T, TArgument>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction, TArgument argument, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        if (!tryValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, argument, throwIfError));
        tryValueTask.Result.Do(valueAction, argument, throwIfError);
        return default;
    }

#endregion

#region DoAsync

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Try<T> @try, [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Try<T> @try, [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        bool throwIfError = false, CancellationToken cancellation = default)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

    /// <inheritdoc cref="Try.DoIfError{T}(Try{T},Action{Exception})" />
    [PublicAPI]
    public static async Task DoIfErrorAsync<T>(this Try<T> @try,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        if (!(@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Task<Try<T>> tryTask, [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result
            : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Task<Try<T>> tryTask, [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        bool throwIfError = false, CancellationToken cancellation = default)
    {
        var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result
            : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

    /// <inheritdoc cref="Try.DoIfError{T}(Try{T},Action{Exception})" />
    [PublicAPI]
    public static async Task DoIfErrorAsync<T>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result
            : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (!@try.Success)
            await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        var @try = tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result
            : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        var @try = tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result
            : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, cancellation).ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

    /// <inheritdoc cref="Try.DoIfError{T}(Try{T},Action{Exception})" />
    [PublicAPI]
    public static async Task DoIfErrorAsync<T>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, CancellationToken cancellation = default)
    {
        var @try = tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result
            : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (!@try.Success)
            await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

#endregion

#region DoAsyncWithArgument

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument},Action{Exception}, TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this Try<T> @try,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument, cancellation)
                                                                                       .ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument}, TArgument,bool)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this Try<T> @try,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> valueAction, TArgument argument, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument, cancellation)
                                                                                       .ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument},Action{Exception}, TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result
            : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument, cancellation)
                                                                                       .ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument}, TArgument,bool)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> valueAction, TArgument argument, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result
            : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument, cancellation)
                                                                                       .ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument},Action{Exception}, TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> errorAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var @try = tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result
            : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument, cancellation)
                                                                                       .ConfigureAwait(false);
        else await (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(@try.Error!, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Try.Do{T, TArgument}(Try{T},Action{T, TArgument}, TArgument,bool)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> valueAction, TArgument argument, bool throwIfError = false,
        CancellationToken cancellation = default)
    {
        var @try = tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result
            : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (@try.Success)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument, cancellation)
                                                                                       .ConfigureAwait(false);
        else if (throwIfError) throw @try.Error!;
    }

#endregion
}
