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

一个轻量级、**零GC分配**的函数式扩展库，为Unity设计，灵感来源于**CSharpFunctionalExtensions**。  
专注于通过`Result<T,E>`和`Maybe<T>`类型来**明确处理成功/失败**而不抛出异常，同时处理**可选值**而不使用null。

- **同时支持同步和异步**工作流:
    - 如果安装了`Cysharp.Threading.Tasks`并设置了`NOPE_UNITASK`符号，则可与**UniTask**无缝集成。
    - 如果使用**Unity6+**并设置了`NOPE_AWAITABLE`符号，则可使用内置的**Awaitable**功能。
- 为`Result<T,E>`和`Maybe<T>`提供**完整的同步 ↔ 异步桥接**:  
  Map/Bind/Tap/Match/Finally现在支持**"所有组合"**（同步→异步、异步→同步、异步→异步）。
- **极低GC压力**: 实现为`readonly struct`以减少内存分配。

> **定义符号**使用说明:  
> \- 如果想使用基于UniTask的异步功能，在**项目设置**中定义**`NOPE_UNITASK`**。  
> \- 如果使用**Unity6+**并想要内置Awaitable集成，则定义**`NOPE_AWAITABLE`**。  
> \- 如果只计划使用同步方法，可以不添加这两个定义符号。

---

## 目录

1. [设计理念](#设计理念)
2. [性能对比](#性能对比)
3. [安装方法](#安装方法)
4. [示例项目](#示例项目)
5. [快速对比](#快速对比)
6. [功能概览](#功能概览)
7. [Result\<T,E\>使用指南](#resultte使用指南)
    - [创建Result](#1-创建result)
    - [Combine / CombineValues](#2-combine--combinevalues)
    - [SuccessIf, FailureIf, Of](#3-successif-failureif-of)
    - [Bind, Map, MapError, Tap, Ensure, Match, Finally](#4-bind-map-maperror-tap-ensure-match-finally)
8. [Maybe\<T\>使用指南](#maybet使用指南)
    - [创建Maybe](#1-创建maybe)
    - [核心Maybe方法](#2-核心maybe方法)
    - [集合辅助方法](#3-集合辅助方法)
    - [LINQ集成](#4-linq集成)
9. [异步支持](#异步支持)
    - [NOPE_UNITASK 与 NOPE_AWAITABLE](#nope_unitask-与-nope_awaitable)
    - [同步 ↔ 异步桥接](#同步--异步桥接)
10. [使用示例](#使用示例)
11. [API参考](#api参考)
12. [许可证](#许可证)

---

## 设计理念

**NOPE**旨在消除代码中的**隐式`null`检查**和**隐藏的异常**。我们使用:
- **Result\<T,E\>** 用于**明确表示成功/失败**。
- **Maybe\<T\>** 用于处理可选值，类似于"可为空但不会导致空指针异常"的类型。

通过这种方式，你可以使用**简洁、函数式的风格**链式调用安全的转换方法（`Map`、`Bind`、`Tap`）或处理结果（`Match`、`Finally`）。

**目标**：让复杂代码更**易读**、更安全，并使错误处理更加明确。  
**理念**：没有隐藏的异常或`null`带来的意外。明确返回"**失败**"或"**无值**"状态，可以选择是否使用自定义错误类型。

---

## 性能对比
以下性能测试是在全面使用NOPE库功能的环境中进行的。测试包括与`CSharpFunctionalExtensions`、`Optional`、`LanguageExt`和`OneOf`等库的比较。

> 请注意，并非所有库都提供完全相同的功能。在某些情况下，我们比较了从用户角度看效果相似的功能。

![Image 2](Documentation~/Bench_Memory_250129.svg)
![Image 1](Documentation~/Bench_Time_250129.svg)


## 安装方法

1. **通过Git (UPM)**:  
   在`Packages/manifest.json`中添加:
   ```json
   {
     "dependencies": {
       "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE"
     }
   }
   ```
   指定版本可以使用:
   ```json
    {
      "dependencies": {
        "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE#1.3.2"
      }
    }
   ```
2. **Unity包管理器 (Git)**:
    1) 打开`Window → Package Manager`
    2) 点击"+" → "Add package from git URL…"
    3) 粘贴 `https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE`，若要指定版本，附加版本标签如 `https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE#1.3.2`。

3. **OpenUPM**:  
   在命令行中运行 `openupm add com.kwanjoong.nope`。
3. **手动下载**:  
   克隆或下载仓库，然后放置在`Packages/`或`Assets/Plugins`文件夹中。

> [!NOTE] 
> **定义符号说明**:
> - 使用`NOPE_UNITASK`启用**UniTask**集成
> - 使用`NOPE_AWAITABLE`启用Unity6+内置的**Awaitable**集成
> - 如果只需要同步功能，可以不添加任何定义符号
> - *不要同时定义两者*

---

## 示例项目

本仓库包含一个演示NOPE库实际应用的Unity示例项目。使用示例项目的步骤:

1. 克隆整个仓库:
   ```bash
   git clone https://github.com/kwan3854/Unity-NOPE.git
   ```
2. 在Unity中打开克隆的仓库（仓库本身就是一个Unity项目）。
3. 在Unity编辑器中，导航并打开位于`Assets/NOPE_Examples/Scene/`的示例场景。
4. 运行示例场景，体验NOPE库的各种功能。
5. 学习`Assets/NOPE_Examples/Scripts/`中的示例代码。

## 快速对比

**想象**一个函数，它需要检查几个条件，异步获取数据，确保数据有效，然后返回成功结果或记录错误。

### 不使用NOPE

```csharp
public async Task<string> DoStuff()
{
    // a) 检查条件
    if (!CheckA()) 
        throw new Exception("Condition A failed!");

    // b) 获取数据
    var data = await FetchData(); // 可能返回null？
    if (data == null)
        return null; // 不够明确

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
**问题**: 混合了异常抛出、`null`和特殊字符串返回值。容易遗漏检查或意外跳过错误处理。

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

使用NOPE后，每一步都返回`Result<T>`，我们通过**Bind/Map/Ensure**在**一个链**中统一处理成功/失败。没有`null`或抛出的异常。

---

## 功能概览

- **Result<T,E>**
    - 链式方法: `Map`、`Bind`、`Tap`、`Ensure`、`MapError`、`Match`、`Finally`
    - 使用`Combine`(无值)或`CombineValues`(生成新元组/数组)组合多个结果

- **Maybe<T>**
    - "可选"类型，无需使用`null`
    - 提供`Map`、`Bind`、`Tap`、`Match`、`Where`、`Execute`等方法
    - 集成LINQ接口(`Select`、`SelectMany`、`Where`)

- **同步 ↔ 异步桥接**
    - 对于每个方法(`Bind`、`Map`等)，提供:
        - 同步→同步、同步→异步、异步→同步、异步→异步的转换
    - 与**UniTask**(`NOPE_UNITASK`)或**Awaitable**(`NOPE_AWAITABLE`)集成
    - 可以在单个链中无缝混合同步和异步操作

- **集合工具**
    - 针对`Maybe<T>`提供: `TryFind`、`TryFirst`、`TryLast`、`Choose`等

---

## Result\<T,E\>使用指南

### 1) 创建Result

```csharp
// 基本的成功/失败创建
var r1 = Result<int, string>.Success(100);
var r2 = Result<int, string>.Failure("Oops"); 

// 隐式转换
Result<int, string> r3 = 10;  // 成功
Assert.IsTrue(r3.IsSuccess);
Assert.AreEqual(10, r3.Value);

Result<int, string> r4 = "Error";  // 失败
Assert.IsTrue(r4.IsFailure);
Assert.AreEqual("Error", r4.Error);

var a = 100;
var b = 200;
Result<int, string> r5 = b == 0 ?
    "除数不能为零"  // 失败时的错误消息
    : 100;  // 成功时的值
Assert.IsTrue(r5.IsSuccess);
Assert.AreEqual(100, r5.Value);

// 使用自定义错误类型E:
var r6 = Result<int, SomeErrorEnum>.Failure(SomeErrorEnum.FileNotFound);
```

### 2) Combine / CombineValues

1. **`Combine`**
    - 将多个`Result<T,E>`合并为单个**"无值"**的`Result<Unit, E>`（仅表示成功/失败）。
    - 如果**全部**成功 → 返回Success()。如果**任一**失败 → 返回第一个错误。

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
    - 将多个`Result<T,E>`合并为单个`Result<(T1,T2,...) , E>`或`Result<T[], E>`。
    - 如果有任何一个失败，返回该错误。否则，返回合并后的新值。

   ```csharp
    var r1 = Result<int, string>.Success(2);
    var r2 = Result<int, string>.Success(3);
    var r3 = Result<int, string>.Failure("Fail");
   
    // 合并两个结果为元组
    var combinedTuple = Result.CombineValues(r1, r2);
    Assert.IsTrue(combinedTuple.IsSuccess);
    Assert.AreEqual((2, 3), combinedTuple.Value);
   
    // 合并三个结果为数组
    var combinedArray = Result.CombineValues(r1, r2, r3);
    Assert.IsTrue(combinedArray.IsFailure);
    Assert.AreEqual("Fail", combinedArray.Error)
   ```

### 3) SuccessIf, FailureIf, Of

- **`SuccessIf(condition, successValue, error)`**  
  → "条件为真时返回成功，否则返回失败"
- **`FailureIf(condition, successValue, error)`**  
  → "条件为真时返回失败，否则返回成功"
- **`Of(func, errorConverter)`**  
  → 包装try/catch块，如果无异常则返回成功，否则转换异常并返回失败

```csharp
var x = 10;

var r1 = Result.SuccessIf(() => x > 5, x, "值太小");
Assert.IsTrue(r1.IsSuccess);

var r2 = Result.FailureIf(() => x % 2 == 0, 999, "条件失败");
Assert.IsTrue(r2.IsFailure);
Assert.AreEqual("条件失败", r2.Error);

var r3 = Result.Of(() => x / 0, ex => $"{ex.Message} 附加信息");
Assert.IsTrue(r3.IsFailure);
Assert.AreEqual("尝试除以零。附加信息", r3.Error);
```

### 4) Bind, Map, MapError, Tap, Ensure, Match, Finally

- **Bind**: 如果成功，将`Result<TOriginal,E>` → `Result<TNew,E>`，否则保持错误不变。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var r2 = r1.Bind(x => Result<string, string>.Success($"值是 {x}"));
    
  Assert.IsTrue(r2.IsSuccess);
  Assert.AreEqual("值是 10", r2.Value);
    
  var r3 = Result<int, string>.Failure("初始失败");
  var r4 = r3.Bind(x => Result<string, string>.Success($"值是 {x}"));
    
  Assert.IsTrue(r4.IsFailure);
  Assert.AreEqual("初始失败", r4.Error);
  ```
- **Map**: 如果成功，转换**值**并返回 → `Result<U,E>`，不添加新错误。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var r2 = r1.Map(x => x + 1);
  
  Assert.IsTrue(r2.IsSuccess);
  Assert.AreEqual(11, r2.Value);
  
  var r3 = Result<int, string>.Failure("初始失败");
  var r4 = r3.Map(x => x + 1);
  
  Assert.IsTrue(r4.IsFailure);
  Assert.AreEqual("初始失败", r4.Error);
  ```
> [!TIP]
> ## Bind 与 Map 的区别
> ### Map
> 成功时的简单值转换 (T → U)
> ```csharp
> // mapFunc:  int => string
> string mapFunc(int x) => $"值是 {x}";
> 
> var r1 = Result<int, string>.Success(10);
> var r2 = r1.Map(mapFunc);
> 
> // r2 : Result<string, string>
> // 成功 => "值是 10"
> ```
> 由于`mapFunc`本身返回字符串，`Map`内部会创建`Result<string, E>.Success(mapFunc(x))`。如果`mapFunc`需要产生异常或失败结果，是做不到的（只能直接抛出异常，但这违背了Result模式的初衷）。
> ### Bind
> 成功时返回另一个Result (T → Result<U,E>)
> ```csharp
> // bindFunc:  int => Result<string,string>
> Result<string,string> bindFunc(int x)
> {
>   if (x > 5)
>     return Result<string,string>.Success($"值是 {x}");
>   else
>     return Result<string,string>.Failure("x <= 5");
> }
> 
> var r3 = Result<int,string>.Success(10);
> var r4 = r3.Bind(bindFunc);
> 
> // r4 : Result<string,string>
> // 成功 => "值是 10"
> ```
> `bindFunc`包含直接产生"成功或失败"的逻辑。`Bind`的工作方式是"如果输入成功则调用`bindFunc`并返回其结果（成功或失败）"，"如果输入失败则保持现有失败状态"。

- **MapError**: 只修改错误信息。
  ```csharp
  var r1 = Result<int, string>.Failure("初始错误");
  var r2 = r1.MapError(e => $"自定义: {e}");
  
  Assert.IsTrue(r2.IsFailure);
  Assert.AreEqual("自定义: 初始错误", r2.Error);
  
  var r3 = Result<int, string>.Success(10);
  var r4 = r3.MapError(e => $"自定义: {e}");
  
  Assert.IsTrue(r4.IsSuccess);
  Assert.AreEqual(10, r4.Value);
  ```
- **Tap**: 成功时执行副作用，不改变结果。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var r2 = r1.Tap(x => Debug.Log($"值 = {x}"));
  
  Assert.IsTrue(r2.IsSuccess);
  Assert.AreEqual(10, r2.Value);
  
  var r3 = Result<int, string>.Failure("初始失败");
  var r4 = r3.Tap(x => Debug.Log($"值 = {x}"));  // 不会执行
  
  Assert.IsTrue(r4.IsFailure);
  Assert.AreEqual("初始失败", r4.Error);
  ```
- **Ensure**: "如果成功但不满足条件 => 变为失败(error)"。
  ```csharp
  var r1 = Result<int, string>.Success(15);
  var r2 = r1.Ensure(x => x > 10, "值太小");
  
  Assert.IsTrue(r2.IsSuccess);
  Assert.AreEqual(15, r2.Value);
  
  var r3 = Result<int, string>.Success(5);
  var r4 = r3.Ensure(x => x > 10, "值太小");
  
  Assert.IsTrue(r4.IsFailure);
  Assert.AreEqual("值太小", r4.Error);
  ```
- **Match**: 将`Result<T,E>`转换为单一结果:
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var result1 = r1.Match(
      onSuccess: val => $"值 = {val}",
      onFailure: err => $"错误 = {err}"
  );
  
  Assert.AreEqual("值 = 10", result1);
  
  var r2 = Result<int, string>.Failure("初始失败");
  var result2 = r2.Match(
      onSuccess: val => $"值 = {val}",
      onFailure: err => $"错误 = {err}"
  );
  
  Assert.AreEqual("错误 = 初始失败", result2);
  ```
- **Finally**: "链式终止"，提供最终处理函数。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var finalString1 = r1.Finally(res =>
  {
      // 执行副作用
      return res.IsSuccess ? "成功" : $"失败({res.Error})";
  });
  
  Assert.AreEqual("成功", finalString1);
  
  var r2 = Result<int, string>.Failure("初始失败");
  var finalString2 = r2.Finally(res =>
  {
      // 执行副作用
      return res.IsSuccess ? "成功" : $"失败({res.Error})";
  });
  
  Assert.AreEqual("失败(初始失败)", finalString2);
  ```
- **Or**: 如果当前Result是失败，提供一个备用的Result<T,E>。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var r2 = Result<int, string>.Success(20);
  var result1 = r1.Or(r2);
  
  Assert.IsTrue(result1.IsSuccess);
  Assert.AreEqual(10, result1.Value);  // 原始成功值
  
  var r3 = Result<int, string>.Failure("第一个错误");
  var r4 = Result<int, string>.Success(30);
  var result2 = r3.Or(r4);
  
  Assert.IsTrue(result2.IsSuccess);
  Assert.AreEqual(30, result2.Value);  // 备用值
  
  var r5 = Result<int, string>.Failure("第一个错误");
  var r6 = Result<int, string>.Failure("第二个错误");
  var result3 = r5.Or(r6);
  
  Assert.IsTrue(result3.IsFailure);
  Assert.AreEqual("第二个错误", result3.Error);  // 备用错误
  ```
- **OrElse**: 如果当前Result是失败，通过函数提供一个备用的Result<T,E>。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var result1 = r1.OrElse(() => Result<int, string>.Success(100));
  
  Assert.IsTrue(result1.IsSuccess);
  Assert.AreEqual(10, result1.Value);  // 原始值
  
  var r2 = Result<int, string>.Failure("错误");
  var result2 = r2.OrElse(() => Result<int, string>.Success(100));
  
  Assert.IsTrue(result2.IsSuccess);
  Assert.AreEqual(100, result2.Value);  // 备用值
  
  // 备用函数只在需要时执行
  var r3 = Result<int, string>.Success(10);
  var executionCount = 0;
  var result3 = r3.OrElse(() => 
  {
      executionCount++;
      return Result<int, string>.Success(100);
  });
  
  Assert.AreEqual(0, executionCount);  // 未执行
  Assert.AreEqual(10, result3.Value);
  ```

> 如果定义了`NOPE_UNITASK`/`NOPE_AWAITABLE`，所有这些方法都有**同步→异步**或**异步→异步**的变体。

---

## Maybe\<T\>使用指南

`Maybe<T>`表示一个可选值（类似于`Nullable<T>`但没有装箱和空检查的问题）。

```csharp
Maybe<int> m1 = 100;         // => HasValue=true
Maybe<int> m2 = Maybe<int>.None; // => 无值
```

### 1) 创建Maybe

```csharp
// 基本创建方法
Maybe<int> m1 = 100;         // => HasValue=true
Maybe<int> m2 = Maybe<int>.None; // => 无值

// 从可空类型创建
int? nullableInt = 10;
Maybe<int?> m3 = Maybe<int?>.From(nullableInt); // => HasValue=true
Assert.IsTrue(m3.HasValue);

nullableInt = null;
Maybe<int?> m4 = Maybe<int?>.From(nullableInt); // => 无值
Assert.IsFalse(m4.HasValue);
```

### 2) 核心Maybe方法

- **Map**: 如果有值则转换该值。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<string> m2 = m1.Map(x => $"值是 {x}");
  
  Assert.IsTrue(m2.HasValue);
  Assert.AreEqual("值是 10", m2.Value);
  
  Maybe<int> m3 = Maybe<int>.None;
  Maybe<string> m4 = m3.Map(x => $"值是 {x}");
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Bind**: 将值转换为另一个`Maybe<T>`。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<string> m2 = m1.Bind(x => Maybe<string>.From($"值是 {x}"));
  
  Assert.IsTrue(m2.HasValue);
  Assert.AreEqual("值是 10", m2.Value);
  
  Maybe<int> m3 = Maybe<int>.None;
  Maybe<string> m4 = m3.Bind(x => Maybe<string>.From($"值是 {x}"));
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Tap**: 如果有值则执行副作用，不改变原值。
  ```csharp
  Maybe<int> m1 = 10;
  m1.Tap(x => Console.WriteLine($"值 = {x}"));
  
  Maybe<int> m2 = Maybe<int>.None;
  m2.Tap(x => Console.WriteLine($"值 = {x}")); // 不会输出任何内容
  ```

- **Match**: 将`Maybe<T>`转换为单一结果。
  ```csharp
  Maybe<int> m1 = 10;
  string result1 = m1.Match(
      onValue: val => $"值 = {val}",
      onNone: () => "无值"
  );
  
  Assert.AreEqual("值 = 10", result1);
  
  Maybe<int> m2 = Maybe<int>.None;
  string result2 = m2.Match(
      onValue: val => $"值 = {val}",
      onNone: () => "无值"
  );
  
  Assert.AreEqual("无值", result2);
  ```

- **Where**: 如果`HasValue`但不满足条件，则变为None。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<int> m2 = m1.Where(x => x > 5);
  
  Assert.IsTrue(m2.HasValue);
  
  Maybe<int> m3 = 3;
  Maybe<int> m4 = m3.Where(x => x > 5);
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Execute**: 如果Maybe<T>有值则执行操作。
  ```csharp
    Maybe<int> m1 = 10;
    m1.Execute(val => Console.WriteLine($"将会打印: {val}"));
    Assert.AreEqual(10, m1.Value);
    
    Maybe<int> m2 = Maybe<int>.None;
    m2.Execute(val => Console.WriteLine($"不会打印: {val}"));
    Assert.IsFalse(m2.HasValue);
  ```

- **Or**: 如果无值则提供默认值。
  ```csharp
    Maybe<int> m1 = 10;
    Maybe<int> maybeValue1 = m1.Or(0);
  
    Assert.AreEqual(10, maybeValue1.Value);
  
    Maybe<int> m2 = Maybe<int>.None;
    var maybeValue2 = m2.Or(0);
  
    Assert.AreEqual(0, maybeValue2.Value);
  ```

- **GetValueOrThrow**, **GetValueOrDefault**: 直接提取值。
  ```csharp
  Maybe<int> m1 = 10;
  int value1 = m1.GetValueOrThrow(); // 有值，返回10
  
  Assert.AreEqual(10, value1);
  
  Maybe<int> m2 = Maybe<int>.None;
  int value2 = m2.GetValueOrDefault(0); // 无值，返回提供的默认值0
  
  Assert.AreEqual(0, value2);
  ```

- **OrElse**: 如果无值，通过函数提供备用的Maybe<T>。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<int> result1 = m1.OrElse(() => Maybe<int>.From(100));
  
  Assert.AreEqual(10, result1.Value);  // 原始值
  
  Maybe<int> m2 = Maybe<int>.None;
  Maybe<int> result2 = m2.OrElse(() => Maybe<int>.From(100));
  
  Assert.AreEqual(100, result2.Value);  // 备用值
  
  // 当Maybe无值时也可以返回Result<T,E>
  Maybe<int> m3 = Maybe<int>.None;
  Result<int, string> result3 = m3.OrElse(() => 
      Result<int, string>.Failure("未找到值"));
  
  Assert.IsTrue(result3.IsFailure);
  ```

- **ToResult**: 将Maybe<T>转换为Result<T,E>，无值时使用提供的错误。
  ```csharp
  Maybe<int> m1 = 10;
  Result<int, string> result1 = m1.ToResult("无值");
  
  Assert.IsTrue(result1.IsSuccess);
  Assert.AreEqual(10, result1.Value);
  
  Maybe<int> m2 = Maybe<int>.None;
  Result<int, string> result2 = m2.ToResult("无值");
  
  Assert.IsTrue(result2.IsFailure);
  Assert.AreEqual("无值", result2.Error);
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

- `Choose(...)`从`Maybe<T>`序列中过滤出有值的项目。
  ```csharp
  List<Maybe<int>> list = new() { Maybe<int>.From(1), Maybe<int>.None, Maybe<int>.From(3) };
  List<int> chosen = list.Choose().ToList();
  
  Assert.AreEqual(2, chosen.Count);
  Assert.AreEqual(1, chosen[0]);
  Assert.AreEqual(3, chosen[1]);
  ```

### 4) LINQ集成

我们实现了`Select`, `SelectMany`, `Where`接口，因此你可以使用LINQ查询语法:
```csharp
Maybe<int> maybeNum = 50;
var query =
    from x in maybeNum
    where x > 10
    select x * 2;
// => Maybe(100)
```

---

## 异步支持

### NOPE_UNITASK 与 NOPE_AWAITABLE

如果定义了**`NOPE_UNITASK`**，我们会为Map/Bind等方法添加`UniTask<Result<T,E>>` / `UniTask<Maybe<T>>`重载。  
如果定义了**`NOPE_AWAITABLE`**(Unity6+)，我们会添加`Awaitable<Result<T,E>>` / `Awaitable<Maybe<T>>`重载。

### 同步 ↔ 异步桥接

```csharp
// 同步结果 + 异步绑定函数
public static async UniTask<Result<TNew>> Bind<T,TNew>(
   this Result<T> result,
   Func<T, UniTask<Result<TNew>>> asyncBinder);

public static async Awaitable<Result<TNew>> Bind<T,TNew>(
   this Result<T> result,
   Func<T, Awaitable<Result<TNew>>> asyncBinder);
```

这样你可以无缝地将同步步骤链接到异步步骤。类似地，我们也提供了**异步结果 + 同步转换**的重载。

---

## 使用示例

1. **链式处理多个检查和异步调用**，使用`Result<int>`:
   ```csharp
    public async UniTask<string> ComplexOperation()
    {
        return await Result.SuccessIf(CheckA(), 0, "CheckA失败!")
            .Bind(_ => FetchDataAsync()) // => UniTask<Result<string>>
            .Ensure(str => !string.IsNullOrEmpty(str), "数据为空!")
            .Map(str => str.Length)
            .Bind(FinalStepAsync)
            .Match(
                onSuccess: val => $"成功: {val}",
                onFailure: err => $"失败: {err}"
            );
    }
   ```

2. **字典中使用Maybe**:
   ```csharp
   Dictionary<string,int> dict = new() {
     {"apple", 10}, {"banana", 5}
   };
   var found = dict.TryFind("banana")
       .Where(x => x >= 5)
       .Map(x => x*2) // => Maybe(10)
       .Execute(value => Debug.Log("有值: " + value))
       .ExecuteNoValue(() => Debug.LogWarning("未找到或值为零"));
   
   // found => Maybe(10)
   ```

3. **Combine / CombineValues**:
   ```csharp
    var r1 = Result<int, string>.Success(2);
    var r2 = Result<int, string>.Success(3);
    var merged = Result.CombineValues(r1, r2);
    // => Result<(int,int)>.Success((2,3))
   
    var justCheck = Result.Combine(r1, r2);
    // => Result.Success() 或返回第一个错误
   ```

4. **LINQ与Maybe结合使用**:
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
- **Bind**, **Map**, **MapError**, **Tap**, **Ensure**, **Match**, **Finally**, **Or**, **OrElse**
- **BindSafe**, **MapSafe**, **TapSafe**
- 用于同步→异步桥接的重载

**Maybe\<T\>**
- **Map**, **Bind**, **Tap**, **Match**, **Finally**
- **Where**, **Execute**, **Or**, **OrElse**, **ToResult**, **GetValueOrThrow**, **GetValueOrDefault**
- 集合操作: **TryFind**, **TryFirst**, **TryLast**, **Choose**
- LINQ操作符: **Select**, **SelectMany**, **Where**

> 完整API列表请查看`NOPE.Runtime.Core.Result` / `NOPE.Runtime.Core.Maybe`中的源代码文件

---

## 许可证

**MIT**许可证  
欢迎贡献和提交PR

---