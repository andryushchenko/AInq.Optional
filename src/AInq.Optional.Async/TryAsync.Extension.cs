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
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Source value type </typeparam>
    extension<T>(Try<T> @try)
    {
#region SelectAsync

        /// <inheritdoc cref="Try.Select{T,TResult}(AInq.Optional.Try{T},System.Func{T,TResult})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
            CancellationToken cancellation = default)
            => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(@try.Value, cancellation).AsTryAsync(cancellation)
                : new ValueTask<Try<TResult>>(Try<TResult>.ConvertError(@try));

        /// <inheritdoc cref="Try.Select{T,TResult}(AInq.Optional.Try{T},System.Func{T,AInq.Optional.Try{TResult}})" />
        [PublicAPI, Pure]
        public ValueTask<Try<TResult>> SelectAsync<TResult>(
            [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Try<TResult>>> asyncSelector,
            CancellationToken cancellation = default)
            => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
                ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(@try.Value, cancellation)
                : new ValueTask<Try<TResult>>(Try<TResult>.ConvertError(@try));

#endregion

#region DoAsync

        /// <inheritdoc cref="Try.Do{T}(AInq.Optional.Try{T},System.Action{T},System.Action{System.Exception})" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> asyncErrorAction, CancellationToken cancellation = default)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
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

        /// <inheritdoc cref="Try.Do{T}(AInq.Optional.Try{T},System.Action{T},bool)" />
        [PublicAPI]
        public async Task DoAsync([InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction, bool throwIfError = false,
            CancellationToken cancellation = default)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(@try.Value, cancellation)
                    .ConfigureAwait(false);
            else if (throwIfError) @try.Throw();
        }

        /// <inheritdoc cref="Try.DoIfError{T}" />
        [PublicAPI]
        public async Task DoIfErrorAsync([InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> asyncErrorAction,
            CancellationToken cancellation = default)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success) return;
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

        /// <inheritdoc cref="Try.Do{T,TArgument}(AInq.Optional.Try{T},System.Action{T,TArgument},System.Action{System.Exception},TArgument)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            [InstantHandle(RequireAwait = true)] Func<Exception, CancellationToken, Task> asyncErrorAction, TArgument argument,
            CancellationToken cancellation = default)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
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

        /// <inheritdoc cref="Try.Do{T,TArgument}(AInq.Optional.Try{T},System.Action{T,TArgument},TArgument,bool)" />
        [PublicAPI]
        public async Task DoAsync<TArgument>([InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
            TArgument argument, bool throwIfError = false, CancellationToken cancellation = default)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
                await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction))).Invoke(@try.Value, argument, cancellation)
                    .ConfigureAwait(false);
            else if (throwIfError) @try.Throw();
        }

#endregion
    }
}
