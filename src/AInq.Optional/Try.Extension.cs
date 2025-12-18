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

public static partial class Try
{
    /// <param name="try"> Try item </param>
    /// <typeparam name="T"> Source value type </typeparam>
    extension<T>(Try<T> @try)
    {
#region Convert

        /// <summary> Convert <see cref="Try{T}" /> to <see cref="Maybe{T}" />, throw if not success </summary>
        /// <param name="suppressException"> Don't throw exception </param>
        [PublicAPI, Pure]
        public Maybe<T> MaybeValue(bool suppressException = false)
        {
            _ = @try ?? throw new ArgumentNullException(nameof(@try));
            if (!suppressException) @try.Throw();
            return @try.Success ? Maybe<T>.FromValue(@try.Value) : Maybe<T>.None;
        }

#endregion

#region Select

        /// <summary> Convert to other value type </summary>
        /// <param name="selector"> Converter </param>
        /// <typeparam name="TResult"> Result value type </typeparam>
        [PublicAPI, Pure]
        public Try<TResult> Select<TResult>([InstantHandle] Func<T, TResult> selector)
            => !(@try ?? throw new ArgumentNullException(nameof(@try))).Success
                ? Try<TResult>.ConvertError(@try)
                : Result(selector ?? throw new ArgumentNullException(nameof(selector)), @try.Value);

        /// <inheritdoc cref="Select{T,TResult}(Try{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public Try<TResult> Select<TResult>([InstantHandle] Func<T, Try<TResult>> selector)
            => !(@try ?? throw new ArgumentNullException(nameof(@try))).Success
                ? Try<TResult>.ConvertError(@try)
                : Result(selector ?? throw new ArgumentNullException(nameof(selector)), @try.Value).Unwrap();

#endregion

#region Do

        /// <summary> Try to execute action </summary>
        /// <param name="valueAction"> Action if value exists </param>
        /// <param name="errorAction"> Action if error </param>
        [PublicAPI]
        public void Do([InstantHandle] Action<T> valueAction, [InstantHandle] Action<Exception> errorAction)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
                (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value);
            else
                try
                {
                    @try.Throw();
                }
                catch (Exception exception)
                {
                    (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(exception);
                }
        }

        /// <summary> Try to execute action with additional argument </summary>
        /// <param name="valueAction"> Action if value exists </param>
        /// <param name="errorAction"> Action if error </param>
        /// <param name="argument"> Additional action argument </param>
        /// <typeparam name="TArgument"> Additional action argument type </typeparam>
        [PublicAPI]
        public void Do<TArgument>([InstantHandle] Action<T, TArgument> valueAction, [InstantHandle] Action<Exception> errorAction, TArgument argument)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
                (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument);
            else
                try
                {
                    @try.Throw();
                }
                catch (Exception exception)
                {
                    (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(exception);
                }
        }

        /// <summary> Try to execute action with value </summary>
        /// <param name="valueAction"> Action if value exists </param>
        /// <param name="throwIfError"> Throw exception if item contains error </param>
        [PublicAPI]
        public void Do([InstantHandle] Action<T> valueAction, bool throwIfError = false)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
                (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value);
            else if (throwIfError) @try.Throw();
        }

        /// <summary> Try to execute action with value </summary>
        /// <param name="valueAction"> Action if value exists </param>
        /// <param name="argument"> Additional action argument </param>
        /// <param name="throwIfError"> Throw exception if item contains error </param>
        /// <typeparam name="TArgument"> Additional action argument type </typeparam>
        [PublicAPI]
        public void Do<TArgument>([InstantHandle] Action<T, TArgument> valueAction, TArgument argument, bool throwIfError = false)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success)
                (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(@try.Value, argument);
            else if (throwIfError) @try.Throw();
        }

        /// <summary> Try to execute action with error </summary>
        /// <param name="errorAction"> Action if error </param>
        [PublicAPI]
        public void DoIfError([InstantHandle] Action<Exception> errorAction)
        {
            if ((@try ?? throw new ArgumentNullException(nameof(@try))).Success) return;
            try
            {
                @try.Throw();
            }
            catch (Exception exception)
            {
                (errorAction ?? throw new ArgumentNullException(nameof(errorAction))).Invoke(exception);
            }
        }

#endregion
    }
}
