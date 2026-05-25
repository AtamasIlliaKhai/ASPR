using ASPR_Practice1;
using System;
using System.Globalization;
using System.Text;

namespace ASPR_Practice1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Практична робота №2");
                Console.WriteLine("Двоїста задача лінійного програмування");
                Console.WriteLine("Симплекс-метод, модифіковані жорданові виключення, графічний метод");
                Console.WriteLine();
                Console.WriteLine("1 - Розв'язати задачу лінійного програмування вручну");
                Console.WriteLine("2 - Розв'язати пряму задачу Z з практичної 1B, варіант 3");
                Console.WriteLine("3 - Побудувати і розв'язати двоїсту задачу W для варіанту 3");
                Console.WriteLine("4 - Розв'язати пару взаємно двоїстих задач Z і W");
                Console.WriteLine("5 - Розв'язати W безпосередньо як задачу практичної 1B");
                Console.WriteLine("6 - Графічно розв'язати пряму задачу варіанту 3");
                Console.WriteLine("7 - Графічно розв'язати двоїсту задачу варіанту 3");
                Console.WriteLine("8 - Графічно розв'язати пару взаємно двоїстих задач");
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
                        RunSimplexProblem(LinearProgrammingProblem.CreatePractice1BVariant3Primal());
                    }
                    else if (choice == "3")
                    {
                        RunDualProblem();
                    }
                    else if (choice == "4")
                    {
                        RunDualPair();
                    }
                    else if (choice == "5")
                    {
                        RunDirectDualAsSimplexProblem();
                    }
                    else if (choice == "6")
                    {
                        RunGraphicalPrimal();
                    }
                    else if (choice == "7")
                    {
                        RunGraphicalDual();
                    }
                    else if (choice == "8")
                    {
                        RunGraphicalPair();
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

        private static void RunManualProblem()
        {
            LinearProgrammingProblem problem = ReadProblemFromConsole();
            RunSimplexProblem(problem);
        }

        private static void RunSimplexProblem(LinearProgrammingProblem problem)
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

        private static void RunDualProblem()
        {
            LinearProgrammingProblem primal = LinearProgrammingProblem.CreatePractice1BVariant3Primal();
            LinearProgrammingProblem dual = DualProblemBuilder.BuildDualForMaxLessOrEqual(primal);

            Console.WriteLine("Пряма задача Z:");
            Console.WriteLine(primal);

            Console.WriteLine("Побудована двоїста задача W:");
            Console.WriteLine(dual);

            ReportBuilder report = new ReportBuilder();
            ModifiedJordanSimplexSolver solver = new ModifiedJordanSimplexSolver();

            SimplexResult result = solver.Solve(dual, report);

            Console.WriteLine("Результат розв'язання двоїстої задачі W:");
            Console.WriteLine(result);

            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunDualPair()
        {
            LinearProgrammingProblem primal = LinearProgrammingProblem.CreatePractice1BVariant3Primal();

            ReportBuilder report = new ReportBuilder();
            DualPairSolver solver = new DualPairSolver();

            DualPairResult result = solver.SolvePair(primal, report);

            Console.WriteLine(result);

            Console.WriteLine("Протокол розрахунку:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunDirectDualAsSimplexProblem()
        {
            LinearProgrammingProblem primal = LinearProgrammingProblem.CreatePractice1BVariant3Primal();
            LinearProgrammingProblem dual = DualProblemBuilder.BuildDualForMaxLessOrEqual(primal);

            Console.WriteLine("Двоїста задача W розв'язується безпосередньо як окрема задача ЛП.");
            Console.WriteLine();

            RunSimplexProblem(dual);
        }

        private static void RunGraphicalPrimal()
        {
            GraphicalProblem problem = GraphicalProblem.CreatePractice2Variant3Primal();

            ReportBuilder report = new ReportBuilder();
            GraphicalSolver solver = new GraphicalSolver();

            GraphicalResult result = solver.Solve(problem, report);

            Console.WriteLine(result);

            Console.WriteLine("Протокол графічного розв'язання:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunGraphicalDual()
        {
            GraphicalProblem primal = GraphicalProblem.CreatePractice2Variant3Primal();
            GraphicalProblem dual = GraphicalProblem.CreatePractice2Variant3Dual();

            Console.WriteLine("Пряма задача для графічного методу:");
            Console.WriteLine(primal);

            Console.WriteLine("Двоїста задача для графічного методу:");
            Console.WriteLine(dual);

            ReportBuilder report = new ReportBuilder();
            GraphicalSolver solver = new GraphicalSolver();

            GraphicalResult result = solver.Solve(dual, report);

            Console.WriteLine(result);

            Console.WriteLine("Протокол графічного розв'язання:");
            Console.WriteLine(report.GetReport());
        }

        private static void RunGraphicalPair()
        {
            GraphicalProblem primal = GraphicalProblem.CreatePractice2Variant3Primal();
            GraphicalProblem dual = GraphicalProblem.CreatePractice2Variant3Dual();

            ReportBuilder reportPrimal = new ReportBuilder();
            ReportBuilder reportDual = new ReportBuilder();

            GraphicalSolver solver = new GraphicalSolver();

            GraphicalResult primalResult = solver.Solve(primal, reportPrimal);
            GraphicalResult dualResult = solver.Solve(dual, reportDual);

            Console.WriteLine("Графічне розв'язання прямої задачі Z:");
            Console.WriteLine(primalResult);

            Console.WriteLine("Графічне розв'язання двоїстої задачі W:");
            Console.WriteLine(dualResult);

            Console.WriteLine("Порівняння:");
            if (primalResult.IsUnbounded && dualResult.IsInfeasible)
            {
                Console.WriteLine("Пряма задача є необмеженою, а двоїста задача є несумісною. Це відповідає теоремі двоїстості.");
            }
            else if (primalResult.IsOptimal && dualResult.IsOptimal)
            {
                Console.WriteLine("Z = " + primalResult.ObjectiveValue.ToString("0.####"));
                Console.WriteLine("W = " + dualResult.ObjectiveValue.ToString("0.####"));
                Console.WriteLine("Різниця |Z - W| = " + Math.Abs(primalResult.ObjectiveValue - dualResult.ObjectiveValue).ToString("0.####"));
            }

            Console.WriteLine();
            Console.WriteLine("Протокол прямої задачі:");
            Console.WriteLine(reportPrimal.GetReport());

            Console.WriteLine("Протокол двоїстої задачі:");
            Console.WriteLine(reportDual.GetReport());
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