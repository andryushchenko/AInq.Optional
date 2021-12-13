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

using static AInq.Optional.TaskHelper;

namespace AInq.Optional;

/// <summary> Converter async utils </summary>
public static class ConverterAsync
{
    /// <inheritdoc cref="Converter.MaybeLeft{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Maybe<TLeft>> MaybeLeft<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TLeft>>(eitherTask.Result.MaybeLeft())
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).MaybeLeft());

    /// <inheritdoc cref="Converter.MaybeLeft{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Maybe<TLeft>> MaybeLeft<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TLeft>>(eitherValueTask.Result.MaybeLeft())
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).MaybeLeft());

    /// <inheritdoc cref="Converter.MaybeRight{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Maybe<TRight>> MaybeRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<TRight>>(eitherTask.Result.MaybeRight())
            : FromFunctionAsync(async () => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).MaybeRight());

    /// <inheritdoc cref="Converter.MaybeRight{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Maybe<TRight>> MaybeRight<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<TRight>>(eitherValueTask.Result.MaybeRight())
            : FromFunctionAsync(async () => (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).MaybeRight());

    /// <inheritdoc cref="Converter.TryLeft{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Try<TLeft>> TryLeft<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<TLeft>>(eitherTask.Result.TryLeft())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).TryLeft();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<TLeft>(ex);
                }
            });

    /// <inheritdoc cref="Converter.TryLeft{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Try<TLeft>> TryLeft<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<TLeft>>(eitherValueTask.Result.TryLeft())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).TryLeft();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<TLeft>(ex);
                }
            });

    /// <inheritdoc cref="Converter.TryRight{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Try<TRight>> TryRight<TLeft, TRight>(this Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation = default)
        => (eitherTask ?? throw new ArgumentNullException(nameof(eitherTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<TRight>>(eitherTask.Result.TryRight())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).TryRight();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<TRight>(ex);
                }
            });

    /// <inheritdoc cref="Converter.TryRight{TLeft,TRight}(Either{TLeft,TRight})" />
    public static ValueTask<Try<TRight>> TryRight<TLeft, TRight>(this ValueTask<Either<TLeft, TRight>> eitherValueTask,
        CancellationToken cancellation = default)
        => eitherValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<TRight>>(eitherValueTask.Result.TryRight())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await eitherValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).TryRight();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<TRight>(ex);
                }
            });

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},TRight)" />
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this Task<Maybe<TLeft>> maybeTask, TRight other,
        CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(maybeTask.Result.Or(other))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other));

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Maybe{TLeft},TRight)" />
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this ValueTask<Maybe<TLeft>> maybeValueTask, TRight other,
        CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(maybeValueTask.Result.Or(other))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Or(other));

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},TRight)" />
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this Task<Try<TLeft>> tryTask, TRight other,
        CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Either<TLeft, TRight>>(tryTask.Result.Or(other))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(other);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Either.Right<TLeft, TRight>(other);
                }
            });

    /// <inheritdoc cref="Converter.Or{TLeft,TRight}(Try{TLeft},TRight)" />
    public static ValueTask<Either<TLeft, TRight>> Or<TLeft, TRight>(this ValueTask<Try<TLeft>> tryValueTask, TRight other,
        CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Either<TLeft, TRight>>(tryValueTask.Result.Or(other))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Or(other);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Either.Right<TLeft, TRight>(other);
                }
            });

    /// <inheritdoc cref="Converter.Or{T}(Maybe{T},Try{T})" />
    public static ValueTask<Try<T>> Or<T>(this Task<Maybe<T>> maybeTask, Try<T> @try, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(maybeTask.Result.Or(@try))
            : FromFunctionAsync(async () => (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(@try));

    /// <inheritdoc cref="Converter.Or{T}(Maybe{T},Try{T})" />
    public static ValueTask<Try<T>> Or<T>(this ValueTask<Maybe<T>> maybeValueTask, Try<T> @try, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(maybeValueTask.Result.Or(@try))
            : FromFunctionAsync(async () => (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Or(@try));

    /// <inheritdoc cref="Converter.Or{T}(Try{T},Maybe{T})" />
    public static ValueTask<Maybe<T>> Or<T>(this Task<Try<T>> tryTask, Maybe<T> maybe, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(tryTask.Result.Or(maybe))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).Or(maybe);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return maybe;
                }
            });

    /// <inheritdoc cref="Converter.Or{T}(Try{T},Maybe{T})" />
    public static ValueTask<Maybe<T>> Or<T>(this ValueTask<Try<T>> tryValueTask, Maybe<T> maybe, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(tryValueTask.Result.Or(maybe))
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).Or(maybe);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return maybe;
                }
            });

    /// <inheritdoc cref="Converter.AsTry{T}(Maybe{T})" />
    public static ValueTask<Try<T>> AsTry<T>(this Task<Maybe<T>> maybeTask, CancellationToken cancellation = default)
        => (maybeTask ?? throw new ArgumentNullException(nameof(maybeTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Try<T>>(maybeTask.Result.AsTry())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await maybeTask.WaitAsync(cancellation).ConfigureAwait(false)).AsTry();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<T>(ex);
                }
            });

    /// <inheritdoc cref="Converter.AsTry{T}(Maybe{T})" />
    public static ValueTask<Try<T>> AsTry<T>(this ValueTask<Maybe<T>> maybeValueTask, CancellationToken cancellation = default)
        => maybeValueTask.IsCompletedSuccessfully
            ? new ValueTask<Try<T>>(maybeValueTask.Result.AsTry())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await maybeValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).AsTry();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Try.Error<T>(ex);
                }
            });

    /// <inheritdoc cref="Converter.AsMaybe{T}(Try{T})" />
    public static ValueTask<Maybe<T>> AsMaybe<T>(this Task<Try<T>> tryTask, CancellationToken cancellation = default)
        => (tryTask ?? throw new ArgumentNullException(nameof(tryTask))).Status is TaskStatus.RanToCompletion
            ? new ValueTask<Maybe<T>>(tryTask.Result.AsMaybe())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryTask.WaitAsync(cancellation).ConfigureAwait(false)).AsMaybe();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Maybe.None<T>();
                }
            });

    /// <inheritdoc cref="Converter.AsMaybe{T}(Try{T})" />
    public static ValueTask<Maybe<T>> AsMaybe<T>(this ValueTask<Try<T>> tryValueTask, CancellationToken cancellation = default)
        => tryValueTask.IsCompletedSuccessfully
            ? new ValueTask<Maybe<T>>(tryValueTask.Result.AsMaybe())
            : FromFunctionAsync(async () =>
            {
                try
                {
                    return (await tryValueTask.AsTask().WaitAsync(cancellation).ConfigureAwait(false)).AsMaybe();
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    return Maybe.None<T>();
                }
            });
}
