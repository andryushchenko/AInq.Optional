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

/// <summary> Converter utils </summary>
public static class Converter
{
    /// <param name="either"> Either source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    extension<TLeft, TRight>(Either<TLeft, TRight> either)
    {
        /// <summary> Get left value or none </summary>
        /// <returns> Maybe with left value </returns>
        [PublicAPI, Pure]
        public Maybe<TLeft> MaybeLeft()
            => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft ? Maybe.Value(either.Left) : Maybe.None<TLeft>();

        /// <summary> Get right value or none </summary>
        /// <returns> Maybe with right value </returns>
        [PublicAPI, Pure]
        public Maybe<TRight> MaybeRight()
            => (either ?? throw new ArgumentNullException(nameof(either))).HasRight ? Maybe.Value(either.Right) : Maybe.None<TRight>();
    }

    /// <param name="either"> Either source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    /// <typeparam name="TRight"> Right source type </typeparam>
    extension<TLeft, TRight>(Either<TLeft, TRight> either)
    {
        /// <summary> Try get left value </summary>
        /// <returns> Try with left value </returns>
        [PublicAPI, Pure]
        public Try<TLeft> TryLeft()
            => (either ?? throw new ArgumentNullException(nameof(either))).HasLeft
                ? Try.Value(either.Left)
                : Try.Error<TLeft>(new InvalidOperationException("No left value"));

        /// <summary> Try get right value </summary>
        /// <returns> Try with right value </returns>
        [PublicAPI, Pure]
        public Try<TRight> TryRight()
            => (either ?? throw new ArgumentNullException(nameof(either))).HasRight
                ? Try.Value(either.Right)
                : Try.Error<TRight>(new InvalidOperationException("No right value"));
    }

    /// <param name="maybe"> Maybe source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    extension<TLeft>(Maybe<TLeft> maybe)
    {
        /// <summary> Get source value or other if empty </summary>
        /// <param name="other"> Other value </param>
        /// <typeparam name="TRight"> Right source type </typeparam>
        /// <returns> Either </returns>
        [PublicAPI, Pure]
        public Either<TLeft, TRight> Or<TRight>([NoEnumeration] TRight other)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? Either.Left<TLeft, TRight>(maybe.Value)
                : Either.Right<TLeft, TRight>(other);

        /// <summary> Get source value or other if empty </summary>
        /// <param name="otherGenerator"> Other generator </param>
        /// <typeparam name="TRight"> Right source type </typeparam>
        /// <returns> Either </returns>
        [PublicAPI, Pure]
        public Either<TLeft, TRight> Or<TRight>([InstantHandle] Func<TRight> otherGenerator)
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? Either.Left<TLeft, TRight>(maybe.Value)
                : Either.Right<TLeft, TRight>((otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))).Invoke());

        /// <summary> Convert <see cref="Maybe{T}" /> to <see cref="Try{T}" /> </summary>
        [PublicAPI, Pure]
        public Try<TLeft> AsTry()
            => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
                ? Try.Value(maybe.Value)
                : Try.Error<TLeft>(new InvalidOperationException("No value"));
    }

    /// <param name="try"> Try source </param>
    /// <typeparam name="TLeft"> Left source type </typeparam>
    extension<TLeft>(Try<TLeft> @try)
    {
        /// <summary> Get source value or other if exception </summary>
        /// <param name="other"> Other value </param>
        /// <typeparam name="TRight"> Right source type </typeparam>
        /// <returns> Either </returns>
        [PublicAPI, Pure]
        public Either<TLeft, TRight> Or<TRight>([NoEnumeration] TRight other)
            => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
                ? Either.Left<TLeft, TRight>(@try.Value)
                : Either.Right<TLeft, TRight>(other);

        /// <summary> Get source value or other if exception </summary>
        /// <param name="otherGenerator"> Other generator </param>
        /// <typeparam name="TRight"> Right source type </typeparam>
        /// <returns> Either </returns>
        [PublicAPI, Pure]
        public Either<TLeft, TRight> Or<TRight>([InstantHandle] Func<TRight> otherGenerator)
            => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
                ? Either.Left<TLeft, TRight>(@try.Value)
                : Either.Right<TLeft, TRight>((otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))).Invoke());

        /// <summary> Convert <see cref="Try{T}" /> to <see cref="Maybe{T}" /> </summary>
        [PublicAPI, Pure]
        public Maybe<TLeft> AsMaybe()
            => (@try ?? throw new ArgumentNullException(nameof(@try))).Success ? Maybe.Value(@try.Value) : Maybe.None<TLeft>();
    }
}
