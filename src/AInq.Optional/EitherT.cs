﻿// Copyright 2021 Anton Andryushchenko
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

/// <summary> Value-either-value type </summary>
/// <typeparam name="TLeft"> Left value type </typeparam>
/// <typeparam name="TRight"> Right value type </typeparam>
public abstract class Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>
{
    /// <summary> Check if item contains left value </summary>
    [PublicAPI]
    public bool HasLeft => IsLeft();

    /// <summary> Check if item contains right value </summary>
    [PublicAPI]
    public bool HasRight => !IsLeft();

    /// <summary> Left value (if exists) </summary>
    [PublicAPI]
    public TLeft Left => IsLeft() ? GetLeft() : throw new InvalidOperationException("No left value");

    /// <summary> Right value (if exists) </summary>
    [PublicAPI]
    public TRight Right => IsLeft() ? throw new InvalidOperationException("No right value") : GetRight();

    /// <inheritdoc />
    public bool Equals(Either<TLeft, TRight>? other)
        => other is not null
           && (HasLeft, other.HasLeft) switch
           {
               (true, true) => EqualityComparer<TLeft>.Default.Equals(Left, other.Left),
               (false, false) => EqualityComparer<TRight>.Default.Equals(Right, other.Right),
               _ => false
           };

    /// <summary> Create Either from left value </summary>
    /// <param name="left"> Left value </param>
    [PublicAPI]
    public static Either<TLeft, TRight> FromLeft([NoEnumeration] TLeft left)
        => new EitherLeft(left);

    /// <summary> Create Either from right value </summary>
    /// <param name="right"> Right value </param>
    [PublicAPI]
    public static Either<TLeft, TRight> FromRight([NoEnumeration] TRight right)
        => new EitherRight(right);

    /// <summary> Cast left value to Either </summary>
    /// <param name="left"> Left value </param>
    [PublicAPI]
    public static implicit operator Either<TLeft, TRight>(TLeft left)
        => FromLeft(left);

    /// <summary> Cast right value to Either </summary>
    /// <param name="right"> Right value </param>
    [PublicAPI]
    public static implicit operator Either<TLeft, TRight>(TRight right)
        => FromRight(right);

    private protected abstract bool IsLeft();
    private protected abstract TLeft GetLeft();
    private protected abstract TRight GetRight();

    /// <inheritdoc />
    public override string? ToString()
        => HasLeft ? Left?.ToString() : Right?.ToString();

    /// <inheritdoc />
    public override int GetHashCode()
        => HasLeft ? Left?.GetHashCode() ?? 0 : Right?.GetHashCode() ?? 0;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj switch
        {
            Either<TLeft, TRight> other => Equals(other),
            Either<TRight, TLeft> invert => Equals(invert),
            TLeft left => Equals(left),
            TRight right => Equals(right),
            _ => false
        };

    /// <inheritdoc cref="Equals(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public bool Equals(Either<TRight, TLeft>? other)
        => other is not null
           && (HasLeft, other.HasRight) switch
           {
               (true, true) => EqualityComparer<TLeft>.Default.Equals(Left, other.Right),
               (false, false) => EqualityComparer<TRight>.Default.Equals(Right, other.Left),
               _ => false
           };

    /// <inheritdoc cref="Equals(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public bool Equals(TRight? other)
        => HasRight && EqualityComparer<TRight?>.Default.Equals(Right, other);

    /// <inheritdoc cref="Equals(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public bool Equals(TLeft? other)
        => HasLeft && EqualityComparer<TLeft?>.Default.Equals(Left, other);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Either<TLeft, TRight>? a, Either<TLeft, TRight>? b)
        => a?.Equals(b) ?? b is null;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Either<TLeft, TRight>? a, Either<TLeft, TRight>? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Either<TLeft, TRight>? a, Either<TRight, TLeft>? b)
        => a?.Equals(b) ?? b is null;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Either<TLeft, TRight>? a, Either<TRight, TLeft>? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Either<TLeft, TRight>? a, TLeft? b)
        => a?.Equals(b) ?? false;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Either<TLeft, TRight>? a, TLeft? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Either<TLeft, TRight>? a, TRight? b)
        => a?.Equals(b) ?? false;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Either<TLeft, TRight>? a, TRight? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(TLeft? a, Either<TLeft, TRight>? b)
        => b?.Equals(a) ?? false;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(TLeft? a, Either<TLeft, TRight>? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(TRight? a, Either<TLeft, TRight>? b)
        => b?.Equals(a) ?? false;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(TRight? a, Either<TLeft, TRight>? b)
        => !(a == b);

    private sealed class EitherLeft : Either<TLeft, TRight>
    {
        private readonly TLeft _left;

        internal EitherLeft(TLeft left)
            => _left = left;

        private protected override bool IsLeft()
            => true;

        private protected override TLeft GetLeft()
            => _left;

        private protected override TRight GetRight()
            => throw new InvalidOperationException("No right value");
    }

    private sealed class EitherRight : Either<TLeft, TRight>
    {
        private readonly TRight _right;

        internal EitherRight(TRight right)
            => _right = right;

        private protected override bool IsLeft()
            => false;

        private protected override TLeft GetLeft()
            => throw new InvalidOperationException("No left value");

        private protected override TRight GetRight()
            => _right;
    }
}
