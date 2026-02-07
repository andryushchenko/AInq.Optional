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
    /// <param name="tryValueTask"> Try item </param>
    /// <typeparam name="T"> Source value type </typeparam>
    extension<T>(ValueTask<Try<T>> tryValueTask)
    {
#region Convert

        /// <inheritdoc cref="Try.MaybeValue{T}" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> MaybeValue(bool suppressException = false, CancellationToken cancellation = default)
            => tryValueTask.IsCompletedSuccessfully
                ? new ValueTask<Maybe<T>>(tryValueTask.Result.MaybeValue(suppressException))
                : AwaitMaybe(tryValueTask.AsTask(), suppressException, cancellation);

#endregion

#region Select

        /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> Select<TResult>([InstantHandle(RequireAwait = true)] Func<T, TResult> selector,
            CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return tryValueTask.IsCompletedSuccessfully
                ? new ValueTask<Try<TResult>>(tryValueTask.Result.Select(selector))
                : AwaitSelect(tryValueTask.AsTask(), selector, cancellation);
        }

        /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> Select<TResult>([InstantHandle(RequireAwait = true)] Func<T, Try<TResult>> selector,
            CancellationToken cancellation = default)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            return tryValueTask.IsCompletedSuccessfully
                ? new ValueTask<Try<TResult>>(tryValueTask.Result.Select(selector))
                : AwaitSelect(tryValueTask.AsTask(), selector, cancellation);
        }

#endregion

#region SelectAsync

        /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            return tryValueTask.IsCompletedSuccessfully
                ? tryValueTask.Result.SelectAsync(asyncSelector, cancellation)
                : AwaitSelect(tryValueTask.AsTask(), asyncSelector, cancellation);
        }

        /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
            CancellationToken cancellation = default)
        {
            _ = asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector));
            return tryValueTask.IsCompletedSuccessfully
                ? tryValueTask.Result.SelectAsync(asyncSelector, cancellation)
                : AwaitSelect(tryValueTask.AsTask(), asyncSelector, cancellation);
        }

#endregion

#region Trow

        /// <inheritdoc cref="Try{T}.Throw()" />
        [PublicAPI, AssertionMethod]
        public ValueTask<Try<T>> Throw(CancellationToken cancellation = default)
            => tryValueTask.IsCompletedSuccessfully
                ? new ValueTask<Try<T>>(tryValueTask.Result.Throw())
                : AwaitThrow(tryValueTask.AsTask(), cancellation);

        /// <inheritdoc cref="Try{T}.Throw{TException}()" />
        [PublicAPI, AssertionMethod]
        public ValueTask<Try<T>> Throw<TException>(CancellationToken cancellation = default)
            where TException : Exception
            => tryValueTask.IsCompletedSuccessfully
                ? new ValueTask<Try<T>>(tryValueTask.Result.Throw<TException>())
                : AwaitThrow<T, TException>(tryValueTask.AsTask(), cancellation);

        /// <inheritdoc cref="Try{T}.Throw(Type)" />
        [PublicAPI, AssertionMethod]
        public ValueTask<Try<T>> Throw(Type exceptionType, CancellationToken cancellation = default)
            => tryValueTask.IsCompletedSuccessfully
                ? new ValueTask<Try<T>>(tryValueTask.Result.Throw(exceptionType))
                : AwaitThrow(tryValueTask.AsTask(), exceptionType, cancellation);

#endregion

#region Do

        /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
        [PublicAPI]
        public ValueTask Do([InstantHandle(RequireAwait = true)] Action<T> valueAction,
            [InstantHandle(RequireAwait = true)] Action<Exception> errorAction, CancellationToken cancellation = default)
        {
            _ = valueAction ?? throw new ArgumentNullException(nameof(valueAction));
            _ = errorAction ?? throw new ArgumentNullException(nameof(errorAction));
            if (!tryValueTask.IsCompletedSuccessfully) return AwaitDo(tryValueTask.AsTask(), valueAction, errorAction, cancellation);
            tryValueTask.Result.Do(valueAction, errorAction);
            return default;
        }

        /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
        [PublicAPI]
        public ValueTask Do([InstantHandle(RequireAwait = true)] Action<T> valueAction, bool throwIfError = false,
            CancellationToken cancellation = default)
        {
            _ = valueAction ?? throw new ArgumentNullException(nameof(valueAction));
            if (!tryValueTask.IsCompletedSuccessfully) return AwaitDo(tryValueTask.AsTask(), valueAction, throwIfError, cancellation);
            tryValueTask.Result.Do(valueAction, throwIfError);
            return default;
        }

        /// <inheritdoc cref="Try.DoIfError{T}" />
        [PublicAPI]
        public ValueTask DoIfError([InstantHandle(RequireAwait = true)] Action<Exception> errorAction, CancellationToken cancellation = default)
        {
            _ = errorAction ?? throw new ArgumentNullException(nameof(errorAction));
            if (!tryValueTask.IsCompletedSuccessfully) return AwaitDoIfError(tryValueTask.AsTask(), errorAction, cancellation);
            tryValueTask.Result.DoIfError(errorAction);
            return default;
        }

#endregion

#region DoWithArgument

        /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},Action{Exception},TArgument)" />
        [PublicAPI]
        public ValueTask Do<TArgument>([InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction,
            [InstantHandle(RequireAwait = true)] Action<Exception> errorAction, TArgument argument, CancellationToken cancellation = default)
        {
            _ = valueAction ?? throw new ArgumentNullException(nameof(valueAction));
            _ = errorAction ?? throw new ArgumentNullException(nameof(errorAction));
            if (!tryValueTask.IsCompletedSuccessfully) return AwaitDo(tryValueTask.AsTask(), valueAction, errorAction, argument, cancellation);
            tryValueTask.Result.Do(valueAction, errorAction, argument);
            return default;
        }

        /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
        [PublicAPI]
        public ValueTask Do<TArgument>([InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction, TArgument argument,
            bool throwIfError = false, CancellationToken cancellation = default)
        {
            _ = valueAction ?? throw new ArgumentNullException(nameof(valueAction));
            if (!tryValueTask.IsCompletedSuccessfully) return AwaitDo(tryValueTask.AsTask(), valueAction, argument, throwIfError, cancellation);
            tryValueTask.Result.Do(valueAction, argument, throwIfError);
            return default;
        }

#endregion

#region DoAsync

        /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> asyncErrorAction, CancellationToken cancellation = default)
        {
            _ = asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction));
            _ = asyncErrorAction ?? throw new ArgumentNullException(nameof(asyncErrorAction));
            var @try = tryValueTask.IsCompletedSuccessfully
                ? tryValueTask.Result
                : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success) await asyncValueAction.Invoke(@try.Value, cancellation).ConfigureAwait(false);
            else
                try
                {
                    @try.Throw();
                }
                catch (Exception exception)
                {
                    await asyncErrorAction.Invoke(exception, cancellation).ConfigureAwait(false);
                }
        }

        /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction, bool throwIfError = false,
            CancellationToken cancellation = default)
        {
            _ = asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction));
            var @try = tryValueTask.IsCompletedSuccessfully
                ? tryValueTask.Result
                : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success) await asyncValueAction.Invoke(@try.Value, cancellation).ConfigureAwait(false);
            else if (throwIfError) @try.Throw();
        }

        /// <inheritdoc cref="Try.DoIfError{T}" />
        [PublicAPI]
        public async Task DoIfErrorAsync([InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> asyncErrorAction,
            CancellationToken cancellation = default)
        {
            _ = asyncErrorAction ?? throw new ArgumentNullException(nameof(asyncErrorAction));
            var @try = tryValueTask.IsCompletedSuccessfully
                ? tryValueTask.Result
                : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success) return;
            try
            {
                @try.Throw();
            }
            catch (Exception exception)
            {
                await asyncErrorAction.Invoke(exception, cancellation).ConfigureAwait(false);
            }
        }

#endregion

#region DoAsyncWithArgument

        /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},Action{Exception},TArgument)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> asyncErrorAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            _ = asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction));
            _ = asyncErrorAction ?? throw new ArgumentNullException(nameof(asyncErrorAction));
            var @try = tryValueTask.IsCompletedSuccessfully
                ? tryValueTask.Result
                : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success) await asyncValueAction.Invoke(@try.Value, argument, cancellation).ConfigureAwait(false);
            else
                try
                {
                    @try.Throw();
                }
                catch (Exception exception)
                {
                    await asyncErrorAction.Invoke(exception, cancellation).ConfigureAwait(false);
                }
        }

        /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            TArgument argument, bool throwIfError = false, CancellationToken cancellation = default)
        {
            _ = asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction));
            var @try = tryValueTask.IsCompletedSuccessfully
                ? tryValueTask.Result
                : await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success) await asyncValueAction.Invoke(@try.Value, argument, cancellation).ConfigureAwait(false);
            else if (throwIfError) @try.Throw();
        }

#endregion
    }
}
