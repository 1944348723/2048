using System.Collections.Generic;
using UnityEngine;

public class Test
{
    public static void TestPushLine()
    {
        KeyValuePair<int[], int[]>[] pushLineTestData = new KeyValuePair<int[], int[]>[]
        {
            new KeyValuePair<int[], int[]>(
                new int[] { 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 0, 0, 0 },
                new int[] { 2, 0, 0, 0 }
            ),
            // 移动单个
            new KeyValuePair<int[], int[]>(
                new int[] { 0, 2, 0, 0 },
                new int[] { 2, 0, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 0, 0, 2, 0 },
                new int[] { 2, 0, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 0, 0, 0, 2 },
                new int[] { 2, 0, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 4, 0, 0 },
                new int[] { 2, 4, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 0, 4, 0 },
                new int[] { 2, 4, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 0, 0, 4 },
                new int[] { 2, 4, 0, 0 }
            ),
            // 移动两个
            new KeyValuePair<int[], int[]>(
                new int[] { 0, 2, 4, 0 },
                new int[] { 2, 4, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 0, 2, 0, 4 },
                new int[] { 2, 4, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 0, 0, 2, 4 },
                new int[] { 2, 4, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 0, 4, 8 },
                new int[] { 2, 4, 8, 0 }
            ),
            // 全部不同不移动
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 4, 8, 16 },
                new int[] { 2, 4, 8, 16 }
            ),
            // 合并两个
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 2, 0, 0 },
                new int[] { 4, 0, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 4, 4, 0 },
                new int[] { 2, 8, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 0, 4, 4 },
                new int[] { 2, 8, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 0, 2, 2, 4 },
                new int[] { 4, 4, 0, 0 }
            ),
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 2, 2, 0 },
                new int[] { 4, 2, 0, 0 }
            ),
            // 合并4个
            new KeyValuePair<int[], int[]>(
                new int[] { 2, 2, 2, 2 },
                new int[] { 4, 4, 0, 0 }
            ),
        };

        int passCount = 0;
        foreach (var testData in pushLineTestData)
        {
            int[] input = testData.Key;
            int[] expected = testData.Value;
            Board.PushLine(input);
            int[] result = input;
            if (System.Linq.Enumerable.SequenceEqual(result, expected))
            {
                ++passCount;
            } else
            {
                Debug.LogError($"PushLine test failed. Expected: {string.Join(',', expected)}, Got: {string.Join(',', result)}");
            }
        }
        Debug.Log($"PushLine test passed {passCount}/{pushLineTestData.Length}");
    }
}
