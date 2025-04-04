[![openupm](https://img.shields.io/npm/v/com.kwanjoong.nope?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.kwanjoong.nope/)
[![License](https://img.shields.io/badge/License-MIT-brightgreen.svg)](LICENSE.md)

<div align="center" style="margin: 20px 0">
  <h3>📚 文档语言</h3>
  <a href="README.md"><img src="https://img.shields.io/badge/🇺🇸_English-Available-success?style=for-the-badge" alt="English"></a>
  <a href="README-KR.md"><img src="https://img.shields.io/badge/🇰🇷_한국어-Available-success?style=for-the-badge" alt="Korean"></a>
  <a href="README-JP.md"><img src="https://img.shields.io/badge/🇯🇵_日本語-Available-success?style=for-the-badge" alt="Japanese"></a>
  <a href="README-CN.md"><img src="https://img.shields.io/badge/🇨🇳_中文-Current-blue?style=for-the-badge" alt="Chinese"></a>
</div>

# NOPE (No Overused Possibly Evil Exceptions)

![Image 1](Documentation~/NOPE.png)

一个轻量级的、**零内存分配**的函数式扩展库，适用于Unity，灵感来自**CSharpFunctionalExtensions**。  
专注于**显式处理成功/失败**而不抛出异常，使用`Result<T,E>`和`Maybe<T>`类型实现**可选值**而不使用null。

- **同时支持同步和异步**工作流:
    - **UniTask**集成(如果安装了`Cysharp.Threading.Tasks`并设置了`NOPE_UNITASK`定义符号)。
    - **Awaitable**集成(如果使用**Unity6+**内置的`Awaitable`，使用`NOPE_AWAITABLE`定义符号)。
- 为`Result<T,E>`和`Maybe<T>`提供**完整的同步 ↔ 异步桥接**:  
  Map/Bind/Tap/Match/Finally现在拥有**"所有组合"**(同步→异步、异步→同步、异步→异步)。
- **最小化GC压力**: 实现为`readonly struct`以保持低内存分配。

> **定义符号**使用:  
> \- 在你的**项目设置**中，如果你想使用基于UniTask的异步，定义**`NOPE_UNITASK`**。  
> \- 或者定义**`NOPE_AWAITABLE`**(Unity6+)如果你想使用内置的Awaitable集成。  
> \- 如果你只计划使用同步方法，可以省略这两个定义。

---

## 目录

1. [动机和身份](#动机和身份)
2. [性能比较](#性能比较)
3. [安装](#安装)
4. [示例项目](#示例项目)
5. [快速"前后对比"](#快速前后对比)
6. [功能概述](#功能概述)
7. [Result\<T,E\>用法](#resultte用法)
    - [创建Result](#1-创建result)
    - [Combine / CombineValues](#2-combine--combinevalues)
    - [SuccessIf, FailureIf, Of](#3-successif-failureif-of)
    - [Bind, Map, MapError, Tap, Ensure, Match, Finally](#4-bind-map-maperror-tap-ensure-match-finally)
8. [Maybe\<T\>用法](#maybet用法)
    - [创建Maybe](#1-创建maybe)
    - [关键Maybe方法](#2-关键maybe方法)
    - [集合辅助方法](#3-集合辅助方法)
    - [LINQ集成](#4-linq集成)
9. [异步支持](#异步支持)
    - [NOPE_UNITASK 或 NOPE_AWAITABLE](#nope_unitask-或-nope_awaitable)
    - [同步 ↔ 异步桥接](#同步--异步桥接)
10. [使用示例](#使用示例)
11. [API参考](#api参考)
12. [许可证](#许可证)

---

## 动机和身份

**NOPE**旨在消除代码中的**隐式`null`检查**和**隐藏的异常**。取而代之，我们使用:
- **Result\<T,E\>** 用于**显式成功/失败**。
- **Maybe\<T\>** 用于可选值，类似于"可为空但没有空指针"。

因此，你可以链式安全转换(`Map`、`Bind`、`Tap`)，或处理结果(`Match`、`Finally`)，使用**干净的函数式风格**。

**目标**：使复杂代码更**可读**，更安全，并使错误处理更明确。  
**理念**：没有隐藏的异常或`null`惊喜。明确返回"**失败**"或"**无**"状态，可以带有或不带有用户定义的错误类型。

---

## 性能比较
以下性能测量是在NOPE库的功能被全面使用的环境中进行的。测试包括与`CSharpFunctionalExtensions`、`Optional`、`LanguageExt`和`OneOf`库的比较。

> 请注意，并非所有库都支持完全相同的功能。在某些情况下，比较使用了从用户角度看产生等效结果的类似函数。

![Image 2](Documentation~/Bench_Memory_250129.svg)
![Image 1](Documentation~/Bench_Time_250129.svg)


## 安装

1. **通过Git (UPM)**:  
   在`Packages/manifest.json`中:
   ```json
   {
     "dependencies": {
       "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE"
     }
   }
   ```
   要指定版本，使用:
   ```json
    {
      "dependencies": {
        "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE#1.3.2"
      }
    }
   ```
2. **Unity Package Manager (Git)**:
    1) `Window → Package Manager`
    2) "+" → "Add package from git URL…"
    3) 粘贴 `https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE`，要指定版本，附加版本标签如 `https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE#1.3.2`。

3. **OpenUPM**:  
   在CLI中，`openupm add com.kwanjoong.nope`。
3. **手动下载**:  
   克隆或下载，然后放置在`Packages/`或`Assets/Plugins`中。

> [!NOTE] 
> **定义**:
> - `NOPE_UNITASK` 用于使用**UniTask**集成
> - `NOPE_AWAITABLE` 用于Unity6+内置的**Awaitable**集成
> - 如果你只想使用同步功能，可以省略这两个定义。
> - *不要同时定义两者。*

---

## 示例项目

这个仓库包含一个演示NOPE库实际应用的示例Unity项目。要使用示例项目:

1. 克隆整个仓库:
   ```bash
   git clone https://github.com/kwan3854/Unity-NOPE.git
   ```
2. 将克隆的仓库作为Unity项目打开(仓库本身就是Unity项目)。
3. 在Unity编辑器中，导航并打开位于: `Assets/NOPE_Examples/Scene/`的示例场景。
4. 运行示例场景，查看各种NOPE库功能的实际应用。
5. 学习`Assets/NOPE_Examples/Scripts/`中的示例代码。

## 快速"前后对比"

**想象**一个函数，它检查两三个条件，异步获取一些数据，确保数据有效，然后返回成功结果或记录一些错误。

### 不使用NOPE

```csharp
public async Task<string> DoStuff()
{
    // a) 检查某个条件
    if (!CheckA()) 
        throw new Exception("Condition A failed!");

    // b) 获取数据
    var data = await FetchData(); // 可能返回null？
    if (data == null)
        return null; // ?

    // c) 解析和验证
    var parsed = Parse(data);
    if (parsed <= 0)
        return "Negative value?";

    // d) 执行最后一步
    if (!await FinalStep(parsed))
        return "Final step failed!";
    
    return "All Good!";
}
```
**问题**: 我们混合了抛出的异常、`null`、特殊字符串。很容易忘记检查或意外跳过错误路径。

### 使用NOPE

```csharp
public async UniTask<Result<string, string>> DoStuff()
{
    return await Result.SuccessIf(CheckA(), Unit.Value, "Condition A failed!")
        .Bind(_ =>  FetchData()
            .Map(data => Parse(data))
            .Ensure(x => x > 0, "Parsed <= 0?"))
        .Bind(parsed => FinalStep(parsed)
            .Map(success => success 
                ? "All Good!" 
                : "Final step failed!"));
}
```

在这里，每一步都返回一个`Result<T>`，我们使用**Bind/Map/Ensure**在**一个链**中统一成功/失败。没有`null`或抛出的异常。

---

## 功能概述

- **Result<T,E>**
    - 可链式方法: `Map`、`Bind`、`Tap`、`Ensure`、`MapError`、`Match`、`Finally`
    - 使用`Combine`(无值)或`CombineValues`(有新元组/数组)组合多个结果

- **Maybe<T>**
    - "可选"类型，无需`null`
    - `Map`、`Bind`、`Tap`、`Match`、`Where`、`Execute`等
    - LINQ集成(`Select`、`SelectMany`、`Where`)

- **同步 ↔ 异步桥接**
    - 对于每个方法(`Bind`、`Map`等)，我们有:
        - 同步→同步、同步→异步、异步→同步、异步→异步
    - 与**UniTask**(如果是`NOPE_UNITASK`)或**Awaitable**(如果是`NOPE_AWAITABLE`)配合使用
    - 因此，你可以在单个链中无缝混合同步和异步步骤。

- **集合工具**
    - 用于`Maybe<T>`: `TryFind`、`TryFirst`、`TryLast`、`Choose`等

---

## Result\<T,E\>用法

### 1) 创建Result

```csharp
// 基本成功/失败
var r1 = Result<int, string>.Success(100);
var r2 = Result<int, string>.Failure("Oops"); 

// 隐式转换
Result<int, string> r3 = 10;
Assert.IsTrue(r3.IsSuccess);
Assert.AreEqual(10, r3.Value);

Result<int, string> r4 = "Error";
Assert.IsTrue(r4.IsFailure);
Assert.AreEqual("Error", r4.Error);

var a = 100;
var b = 200;
Result<int, string> r5 = b == 0 ?
    "Divide by zero"
    : 100;
Assert.IsTrue(r5.IsSuccess);
Assert.AreEqual(100, r5.Value);

// 如果使用自定义错误类型E:
var r6 = Result<int, SomeErrorEnum>.Failure(SomeErrorEnum.FileNotFound);
```

### 2) Combine / CombineValues

1. **`Combine`**
    - 将多个`Result<T,E>`收集到单个**"无值"**`Result<Unit, E>`中(仅成功/失败)。
    - 如果**全部**成功 → 返回Success()。如果**任何一个**失败 → 返回第一个错误。

   ```csharp
    var r1 = Result<int, string>.Success(2);
    var r2 = Result<int, string>.Success(3);
    var combined = Result.Combine(r1, r2);
    
    Assert.IsTrue(combined.IsSuccess);
    Assert.AreEqual(Unit.Value, combined.Value);
    
    var r3 = Result<int, string>.Failure("Fail");
    var combined2 = Result.Combine(r1, r3);
    Assert.IsTrue(combined2.IsFailure);
    Assert.AreEqual("Fail", combined2.Error);
   ```

2. **`CombineValues`**
    - 将多个`Result<T,E>`收集到单个`Result<(T1,T2,...) , E>`或`Result<T[], E>`中。
    - 如果任何一个失败，返回该错误。否则，返回一个新组合的"值"。

   ```csharp
    var r1 = Result<int, string>.Success(2);
    var r2 = Result<int, string>.Success(3);
    var r3 = Result<int, string>.Failure("Fail");
   
    // 将两个结果合并为元组
    var combinedTuple = Result.CombineValues(r1, r2);
    Assert.IsTrue(combinedTuple.IsSuccess);
    Assert.AreEqual((2, 3), combinedTuple.Value);
   
    // 将三个结果合并为数组
    var combinedArray = Result.CombineValues(r1, r2, r3);
    Assert.IsTrue(combinedArray.IsFailure);
    Assert.AreEqual("Fail", combinedArray.Error)
   ```

### 3) SuccessIf, FailureIf, Of

- **`SuccessIf(condition, successValue, error)`**  
  → "如果条件为真 → 成功，否则 → 失败。"
- **`FailureIf(condition, successValue, error)`**  
  → "如果条件为真 → 失败，否则 → 成功。"
- **`Of(func, errorConverter)`**  
  → 包装try/catch块，如果没有异常则返回成功，否则返回fail(error)。

```csharp
var x = 10;

var r1 = Result.SuccessIf(() => x > 5, x, "TooSmall");
Assert.IsTrue(r1.IsSuccess);

var r2 = Result.FailureIf(() => x % 2 == 0, 999, "CondFailed");
Assert.IsTrue(r2.IsFailure);
Assert.AreEqual("CondFailed", r2.Error);

var r3 = Result.Of(() => x / 0, ex => $"{ex.Message} Added info");
Assert.IsTrue(r3.IsFailure);
Assert.AreEqual("Attempted to divide by zero. Added info", r3.Error);
```

### 4) Bind, Map, MapError, Tap, Ensure, Match, Finally

- **Bind**: 转换`Result<TOriginal,E>` → `Result<TNew,E>`(如果成功)，否则传递错误。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var r2 = r1.Bind(x => Result<string, string>.Success($"Value is {x}"));
    
  Assert.IsTrue(r2.IsSuccess);
  Assert.AreEqual("Value is 10", r2.Value);
    
  var r3 = Result<int, string>.Failure("Initial failure");
  var r4 = r3.Bind(x => Result<string, string>.Success($"Value is {x}"));
    
  Assert.IsTrue(r4.IsFailure);
  Assert.AreEqual("Initial failure", r4.Error);
  ```
- **Map**: 转换成功时的**值** → `Result<U,E>`，没有额外错误。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var r2 = r1.Map(x => x + 1);
  
  Assert.IsTrue(r2.IsSuccess);
  Assert.AreEqual(11, r2.Value);
  
  var r3 = Result<int, string>.Failure("Initial failure");
  var r4 = r3.Map(x => x + 1);
  
  Assert.IsTrue(r4.IsFailure);
  Assert.AreEqual("Initial failure", r4.Error);
  ```
> [!TIP]
> ## Bind vs Map
> ### Map
> 成功时的简单转换(T → U)
> ```csharp
> // mapFunc:  int => string
> string mapFunc(int x) => $"Value is {x}";
> 
> var r1 = Result<int, string>.Success(10);
> var r2 = r1.Map(mapFunc);
> 
> // r2 : Result<string, string>
> // Success => "Value is 10"
> ```
> 由于`mapFunc`本身返回一个字符串，`Map`在内部创建`Result<string, E>.Success(mapFunc(x))`。如果`mapFunc`需要产生异常或失败，这是不可能的(你必须直接抛出，这超出了Result模式)。
> ### Bind
> 成功时返回另一个Result(T → Result<U,E>)
> ```csharp
> // bindFunc:  int => Result<string,string>
> Result<string,string> bindFunc(int x)
> {
>   if (x > 5)
>     return Result<string,string>.Success($"Value is {x}");
>   else
>     return Result<string,string>.Failure("x <= 5");
> }
> 
> var r3 = Result<int,string>.Success(10);
> var r4 = r3.Bind(bindFunc);
> 
> // r4 : Result<string,string>
> // Success => "Value is 10"
> ```
> `bindFunc`包含逻辑来直接产生"成功或失败"。`Bind`的工作原理是"如果输入成功则调用`bindFunc`并返回其结果(成功或失败)"，"如果输入失败则保持现有失败"。

- **MapError**: 仅改变错误。
  ```csharp
  var r1 = Result<int, string>.Failure("Initial error");
  var r2 = r1.MapError(e => $"Custom: {e}");
  
  Assert.IsTrue(r2.IsFailure);
  Assert.AreEqual("Custom: Initial error", r2.Error);
  
  var r3 = Result<int, string>.Success(10);
  var r4 = r3.MapError(e => $"Custom: {e}");
  
  Assert.IsTrue(r4.IsSuccess);
  Assert.AreEqual(10, r4.Value);
  ```
- **Tap**: 如果成功则运行副作用。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var r2 = r1.Tap(x => Debug.Log($"Value = {x}"));
  
  Assert.IsTrue(r2.IsSuccess);
  Assert.AreEqual(10, r2.Value);
  
  var r3 = Result<int, string>.Failure("Initial failure");
  var r4 = r3.Tap(x => Debug.Log($"Value = {x}"));
  
  Assert.IsTrue(r4.IsFailure);
  Assert.AreEqual("Initial failure", r4.Error);
  ```
- **Ensure**: "如果成功但不满足谓词 => 变为fail(error)。"
  ```csharp
  var r1 = Result<int, string>.Success(15);
  var r2 = r1.Ensure(x => x > 10, "too small?");
  
  Assert.IsTrue(r2.IsSuccess);
  Assert.AreEqual(15, r2.Value);
  
  var r3 = Result<int, string>.Success(5);
  var r4 = r3.Ensure(x => x > 10, "too small?");
  
  Assert.IsTrue(r4.IsFailure);
  Assert.AreEqual("too small?", r4.Error);
  ```
- **Match**: 将`Result<T,E>`转换为单一结果:
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var result1 = r1.Match(
      onSuccess: val => $"Value = {val}",
      onFailure: err => $"Err = {err}"
  );
  
  Assert.AreEqual("Value = 10", result1);
  
  var r2 = Result<int, string>.Failure("Initial failure");
  var result2 = r2.Match(
      onSuccess: val => $"Value = {val}",
      onFailure: err => $"Err = {err}"
  );
  
  Assert.AreEqual("Err = Initial failure", result2);
  ```
- **Finally**: "链终止"与最终函数。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var finalString1 = r1.Finally(res =>
  {
      // 执行副作用
      return res.IsSuccess ? "OK" : $"Fail({res.Error})";
  });
  
  Assert.AreEqual("OK", finalString1);
  
  var r2 = Result<int, string>.Failure("Initial failure");
  var finalString2 = r2.Finally(res =>
  {
      // 执行副作用
      return res.IsSuccess ? "OK" : $"Fail({res.Error})";
  });
  
  Assert.AreEqual("Fail(Initial failure)", finalString2);
  ```

> 如果设置了`NOPE_UNITASK`/`NOPE_AWAITABLE`，所有这些方法都有**同步→异步**或**异步→异步**变体。

---

## Maybe\<T\>用法

`Maybe<T>`表示一个可选值(类似于`Nullable<T>`但没有装箱和空检查)。

```csharp
Maybe<int> m1 = 100;         // => HasValue=true
Maybe<int> m2 = Maybe<int>.None; // => 没有值
```

### 1) 创建Maybe

```csharp
// 基本创建
Maybe<int> m1 = 100;         // => HasValue=true
Maybe<int> m2 = Maybe<int>.None; // => 没有值

// 从可空类型
int? nullableInt = 10;
Maybe<int?> m3 = Maybe<int?>.From(nullableInt); // => HasValue=true
Assert.IsTrue(m3.HasValue);

nullableInt = null;
Maybe<int?> m4 = Maybe<int?>.From(nullableInt); // => 没有值
Assert.IsFalse(m4.HasValue);
```

### 2) 关键Maybe方法

- **Map**: 如果值存在则转换。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<string> m2 = m1.Map(x => $"Value is {x}");
  
  Assert.IsTrue(m2.HasValue);
  Assert.AreEqual("Value is 10", m2.Value);
  
  Maybe<int> m3 = Maybe<int>.None;
  Maybe<string> m4 = m3.Map(x => $"Value is {x}");
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Bind**: 将值转换为另一个`Maybe<T>`。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<string> m2 = m1.Bind(x => Maybe<string>.From($"Value is {x}"));
  
  Assert.IsTrue(m2.HasValue);
  Assert.AreEqual("Value is 10", m2.Value);
  
  Maybe<int> m3 = Maybe<int>.None;
  Maybe<string> m4 = m3.Bind(x => Maybe<string>.From($"Value is {x}"));
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Tap**: 如果值存在则运行副作用。
  ```csharp
  Maybe<int> m1 = 10;
  m1.Tap(x => Console.WriteLine($"Value = {x}"));
  
  Maybe<int> m2 = Maybe<int>.None;
  m2.Tap(x => Console.WriteLine($"Value = {x}")); // 无输出
  ```

- **Match**: 将`Maybe<T>`转换为单一结果。
  ```csharp
  Maybe<int> m1 = 10;
  string result1 = m1.Match(
      onValue: val => $"Value = {val}",
      onNone: () => "No value"
  );
  
  Assert.AreEqual("Value = 10", result1);
  
  Maybe<int> m2 = Maybe<int>.None;
  string result2 = m2.Match(
      onValue: val => $"Value = {val}",
      onNone: () => "No value"
  );
  
  Assert.AreEqual("No value", result2);
  ```

- **Where**: 如果`HasValue`但不满足谓词，变为None。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<int> m2 = m1.Where(x => x > 5);
  
  Assert.IsTrue(m2.HasValue);
  
  Maybe<int> m3 = 3;
  Maybe<int> m4 = m3.Where(x => x > 5);
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Execute**: 如果Maybe<T>有值则执行动作。
  ```csharp
    Maybe<int> m1 = 10;
    m1.Execute(val => Console.WriteLine($"This will print: {val}"));
    Assert.AreEqual(10, m1.Value);
    
    Maybe<int> m2 = Maybe<int>.None;
    m2.Execute(val => Console.WriteLine($"This will not print: {val}"));
    Assert.IsFalse(m2.HasValue);
  ```

- **Or**: 如果是None则提供后备值。
  ```csharp
    Maybe<int> m1 = 10;
    Maybe<int> maybeValue1 = m1.Or(0);
  
    Assert.AreEqual(10, maybeValue1.Value);
  
    Maybe<int> m2 = Maybe<int>.None;
    var maybeValue2 = m2.Or(0);
  
    Assert.AreEqual(0, maybeValue2.Value);
  ```

- **GetValueOrThrow**, **GetValueOrDefault**: 用于直接提取。
  ```csharp
  Maybe<int> m1 = 10;
  int value1 = m1.GetValueOrThrow();
  
  Assert.AreEqual(10, value1);
  
  Maybe<int> m2 = Maybe<int>.None;
  int value2 = m2.GetValueOrDefault(0);
  
  Assert.AreEqual(0, value2);
  ```

### 3) 集合辅助方法

我们提供返回`Maybe<T>`的**集合**辅助方法:

- `dict.TryFind(key) -> Maybe<TValue>`
  ```csharp
  Dictionary<string, int> dict = new() { { "apple", 10 }, { "banana", 5 } };
  Maybe<int> found = dict.TryFind("banana");
  
  Assert.IsTrue(found.HasValue);
  Assert.AreEqual(5, found.Value);
  
  Maybe<int> notFound = dict.TryFind("cherry");
  
  Assert.IsFalse(notFound.HasValue);
  ```

- `source.TryFirst()`, `source.TryLast()` → Maybe<T>
  ```csharp
  List<int> list = new() { 1, 2, 3 };
  Maybe<int> first = list.TryFirst();
  
  Assert.IsTrue(first.HasValue);
  Assert.AreEqual(1, first.Value);
  
  Maybe<int> last = list.TryLast();
  
  Assert.IsTrue(last.HasValue);
  Assert.AreEqual(3, last.Value);
  
  List<int> emptyList = new();
  Maybe<int> none = emptyList.TryFirst();
  
  Assert.IsFalse(none.HasValue);
  ```

- `Choose(...)`从`Maybe<T>`序列中过滤掉None。
  ```csharp
  List<Maybe<int>> list = new() { Maybe<int>.From(1), Maybe<int>.None, Maybe<int>.From(3) };
  List<int> chosen = list.Choose().ToList();
  
  Assert.AreEqual(2, chosen.Count);
  Assert.AreEqual(1, chosen[0]);
  Assert.AreEqual(3, chosen[1]);
  ```

### 4) LINQ集成

我们有`Select`, `SelectMany`, `Where`，所以你可以这样做:
```csharp
Maybe<int> maybeNum = 50;
var query =
    from x in maybeNum
    where x > 10
    select x * 2;
// => Maybe(100)
```

这个详细解释现在应该与`Result<T,E>`部分相当。

---

## 异步支持

### NOPE_UNITASK 或 NOPE_AWAITABLE

如果你定义**`NOPE_UNITASK`**，我们为Map/Bind等添加`UniTask<Result<T,E>>` / `UniTask<Maybe<T>>`重载。  
如果你定义**`NOPE_AWAITABLE`**(Unity6+)，我们添加`Awaitable<Result<T,E>>` / `Awaitable<Maybe<T>>`重载。

### 同步 ↔ 异步桥接

```csharp
// syncResult + asyncBinder
public static async UniTask<Result<TNew>> Bind<T,TNew>(
   this Result<T> result,
   Func<T, UniTask<Result<TNew>>> asyncBinder);

public static async Awaitable<Result<TNew>> Bind<T,TNew>(
   this Result<T> result,
   Func<T, Awaitable<Result<TNew>>> asyncBinder);
```

所以你可以无缝地将同步步骤链接到异步步骤。类似地，我们也有**asyncResult + sync transform**重载。

---

## 使用示例

1. **链接多个检查 & 异步调用**与`Result<int>`:
   ```csharp
    public async UniTask<string> ComplexOperation()
    {
        return await Result.SuccessIf(CheckA(), 0, "CheckA failed!")
            .Bind(_ => FetchDataAsync()) // => UniTask<Result<string>>
            .Ensure(str => !string.IsNullOrEmpty(str), "Empty data!")
            .Map(str => str.Length)
            .Bind(FinalStepAsync)
            .Match(
                onSuccess: val => $"Final OK: {val}",
                onFailure: err => $"Failure: {err}"
            );
    }
   ```

2. **Maybe用法**与字典:
   ```csharp
   Dictionary<string,int> dict = new() {
     {"apple", 10}, {"banana", 5}
   };
   var found = dict.TryFind("banana")
       .Where(x => x >= 5)
       .Map(x => x*2) // => Maybe(10)
       .Execute(value => Debug.Log("HasValue: " + value))
       .ExecuteNoValue(() => Debug.LogWarning("Not found or zero"));
   
   // found => Maybe(10)
   ```

3. **Combine / CombineValues**:
   ```csharp
    var r1 = Result<int, string>.Success(2);
    var r2 = Result<int, string>.Success(3);
    var merged = Result.CombineValues(r1, r2);
    // => Result<(int,int)>.Success((2,3))
   
    var justCheck = Result.Combine(r1, r2);
    // => Result.Success()或第一个错误
   ```

4. **LINQ与Maybe**:
   ```csharp
   Maybe<int> maybeNum = 10;
   var query =
       from x in maybeNum
       where x > 5
       select x*3;
   // => Maybe(30)
   ```

---

## API参考

**Result\<T,E\>**
- **Combine** / **CombineValues**
- **SuccessIf**, **FailureIf**, **Of**
- **Bind**, **Map**, **MapError**, **Tap**, **Ensure**, **Match**, **Finally**
- **BindSafe**, **MapSafe**, **TapSafe**
- 用于同步→异步桥接的重载。

**Maybe\<T\>**
- **Map**, **Bind**, **Tap**, **Match**, **Finally**
- **Where**, **Execute**, **Or**, **GetValueOrThrow**等
- 从集合中的**TryFind**, **TryFirst**, **TryLast**, **Choose**。
- LINQ运算符: **Select**, **SelectMany**, **Where**。

> 完整列表，请参见`NOPE.Runtime.Core.Result` / `NOPE.Runtime.Core.Maybe`中的`.cs`文件。

---

## 许可证

**MIT**许可证。  
欢迎贡献和拉取请求。

---