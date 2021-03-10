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

public readonly struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<T>, IComparable<Maybe<T>>, IComparable<T>
{
    private readonly T _value;

    internal Maybe(T value, bool hasValue)
    {
        _value = value;
        HasValue = hasValue;
    }

    public bool HasValue { get; }

    public T Value => HasValue ? _value : throw new InvalidOperationException("No value");

    public override string ToString()
        => HasValue ? _value?.ToString() ?? "Null" : "None";

    public override int GetHashCode()
        => HasValue ? _value?.GetHashCode() ?? 0 : 1;

    public override bool Equals(object? obj)
        => obj is Maybe<T> other
           && (HasValue, other.HasValue) switch
           {
               (false, false) => true,
               (true, true) => EqualityComparer<T>.Default.Equals(_value, other._value),
               _ => false
           };

    public bool Equals(Maybe<T> other)
        => (HasValue, other.HasValue) switch
        {
            (false, false) => true,
            (true, true) => EqualityComparer<T>.Default.Equals(_value, other._value),
            _ => false
        };

    public bool Equals(T? other)
        => HasValue && EqualityComparer<T?>.Default.Equals(_value, other);

    public int CompareTo(Maybe<T> other)
        => (HasValue, other.HasValue) switch
        {
            (false, false) => 0,
            (false, true) => -1,
            (true, false) => 1,
            _ => Comparer<T>.Default.Compare(_value, other._value)
        };

    public int CompareTo(T? other)
        => HasValue ? Comparer<T?>.Default.Compare(_value, other) : -1;

    public static explicit operator Maybe<T>(T item)
        => new(item, true);

    public static explicit operator T(Maybe<T> item)
        => item.Value;

    public static bool operator ==(Maybe<T> left, Maybe<T> right)
        => left.Equals(right);

    public static bool operator !=(Maybe<T> left, Maybe<T> right)
        => !left.Equals(right);

    public static bool operator <(Maybe<T> left, Maybe<T> right)
        => left.CompareTo(right) < 0;

    public static bool operator >(Maybe<T> left, Maybe<T> right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(Maybe<T> left, Maybe<T> right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(Maybe<T> left, Maybe<T> right)
        => left.CompareTo(right) >= 0;

    public static bool operator ==(Maybe<T> left, T? right)
        => left.Equals(right);

    public static bool operator !=(Maybe<T> left, T? right)
        => !left.Equals(right);

    public static bool operator <(Maybe<T> left, T? right)
        => left.CompareTo(right) < 0;

    public static bool operator >(Maybe<T> left, T? right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(Maybe<T> left, T? right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(Maybe<T> left, T? right)
        => left.CompareTo(right) >= 0;

    public static bool operator ==(T? left, Maybe<T> right)
        => right.Equals(left);

    public static bool operator !=(T? left, Maybe<T> right)
        => right.Equals(left);

    public static bool operator <(T? left, Maybe<T> right)
        => right.CompareTo(left) >= 0;

    public static bool operator >(T? left, Maybe<T> right)
        => right.CompareTo(left) <= 0;

    public static bool operator <=(T? left, Maybe<T> right)
        => right.CompareTo(left) > 0;

    public static bool operator >=(T? left, Maybe<T> right)
        => right.CompareTo(left) < 0;
}

}
