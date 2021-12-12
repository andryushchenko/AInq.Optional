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

    public static ValueTask<Maybe<T>> ValueAsync<T>(Task<T> task, CancellationToken cancellation = default)
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result))
            : FromTaskAsync(task.WaitAsync(cancellation));

    public static ValueTask<Maybe<T>> ValueAsync<T>(ValueTask<T> valueTask, CancellationToken cancellation = default)
        => valueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result))
            : FromTaskAsync(valueTask.AsTask().WaitAsync(cancellation));

    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> task, CancellationToken cancellation = default)
        where T : class
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? task.Result is not null
                ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(task.WaitAsync(cancellation));

    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : class
        => valueTask.IsCompletedSuccessfully
            ? valueTask.Result is not null
                ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(valueTask.AsTask().WaitAsync(cancellation));

    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(Task<T?> task, CancellationToken cancellation = default)
        where T : struct
        => (task ?? throw new ArgumentNullException(nameof(task))).Status is TaskStatus.RanToCompletion
            ? task.Result.HasValue
                ? new ValueTask<Maybe<T>>(Maybe.Value(task.Result.Value))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(task.WaitAsync(cancellation));

    public static ValueTask<Maybe<T>> ValueIfNotNullAsync<T>(ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : struct
        => valueTask.IsCompletedSuccessfully
            ? valueTask.Result.HasValue
                ? new ValueTask<Maybe<T>>(Maybe.Value(valueTask.Result.Value))
                : new ValueTask<Maybe<T>>(Maybe.None<T>())
            : FromTaskIfNotNull(valueTask.AsTask().WaitAsync(cancellation));

    public static ValueTask<Maybe<T>> AsMaybeAsync<T>(this Task<T> task, CancellationToken cancellation = default)
        => ValueAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    public static ValueTask<Maybe<T>> AsMaybeAsync<T>(this ValueTask<T> valueTask, CancellationToken cancellation = default)
        => ValueAsync(valueTask, cancellation);

    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> task, CancellationToken cancellation = default)
        where T : class
        => ValueIfNotNullAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : class
        => ValueIfNotNullAsync(valueTask, cancellation);

    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this Task<T?> task, CancellationToken cancellation = default)
        where T : struct
        => ValueIfNotNullAsync(task ?? throw new ArgumentNullException(nameof(task)), cancellation);

    public static ValueTask<Maybe<T>> AsMaybeIfNotNullAsync<T>(this ValueTask<T?> valueTask, CancellationToken cancellation = default)
        where T : struct
        => ValueIfNotNullAsync(valueTask, cancellation);

#endregion

#region Select

    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TResult>>(maybeTask.Result.Select(selector))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector));

    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask, Func<T, TResult> selector,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TResult>>(maybeValueTask.Result.Select(selector))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(selector));

    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TResult>>(maybeTask.Result.Select(selector))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector));

    public static ValueTask<Maybe<TResult>> Select<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TResult>>(maybeValueTask.Result.Select(selector))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Select(selector));

#endregion

#region SelectAsync

    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<TResult>> asyncSelector,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation).AsMaybeAsync(cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async ()
                => await (await maybeTask.ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false));

    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async ()
                => await (await maybeValueTask.ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false));

    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Maybe<T> maybe,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? (asyncSelector ?? throw new ArgumentNullException(nameof(asyncSelector))).Invoke(maybe.Value, cancellation)
            : new ValueTask<Maybe<TResult>>(Maybe.None<TResult>());

    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.SelectAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async ()
                => await (await maybeTask.ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false));

    public static ValueTask<Maybe<TResult>> SelectAsync<T, TResult>(this ValueTask<Maybe<T>> maybeValueTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.SelectAsync(asyncSelector, cancellation)
            : FromFunctionAsync(async ()
                => await (await maybeValueTask.ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false));

#endregion

#region ValueOrDefault

    public static ValueTask<T?> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T?>(maybeTask.Result.ValueOrDefault())
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault());

    public static ValueTask<T?> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T?>(maybeValueTask.Result.ValueOrDefault())
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault());

    public static ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, T defaultValue, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T>(maybeTask.Result.ValueOrDefault(defaultValue))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultValue));

    public static ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask, T defaultValue, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T>(maybeValueTask.Result.ValueOrDefault(defaultValue))
            : FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultValue));

    public static ValueTask<T> ValueOrDefault<T>(this Task<Maybe<T>> maybeTask, Func<T> defaultGenerator, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<T>(maybeTask.Result.ValueOrDefault(defaultGenerator))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultGenerator));

    public static ValueTask<T> ValueOrDefault<T>(this ValueTask<Maybe<T>> maybeValueTask, Func<T> defaultGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<T>(maybeValueTask.Result.ValueOrDefault(defaultGenerator))
            : FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultGenerator));

#endregion

#region ValueOrDefautAsync

    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, Task<T> defaultTask, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<T>(maybe.Value)
            : (defaultTask ?? throw new ArgumentNullException(nameof(defaultTask))).Status is TaskStatus.RanToCompletion
                ? new ValueTask<T>(defaultTask.Result)
                : new ValueTask<T>(defaultTask.WaitAsync(cancellation));

    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, ValueTask<T> defaultValueTask, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<T>(maybe.Value)
            : defaultValueTask.IsCompletedSuccessfully
                ? defaultValueTask
                : new ValueTask<T>(defaultValueTask.AsTask().WaitAsync(cancellation));

    public static ValueTask<T> ValueOrDefaultAsync<T>(this Maybe<T> maybe, Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<T>(maybe.Value)
            : (asyncDefaultGenerator ?? throw new ArgumentNullException(nameof(asyncDefaultGenerator))).Invoke(cancellation);

    public static ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybeTask, Task<T> defaultTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.ValueOrDefaultAsync(defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    public static ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybeTask, ValueTask<T> defaultValueTask,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.ValueOrDefaultAsync(defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    public static ValueTask<T> ValueOrDefaultAsync<T>(this Task<Maybe<T>> maybeTask, Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                  .ConfigureAwait(false));

    public static ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybeValueTask, Task<T> defaultTask,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.ValueOrDefaultAsync(defaultTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(defaultTask, cancellation)
                                                  .ConfigureAwait(false));

    public static ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybeValueTask, ValueTask<T> defaultValueTask,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.ValueOrDefaultAsync(defaultValueTask, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(defaultValueTask, cancellation)
                                                  .ConfigureAwait(false));

    public static ValueTask<T> ValueOrDefaultAsync<T>(this ValueTask<Maybe<T>> maybeValueTask,
        Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                  .ConfigureAwait(false));

#endregion

#region Utils

    public static ValueTask<Maybe<T>> Or<T>(this Task<Maybe<T>> maybeTask, Maybe<T> other, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Or(other))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other));

    public static ValueTask<Maybe<T>> Or<T>(this ValueTask<Maybe<T>> maybeValueTask, Maybe<T> other, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Or(other))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Or(other));

    public static ValueTask<Maybe<T>> Filter<T>(this Task<Maybe<T>> maybeTask, Func<T, bool> filter, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Filter(filter))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Filter(filter));

    public static ValueTask<Maybe<T>> Filter<T>(this ValueTask<Maybe<T>> maybeValueTask, Func<T, bool> filter,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Filter(filter))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Filter(filter));

    public static ValueTask<Maybe<T>> FilterAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, ValueTask<bool>> asyncFilter,
        CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue) return new ValueTask<Maybe<T>>(Maybe.None<T>());
        var result = (asyncFilter ?? throw new ArgumentNullException(nameof(asyncFilter))).Invoke(maybe.Value, cancellation);
        return result.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(result.Result ? maybe : Maybe.None<T>())
            : FromFunctionAsync(async () => await result.AsTask().WaitAsync(cancellation).ConfigureAwait(false) ? maybe : Maybe.None<T>());
    }

    public static ValueTask<Maybe<T>> FilterAsync<T>(this Task<Maybe<T>> maybeTask, Func<T, CancellationToken, ValueTask<bool>> asyncFilter,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.FilterAsync(asyncFilter, cancellation)
            : FromFunctionAsync(async () => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                                                  .FilterAsync(asyncFilter, cancellation)
                                                  .ConfigureAwait(false));

    public static ValueTask<Maybe<T>> FilterAsync<T>(this ValueTask<Maybe<T>> maybeValueTask, Func<T, CancellationToken, ValueTask<bool>> asyncFilter,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.FilterAsync(asyncFilter, cancellation)
            : FromFunctionAsync(async () => await (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false))
                                                  .FilterAsync(asyncFilter, cancellation)
                                                  .ConfigureAwait(false));

    public static ValueTask<Maybe<T>> Unwrap<T>(this Task<Maybe<Maybe<T>>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(maybeTask.Result.Unwrap())
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap());

    public static ValueTask<Maybe<T>> Unwrap<T>(this ValueTask<Maybe<Maybe<T>>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(maybeValueTask.Result.Unwrap())
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Unwrap());

    public static async IAsyncEnumerable<T> Values<T>(this IAsyncEnumerable<Maybe<T>> collection,
        [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        await foreach (var maybe in collection.WithCancellation(cancellation).ConfigureAwait(false))
            // ReSharper disable once ConstantConditionalAccessQualifier
            if (maybe?.HasValue ?? false)
                yield return maybe.Value;
    }

#endregion

#region Do

    public static ValueTask Do<T>(this Task<Maybe<T>> maybeTask, Action<T> valueAction, Action emptyAction, CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, emptyAction));
        maybeTask.Result.Do(valueAction, emptyAction);
        return default;
    }

    public static ValueTask Do<T>(this Task<Maybe<T>> maybeTask, Action<T> valueAction, CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction));
        maybeTask.Result.Do(valueAction);
        return default;
    }

    public static ValueTask DoIfEmpty<T>(this Task<Maybe<T>> maybeTask, Action emptyAction, CancellationToken cancellation = default)
    {
        if ((maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is not TaskStatus.RanToCompletion)
            return FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfEmpty(emptyAction));
        maybeTask.Result.DoIfEmpty(emptyAction);
        return default;
    }

    public static ValueTask Do<T>(this ValueTask<Maybe<T>> maybeValueTask, Action<T> valueAction, Action emptyAction,
        CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, emptyAction));
        maybeValueTask.Result.Do(valueAction, emptyAction);
        return default;
    }

    public static ValueTask Do<T>(this ValueTask<Maybe<T>> maybeValueTask, Action<T> valueAction, CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction));
        maybeValueTask.Result.Do(valueAction);
        return default;
    }

    public static ValueTask DoIfEmpty<T>(this ValueTask<Maybe<T>> maybeValueTask, Action emptyAction, CancellationToken cancellation = default)
    {
        if (!maybeValueTask.IsCompletedSuccessfully)
            return FromFunctionAsync(async ()
                => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).DoIfEmpty(emptyAction));
        maybeValueTask.Result.DoIfEmpty(emptyAction);
        return default;
    }

#endregion

#region DoAsync

    public static async Task DoAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, Task> valueAction, Func<CancellationToken, Task> emptyAction,
        CancellationToken cancellation = default)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this Maybe<T> maybe, Func<T, CancellationToken, Task> valueAction, CancellationToken cancellation = default)
    {
        if ((maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
    }

    public static async Task DoIfEmptyAsync<T>(this Maybe<T> maybe, Func<CancellationToken, Task> emptyAction,
        CancellationToken cancellation = default)
    {
        if (!(maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue)
            await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this Task<Maybe<T>> maybeTask, Func<T, CancellationToken, Task> valueAction,
        Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this Task<Maybe<T>> maybeTask, Func<T, CancellationToken, Task> valueAction,
        CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
    }

    public static async Task DoIfEmptyAsync<T>(this Task<Maybe<T>> maybeTask, Func<CancellationToken, Task> emptyAction,
        CancellationToken cancellation = default)
    {
        var maybe = (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result
            : await maybeTask.WaitAsync(cancellation).ConfigureAwait(false);
        if (!maybe.HasValue)
            await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this ValueTask<Maybe<T>> maybeValueTask, Func<T, CancellationToken, Task> valueAction,
        Func<CancellationToken, Task> emptyAction, CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
        else await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

    public static async Task DoAsync<T>(this ValueTask<Maybe<T>> maybeValueTask, Func<T, CancellationToken, Task> valueAction,
        CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (maybe.HasValue)
            await (valueAction ?? throw new ArgumentNullException(nameof(valueAction))).Invoke(maybe.Value, cancellation).ConfigureAwait(false);
    }

    public static async Task DoIfEmptyAsync<T>(this ValueTask<Maybe<T>> maybeValueTask, Func<CancellationToken, Task> emptyAction,
        CancellationToken cancellation = default)
    {
        var maybe = maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result
            : await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false);
        if (!maybe.HasValue)
            await (emptyAction ?? throw new ArgumentNullException(nameof(emptyAction))).Invoke(cancellation).ConfigureAwait(false);
    }

#endregion
}
