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

/// <summary> Asynchronous Try utils </summary>
public static class AsyncExtension
{
    /// <summary> Create Try from async value generator </summary>
    /// <param name="generator"> Value generator </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async Task<Try<T>> ResultAsync<T>(Func<Task<T>> generator)
    {
        try
        {
            var result = await generator.Invoke().ConfigureAwait(false);
            return new Try<T>(result);
        }
        catch (Exception ex)
        {
            return new Try<T>(ex);
        }
    }

    /// <summary> Create Try from value task </summary>
    /// <param name="task"> Value task </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async Task<Try<T>> ResultAsync<T>(Task<T> task)
    {
        try
        {
            var result = await task.ConfigureAwait(false);
            return new Try<T>(result);
        }
        catch (Exception ex)
        {
            return new Try<T>(ex);
        }
    }

    /// <summary> Unwrap to Task </summary>
    /// <param name="task"> Try with task </param>
    /// <typeparam name="T"> Value type </typeparam>
    public static async Task<Try<T>> UnwrapAsync<T>(Try<Task<T>> task)
    {
        if (!task.Success) return Try.Error<T>(task.Error!);
        try
        {
            var result = await task.Value.ConfigureAwait(false);
            return new Try<T>(result);
        }
        catch (Exception ex)
        {
            return new Try<T>(ex);
        }
    }
}

}
