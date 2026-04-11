using System;
using System.Collections.Generic;

public enum LineOperationType { Move, Merge };

public readonly struct LineOperation
{
    public LineOperationType Type { get; }
    public int From1 { get; }   // Move: 起点列     Merge: 第一个起点列
    public int From2 { get; }   // Move: -1        Merge: 第二个起点列
    public int To { get; }      // 终点列
    public int Value { get; }   // 对于合并来说是合并后的值

    private LineOperation(LineOperationType type, int from1, int from2, int to, int val)
    {
        Type = type;
        From1 = from1;
        From2 = from2;
        To = to;
        Value = val;
    }

    public static LineOperation Move(int from, int to)
    {
        return new LineOperation(LineOperationType.Move, from, -1, to, -1);
    }

    public static LineOperation Merge(int from1, int from2, int to, int val)
    {
        return new LineOperation(LineOperationType.Merge, from1, from2, to, val);
    }
}

public readonly struct LineResolveResult
{
    public bool HasChanged { get; }
    public int[] NewLine { get; }
    public IReadOnlyList<LineOperation> Operations { get; }

    public LineResolveResult(bool hasChanged, int[] newLine, IReadOnlyList<LineOperation> operations)
    {
        HasChanged = hasChanged;
        NewLine = newLine;
        Operations = operations;
    }
}

// 核心算法单独拎出来，方便测试
public static class LineResolver
{
    public static LineResolveResult Resolve(int[] line)
    {
        bool hasChanged = false;
        int[] newLine = new int[line.Length];
        Array.Copy(line, newLine, line.Length);
        List<LineOperation> ops = new();

        // 非零数字提取到前面，并记录原始列索引，方便后续生成TileAction
        // <val, source column>
        var entries = new List<(int val, int sourceCol)>();
        for (int col = 0; col < line.Length; ++col)
        {
            if (line[col] != 0)
            {
                entries.Add((line[col], col));
            }
        }
        
        // 所有非零数字都在entries中，对于任意一个非零数字，要么向左合并，要么向左移动，要么不动
        // 每次尝试取read和read + 1处的数字，首先判断是否是合并。如果不是，根据sourceCol和write的关系判断是移动还是不动
        // 合并时read+2跳过两个合并的数字，移动时read+1跳过一个被移动的数字，不动时read+1跳过一个不动的数字
        int write = 0;
        for (int read = 0; read < entries.Count; ++write)
        {
            int val = entries[read].val;
            int sourceCol = entries[read].sourceCol;

            // 合并
            if (read + 1 < entries.Count && entries[read + 1].val == val)
            {
                hasChanged = true;
                newLine[write] = val * 2;
                read += 2;

                ops.Add(LineOperation.Merge(sourceCol, entries[read - 1].sourceCol, write, 2 * val));
            }
            else if (sourceCol != write) // 移动
            {
                hasChanged = true;
                newLine[write] = val;
                ++read;

                ops.Add(LineOperation.Move(sourceCol, write));
            } else // 不动
            {
                ++read;
            }
        }
        for (; write < newLine.Length; ++write)
        {
            newLine[write] = 0;
        }
        return new LineResolveResult(hasChanged, newLine, ops);
    }
}