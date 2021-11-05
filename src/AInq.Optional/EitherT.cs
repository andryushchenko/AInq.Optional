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

/// <summary> Either monad </summary>
/// <typeparam name="TLeft"> Left value type </typeparam>
/// <typeparam name="TRight"> Right value type </typeparam>
public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>

{
    private readonly TLeft _left;
    private readonly TRight _right;

    /// <summary> Create empty <see cref="Either{TLeft,TRight}" /> </summary>
    /// <remarks> DO NOT use this </remarks>
    public Either()
    {
        HasLeft = false;
        HasRight = false;
        _left = default!;
        _right = default!;
    }

    /// <summary> Create <see cref="Either{TLeft,TRight}" /> with left value </summary>
    public Either(TLeft left)
    {
        HasLeft = true;
        HasRight = false;
        _left = left;
        _right = default!;
    }

    /// <summary> Create <see cref="Either{TLeft,TRight}" /> with left value </summary>
    public Either(TRight right)
    {
        HasLeft = false;
        HasRight = true;
        _left = default!;
        _right = right;
    }

    /// <summary> Check if item contains left value </summary>
    public bool HasLeft { get; }

    /// <summary> Check if item contains right value </summary>
    public bool HasRight { get; }

    /// <summary> Left value (if exists) </summary>
    public TLeft Left => HasLeft ? _left : throw new InvalidOperationException("No left value");

    /// <summary> Right value (if exists) </summary>
    public TRight Right => HasRight ? _right : throw new InvalidOperationException("No right value");

    /// <summary> Swap left and right values </summary>
    public Either<TRight, TLeft> Invert()
        => HasLeft ? new Either<TRight, TLeft>(_left) : new Either<TRight, TLeft>(_right);

    /// <inheritdoc />
    public override string ToString()
        => HasLeft ? _left?.ToString() ?? "Null" : _right?.ToString() ?? "Null";

    /// <inheritdoc />
    public override int GetHashCode()
        => HasLeft ? _left?.GetHashCode() ?? 0 : _right?.GetHashCode() ?? 0;

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

    /// <inheritdoc />
    public bool Equals(Either<TLeft, TRight> other)
        => (HasLeft, other.HasLeft) switch
        {
            (true, true) => EqualityComparer<TLeft>.Default.Equals(_left, other._left),
            (false, false) => EqualityComparer<TRight>.Default.Equals(_right, other._right),
            _ => false
        };

    /// <inheritdoc cref="Equals(Either{TLeft,TRight})" />
    public bool Equals(Either<TRight, TLeft> other)
        => (HasLeft, other.HasRight) switch
        {
            (true, true) => EqualityComparer<TLeft>.Default.Equals(_left, other._right),
            (false, false) => EqualityComparer<TRight>.Default.Equals(_right, other._left),
            _ => false
        };

    /// <inheritdoc cref="Equals(Either{TLeft,TRight})" />
    public bool Equals(TRight? other)
        => HasRight && EqualityComparer<TRight?>.Default.Equals(_right, other);

    /// <inheritdoc cref="Equals(Either{TLeft,TRight})" />
    public bool Equals(TLeft? other)
        => HasLeft && EqualityComparer<TLeft?>.Default.Equals(_left, other);

    /// <summary> Explicit cast to Either </summary>
    /// <param name="item"> Value </param>
    public static explicit operator Either<TLeft, TRight>(TLeft item)
        => new(item);

    /// <summary> Explicit cast to Either </summary>
    /// <param name="item"> Value </param>
    public static explicit operator Either<TLeft, TRight>(TRight item)
        => new(item);

    /// <summary> Explicit cast to left value type </summary>
    /// <param name="item"> Either item </param>
    public static explicit operator TLeft(Either<TLeft, TRight> item)
        => item.Left;

    /// <summary> Explicit cast to right value type </summary>
    /// <param name="item"> Either item </param>
    public static explicit operator TRight(Either<TLeft, TRight> item)
        => item.Right;

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Either<TLeft, TRight> a, Either<TLeft, TRight> b)
        => a.Equals(b);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Either<TLeft, TRight> a, Either<TLeft, TRight> b)
        => !a.Equals(b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Either<TLeft, TRight> a, Either<TRight, TLeft> b)
        => a.Equals(b);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Either<TLeft, TRight> a, Either<TRight, TLeft> b)
        => !a.Equals(b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Either<TLeft, TRight> a, TLeft? b)
        => a.Equals(b);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Either<TLeft, TRight> a, TLeft? b)
        => !a.Equals(b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Either<TLeft, TRight> a, TRight? b)
        => a.Equals(b);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Either<TLeft, TRight> a, TRight? b)
        => !a.Equals(b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(TLeft? a, Either<TLeft, TRight> b)
        => b.Equals(a);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(TLeft? a, Either<TLeft, TRight> b)
        => !b.Equals(a);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(TRight? a, Either<TLeft, TRight> b)
        => b.Equals(a);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(TRight? a, Either<TLeft, TRight> b)
        => !b.Equals(a);
}
