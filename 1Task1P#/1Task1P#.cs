using System;
using System.Collections.Generic;

class Program
{
    static Random random = new Random();

    static int GetRandomNumber()
    {
        return random.Next(0, 101);
    }

    static List<int> FindIncreasingSequence(List<int> row)
    {
        List<int> bestSequence = new List<int>();
        List<int> currentSequence = new List<int>();

        for (int i = 0; i < row.Count; ++i)
        {
            currentSequence.Clear();
            currentSequence.Add(row[i]);

            for (int j = i + 1; j < row.Count; ++j)
            {
                if (row[j] > currentSequence[currentSequence.Count - 1])
                {
                    currentSequence.Add(row[j]);
                }
            }

            if (bestSequence.Count == 0 || 
                (currentSequence[0] < bestSequence[0]) ||
                (currentSequence[0] == bestSequence[0] && currentSequence.Count > bestSequence.Count))
            {
                bestSequence = new List<int>(currentSequence);
            }
        }

        return bestSequence;
    }

    static void Main(string[] args)
    {
        const int M = 5;
        const int N = 10;

        List<List<int>> matrix = new List<List<int>>();

        for (int i = 0; i < M; ++i)
        {
            matrix.Add(new List<int>());
            for (int j = 0; j < N; ++j)
            {
                matrix[i].Add(GetRandomNumber());
            }
        }

        Console.WriteLine("Полученная матрица:");
        foreach (var row in matrix)
        {
            foreach (int num in row)
            {
                Console.Write(num + " ");
            }
            Console.WriteLine();
        }

        List<int> resultingSequence = new List<int>();

        foreach (var row in matrix)
        {
            List<int> sequence = FindIncreasingSequence(row);

            if (resultingSequence.Count == 0 ||
                (sequence.Count > 0 && sequence[0] < resultingSequence[0]) ||
                (sequence.Count > 0 && sequence[0] == resultingSequence[0] && sequence.Count > resultingSequence.Count))
            {
                resultingSequence = new List<int>(sequence);
            }
        }

        Console.WriteLine("Возрастающая последовательность с минимальным элементом:");
        foreach (int num in resultingSequence)
        {
            Console.Write(num + " ");
        }
        Console.WriteLine();
    }
}
