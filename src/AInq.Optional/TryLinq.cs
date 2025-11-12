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

/// <summary> <see cref="Try{T}" /> LINQ utils </summary>
public static class TryLinq
{
    /// <summary> Select existing values </summary>
    /// <param name="collection"> Try collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    /// <returns> Values collection </returns>
    [PublicAPI, LinqTunnel]
    public static IEnumerable<T> Values<T>(this IEnumerable<Try<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection))).Where(item => item is {Success: true}).Select(item => item.Value);

    /// <inheritdoc cref="TryLinq.Values{T}(IEnumerable{Try{T}})" />
    [PublicAPI, LinqTunnel]
    public static ParallelQuery<T> Values<T>(this ParallelQuery<Try<T>> collection)
        => (collection ?? throw new ArgumentNullException(nameof(collection))).Where(item => item is {Success: true}).Select(item => item.Value);

}
