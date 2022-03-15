// Copyright 2021-2022 Anton Andryushchenko
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
using static AInq.Optional.TaskHelper;

namespace AInq.Optional;

/// <summary> Maybe async extension </summary>
public static class MaybeAsync
{
#region ValueAsync

    private static async ValueTask<Maybe<T>> FromTaskAsync<T>(Task<T> task)
        => Maybe.Value(await task.ConfigureAwait(false));

    private static async ValueTask<Maybe<T>> FromTaskIfNotNull<T>(Task<T?> task)
        where T : class
    {
        var result = await task.ConfigureAwait(false);
        return result is not null ? Maybe.Value(result) : Maybe.None<T>();
    }

    private static async ValueTask<Maybe<T>> FromTaskIfNotNull<T>(Task<T?> task)
        where T : struct
    {
        var result = await task.ConfigureAwait(false);
        return result.HasValue ? Maybe.Value(result.Value) : Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.Value{T}(T)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> ValueAsync<T>(Task<T> task, CancellationToken cancellation = default)
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result))
            : FromTaskAsync(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.Value{T}(T)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> ValueAsync<T>(ValueTask<T> valueTask, CancellationToken cancellation = default)
        => valueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result))
            : FromTaskAsync(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> task, CancellationToken cancellation = default)
        where T : class
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? task.Result is not null
                ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : class
        => valueTask.IsCompletedSuccessfully
            ? valueTask.Result is not null
                ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> task, CancellationToken cancellation = default)
        where T : struct
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? task.Result.HasValue
                ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result.Value))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : struct
        => valueTask.IsCompletedSuccessfully
            ? valueTask.Result.HasValue
                ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result.Value))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="ValueAsync{T}(Task{T},CancellationToken)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> AsMaybeAsync<T>(this Task<T> task, CancellationToken cancellation = default)
        => ValueAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueAsync{T}(ValueTask{T},CancellationToken)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> AsMaybeAsync<T>(this ValueTask<T> valueTask, CancellationToken cancellation = default)
        => ValueAsync(valueTask, cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?},CancellationToken)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> task, CancellationToken cancellation = default)
        where T : class
        => ValueIfNotNullAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(ValueTask{T?},CancellationToken)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : class
        => ValueIfNotNullAsync(valueTask, cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?},CancellationToken)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> task, CancellationToken cancellation = default)
        where T : struct
        => ValueIfNotNullAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(ValueTask{T?},CancellationToken)" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : struct
        => ValueIfNotNullAsync(valueTask, cancellation);

#endregion

#region Select

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TResult>>(maybeTask.Result.Select(selector))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector));

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TResult>>(maybeValueTask.Result.Select(selector))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(selector));

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TResult>>(maybeTask.Result.Select(selector))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector));

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TResult>>(maybeValueTask.Result.Select(selector))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(selector));

#endregion

#region SelectAsync

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation).AsMaybeAsync(cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(asyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(asyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(asyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectAsync(asyncSelector, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region SelectOrDefault

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<TResult?> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult?>(maybeTask.Result.SelectOrDefault(selector))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<TResult?> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult?>(maybeValueTask.Result.SelectOrDefault(selector))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, TResult defaultValue, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult>(maybeTask.Result.SelectOrDefault(selector, defaultValue))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, TResult defaultValue, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector, defaultValue))
            : FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult>(maybeTask.Result.SelectOrDefault(selector, defaultGenerator))
            : FromFunctionAsync(async ()
                => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector, defaultGenerator))
            : FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator));

#endregion

#region SelectOrDefaultAsync

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            return new ValueTask<TResult?>(default(TResult));
        var selected = (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation);
        return selected.IsCompletedSuccessfully
            ? new ValueTask<TResult?>(selected.Result)
            : FromFunctionAsync(async () => (TResult?) await selected.ConfigureAwait(false));
    }

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, TResult defaultValue,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<TResult>(defaultValue);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, Task<TResult> defaultTask,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : (defaultTask ?? throw new ArgumentNullException(nameof(defaultTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<TResult>(defaultTask.Result)
                : new ValueTask<TResult>(defaultTask.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, ValueTask<TResult> defaultValueTask,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : defaultValueTask.IsCompletedSuccessfully
                ? defaultValueTask
                : new ValueTask<TResult>(defaultValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, TResult defaultValue,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, Task<TResult> defaultTask,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector, defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, ValueTask<TResult> defaultValueTask,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector, defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, TResult defaultValue,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, Task<TResult> defaultTask,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, ValueTask<TResult> defaultValueTask,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region ValueOrDefault

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T})" />
    [PublicAPI]
    public static ValueTask<T?> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T?>(maybeTask.Result.ValueOrDefault())
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault());

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T})" />
    [PublicAPI]
    public static ValueTask<T?> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T?>(maybeValueTask.Result.ValueOrDefault())
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault());

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, T defaultValue, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T>(maybeTask.Result.ValueOrDefault(defaultValue))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultValue));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask, T defaultValue, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T>(maybeValueTask.Result.ValueOrDefault(defaultValue))
            : FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultValue));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Func<T> defaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T>(maybeTask.Result.ValueOrDefault(defaultGenerator))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultGenerator));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T> defaultGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T>(maybeValueTask.Result.ValueOrDefault(defaultGenerator))
            : FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultGenerator));

#endregion

#region ValueOrDefautAsync

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, Task<T> defaultTask, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<T>(maybe.Value)
            : (defaultTask ?? throw new ArgumentNullException(nameof(defaultTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<T>(defaultTask.Result)
                : new ValueTask<T>(defaultTask.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, ValueTask<T> defaultValueTask, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<T>(maybe.Value)
            : defaultValueTask.IsCompletedSuccessfully
                ? defaultValueTask
                : new ValueTask<T>(defaultValueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<T>(maybe.Value)
            : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybeTask, Task<T> defaultTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.ValueOrDefaultAsync(defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybeTask, ValueTask<T> defaultValueTask,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.ValueOrDefaultAsync(defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybeValueTask, Task<T> defaultTask,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.ValueOrDefaultAsync(defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybeValueTask, ValueTask<T> defaultValueTask,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.ValueOrDefaultAsync(defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region Utils

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> Or<T>(this Task<Maybe<T>> maybeTask, Maybe<T> other, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Or(other))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other));

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> Or<T>(this ValueTask<Maybe<T>> maybeValueTask, Maybe<T> other, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Or(other))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Or(other));

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> Filter<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Func<T, bool> filter,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Filter(filter))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Filter(filter));

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> Filter<T>(this ValueTask<Maybe<T>> maybeValueTask, [InstantHandle(RequireAwait = true)] Func<T, bool> filter,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Filter(filter))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Filter(filter));

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> FilterAsync<T>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> asyncFilter, CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue) return new ValueTask<Maybe<T>>(Maybe.None<T>());
        var result = (asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter))).Invoke(maybe.Value, cancellation);
        return result.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(result.Result ? maybe : Maybe.None<T>())
            : FromFunctionAsync(async () => await result.AsTask().WaitAsync(cancellation).ConfigureAwait(false) ? maybe : Maybe.None<T>());
    }

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> FilterAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> asyncFilter, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.FilterAsync(asyncFilter, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .FilterAsync(asyncFilter, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> FilterAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> asyncFilter, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.FilterAsync(asyncFilter, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .FilterAsync(asyncFilter, cancellation)
                                                  .ConfigureAwait(false));

    /// <inheritdoc cref="Maybe.Unwrap{T}(Maybe{Maybe{T}})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> Unwrap<T>(this Task<Maybe<Maybe<T>>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Unwrap())
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap());

    /// <inheritdoc cref="Maybe.Unwrap{T}(Maybe{Maybe{T}})" />
    [PublicAPI]
    public static ValueTask<Maybe<T>> Unwrap<T>(this ValueTask<Maybe<Maybe<T>>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Unwrap())
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Unwrap());

    /// <inheritdoc cref="Maybe.Values{T}(IEnumerable{Maybe{T}})" />
    [PublicAPI]
    public static async IAsyncEnumerable<T> Values<T>(this IAsyncEnumerable<Maybe<T>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var maybe in collection.WithCancellation(cancellation).ConfigureAwait(false))
            if (maybe is {HasValue: true})
                yield return maybe.Value;
    }

#endregion

#region Do

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static ValueTask Do<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        [InstantHandle(RequireAwait = true)] Action emptyAction, CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, emptyAction));
        maybeTask.Result.Do(valueAction, emptyAction);
        return default;
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static ValueTask Do<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction));
        maybeTask.Result.Do(valueAction);
        return default;
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static ValueTask DoIfEmpty<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action emptyAction,
        CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfEmpty(emptyAction));
        maybeTask.Result.DoIfEmpty(emptyAction);
        return default;
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static ValueTask Do<T>(this ValueTask<Maybe<T>> maybeValueTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        [InstantHandle(RequireAwait = true)] Action emptyAction, CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, emptyAction));
        maybeValueTask.Result.Do(valueAction, emptyAction);
        return default;
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static ValueTask Do<T>(this ValueTask<Maybe<T>> maybeValueTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction));
        maybeValueTask.Result.Do(valueAction);
        return default;
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static ValueTask DoIfEmpty<T>(this ValueTask<Maybe<T>> maybeValueTask, [InstantHandle(RequireAwait = true)] Action emptyAction,
        CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).DoIfEmpty(emptyAction));
        maybeValueTask.Result.DoIfEmpty(emptyAction);
        return default;
    }

#endregion

#region DoAsync

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Maybe<T> maybe, [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Maybe<T> maybe, [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        CancellationToken cancellation = default)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T>(this Maybe<T> maybe, [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> emptyAction,
        CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction, CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (!maybe.HasValue)
            await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> valueAction, CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (!maybe.HasValue)
            await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

#endregion
}
