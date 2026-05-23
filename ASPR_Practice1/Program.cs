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
                Console.WriteLine("Застосування звичайних і модифікованих жорданових виключень");
                Console.WriteLine();
                Console.WriteLine("1 - Пошук оберненої матриці");
                Console.WriteLine("2 - Обчислення рангу матриці");
                Console.WriteLine("5 - Розв'язання СЛАР методом Гауса");
                Console.WriteLine("6 - Пошук опорного розв'язку задачі ЛП");
                Console.WriteLine("7 - Пошук оптимального розв'язку задачі ЛП");
                Console.WriteLine("8 - Виконати власний варіант 3 для задачі ЛП 1B");
                Console.WriteLine("9 - Виконати власний варіант 3 для задачі СЛАР");
                Console.WriteLine("10 - Видалення нуль-рядків у симплекс-таблиці");
                Console.WriteLine("11 - Пошук опорного розв'язку задачі ЛП зі змішаною системою");
                Console.WriteLine("12 - Пошук оптимального розв'язку задачі ЛП зі змішаною системою");
                Console.WriteLine("13 - Виконати власний варіант 3 для роботи 1C");
                Console.WriteLine("14 - Виконати тестовий приклад для змішаної системи");
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

                        case "6":
                            RunReferenceSolutionTask();
                            break;

                        case "7":
                            RunOptimalSolutionTask();
                            break;

                        case "8":
                            RunLinearProgrammingVariant3();
                            break;

                        case "9":
                            RunVariant3();
                            break;

                        case "10":
                            RunCrossOutZeroRowsTask();
                            break;

                        case "11":
                            RunMixedReferenceSolutionTask();
                            break;

                        case "12":
                            RunMixedOptimalSolutionTask();
                            break;

                        case "13":
                            RunLinearProgrammingVariant3Mixed();
                            break;

                        case "14":
                            RunMixedSample();
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

        private static void RunReferenceSolutionTask()
        {
            Console.WriteLine("Пошук опорного розв'язку задачі лінійного програмування");
            LinearProgrammingProblem problem = ReadLinearProgrammingProblem();

            ReportBuilder report = new ReportBuilder();
            SimplexSolver solver = new SimplexSolver();

            SimplexResult result = solver.FindReferenceSolution(problem, report);

            Console.WriteLine();
            Console.WriteLine("Опорний розв'язок:");
            Console.WriteLine(result);

            Console.WriteLine();
            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunOptimalSolutionTask()
        {
            Console.WriteLine("Пошук оптимального розв'язку задачі лінійного програмування");
            LinearProgrammingProblem problem = ReadLinearProgrammingProblem();

            ReportBuilder report = new ReportBuilder();
            SimplexSolver solver = new SimplexSolver();

            SimplexResult result = solver.FindOptimalSolution(problem, report);

            Console.WriteLine();
            Console.WriteLine("Оптимальний розв'язок:");
            Console.WriteLine(result);

            Console.WriteLine();
            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunLinearProgrammingVariant3()
        {
            Console.WriteLine("Власний варіант 3. Практична робота 1B.");
            Console.WriteLine("У другій нерівності знак змінено на протилежний: <= замінено на >=.");
            Console.WriteLine();

            LinearProgrammingProblem problem = LinearProgrammingProblem.CreateVariant3();

            Console.WriteLine("Постановка задачі:");
            Console.WriteLine(problem);
            Console.WriteLine();

            ReportBuilder referenceReport = new ReportBuilder();
            SimplexSolver solver = new SimplexSolver();

            SimplexResult referenceResult = solver.FindReferenceSolution(problem, referenceReport);

            Console.WriteLine("Опорний розв'язок:");
            Console.WriteLine(referenceResult);
            Console.WriteLine("Протокол пошуку опорного розв'язку:");
            Console.WriteLine(referenceReport.GetReport());

            ReportBuilder optimalReport = new ReportBuilder();
            SimplexResult optimalResult = solver.FindOptimalSolution(problem, optimalReport);

            Console.WriteLine("Оптимальний розв'язок:");
            Console.WriteLine(optimalResult);
            Console.WriteLine("Протокол пошуку оптимального розв'язку:");
            Console.WriteLine(optimalReport.GetReport());
        }

        private static void RunCrossOutZeroRowsTask()
        {
            Console.WriteLine("Видалення нуль-рядків у симплекс-таблиці");
            LinearProgrammingProblem problem = ReadLinearProgrammingProblem();

            ReportBuilder report = new ReportBuilder();
            SimplexSolver solver = new SimplexSolver();

            SimplexTable table = solver.CrossOutZeroRowsOnly(problem, report);

            Console.WriteLine();
            Console.WriteLine("Таблиця після видалення нуль-рядків:");
            Console.WriteLine(table);

            Console.WriteLine();
            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunMixedReferenceSolutionTask()
        {
            Console.WriteLine("Пошук опорного розв'язку задачі ЛП зі змішаною системою обмежень");
            LinearProgrammingProblem problem = ReadLinearProgrammingProblem();

            ReportBuilder report = new ReportBuilder();
            SimplexSolver solver = new SimplexSolver();

            SimplexResult result = solver.FindReferenceSolution(problem, report);

            Console.WriteLine();
            Console.WriteLine("Опорний розв'язок:");
            Console.WriteLine(result);

            Console.WriteLine();
            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunMixedOptimalSolutionTask()
        {
            Console.WriteLine("Пошук оптимального розв'язку задачі ЛП зі змішаною системою обмежень");
            LinearProgrammingProblem problem = ReadLinearProgrammingProblem();

            ReportBuilder report = new ReportBuilder();
            SimplexSolver solver = new SimplexSolver();

            SimplexResult result = solver.FindOptimalSolution(problem, report);

            Console.WriteLine();
            Console.WriteLine("Оптимальний розв'язок:");
            Console.WriteLine(result);

            Console.WriteLine();
            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunLinearProgrammingVariant3Mixed()
        {
            Console.WriteLine("Власний варіант 3. Практична робота 1C.");
            Console.WriteLine("У другій нерівності знак змінено на протилежний: <= замінено на >=.");
            Console.WriteLine();

            LinearProgrammingProblem problem = LinearProgrammingProblem.CreateVariant3Mixed();

            Console.WriteLine("Постановка задачі:");
            Console.WriteLine(problem);
            Console.WriteLine();

            SimplexSolver solver = new SimplexSolver();

            ReportBuilder zeroReport = new ReportBuilder();
            SimplexTable table = solver.CrossOutZeroRowsOnly(problem, zeroReport);

            Console.WriteLine("Видалення нуль-рядків:");
            Console.WriteLine(table);
            Console.WriteLine("Протокол видалення нуль-рядків:");
            Console.WriteLine(zeroReport.GetReport());

            ReportBuilder referenceReport = new ReportBuilder();
            SimplexResult referenceResult = solver.FindReferenceSolution(problem, referenceReport);

            Console.WriteLine("Опорний розв'язок:");
            Console.WriteLine(referenceResult);
            Console.WriteLine("Протокол пошуку опорного розв'язку:");
            Console.WriteLine(referenceReport.GetReport());

            ReportBuilder optimalReport = new ReportBuilder();
            SimplexResult optimalResult = solver.FindOptimalSolution(problem, optimalReport);

            Console.WriteLine("Оптимальний розв'язок:");
            Console.WriteLine(optimalResult);
            Console.WriteLine("Протокол пошуку оптимального розв'язку:");
            Console.WriteLine(optimalReport.GetReport());
        }

        private static void RunMixedSample()
        {
            Console.WriteLine("Тестовий приклад для змішаної системи обмежень.");

            LinearProgrammingProblem problem = LinearProgrammingProblem.CreateMixedSampleFromLecture();

            Console.WriteLine("Постановка задачі:");
            Console.WriteLine(problem);
            Console.WriteLine();

            ReportBuilder report = new ReportBuilder();
            SimplexSolver solver = new SimplexSolver();

            SimplexResult result = solver.FindOptimalSolution(problem, report);

            Console.WriteLine("Оптимальний розв'язок:");
            Console.WriteLine(result);

            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static LinearProgrammingProblem ReadLinearProgrammingProblem()
        {
            Console.Write("Кількість змінних: ");
            int variableCount = ReadInt();

            Console.Write("Кількість обмежень: ");
            int inequalityCount = ReadInt();

            Inequality[] inequalities = new Inequality[inequalityCount];

            for (int i = 0; i < inequalityCount; i++)
            {
                Console.WriteLine();
                Console.WriteLine("Обмеження " + (i + 1));

                double[] coefficients = ReadDoubleArray(variableCount);

                Console.WriteLine("Оберіть знак обмеження:");
                Console.WriteLine("1 - <=");
                Console.WriteLine("2 - >=");
                Console.WriteLine("3 - =");
                Console.Write("Ваш вибір: ");

                int signNumber = ReadInt();

                InequalitySign sign;

                if (signNumber == 1)
                {
                    sign = InequalitySign.LessOrEqual;
                }
                else if (signNumber == 2)
                {
                    sign = InequalitySign.GreaterOrEqual;
                }
                else
                {
                    sign = InequalitySign.Equals;
                }

                Console.Write("Права частина обмеження: ");
                double rightPart = ReadDouble();

                inequalities[i] = new Inequality(coefficients, sign, rightPart);
            }

            Console.WriteLine();
            Console.WriteLine("Введіть коефіцієнти функції мети:");
            double[] goal = ReadDoubleArray(variableCount);

            Console.WriteLine("Оберіть тип задачі:");
            Console.WriteLine("1 - max");
            Console.WriteLine("2 - min");
            Console.Write("Ваш вибір: ");

            int goalTypeNumber = ReadInt();

            GoalType goalType;

            if (goalTypeNumber == 2)
            {
                goalType = GoalType.Minimize;
            }
            else
            {
                goalType = GoalType.Maximize;
            }

            return new LinearProgrammingProblem(goal, goalType, inequalities);
        }

        private static double[] ReadDoubleArray(int count)
        {
            while (true)
            {
                Console.WriteLine("Введіть " + count + " чисел через пробіл, кому або крапку з комою:");
                string line = Console.ReadLine();

                string[] parts = line.Split(
                    new char[] { ' ', ',', ';' },
                    StringSplitOptions.RemoveEmptyEntries
                );

                if (parts.Length != count)
                {
                    Console.WriteLine("Помилка: потрібно ввести " + count + " чисел.");
                    continue;
                }

                double[] result = new double[count];
                bool success = true;

                for (int i = 0; i < count; i++)
                {
                    double value;

                    if (!double.TryParse(parts[i].Replace('.', ','), out value))
                    {
                        success = false;
                        break;
                    }

                    result[i] = value;
                }

                if (success)
                {
                    return result;
                }

                Console.WriteLine("Помилка формату числа.");
            }
        }

        private static double ReadDouble()
        {
            double value;

            while (!double.TryParse(Console.ReadLine().Replace('.', ','), out value))
            {
                Console.Write("Введіть число: ");
            }

            return value;
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