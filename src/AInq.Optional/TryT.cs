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

using System.Runtime.ExceptionServices;

namespace AInq.Optional;

/// <summary> Value-or-error type </summary>
/// <typeparam name="T"> Value type </typeparam>
public abstract class Try<T> : IEquatable<Try<T>>, IEquatable<T>
{
    /// <summary> Check if item is success </summary>
    [PublicAPI]
    public bool Success => IsSuccess();

    /// <summary> Item value (if success) </summary>
    [PublicAPI]
    public T Value => GetValue();

    /// <summary> Exception or null if success </summary>
    [PublicAPI]
    public Exception? Error => GetError()?.SourceException;

    /// <inheritdoc />
    public bool Equals(T? other)
        => Success && EqualityComparer<T?>.Default.Equals(Value, other);

    /// <inheritdoc />
    public bool Equals(Try<T>? other)
        => other is not null && Success && other.Success && EqualityComparer<T>.Default.Equals(Value, other.Value);

    /// <summary> Create Try from value </summary>
    /// <param name="value"> Value </param>
    public static Try<T> FromValue([NoEnumeration] T value)
        => new TryValue(value);

    /// <summary> Create Try from exception </summary>
    /// <param name="exception"> Exception </param>
    public static Try<T> FromError(Exception exception)
        => new TryError(exception);

    /// <summary> Create Try with error from other </summary>
    /// <param name="source"> Source item </param>
    /// <typeparam name="TSource"> Source value type </typeparam>
    /// <exception cref="ArgumentException"> Thrown when source item is success </exception>
    /// <remarks> <b> FOR INTERNAL USE ONLY </b> </remarks>
    public static Try<T> ConvertError<TSource>(Try<TSource> source)
        => new TryError(source.GetError() ?? throw new ArgumentException("Source item doesn't contain error", nameof(source)));

    /// <summary> Cast value to Try </summary>
    /// <param name="value"> Value </param>
    [PublicAPI]
    public static implicit operator Try<T>([NoEnumeration] T value)
        => FromValue(value);

    /// <summary> Cast exception to Try </summary>
    /// <param name="error"> Exception </param>
    [PublicAPI]
    public static implicit operator Try<T>(Exception error)
        => FromError(error);

    /// <summary> True if success </summary>
    /// <param name="try"> Try item </param>
    [PublicAPI]
    public static explicit operator bool(Try<T> @try)
        => @try.Success;

    /// <summary> True if error </summary>
    /// <param name="try"> Try item </param>
    [PublicAPI]
    public static bool operator !(Try<T> @try)
        => !@try.Success;

    private protected abstract bool IsSuccess();
    private protected abstract T GetValue();
    private protected abstract ExceptionDispatchInfo? GetError();

    /// <summary> Throw if contains exception </summary>
    [PublicAPI]
    public abstract Try<T> Throw();

    // <summary> Throw if contains target type exception </summary>
    /// <param name="exceptionType"> Target exception type </param>
    [PublicAPI]
    public abstract Try<T> Throw(Type exceptionType);

    /// <summary> Throw if contains target type exception </summary>
    /// <typeparam name="TException"> Target exception type </typeparam>
    [PublicAPI]
    public abstract Try<T> Throw<TException>()
        where TException : Exception;

    /// <inheritdoc />
    public override int GetHashCode()
        => Success ? Value?.GetHashCode() ?? 0 : 1;

    /// <inheritdoc />
    public override bool Equals(object? obj)
        => obj switch
        {
            Try<T> other => Equals(other),
            T value => Equals(value),
            _ => false
        };

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Try<T>? a, Try<T>? b)
        => a?.Equals(b) ?? b is null;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Try<T>? a, Try<T>? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(Try<T>? a, T? b)
        => a?.Equals(b) ?? false;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(Try<T>? a, T? b)
        => !(a == b);

    /// <summary> Equality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator ==(T? a, Try<T>? b)
        => b?.Equals(a) ?? false;

    /// <summary> Inequality comparison </summary>
    /// <param name="a"> First element </param>
    /// <param name="b"> Second element </param>
    public static bool operator !=(T? a, Try<T>? b)
        => !(a == b);

    private sealed class TryValue : Try<T>
    {
        private readonly T _value;

        internal TryValue(T value)
            => _value = value;

        private protected override bool IsSuccess()
            => true;

        private protected override T GetValue()
            => _value;

        private protected override ExceptionDispatchInfo? GetError()
            => null;

        public override Try<T> Throw()
            => this;

        public override Try<T> Throw(Type exceptionType)
            => this;

        public override Try<T> Throw<TException>()
            => this;

        /// <inheritdoc />
        public override string? ToString()
            => Value?.ToString();
    }

    private sealed class TryError : Try<T>
    {
        private readonly ExceptionDispatchInfo _error;

        internal TryError(Exception error)
            => _error = ExceptionDispatchInfo.Capture(error);

        internal TryError(ExceptionDispatchInfo error)
            => _error = error;

        private protected override bool IsSuccess()
            => false;

        private protected override T GetValue()
        {
            _error.Throw();
            return default!;
        }

        private protected override ExceptionDispatchInfo GetError()
            => _error;

        [AssertionMethod]
        public override Try<T> Throw()
        {
            _error.Throw();
            return this;
        }

        [AssertionMethod]
        public override Try<T> Throw(Type exceptionType)
        {
            if (_error.SourceException.GetType() == exceptionType) _error.Throw();
            return this;
        }

        [AssertionMethod]
        public override Try<T> Throw<TException>()
        {
            if (_error.SourceException is TException) _error.Throw();
            return this;
        }

        /// <inheritdoc />
        public override string ToString()
            => _error.SourceException.ToString();
    }
}
