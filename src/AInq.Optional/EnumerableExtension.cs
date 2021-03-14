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

using System.Collections.Generic;
using System.Linq;

namespace AInq.Optional
{

public static class EnumerableExtension
{
    public static IEnumerable<T> Values<T>(this IEnumerable<Maybe<T>> collection)
        => collection.Where(item => item.HasValue)
                     .Select(item => item.Value);

    public static IEnumerable<T> Values<T>(this IEnumerable<Try<T>> collection)
        => collection.Where(item => item.Success)
                     .Select(item => item.Value);

    public static IEnumerable<TLeft> LeftValues<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> collection)
        => collection.Where(item => item.HasLeft)
                     .Select(item => item.Left);

    public static IEnumerable<TRight> RightValues<TLeft, TRight>(this IEnumerable<Either<TLeft, TRight>> collection)
        => collection.Where(item => item.HasRight)
                     .Select(item => item.Right);
}

}
