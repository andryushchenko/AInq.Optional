// Copyright 2021-2022 Anton Andryushchenko
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

/// <summary> Converter async utils </summary>
public static class ConverterAsync
{
#region Maybe

    private static async ValueTask<Maybe<TLeft>> AwaitMaybeLeft<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).MaybeLeft();

    /// <inheritdoc cref="Converter.MaybeLeft{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TLeft>> MaybeLeft<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TLeft>>(eitherTask.Result.MaybeLeft())
            : AwaitMaybeLeft(eitherTask, cancellation);

    /// <inheritdoc cref="Converter.MaybeLeft{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TLeft>> MaybeLeft<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TLeft>>(eitherValueTask.Result.MaybeLeft())
            : AwaitMaybeLeft(eitherValueTask.AsTask(), cancellation);

    private static async ValueTask<Maybe<TRight>> AwaitMaybeRight<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).MaybeRight();

    /// <inheritdoc cref="Converter.MaybeRight{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TRight>> MaybeRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TRight>>(eitherTask.Result.MaybeRight())
            : AwaitMaybeRight(eitherTask, cancellation);

    /// <inheritdoc cref="Converter.MaybeRight{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<TRight>> MaybeRight<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TRight>>(eitherValueTask.Result.MaybeRight())
            : AwaitMaybeRight(eitherValueTask.AsTask(), cancellation);

    private static async ValueTask<Maybe<T>> AwaitAsMaybe<T>(Task<Try<T>> tryTask, CancellationToken cancellation)
    {
        try
        {
            return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).AsMaybe();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Maybe.None<T>();
        }
    }

    /// <inheritdoc cref="Converter.AsMaybe{T}(Try{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybe<T>(this Task<Try<T>> tryTask, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(tryTask.Result.AsMaybe())
            : AwaitAsMaybe(tryTask, cancellation);

    /// <inheritdoc cref="Converter.AsMaybe{T}(Try{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Maybe<T>> AsMaybe<T>(this ValueTask<Try<T>> tryValueTask, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(tryValueTask.Result.AsMaybe())
            : AwaitAsMaybe(tryValueTask.AsTask(), cancellation);

#endregion

#region Try

    private static async ValueTask<Try<TLeft>> AwaitTryLeft<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation)
    {
        try
        {
            return (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).TryLeft();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<TLeft>(ex);
        }
    }

    /// <inheritdoc cref="Converter.TryLeft{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TLeft>> TryLeft<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<TLeft>>(eitherTask.Result.TryLeft())
            : AwaitTryLeft(eitherTask, cancellation);

    /// <inheritdoc cref="Converter.TryLeft{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TLeft>> TryLeft<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<TLeft>>(eitherValueTask.Result.TryLeft())
            : AwaitTryLeft(eitherValueTask.AsTask(), cancellation);

    private static async ValueTask<Try<TRight>> AwaitTryRight<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation)
    {
        try
        {
            return (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).TryRight();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<TRight>(ex);
        }
    }

    /// <inheritdoc cref="Converter.TryRight{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TRight>> TryRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<TRight>>(eitherTask.Result.TryRight())
            : AwaitTryRight(eitherTask, cancellation);

    /// <inheritdoc cref="Converter.TryRight{TLeft,TRight}(Either{TLeft,TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<TRight>> TryRight<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<TRight>>(eitherValueTask.Result.TryRight())
            : AwaitTryRight(eitherValueTask.AsTask(), cancellation);

    private static async ValueTask<Try<T>> AwaitAsTry<T>(Task<Maybe<T>> maybeTask, CancellationToken cancellation)
    {
        try
        {
            return (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).AsTry();
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Try.Error<T>(ex);
        }
    }

    /// <inheritdoc cref="Converter.AsTry{T}(Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> AsTry<T>(this Task<Maybe<T>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(maybeTask.Result.AsTry())
            : AwaitAsTry(maybeTask, cancellation);

    /// <inheritdoc cref="Converter.AsTry{T}(Maybe{T})" />
    [PublicAPI, Pure]
    public static ValueTask<Try<T>> AsTry<T>(this ValueTask<Maybe<T>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(maybeValueTask.Result.AsTry())
            : AwaitAsTry(maybeValueTask.AsTask(), cancellation);

#endregion

#region Or

    private static async ValueTask<Either<TLeft, TRight>> AwaitOr<TLeft, TRight>(Task<Maybe<TLeft>> maybeTask, TRight other,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this Task<Maybe<TLeft>> maybeTask, [NoEnumeration] TRight other,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(maybeTask.Result.Or(other))
            : AwaitOr(maybeTask, other, cancellation);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this ValueTask<Maybe<TLeft>> maybeValueTask, [NoEnumeration] TRight other,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(maybeValueTask.Result.Or(other))
            : AwaitOr(maybeValueTask.AsTask(), other, cancellation);

    private static async ValueTask<Either<TLeft, TRight>> AwaitOr<TLeft, TRight>(Task<Maybe<TLeft>> maybeTask, Func<TRight> otherGenerator,
        CancellationToken cancellation)
        => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(otherGenerator);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this Task<Maybe<TLeft>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<TRight> otherGenerator, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(maybeTask.Result.Or(otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))))
            : AwaitOr(maybeTask, otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator)), cancellation);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this ValueTask<Maybe<TLeft>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight> otherGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(
                maybeValueTask.Result.Or(otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))))
            : AwaitOr(maybeValueTask.AsTask(), otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator)), cancellation);

    private static async ValueTask<Either<TLeft, TRight>> AwaitOr<TLeft, TRight>(Task<Try<TLeft>> tryTask, TRight other,
        CancellationToken cancellation)
    {
        try
        {
            return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            return Either.Right<TLeft, TRight>(other);
        }
    }

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this Task<Try<TLeft>> tryTask, [NoEnumeration] TRight other,
        CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(tryTask.Result.Or(other))
            : AwaitOr(tryTask, other, cancellation);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},TRight)" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this ValueTask<Try<TLeft>> tryValueTask, [NoEnumeration] TRight other,
        CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(tryValueTask.Result.Or(other))
            : AwaitOr(tryValueTask.AsTask(), other, cancellation);

    private static async ValueTask<Either<TLeft, TRight>> AwaitOr<TLeft, TRight>(Task<Try<TLeft>> tryTask, Func<TRight> otherGenerator,
        CancellationToken cancellation)
    {
        Try<TLeft> @try;
        try
        {
            @try = await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            @try = ex.AsTry<TLeft>();
        }
        return @try.Or(otherGenerator);
    }

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this Task<Try<TLeft>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<TRight> otherGenerator, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(tryTask.Result.Or(otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))))
            : AwaitOr(tryTask, otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator)), cancellation);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this ValueTask<Try<TLeft>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<TRight> otherGenerator, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(tryValueTask.Result.Or(otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator))))
            : AwaitOr(tryValueTask.AsTask(), otherGenerator ?? throw new ArgumentNullException(nameof(otherGenerator)), cancellation);

#endregion

#region OrAsync

    private static async ValueTask<Either<TLeft, TRight>> AwaitGenerator<TLeft, TRight>(
        Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation)
        => Either.Right<TLeft, TRight>(await asyncOtherGenerator.Invoke(cancellation).ConfigureAwait(false));

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> OrAsync<TLeft, TRight>(this Maybe<TLeft> maybe,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation = default)
        => (maybe ?? throw new ArgumentNullException(nameof(maybe))).HasValue
            ? new ValueTask<Either<TLeft, TRight>>(Either.Left<TLeft, TRight>(maybe.Value))
            : AwaitGenerator<TLeft, TRight>(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> OrAsync<TLeft, TRight>(this Try<TLeft> @try,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation = default)
        => (@try ?? throw new ArgumentNullException(nameof(@try))).Success
            ? new ValueTask<Either<TLeft, TRight>>(Either.Left<TLeft, TRight>(@try.Value))
            : AwaitGenerator<TLeft, TRight>(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

    private static async ValueTask<Either<TLeft, TRight>> AwaitOrAsync<TLeft, TRight>(Task<Maybe<TLeft>> maybeTask,
        Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation)
        => await (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).OrAsync(asyncOtherGenerator, cancellation).ConfigureAwait(false);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> OrAsync<TLeft, TRight>(this Task<Maybe<TLeft>> maybeTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? maybeTask.Result.OrAsync(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation)
            : AwaitOrAsync(maybeTask, asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> OrAsync<TLeft, TRight>(this ValueTask<Maybe<TLeft>> maybeValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? maybeValueTask.Result.OrAsync(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation)
            : AwaitOrAsync(maybeValueTask.AsTask(),
                asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)),
                cancellation);

    private static async ValueTask<Either<TLeft, TRight>> AwaitOrAsync<TLeft, TRight>(Task<Try<TLeft>> tryTask,
        Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation)

    {
        Try<TLeft> @try;
        try
        {
            @try = await tryTask.WaitAsync(cancellation).ConfigureAwait(false);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            @try = ex.AsTry<TLeft>();
        }
        return await @try.OrAsync(asyncOtherGenerator, cancellation).ConfigureAwait(false);
    }

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> OrAsync<TLeft, TRight>(this Task<Try<TLeft>> tryTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? tryTask.Result.OrAsync(asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation)
            : AwaitOrAsync(tryTask, asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},Func{TRight})" />
    [PublicAPI, Pure]
    public static ValueTask<Either<TLeft, TRight>> OrAsync<TLeft, TRight>(this ValueTask<Try<TLeft>> tryValueTask,
        [InstantHandle(RequireAwait = true)] Func<CancellationToken, ValueTask<TRight>> asyncOtherGenerator, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? tryValueTask.Result.OrAsync(asyncOtherGenerator, cancellation)
            : AwaitOrAsync(tryValueTask.AsTask(), asyncOtherGenerator ?? throw new ArgumentNullException(nameof(asyncOtherGenerator)), cancellation);

#endregion
}
