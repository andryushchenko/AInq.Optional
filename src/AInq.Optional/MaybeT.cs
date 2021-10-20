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

/// <summary> Maybe monad </summary>
/// <typeparam name="T"> Value type </typeparam>
public readonly struct Maybe<T> : IEquatable<Maybe<T>>, IEquatable<T>, IComparable<Maybe<T>>, IComparable<T>
{
    private readonly T _value;

    /// <summary> Create empty <see cref="Maybe{T}" /> </summary>
    public Maybe()
    {
        _value = default!;
        HasValue = false;
    }

    /// <summary> Create <see cref="Maybe{T}" /> with value </summary>
    public Maybe(T value)
    {
        _value = value;
        HasValue = true;
    }

    /// <summary> Check if item contains value </summary>
    public bool HasValue { get; }

    /// <summary> Item value (if exists) </summary>
    public T Value => HasValue ? _value : throw new InvalidOperationException("No value");

    /// <inheritdoc />
    public override string ToString()
        => HasValue ? _value?.ToString() ?? "Null" : "None";

    /// <inheritdoc />
    public override int GetHashCode()
        => HasValue ? _value?.GetHashCode() ?? 0 : 1;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj is Maybe<T> other
           && (HasValue, other.HasValue) switch
           {
               (false, false) => true,
               (true, true) => EqualityComparer<T>.Default.Equals(_value, other._value),
               _ => false
           };

    /// <inheritdoc />
    public bool Equals(Maybe<T> other)
        => (HasValue, other.HasValue) switch
        {
            (false, false) => true,
            (true, true) => EqualityComparer<T>.Default.Equals(_value, other._value),
            _ => false
        };

    /// <inheritdoc />
    public bool Equals(T? other)
        => HasValue && EqualityComparer<T?>.Default.Equals(_value, other);

    /// <inheritdoc />
    public int CompareTo(Maybe<T> other)
        => (HasValue, other.HasValue) switch
        {
            (false, false) => 0,
            (false, true) => -1,
            (true, false) => 1,
            _ => Comparer<T>.Default.Compare(_value, other._value)
        };

    /// <inheritdoc />
    public int CompareTo(T? other)
        => HasValue ? Comparer<T?>.Default.Compare(_value, other) : -1;

    /// <summary> Explicit cast to Maybe </summary>
    /// <param name="item"> Value </param>
    public static explicit operator Maybe<T>(T item)
        => new(item);

    /// <summary> Explicit cast to value type </summary>
    /// <param name="item"> Maybe item </param>
    public static explicit operator T(Maybe<T> item)
        => item.Value;

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Maybe<T> a, Maybe<T> b)
        => a.Equals(b);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Maybe<T> a, Maybe<T> b)
        => !a.Equals(b);

    /// <summary> Less comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <(Maybe<T> a, Maybe<T> b)
        => a.CompareTo(b) < 0;

    /// <summary> Greater comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >(Maybe<T> a, Maybe<T> b)
        => a.CompareTo(b) > 0;

    /// <summary> Less or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <=(Maybe<T> a, Maybe<T> b)
        => a.CompareTo(b) <= 0;

    /// <summary> Greater or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >=(Maybe<T> a, Maybe<T> b)
        => a.CompareTo(b) >= 0;

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Maybe<T> a, T? b)
        => a.Equals(b);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Maybe<T> a, T? b)
        => !a.Equals(b);

    /// <summary> Less comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <(Maybe<T> a, T? b)
        => a.CompareTo(b) < 0;

    /// <summary> Greater comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >(Maybe<T> a, T? b)
        => a.CompareTo(b) > 0;

    /// <summary> Less or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <=(Maybe<T> a, T? b)
        => a.CompareTo(b) <= 0;

    /// <summary> Greater or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >=(Maybe<T> a, T? b)
        => a.CompareTo(b) >= 0;

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(T? a, Maybe<T> b)
        => b.Equals(a);

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(T? a, Maybe<T> b)
        => b.Equals(a);

    /// <summary> Less comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <(T? a, Maybe<T> b)
        => b.CompareTo(a) >= 0;

    /// <summary> Greater comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >(T? a, Maybe<T> b)
        => b.CompareTo(a) <= 0;

    /// <summary> Less or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator <=(T? a, Maybe<T> b)
        => b.CompareTo(a) > 0;

    /// <summary> Greater or equal comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator >=(T? a, Maybe<T> b)
        => b.CompareTo(a) < 0;
}
