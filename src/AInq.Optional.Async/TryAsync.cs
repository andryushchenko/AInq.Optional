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

    private static async ValueTask<Try<TResult>> AwaitSelect<T, TResult>(Task<Try<T>> tryTask, Func<T, TResult> selector,
        CancellationToken cancellation)
    {
        try
        {
            return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<TResult>(ex);
        }
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> Select<T, TResult>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<TResult>>(tryTask.Result.Select(selector ?? throw new ArgumentNullException(nameof(selector))))
            : AwaitSelect(tryTask, selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> Select<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<TResult>>(tryValueTask.Result.Select(selector ?? throw new ArgumentNullException(nameof(selector))))
            : AwaitSelect(tryValueTask.AsTask(), selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    private static async ValueTask<Try<TResult>> AwaitSelect<T, TResult>(Task<Try<T>> tryTask, Func<T, Try<TResult>> selector,
        CancellationToken cancellation)
    {
        try
        {
            return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<TResult>(ex);
        }
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> Select<T, TResult>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, Try<TResult>> selector, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<TResult>>(tryTask.Result.Select(selector ?? throw new ArgumentNullException(nameof(selector))))
            : AwaitSelect(tryTask, selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> Select<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, Try<TResult>> selector, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<TResult>>(tryValueTask.Result.Select(selector ?? throw new ArgumentNullException(nameof(selector))))
            : AwaitSelect(tryValueTask.AsTask(), selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

#endregion

#region SelectAsync

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(@try.Value, cancellation).AsTryAsync(cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Try<T> @try,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(@try.Value, cancellation)
            : new ValueTask<Try<TResult>>(Try.Error<TResult>(@try.Error!));

    private static async ValueTask<Try<TResult>> AwaitSelectAsync<T, TResult>(Task<Try<T>> tryTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation)
    {
        try
        {
            return await (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<TResult>(ex);
        }
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectAsync(tryTask, asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectAsync(tryValueTask.AsTask(), asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

    private static async ValueTask<Try<TResult>> AwaitSelectAsync<T, TResult>(Task<Try<T>> tryTask,
        Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector, CancellationToken cancellation)
    {
        try
        {
            return await (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<TResult>(ex);
        }
    }

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectAsync(tryTask, asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

    /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TResult>> SelectAsync<T, TResult>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectAsync(tryValueTask.AsTask(), asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

#endregion

#region Or

    private static async ValueTask<Try<T>> AwaitOr<T>(Task<Try<T>> tryTask, Try<T> other, CancellationToken cancellation)
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
    }

    /// <inheritdoc cref="Try.Or{T}(Try{T},Try{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> Or<T>(this Task<Try<T>> tryTask, Try<T> other, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Or(other ?? throw new ArgumentNullException(nameof(other))))
            : AwaitOr(tryTask, other ?? throw new ArgumentNullException(nameof(other)), cancellation);

    /// <inheritdoc cref="Try.Or{T}(Try{T},Try{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> Or<T>(this ValueTask<Try<T>> tryValueTask, Try<T> other, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Or(other ?? throw new ArgumentNullException(nameof(other))))
            : AwaitOr(tryValueTask.AsTask(), other ?? throw new ArgumentNullException(nameof(other)), cancellation);

    private static async ValueTask<Try<T>> AwaitOr<T>(Task<Try<T>> tryTask, Func<Try<T>> otherGenerator, CancellationToken cancellation)
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
        return @try.Or(otherGenerator);
    }

    /// <inheritdoc cref="Try.Or{T}(Try{T},Func{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> Or<T>(this Task<Try<T>> tryTask, [InstantHandle(RequireAwait = true)] Func<Try<T>> otherGenerator,
        CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Or(otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))))
            : AwaitOr(tryTask, otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator)), cancellation);

    /// <inheritdoc cref="Try.Or{T}(Try{T},Func{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> Or<T>(this ValueTask<Try<T>> tryValueTask, [InstantHandle(RequireAwait = true)] Func<Try<T>> otherGenerator,
        CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Or(otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))))
            : AwaitOr(tryValueTask.AsTask(), otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator)), cancellation);

#endregion

#region OrAsync

    /// <inheritdoc cref="Try.Or{T}(Try{T},Func{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> OrAsync<T>(this Try<T> @try,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<Try<T>>> asyncOtherGenerator, CancellationToken cancellation = default)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? new ValueTask<Try<T>>(@try)
            : (asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator))).Invoke(cancellation)
                                                                                                   .AsTryAsync(cancellation)
                                                                                                   .Unwrap(cancellation);

    private static async ValueTask<Try<T>> AwaitOrAsync<T>(Task<Try<T>> tryTask, Func<CancellationToken, ValueTask<Try<T>>> asyncOtherGenerator,
        CancellationToken cancellation)
        => await (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).OrAsync(asyncOtherGenerator, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="Try.Or{T}(Try{T},Func{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> OrAsync<T>(this Task<Try<T>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<Try<T>>> asyncOtherGenerator, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result.OrAsync(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation)
            : AwaitOrAsync(tryTask, asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

    /// <inheritdoc cref="Try.Or{T}(Try{T},Func{Try{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> OrAsync<T>(this ValueTask<Try<T>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<Try<T>>> asyncOtherGenerator, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result.OrAsync(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation)
            : AwaitOrAsync(tryValueTask.AsTask(), asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

#endregion

#region Utils

    private static async ValueTask<Try<T>> AwaitUnwrap<T>(Task<Try<Try<T>>> tryTask, CancellationToken cancellation)
    {
        try
        {
            return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<T>(ex);
        }
    }

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

    private static async ValueTask<Try<T>> AwaitThrow<T>(Task<Try<T>> tryTask, CancellationToken cancellation)
        => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Throw();

    /// <inheritdoc cref="Try{T}.Throw()" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T>(this Task<Try<T>> tryTask, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Throw())
            : AwaitThrow(tryTask, cancellation);

    /// <inheritdoc cref="Try{T}.Throw()" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T>(this ValueTask<Try<T>> tryValueTask, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Throw())
            : AwaitThrow(tryValueTask.AsTask(), cancellation);

    private static async ValueTask<Try<T>> AwaitThrow<T, TException>(Task<Try<T>> tryTask, CancellationToken cancellation)
        where TException : Exception
    {
        try
        {
            return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Throw<TException>();
        }
        catch (Exception ex) when (ex is not OperationCanceledException && ex is not TException)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="Try{T}.Throw{TException}()" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T, TException>(this Task<Try<T>> tryTask, CancellationToken cancellation = default)
        where TException : Exception
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Throw<TException>())
            : AwaitThrow<T, TException>(tryTask, cancellation);

    /// <inheritdoc cref="Try{T}.Throw{TException}()" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T, TException>(this ValueTask<Try<T>> tryValueTask, CancellationToken cancellation = default)
        where TException : Exception
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Throw<TException>())
            : AwaitThrow<T, TException>(tryValueTask.AsTask(), cancellation);

    private static async ValueTask<Try<T>> AwaitThrow<T>(Task<Try<T>> tryTask, Type exceptionType, CancellationToken cancellation)
    {
        try
        {
            return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Throw(exceptionType);
        }
        catch (Exception ex) when (ex is not OperationCanceledException && ex.GetType() != exceptionType)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="Try{T}.Throw(Type)" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T>(this Task<Try<T>> tryTask, Type exceptionType, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(tryTask.Result.Throw(exceptionType))
            : AwaitThrow(tryTask, exceptionType, cancellation);

    /// <inheritdoc cref="Try{T}.Throw(Type)" />
    [PublicAPI, AssertionMethod]
    public static ValueTask<Try<T>> Throw<T>(this ValueTask<Try<T>> tryValueTask, Type exceptionType, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(tryValueTask.Result.Throw(exceptionType))
            : AwaitThrow(tryValueTask.AsTask(), exceptionType, cancellation);

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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},Action{Exception},TArgument)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},Action{Exception},TArgument)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},Action{Exception},TArgument)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},Action{Exception},TArgument)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},Action{Exception},TArgument)" />
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

    /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
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
