// Copyright 2021-2023 Anton Andryushchenko
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

/// <summary> Value-or-none type </summary>
/// <typeparam name="T"> Value type </typeparam>
public abstract class Maybe<T> : IEquatable<Maybe<T>>, IEquatable<T>
{
    private static readonly Lazy<MaybeEmpty> Empty = new();

    /// <summary> Get empty Maybe </summary>
    [PublicAPI]
    public static Maybe<T> None => Empty.Value;

    /// <summary> Check if item contains value </summary>
    [PublicAPI]
    public bool HasValue => IsNotEmpty();

    /// <summary> Item value (if exists) </summary>
    [PublicAPI]
    public T Value => IsNotEmpty() ? GetValue() : throw new InvalidOperationException("No value");

    /// <inheritdoc />
    public bool Equals(Maybe<T>? other)
        => other is not null && !(HasValue ^ other.HasValue) && (!HasValue || EqualityComparer<T>.Default.Equals(Value, other.Value));

    /// <inheritdoc />
    public bool Equals(T? other)
        => HasValue && EqualityComparer<T?>.Default.Equals(Value, other);

    /// <summary> Create Maybe from value </summary>
    /// <param name="value"> Value </param>
    [PublicAPI]
    public static Maybe<T> FromValue([NoEnumeration] T value)
        => new MaybeValue(value);

    /// <summary> Cast value to Maybe </summary>
    /// <param name="value"> Value </param>
    [PublicAPI]
    public static implicit operator Maybe<T>([NoEnumeration] T value)
        => FromValue(value);

    private protected abstract bool IsNotEmpty();
    private protected abstract T GetValue();

    /// <inheritdoc />
    public override string? ToString()
        => IsNotEmpty() ? Value?.ToString() : "No value";

    /// <inheritdoc />
    public override int GetHashCode()
        => IsNotEmpty() ? Value?.GetHashCode() ?? 0 : 0;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj switch
        {
            Maybe<T> other => Equals(other),
            T value => Equals(value),
            _ => false
        };

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Maybe<T>? a, Maybe<T>? b)
        => a?.Equals(b) ?? b is null;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Maybe<T>? a, Maybe<T>? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Maybe<T>? a, T? b)
        => a?.Equals(b) ?? false;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Maybe<T>? a, T? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(T? a, Maybe<T>? b)
        => b?.Equals(a) ?? false;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(T? a, Maybe<T>? b)
        => !(a == b);

    private sealed class MaybeEmpty : Maybe<T>
    {
        private protected override bool IsNotEmpty()
            => false;

        private protected override T GetValue()
            => throw new InvalidOperationException("No value");
    }

    private sealed class MaybeValue : Maybe<T>
    {
        private readonly T _value;

        internal MaybeValue(T value)
            => _value = value;

        private protected override bool IsNotEmpty()
            => true;

        private protected override T GetValue()
            => _value;
    }
}
