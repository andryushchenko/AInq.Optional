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

/// <summary> Try monad </summary>
/// <typeparam name="T"> Value type </typeparam>
public readonly struct Try<T> : IEquatable<Try<T>>, IEquatable<T>, IComparable<Try<T>>, IComparable<T>
{
    private readonly T _value;

    internal Try(T value)
    {
        _value = value;
        Error = null;
    }

    internal Try(Exception exception)
    {
        _value = default!;
        Error = exception ?? throw new ArgumentNullException(nameof(exception));
    }

    /// <summary> Check if item is success </summary>
    public bool Success => Error == null;

    /// <summary> Item value (if success) </summary>
    public T Value => Error == null ? _value : throw Error;

    /// <summary> Exception or null if success </summary>
    public Exception? Error { get; }

    /// <inheritdoc />
    public override string ToString()
        => Error == null ? _value?.ToString() ?? "Null" : Error.ToString();

    /// <inheritdoc />
    public override int GetHashCode()
        => Error == null ? _value?.GetHashCode() ?? 0 : 1;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is Try<T> other
           && (Error, other.Error) switch
           {
               (not null, not null) => true,
               (null, null) => EqualityComparer<T>.Default.Equals(_value, other._value),
               _ => false
           };

    /// <inheritdoc />
    public bool Equals(Try<T> other)
        => (Error, other.Error) switch
        {
            (not null, not null) => true,
            (null, null) => EqualityComparer<T>.Default.Equals(_value, other._value),
            _ => false
        };

    /// <inheritdoc />
    public bool Equals(T? other)
        => Error == null && EqualityComparer<T?>.Default.Equals(_value, other);

    /// <inheritdoc />
    public int CompareTo(Try<T> other)
        => (Error, other.Error) switch
        {
            (not null, not null) => 0,
            (not null, null) => -1,
            (null, not null) => 1,
            _ => Comparer<T>.Default.Compare(_value, other._value)
        };

    /// <inheritdoc />
    public int CompareTo(T? other)
        => Error == null ? Comparer<T?>.Default.Compare(_value, other) : -1;

    /// <summary> Explicit cast to Try </summary>
    /// <param name="item"> Value </param>
    public static explicit operator Try<T>(T item)
        => new(item);

    /// <summary> Explicit cast to Try </summary>
    /// <param name="exception"> Exception </param>
    public static explicit operator Try<T>(Exception exception)
        => new(exception);

    /// <summary> Explicit cast to value type </summary>
    /// <param name="item"> Try item </param>
    public static explicit operator T(Try<T> item)
        => item.Value;

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Try<T> a, Try<T> b)
        => a.Equals(b);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Try<T> a, Try<T> b)
        => !a.Equals(b);

    /// <summary> Less comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <(Try<T> a, Try<T> b)
        => a.CompareTo(b) < 0;

    /// <summary> Greater comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >(Try<T> a, Try<T> b)
        => a.CompareTo(b) > 0;

    /// <summary> Less or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <=(Try<T> a, Try<T> b)
        => a.CompareTo(b) <= 0;

    /// <summary> Greater or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >=(Try<T> a, Try<T> b)
        => a.CompareTo(b) >= 0;

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Try<T> a, T? b)
        => a.Equals(b);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Try<T> a, T? b)
        => !a.Equals(b);

    /// <summary> Less comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <(Try<T> a, T? b)
        => a.CompareTo(b) < 0;

    /// <summary> Greater comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >(Try<T> a, T? b)
        => a.CompareTo(b) > 0;

    /// <summary> Less or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <=(Try<T> a, T? b)
        => a.CompareTo(b) <= 0;

    /// <summary> Greater or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >=(Try<T> a, T? b)
        => a.CompareTo(b) >= 0;

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(T? a, Try<T> b)
        => b.Equals(a);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(T? a, Try<T> b)
        => b.Equals(a);

    /// <summary> Less comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <(T? a, Try<T> b)
        => b.CompareTo(a) >= 0;

    /// <summary> Greater comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >(T? a, Try<T> b)
        => b.CompareTo(a) <= 0;

    /// <summary> Less or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <=(T? a, Try<T> b)
        => b.CompareTo(a) > 0;

    /// <summary> Greater or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >=(T? a, Try<T> b)
        => b.CompareTo(a) < 0;
}

}
