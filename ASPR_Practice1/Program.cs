using System;
using System.Globalization;
using System.Text;

namespace ASPR_Practice1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("Практична робота №1");
                Console.WriteLine("Застосування звичайних жорданових виключень");
                Console.WriteLine();
                Console.WriteLine("1 - Пошук оберненої матриці");
                Console.WriteLine("2 - Обчислення рангу матриці");
                Console.WriteLine("5 - Розв'язання СЛАР методом Гауса");
                Console.WriteLine("9 - Виконати власний варіант 3");
                Console.WriteLine("0 - Вихід");
                Console.WriteLine();
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            RunInverseMatrixTask();
                            break;

                        case "2":
                            RunRankTask();
                            break;

                        case "5":
                            RunGaussTask();
                            break;

                        case "9":
                            RunVariant3();
                            break;

                        case "0":
                            isRunning = false;
                            break;

                        default:
                            Console.WriteLine("Невірний вибір.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Помилка: " + ex.Message);
                }

                if (isRunning)
                {
                    Console.WriteLine();
                    Console.WriteLine("Натисніть Enter для продовження...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

        private static void RunInverseMatrixTask()
        {
            Console.WriteLine("Завдання 1. Пошук оберненої матриці");
            Matrix matrix = ReadMatrix();

            ReportBuilder report = new ReportBuilder();
            Matrix inverseMatrix = JordanSolver.FindInverseMatrix(matrix, report);

            Console.WriteLine();
            Console.WriteLine("Обернена матриця:");
            Console.WriteLine(inverseMatrix);

            Console.WriteLine();
            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunRankTask()
        {
            Console.WriteLine("Завдання 2. Обчислення рангу матриці");
            Matrix matrix = ReadMatrix();

            ReportBuilder report = new ReportBuilder();
            int rank = JordanSolver.FindRank(matrix, report);

            Console.WriteLine();
            Console.WriteLine("Ранг матриці: " + rank);

            Console.WriteLine();
            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunGaussTask()
        {
            Console.WriteLine("Завдання 5. Розв'язання СЛАР методом Гауса");
            Console.WriteLine("Введіть матрицю коефіцієнтів A:");
            Matrix matrixA = ReadMatrix();

            Console.WriteLine();
            Console.WriteLine("Введіть вектор правих частин B:");
            Matrix matrixB = ReadMatrix(matrixA.Rows, 1);

            ReportBuilder report = new ReportBuilder();
            Matrix result = JordanSolver.SolveByGauss(matrixA, matrixB, report);

            Console.WriteLine();
            Console.WriteLine("Розв'язок системи:");
            Console.WriteLine(result);

            Console.WriteLine();
            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunVariant3()
        {
            Console.WriteLine("Власний варіант 3");
            Console.WriteLine("У варіанті змінено знак елемента a33: -2 -> 2.");
            Console.WriteLine();

            Matrix matrixA = new Matrix(new double[,]
            {
                { 1, 2, 1 },
                { -1, 1, 3 },
                { 2, 5, 2 }
            });

            Matrix matrixB = new Matrix(new double[,]
            {
                { 2 },
                { 5 },
                { 1 }
            });

            Console.WriteLine("Матриця A:");
            Console.WriteLine(matrixA);

            Console.WriteLine("Матриця B:");
            Console.WriteLine(matrixB);

            ReportBuilder inverseReport = new ReportBuilder();
            Matrix inverseMatrix = JordanSolver.FindInverseMatrix(matrixA, inverseReport);

            Console.WriteLine("Завдання 1. Обернена матриця:");
            Console.WriteLine(inverseMatrix);
            Console.WriteLine("Протокол:");
            Console.WriteLine(inverseReport.GetReport());

            ReportBuilder rankReport = new ReportBuilder();
            int rank = JordanSolver.FindRank(matrixA, rankReport);

            Console.WriteLine("Завдання 2. Ранг матриці:");
            Console.WriteLine(rank);
            Console.WriteLine("Протокол:");
            Console.WriteLine(rankReport.GetReport());

            ReportBuilder gaussReport = new ReportBuilder();
            Matrix solution = JordanSolver.SolveByGauss(matrixA, matrixB, gaussReport);

            Console.WriteLine("Завдання 5. Розв'язок СЛАР методом Гауса:");
            Console.WriteLine(solution);
            Console.WriteLine("Протокол:");
            Console.WriteLine(gaussReport.GetReport());
        }

        private static Matrix ReadMatrix()
        {
            Console.Write("Кількість рядків: ");
            int rows = ReadInt();

            Console.Write("Кількість стовпців: ");
            int columns = ReadInt();

            return ReadMatrix(rows, columns);
        }

        private static Matrix ReadMatrix(int rows, int columns)
        {
            double[,] data = new double[rows, columns];

            Console.WriteLine("Вводьте елементи рядка через пробіл, кому або крапку з комою.");

            for (int i = 0; i < rows; i++)
            {
                bool isCorrect = false;

                while (!isCorrect)
                {
                    Console.Write("Рядок " + (i + 1) + ": ");
                    string line = Console.ReadLine();

                    string[] parts = line.Split(
                        new char[] { ' ', ';', ',' },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    if (parts.Length != columns)
                    {
                        Console.WriteLine("Помилка: потрібно ввести " + columns + " елементів.");
                        continue;
                    }

                    isCorrect = true;

                    for (int j = 0; j < columns; j++)
                    {
                        double value;
                        string normalized = parts[j].Replace(',', '.');

                        if (!double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                        {
                            Console.WriteLine("Помилка формату числа.");
                            isCorrect = false;
                            break;
                        }

                        data[i, j] = value;
                    }
                }
            }

            return new Matrix(data);
        }

        private static int ReadInt()
        {
            int value;

            while (!int.TryParse(Console.ReadLine(), out value))
            {
                Console.Write("Введіть ціле число: ");
            }

            return value;
        }
    }
}