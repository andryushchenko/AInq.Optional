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

public static class AsyncExtension
{
    public static async Task<Try<T>> ResultAsync<T>(Func<Task<T>> generator)
    {
        try
        {
            var result = await generator.Invoke();
            return Try.Value(result);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    public static async Task<Try<T>> ResultAsync<T>(Task<T> task)
    {
        try
        {
            var result = await task;
            return Try.Value(result);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }

    public static async Task<Try<T>> UnwrapAsync<T>(Try<Task<T>> task)
    {
        if(!task.Success) return Try.Error<T>(task.Exception!);
        try
        {
            var result = await task.Value;
            return Try.Value(result);
        }
        catch (Exception ex)
        {
            return Try.Error<T>(ex);
        }
    }
}

}
