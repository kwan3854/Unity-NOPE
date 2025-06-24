[![openupm](https://img.shields.io/npm/v/com.kwanjoong.nope?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.kwanjoong.nope/)
[![License](https://img.shields.io/badge/License-MIT-brightgreen.svg)](LICENSE.md)

<div align="center" style="margin: 20px 0">
  <h3>📚 ドキュメント言語</h3>
  <a href="README.md"><img src="https://img.shields.io/badge/🇺🇸_English-Available-success?style=for-the-badge" alt="English"></a>
  <a href="README-KR.md"><img src="https://img.shields.io/badge/🇰🇷_한국어-Available-success?style=for-the-badge" alt="Korean"></a>
  <a href="README-JP.md"><img src="https://img.shields.io/badge/🇯🇵_日本語-Current-blue?style=for-the-badge" alt="Japanese"></a>
  <a href="README-CN.md"><img src="https://img.shields.io/badge/🇨🇳_中文-Available-success?style=for-the-badge" alt="Chinese"></a>
</div>

# NOPE (No Overused Possibly Evil Exceptions)

![Image 1](Documentation~/NOPE.png)

**CSharpFunctionalExtensions**に触発された、Unityのための軽量な**ゼロアロケーション**関数型拡張ライブラリです。  
例外を投げることなく**成功/失敗を明示的に扱い**、nullを使わずに**オプショナル値**を扱う`Result<T,E>`と`Maybe<T>`型に焦点を当てています。

- **同期**と**非同期**の両方のワークフローを**サポート**:
    - **UniTask**連携（`Cysharp.Threading.Tasks`がインストールされ、`NOPE_UNITASK`が定義されている場合）
    - **Awaitable**連携（**Unity6+**で内蔵の`Awaitable`を使用し、`NOPE_AWAITABLE`を定義した場合）
- `Result<T,E>`と`Maybe<T>`の両方に対する**完全な同期 ↔ 非同期ブリッジング**:  
  Map/Bind/Tap/Match/Finallyなどが**「すべての組み合わせ」**（同期→非同期、非同期→同期、非同期→非同期）に対応
- **最小限のGCプレッシャー**: メモリ割り当てを抑えるために`readonly struct`として実装

> **定義シンボル**の使い方:  
> \- **プロジェクト設定**で、UniTaskベースの非同期を使いたい場合は**`NOPE_UNITASK`**を定義してください  
> \- あるいは内蔵のAwaitable連携を使いたい場合は**`NOPE_AWAITABLE`**（Unity6+）を定義してください  
> \- 同期メソッドのみを使う予定なら、両方の定義を省略できます  
> \- *両方を同時に定義しないでください*

---

## 目次

1. [背景と特徴](#背景と特徴)
2. [パフォーマンス比較](#パフォーマンス比較)
3. [インストール方法](#インストール方法)
4. [サンプルプロジェクト](#サンプルプロジェクト)
5. [簡単な「ビフォー＆アフター」](#簡単なビフォーアフター)
6. [機能概要](#機能概要)
7. [Result\<T,E\>の使い方](#resultte-の使い方)
    - [Resultの作成](#1-resultの作成)
    - [Combine / CombineValues](#2-combine--combinevalues)
    - [SuccessIf, FailureIf, Of](#3-successif-failureif-of)
    - [Bind, Map, MapError, Tap, Ensure, Match, Finally](#4-bind-map-maperror-tap-ensure-match-finally)
8. [Maybe\<T\>の使い方](#maybet-の使い方)
    - [Maybeの作成](#1-maybeの作成)
    - [主要なMaybeメソッド](#2-主要なmaybeメソッド)
    - [コレクションヘルパー](#3-コレクションヘルパー)
    - [LINQ連携](#4-linq連携)
9. [非同期サポート](#非同期サポート)
    - [NOPE_UNITASK または NOPE_AWAITABLE](#nope_unitask-または-nope_awaitable)
    - [同期 ↔ 非同期ブリッジング](#同期--非同期ブリッジング)
10. [使用例](#使用例)
11. [APIリファレンス](#apiリファレンス)
12. [ライセンス](#ライセンス)

---

## 背景と特徴

**NOPE**は、コードから**暗黙的な`null`チェック**と**隠れた例外**をなくすことを目指しています。代わりに以下を使用します:
- **明示的な成功/失敗**のための**Result\<T,E\>**
- オプショナル値のための**Maybe\<T\>**（nullポインタなしでnullableのような機能を実現）

これにより、安全な変換（`Map`、`Bind`、`Tap`）をチェーンしたり、結果を処理（`Match`、`Finally`）したりすることが、**クリーンな関数型スタイル**で可能になります。

**目標**: 複雑なコードをより**読みやすく**、安全に、そしてエラー処理を明示的にすること  
**思想**: 隠れた例外や`null`による予期せぬ問題をなくす。カスタムエラータイプの有無にかかわらず、「**失敗**」または「**存在しない**」状態を明示的に返す

---

## パフォーマンス比較
以下のパフォーマンス測定は、NOPEライブラリの機能を網羅的に使用した環境で行われました。`CSharpFunctionalExtensions`、`Optional`、`LanguageExt`、`OneOf`ライブラリとの比較を含みます。

> すべてのライブラリが同じ機能を完全にサポートしているわけではありません。一部のケースでは、ユーザー視点で同等の結果を生み出す類似の機能で比較しています。

![Image 2](Documentation~/Bench_Memory_250129.svg)
![Image 1](Documentation~/Bench_Time_250129.svg)


## インストール方法

1. **Git (UPM)経由**:  
   `Packages/manifest.json`に以下を追加:
   ```json
   {
     "dependencies": {
       "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE"
     }
   }
   ```
   特定のバージョンを指定する場合:
   ```json
    {
      "dependencies": {
        "com.kwanjoong.nope": "https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE#1.3.2"
      }
    }
   ```
2. **Unity Package Manager (Git)**:
    1) `Window → Package Manager`を開く
    2) "+" → "Add package from git URL…"をクリック
    3) `https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE`を貼り付け。特定のバージョンを使用するには`https://github.com/kwan3854/Unity-NOPE.git?path=/Packages/Unity-NOPE#1.3.2`のようにバージョンタグを追加します。

3. **OpenUPM**:  
   コマンドラインで`openupm add com.kwanjoong.nope`を実行。
3. **手動ダウンロード**:  
   リポジトリをクローンまたはダウンロードし、`Packages/`または`Assets/Plugins`に配置。

> [!NOTE] 
> **定義シンボル**:
> - **UniTask**連携を使うには`NOPE_UNITASK`
> - Unity6+の内蔵**Awaitable**連携を使うには`NOPE_AWAITABLE`
> - 同期メソッドのみを使う場合は、両方とも省略可能
> - *両方を同時に定義しないでください*

---

## サンプルプロジェクト

このリポジトリには、NOPEライブラリの実際の使い方を示すサンプルUnityプロジェクトが含まれています。サンプルプロジェクトの使い方:

1. リポジトリ全体をクローン:
   ```bash
   git clone https://github.com/kwan3854/Unity-NOPE.git
   ```
2. クローンしたリポジトリをUnityプロジェクトとして開く（リポジトリ自体がUnityプロジェクトです）
3. Unityエディタで、次の場所にあるサンプルシーンを開く: `Assets/NOPE_Examples/Scene/`
4. サンプルシーンを実行して、様々なNOPEライブラリの機能を確認
5. `Assets/NOPE_Examples/Scripts/`にあるサンプルコードを参考に学習

## 簡単な「ビフォー＆アフター」

**想像してみてください**：いくつかの条件をチェックし、非同期でデータを取得し、データの有効性を確認して、成功結果を返すか、エラーをログに記録する関数が必要な場面。

### NOPEなし

```csharp
public async Task<string> DoStuff()
{
    // a) 条件のチェック
    if (!CheckA()) 
        throw new Exception("Condition A failed!");

    // b) データの取得
    var data = await FetchData(); // nullを返す可能性があるか？
    if (data == null)
        return null; // ?

    // c) パース＆検証
    var parsed = Parse(data);
    if (parsed <= 0)
        return "Negative value?";

    // d) 最終ステップ
    if (!await FinalStep(parsed))
        return "Final step failed!";
    
    return "All Good!";
}
```
**問題**: スローされた例外、`null`、特殊な文字列が混在しています。チェックを忘れたり、誤ってエラーパスをスキップしたりする可能性があります。

### NOPEあり

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

ここでは、各ステップが`Result<T>`を返し、**Bind/Map/Ensure**を使用して**1つのチェーン**で成功/失敗を統合します。`null`やスローされた例外はありません。

---

## 機能概要

- **Result<T,E>**
    - チェーン可能なメソッド: `Map`、`Bind`、`Tap`、`Ensure`、`MapError`、`Match`、`Finally`
    - `Combine`（値なし）または`CombineValues`（新しいタプル/配列付き）で複数の結果を結合

- **Maybe<T>**
    - 「オプショナル」型、`null`不要
    - `Map`、`Bind`、`Tap`、`Match`、`Where`、`Execute`など
    - LINQ連携（`Select`、`SelectMany`、`Where`）

- **同期 ↔ 非同期ブリッジング**
    - すべてのメソッド（`Bind`、`Map`など）に対して:
        - 同期→同期、同期→非同期、非同期→同期、非同期→非同期
    - **UniTask**（`NOPE_UNITASK`の場合）または**Awaitable**（`NOPE_AWAITABLE`の場合）で動作
    - 単一のチェーンで同期ステップと非同期ステップをシームレスに混在させることができます。

- **コレクションユーティリティ**
    - `Maybe<T>`用: `TryFind`、`TryFirst`、`TryLast`、`Choose`など

---

## Result\<T,E\>の使い方

### 1) Resultの作成

```csharp
// 基本的な成功/失敗
var r1 = Result<int, string>.Success(100);
var r2 = Result<int, string>.Failure("Oops"); 

// 暗黙的な変換
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

// カスタムエラータイプEを使用する場合:
var r6 = Result<int, SomeErrorEnum>.Failure(SomeErrorEnum.FileNotFound);
```

### 2) Combine / CombineValues

1. **`Combine`**
    - 複数の`Result<T,E>`を単一の**「値のない」**`Result<Unit, E>`（成功/失敗のみ）に集約します。
    - **すべて**成功 → Success()を返します。**いずれか**が失敗 → 最初のエラーを返します。

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
    - 複数の`Result<T,E>`を単一の`Result<(T1,T2,...) , E>`または`Result<T[], E>`に集約します。
    - いずれかが失敗した場合、そのエラーを返します。それ以外の場合は、新しい結合された「値」を返します。

   ```csharp
    var r1 = Result<int, string>.Success(2);
    var r2 = Result<int, string>.Success(3);
    var r3 = Result<int, string>.Failure("Fail");
   
    // 2つの結果をタプルに結合
    var combinedTuple = Result.CombineValues(r1, r2);
    Assert.IsTrue(combinedTuple.IsSuccess);
    Assert.AreEqual((2, 3), combinedTuple.Value);
   
    // 3つの結果を配列に結合
    var combinedArray = Result.CombineValues(r1, r2, r3);
    Assert.IsTrue(combinedArray.IsFailure);
    Assert.AreEqual("Fail", combinedArray.Error)
   ```

### 3) SuccessIf, FailureIf, Of

- **`SuccessIf(condition, successValue, error)`**  
  → 「条件が真 → 成功、そうでなければ → 失敗」。
- **`FailureIf(condition, successValue, error)`**  
  → 「条件が真 → 失敗、そうでなければ → 成功」。
- **`Of(func, errorConverter)`**  
  → try/catchブロックをラップし、例外がなければ成功を返し、そうでなければfail(error)を返します。

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

- **Bind**: 成功時に`Result<TOriginal,E>` → `Result<TNew,E>`に変換し、失敗時はエラーをそのまま渡します。
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
- **Map**: 成功時に**値**を変換 → `Result<U,E>`、追加のエラーなし。
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
> 成功時の単純な変換（T → U）
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
> `mapFunc`自体が文字列を返すため、`Map`は内部的に`Result<string, E>.Success(mapFunc(x))`を作成します。`mapFunc`が例外や失敗を生成する必要がある場合、これは不可能です（直接throwする必要があり、これはResultパターンの外部にあります）。
> ### Bind
> 成功時に別のResult（T → Result<U,E>）
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
> `bindFunc`は「成功または失敗」を直接生成するロジックを含みます。`Bind`は「入力が成功の場合は`bindFunc`を呼び出してその結果（成功または失敗）を返す」、「入力が失敗の場合は既存の失敗を維持する」という方法で動作します。

- **MapError**: エラーのみを変更します。
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
- **Tap**: 成功時に副作用を実行します。
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
- **Ensure**: 「成功だが条件を満たさない → fail(error)になる」。
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
- **Match**: `Result<T,E>`を単一の結果に変換します:
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
- **Finally**: 最終関数による「チェーン終了」。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var finalString1 = r1.Finally(res =>
  {
      // 副作用を実行
      return res.IsSuccess ? "OK" : $"Fail({res.Error})";
  });
  
  Assert.AreEqual("OK", finalString1);
  
  var r2 = Result<int, string>.Failure("Initial failure");
  var finalString2 = r2.Finally(res =>
  {
      // 副作用を実行
      return res.IsSuccess ? "OK" : $"Fail({res.Error})";
  });
  
  Assert.AreEqual("Fail(Initial failure)", finalString2);
  ```
- **Or**: 現在のResultが失敗の場合、フォールバックのResult<T,E>を提供します。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var r2 = Result<int, string>.Success(20);
  var result1 = r1.Or(r2);
  
  Assert.IsTrue(result1.IsSuccess);
  Assert.AreEqual(10, result1.Value);  // 元の成功値
  
  var r3 = Result<int, string>.Failure("最初のエラー");
  var r4 = Result<int, string>.Success(30);
  var result2 = r3.Or(r4);
  
  Assert.IsTrue(result2.IsSuccess);
  Assert.AreEqual(30, result2.Value);  // フォールバック値
  
  var r5 = Result<int, string>.Failure("最初のエラー");
  var r6 = Result<int, string>.Failure("2番目のエラー");
  var result3 = r5.Or(r6);
  
  Assert.IsTrue(result3.IsFailure);
  Assert.AreEqual("2番目のエラー", result3.Error);  // フォールバックエラー
  ```
- **OrElse**: 現在のResultが失敗の場合、関数を通じてフォールバックのResult<T,E>を提供します。
  ```csharp
  var r1 = Result<int, string>.Success(10);
  var result1 = r1.OrElse(() => Result<int, string>.Success(100));
  
  Assert.IsTrue(result1.IsSuccess);
  Assert.AreEqual(10, result1.Value);  // 元の値
  
  var r2 = Result<int, string>.Failure("エラー");
  var result2 = r2.OrElse(() => Result<int, string>.Success(100));
  
  Assert.IsTrue(result2.IsSuccess);
  Assert.AreEqual(100, result2.Value);  // フォールバック値
  
  // フォールバック関数は必要な場合にのみ実行されます
  var r3 = Result<int, string>.Success(10);
  var executionCount = 0;
  var result3 = r3.OrElse(() => 
  {
      executionCount++;
      return Result<int, string>.Success(100);
  });
  
  Assert.AreEqual(0, executionCount);  // 実行されない
  Assert.AreEqual(10, result3.Value);
  ```

> これらのすべてのメソッドは、`NOPE_UNITASK`/`NOPE_AWAITABLE`が設定されている場合、**同期 → 非同期**または**非同期 → 非同期**のバリアントを持ちます。

---

## Maybe\<T\>の使い方

`Maybe<T>`はオプショナル値を表します（ボクシングなしの`Nullable<T>`のようなもので、nullチェックがありません）。

```csharp
Maybe<int> m1 = 100;         // => HasValue=true
Maybe<int> m2 = Maybe<int>.None; // => 値なし
```

### 1) Maybeの作成

```csharp
// 基本的な作成
Maybe<int> m1 = 100;         // => HasValue=true
Maybe<int> m2 = Maybe<int>.None; // => 値なし

// nullableタイプから
int? nullableInt = 10;
Maybe<int?> m3 = Maybe<int?>.From(nullableInt); // => HasValue=true
Assert.IsTrue(m3.HasValue);

nullableInt = null;
Maybe<int?> m4 = Maybe<int?>.From(nullableInt); // => 値なし
Assert.IsFalse(m4.HasValue);
```

### 2) 主要なMaybeメソッド

- **Map**: 値が存在する場合に変換します。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<string> m2 = m1.Map(x => $"Value is {x}");
  
  Assert.IsTrue(m2.HasValue);
  Assert.AreEqual("Value is 10", m2.Value);
  
  Maybe<int> m3 = Maybe<int>.None;
  Maybe<string> m4 = m3.Map(x => $"Value is {x}");
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Bind**: 値を別の`Maybe<T>`に変換します。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<string> m2 = m1.Bind(x => Maybe<string>.From($"Value is {x}"));
  
  Assert.IsTrue(m2.HasValue);
  Assert.AreEqual("Value is 10", m2.Value);
  
  Maybe<int> m3 = Maybe<int>.None;
  Maybe<string> m4 = m3.Bind(x => Maybe<string>.From($"Value is {x}"));
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Tap**: 値が存在する場合に副作用を実行します。
  ```csharp
  Maybe<int> m1 = 10;
  m1.Tap(x => Console.WriteLine($"Value = {x}"));
  
  Maybe<int> m2 = Maybe<int>.None;
  m2.Tap(x => Console.WriteLine($"Value = {x}")); // 出力なし
  ```

- **Match**: `Maybe<T>`を単一の結果に変換します。
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

- **Where**: `HasValue`があるが条件を満たさない場合、Noneになります。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<int> m2 = m1.Where(x => x > 5);
  
  Assert.IsTrue(m2.HasValue);
  
  Maybe<int> m3 = 3;
  Maybe<int> m4 = m3.Where(x => x > 5);
  
  Assert.IsFalse(m4.HasValue);
  ```

- **Execute**: Maybe<T>に値がある場合にアクションを実行します。
  ```csharp
    Maybe<int> m1 = 10;
    m1.Execute(val => Console.WriteLine($"This will print: {val}"));
    Assert.AreEqual(10, m1.Value);
    
    Maybe<int> m2 = Maybe<int>.None;
    m2.Execute(val => Console.WriteLine($"This will not print: {val}"));
    Assert.IsFalse(m2.HasValue);
  ```

- **Or**: Noneの場合にフォールバック値を提供します。
  ```csharp
    Maybe<int> m1 = 10;
    Maybe<int> maybeValue1 = m1.Or(0);
  
    Assert.AreEqual(10, maybeValue1.Value);
  
    Maybe<int> m2 = Maybe<int>.None;
    var maybeValue2 = m2.Or(0);
  
    Assert.AreEqual(0, maybeValue2.Value);
  ```

- **GetValueOrThrow**, **GetValueOrDefault**: 直接抽出のため。
  ```csharp
  Maybe<int> m1 = 10;
  int value1 = m1.GetValueOrThrow();
  
  Assert.AreEqual(10, value1);
  
  Maybe<int> m2 = Maybe<int>.None;
  int value2 = m2.GetValueOrDefault(0);
  
  Assert.AreEqual(0, value2);
  ```

- **OrElse**: Noneの場合、関数を通じてフォールバックのMaybe<T>を提供します。
  ```csharp
  Maybe<int> m1 = 10;
  Maybe<int> result1 = m1.OrElse(() => Maybe<int>.From(100));
  
  Assert.AreEqual(10, result1.Value);  // 元の値
  
  Maybe<int> m2 = Maybe<int>.None;
  Maybe<int> result2 = m2.OrElse(() => Maybe<int>.From(100));
  
  Assert.AreEqual(100, result2.Value);  // フォールバック値
  
  // MaybeがNoneの場合、Result<T,E>を返すこともできます
  Maybe<int> m3 = Maybe<int>.None;
  Result<int, string> result3 = m3.OrElse(() => 
      Result<int, string>.Failure("値が見つかりません"));
  
  Assert.IsTrue(result3.IsFailure);
  ```

- **ToResult**: Maybe<T>をResult<T,E>に変換し、Noneの場合のエラーを指定します。
  ```csharp
  Maybe<int> m1 = 10;
  Result<int, string> result1 = m1.ToResult("値なし");
  
  Assert.IsTrue(result1.IsSuccess);
  Assert.AreEqual(10, result1.Value);
  
  Maybe<int> m2 = Maybe<int>.None;
  Result<int, string> result2 = m2.ToResult("値なし");
  
  Assert.IsTrue(result2.IsFailure);
  Assert.AreEqual("値なし", result2.Error);
  ```

### 3) コレクションヘルパー

`Maybe<T>`を返す**コレクション**ヘルパーを提供します:

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

- `Choose(...)`で`Maybe<T>`シーケンスからNoneをフィルタリングします。
  ```csharp
  List<Maybe<int>> list = new() { Maybe<int>.From(1), Maybe<int>.None, Maybe<int>.From(3) };
  List<int> chosen = list.Choose().ToList();
  
  Assert.AreEqual(2, chosen.Count);
  Assert.AreEqual(1, chosen[0]);
  Assert.AreEqual(3, chosen[1]);
  ```

### 4) LINQ連携

`Select`, `SelectMany`, `Where`が用意されているので、以下のようなことができます:
```csharp
Maybe<int> maybeNum = 50;
var query =
    from x in maybeNum
    where x > 10
    select x * 2;
// => Maybe(100)
```

これで、この詳細な説明は`Result<T,E>`セクションと同等のレベルになりました。

---

## 非同期サポート

### NOPE_UNITASK または NOPE_AWAITABLE

**`NOPE_UNITASK`**を定義すると、Map/Bindなどに`UniTask<Result<T,E>>` / `UniTask<Maybe<T>>`オーバーロードが追加されます。  
**`NOPE_AWAITABLE`**（Unity6+）を定義すると、`Awaitable<Result<T,E>>` / `Awaitable<Maybe<T>>`オーバーロードが追加されます。

### 同期 ↔ 非同期ブリッジング

```csharp
// syncResult + asyncBinder
public static async UniTask<Result<TNew>> Bind<T,TNew>(
   this Result<T> result,
   Func<T, UniTask<Result<TNew>>> asyncBinder);

public static async Awaitable<Result<TNew>> Bind<T,TNew>(
   this Result<T> result,
   Func<T, Awaitable<Result<TNew>>> asyncBinder);
```

これにより、同期ステップを非同期ステップにシームレスにチェーンできます。同様に、**asyncResult + sync transform**オーバーロードもあります。

---

## 使用例

1. **複数のチェック＆非同期呼び出しをチェーン**する（`Result<int>`を使用）:
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

2. **ディクショナリとMaybeの使用**:
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
    // => Result.Success()または最初のエラー
   ```

4. **MaybeとLINQの使用**:
   ```csharp
   Maybe<int> maybeNum = 10;
   var query =
       from x in maybeNum
       where x > 5
       select x*3;
   // => Maybe(30)
   ```

---

## APIリファレンス

**Result\<T,E\>**
- **Combine** / **CombineValues**
- **SuccessIf**, **FailureIf**, **Of**
- **Bind**, **Map**, **MapError**, **Tap**, **Ensure**, **Match**, **Finally**, **Or**, **OrElse**
- **BindSafe**, **MapSafe**, **TapSafe**
- 同期→非同期ブリッジング用のオーバーロード。

**Maybe\<T\>**
- **Map**, **Bind**, **Tap**, **Match**, **Finally**
- **Where**, **Execute**, **Or**, **OrElse**, **ToResult**, **GetValueOrThrow**など
- コレクションからの**TryFind**, **TryFirst**, **TryLast**, **Choose**。
- LINQ演算子: **Select**, **SelectMany**, **Where**。

> 完全なリストについては、`NOPE.Runtime.Core.Result` / `NOPE.Runtime.Core.Maybe`の`.cs`ファイルを参照してください。

---

## ライセンス

**MIT**ライセンス。  
貢献とプルリクエストを歓迎します。

---