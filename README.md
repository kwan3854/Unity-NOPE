# NOPE — No Overused Possibly Evil Exceptions

A lightweight, **zero-allocation** functional extensions library for Unity, inspired by **CSharpFunctionalExtensions**.  
Focuses on **explicitly handling success/failure** without throwing exceptions and **optional values** without null, using `Result<T>` and `Maybe<T>` types.

- **Supports** both **synchronous** and **asynchronous** workflows:
  - **UniTask** integration (if `Cysharp.Threading.Tasks` is installed, with `NOPE_UNITASK` define).
  - **Awaitable** integration (if on **Unity6+** with built-in `Awaitable`, using `NOPE_AWAITABLE` define).
- **Full sync ↔ async bridging** for both `Result<T>` and `Maybe<T>`:  
  Map/Bind/Tap/Match/Finally now have **“all combos”** (sync→async, async→sync, async→async).
- **Minimal GC pressure**: implemented as `readonly struct` to keep allocations low.

---

## Table of Contents

1. [Features Overview](#features-overview)  
2. [Installation](#installation)  
3. [Quick Start](#quick-start)
4. [Common Usage Examples](#common-usage-examples)
   - [Working with Results](#working-with-results)
   - [Optional Values with Maybe](#optional-values-with-maybe)
   - [Async Support (UniTask / Awaitable)](#async-support-unitask--awaitable)
   - [Collection Utilities](#collection-utilities)
   - [Advanced Chaining with `Finally`](#advanced-chaining-with-finally)
5. [API Reference](#api-reference)
   - [Result\<T\> API](#resultt-api)
   - [Maybe\<T\> API](#maybet-api)
   - [Sync ↔ Async Combos](#sync--async-combos)
   - [Best Practices](#best-practices)
6. [License](#license)

---

## Features Overview

- **Result\<T\>** type:  
  - Represents success/failure with a `Value` or an `Error (string)`.
  - Provides functional extension methods (Map, Bind, Tap, Match, Ensure, MapError, etc.).
  - **CFE-style** “Finally” to end the chain with a final output or do a final side effect.

- **Maybe\<T\>** type:  
  - Represents an optional value (`HasValue` vs. `None`).
  - Also has `Map`, `Bind`, `Tap`, `Match`, `Finally`, etc.

- **Complete Sync ↔ Async bridging**:  
  - For each operation (`Map`, `Bind`, `Tap`, `Match`, `Finally`), we have:
    - **Sync** input → **Async** transform
    - **Async** input → **Sync** transform
    - **Async** input → **Async** transform
  - Works with **UniTask** or **Awaitable** (Unity6+), depending on which define (`NOPE_UNITASK` or `NOPE_AWAITABLE`) is active.

- **No Null, No Exceptions** (where feasible):  
  - Instead, use `Result.Failure(...)` or `Maybe.None` to represent errors or absent data.

---

## Installation

### 1) Via Git (UPM)

In your `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git#v0.2.0"
  }
}
```

You can omit `#v0.2.0` to get the latest commit, or specify any Git tag/branch.

### 2) Unity Package Manager (Git URL)

1. Open `Window → Package Manager`
2. Click the “+” → “Add package from git URL…”
3. Paste: `https://github.com/kwan3854/Unity-NOPE.git#v0.2.0`

### 3) Manual Download

Clone or download the repo, and place the **Unity-NOPE** folder under your `Packages` directory or `Assets/Plugins`.

---

## Quick Start

Below are minimal examples showing how to use **NOPE**.

### Example 1: `Result<T>` (Synchronous)
```csharp
Result<int> result1 = 100;         // Implicit => Success(100)
Result<int> result2 = "Bad stuff"; // Implicit => Failure("Bad stuff")

var final = result1
    .Ensure(x => x < 50, "Value is too large!")
    .Map(x => x * 2)
    .Tap(x => UnityEngine.Debug.Log("Value is " + x))
    .Match(
       onSuccess: val => $"OK: {val}",
       onFailure: err => $"FAIL: {err}"
    );
// => If result1 was 100, final == "FAIL: Value is too large!"
```

### Example 2: `Maybe<T>`
```csharp
Maybe<int> m1 = 42;           // HasValue = true
Maybe<int> m2 = Maybe<int>.None;  // No value

var mapped = m1.Map(x => x + 10); // => Maybe(52)

mapped.Match(
    onValue: v => $"Value is {v}",
    onNone: () => "No value"
);
// => "Value is 52"
```

### Example 3: Async Support (UniTask / Awaitable)

```csharp
// If NOPE_UNITASK is defined:
async UniTask<Result<string>> LoadDataAsync()
{
    await UniTask.Delay(1000);
    return "LoadedData";
}

async UniTask SomeOperation()
{
    var result = await LoadDataAsync().Bind(async data => {
        await UniTask.Delay(500);
        return Result<int>.Success(data.Length);
    });
    // ...
}

// If NOPE_AWAITABLE is defined (Unity6+):
public async Awaitable<Result<int>> DoStuff(Awaitable<Result<int>> input)
{
    var mapped = await input.Map(x => x + 1);
    return mapped;
}
```

### Example 4: Collection Utilities
```csharp
Dictionary<string, int> fruitCounts = ...;

Maybe<int> appleCount = fruitCounts.TryFind("apple");  // Maybe<int>
Maybe<int> kiwiCount = fruitCounts.TryFind("kiwi");    // None if not found
```

### Example 5: Advanced Chaining with `Finally`

Use **CFE-style** `Finally` to run a final action or produce a final typed result, ending the chain:

```csharp
Result<int> r = 10;
var finalStr = await r
    .Map(x => x + 5)
    .Bind(x => x > 10 ? Result<string>.Success("OK") : Result<string>.Failure("Not OK"))
    .Finally<string, string>(async res =>
    {
        // Possibly log or do side effects:
        if (res.IsSuccess) UnityEngine.Debug.Log($"Result: {res.Value}");
        else UnityEngine.Debug.LogError($"Error: {res.Error}");

        // Return final string
        return res.IsSuccess ? $"Result was {res.Value}" : "Failure ended chain";
    });
// finalStr => "Result was OK"
```

For `Maybe<T>`:
```csharp
Maybe<int> m = 50;
var finalMaybe = await m
    .Tap(x => UniTask.Run(() => Debug.Log($"Tap: {x}")))
    .Finally(async maybe =>
    {
        Debug.Log($"Finally side effect, hasValue={maybe.HasValue}");
        await UniTask.CompletedTask;
    });
// finalMaybe == Maybe(50)
```

---

## API Reference

### Result<T> API

- **Creation**
  ```csharp
  var r1 = Result<int>.Success(123);
  var r2 = Result<int>.Failure("Error");
  Result<int> r3 = 999;        // Implicit => Success(999)
  Result<int> r4 = "Bad Error"; // Implicit => Failure("Bad Error")
  ```
- **Properties**: `IsSuccess`, `IsFailure`, `Value`, `Error`
- **Core Methods**:
    - `Map`, `Bind`, `Tap`, `Ensure`, `Match`, `MapError`, `Finally`
    - “Sync ↔ Async” combos if `NOPE_UNITASK`/`NOPE_AWAITABLE` is enabled.

### Maybe<T> API

- **Creation**:
  ```csharp
  Maybe<int> m1 = Maybe<int>.From(123);
  Maybe<int> m2 = Maybe<int>.None;
  Maybe<int> m3 = 999; // implicit
  ```
- **Properties**: `HasValue`, `HasNoValue`, `Value`
- **Core Methods**:
    - `Map`, `Bind`, `Tap`, `Match`, `Finally`
    - “Sync ↔ Async” combos if `NOPE_UNITASK`/`NOPE_AWAITABLE`.

### Sync ↔ Async Combos

For both `Result<T>` and `Maybe<T>`, each operation (`Map`, `Bind`, `Tap`, `Match`, `Finally`) has **multiple overloads**:

1. **Sync → Sync**: Already present in the `ResultExtensions` / `MaybeExtensions`.
2. **Sync → Async**:
   ```csharp
   public static UniTask<Result<TNew>> Map<T, TNew>(
       this Result<T> result,
       Func<T, UniTask<TNew>> asyncSelector) { ... }
   ```
3. **Async → Sync**:
   ```csharp
   public static async UniTask<Result<TNew>> Map<T, TNew>(
       this UniTask<Result<T>> asyncResult,
       Func<T, TNew> selector) { ... }
   ```
4. **Async → Async**:
   ```csharp
   public static async UniTask<Result<TNew>> Map<T, TNew>(
       this UniTask<Result<T>> asyncResult,
       Func<T, UniTask<TNew>> asyncSelector) { ... }
   ```
*(And the same logic for `Maybe<T>` + `Awaitable`)  
This ensures you can chain sync and async operations seamlessly.

### Best Practices

1. Use `Result<T>` or `Maybe<T>` instead of throwing exceptions or using `null`.
2. Keep your “happy path” obvious, with errors explicitly in `IsFailure` or `HasNoValue` states.
3. For complex async flows, prefer “**sync → async**” or “**async → async**” to reduce overhead.
4. For “chain termination,” consider using:
    - **CFE-style `Finally`** for `Result<T>` if you want a final typed outcome or final side effect.
    - **Match** if you want to transform `Result<T>`/`Maybe<T>` into a single `TResult`.

---

## License

NOPE is licensed under the **MIT License**.  
Feel free to contribute, open issues, or fork for your own requirements.  
Pull requests are also welcome!

---
