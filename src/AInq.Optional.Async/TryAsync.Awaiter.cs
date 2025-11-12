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

public static partial class TryAsync
{
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

#endregion

#region SelectAsync

    private static async ValueTask<Try<TResult>> AwaitSelect<T, TResult>(Task<Try<T>> tryTask,
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

    private static async ValueTask<Try<TResult>> AwaitSelect<T, TResult>(Task<Try<T>> tryTask,
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

#endregion

#region Throw

    private static async ValueTask<Try<T>> AwaitThrow<T>(Task<Try<T>> tryTask, CancellationToken cancellation)
        => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Throw();

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

#endregion

#region Do

    private static async ValueTask AwaitDo<T>(Task<Try<T>> tryTask, Action<T> valueAction, Action<Exception> errorAction,
        CancellationToken cancellation)
        => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, errorAction);

    private static async ValueTask AwaitDo<T>(Task<Try<T>> tryTask, Action<T> valueAction, bool throwIfError, CancellationToken cancellation)
        => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, throwIfError);

    private static async ValueTask AwaitDoIfError<T>(Task<Try<T>> tryTask, Action<Exception> errorAction, CancellationToken cancellation)
        => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfError(errorAction);

#endregion

#region DoWithArgument

    private static async ValueTask AwaitDo<T, TArgument>(Task<Try<T>> tryTask, Action<T, TArgument> valueAction, Action<Exception> errorAction,
        TArgument argument, CancellationToken cancellation)
        => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, errorAction, argument);

    private static async ValueTask AwaitDo<T, TArgument>(Task<Try<T>> tryTask, Action<T, TArgument> valueAction, TArgument argument,
        bool throwIfError, CancellationToken cancellation)
        => (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, argument, throwIfError);

#endregion
}
