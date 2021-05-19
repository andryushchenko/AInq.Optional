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

using System;
using System.Threading.Tasks;

namespace AInq.Optional
{

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
    /// <typeparam name="T"> Source value type </typeparam>
    public static async Task DoAsync<T>(this Maybe<T> item, Func<T, Task> action)
    {
        if (item.HasValue) await action.Invoke(item.Value).ConfigureAwait(false);
    }

    /// <summary> Do asynchronous action with value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <typeparam name="T"> Source value type </typeparam>
    public static async Task DoAsync<T>(this Try<T> item, Func<T, Task> action)
    {
        if (item.Success) await action.Invoke(item.Value).ConfigureAwait(false);
    }

    /// <summary> Do asynchronous action with left value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static async Task DoLeftAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TLeft, Task> action)
    {
        if (item.HasLeft) await action.Invoke(item.Left).ConfigureAwait(false);
    }

    /// <summary> Do asynchronous action with right value (if exists) </summary>
    /// <param name="item"> Source </param>
    /// <param name="action"> Action </param>
    /// <typeparam name="TLeft"> Left value type </typeparam>
    /// <typeparam name="TRight"> Right value type </typeparam>
    public static async Task DoRightAsync<TLeft, TRight>(this Either<TLeft, TRight> item, Func<TRight, Task> action)
    {
        if (item.HasRight) await action.Invoke(item.Right).ConfigureAwait(false);
    }
}

}
