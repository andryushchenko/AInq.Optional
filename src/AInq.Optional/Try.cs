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

namespace AInq.Optional
{

public static class Try
{
    public static Try<T> Value<T>(T value)
        => new(value);

    public static Try<T> Error<T>(Exception ex)
        => new(ex);

    public static Try<T> Result<T>(Func<T> generator)
    {
        try
        {
            return Value(generator.Invoke());
        }
        catch (Exception ex)
        {
            return Error<T>(ex);
        }
    }

    public static Try<TResult> Select<T, TResult>(this Try<T> item, Func<T, TResult> selector)
        => item.Success ? Result(() => selector.Invoke(item.Value)) : Error<TResult>(item.Exception!);

    public static Try<TResult> Select<T, TResult>(this Try<T> item, Func<T, Try<TResult>> selector)
        => item.Success ? selector.Invoke(item.Value) : Error<TResult>(item.Exception!);

    public static T ValueOrDefault<T>(this Try<T> item, T defaultValue)
        => item.Success ? item.Value : defaultValue;

    public static T ValueOrDefault<T>(this Try<T> item, Func<T> defaultGenerator)
        => item.Success ? item.Value : defaultGenerator.Invoke();

    public static Try<T> Or<T>(this Try<T> item, Try<T> other)
        => item.Success ? item : other;

    public static Try<T> Unwrap<T>(this Try<Try<T>> item)
        => item.Success ? item.Value : Error<T>(item.Exception!);
}

}
