# AInq.Optional [![GitHub release (latest by date)](https://img.shields.io/github/v/release/andryushchenko/AInq.Optional)](https://github.com/andryushchenko/AInq.Optional/releases) [![GitHub](https://img.shields.io/github/license/andryushchenko/AInq.Optional)](LICENSE)

![AInq](https://raw.githubusercontent.com/andryushchenko/AInq.Optional/main/AInq.png)

## What is it?

Simple optional types with basic helpers and converters

- **`Maybe<T>`** value or nothing
- **`Try<T>`** value or error
- **`Either<TLeft, TRight>`** value either value

[![Nuget](https://img.shields.io/nuget/v/AInq.Optional)](https://www.nuget.org/packages/AInq.Optional/)
**AInq.Optional** - Types and basic helpers

[![Nuget](https://img.shields.io/nuget/v/AInq.Optional.Async)](https://www.nuget.org/packages/AInq.Optional.Async/)
**AInq.Optional.Async** - Async helpers and extensions

## New in 4.0

Large refactoring and internal optimization with some **breaking changes**

- Remove build tagets for STS .Net versions
- Remove all `Try<T>` extensions (except `Try.AsMaybe`), which can implicitly hide exceptions
- Save exception stacktrace in `Try<T>`
- Use `System.Linq` for collection extensions, which may cause minor behavior changes
- Use `System.Linq.AsyncEnumerable` in .net10 with minor API changes
- New APIs
    - `|` (or) operator for `Maybe<T>`
    - `!` (invert) operator for `Either<TLeft, TRight>`
    - `Maybe.Values` collection extension with filtering
    - `Try.Result` with additional gerenrator parameter

### New in 3.0

For version 3.0 this lib was completely rewritten with some **breaking changes**

- All types are now `class` instead of `struct`
- Remove some logically obscure methods
    - `IComparable` implementation (problems with comparing `null` and empty item)
    - Type cast operators (unused)
    - `Try.SelectOrDefault` and `Try.ValueOrDefault` (implicitly hides error, can be replaced with `Try.AsMaybe`)
- Async extensions completely rewritten using `ValueTask` and moved to separate package

****

## Contribution

If you find a bug, have a question or something else - you are friendly welcome to open an issue.

## License

Copyright © 2021 Anton Andryushchenko. AInq.Optional is licensed under Apache License 2.0
