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

using System.Runtime.CompilerServices;

namespace AInq.Optional;

/// <summary> Maybe async extension </summary>
public static class MaybeAsync
{
#region Value

    /// <summary> Create Maybe from value asynchronously </summary>
    /// <param name="value"> Value </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Maybe<T>> ValueAsync<T>(Task<T> value, CancellationToken cancellation = default)
        => Maybe.Value(await value.WaitAsync(cancellation).ConfigureAwait(false));

    /// <inheritdoc cref="ValueAsync{T}(Task{T}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> ValueAsync<T>(ValueTask<T> value, CancellationToken cancellation = default)
        => Maybe.Value(await value.WaitAsync(cancellation).ConfigureAwait(false));

    /// <inheritdoc cref="ValueAsync{T}(Task{T}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> AsMaybeAsync<T>(this Task<T> value, CancellationToken cancellation = default)
        => Maybe.Value(await value.WaitAsync(cancellation).ConfigureAwait(false));

    /// <inheritdoc cref="ValueAsync{T}(Task{T}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> AsMaybeAsync<T>(this ValueTask<T> value, CancellationToken cancellation = default)
        => Maybe.Value(await value.WaitAsync(cancellation).ConfigureAwait(false));

    /// <summary> Create Maybe from value if not null asynchronously </summary>
    /// <param name="value"> Value </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> value, CancellationToken cancellation = default)
        where T : class
        => await value.WaitAsync(cancellation).ConfigureAwait(false) is { } result ? Maybe.Value(result) : Maybe.None<T>();

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> value, CancellationToken cancellation = default)
        where T : class
        => await value.WaitAsync(cancellation).ConfigureAwait(false) is { } result ? Maybe.Value(result) : Maybe.None<T>();

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> value, CancellationToken cancellation = default)
        where T : class
        => await value.WaitAsync(cancellation).ConfigureAwait(false) is { } result ? Maybe.Value(result) : Maybe.None<T>();

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> value, CancellationToken cancellation = default)
        where T : class
        => await value.WaitAsync(cancellation).ConfigureAwait(false) is { } result ? Maybe.Value(result) : Maybe.None<T>();

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> value, CancellationToken cancellation = default)
        where T : struct
    {
        var result = await value.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue ? Maybe.Value(result.Value) : Maybe.None<T>();
    }

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> value, CancellationToken cancellation = default)
        where T : struct
    {
        var result = await value.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue ? Maybe.Value(result.Value) : Maybe.None<T>();
    }

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> value, CancellationToken cancellation = default)
        where T : struct
    {
        var result = await value.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue ? Maybe.Value(result.Value) : Maybe.None<T>();
    }

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?}, CancellationToken)" />
    public static async ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> value, CancellationToken cancellation = default)
        where T : struct
    {
        var result = await value.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue ? Maybe.Value(result.Value) : Maybe.None<T>();
    }

#endregion

#region Select

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybe, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector ?? throw new ArgumentNullException(nameof(selector)));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector ?? throw new ArgumentNullException(nameof(selector)));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector ?? throw new ArgumentNullException(nameof(selector)));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector ?? throw new ArgumentNullException(nameof(selector)));

#endregion

#region SelectAsync

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<TResult>> selector)
        => maybe.HasValue
            ? ValueAsync((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <summary> Convert to other value type asynchronously </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? ValueAsync((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation), cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<TResult>> selector)
        => maybe.HasValue
            ? ValueAsync((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<TResult>> selector,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? ValueAsync((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation), cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<TResult>> selector)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? Maybe.Value(await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false))
            : Maybe.None<TResult>();
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<TResult>> selector, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? Maybe.Value(await (selector ?? throw new ArgumentNullException(nameof(selector)))
                                .Invoke(result.Value, cancellation)
                                .ConfigureAwait(false))
            : Maybe.None<TResult>();
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<TResult>> selector)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? Maybe.Value(await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false))
            : Maybe.None<TResult>();
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? Maybe.Value(await (selector ?? throw new ArgumentNullException(nameof(selector)))
                                .Invoke(result.Value, cancellation)
                                .ConfigureAwait(false))
            : Maybe.None<TResult>();
    }

#endregion

#region SelectAsync_Maybe

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<Maybe<TResult>>> selector)
        => maybe.HasValue
            ? new ValueTask<Maybe<TResult>>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<Maybe<TResult>>> selector,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? new ValueTask<Maybe<TResult>>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation))
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<Maybe<TResult>>> selector)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<TResult>>> selector)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : Maybe.None<TResult>();
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<Maybe<TResult>>> selector, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : Maybe.None<TResult>();
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<Maybe<TResult>>> selector)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : Maybe.None<TResult>();
    }

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : Maybe.None<TResult>();
    }

#endregion

#region SelectOrDefault

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybe, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybe, Func<T, TResult> selector, TResult defaultValue,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, TResult> selector,
        TResult defaultValue, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybe, Func<T, TResult> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, TResult> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator);

#endregion

#region SelectOrDefault_Maybe

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        TResult defaultValue, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        TResult defaultValue, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator);

#endregion

#region SelectOrDefaultAsync

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<TResult>> selector)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)).AsNullable()
            : new ValueTask<TResult?>(default(TResult));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<TResult>> selector)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).AsNullable()
            : new ValueTask<TResult?>(default(TResult));

    /// <summary> Convert to other value type or default asynchronously </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<TResult>> selector,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation)).AsNullable()
            : new ValueTask<TResult?>(default(TResult));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<TResult>> selector,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation).AsNullable()
            : new ValueTask<TResult?>(default(TResult));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<TResult>> selector)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : default;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<TResult>> selector)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : default;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<TResult>> selector, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : default;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : default;
    }

#endregion

#region SelectOrDefaultAsync_Maybe

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<Maybe<TResult>>> selector)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault()
            : new ValueTask<TResult?>(default(TResult));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<Maybe<TResult>>> selector)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault()
            : new ValueTask<TResult?>(default(TResult));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<Maybe<TResult>>> selector,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation).ValueOrDefault(cancellation)
            : new ValueTask<TResult?>(default(TResult));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation).ValueOrDefault(cancellation)
            : new ValueTask<TResult?>(default(TResult));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<TResult>>> selector)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ValueOrDefault().ConfigureAwait(false)
            : default;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, ValueTask<Maybe<TResult>>> selector)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ValueOrDefault().ConfigureAwait(false)
            : default;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<Maybe<TResult>>> selector, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefault(cancellation)
                    .ConfigureAwait(false)
            : default;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}}, CancellationToken)" />
    public static async ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefault(cancellation)
                    .ConfigureAwait(false)
            : default;
    }

#endregion

#region SelectOrDefaultAsync_Value

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<TResult>> selector, TResult defaultValue)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<TResult>> selector, TResult defaultValue)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : new ValueTask<TResult>(defaultValue);

    /// <summary> Convert to other value type or default asynchronously </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultValue"> Default value </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<TResult>> selector,
        TResult defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation))
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<TResult>> selector,
        TResult defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<TResult>> selector,
        TResult defaultValue)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : defaultValue;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<TResult>> selector,
        TResult defaultValue)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : defaultValue;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<TResult>> selector, TResult defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : defaultValue;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<TResult>> selector, TResult defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : defaultValue;
    }

#endregion

#region SelectOrDefaultAsync_Value_Maybe

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<Maybe<TResult>>> selector,
        TResult defaultValue)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault(defaultValue)
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<Maybe<TResult>>> selector,
        TResult defaultValue)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault(defaultValue)
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<Maybe<TResult>>> selector,
        TResult defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefault(defaultValue, cancellation)
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, TResult defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefault(defaultValue, cancellation)
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<TResult>>> selector,
        TResult defaultValue)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value)
                    .ValueOrDefault(defaultValue)
                    .ConfigureAwait(false)
            : defaultValue;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, ValueTask<Maybe<TResult>>> selector, TResult defaultValue)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value)
                    .ValueOrDefault(defaultValue)
                    .ConfigureAwait(false)
            : defaultValue;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<Maybe<TResult>>> selector, TResult defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefault(defaultValue, cancellation)
                    .ConfigureAwait(false)
            : defaultValue;
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, TResult defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefault(defaultValue, cancellation)
                    .ConfigureAwait(false)
            : defaultValue;
    }

#endregion

#region SelectOrDefaultAsync_Task

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<TResult>> selector,
        Task<TResult> defaultValue)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<TResult>> selector,
        ValueTask<TResult> defaultValue)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : defaultValue;

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<TResult>> selector,
        Task<TResult> defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation))
            : new ValueTask<TResult>(defaultValue.WaitAsync(cancellation));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<TResult>> selector,
        ValueTask<TResult> defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation)
            : defaultValue.WaitAsync(cancellation);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<TResult>> selector,
        Task<TResult> defaultValue)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : await defaultValue.ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<TResult>> selector,
        ValueTask<TResult> defaultValue)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : await defaultValue.ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<TResult>> selector, Task<TResult> defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : await defaultValue.WaitAsync(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<TResult>> selector, ValueTask<TResult> defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : await defaultValue.WaitAsync(cancellation).ConfigureAwait(false);
    }

#endregion

#region SelectOrDefaultAsync_Task_Maybe

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<Maybe<TResult>>> selector,
        Task<TResult> defaultValue)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefaultAsync(defaultValue)
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<Maybe<TResult>>> selector,
        ValueTask<TResult> defaultValue)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefaultAsync(defaultValue)
            : defaultValue;

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<Maybe<TResult>>> selector,
        Task<TResult> defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefaultAsync(defaultValue, cancellation)
            : new ValueTask<TResult>(defaultValue.WaitAsync(cancellation));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, ValueTask<TResult> defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefaultAsync(defaultValue, cancellation)
            : defaultValue.WaitAsync(cancellation);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<TResult>>> selector,
        Task<TResult> defaultValue)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value)
                    .ValueOrDefaultAsync(defaultValue)
                    .ConfigureAwait(false)
            : await defaultValue.ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, ValueTask<Maybe<TResult>>> selector, ValueTask<TResult> defaultValue)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value)
                    .ValueOrDefaultAsync(defaultValue)
                    .ConfigureAwait(false)
            : await defaultValue.ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<Maybe<TResult>>> selector, Task<TResult> defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefaultAsync(defaultValue, cancellation)
                    .ConfigureAwait(false)
            : await defaultValue.WaitAsync(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T, CancellationToken,Task{TResult}},TResult, CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, ValueTask<TResult> defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefaultAsync(defaultValue, cancellation)
                    .ConfigureAwait(false)
            : await defaultValue.WaitAsync(cancellation).ConfigureAwait(false);
    }

#endregion

#region SelectOrDefaultAsync_Generator

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<TResult>> selector,
        Func<TResult> defaultGenerator)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<TResult>> selector,
        Func<TResult> defaultGenerator)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <summary> Convert to other value type or default asynchronously </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="selector"> Converter </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    /// <typeparam name="TResult"> Result value type </typeparam>
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<TResult>> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation))
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<TResult>> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<TResult>> selector,
        Func<TResult> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<TResult>> selector,
        Func<TResult> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<TResult>> selector, Func<TResult> defaultGenerator, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<TResult>> selector, Func<TResult> defaultGenerator, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();
    }

#endregion

#region SelectOrDefaultAsync_Generator_Maybe

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<Maybe<TResult>>> selector,
        Func<TResult> defaultGenerator)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault(defaultGenerator)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<Maybe<TResult>>> selector,
        Func<TResult> defaultGenerator)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefault(defaultGenerator)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<Maybe<TResult>>> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefault(defaultGenerator, cancellation)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefault(defaultGenerator, cancellation)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<TResult>>> selector,
        Func<TResult> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value)
                    .ValueOrDefault(defaultGenerator)
                    .ConfigureAwait(false)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, ValueTask<Maybe<TResult>>> selector, Func<TResult> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value)
                    .ValueOrDefault(defaultGenerator)
                    .ConfigureAwait(false)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<Maybe<TResult>>> selector, Func<TResult> defaultGenerator, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefault(defaultGenerator, cancellation)
                    .ConfigureAwait(false)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, Func<TResult> defaultGenerator, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefault(defaultGenerator, cancellation)
                    .ConfigureAwait(false)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();
    }

#endregion

#region SelectOrDefaultAsync_AsyncGenerator

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<TResult>> selector,
        Func<Task<TResult>> defaultGenerator)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value))
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<TResult>> selector,
        Func<ValueTask<TResult>> defaultGenerator)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<TResult>> selector,
        Func<CancellationToken, Task<TResult>> defaultGenerator, CancellationToken cancellation = default)
        => maybe.HasValue
            ? new ValueTask<TResult>((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation))
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<TResult>> selector,
        Func<CancellationToken, ValueTask<TResult>> defaultGenerator, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<TResult>> selector,
        Func<Task<TResult>> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<TResult>> selector,
        Func<ValueTask<TResult>> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value).ConfigureAwait(false)
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<TResult>> selector, Func<CancellationToken, Task<TResult>> defaultGenerator,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<TResult>> selector, Func<CancellationToken, ValueTask<TResult>> defaultGenerator,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation).ConfigureAwait(false);
    }

#endregion

#region SelectOrDefaultAsync_AsyncGenerator_Maybe

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, Task<Maybe<TResult>>> selector,
        Func<Task<TResult>> defaultGenerator)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefaultAsync(defaultGenerator)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, ValueTask<Maybe<TResult>>> selector,
        Func<ValueTask<TResult>> defaultGenerator)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value).ValueOrDefaultAsync(defaultGenerator)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, Task<Maybe<TResult>>> selector,
        Func<CancellationToken, Task<TResult>> defaultGenerator, CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefaultAsync(defaultGenerator, cancellation)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation));

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, Func<CancellationToken, ValueTask<TResult>> defaultGenerator,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? (selector ?? throw new ArgumentNullException(nameof(selector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefaultAsync(defaultGenerator, cancellation)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation);

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<TResult>>> selector,
        Func<Task<TResult>> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value)
                    .ValueOrDefaultAsync(defaultGenerator)
                    .ConfigureAwait(false)
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, ValueTask<Maybe<TResult>>> selector, Func<ValueTask<TResult>> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value)
                    .ValueOrDefaultAsync(defaultGenerator)
                    .ConfigureAwait(false)
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<Maybe<TResult>>> selector, Func<CancellationToken, Task<TResult>> defaultGenerator,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefaultAsync(defaultGenerator, cancellation)
                    .ConfigureAwait(false)
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="SelectOrDefaultAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},Func{TResult},CancellationToken)" />
    public static async ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, Func<CancellationToken, ValueTask<TResult>> defaultGenerator,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? await (selector ?? throw new ArgumentNullException(nameof(selector)))
                    .Invoke(result.Value, cancellation)
                    .ValueOrDefaultAsync(defaultGenerator, cancellation)
                    .ConfigureAwait(false)
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation).ConfigureAwait(false);
    }

#endregion

#region ValueOrDefault

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe task </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<T?> ValueOrDefault<T>(this Task<Maybe<T>> maybe, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault();

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},CancellationToken)" />
    public static async ValueTask<T?> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybe, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault();

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe task </param>
    /// <param name="defaultValue"> Default value </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybe, T defaultValue, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultValue);

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},T,CancellationToken)" />
    public static async ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybe, T defaultValue, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultValue);

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe task </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybe, Func<T> defaultGenerator, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false))
            .ValueOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)));

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static async ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybe, Func<T> defaultGenerator,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false))
            .ValueOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)));

#endregion

#region ValueOrDefautAsync

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},T,CancellationToken)" />
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, Task<T> defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue ? new ValueTask<T>(maybe.Value) : new ValueTask<T>(defaultValue.WaitAsync(cancellation));

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},T,CancellationToken)" />
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, ValueTask<T> defaultValue, CancellationToken cancellation = default)
        => maybe.HasValue ? new ValueTask<T>(maybe.Value) : defaultValue.WaitAsync(cancellation);

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},T,CancellationToken)" />
    public static async ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybe, Task<T> defaultValue, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? result.Value
            : await defaultValue.WaitAsync(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},T,CancellationToken)" />
    public static async ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybe, ValueTask<T> defaultValue,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? result.Value
            : await defaultValue.WaitAsync(cancellation).ConfigureAwait(false);
    }

#endregion

#region ValueOrDefautAsync_Generator

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, Func<Task<T>> defaultGenerator)
        => maybe.HasValue
            ? new ValueTask<T>(maybe.Value)
            : new ValueTask<T>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, Func<ValueTask<T>> defaultGenerator)
        => maybe.HasValue
            ? new ValueTask<T>(maybe.Value)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke();

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, Func<CancellationToken, Task<T>> defaultGenerator,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? new ValueTask<T>(maybe.Value)
            : new ValueTask<T>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation));

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, Func<CancellationToken, ValueTask<T>> defaultGenerator,
        CancellationToken cancellation = default)
        => maybe.HasValue
            ? new ValueTask<T>(maybe.Value)
            : (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation);

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static async ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybe, Func<Task<T>> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? result.Value
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static async ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybe, Func<ValueTask<T>> defaultGenerator)
    {
        var result = await maybe.ConfigureAwait(false);
        return result.HasValue
            ? result.Value
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static async ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybe, Func<CancellationToken, Task<T>> defaultGenerator,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? result.Value
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static async ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybe, Func<CancellationToken, ValueTask<T>> defaultGenerator,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? result.Value
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation).ConfigureAwait(false);
    }

#endregion

#region OrAsync

    /// <summary> Get value form this item or other asynchronously </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="other"> Other </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static ValueTask<Maybe<T>> OrAsync<T>(this Maybe<T> maybe, Task<Maybe<T>> other, CancellationToken cancellation = default)
        => maybe.HasValue ? new ValueTask<Maybe<T>>(maybe) : new ValueTask<Maybe<T>>(other.WaitAsync(cancellation));

    /// <inheritdoc cref="OrAsync{T}(Maybe{T},Task{Maybe{T}},CancellationToken)" />
    public static ValueTask<Maybe<T>> OrAsync<T>(this Maybe<T> maybe, ValueTask<Maybe<T>> other, CancellationToken cancellation = default)
        => maybe.HasValue ? new ValueTask<Maybe<T>>(maybe) : other.WaitAsync(cancellation);

    /// <inheritdoc cref="OrAsync{T}(Maybe{T},Task{Maybe{T}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> OrAsync<T>(this Task<Maybe<T>> maybe, Task<Maybe<T>> other, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue ? result : await other.WaitAsync(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="OrAsync{T}(Maybe{T},Task{Maybe{T}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> OrAsync<T>(this ValueTask<Maybe<T>> maybe, ValueTask<Maybe<T>> other,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue ? result : await other.WaitAsync(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="OrAsync{T}(Maybe{T},Task{Maybe{T}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> Or<T>(this Task<Maybe<T>> maybe, Maybe<T> other, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Or(other);

    /// <inheritdoc cref="OrAsync{T}(Maybe{T},Task{Maybe{T}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> Or<T>(this ValueTask<Maybe<T>> maybe, Maybe<T> other, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Or(other);

#endregion

#region Utils

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> FilterAsync<T>(this Maybe<T> maybe, Func<T, Task<bool>> filter)
        => !maybe.HasValue
           || maybe.HasValue
           && await (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(maybe.Value).ConfigureAwait(false)
            ? maybe
            : Maybe.None<T>();

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> FilterAsync<T>(this Maybe<T> maybe, Func<T, ValueTask<bool>> filter)
        => !maybe.HasValue
           || maybe.HasValue && await (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(maybe.Value).ConfigureAwait(false)
            ? maybe
            : Maybe.None<T>();

    /// <summary> Filter value asynchronously </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="filter"> Filter </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Maybe<T>> FilterAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, Task<bool>> filter,
        CancellationToken cancellation = default)
        => !maybe.HasValue
           || maybe.HasValue
           && await (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(maybe.Value, cancellation).ConfigureAwait(false)
            ? maybe
            : Maybe.None<T>();

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> FilterAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<bool>> filter,
        CancellationToken cancellation = default)
        => !maybe.HasValue
           || maybe.HasValue
           && await (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(maybe.Value, cancellation).ConfigureAwait(false)
            ? maybe
            : Maybe.None<T>();

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> FilterAsync<T>(this Task<Maybe<T>> maybe, Func<T, Task<bool>> filter)
    {
        var result = await maybe.ConfigureAwait(false);
        return !result.HasValue
               || result.HasValue && await (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(result.Value).ConfigureAwait(false)
            ? result
            : Maybe.None<T>();
    }

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> FilterAsync<T>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<bool>> filter)
    {
        var result = await maybe.ConfigureAwait(false);
        return !result.HasValue
               || result.HasValue && await (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(result.Value).ConfigureAwait(false)
            ? result
            : Maybe.None<T>();
    }

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> FilterAsync<T>(this Task<Maybe<T>> maybe, Func<T, CancellationToken, Task<bool>> filter,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return !result.HasValue
               || result.HasValue
               && await (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            ? result
            : Maybe.None<T>();
    }

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> FilterAsync<T>(this ValueTask<Maybe<T>> maybe, Func<T, CancellationToken, ValueTask<bool>> filter,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return !result.HasValue
               || result.HasValue
               && await (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(result.Value, cancellation).ConfigureAwait(false)
            ? result
            : Maybe.None<T>();
    }

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> Filter<T>(this Task<Maybe<T>> maybe, Func<T, bool> filter, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return !result.HasValue || result.HasValue && (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(result.Value)
            ? result
            : Maybe.None<T>();
    }

    /// <inheritdoc cref="FilterAsync{T}(Maybe{T},Func{T,CancellationToken,Task{bool}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> Filter<T>(this ValueTask<Maybe<T>> maybe, Func<T, bool> filter, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        return !result.HasValue || result.HasValue && (filter ?? throw new ArgumentNullException(nameof(filter))).Invoke(result.Value)
            ? result
            : Maybe.None<T>();
    }

    /// <summary> Unwrap nested Maybe </summary>
    /// <param name="maybe"> Maybe task </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Maybe<T>> Unwrap<T>(this Task<Maybe<Maybe<T>>> maybe, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();

    /// <inheritdoc cref="Unwrap{T}(Task{Maybe{Maybe{T}}},CancellationToken)" />
    public static async ValueTask<Maybe<T>> Unwrap<T>(this ValueTask<Maybe<Maybe<T>>> maybe, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();

    /// <inheritdoc cref="Unwrap{T}(Task{Maybe{Maybe{T}}},CancellationToken)" />
    public static ValueTask<Maybe<T>> Unwrap<T>(this Maybe<Task<Maybe<T>>> maybe, CancellationToken cancellation = default)
        => maybe.HasValue ? new ValueTask<Maybe<T>>(maybe.Value.WaitAsync(cancellation)) : new ValueTask<Maybe<T>>(Maybe.None<T>());

    /// <inheritdoc cref="Unwrap{T}(Task{Maybe{Maybe{T}}},CancellationToken)" />
    public static ValueTask<Maybe<T>> Unwrap<T>(this Maybe<ValueTask<Maybe<T>>> maybe, CancellationToken cancellation = default)
        => maybe.HasValue ? maybe.Value.WaitAsync(cancellation) : new ValueTask<Maybe<T>>(Maybe.None<T>());

    /// <summary> Select existing values </summary>
    /// <param name="collection"> Maybe collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <param name="cancellation"> Cancellation token </param>
    /// <returns> Values collection </returns>
    public static async IAsyncEnumerable<T> Values<T>(this IAsyncEnumerable<Maybe<T>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var maybe in collection.WithCancellation(cancellation).ConfigureAwait(false))
            if (maybe.HasValue)
                yield return maybe.Value;
    }

#endregion

#region DoAsync

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Maybe<T> maybe, Func<T, Task> valueAction)
    {
        if (maybe.HasValue) await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Maybe<T> maybe, Func<T, ValueTask> valueAction)
    {
        if (maybe.HasValue) await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value).ConfigureAwait(false);
    }

    /// <summary> Do async action with value (if exists) </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static async ValueTask DoAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, Task> valueAction,
        CancellationToken cancellation = default)
    {
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask> valueAction,
        CancellationToken cancellation = default)
    {
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Maybe<T> maybe, Func<T, Task> valueAction, Func<Task> emptyAction)
    {
        if (maybe.HasValue) await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Maybe<T> maybe, Func<T, ValueTask> valueAction, Func<ValueTask> emptyAction)
    {
        if (maybe.HasValue) await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke().ConfigureAwait(false);
    }

    /// <summary> Do async action </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="valueAction"> Action if value exists </param>
    /// <param name="emptyAction"> Action if empty </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static async ValueTask DoAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, Task> valueAction,
        Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask> valueAction,
        Func<CancellationToken, ValueTask> emptyAction, CancellationToken cancellation = default)
    {
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmptyAsync<T>(this Maybe<T> maybe, Func<Task> emptyAction)
    {
        if (!maybe.HasValue) await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmptyAsync<T>(this Maybe<T> maybe, Func<ValueTask> emptyAction)
    {
        if (!maybe.HasValue) await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke().ConfigureAwait(false);
    }

    /// <summary> Do async action if empty </summary>
    /// <param name="maybe"> Maybe item </param>
    /// <param name="emptyAction"> Action if empty </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static async ValueTask DoIfEmptyAsync<T>(this Maybe<T> maybe, Func<CancellationToken, Task> emptyAction,
        CancellationToken cancellation = default)
    {
        if (!maybe.HasValue) await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmptyAsync<T>(this Maybe<T> maybe, Func<CancellationToken, ValueTask> emptyAction,
        CancellationToken cancellation = default)
    {
        if (!maybe.HasValue) await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Task<Maybe<T>> maybe, Func<T, Task> valueAction)
    {
        var result = await maybe.ConfigureAwait(false);
        if (result.HasValue) await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask> valueAction)
    {
        var result = await maybe.ConfigureAwait(false);
        if (result.HasValue) await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Task<Maybe<T>> maybe, Func<T, CancellationToken, Task> valueAction,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (result.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this ValueTask<Maybe<T>> maybe, Func<T, CancellationToken, ValueTask> valueAction,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (result.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Task<Maybe<T>> maybe, Func<T, Task> valueAction, Func<Task> emptyAction)
    {
        var result = await maybe.ConfigureAwait(false);
        if (result.HasValue) await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask> valueAction, Func<ValueTask> emptyAction)
    {
        var result = await maybe.ConfigureAwait(false);
        if (result.HasValue) await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this Task<Maybe<T>> maybe, Func<T, CancellationToken, Task> valueAction,
        Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (result.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoAsync<T>(this ValueTask<Maybe<T>> maybe, Func<T, CancellationToken, ValueTask> valueAction,
        Func<CancellationToken, ValueTask> emptyAction, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (result.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmptyAsync<T>(this Task<Maybe<T>> maybe, Func<Task> emptyAction)
    {
        var result = await maybe.ConfigureAwait(false);
        if (!result.HasValue) await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmptyAsync<T>(this ValueTask<Maybe<T>> maybe, Func<ValueTask> emptyAction)
    {
        var result = await maybe.ConfigureAwait(false);
        if (!result.HasValue) await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke().ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmptyAsync<T>(this Task<Maybe<T>> maybe, Func<CancellationToken, Task> emptyAction,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (!result.HasValue) await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmptyAsync<T>(this ValueTask<Maybe<T>> maybe, Func<CancellationToken, ValueTask> emptyAction,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (!result.HasValue) await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask Do<T>(this Task<Maybe<T>> maybe, Action<T> valueAction, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (result.HasValue) (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async ValueTask Do<T>(this ValueTask<Maybe<T>> maybe, Action<T> valueAction, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (result.HasValue) (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask Do<T>(this Task<Maybe<T>> maybe, Action<T> valueAction, Action emptyAction,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (result.HasValue) (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value);
        else (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask Do<T>(this ValueTask<Maybe<T>> maybe, Action<T> valueAction, Action emptyAction,
        CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (result.HasValue) (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(result.Value);
        else (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmpty<T>(this Task<Maybe<T>> maybe, Action emptyAction, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (!result.HasValue) (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }

    /// <inheritdoc cref="DoIfEmptyAsync{T}(Maybe{T},Func{CancellationToken,Task},CancellationToken)" />
    public static async ValueTask DoIfEmpty<T>(this ValueTask<Maybe<T>> maybe, Action emptyAction, CancellationToken cancellation = default)
    {
        var result = await maybe.WaitAsync(cancellation).ConfigureAwait(false);
        if (!result.HasValue) (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke();
    }

#endregion
}
