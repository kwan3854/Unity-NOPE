# NOPE — No Overused Possibly Evil Exceptions

A lightweight, **zero-allocation** functional extensions library for Unity, inspired by CSharpFunctionalExtensions.  
Focuses on **explicitly handling success/failure** without throwing exceptions, and **optional values** without null, using `Result<T>` and `Maybe<T>` types.

- **Unity 6+ support** for `Awaitable` (built-in async/await mechanism).  
- **UniTask** integration (if you install `Cysharp.Threading.Tasks`).  
- Fully works with **async/await** chain, or in pure synchronous code.

---

## Table of Contents

1. [Features Overview](#features-overview)  
2. [Installation](#installation)  
3. [Quick Start](#quick-start)  
4. [API Reference](#api-reference)  
   - [Result<T> Basics](#resultt-basics)  
   - [Maybe<T> Basics](#maybet-basics)  
   - [Core Extensions (Map, Bind, Tap, etc.)](#core-extensions)  
   - [Async Extensions (UniTask / Awaitable)](#async-extensions-unitask--awaitable)  
5. [License](#license)

---

## Features Overview

- **Result<T>** type representing success/failure with a `Value` or an `Error` (string).  
- **Maybe<T>** type representing an optional value (has value or not).  
- **Low Allocation**: Implemented as `readonly struct` to minimize GC pressure.
- **Core Extension Methods** for functional chaining: `Map`, `Bind`, `Tap`, `Ensure`, `Match`, `MapError`, etc.  
- **Async**:  
  - Supports **UniTask** if installed.  
  - Supports **Unity6+ Awaitable** if you’re using Unity version 6000.0.x or newer.

With **NOPE**, you can significantly reduce reliance on exceptions by explicitly modeling success/failure, or presence/absence of data.

---

## Installation

### 1) Via Git (UPM)

Add the following to your `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git#v0.1.0"
  }
}
```

- You can omit #v0.1.0 to always get the latest commit, or specify any Git tag/branch you want.
### 2) Via Unity Package Manager (Git URL)
Open Window → Package Manager
Click the “+” button and “Add package from git URL”
Paste: `https://github.com/kwan3854/Unity-NOPE.git#v0.1.0`

### 3) Manual Download
You can also clone or download this repo and place the Unity-NOPE folder under your Packages folder.

---

## Quick Start
Below are minimal examples showing how to use NOPE:

### Example 1: Result<T> (Synchronous)
```csharp
// ...existing code...
using NOPE;

Result<int> result1 = 100;         // Implicit => Success(100)
Result<int> result2 = "Bad stuff"; // Implicit => Failure("Bad stuff")

// Chaining
var final = result1
    .Ensure(x => x < 50, "Value is too large!")
    .Map(x => x * 2)
    .Tap(x => UnityEngine.Debug.Log("Value is " + x))
    .Match(
       onSuccess: val => $"OK: {val}",
       onFailure: err => $"FAIL: {err}"
    );
// If result1 was 100 => final == "FAIL: Value is too large!"
// ...existing code...
```

### Example 2: Maybe<T>
```csharp
// ...existing code...
Maybe<int> m1 = 42;                  // HasValue = true
Maybe<int> m2 = Maybe<int>.None;     // HasNoValue = true

var mapped = m1
    .Map(x => x + 10)  // => Maybe(52)
    .Bind(x => x > 50 ? Maybe<string>.From("large") : Maybe<string>.None); // => Maybe("large")

mapped.Tap(str => UnityEngine.Debug.Log("We have: " + str));

string msg = mapped.Match(
    onValue: v => $"Value is {v}",
    onNone: () => "No Value"
);
// => "Value is large"
// ...existing code...
```

### Example 3: UniTask or Awaitable (Async)
```csharp
// ...existing code...
using Cysharp.Threading.Tasks;

async UniTask<Result<string>> LoadData()
{
    await UniTask.Delay(1000); // simulate something
    return "LoadedData";       // Implicit => Success("LoadedData")
}

async UniTask SomeAsyncOperation()
{
    var result = await LoadData().Bind(async data => {
        await UniTask.Delay(500);
        return Result<int>.Success(data.Length);
    });
    // ...
}

// ...existing code...

using UnityEngine.Awaitable;

public async Awaitable<Result<int>> DoStuffAwaitable(Awaitable<Result<int>> input)
{
    var mapped = await input.Map(x => x + 1);
    // ...
    return mapped;
}

// ...existing code...
```

---

## API Reference
### Result<T> Basics
1. **Creation**
    ```csharp
    var success = Result<int>.Success(123);
    var failure = Result<int>.Failure("Some Error");

    // Implicit conversions:
    Result<int> alsoSuccess = 123;
    Result<int> alsoFailure = "Oops!";
    ```
2. **Properties**
    - `IsSuccess` / `IsFailure`: Boolean flags
    - `Value`: Valid only if `IsSuccess == true`
    - `Error`: Valid only if `IsFailure == true`
3. **Methods**
    - `Value` / `Error`: throws `InvalidOperationException` if used incorrectly

---

### Maybe<T> Basics
1. **Creation**
    ```csharp
    Maybe<string> m1 = Maybe<string>.From("Hello");
    Maybe<string> m2 = Maybe<string>.None;
    // Implicit
    Maybe<int> m3 = 42;
    ```
2. **Properties**
    - `HasValue` / `HasNoValue`
    - `Value`: throws if `HasValue == false`

---

### Core Extensions
- **Map**
    - `Result: Map<T, U>(Func<T,U>)` transforms the success value; if failure, remains failure.
    - `Maybe: Map<T, U>(Func<T,U>)` transforms the inner value if present; otherwise returns None.
- **Bind**
    - `Result: Bind<T, U>(Func<T,Result<U>>)` flat-map a success value into another Result.
    - `Maybe: Bind<T, U>(Func<T,Maybe<U>>)`
- **Tap**
    - `Result: Tap<T>(Action<T>)` if success, perform side-effect with the value, then returns the same Result.
    - `Maybe: Tap<T>(Action<T>)`
- **Ensure (Result only)**
    - `Ensure<T>(Func<T,bool> predicate, string errorMessage)` If success but predicate fails, turn into failure with given message.
- **Match**
    - `Result: Match<T, R>(Func<T,R> onSuccess, Func<string,R> onFailure)`
    - `Maybe: Match<T, R>(Func<T,R> onValue, Func<R> onNone)`
- **MapError (Result only)**
    - `MapError<T>(Func<string,string> errorSelector)` if failure, transform the error message; otherwise unchanged success.

---

### Async Extensions (UniTask / Awaitable)
If you have `NOPE_UNITASK` define (i.e. installed Cysharp.Threading.Tasks):
- `Map`, `Bind`, `Tap`, `Ensure`, `Match` for `UniTask<Result<T>>` or sometimes `Result<T>` → `UniTask<Result<U>>`

If you have `NOPE_AWAITABLE` define (i.e. Unity6 version 6000.0.x or above):
- `Map`, `Bind`, `Tap`, `Ensure`, `Match` for `Awaitable<Result<T>>`.

The usage is analogous to the synchronous versions, but you can await them in an async context.

---

## License
NOPE is released under the MIT License.
Feel free to contribute, report issues, or fork for your own needs!
