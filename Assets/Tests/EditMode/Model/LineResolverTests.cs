using System.Collections.Generic;
using NUnit.Framework;

public class LineResolverTests 
{
    public static IEnumerable<TestCaseData> DoNothingCases()
    {
        yield return new TestCaseData(
            new[] { 0, 0, 0, 0},                // 输入
            new[] { 0, 0, 0, 0},                // expected newLine
            false                               // expected 是否有修改
        );

        yield return new TestCaseData(
            new[] { 2, 0, 0, 0},                // 输入
            new[] { 2, 0, 0, 0},                // expected newLine
            false                               // expected 是否有修改
        );

        yield return new TestCaseData(
            new[] { 2, 4, 0, 0},                // 输入
            new[] { 2, 4, 0, 0},                // expected newLine
            false                               // expected 是否有修改
        );

        yield return new TestCaseData(
            new[] { 4, 2, 8, 0},                // 输入
            new[] { 4, 2, 8, 0},                // expected newLine
            false                               // expected 是否有修改
        );

        yield return new TestCaseData(
            new[] { 2, 4, 8, 16},                // 输入
            new[] { 2, 4, 8, 16},                // expected newLine
            false                                // expected 是否有修改
        );
        yield return new TestCaseData(
            new[] { 2, 4, 2, 4},                // 输入
            new[] { 2, 4, 2, 4},                // expected newLine
            false                                // expected 是否有修改
        );
    }

    public static IEnumerable<TestCaseData> MoveCases()
    {
        yield return new TestCaseData(
            new[] { 0, 2, 0, 0},                // 输入
            new[] { 2, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(1, 0)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 0, 2, 0},                // 输入
            new[] { 2, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(2, 0)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 0, 0, 2},                // 输入
            new[] { 2, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(3, 0)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 2, 4, 0},                // 输入
            new[] { 2, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(1, 0),
                LineOperation.Move(2, 1)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 2, 0, 4},                // 输入
            new[] { 2, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(1, 0),
                LineOperation.Move(3, 1)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 0, 2, 4},                // 输入
            new[] { 2, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(2, 0),
                LineOperation.Move(3, 1)
            }
        );
        yield return new TestCaseData(
            new[] { 2, 0, 4, 0},                // 输入
            new[] { 2, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(2, 1),
            }
        );
        yield return new TestCaseData(
            new[] { 2, 0, 0, 4},                // 输入
            new[] { 2, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(3, 1),
            }
        );
        yield return new TestCaseData(
            new[] { 4, 2, 0, 8},                // 输入
            new[] { 4, 2, 8, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(3, 2),
            }
        );
        yield return new TestCaseData(
            new[] { 4, 2, 0, 4},                // 输入
            new[] { 4, 2, 4, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(3, 2),
            }
        );
    }

    public static IEnumerable<TestCaseData> MergeCases()
    {
        yield return new TestCaseData(
            new[] { 2, 2, 0, 0},                // 输入
            new[] { 4, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(0, 1, 0, 4)
            }
        );
        yield return new TestCaseData(
            new[] { 4, 0, 4, 0},                // 输入
            new[] { 8, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(0, 2, 0, 8)
            }
        );
        yield return new TestCaseData(
            new[] { 8, 0, 0, 8},                // 输入
            new[] { 16, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(0, 3, 0, 16)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 2, 2, 0},                // 输入
            new[] { 4, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(1, 2, 0, 4)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 4, 0, 4},                // 输入
            new[] { 8, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(1, 3, 0, 8)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 0, 8, 8},                // 输入
            new[] { 16, 0, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(2, 3, 0, 16)
            }
        );
        // ==========================!!!!合并后的不会再次合并!!!!===================
        yield return new TestCaseData(
            new[] { 4, 2, 2, 0},                // 输入
            new[] { 4, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(1, 2, 1, 4)
            }
        );
        yield return new TestCaseData(
            new[] { 2, 0, 4, 4},                // 输入
            new[] { 2, 8, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(2, 3, 1, 8)
            }
        );
        // =========================两个合并====================
        yield return new TestCaseData(
            new[] { 2, 2, 2, 2},                // 输入
            new[] { 4, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(0, 1, 0, 4),
                LineOperation.Merge(2, 3, 1, 4)
            }
        );
    }

    public static IEnumerable<TestCaseData> MoveAndMergeCases()
    {
        // ==================合并后的不会再合并=====================
        // 先移动再合并
        yield return new TestCaseData(
            new[] { 0, 4, 2, 2},                // 输入
            new[] { 4, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Move(1, 0),
                LineOperation.Merge(2, 3, 1, 4)
            }
        );
        // 先合并再移动
        yield return new TestCaseData(
            new[] { 2, 2, 4, 0},                // 输入
            new[] { 4, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(0, 1, 0, 4),
                LineOperation.Move(2, 1)
            }
        );
        yield return new TestCaseData(
            new[] { 2, 2, 0, 8},                // 输入
            new[] { 4, 8, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(0, 1, 0, 4),
                LineOperation.Move(3, 1)
            }
        );
        yield return new TestCaseData(
            new[] { 0, 4, 4, 4},                // 输入
            new[] { 8, 4, 0, 0},                // expected newLine
            true,                              // expected 是否有修改
            new[]                               // expected 操作
            {
                LineOperation.Merge(1, 2, 0, 8),
                LineOperation.Move(3, 1)
            }
        );
    }
    

    [TestCaseSource(nameof(DoNothingCases))]
    public void Resolve_ShouldDoNothing(int[] input, int[] expectedLine, bool shouldChange)
    {
        LineResolveResult result = LineResolver.Resolve(input);

        Assert.That(result.NewLine, Is.EqualTo(expectedLine));
        Assert.That(result.HasChanged, Is.EqualTo(shouldChange));
        Assert.That(result.Operations.Count == 0);
    }

    [TestCaseSource(nameof(MoveCases))]
    public void Resolve_ShouldMove(int[] input, int[] expectedLine, bool shouldChange, LineOperation[] expectedOps)
    {
        LineResolveResult result = LineResolver.Resolve(input);

        Assert.That(result.NewLine, Is.EqualTo(expectedLine));
        Assert.That(result.HasChanged, Is.EqualTo(shouldChange));
        Assert.That(result.Operations, Is.EqualTo(expectedOps));
    }

    [TestCaseSource(nameof(MergeCases))]
    public void Resolve_ShouldMerge(int[] input, int[] expectedLine, bool shouldChange, LineOperation[] expectedOps)
    {
        LineResolveResult result = LineResolver.Resolve(input);

        Assert.That(result.NewLine, Is.EqualTo(expectedLine));
        Assert.That(result.HasChanged, Is.EqualTo(shouldChange));
        Assert.That(result.Operations, Is.EqualTo(expectedOps));
    }

    [TestCaseSource(nameof(MoveAndMergeCases))]
    public void Resolve_ShouldMoveAndMerge(int[] input, int[] expectedLine, bool shouldChange, LineOperation[] expectedOps)
    {
        LineResolveResult result = LineResolver.Resolve(input);

        Assert.That(result.NewLine, Is.EqualTo(expectedLine));
        Assert.That(result.HasChanged, Is.EqualTo(shouldChange));
        Assert.That(result.Operations, Is.EqualTo(expectedOps));
    }
}
