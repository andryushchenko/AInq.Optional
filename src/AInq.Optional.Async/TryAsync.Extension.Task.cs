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
    /// <param name="tryTask"> Try item </param>
    /// <typeparam name="T"> Source value type </typeparam>
    extension<T>(Task<Try<T>> tryTask)
    {
#region Convert

        /// <inheritdoc cref="Try.ToMaybe{T}(Try{T},bool)" />
        [PublicAPI, Pure]
        public ValueTask<Maybe<T>> ToMaybe(bool suppressException = false, CancellationToken cancellation = default)
            => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Maybe<T>>(tryTask.Result.ToMaybe(suppressException))
                : AwaitToMaybe(tryTask, suppressException, cancellation);

#endregion

#region Select

        /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> Select<TResult>([InstantHandle(RequireAwait = true)] Func<T, TResult> selector,
            CancellationToken cancellation = default)
            => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Try<TResult>>(tryTask.Result.Select(selector ?? throw new ArgumentNullException(nameof(selector))))
                : AwaitSelect(tryTask, selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

        /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> Select<TResult>([InstantHandle(RequireAwait = true)] Func<T, Try<TResult>> selector,
            CancellationToken cancellation = default)
            => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Try<TResult>>(tryTask.Result.Select(selector ?? throw new ArgumentNullException(nameof(selector))))
                : AwaitSelect(tryTask, selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

#endregion

#region SelectAsync

        /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            CancellationToken cancellation = default)
            => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? tryTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
                : AwaitSelect(tryTask, asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

        /// <inheritdoc cref="Try.Select{T,TResult}(Try{T},Func{T,Try{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
            CancellationToken cancellation = default)
            => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? tryTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
                : AwaitSelect(tryTask, asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

#endregion

#region Throw

        /// <inheritdoc cref="Try{T}.Throw()" />
        [PublicAPI, AssertionMethod]
        public ValueTask<Try<T>> Throw(CancellationToken cancellation = default)
            => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Try<T>>(tryTask.Result.Throw())
                : AwaitThrow(tryTask, cancellation);

        /// <inheritdoc cref="Try{T}.Throw{TException}()" />
        [PublicAPI, AssertionMethod]
        public ValueTask<Try<T>> Throw<TException>(CancellationToken cancellation = default)
            where TException : Exception
            => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Try<T>>(tryTask.Result.Throw<TException>())
                : AwaitThrow<T, TException>(tryTask, cancellation);

        /// <inheritdoc cref="Try{T}.Throw(Type)" />
        [PublicAPI, AssertionMethod]
        public ValueTask<Try<T>> Throw(Type exceptionType, CancellationToken cancellation = default)
            => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<Try<T>>(tryTask.Result.Throw(exceptionType))
                : AwaitThrow(tryTask, exceptionType, cancellation);

#endregion

#region Do

        /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
        [PublicAPI]
        public ValueTask Do([InstantHandle(RequireAwait = true)] Action<T> valueAction,
            [InstantHandle(RequireAwait = true)] Action<Exception> errorAction, CancellationToken cancellation = default)
        {
            if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDo(tryTask,
                    valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                    errorAction ?? throw new ArgumentNullException(nameof(errorAction)),
                    cancellation);
            tryTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                errorAction ?? throw new ArgumentNullException(nameof(errorAction)));
            return default;
        }

        /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
        [PublicAPI]
        public ValueTask Do([InstantHandle(RequireAwait = true)] Action<T> valueAction, bool throwIfError = false,
            CancellationToken cancellation = default)
        {
            if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDo(tryTask, valueAction ?? throw new ArgumentNullException(nameof(valueAction)), throwIfError, cancellation);
            tryTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)), throwIfError);
            return default;
        }

        /// <inheritdoc cref="Try.DoIfError{T}" />
        [PublicAPI]
        public ValueTask DoIfError([InstantHandle(RequireAwait = true)] Action<Exception> errorAction, CancellationToken cancellation = default)
        {
            if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDoIfError(tryTask, errorAction ?? throw new ArgumentNullException(nameof(errorAction)), cancellation);
            tryTask.Result.DoIfError(errorAction ?? throw new ArgumentNullException(nameof(errorAction)));
            return default;
        }

#endregion

#region DoWithArgument

        /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},Action{Exception},TArgument)" />
        [PublicAPI]
        public ValueTask Do<TArgument>([InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction,
            [InstantHandle(RequireAwait = true)] Action<Exception> errorAction, TArgument argument, CancellationToken cancellation = default)
        {
            if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDo(tryTask,
                    valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                    errorAction ?? throw new ArgumentNullException(nameof(errorAction)),
                    argument,
                    cancellation);
            tryTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                errorAction ?? throw new ArgumentNullException(nameof(errorAction)),
                argument);
            return default;
        }

        /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
        [PublicAPI]
        public ValueTask Do<TArgument>([InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction, TArgument argument,
            bool throwIfError = false, CancellationToken cancellation = default)
        {
            if ((tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is not TaskStatus.RanToCompletion)
                return AwaitDo(tryTask, valueAction ?? throw new ArgumentNullException(nameof(valueAction)), argument, throwIfError, cancellation);
            tryTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)), argument, throwIfError);
            return default;
        }

#endregion

#region DoAsync

        /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},Action{Exception})" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> asyncErrorAction, CancellationToken cancellation = default)
        {
            var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? tryTask.Result
                : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(@try.Value, cancellation)
                    .ConfigureAwait(false);
            else
                try
                {
                    @try.Throw();
                }
                catch (Exception exception)
                {
                    await (asyncErrorAction ?? throw new ArgumentNullException(nameof(asyncErrorAction))).Invoke(exception, cancellation)
                        .ConfigureAwait(false);
                }
        }

        /// <inheritdoc cref="Try.Do{T}(Try{T},Action{T},bool)" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction, bool throwIfError = false,
            CancellationToken cancellation = default)
        {
            var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? tryTask.Result
                : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(@try.Value, cancellation)
                    .ConfigureAwait(false);
            else if (throwIfError) @try.Throw();
        }

        /// <inheritdoc cref="Try.DoIfError{T}" />
        [PublicAPI]
        public async Task DoIfErrorAsync([InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> asyncErrorAction,
            CancellationToken cancellation = default)
        {
            var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? tryTask.Result
                : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success) return;
            try
            {
                @try.Throw();
            }
            catch (Exception exception)
            {
                await (asyncErrorAction ?? throw new ArgumentNullException(nameof(asyncErrorAction))).Invoke(exception, cancellation)
                    .ConfigureAwait(false);
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
            var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? tryTask.Result
                : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(@try.Value, argument, cancellation)
                    .ConfigureAwait(false);
            else
                try
                {
                    @try.Throw();
                }
                catch (Exception exception)
                {
                    await (asyncErrorAction ?? throw new ArgumentNullException(nameof(asyncErrorAction))).Invoke(exception, cancellation)
                        .ConfigureAwait(false);
                }
        }

        /// <inheritdoc cref="Try.Do{T,TArgument}(Try{T},Action{T,TArgument},TArgument,bool)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            TArgument argument, bool throwIfError = false, CancellationToken cancellation = default)
        {
            var @try = (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
                ? tryTask.Result
                : await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
            if (@try.Success)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(@try.Value, argument, cancellation)
                    .ConfigureAwait(false);
            else if (throwIfError) @try.Throw();
        }

#endregion
    }
}
