using System;
using System.Collections.Generic;

class Program
{
    static Random random = new Random();

    static int GetRandomNumber()
    {
        return random.Next(-50, 51); // Генерация случайного числа от -50 до 50
    }

    static void Main()
    {
        const int M = 5;
        const int N = 5;

        int[][] matrix = new int[M][];

        for (int i = 0; i < M; i++)
        {
            matrix[i] = new int[N];
            for (int j = 0; j < N; j++)
            {
                matrix[i][j] = GetRandomNumber();
            }
        }

        Console.WriteLine("Матрица:");
        foreach (var row in matrix)
        {
            foreach (var elem in row)
            {
                Console.Write(elem + " ");
            }
            Console.WriteLine();
        }

        Dictionary<int, int> frequency = new Dictionary<int, int>();

        foreach (var row in matrix)
        {
            foreach (var elem in row)
            {
                if (frequency.ContainsKey(elem))
                    frequency[elem]++;
                else
                    frequency.Add(elem, 1);
            }
        }

        Console.WriteLine("\nЧастота уникальных элементов:");
        foreach (var pair in frequency)
        {
            Console.WriteLine($"Элемент {pair.Key} встречается {pair.Value} раз(а).");
        }
    }
}
