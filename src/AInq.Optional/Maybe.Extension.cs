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

public static partial class Maybe
{
    /// <param name="maybe"> Maybe item </param>
    /// <typeparam name="T"> Source value type </typeparam>
    extension<T>(Maybe<T> maybe)
    {
#region Convert

        /// <summary> Get source value or other if empty </summary>
        /// <param name="other"> Other value </param>
        /// <typeparam name="TOther"> Other source type </typeparam>
        /// <returns> Either </returns>
        [PublicAPI, Pure]
        public Either<T, TOther> Or<TOther>([NoEnumeration] TOther other)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? Either<T, TOther>.FromLeft(maybe.Value)
                : Either<T, TOther>.FromRight(other);

        /// <summary> Get source value or other if empty </summary>
        /// <param name="otherGenerator"> Other generator </param>
        /// <typeparam name="TOther"> Other source type </typeparam>
        /// <returns> Either </returns>
        [PublicAPI, Pure]
        public Either<T, TOther> Or<TOther>([InstantHandle] Func<TOther> otherGenerator)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? Either<T, TOther>.FromLeft(maybe.Value)
                : Either<T, TOther>.FromRight((otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))).Invoke());

        /// <summary> Convert <see cref="Maybe{T}" /> to <see cref="Try{T}" /> </summary>
        [PublicAPI, Pure]
        public Try<T> AsTry()
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? Try<T>.FromValue(maybe.Value)
                : Try<T>.FromError(new InvalidOperationException("No value"));

#endregion

#region Select

        /// <summary> Convert to other value type </summary>
        /// <param name="selector"> Converter </param>
        /// <typeparam name="TResult"> Result value type </typeparam>
        [PublicAPI, Pure]
        public Maybe<TResult> Select<TResult>([InstantHandle] Func<T, TResult> selector)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? Maybe<TResult>.FromValue((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
                : Maybe<TResult>.None;

        /// <inheritdoc cref="Select{T,TResult}(Maybe{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public Maybe<TResult> Select<TResult>([InstantHandle] Func<T, Maybe<TResult>> selector)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
                : Maybe<TResult>.None;

        /// <summary> Convert to other value type or default </summary>
        /// <param name="selector"> Converter </param>
        /// <typeparam name="TResult"> Result value type </typeparam>
        [PublicAPI, Pure]
        public TResult? SelectOrDefault<TResult>([InstantHandle] Func<T, TResult> selector)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
                : default;

        /// <inheritdoc cref="SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
        [PublicAPI, Pure]
        public TResult? SelectOrDefault<TResult>([InstantHandle] Func<T, Maybe<TResult>> selector)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault()
                : default;

        /// <summary> Convert to other value type or default </summary>
        /// <param name="selector"> Converter </param>
        /// <param name="defaultValue"> Default value </param>
        /// <typeparam name="TResult"> Result value type </typeparam>
        [PublicAPI, Pure]
        public TResult SelectOrDefault<TResult>([InstantHandle] Func<T, TResult> selector, [NoEnumeration] TResult defaultValue)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
                : defaultValue;

        /// <inheritdoc cref="SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
        [PublicAPI, Pure]
        public TResult SelectOrDefault<TResult>([InstantHandle] Func<T, Maybe<TResult>> selector, [NoEnumeration] TResult defaultValue)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault(defaultValue)
                : defaultValue;

        /// <summary> Convert to other value type or default from generator </summary>
        /// <param name="selector"> Converter </param>
        /// <param name="defaultGenerator"> Default value generator </param>
        /// <typeparam name="TResult"> Result value type </typeparam>
        [PublicAPI, Pure]
        public TResult SelectOrDefault<TResult>([InstantHandle] Func<T, TResult> selector, [InstantHandle] Func<TResult> defaultGenerator)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
                : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

        /// <inheritdoc cref="SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
        [PublicAPI, Pure]
        public TResult SelectOrDefault<TResult>([InstantHandle] Func<T, Maybe<TResult>> selector, [InstantHandle] Func<TResult> defaultGenerator)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
                                                                                 .ValueOrDefault(defaultGenerator
                                                                                                 ?? throw new ArgumentNullException(
                                                                                                     nameof(defaultGenerator)))
                : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

#endregion

#region ValueOrDefault

        /// <summary> Get value or default </summary>
        [PublicAPI, Pure]
        public T? ValueOrDefault()
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue ? maybe.Value : default;

        /// <summary> Get value or default </summary>
        /// <param name="defaultValue"> Default value </param>
        [PublicAPI, Pure]
        public T ValueOrDefault([NoEnumeration] T defaultValue)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue ? maybe.Value : defaultValue;

        /// <summary> Get value or default from generator </summary>
        /// <param name="defaultGenerator"> Default value generator </param>
        [PublicAPI, Pure]
        public T ValueOrDefault([InstantHandle] Func<T> defaultGenerator)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? maybe.Value
                : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

#endregion

#region Utils

        /// <summary> Get value form this item or other </summary>
        /// <param name="other"> Other </param>
        [PublicAPI, Pure]
        public Maybe<T> Or(Maybe<T> other)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue ? maybe : other ?? throw new ArgumentNullException(nameof(other));

        /// <summary> Get value form this item or other </summary>
        /// <param name="otherGenerator"> Other generator </param>
        [PublicAPI, Pure]
        public Maybe<T> Or([InstantHandle] Func<Maybe<T>> otherGenerator)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? maybe
                : (otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))).Invoke();

        /// <summary> Filter value </summary>
        /// <param name="filter"> Filter </param>
        [PublicAPI, Pure]
        public Maybe<T> Filter([InstantHandle] Func<T, bool> filter)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
               && (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(maybe.Value)
                ? maybe
                : Maybe<T>.None;

#endregion

#region Do

        /// <summary> Do action </summary>
        /// <param name="valueAction"> Action if value exists </param>
        /// <param name="emptyAction"> Action if empty </param>
        [PublicAPI]
        public void Do([InstantHandle] Action<T> valueAction, [InstantHandle] Action emptyAction)
        {
            if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value);
            else (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
        }

        /// <summary> Do action with additional argument </summary>
        /// <param name="valueAction"> Action if value exists </param>
        /// <param name="emptyAction"> Action if empty </param>
        /// <param name="argument"> Additional action argument </param>
        /// <typeparam name="TArgument"> Additional action argument type </typeparam>
        [PublicAPI]
        public void Do<TArgument>([InstantHandle] Action<T, TArgument> valueAction, [InstantHandle] Action<TArgument> emptyAction, TArgument argument)
        {
            if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, argument);
            else (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(argument);
        }

        /// <summary> Do action with value (if exists) </summary>
        /// <param name="valueAction"> Action if value exists </param>
        [PublicAPI]
        public void Do([InstantHandle] Action<T> valueAction)
        {
            if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value);
        }

        /// <summary> Do action with value (if exists) with additional argument </summary>
        /// <param name="valueAction"> Action if value exists </param>
        /// <param name="argument"> Additional action argument </param>
        /// <typeparam name="TArgument"> Additional action argument type </typeparam>
        [PublicAPI]
        public void Do<TArgument>([InstantHandle] Action<T, TArgument> valueAction, TArgument argument)
        {
            if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, argument);
        }

        /// <summary> Do action if empty </summary>
        /// <param name="emptyAction"> Action if empty </param>
        [PublicAPI]
        public void DoIfEmpty([InstantHandle] Action emptyAction)
        {
            if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
        }

        /// <summary> Do action if empty with additional argument </summary>
        /// <param name="emptyAction"> Action if empty </param>
        /// <param name="argument"> Additional action argument </param>
        /// <typeparam name="TArgument"> Additional action argument type </typeparam>
        [PublicAPI]
        public void DoIfEmpty<TArgument>([InstantHandle] Action<TArgument> emptyAction, TArgument argument)
        {
            if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
                (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(argument);
        }

#endregion
    }
}
