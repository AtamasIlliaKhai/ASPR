using System;
using System.Globalization;

namespace ASPR_Practice1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Практична робота 2");
                Console.WriteLine("Симплекс-метод. Модифіковані жорданові виключення");
                Console.WriteLine();
                Console.WriteLine("1 - Розв'язати задачу вручну");
                Console.WriteLine("2 - Виконати тестовий приклад 1");
                Console.WriteLine("3 - Виконати тестовий приклад 2");
                Console.WriteLine("4 - Виконати власний варіант 3");
                Console.WriteLine("0 - Вихід");
                Console.WriteLine();
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine();

                Console.WriteLine();

                try
                {
                    if (choice == "0")
                    {
                        return;
                    }
                    else if (choice == "1")
                    {
                        RunManualProblem();
                    }
                    else if (choice == "2")
                    {
                        RunProblem(LinearProgrammingProblem.CreateTestProblem1());
                    }
                    else if (choice == "3")
                    {
                        RunProblem(LinearProgrammingProblem.CreateTestProblem2());
                    }
                    else if (choice == "4")
                    {
                        RunProblem(LinearProgrammingProblem.CreateVariant3Practice2());
                    }
                    else
                    {
                        Console.WriteLine("Невідомий пункт меню.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Помилка: " + ex.Message);
                }

                Console.WriteLine();
                Console.WriteLine("Натисніть Enter для продовження...");
                Console.ReadLine();
            }
        }

        private static void RunProblem(LinearProgrammingProblem problem)
        {
            Console.WriteLine("Постановка задачі:");
            Console.WriteLine(problem);

            ReportBuilder report = new ReportBuilder();

            ModifiedJordanSimplexSolver solver = new ModifiedJordanSimplexSolver();

            SimplexResult result = solver.Solve(problem, report);

            Console.WriteLine("Результат:");
            Console.WriteLine(result);

            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunManualProblem()
        {
            LinearProgrammingProblem problem = ReadProblemFromConsole();

            RunProblem(problem);
        }

        private static LinearProgrammingProblem ReadProblemFromConsole()
        {
            Console.Write("Кількість змінних: ");
            int variableCount = ReadInt();

            Console.Write("Кількість обмежень: ");
            int constraintCount = ReadInt();

            double[,] coefficients = new double[constraintCount, variableCount];
            InequalitySign[] signs = new InequalitySign[constraintCount];
            double[] rightSides = new double[constraintCount];

            for (int i = 0; i < constraintCount; i++)
            {
                Console.WriteLine();
                Console.WriteLine("Обмеження " + (i + 1));

                Console.WriteLine("Введіть " + variableCount + " коефіцієнтів через пробіл, кому або крапку з комою:");
                double[] row = ReadDoubleArray(variableCount);

                for (int j = 0; j < variableCount; j++)
                {
                    coefficients[i, j] = row[j];
                }

                Console.WriteLine("Оберіть знак:");
                Console.WriteLine("1 - <=");
                Console.WriteLine("2 - >=");
                Console.WriteLine("3 - =");
                Console.Write("Ваш вибір: ");

                int signChoice = ReadInt();

                if (signChoice == 1)
                {
                    signs[i] = InequalitySign.LessOrEqual;
                }
                else if (signChoice == 2)
                {
                    signs[i] = InequalitySign.GreaterOrEqual;
                }
                else if (signChoice == 3)
                {
                    signs[i] = InequalitySign.Equal;
                }
                else
                {
                    throw new Exception("Неправильний знак обмеження.");
                }

                Console.Write("Права частина: ");
                rightSides[i] = ReadDouble();
            }

            Console.WriteLine();
            Console.WriteLine("Введіть коефіцієнти функції мети:");
            double[] objective = ReadDoubleArray(variableCount);

            Console.WriteLine("Оберіть тип задачі:");
            Console.WriteLine("1 - max");
            Console.WriteLine("2 - min");
            Console.Write("Ваш вибір: ");

            int type = ReadInt();

            OptimizationType optimizationType;

            if (type == 1)
            {
                optimizationType = OptimizationType.Maximize;
            }
            else if (type == 2)
            {
                optimizationType = OptimizationType.Minimize;
            }
            else
            {
                throw new Exception("Неправильний тип задачі.");
            }

            return new LinearProgrammingProblem(
                coefficients,
                signs,
                rightSides,
                objective,
                optimizationType,
                "Задача, введена користувачем");
        }

        private static int ReadInt()
        {
            while (true)
            {
                string line = Console.ReadLine();

                int value;

                if (int.TryParse(line, out value))
                {
                    return value;
                }

                Console.Write("Введіть ціле число: ");
            }
        }

        private static double ReadDouble()
        {
            while (true)
            {
                string line = Console.ReadLine();

                if (line != null)
                {
                    line = line.Replace(',', '.');
                }

                double value;

                if (double.TryParse(line, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }

                Console.Write("Введіть число: ");
            }
        }

        private static double[] ReadDoubleArray(int count)
        {
            while (true)
            {
                string line = Console.ReadLine();

                if (line == null)
                {
                    line = "";
                }

                line = line.Replace(',', '.');

                string[] parts = line.Split(
                    new char[] { ' ', ';', '\t' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != count)
                {
                    Console.WriteLine("Потрібно ввести рівно " + count + " чисел.");
                    continue;
                }

                double[] result = new double[count];
                bool ok = true;

                for (int i = 0; i < count; i++)
                {
                    double value;

                    if (!double.TryParse(parts[i], NumberStyles.Float, CultureInfo.InvariantCulture, out value))
                    {
                        ok = false;
                        break;
                    }

                    result[i] = value;
                }

                if (ok)
                {
                    return result;
                }

                Console.WriteLine("Некоректне введення. Повторіть рядок.");
            }
        }
    }
}