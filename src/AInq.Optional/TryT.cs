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

public readonly struct Try<T> : IEquatable<Try<T>>, IEquatable<T>, IComparable<Try<T>>, IComparable<T>
{
    private readonly T _value;

    internal Try(T value)
    {
        _value = value;
        Exception = null;
    }

    internal Try(Exception exception)
    {
        _value = default!;
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    public bool Success => Exception == null;
    public T Value => Exception == null ? _value : throw Exception;
    internal Exception? Exception { get; }

    public override string ToString()
        => Exception == null ? _value?.ToString() ?? "Null" : Exception.ToString();

    public override int GetHashCode()
        => Exception == null ? _value?.GetHashCode() ?? 0 : 1;

    public override bool Equals(object? obj)
        => obj is Try<T> other
           && (Exception, other.Exception) switch
           {
               (not null, not null) => true,
               (null, null) => EqualityComparer<T>.Default.Equals(_value, other._value),
               _ => false
           };

    public bool Equals(Try<T> other)
        => (Exception, other.Exception) switch
        {
            (not null, not null) => true,
            (null, null) => EqualityComparer<T>.Default.Equals(_value, other._value),
            _ => false
        };

    public bool Equals(T? other)
        => Exception == null && EqualityComparer<T?>.Default.Equals(_value, other);

    public int CompareTo(Try<T> other)
        => (Exception, other.Exception) switch
        {
            (not null, not null) => 0,
            (not null, null) => -1,
            (null, not null) => 1,
            _ => Comparer<T>.Default.Compare(_value, other._value)
        };

    public int CompareTo(T? other)
        => Exception == null ? Comparer<T?>.Default.Compare(_value, other) : -1;

    public static explicit operator Try<T>(T item)
        => new(item);

    public static explicit operator Try<T>(Exception exception)
        => new(exception);

    public static explicit operator T(Try<T> item)
        => item.Value;

    public static bool operator ==(Try<T> left, Try<T> right)
        => left.Equals(right);

    public static bool operator !=(Try<T> left, Try<T> right)
        => !left.Equals(right);

    public static bool operator <(Try<T> left, Try<T> right)
        => left.CompareTo(right) < 0;

    public static bool operator >(Try<T> left, Try<T> right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(Try<T> left, Try<T> right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(Try<T> left, Try<T> right)
        => left.CompareTo(right) >= 0;

    public static bool operator ==(Try<T> left, T? right)
        => left.Equals(right);

    public static bool operator !=(Try<T> left, T? right)
        => !left.Equals(right);

    public static bool operator <(Try<T> left, T? right)
        => left.CompareTo(right) < 0;

    public static bool operator >(Try<T> left, T? right)
        => left.CompareTo(right) > 0;

    public static bool operator <=(Try<T> left, T? right)
        => left.CompareTo(right) <= 0;

    public static bool operator >=(Try<T> left, T? right)
        => left.CompareTo(right) >= 0;

    public static bool operator ==(T? left, Try<T> right)
        => right.Equals(left);

    public static bool operator !=(T? left, Try<T> right)
        => right.Equals(left);

    public static bool operator <(T? left, Try<T> right)
        => right.CompareTo(left) >= 0;

    public static bool operator >(T? left, Try<T> right)
        => right.CompareTo(left) <= 0;

    public static bool operator <=(T? left, Try<T> right)
        => right.CompareTo(left) > 0;

    public static bool operator >=(T? left, Try<T> right)
        => right.CompareTo(left) < 0;
}

}
