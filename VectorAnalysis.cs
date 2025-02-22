using System;
using System.Numerics;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите размерность квадратной матрицы (NxN): ");
        int n = int.Parse(Console.ReadLine());

        Complex[,] matrix = new Complex[n, n];
        Complex[] results = new Complex[n];

        Console.WriteLine("Введите коэффициенты матрицы (n x n): ");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                while (true)
                {
                    Console.Write($"a[{i + 1},{j + 1}]: ");
                    if (TryParseComplex(Console.ReadLine(), out matrix[i, j]))
                    {
                        break; // Успешный ввод, выходим из цикла
                    }
                    else
                    {
                        Console.WriteLine("Некорректный ввод. Пожалуйста, введите комплексное число.");
                    }
                }
            }
        }

        Console.WriteLine("Введите свободные члены (n): ");
        for (int i = 0; i < n; i++)
        {
            while (true)
            {
                Console.Write($"b[{i + 1}]: ");
                if (TryParseComplex(Console.ReadLine(), out results[i]))
                {
                    break; // Успешный ввод, выходим из цикла
                }
                else
                {
                    Console.WriteLine("Некорректный ввод. Пожалуйста, введите комплексное число.");
                }
            }
        }

        try
        {
            Complex[] solution = SolveUsingCramersRule(matrix, results, n);

            Console.WriteLine("Решение системы:");
            for (int i = 0; i < n; i++)
            {
                Console.WriteLine($"x[{i + 1}] = {solution[i]}");
            }
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static bool TryParseComplex(string input, out Complex result)
    {
        result = Complex.Zero;
        string[] parts = input.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 2)
        {
            if (double.TryParse(parts[0], out double realPart) &&
                double.TryParse(parts[1].Replace("i", "").Trim(), out double imaginaryPart))
            {
                result = new Complex(realPart, imaginaryPart);
                return true;
            }
        }
        else if (parts.Length == 1)
        {
            if (double.TryParse(parts[0], out double realPartOnly))
            {
                result = new Complex(realPartOnly, 0);
                return true;
            }
        }

        return false;
    }

    static Complex[] SolveUsingCramersRule(Complex[,] matrix, Complex[] results, int n)
    {
        Complex det = Determinant(matrix, n);
        if (det == Complex.Zero)
        {
            throw new InvalidOperationException("Определитель матрицы равен нулю. Система не имеет единственного решения.");
        }

        Complex[] solution = new Complex[n];

        for (int i = 0; i < n; i++)
        {
            Complex[,] tempMatrix = (Complex[,])matrix.Clone();
            for (int j = 0; j < n; j++)
            {
                tempMatrix[j, i] = results[j];
            }
            solution[i] = Determinant(tempMatrix, n) / det;
        }

        return solution;
    }

    static Complex Determinant(Complex[,] matrix, int n)
    {
        if (n == 1)
        {
            return matrix[0, 0];
        }

        Complex det = Complex.Zero;

        for (int p = 0; p < n; p++)
        {
            Complex[,] subMatrix = new Complex[n - 1, n - 1];
            for (int i = 1; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (j < p)
                    {
                        subMatrix[i - 1, j] = matrix[i, j];
                    }
                    else if (j > p)
                    {
                        subMatrix[i - 1, j - 1] = matrix[i, j];
                    }
                }
            }
            det += (p % 2 == 0 ? Complex.One : Complex.One * -1) * matrix[0, p] * Determinant(subMatrix, n - 1);
        }

        return det;
    }
}
