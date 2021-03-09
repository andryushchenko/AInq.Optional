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

using System;
using System.Collections.Generic;

namespace AInq.Optional
{

public readonly struct Either<TLeft, TRight> : IEquatable<Either<TLeft, TRight>>

{
    private readonly TLeft _left;
    private readonly TRight _right;

    internal Either(TLeft left)
    {
        HasLeft = true;
        _left = left;
        _right = default!;
    }

    internal Either(TRight right)
    {
        HasLeft = false;
        _left = default!;
        _right = right;
    }

    public bool HasLeft { get; }

    public bool HasRight => !HasLeft;
    public TLeft Left => HasLeft ? _left : throw new InvalidOperationException("No left value");
    public TRight Right => !HasLeft ? _right : throw new InvalidOperationException("No right value");

    public Either<TRight, TLeft> Invert()
        => HasLeft ? new Either<TRight, TLeft>(_left) : new Either<TRight, TLeft>(_right);

    public override string ToString()
        => HasLeft ? _left?.ToString() ?? "Null" : _right?.ToString() ?? "Null";

    public override int GetHashCode()
        => HasLeft ? _left?.GetHashCode() ?? 0 : _right?.GetHashCode() ?? 0;

    public override bool Equals(object obj)
        => obj is Either<TLeft, TRight> other
            ? (HasLeft, other.HasLeft) switch
            {
                (true, true) => EqualityComparer<TLeft>.Default.Equals(_left, other.Left),
                (false, false) => EqualityComparer<TRight>.Default.Equals(_right, other._right),
                _ => false
            }
            : obj is Either<TRight, TLeft> inverse
              && (HasLeft, inverse.HasLeft) switch
              {
                  (true, false) => EqualityComparer<TLeft>.Default.Equals(_left, inverse._right),
                  (false, true) => EqualityComparer<TRight>.Default.Equals(_right, inverse._left),
                  _ => false
              };

    public bool Equals(Either<TLeft, TRight> other)
        => (HasLeft, other.HasLeft) switch
        {
            (true, true) => EqualityComparer<TLeft>.Default.Equals(_left, other.Left),
            (false, false) => EqualityComparer<TRight>.Default.Equals(_right, other._right),
            _ => false
        };

    public static explicit operator Either<TLeft, TRight>(TLeft item)
        => new(item);

    public static explicit operator Either<TLeft, TRight>(TRight item)
        => new(item);

    public static explicit operator TLeft(Either<TLeft, TRight> item)
        => item.Left;

    public static explicit operator TRight(Either<TLeft, TRight> item)
        => item.Right;

    public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        => left.Equals(right);

    public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right)
        => !left.Equals(right);

    public static bool operator ==(Either<TLeft, TRight> left, Either<TRight, TLeft> right)
        => left.Equals(right.Invert());

    public static bool operator !=(Either<TLeft, TRight> left, Either<TRight, TLeft> right)
        => !left.Equals(right.Invert());

    public static bool operator ==(Either<TLeft, TRight> left, TLeft right)
        => left.HasLeft && EqualityComparer<TLeft>.Default.Equals(left._left, right);

    public static bool operator !=(Either<TLeft, TRight> left, TLeft right)
        => !left.HasLeft || !EqualityComparer<TLeft>.Default.Equals(left._left, right);

    public static bool operator ==(Either<TLeft, TRight> left, TRight right)
        => !left.HasLeft && EqualityComparer<TRight>.Default.Equals(left._right, right);

    public static bool operator !=(Either<TLeft, TRight> left, TRight right)
        => left.HasLeft || !EqualityComparer<TRight>.Default.Equals(left._right, right);

    public static bool operator ==(TLeft left, Either<TLeft, TRight> right)
        => right.HasLeft && EqualityComparer<TLeft>.Default.Equals(left, right._left);

    public static bool operator !=(TLeft left, Either<TLeft, TRight> right)
        => !right.HasLeft && !EqualityComparer<TLeft>.Default.Equals(left, right._left);

    public static bool operator ==(TRight left, Either<TLeft, TRight> right)
        => !right.HasLeft && EqualityComparer<TRight>.Default.Equals(left, right._right);

    public static bool operator !=(TRight left, Either<TLeft, TRight> right)
        => right.HasLeft || !EqualityComparer<TRight>.Default.Equals(left, right._right);
}

}
