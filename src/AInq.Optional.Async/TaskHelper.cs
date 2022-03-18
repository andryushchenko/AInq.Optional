﻿// Copyright 2021-2022 Anton Andryushchenko
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

internal static class TaskHelper
{
    public static ValueTask FromFunctionAsync(Func<Task> function)
        => new(function.Invoke());

    public static ValueTask<T> FromFunctionAsync<T>(Func<Task<T>> function)
        => new(function.Invoke());

#if !NET6_0_OR_GREATER
    public static Task<T> WaitAsync<T>(this Task<T> task, CancellationToken cancellation)
        => cancellation.CanBeCanceled && !task.IsCompleted
            ? cancellation.IsCancellationRequested
                ? Task.FromCanceled<T>(cancellation)
                : WaitWithCancellationAsync(task, cancellation)
            : task;

    private static async Task<T> WaitWithCancellationAsync<T>(Task<T> task, CancellationToken cancellation)
    {
        var completion = new TaskCompletionSource<T>();
#if NETSTANDARD2_0
        using var canceled = cancellation.Register(() => completion.TrySetCanceled(cancellation));
#else
        await using var canceled = cancellation.Register(() => completion.TrySetCanceled(cancellation)).ConfigureAwait(false);
#endif
        return await (await Task.WhenAny(task, completion.Task).ConfigureAwait(false)).ConfigureAwait(false);
    }
#endif
}
