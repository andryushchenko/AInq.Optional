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

/// <summary> Value processing extension </summary>
public static class ActionExtension
{
    /// <summary> Do action with value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static void Do<T>(this Maybe<T> item, Action<T> action)
    {
        if (item.HasValue) action.Invoke(item.Value);
    }

    /// <summary> Do action with value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static void Do<T>(this Try<T> item, Action<T> action)
    {
        if (item.Success) action.Invoke(item.Value);
    }

    /// <summary> Do action with left value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static void DoLeft<TLeft, TRight>(this Either<TLeft, TRight> item, Action<TLeft> action)
    {
        if (item.HasLeft) action.Invoke(item.Left);
    }

    /// <summary> Do action with right value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static void DoRight<TLeft, TRight>(this Either<TLeft, TRight> item, Action<TRight> action)
    {
        if (item.HasRight) action.Invoke(item.Right);
    }

    /// <summary> Do asynchronous action with value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <param name="cancellation"> Cancellation Token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static async Task DoAsync<T>(this Maybe<T> item, Func<T, CancellationToken, Task> action, CancellationToken cancellation = default)
    {
        if (item.HasValue) await action.Invoke(item.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Maybe{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async Task DoAsync<T>(this Maybe<T> item, Func<T, Task> action)
        => await item.DoAsync((source, _) => action.Invoke(source));

    /// <summary> Do asynchronous action with value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <param name="cancellation"> Cancellation Token </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static async Task DoAsync<T>(this Try<T> item, Func<T, CancellationToken, Task> action, CancellationToken cancellation = default)
    {
        if (item.Success) await action.Invoke(item.Value, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoAsync{T}(Try{T},Func{T,CancellationToken,Task},CancellationToken)" />
    public static async Task DoAsync<T>(this Try<T> item, Func<T, Task> action)
        => await item.DoAsync((source, _) => action.Invoke(source));

    /// <summary> Do asynchronous action with left value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <param name="cancellation"> Cancellation Token </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static async Task DoLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft, CancellationToken, Task> action,
        CancellationToken cancellation = default)
    {
        if (item.HasLeft) await action.Invoke(item.Left, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoLeftAsync{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task},CancellationToken)" />
    public static async Task DoLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft, Task> action)
        => await item.DoLeftAsync((source, _) => action.Invoke(source)).ConfigureAwait(false);

    /// <summary> Do asynchronous action with right value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <param name="cancellation"> Cancellation Token </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static async Task DoRightAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TRight, CancellationToken, Task> action,
        CancellationToken cancellation = default)
    {
        if (item.HasRight) await action.Invoke(item.Right, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="DoRightAsync{TLeft,TRight}(Either{TLeft,TRight},Func{TRight,CancellationToken,Task},CancellationToken)" />
    public static async Task DoRightAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TRight, Task> action)
        => await item.DoRightAsync((source, _) => action.Invoke(source)).ConfigureAwait(false);

    /// <summary> Do asynchronous action with left or right value </summary>
    /// <param name="item"> Source </param>
    /// <param name="leftAction"> Left value action </param>
    /// <param name="rightAction"> Right value action </param>
    /// <param name="cancellation"> Cancellation Token </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static async Task DoAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft, CancellationToken, Task> leftAction,
        Func<TRight, CancellationToken, Task> rightAction, CancellationToken cancellation = default)
    {
        if (item.HasLeft) await leftAction.Invoke(item.Left, cancellation).ConfigureAwait(false);
        else await rightAction.Invoke(item.Right, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc
    ///     cref="DoAsync{TLeft,TRight}(Either{TLeft,TRight},Func{TLeft,CancellationToken,Task},Func{TRight,CancellationToken,Task},CancellationToken)" />
    public static async Task DoAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft, Task> leftAction, Func<TRight, Task> rightAction)
        => await item.DoAsync((source, _) => leftAction.Invoke(source), (source, _) => rightAction.Invoke(source)).ConfigureAwait(false);
}
