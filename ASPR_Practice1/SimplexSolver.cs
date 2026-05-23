using System;

namespace ASPR_Practice1
{
    internal class SimplexSolver
    {
        private const double Epsilon = 0.0000001;

        public SimplexTable CrossOutZeroRowsOnly(LinearProgrammingProblem problem, ReportBuilder report)
        {
            SimplexTable table = BuildInitialTable(problem);

            report.AddTitle("Видалення нуль-рядків у симплекс-таблиці");
            report.AddText("Постановка задачі:");
            report.AddText(problem.ToString());
            report.AddText("");
            report.AddText("Вхідна симплекс-таблиця:");
            report.AddText(table.ToString());

            table = CrossOutZeroRows(table, report);

            report.AddText("Вихідна симплекс-таблиця після видалення нуль-рядків:");
            report.AddText(table.ToString());

            return table;
        }

        public SimplexResult FindReferenceSolution(LinearProgrammingProblem problem, ReportBuilder report)
        {
            SimplexTable table = BuildInitialTable(problem);

            report.AddTitle("Пошук опорного розв'язку");
            report.AddText("Постановка задачі:");
            report.AddText(problem.ToString());
            report.AddText("");
            report.AddText("Вхідна симплекс-таблиця:");
            report.AddText(table.ToString());

            table = CrossOutZeroRows(table, report);
            report.AddText("Симплекс-таблиця після видалення нуль-рядків:");
            report.AddText(table.ToString());

            table = MakeReferenceSolution(table, report);

            SimplexResult result = BuildResult(problem, table, false);

            report.AddText("Знайдено опорний розв'язок:");
            report.AddText(result.ToString());

            return result;
        }

        public SimplexResult FindOptimalSolution(LinearProgrammingProblem problem, ReportBuilder report)
        {
            SimplexTable table = BuildInitialTable(problem);

            report.AddTitle("Пошук оптимального розв'язку");
            report.AddText("Постановка задачі:");
            report.AddText(problem.ToString());
            report.AddText("");
            report.AddText("Вхідна симплекс-таблиця:");
            report.AddText(table.ToString());

            table = CrossOutZeroRows(table, report);
            report.AddText("Симплекс-таблиця після видалення нуль-рядків:");
            report.AddText(table.ToString());

            table = MakeReferenceSolution(table, report);
            table = MakeOptimalSolution(table, report);

            SimplexResult result = BuildResult(problem, table, true);

            report.AddText("Знайдено оптимальний розв'язок:");
            report.AddText(result.ToString());

            return result;
        }

        private SimplexTable BuildInitialTable(LinearProgrammingProblem problem)
        {
            double[,] rowsData = problem.GetSimplexRows();
            string[] rowNames = problem.GetInitialRowNames();

            int constraintCount = rowsData.GetLength(0);
            int variableCount = problem.VariableCount;

            int rows = constraintCount + 1;
            int columns = variableCount + 1;

            SimplexTable table = new SimplexTable(rows, columns);

            for (int i = 0; i < constraintCount; i++)
            {
                table.RowNames[i] = rowNames[i];
            }

            table.RowNames[rows - 1] = "Z";

            for (int j = 0; j < variableCount; j++)
            {
                table.ColumnNames[j] = "x" + (j + 1);
            }

            table.ColumnNames[columns - 1] = "1";

            for (int i = 0; i < constraintCount; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    table[i, j] = rowsData[i, j];
                }
            }

            for (int j = 0; j < variableCount; j++)
            {
                if (problem.GoalType == GoalType.Maximize)
                {
                    table[rows - 1, j] = -problem.GoalCoefficients[j];
                }
                else
                {
                    table[rows - 1, j] = problem.GoalCoefficients[j];
                }
            }

            table[rows - 1, columns - 1] = 0;

            return table;
        }

        private SimplexTable CrossOutZeroRows(SimplexTable table, ReportBuilder report)
        {
            table = table.Copy();

            int step = 1;
            int maxSteps = 100;

            report.AddTitle("Етап 0. Видалення нуль-рядків");

            while (true)
            {
                if (step > maxSteps)
                {
                    throw new Exception("Перевищено допустиму кількість кроків під час видалення нуль-рядків.");
                }

                int zeroRow = FindFirstZeroRow(table);

                if (zeroRow == -1)
                {
                    report.AddText("Нуль-рядків у симплекс-таблиці немає.");
                    break;
                }

                int pivotColumn = FindFirstPositiveInRow(table, zeroRow);

                if (pivotColumn == -1)
                {
                    throw new Exception("Система обмежень є суперечливою: у нуль-рядку немає додатних коефіцієнтів.");
                }

                int pivotRow = FindPivotRowByLectureRule(table, pivotColumn);

                if (pivotRow == -1)
                {
                    break;
                }

                report.AddStep(
                    step,
                    "Розв'язувальний рядок: " + pivotRow + " (" + table.RowNames[pivotRow] + ")\r\n" +
                    "Розв'язувальний стовпець: " + pivotColumn + " (" + table.ColumnNames[pivotColumn] + ")\r\n" +
                    "Розв'язувальний елемент: " + Math.Round(table[pivotRow, pivotColumn], 4)
                );

                table = table.ModifiedJordanElimination(pivotRow, pivotColumn);

                if (table.ColumnNames[pivotColumn].StartsWith("0"))
                {
                    report.AddText("Після МЖВ у заголовку стовпця отримано нуль-маркер. Виконується видалення стовпця " + pivotColumn + ".");
                    table.RemoveColumn(pivotColumn);
                }

                report.AddText("Таблиця після виконання МЖВ:");
                report.AddText(table.ToString());

                step++;
            }

            return table;
        }

        private SimplexTable MakeReferenceSolution(SimplexTable table, ReportBuilder report)
        {
            int step = 1;
            int maxSteps = 100;

            report.AddTitle("Етап 1. Пошук опорного розв'язку");

            while (true)
            {
                if (step > maxSteps)
                {
                    throw new Exception("Перевищено допустиму кількість кроків під час пошуку опорного розв'язку.");
                }

                int negativeFreeMemberRow = FindFirstNegativeFreeMemberRow(table);

                if (negativeFreeMemberRow == -1)
                {
                    report.AddText("Опорний розв'язок знайдено.");
                    break;
                }

                int pivotColumn = FindFirstNegativeInRow(table, negativeFreeMemberRow);

                if (pivotColumn == -1)
                {
                    throw new Exception("Система обмежень є суперечливою.");
                }

                int pivotRow = FindPivotRowByLectureRule(table, pivotColumn);

                if (pivotRow == -1)
                {
                    throw new Exception("Система обмежень є суперечливою.");
                }

                report.AddStep(
                    step,
                    "Розв'язувальний рядок: " + pivotRow + " (" + table.RowNames[pivotRow] + ")\r\n" +
                    "Розв'язувальний стовпець: " + pivotColumn + " (" + table.ColumnNames[pivotColumn] + ")\r\n" +
                    "Розв'язувальний елемент: " + Math.Round(table[pivotRow, pivotColumn], 4)
                );

                table = table.ModifiedJordanElimination(pivotRow, pivotColumn);

                report.AddText("Таблиця після виконання МЖВ:");
                report.AddText(table.ToString());

                step++;
            }

            return table;
        }

        private SimplexTable MakeOptimalSolution(SimplexTable table, ReportBuilder report)
        {
            int step = 1;
            int maxSteps = 100;

            report.AddTitle("Етап 2. Пошук оптимального розв'язку");

            while (true)
            {
                if (step > maxSteps)
                {
                    throw new Exception("Перевищено допустиму кількість кроків під час пошуку оптимального розв'язку.");
                }

                int pivotColumn = FindFirstNegativeInZRow(table);

                if (pivotColumn == -1)
                {
                    report.AddText("Оптимальний розв'язок вже знайдено.");
                    break;
                }

                int pivotRow = FindPivotRowByLectureRule(table, pivotColumn);

                if (pivotRow == -1)
                {
                    throw new Exception("Функція не обмежена зверху.");
                }

                report.AddStep(
                    step,
                    "Розв'язувальний рядок: " + pivotRow + " (" + table.RowNames[pivotRow] + ")\r\n" +
                    "Розв'язувальний стовпець: " + pivotColumn + " (" + table.ColumnNames[pivotColumn] + ")\r\n" +
                    "Розв'язувальний елемент: " + Math.Round(table[pivotRow, pivotColumn], 4)
                );

                table = table.ModifiedJordanElimination(pivotRow, pivotColumn);

                report.AddText("Таблиця після виконання МЖВ:");
                report.AddText(table.ToString());

                step++;
            }

            return table;
        }

        private int FindFirstZeroRow(SimplexTable table)
        {
            for (int i = 0; i < table.Rows - 1; i++)
            {
                if (table.RowNames[i].StartsWith("0"))
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindFirstPositiveInRow(SimplexTable table, int row)
        {
            for (int j = 0; j < table.Columns - 1; j++)
            {
                if (table[row, j] > Epsilon)
                {
                    return j;
                }
            }

            return -1;
        }

        private int FindFirstNegativeFreeMemberRow(SimplexTable table)
        {
            int constantColumn = table.Columns - 1;

            for (int i = 0; i < table.Rows - 1; i++)
            {
                if (table[i, constantColumn] < -Epsilon)
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindFirstNegativeInRow(SimplexTable table, int row)
        {
            for (int j = 0; j < table.Columns - 1; j++)
            {
                if (table[row, j] < -Epsilon)
                {
                    return j;
                }
            }

            return -1;
        }

        private int FindFirstNegativeInZRow(SimplexTable table)
        {
            int zRow = table.Rows - 1;

            for (int j = 0; j < table.Columns - 1; j++)
            {
                if (table[zRow, j] < -Epsilon)
                {
                    return j;
                }
            }

            return -1;
        }

        private int FindPivotRowByLectureRule(SimplexTable table, int pivotColumn)
        {
            int constantColumn = table.Columns - 1;

            int pivotRow = -1;
            double minimalNonNegativeRatio = double.MaxValue;

            for (int i = 0; i < table.Rows - 1; i++)
            {
                double columnValue = table[i, pivotColumn];
                double freeValue = table[i, constantColumn];

                if (Math.Abs(columnValue) <= Epsilon)
                {
                    continue;
                }

                double ratio = freeValue / columnValue;

                int signum1 = columnValue < 0 ? -1 : 1;
                int signum2;

                if (Math.Abs(freeValue) <= Epsilon)
                {
                    signum2 = 1;
                }
                else
                {
                    signum2 = freeValue < 0 ? -1 : 1;
                }

                if (ratio < -Epsilon || signum1 * signum2 < 0)
                {
                    continue;
                }

                if (ratio < minimalNonNegativeRatio)
                {
                    minimalNonNegativeRatio = ratio;
                    pivotRow = i;
                }
            }

            return pivotRow;
        }

        private SimplexResult BuildResult(LinearProgrammingProblem problem, SimplexTable table, bool isOptimal)
        {
            double[] x = new double[problem.VariableCount];

            for (int i = 0; i < table.Rows - 1; i++)
            {
                string rowName = table.RowNames[i];

                if (rowName.StartsWith("x"))
                {
                    int index = int.Parse(rowName.Substring(1)) - 1;

                    if (index >= 0 && index < x.Length)
                    {
                        x[index] = table[i, table.Columns - 1];
                    }
                }
            }

            double z = 0;

            for (int i = 0; i < problem.VariableCount; i++)
            {
                z += problem.GoalCoefficients[i] * x[i];
            }

            return new SimplexResult(x, z, isOptimal, false, false);
        }
    }
}