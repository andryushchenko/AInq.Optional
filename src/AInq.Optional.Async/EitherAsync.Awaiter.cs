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

public static partial class EitherAsync
{
#region ValueAsync

    private static async ValueTask<Either<TLeft, TRight>> AwaitLeft<TLeft, TRight>(Task<TLeft> leftTask)
        => Either.Left<TLeft, TRight>(await leftTask.ConfigureAwait(false));

    private static async ValueTask<Either<TLeft, TRight>> AwaitRight<TLeft, TRight>(Task<TRight> rightTask)
        => Either.Right<TLeft, TRight>(await rightTask.ConfigureAwait(false));

    private static async ValueTask<Either<TRight, TLeft>> AwaitInvert<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Invert();

#endregion

#region Convert

    private static async ValueTask<Maybe<TLeft>> AwaitMaybeLeft<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).MaybeLeft();

    private static async ValueTask<Maybe<TRight>> AwaitMaybeRight<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).MaybeRight();
    
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

#endregion
    
#region SelectLeft

    private static async ValueTask<Either<TLeftResult, TRight>> AwaitSelectLeft<TLeft, TRight, TLeftResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TLeftResult> leftSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeft(leftSelector);

    private static async ValueTask<Either<TLeftResult, TRight>> AwaitSelectLeft<TLeft, TRight, TLeftResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Either<TLeftResult, TRight>> leftSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeft(leftSelector);

#endregion

#region SelectRight

    private static async ValueTask<Either<TLeft, TRightResult>> AwaitSelectRight<TLeft, TRight, TRightResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, TRightResult> rightSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRight(rightSelector);

    private static async ValueTask<Either<TLeft, TRightResult>> AwaitSelectRight<TLeft, TRight, TRightResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, Either<TLeft, TRightResult>> rightSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRight(rightSelector);

#endregion

#region Select

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector, Func<TRight, TRightResult> rightSelector,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TLeftResult> leftSelector, Func<TRight, Either<TLeftResult, TRightResult>> rightSelector,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Select(leftSelector, rightSelector);

#endregion

#region SelectLeftAsync

    private static async ValueTask<Either<TLeftResult, TRight>> AwaitSelectLeft<TLeft, TRight, TLeftResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeftAsync(asyncLeftSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    private static async ValueTask<Either<TLeftResult, TRight>> AwaitSelectLeft<TLeft, TRight, TLeftResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRight>>> asyncLeftSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectLeftAsync(asyncLeftSelector, cancellation)
                                                                                 .ConfigureAwait(false);

#endregion

#region SelectRightAsync

    private static async ValueTask<Either<TLeft, TRightResult>> AwaitSelectRight<TLeft, TRight, TRightResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRightAsync(asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    private static async ValueTask<Either<TLeft, TRightResult>> AwaitSelectRight<TLeft, TRight, TRightResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, CancellationToken, ValueTask<Either<TLeft, TRightResult>>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectRightAsync(asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

#endregion

#region SelectAsync

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncLeftSelector, asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        Func<TRight, CancellationToken, ValueTask<TRightResult>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncLeftSelector, asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<TLeftResult>> asyncLeftSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncLeftSelector, asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

    private static async ValueTask<Either<TLeftResult, TRightResult>> AwaitSelect<TLeft, TRight, TLeftResult, TRightResult>(
        Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncLeftSelector,
        Func<TRight, CancellationToken, ValueTask<Either<TLeftResult, TRightResult>>> asyncRightSelector, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).SelectAsync(asyncLeftSelector, asyncRightSelector, cancellation)
                                                                                 .ConfigureAwait(false);

#endregion

#region ValueOrDefault

    private static async ValueTask<TLeft?> AwaitLeftOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault();

    private static async ValueTask<TLeft> AwaitLeftOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, TLeft defaultValue,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault(defaultValue);

    private static async ValueTask<TLeft> AwaitLeftOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Func<TLeft> defaultGenerator,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefault(defaultGenerator);

    private static async ValueTask<TRight?> AwaitRightOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault();

    private static async ValueTask<TRight> AwaitRightOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, TRight defaultValue,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault(defaultValue);

    private static async ValueTask<TRight> AwaitRightOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Func<TRight> defaultGenerator,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefault(defaultGenerator);

#endregion

#region ValueOrDefaultAsync

    private static async ValueTask<TLeft> AwaitLeftOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        Func<CancellationToken, ValueTask<TLeft>> asyncDefaultGenerator, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).LeftOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                                                 .ConfigureAwait(false);

    private static async ValueTask<TRight> AwaitRightOrDefault<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        Func<CancellationToken, ValueTask<TRight>> asyncDefaultGenerator, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).RightOrDefaultAsync(asyncDefaultGenerator, cancellation)
                                                                                 .ConfigureAwait(false);

#endregion

#region ToValue

    private static async ValueTask<TLeft> AwaitToLeft<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Func<TRight, TLeft> rightToLeft,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToLeft(rightToLeft);

    private static async ValueTask<TRight> AwaitToRight<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Func<TLeft, TRight> leftToRight,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToRight(leftToRight);

    private static async ValueTask<TResult> AwaitToValue<TLeft, TRight, TResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TResult> fromLeft, Func<TRight, TResult> fromRight, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToValue(fromLeft, fromRight);

#endregion

#region ToValueAsync

    private static async ValueTask<TLeft> AwaitToLeft<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, CancellationToken, ValueTask<TLeft>> asyncRightToLeft, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToLeftAsync(asyncRightToLeft, cancellation).ConfigureAwait(false);

    private static async ValueTask<TRight> AwaitToRight<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<TRight>> asyncLeftToRight, CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToRightAsync(asyncLeftToRight, cancellation).ConfigureAwait(false);

    private static async ValueTask<TResult> AwaitToValue<TLeft, TRight, TResult>(Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, CancellationToken, ValueTask<TResult>> asyncFromLeft, Func<TRight, CancellationToken, ValueTask<TResult>> asyncFromRight,
        CancellationToken cancellation)
        => await (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).ToValueAsync(asyncFromLeft, asyncFromRight, cancellation)
                                                                                 .ConfigureAwait(false);

#endregion

#region Do

    private static async ValueTask AwaitDo<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Action<TLeft> leftAction,
        Action<TRight> rightAction, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(leftAction, rightAction);

    private static async ValueTask AwaitDoLeft<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Action<TLeft> leftAction,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoLeft(leftAction);

    private static async ValueTask AwaitDoRight<TLeft, TRight>(Task<Either<TLeft, TRight>> eitherTask, Action<TRight> rightAction,
        CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoRight(rightAction);

#endregion

#region DoWithArgument

    private static async ValueTask AwaitDo<TLeft, TRight, TArgument>(Task<Either<TLeft, TRight>> eitherTask, Action<TLeft, TArgument> leftAction,
        Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).Do(leftAction, rightAction, argument);

    private static async ValueTask AwaitDoLeft<TLeft, TRight, TArgument>(Task<Either<TLeft, TRight>> eitherTask, Action<TLeft, TArgument> leftAction,
        TArgument argument, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoLeft(leftAction, argument);

    private static async ValueTask AwaitDoRight<TLeft, TRight, TArgument>(Task<Either<TLeft, TRight>> eitherTask,
        Action<TRight, TArgument> rightAction, TArgument argument, CancellationToken cancellation)
        => (await eitherTask.WaitAsync(cancellation).ConfigureAwait(false)).DoRight(rightAction, argument);

#endregion
}
