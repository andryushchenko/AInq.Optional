// Copyright 2021 Anton Andryushchenko
// 
// Licensed under the Apache License, Version 2.0 (the "License")

namespace AInq.Optional;

public static partial class Maybe
{
    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<T> collection)
    {
        /// <inheritdoc cref="MaybeFirst{T}(IEnumerable{T})" />
        [PublicAPI, Obsolete("Use MaybeFirst instead")]
        public Maybe<T> FirstOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeFirst();
        }

        /// <inheritdoc cref="MaybeFirst{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI, Obsolete("Use MaybeFirst instead")]
        public Maybe<T> FirstOrNone([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeFirst(filter);
        }

        /// <inheritdoc cref="MaybeLast{T}(IEnumerable{T})" />
        [PublicAPI, Obsolete("Use MaybeLast instead")]
        public Maybe<T> LastOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeLast();
        }

        /// <inheritdoc cref="MaybeLast{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI, Obsolete("Use MaybeLast instead")]
        public Maybe<T> LastOrNone([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeLast(filter);
        }

        /// <inheritdoc cref="MaybeSingle{T}(IEnumerable{T})" />
        [PublicAPI, Obsolete("Use MaybeSingle instead")]
        public Maybe<T> SingleOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeSingle();
        }

        /// <inheritdoc cref="MaybeSingle{T}(IEnumerable{T},Func{T,bool})" />
        [PublicAPI, Obsolete("Use MaybeSingle instead")]
        public Maybe<T> SingleOrNone([InstantHandle] Func<T, bool> filter)
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            _ = filter ?? throw new ArgumentNullException(nameof(filter));
            return collection.MaybeSingle(filter);
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<T?> collection)
        where T : class
    {
        /// <inheritdoc cref="MaybeFirstNotNull{T}(IEnumerable{T?})" />
        [PublicAPI, Obsolete("Use MaybeFirstNotNull instead")]
        public Maybe<T> FirstNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeFirstNotNull();
        }

        /// <inheritdoc cref="MaybeLastNotNull{T}(IEnumerable{T?})" />
        [PublicAPI, Obsolete("Use MaybeLastNotNull instead")]
        public Maybe<T> LastNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeLastNotNull();
        }

        /// <inheritdoc cref="MaybeSingleNotNull{T}(IEnumerable{T?})" />
        [PublicAPI, Obsolete("Use MaybeSingleNotNull instead")]
        public Maybe<T> SingleNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeSingleNotNull();
        }
    }

    /// <param name="collection"> Value collection </param>
    /// <typeparam name="T"> Value type </typeparam>
    extension<T>(IEnumerable<T?> collection)
        where T : struct
    {
        /// <inheritdoc cref="MaybeFirstNotNull{T}(IEnumerable{T?})" />
        [PublicAPI, Obsolete("Use MaybeFirstNotNull instead")]
        public Maybe<T> FirstNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeFirstNotNull();
        }

        /// <inheritdoc cref="MaybeLastNotNull{T}(IEnumerable{T?})" />
        [PublicAPI, Obsolete("Use MaybeLastNotNull instead")]
        public Maybe<T> LastNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeLastNotNull();
        }

        /// <inheritdoc cref="MaybeSingleNotNull{T}(IEnumerable{T?})" />
        [PublicAPI, Obsolete("Use MaybeSingleNotNull instead")]
        public Maybe<T> SingleNotNullOrNone()
        {
            _ = collection ?? throw new ArgumentNullException(nameof(collection));
            return collection.MaybeSingleNotNull();
        }
    }
}
