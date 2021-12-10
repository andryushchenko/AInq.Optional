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

#if !NET6_0_OR_GREATER
using Nito.AsyncEx;
#endif

namespace AInq.Optional;

/// <summary> Maybe monad utils </summary>
public static class MaybeAsync
{
    /// <summary> Create Maybe from value asynchronously </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Maybe<T>> ValueAsync<T>(Task<T> value)
        => Maybe.Value(await value.ConfigureAwait(false));

    /// <inheritdoc cref="ValueAsync{T}(Task{T})" />
    public static async ValueTask<Maybe<T>> ValueAsync<T>(ValueTask<T> value)
        => Maybe.Value(await value.ConfigureAwait(false));

    /// <inheritdoc cref="ValueAsync{T}(Task{T})" />
    public static async ValueTask<Maybe<T>> AsMaybeAsync<T>(this Task<T> value)
        => Maybe.Value(await value.ConfigureAwait(false));

    /// <inheritdoc cref="ValueAsync{T}(Task{T})" />
    public static async ValueTask<Maybe<T>> AsMaybeAsync<T>(this ValueTask<T> value)
        => Maybe.Value(await value.ConfigureAwait(false));

    /// <summary> Create Maybe from value if not null asynchronously </summary>
    /// <param name="value"> Value </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> value)
        where T : class
        => await value.ConfigureAwait(false) is { } result ? Maybe.Value(result) : Maybe.None<T>();

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?})" />
    public static async ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> value)
        where T : class
        => await value.ConfigureAwait(false) is { } result ? Maybe.Value(result) : Maybe.None<T>();

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?})" />
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> value)
        where T : class
        => ValueIfNotNullAsync(value);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?})" />
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> value)
        where T : class
        => ValueIfNotNullAsync(value);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?})" />
    public static async ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> value)
        where T : struct
    {
        var result = await value.ConfigureAwait(false);
        return result.HasValue ? Maybe.Value(result.Value) : Maybe.None<T>();
    }

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?})" />
    public static async ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> value)
        where T : struct
    {
        var result = await value.ConfigureAwait(false);
        return result.HasValue ? Maybe.Value(result.Value) : Maybe.None<T>();
    }

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?})" />
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> value)
        where T : struct
        => ValueIfNotNullAsync(value);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?})" />
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> value)
        where T : struct
        => ValueIfNotNullAsync(value);

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
            ? ValueAsync((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation))
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
            ? ValueAsync((selector ?? throw new ArgumentNullException(nameof(selector))).Invoke(maybe.Value, cancellation))
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

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
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<TResult>> selector)
        => await (await maybe.ConfigureAwait(false)).SelectAsync(selector).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<TResult>> selector, CancellationToken cancellation = default)
        => await (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(selector, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<TResult>> selector)
        => await (await maybe.ConfigureAwait(false)).SelectAsync(selector).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellation = default)
        => await (await maybe.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(selector, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Task<Maybe<TResult>>> selector)
        => await (await maybe.ConfigureAwait(false)).SelectAsync(selector).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybe,
        Func<T, CancellationToken, Task<Maybe<TResult>>> selector, CancellationToken cancellation = default)
        => await (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(selector, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, ValueTask<Maybe<TResult>>> selector)
        => await (await maybe.ConfigureAwait(false)).SelectAsync(selector).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> selector, CancellationToken cancellation = default)
        => await (await maybe.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(selector, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybe, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector ?? throw new ArgumentNullException(nameof(selector)));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (await maybe.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(selector ?? throw new ArgumentNullException(nameof(selector)));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector ?? throw new ArgumentNullException(nameof(selector)));

    /// <inheritdoc cref="SelectAsync{T,TResult}(Maybe{T},Func{T,CancellationToken,Task{TResult}},CancellationToken)" />
    public static async ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybe, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation = default)
        => (await maybe.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(selector ?? throw new ArgumentNullException(nameof(selector)));

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe task </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<T?> ValueOrDefault<T>(this Task<Maybe<T>> maybe, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation)).ValueOrDefault();

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},CancellationToken)" />
    public static async ValueTask<T?> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybe, CancellationToken cancellation = default)
        => (await maybe.AsTask().WaitAsync(cancellation)).ValueOrDefault();

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe task </param>
    /// <param name="defaultValue"> Default value </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybe, T defaultValue, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation)).ValueOrDefault(defaultValue);

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},T,CancellationToken)" />
    public static async ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybe, T defaultValue, CancellationToken cancellation = default)
        => (await maybe.AsTask().WaitAsync(cancellation)).ValueOrDefault(defaultValue);

    /// <summary> Get value or default </summary>
    /// <param name="maybe"> Maybe task </param>
    /// <param name="defaultGenerator"> Default value generator </param>
    /// <param name="cancellation"> Cancellation token </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybe, Func<T> defaultGenerator, CancellationToken cancellation = default)
        => (await maybe.WaitAsync(cancellation)).ValueOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)));

    /// <inheritdoc cref="ValueOrDefault{T}(Task{Maybe{T}},Func{T},CancellationToken)" />
    public static async ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybe, Func<T> defaultGenerator,
        CancellationToken cancellation = default)
        => (await maybe.AsTask().WaitAsync(cancellation))
            .ValueOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)));

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
        var result = await maybe.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        return result.HasValue
            ? result.Value
            : await (defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke(cancellation).ConfigureAwait(false);
    }
}
