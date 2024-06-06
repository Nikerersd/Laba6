using System;
using System.Collections.Generic;

class Program
{
    static void LU(List<List<double>> A, List<double> B, ref List<double> x_k)
    {
        int n = A.Count;
        List<List<double>> L = new List<List<double>>();
        List<List<double>> U = new List<List<double>>();

        for (int i = 0; i < n; i++)
        {
            L.Add(new List<double>(new double[n]));
            U.Add(new List<double>(new double[n]));
        }

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (j < i)
                {
                    L[j][i] = 0;
                }
                else
                {
                    L[j][i] = A[j][i];
                    for (int k = 0; k < i; k++)
                    {
                        L[j][i] -= L[j][k] * U[k][i];
                    }
                }
            }

            for (int j = 0; j < n; j++)
            {
                if (j < i)
                {
                    U[i][j] = 0;
                }
                else if (j == i)
                {
                    U[i][j] = A[i][j];
                    for (int k = 0; k < i; k++)
                    {
                        U[i][j] -= L[i][k] * U[k][j];
                    }
                }
                else
                {
                    U[i][j] = A[i][j];
                    for (int k = 0; k < i; k++)
                    {
                        U[i][j] -= L[i][k] * U[k][j];
                    }
                    U[i][j] /= L[i][i];
                }
            }

            // Вывод L и U после каждой итерации
            Console.WriteLine($"\nL - матрица ({i + 1} итерация):");
            for (int a = 0; a < n; a++)
            {
                for (int b = 0; b < n; b++)
                {
                    if (a > b)
                    {
                        Console.Write($"{L[a][b]}\t");
                    }
                    else if (a == b)
                    {
                        Console.Write($"1\t");
                    }
                    else
                    {
                        Console.Write($"0\t");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine($"\nU - матрица ({i + 1} итерация):");
            for (int a = 0; a < n; a++)
            {
                for (int b = 0; b < n; b++)
                {
                    if (a <= b)
                    {
                        Console.Write($"{U[a][b]}\t");
                    }
                    else
                    {
                        Console.Write($"0\t");
                    }
                }
                Console.WriteLine();
            }
        }

        // Решение системы LUx = RIGHT
        for (int i = 0; i < n; i++)
        {
            x_k[i] = B[i];
            for (int j = 0; j < i; j++)
            {
                x_k[i] -= L[i][j] * x_k[j];
            }
            x_k[i] /= L[i][i];
        }

        for (int i = n - 1; i >= 0; i--)
        {
            for (int j = i + 1; j < n; j++)
            {
                x_k[i] -= U[i][j] * x_k[j];
            }
        }

        Console.WriteLine("\nРешение системы:");
        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"x{i + 1} = {x_k[i]}");
        }
    }

    static void SimpleIteration(List<List<double>> C, ref List<double> f, ref List<double> x, int n, double epsilon)
    {
        List<double> xNew = new List<double>(new double[n]);
        int k = 0;
        double maxDiff = 0.0;

        for (int i = 0; i < n; i++)
        {
            x[i] = 0.0;
        }

        Console.WriteLine("N\t   x1\t   x2\t   x3\t   x4\t   εn");

        do
        {
            // Вычисляем новое приближение
            for (int i = 0; i < n; i++)
            {
                double sum = f[i];
                for (int j = 0; j < n; j++)
                {
                    sum += C[i][j] * x[j];
                }
                xNew[i] = sum;
            }

            // Проверяем условие остановки
            maxDiff = 0.0;
            for (int i = 0; i < n; i++)
            {
                if (Math.Abs(Math.Abs(xNew[i]) - Math.Abs(x[i])) > maxDiff)
                {
                    maxDiff = Math.Abs(Math.Abs(xNew[i]) - Math.Abs(x[i]));
                }
                x[i] = xNew[i];
            }
            k++;

            // Вывод результатов в таблицу
            Console.WriteLine($"{k}\t{string.Join("\t", x)}\t{maxDiff}");
        } while (maxDiff > epsilon);

        Console.WriteLine($"Число итераций: {k}");

        // Вывод сообщения о сходимости или расходимости
        if (maxDiff <= epsilon)
        {
            Console.WriteLine("Метод сходится.");
        }
        else
        {
            Console.WriteLine("Метод расходится.");
        }
    }

    static void Main()
    {
        List<List<double>> A = new List<List<double>>
        {
            new List<double> {0.89, -0.04, 0.21, -18},
            new List<double> {0.25, -1.23, 0.08, -0.09},
            new List<double> {-0.21, 0.08, 0.8, -0.13},
            new List<double> {0.15, -1.31, 0.06, -1.21}
        };

        List<double> B = new List<double> { -1.24, -1.21, 2.56, 0.89 };

        List<double> x = new List<double>(new double[4]);

        LU(A, B, ref x);

        // Преобразование A и b для метода простой итерации
        for (int k = 0; k < 3; k++)
        {
            for (int i = k + 1; i < 4; i++)
            {
                double factor = A[i][k] / A[k][k];
                for (int j = k; j < 4; j++)
                {
                    A[i][j] -= factor * A[k][j];
                }
                B[i] -= factor * B[k];
            }
        }

        List<List<double>> C = new List<List<double>>();
        for (int i = 0; i < 4; i++)
        {
            C.Add(new List<double>(new double[4]));
            for (int j = 0; j < 4; j++)
            {
                if (i != j)
                {
                    C[i].Add(-A[i][j] / A[i][i]); // Заполняем матрицу C
                }
                else
                {
                    C[i].Add(0.0); // Диагональные элементы должны быть нулевыми
                }
            }
        }


        List<double> f = new List<double>(new double[4]);
        for (int i = 0; i < 4; i++)
        {
            f[i] = B[i] / A[i][i];
        }

        SimpleIteration(C, ref f, ref x, 4, 0.001);
    }
}
