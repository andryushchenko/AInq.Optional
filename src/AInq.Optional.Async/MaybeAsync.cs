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
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueAsync<T>(Task<T> task, CancellationToken cancellation = default)
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result))
            : FromTaskAsync(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.Value{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueAsync<T>(ValueTask<T> valueTask, CancellationToken cancellation = default)
        => valueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result))
            : FromTaskAsync(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> task, CancellationToken cancellation = default)
        where T : class
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? task.Result is not null
                ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : class
        => valueTask.IsCompletedSuccessfully
            ? valueTask.Result is not null
                ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> task, CancellationToken cancellation = default)
        where T : struct
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? task.Result.HasValue
                ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result.Value))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(task.WaitAsync(cancellation));

    /// <inheritdoc cref="Maybe.ValueIfNotNull{T}(T)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : struct
        => valueTask.IsCompletedSuccessfully
            ? valueTask.Result.HasValue
                ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result.Value))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(valueTask.AsTask().WaitAsync(cancellation));

    /// <inheritdoc cref="ValueAsync{T}(Task{T},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeAsync<T>(this Task<T> task, CancellationToken cancellation = default)
        => ValueAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueAsync{T}(ValueTask{T},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeAsync<T>(this ValueTask<T> valueTask, CancellationToken cancellation = default)
        => ValueAsync(valueTask, cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> task, CancellationToken cancellation = default)
        where T : class
        => ValueIfNotNullAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(ValueTask{T?},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : class
        => ValueIfNotNullAsync(valueTask, cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(Task{T?},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> task, CancellationToken cancellation = default)
        where T : struct
        => ValueIfNotNullAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    /// <inheritdoc cref="ValueIfNotNullAsync{T}(ValueTask{T?},CancellationToken)" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : struct
        => ValueIfNotNullAsync(valueTask, cancellation);

#endregion

#region Select

    private static async ValueTask<Maybe<TResult>> AwaitSelect<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TResult>>(maybeTask.Result.Select(selector) ?? throw new ArgumentNullException(nameof(selector)))
            : AwaitSelect(maybeTask, selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TResult>>(maybeValueTask.Result.Select(selector) ?? throw new ArgumentNullException(nameof(selector)))
            : AwaitSelect(maybeValueTask.AsTask(), selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    private static async ValueTask<Maybe<TResult>> AwaitSelect<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TResult>>(maybeTask.Result.Select(selector ?? throw new ArgumentNullException(nameof(selector))))
            : AwaitSelect(maybeTask, selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TResult>>(maybeValueTask.Result.Select(selector))
            : AwaitSelect(maybeValueTask.AsTask(), selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

#endregion

#region SelectAsync

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation).AsMaybeAsync(cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    private static async ValueTask<Maybe<TResult>> AwaitSelectAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectAsync(maybeTask, asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectAsync(maybeValueTask.AsTask(), asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    private static async ValueTask<Maybe<TResult>> AwaitSelectAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectAsync(maybeTask, asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

    /// <inheritdoc cref="Maybe.Select{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectAsync(maybeValueTask.AsTask(), asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

#endregion

#region SelectOrDefault

    private static async ValueTask<TResult?> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult?>(maybeTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector))))
            : AwaitSelectOrDefault(maybeTask, selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult?>(maybeValueTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector))))
            : AwaitSelectOrDefault(maybeValueTask.AsTask(), selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        TResult defaultValue, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, [NoEnumeration] TResult defaultValue,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult>(maybeTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector)), defaultValue))
            : AwaitSelectOrDefault(maybeTask, selector ?? throw new ArgumentNullException(nameof(selector)), defaultValue, cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, [NoEnumeration] TResult defaultValue,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultValue))
            : AwaitSelectOrDefault(maybeValueTask.AsTask(),
                selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultValue,
                cancellation);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult>(maybeTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitSelectOrDefault(maybeTask,
                selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TResult> selector, [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitSelectOrDefault(maybeValueTask.AsTask(),
                selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

#endregion

#region SelectOrDefault_Maybe

    private static async ValueTask<TResult?> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult?>(maybeTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector))))
            : AwaitSelectOrDefault(maybeTask, selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult?>(maybeValueTask.Result.SelectOrDefault(selector))
            : AwaitSelectOrDefault(maybeValueTask.AsTask(), selector ?? throw new ArgumentNullException(nameof(selector)), cancellation);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        TResult defaultValue, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, [NoEnumeration] TResult defaultValue,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult>(maybeTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector)), defaultValue))
            : AwaitSelectOrDefault(maybeTask, selector ?? throw new ArgumentNullException(nameof(selector)), defaultValue, cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, [NoEnumeration] TResult defaultValue,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultValue))
            : AwaitSelectOrDefault(maybeValueTask.AsTask(),
                selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultValue,
                cancellation);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<TResult>(maybeTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitSelectOrDefault(maybeTask,
                selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefault<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, Maybe<TResult>> selector, [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<TResult>(maybeValueTask.Result.SelectOrDefault(selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitSelectOrDefault(maybeValueTask.AsTask(),
                selector ?? throw new ArgumentNullException(nameof(selector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

#endregion

#region SelectOrDefaultAsync

    private static async ValueTask<T?> AwaitNullable<T>(ValueTask<T> valueTask)
        => await valueTask.ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            return new ValueTask<TResult?>(default(TResult));
        var selected = (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation);
        return selected.IsCompletedSuccessfully
            ? new ValueTask<TResult?>(selected.Result)
            : AwaitNullable(selected);
    }

    private static async ValueTask<TResult?> AwaitSelectOrDefaultAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, cancellation)
                                                                                .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectOrDefaultAsync(maybeTask, asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectOrDefaultAsync(maybeValueTask.AsTask(),
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, [NoEnumeration] TResult defaultValue,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<TResult>(defaultValue);

    private static async ValueTask<TResult> AwaitSelectOrDefaultAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, TResult defaultValue, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
                                                                                .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, [NoEnumeration] TResult defaultValue,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultValue,
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeTask,
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultValue,
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, [NoEnumeration] TResult defaultValue,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultValue,
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeValueTask.AsTask(),
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultValue,
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    private static async ValueTask<TResult> AwaitSelectOrDefaultAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, Func<TResult> defaultGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
                                                                                .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeTask,
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeValueTask.AsTask(),
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

    private static async ValueTask<TResult> AwaitSelectOrDefaultAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                 .SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
                 .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeTask,
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,TResult},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeValueTask.AsTask(),
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

#endregion

#region SelectOrDefaultAsync_Maybe

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation).ValueOrDefault(cancellation)
            : new ValueTask<TResult?>(default(TResult));

    private static async ValueTask<TResult?> AwaitSelectOrDefaultAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, cancellation)
                                                                                .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectOrDefaultAsync(maybeTask, asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult?> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)), cancellation)
            : AwaitSelectOrDefaultAsync(maybeValueTask.AsTask(),
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [NoEnumeration] TResult defaultValue, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
                                                                                       .ValueOrDefault(defaultValue, cancellation)
            : new ValueTask<TResult>(defaultValue);

    private static async ValueTask<TResult> AwaitSelectOrDefaultAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, TResult defaultValue, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
                                                                                .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [NoEnumeration] TResult defaultValue, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultValue,
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeTask,
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultValue,
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},TResult)" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [NoEnumeration] TResult defaultValue, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultValue,
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeValueTask.AsTask(),
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultValue,
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)), cancellation)
            : new ValueTask<TResult>((defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))).Invoke());

    private static async ValueTask<TResult> AwaitSelectOrDefaultAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, Func<TResult> defaultGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
                                                                                .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeTask,
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<TResult> defaultGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeValueTask.AsTask(),
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)))
              .Invoke(maybe.Value, cancellation)
              .ValueOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)), cancellation)
            : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

    private static async ValueTask<TResult> AwaitSelectOrDefaultAsync<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                 .SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
                 .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeTask,
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.SelectOrDefault{T,TResult}(Maybe{T},Func{T,Maybe{TResult}},Func{TResult})" />
    [PublicAPI, Pure]
    public static ValueTask<TResult> SelectOrDefaultAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectOrDefaultAsync(asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitSelectOrDefaultAsync(maybeValueTask.AsTask(),
                asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector)),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

#endregion

#region ValueOrDefault

    private static async ValueTask<T?> AwaitValueOrDefault<T>(Task<Maybe<T>> maybeTask, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault();

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<T?> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T?>(maybeTask.Result.ValueOrDefault())
            : AwaitValueOrDefault(maybeTask, cancellation);

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<T?> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T?>(maybeValueTask.Result.ValueOrDefault())
            : AwaitValueOrDefault(maybeValueTask.AsTask(), cancellation);

    private static async ValueTask<T> AwaitValueOrDefault<T>(Task<Maybe<T>> maybeTask, T defaultValue, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultValue);

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI, Pure]
    public static ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, [NoEnumeration] T defaultValue,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T>(maybeTask.Result.ValueOrDefault(defaultValue))
            : AwaitValueOrDefault(maybeTask, defaultValue, cancellation);

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},T)" />
    [PublicAPI, Pure]
    public static ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask, [NoEnumeration] T defaultValue,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T>(maybeValueTask.Result.ValueOrDefault(defaultValue))
            : AwaitValueOrDefault(maybeValueTask.AsTask(), defaultValue, cancellation);

    private static async ValueTask<T> AwaitValueOrDefault<T>(Task<Maybe<T>> maybeTask, Func<T> defaultGenerator, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultGenerator);

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI, Pure]
    public static ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Func<T> defaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T>(maybeTask.Result.ValueOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitValueOrDefault(maybeTask, defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)), cancellation);

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI, Pure]
    public static ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T> defaultGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T>(maybeValueTask.Result.ValueOrDefault(defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator))))
            : AwaitValueOrDefault(maybeValueTask.AsTask(),
                defaultGenerator ?? throw new ArgumentNullException(nameof(defaultGenerator)),
                cancellation);

#endregion

#region ValueOrDefautAsync

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI, Pure]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<T>(maybe.Value)
            : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

    private static async ValueTask<T> AwaitValueOrDefaultAsync<T>(this Task<Maybe<T>> maybeTask,
        Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                                                .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI, Pure]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.ValueOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitValueOrDefaultAsync(maybeTask,
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

    /// <inheritdoc cref="Maybe.ValueOrDefault{T}(Maybe{T},Func{T})" />
    [PublicAPI, Pure]
    public static ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.ValueOrDefaultAsync(asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation)
            : AwaitValueOrDefaultAsync(maybeValueTask.AsTask(),
                asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator)),
                cancellation);

#endregion

#region Or

    private static async ValueTask<Maybe<T>> AwaitOr<T>(Task<Maybe<T>> maybeTask, Maybe<T> other, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other);

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Or<T>(this Task<Maybe<T>> maybeTask, Maybe<T> other, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Or(other ?? throw new ArgumentNullException(nameof(other))))
            : AwaitOr(maybeTask, other ?? throw new ArgumentNullException(nameof(other)), cancellation);

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Or<T>(this ValueTask<Maybe<T>> maybeValueTask, Maybe<T> other, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Or(other ?? throw new ArgumentNullException(nameof(other))))
            : AwaitOr(maybeValueTask.AsTask(), other ?? throw new ArgumentNullException(nameof(other)), cancellation);

    private static async ValueTask<Maybe<T>> AwaitOr<T>(Task<Maybe<T>> maybeTask, Func<Maybe<T>> otherGenerator, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(otherGenerator);

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Func{Maybe{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Or<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Func<Maybe<T>> otherGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Or(otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))))
            : AwaitOr(maybeTask, otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator)), cancellation);

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Func{Maybe{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Or<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<Maybe<T>> otherGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Or(otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))))
            : AwaitOr(maybeValueTask.AsTask(), otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator)), cancellation);

#endregion

#region OrAsync

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> OrAsync<T>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<Maybe<T>>> asyncOtherGenerator,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<Maybe<T>>(maybe)
            : (asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator))).Invoke(cancellation);

    private static async ValueTask<Maybe<T>> AwaitOrAsync<T>(Task<Maybe<T>> maybeTask,
        Func<CancellationToken, ValueTask<Maybe<T>>> asyncOtherGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).OrAsync(asyncOtherGenerator, cancellation)
                                                                                .ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> OrAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<Maybe<T>>> asyncOtherGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.OrAsync(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation)
            : AwaitOrAsync(maybeTask, asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

    /// <inheritdoc cref="Maybe.Or{T}(Maybe{T},Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> OrAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<Maybe<T>>> asyncOtherGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.OrAsync(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation)
            : AwaitOrAsync(maybeValueTask.AsTask(),
                asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)),
                cancellation);

#endregion

#region Utils

    private static async ValueTask<Maybe<T>> AwaitFilter<T>(Task<Maybe<T>> maybeTask, Func<T, bool> filter, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Filter(filter);

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Filter<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Func<T, bool> filter,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Filter(filter ?? throw new ArgumentNullException(nameof(filter))))
            : AwaitFilter(maybeTask, filter ?? throw new ArgumentNullException(nameof(filter)), cancellation);

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Filter<T>(this ValueTask<Maybe<T>> maybeValueTask, [InstantHandle(RequireAwait = true)] Func<T, bool> filter,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Filter(filter ?? throw new ArgumentNullException(nameof(filter))))
            : AwaitFilter(maybeValueTask.AsTask(), filter ?? throw new ArgumentNullException(nameof(filter)), cancellation);

    private static async ValueTask<Maybe<T>> AwaitFiltered<T>(Maybe<T> maybe, ValueTask<bool> filter)
        => await filter.ConfigureAwait(false) ? maybe : Maybe.None<T>();

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> FilterAsync<T>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> asyncFilter, CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue) return new ValueTask<Maybe<T>>(Maybe.None<T>());
        var result = (asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter))).Invoke(maybe.Value, cancellation);
        return result.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(result.Result ? maybe : Maybe.None<T>())
            : AwaitFiltered(maybe, result);
    }

    private static async ValueTask<Maybe<T>> AwaitFilterAsync<T>(Task<Maybe<T>> maybeTask, Func<T, CancellationToken, ValueTask<bool>> asyncFilter,
        CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).FilterAsync(asyncFilter, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> FilterAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> asyncFilter, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.FilterAsync(asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter)), cancellation)
            : AwaitFilterAsync(maybeTask, asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter)), cancellation);

    /// <inheritdoc cref="Maybe.Filter{T}(Maybe{T},Func{T,bool})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> FilterAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> asyncFilter, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.FilterAsync(asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter)), cancellation)
            : AwaitFilterAsync(maybeValueTask.AsTask(), asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter)), cancellation);

    private static async ValueTask<Maybe<T>> AwaitUnwrap<T>(Task<Maybe<Maybe<T>>> maybeTask, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();

    /// <inheritdoc cref="Maybe.Unwrap{T}(Maybe{Maybe{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Unwrap<T>(this Task<Maybe<Maybe<T>>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Unwrap())
            : AwaitUnwrap(maybeTask, cancellation);

    /// <inheritdoc cref="Maybe.Unwrap{T}(Maybe{Maybe{T}})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> Unwrap<T>(this ValueTask<Maybe<Maybe<T>>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Unwrap())
            : AwaitUnwrap(maybeValueTask.AsTask(), cancellation);

#endregion

#region Linq

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

    /// <inheritdoc cref="Maybe.FirstOrNone{T}(IEnumerable{T})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> FirstOrNoneAsync<T>(this IAsyncEnumerable<Maybe<T>> collection, CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        return await enumerator.MoveNextAsync().ConfigureAwait(false) ? enumerator.Current : Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.FirstOrNone{T}(IEnumerable{T},Func{T,bool})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> FirstOrNoneAsync<T>(this IAsyncEnumerable<T> collection,
        [InstantHandle(RequireAwait = true)] Func<T, bool> filter, CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        _ = filter ?? throw new ArgumentNullException(nameof(filter));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            if (filter.Invoke(enumerator.Current))
                return enumerator.Current;
        return Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.FirstOrNone{T}(IEnumerable{T},Func{T,bool})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> FirstOrNoneAsync<T>(this IAsyncEnumerable<T> collection,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter, CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        _ = filter ?? throw new ArgumentNullException(nameof(filter));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            if (await filter.Invoke(enumerator.Current, cancellation).ConfigureAwait(false))
                return enumerator.Current;
        return Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.FirstNotNullOrNone{T}(IEnumerable{T})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> FirstNotNullOrNoneAsync<T>(this IAsyncEnumerable<T?> collection, CancellationToken cancellation = default)
        where T : class
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            if (enumerator.Current is not null)
                return enumerator.Current;
        return Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.FirstNotNullOrNone{T}(IEnumerable{T})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> FirstNotNullOrNoneAsync<T>(this IAsyncEnumerable<T?> collection, CancellationToken cancellation = default)
        where T : struct
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
            if (enumerator.Current.HasValue)
                return enumerator.Current.Value;
        return Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.SingleOrNone{T}(IEnumerable{T})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> SingleOrNoneAsync<T>(this IAsyncEnumerable<Maybe<T>> collection, CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        if (!await enumerator.MoveNextAsync().ConfigureAwait(false)) return Maybe.None<T>();
        var result = enumerator.Current;
        return await enumerator.MoveNextAsync().ConfigureAwait(false)
            ? throw new InvalidOperationException("Collection contains more than one element")
            : result;
    }

    /// <inheritdoc cref="Maybe.SingleOrNone{T}(IEnumerable{T},Func{T,bool})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> SingleOrNoneAsync<T>(this IAsyncEnumerable<T> collection,
        [InstantHandle(RequireAwait = true)] Func<T, bool> filter, CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        _ = filter ?? throw new ArgumentNullException(nameof(filter));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var result = enumerator.Current;
            if (!filter.Invoke(result)) continue;
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                if (filter.Invoke(enumerator.Current))
                    throw new InvalidOperationException("Collection contains more than one matching element");
            return result;
        }
        return Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.SingleOrNone{T}(IEnumerable{T},Func{T,bool})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> SingleOrNoneAsync<T>(this IAsyncEnumerable<T> collection,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, ValueTask<bool>> filter, CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        _ = filter ?? throw new ArgumentNullException(nameof(filter));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var result = enumerator.Current;
            if (!await filter.Invoke(result, cancellation).ConfigureAwait(false)) continue;
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                if (await filter.Invoke(enumerator.Current, cancellation).ConfigureAwait(false))
                    throw new InvalidOperationException("Collection contains more than one matching element");
            return result;
        }
        return Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.SingleNotNullOrNone{T}(IEnumerable{T})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> SingleNotNullOrNoneAsync<T>(this IAsyncEnumerable<T?> collection,
        CancellationToken cancellation = default)
        where T : class
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var result = enumerator.Current;
            if (result is null) continue;
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                if (enumerator.Current is not null)
                    throw new InvalidOperationException("Collection contains more than one not null element");
            return result;
        }
        return Maybe.None<T>();
    }

    /// <inheritdoc cref="Maybe.SingleNotNullOrNone{T}(IEnumerable{T})" />
    [PublicAPI]
    public static async ValueTask<Maybe<T>> SingleNotNullOrNoneAsync<T>(this IAsyncEnumerable<T?> collection,
        CancellationToken cancellation = default)
        where T : struct
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await using var enumerator = collection.GetAsyncEnumerator(cancellation);
        while (await enumerator.MoveNextAsync().ConfigureAwait(false))
        {
            var result = enumerator.Current;
            if (!result.HasValue) continue;
            while (await enumerator.MoveNextAsync().ConfigureAwait(false))
                if (enumerator.Current.HasValue)
                    throw new InvalidOperationException("Collection contains more than one not null element");
            return result.Value;
        }
        return Maybe.None<T>();
    }

#endregion

#region Do

    private static async ValueTask AwaitDo<T>(Task<Maybe<T>> maybeTask, Action<T> valueAction, Action emptyAction,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, emptyAction);

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static ValueTask Do<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        [InstantHandle(RequireAwait = true)] Action emptyAction, CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDo(maybeTask,
                valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)),
                cancellation);
        maybeTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
            emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)));
        return default;
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static ValueTask Do<T>(this ValueTask<Maybe<T>> maybeValueTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        [InstantHandle(RequireAwait = true)] Action emptyAction, CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return AwaitDo(maybeValueTask.AsTask(),
                valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)),
                cancellation);
        maybeValueTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
            emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)));
        return default;
    }

    private static async ValueTask AwaitDo<T>(Task<Maybe<T>> maybeTask, Action<T> valueAction, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction);

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static ValueTask Do<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDo(maybeTask, valueAction ?? throw new ArgumentNullException(nameof(valueAction)), cancellation);
        maybeTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)));
        return default;
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static ValueTask Do<T>(this ValueTask<Maybe<T>> maybeValueTask, [InstantHandle(RequireAwait = true)] Action<T> valueAction,
        CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return AwaitDo(maybeValueTask.AsTask(), valueAction ?? throw new ArgumentNullException(nameof(valueAction)), cancellation);
        maybeValueTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)));
        return default;
    }

    private static async ValueTask AwaitDoIfEmpty<T>(Task<Maybe<T>> maybeTask, Action emptyAction, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfEmpty(emptyAction);

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static ValueTask DoIfEmpty<T>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action emptyAction,
        CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDoIfEmpty(maybeTask, emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)), cancellation);
        maybeTask.Result.DoIfEmpty(emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)));
        return default;
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static ValueTask DoIfEmpty<T>(this ValueTask<Maybe<T>> maybeValueTask, [InstantHandle(RequireAwait = true)] Action emptyAction,
        CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return AwaitDoIfEmpty(maybeValueTask.AsTask(),
                emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)),
                cancellation);
        maybeValueTask.Result.DoIfEmpty(emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)));
        return default;
    }

#endregion

#region DoWithArgument

    private static async ValueTask AwaitDo<T, TArgument>(Task<Maybe<T>> maybeTask, Action<T, TArgument> valueAction,
        Action<TArgument> emptyAction,
        TArgument argument, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, emptyAction, argument);

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask Do<T, TArgument>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction,
        [InstantHandle(RequireAwait = true)] Action<TArgument> emptyAction, TArgument argument, CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDo(maybeTask,
                valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)),
                argument,
                cancellation);
        maybeTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
            emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)),
            argument);
        return default;
    }

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask Do<T, TArgument>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction, [InstantHandle(RequireAwait = true)] Action<TArgument> emptyAction,
        TArgument argument, CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return AwaitDo(maybeValueTask.AsTask(),
                valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)),
                argument,
                cancellation);
        maybeValueTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
            emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)),
            argument);
        return default;
    }

    private static async ValueTask AwaitDo<T, TArgument>(Task<Maybe<T>> maybeTask, Action<T, TArgument> valueAction, TArgument argument,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, argument);

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask Do<T, TArgument>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction,
        TArgument argument, CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDo(maybeTask, valueAction ?? throw new ArgumentNullException(nameof(valueAction)), argument, cancellation);
        maybeTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)), argument);
        return default;
    }

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask Do<T, TArgument>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Action<T, TArgument> valueAction, TArgument argument, CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return AwaitDo(maybeValueTask.AsTask(),
                valueAction ?? throw new ArgumentNullException(nameof(valueAction)),
                argument,
                cancellation);
        maybeValueTask.Result.Do(valueAction ?? throw new ArgumentNullException(nameof(valueAction)), argument);
        return default;
    }

    private static async ValueTask AwaitDoIfEmpty<T, TArgument>(Task<Maybe<T>> maybeTask, Action<TArgument> emptyAction, TArgument argument,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfEmpty(emptyAction, argument);

    /// <inheritdoc cref="Maybe.DoIfEmpty{T,TArgument}(Maybe{T},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask DoIfEmpty<T, TArgument>(this Task<Maybe<T>> maybeTask, [InstantHandle(RequireAwait = true)] Action<TArgument> emptyAction,
        TArgument argument, CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return AwaitDoIfEmpty(maybeTask, emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)), argument, cancellation);
        maybeTask.Result.DoIfEmpty(emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)), argument);
        return default;
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T,TArgument}(Maybe{T},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static ValueTask DoIfEmpty<T, TArgument>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Action<TArgument> emptyAction, TArgument argument, CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return AwaitDoIfEmpty(maybeValueTask.AsTask(),
                emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)),
                argument,
                cancellation);
        maybeValueTask.Result.DoIfEmpty(emptyAction ?? throw new ArgumentNullException(nameof(emptyAction)), argument);
        return default;
    }

#endregion

#region DoAsync

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Maybe<T> maybe, [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction, CancellationToken cancellation = default)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, cancellation)
                  .ConfigureAwait(false);
        else await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Maybe<T> maybe, [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
        CancellationToken cancellation = default)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction, CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction, CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, cancellation)
                  .ConfigureAwait(false);
        else await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction, CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction, CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (!maybe.HasValue)
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T},Action)" />
    [PublicAPI]
    public static async Task DoAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction, CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, cancellation)
                  .ConfigureAwait(false);
        else await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T}(Maybe{T},Action{T})" />
    [PublicAPI]
    public static async Task DoAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, CancellationToken, Task> asyncValueAction, CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T}(Maybe{T},Action)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, Task> asyncEmptyAction, CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (!maybe.HasValue)
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

#endregion

#region DoAsyncWithArgument

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
        [InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, argument, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction)))
                  .Invoke(argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T,TArgument}(Maybe{T},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T, TArgument>(this Maybe<T> maybe,
        [InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction)))
                  .Invoke(argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
        [InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, argument, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction)))
                  .Invoke(argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T,TArgument}(Maybe{T},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T, TArgument>(this Task<Maybe<T>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (!maybe.HasValue)
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction)))
                  .Invoke(argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction,
        [InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, argument, cancellation)
                  .ConfigureAwait(false);
        else
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction)))
                  .Invoke(argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.Do{T,TArgument}(Maybe{T},Action{T,TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoAsync<T, TArgument>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<T, TArgument, CancellationToken, Task> asyncValueAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (asyncValueAction ?? throw new ArgumentNullException(nameof(asyncValueAction)))
                  .Invoke(maybe.Value, argument, cancellation)
                  .ConfigureAwait(false);
    }

    /// <inheritdoc cref="Maybe.DoIfEmpty{T,TArgument}(Maybe{T},Action{TArgument},TArgument)" />
    [PublicAPI]
    public static async Task DoIfEmptyAsync<T, TArgument>(this ValueTask<Maybe<T>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<TArgument, CancellationToken, Task> asyncEmptyAction, TArgument argument,
        CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (!maybe.HasValue)
            await (asyncEmptyAction ?? throw new ArgumentNullException(nameof(asyncEmptyAction)))
                  .Invoke(argument, cancellation)
                  .ConfigureAwait(false);
    }

#endregion
}
