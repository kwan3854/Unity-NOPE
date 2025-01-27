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
4. [Common Usage Examples](#common-usage-examples)
   - [Working with Results](#working-with-results)
   - [Optional Values with Maybe](#optional-values-with-maybe)
   - [Collection Utilities](#collection-utilities)
   - [Async Operations](#async-operations)
5. [API Reference](#api-reference)
   - [Result<T> API](#resultt-api)
   - [Maybe<T> API](#maybet-api)
   - [Core Extensions](#core-extensions)
   - [Collection Extensions](#collection-extensions)
   - [Advanced Extensions](#advanced-extensions)
   - [Async Extensions](#async-extensions)
6. [Best Practices](#best-practices)
7. [License](#license)

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
    "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git#v0.2.0"
  }
}
```

- You can omit #v0.1.0 to always get the latest commit, or specify any Git tag/branch you want.
### 2) Via Unity Package Manager (Git URL)
Open Window → Package Manager
Click the “+” button and “Add package from git URL”
Paste: `https://github.com/kwan3854/Unity-NOPE.git`

Or paste with a specific version: `https://github.com/kwan3854/Unity-NOPE.git#v0.2.0`

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

### Example 4: Advanced Chaining
```csharp
// ...existing code...
using NOPE;

Result<int> result = 100; // Implicit => Success(100)

var final = result
    .Ensure(x => x < 150, "Value is too large!")
    .Map(x => x * 2)
    .Bind(x => x > 50 ? Result<string>.Success("large") : Result<string>.Failure("small"))
    .Tap(x => UnityEngine.Debug.Log("Value is " + x))
    .Match(
       onSuccess: val => $"OK: {val}",
       onFailure: err => $"FAIL: {err}"
    );
// If result was 100 => final == "OK: large"
// ...existing code...
```

### Example 5: Collection Operations
```csharp
// Safe collection handling with Maybe
public class UserService
{
    private Dictionary<string, User> _users;
    private List<Order> _orders;

    public Maybe<Order> GetLatestOrder(string userId)
    {
        return _users.TryFind(userId)                     // Maybe<User>
            .Bind(user => _orders                         // Maybe<Order>
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.Date)
                .TryFirst());
    }

    public IEnumerable<Order> GetValidOrders()
    {
        return _orders
            .Select(order => ValidateOrder(order))        // IEnumerable<Maybe<Order>>
            .Choose();                                    // Only valid orders
    }
}
```

### Example 6: Async with Error Handling
```csharp
public async UniTask<Result<GameState>> LoadGameState(string saveId)
{
    return await Result.Of(async () => 
    {
        var saveFile = await LoadSaveFile(saveId);       // might throw
        var state = await ParseSaveFile(saveFile);       // might throw
        return state;
    }, ex => $"Failed to load save: {ex.Message}")
    .Ensure(state => state.Version.IsCompatible(), "Incompatible save version")
    .Tap(state => Analytics.TrackGameLoad(state.Version));
}

// Combining async operations with Maybe
public async UniTask<Maybe<UserData>> GetCachedUserData(string userId)
{
    var cached = await cache.TryFind(userId);            // Maybe<UserData>
    return await cached
        .Where(data => !data.IsExpired)                 // Filter if expired
        .Or(LoadFromDatabase(userId))                   // Fallback to DB
        .Tap(data => UpdateCache(userId, data));        // Update cache if found
}
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

NOPE's core functionalities can be used in a chainable manner through extension methods.

#### Map - Value Transformation
Transforms a success/present value into a different form.
```csharp
Result<int> result = Result<int>.Success(5);
Result<string> mapped = result.Map(x => $"Value is {x}");
// mapped == Success("Value is 5")

Maybe<int> maybe = 10;
Maybe<string> mappedMaybe = maybe.Map(x => $"Value is {x}");
// mappedMaybe == Maybe("Value is 10")
```

#### Bind - Value Binding
Binds a success/present value to another `Result` or `Maybe`.
```csharp
Result<int> result = Result<int>.Success(5);
Result<string> bound = result.Bind(x => Result<string>.Success($"Value is {x}"));
// bound == Success("Value is 5")

Maybe<int> maybe = 10;
Maybe<string> boundMaybe = maybe.Bind(x => Maybe<string>.From($"Value is {x}"));
// boundMaybe == Maybe("Value is 10")
```

#### Tap - Side Effects
Performs side effects on a success/present value and returns the same `Result` or `Maybe`.
```csharp
Result<int> result = Result<int>.Success(5);
result.Tap(x => UnityEngine.Debug.Log($"Value is {x}"));
// Log output: "Value is 5"

Maybe<int> maybe = 10;
maybe.Tap(x => UnityEngine.Debug.Log($"Value is {x}"));
// Log output: "Value is 10"
```

#### Ensure - Condition Validation (Result only)
Transforms a success value into a failure if it does not meet a given condition.
```csharp
Result<int> result = Result<int>.Success(5);
Result<int> ensured = result.Ensure(x => x > 10, "Value is too small");
// ensured == Failure("Value is too small")
```

#### Match - Value Matching
Returns different results based on success/present value or failure/absence state.
```csharp
Result<int> result = Result<int>.Success(5);
string message = result.Match(
    onSuccess: x => $"Value is {x}",
    onFailure: err => $"Error: {err}"
);
// message == "Value is 5"

Maybe<int> maybe = 10;
string maybeMessage = maybe.Match(
    onValue: x => $"Value is {x}",
    onNone: () => "No value"
);
// maybeMessage == "Value is 10"
```

#### MapError - Error Transformation (Result only)
Transforms the error message of a failure state into a different form.
```csharp
Result<int> result = Result<int>.Failure("Original error");
Result<int> mappedError = result.MapError(err => $"Mapped: {err}");
// mappedError == Failure("Mapped: Original error")
```

### Result Creation Extensions

#### Conditional Creation
Creates Result instances based on conditions.
```csharp
// Create Success/Failure based on condition
Result<int> r1 = Result.SuccessIf(x > 10, value, "Too small");
Result<int> r2 = Result.FailureIf(x < 0, value, "Negative not allowed");

// With lazy evaluation
Result<int> r3 = Result.SuccessIf(() => CheckCondition(), value, "Check failed");
```

#### Exception Handling
Wraps potentially throwing operations.
```csharp
Result<int> r4 = Result.Of(() => DoSomethingRisky());
Result<int> r5 = Result.Of(riskyFunc, ex => $"Custom error: {ex.Message}");
```

### Collection Extensions
Makes collection operations safer by using Maybe instead of throwing exceptions.

#### Dictionary Extensions
Safe dictionary access that never throws KeyNotFoundException.
```csharp
// Safe dictionary access returning Maybe<T>
Maybe<Value> maybeValue = dictionary.TryFind(key);
```
#### Enumerable Extensions
Safe collection operations with explicit handling of empty sequences.
```csharp
// Safe first/last element access
Maybe<T> first = enumerable.TryFirst();
Maybe<T> last = enumerable.TryLast();

// With predicate
Maybe<T> firstMatch = enumerable.TryFirst(x => x.IsValid);
Maybe<T> lastMatch = enumerable.TryLast(x => x.IsValid);

// Filter and transform Maybe sequences
IEnumerable<T> values = maybeSequence.Choose();
IEnumerable<R> transformed = maybeSequence.Choose(x => Transform(x));
```

### Maybe Advanced Extensions
Additional utilities for working with Maybe types in more complex scenarios.
#### Value Extraction
Methods to safely extract values from Maybe with fallback options.
```csharp
// Get value or throw
T value1 = maybe.GetValueOrThrow();
T value2 = maybe.GetValueOrThrow(new CustomException());

// Get value or default
T value3 = maybe.GetValueOrDefault();
T value4 = maybe.GetValueOrDefault(defaultValue);
```
#### Fallback Handling
Chain multiple Maybe values together with fallback options.
```csharp
// Fallback to another Maybe
Maybe<T> result1 = maybe.Or(fallbackMaybe);
Maybe<T> result2 = maybe.Or(fallbackValue);
```
#### LINQ Integration
Full LINQ query syntax support for fluent Maybe operations.
```csharp
// Where, Select, SelectMany support for LINQ syntax
var result = from x in maybe
                where x > 0
                select x * 2;
```

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
