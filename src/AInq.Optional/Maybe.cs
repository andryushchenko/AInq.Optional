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

public static class Maybe
{
    public static Maybe<T> None<T>()
        => new(default!, false);

    public static Maybe<T> Value<T>(T value)
        => new(value, true);

    public static Maybe<TResult> Select<T, TResult>(this Maybe<T> item, Func<T, TResult> selector)
        => item.HasValue ? Value(selector.Invoke(item.Value)) : None<TResult>();

    public static Maybe<TResult> Select<T, TResult>(this Maybe<T> item, Func<T, Maybe<TResult>> selector)
        => item.HasValue ? selector.Invoke(item.Value) : None<TResult>();

    public static T ValueOrDefault<T>(this Maybe<T> item, T defaultValue)
        => item.HasValue ? item.Value : defaultValue;

    public static T ValueOrDefault<T>(this Maybe<T> item, Func<T> defaultGenerator)
        => item.HasValue ? item.Value : defaultGenerator.Invoke();

    public static Maybe<T> Or<T>(this Maybe<T> item, Maybe<T> other)
        => item.HasValue ? item : other;

    public static Maybe<T> Unwrap<T>(this Maybe<Maybe<T>> item)
        => item.HasValue ? item.Value : None<T>();
}

}
