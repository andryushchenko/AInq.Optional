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

public static partial class MaybeAsync
{
#region ValueAsync

    private static async ValueTask<Maybe<T>> AwaitValue<T>(Task<T> task)
        => Maybe.Value(await task.ConfigureAwait(false));

    private static async ValueTask<Maybe<T>> AwaitValueIfNotNull<T>(Task<T?> task)
        where T : class
    {
        var result = await task.ConfigureAwait(false);
        return result is not null ? Maybe.Value(result) : Maybe.None<T>();
    }

    private static async ValueTask<Maybe<T>> AwaitValueIfNotNull<T>(Task<T?> task)
        where T : struct
    {
        var result = await task.ConfigureAwait(false);
        return result.HasValue ? Maybe.Value(result.Value) : Maybe.None<T>();
    }

    private static async ValueTask<Maybe<T>> AwaitUnwrap<T>(Task<Maybe<Maybe<T>>> maybeTask, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Unwrap();

#endregion

#region Convert

    private static async ValueTask<Either<TLeft, TRight>> AwaitOr<TLeft, TRight>(Task<Maybe<TLeft>> maybeTask, TRight other,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other);

    private static async ValueTask<Either<TLeft, TRight>> AwaitOr<TLeft, TRight>(Task<Maybe<TLeft>> maybeTask, Func<TRight> otherGenerator,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(otherGenerator);

    private static async ValueTask<Either<TLeft, TRight>> AwaitOr<TLeft, TRight>(Task<Maybe<TLeft>> maybeTask,
        Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).OrAsync(asyncOtherGenerator, cancellation).ConfigureAwait(false);

    private static async ValueTask<Try<T>> AwaitToTry<T>(Task<Maybe<T>> maybeTask, CancellationToken cancellation)
    {
        try
        {
            return (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ToTry();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<T>(ex);
        }
    }

    private static async ValueTask<Either<TLeft, TRight>> AwaitGenerator<TLeft, TRight>(
        Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation)
        => Either.Right<TLeft, TRight>(await asyncOtherGenerator.Invoke(cancellation).ConfigureAwait(false));

#endregion

#region Select

    private static async ValueTask<Maybe<TResult>> AwaitSelect<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);

    private static async ValueTask<Maybe<TResult>> AwaitSelect<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(selector);

    private static async ValueTask<Maybe<TResult>> AwaitSelect<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false);

    private static async ValueTask<Maybe<TResult>> AwaitSelect<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncSelector, cancellation).ConfigureAwait(false);

#endregion

#region SelectOrDefault

    private static async ValueTask<TResult?> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        TResult defaultValue, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, TResult> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator);

#endregion

#region SelectOrDefault_Maybe

    private static async ValueTask<TResult?> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        TResult defaultValue, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultValue);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask, Func<T, Maybe<TResult>> selector,
        Func<TResult> defaultGenerator, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefault(selector, defaultGenerator);

#endregion

#region SelectOrDefaultAsync

    private static async ValueTask<T?> AwaitNullable<T>(ValueTask<T> valueTask)
        => await valueTask.ConfigureAwait(false);

    private static async ValueTask<TResult?> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, cancellation)
                                                                                .ConfigureAwait(false);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, TResult defaultValue, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
                                                                                .ConfigureAwait(false);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, Func<TResult> defaultGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
                                                                                .ConfigureAwait(false);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<TResult>> asyncSelector, Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                 .SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
                 .ConfigureAwait(false);

#endregion

#region SelectOrDefaultAsync_Maybe

    private static async ValueTask<TResult?> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, cancellation)
                                                                                .ConfigureAwait(false);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, TResult defaultValue, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, defaultValue, cancellation)
                                                                                .ConfigureAwait(false);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, Func<TResult> defaultGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectOrDefaultAsync(asyncSelector, defaultGenerator, cancellation)
                                                                                .ConfigureAwait(false);

    private static async ValueTask<TResult> AwaitSelectOrDefault<T, TResult>(Task<Maybe<T>> maybeTask,
        Func<T, CancellationToken, ValueTask<Maybe<TResult>>> asyncSelector, Func<CancellationToken, ValueTask<TResult>> asyncDefaultGenerator,
        CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false))
                 .SelectOrDefaultAsync(asyncSelector, asyncDefaultGenerator, cancellation)
                 .ConfigureAwait(false);

#endregion

#region ValueOrDefault

    private static async ValueTask<T?> AwaitValueOrDefault<T>(Task<Maybe<T>> maybeTask, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault();

    private static async ValueTask<T> AwaitValueOrDefault<T>(Task<Maybe<T>> maybeTask, T defaultValue, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultValue);

    private static async ValueTask<T> AwaitValueOrDefault<T>(Task<Maybe<T>> maybeTask, Func<T> defaultGenerator, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefault(defaultGenerator);

    private static async ValueTask<T> AwaitValueOrDefault<T>(this Task<Maybe<T>> maybeTask,
        Func<CancellationToken, ValueTask<T>> asyncDefaultGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).ValueOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                                                .ConfigureAwait(false);

#endregion

#region Or

    private static async ValueTask<Maybe<T>> AwaitOr<T>(Task<Maybe<T>> maybeTask, Maybe<T> other, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other);

    private static async ValueTask<Maybe<T>> AwaitOr<T>(Task<Maybe<T>> maybeTask, Func<Maybe<T>> otherGenerator, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(otherGenerator);

    private static async ValueTask<Maybe<T>> AwaitOr<T>(Task<Maybe<T>> maybeTask, Func<CancellationToken, ValueTask<Maybe<T>>> asyncOtherGenerator,
        CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).OrAsync(asyncOtherGenerator, cancellation).ConfigureAwait(false);

#endregion

#region Utils

    private static async ValueTask<Maybe<T>> AwaitFilter<T>(Task<Maybe<T>> maybeTask, Func<T, bool> filter, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Filter(filter);

    private static async ValueTask<Maybe<T>> AwaitFilter<T>(Maybe<T> maybe, ValueTask<bool> filter)
        => await filter.ConfigureAwait(false) ? maybe : Maybe.None<T>();

    private static async ValueTask<Maybe<T>> AwaitFilter<T>(Task<Maybe<T>> maybeTask, Func<T, CancellationToken, ValueTask<bool>> asyncFilter,
        CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).FilterAsync(asyncFilter, cancellation).ConfigureAwait(false);

#endregion

#region Do

    private static async ValueTask AwaitDo<T>(Task<Maybe<T>> maybeTask, Action<T> valueAction, Action emptyAction, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, emptyAction);

    private static async ValueTask AwaitDo<T>(Task<Maybe<T>> maybeTask, Action<T> valueAction, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction);

    private static async ValueTask AwaitDoIfEmpty<T>(Task<Maybe<T>> maybeTask, Action emptyAction, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfEmpty(emptyAction);

#endregion

#region DoWithArgument

    private static async ValueTask AwaitDo<T, TArgument>(Task<Maybe<T>> maybeTask, Action<T, TArgument> valueAction, Action<TArgument> emptyAction,
        TArgument argument, CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, emptyAction, argument);

    private static async ValueTask AwaitDo<T, TArgument>(Task<Maybe<T>> maybeTask, Action<T, TArgument> valueAction, TArgument argument,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(valueAction, argument);

    private static async ValueTask AwaitDoIfEmpty<T, TArgument>(Task<Maybe<T>> maybeTask, Action<TArgument> emptyAction, TArgument argument,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).DoIfEmpty(emptyAction, argument);

#endregion
}
