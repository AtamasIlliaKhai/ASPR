using System;

namespace ASPR_Practice1
{
    internal class GomorySolver
    {
        private const double Epsilon = 0.0000001;

        public GomoryResult SolveInteger(LinearProgrammingProblem problem, ReportBuilder report)
        {
            SimplexSolver simplexSolver = new SimplexSolver();

            report.AddTitle("Розв'язання задачі цілочислового лінійного програмування методом Гоморі");
            report.AddText("Постановка задачі:");
            report.AddText(problem.ToString());
            report.AddText("");
            report.AddText("Додаткова умова для роботи 1D:");
            report.AddText("xj повинні бути цілими невід'ємними числами.");
            report.AddText("");

            SimplexFullResult current = simplexSolver.FindOptimalSolutionWithTable(problem, report);

            int cutCount = 0;
            int maxCuts = 20;

            while (cutCount < maxCuts)
            {
                report.AddTitle("Перевірка цілочисловості розв'язку");

                if (IsIntegerVector(current.Result.X))
                {
                    report.AddText("Усі компоненти вектора X є цілими.");
                    report.AddText("Метод Гоморі завершено.");
                    return new GomoryResult(current.Result, current.FinalTable, cutCount, true);
                }

                int sourceRow = FindRowForGomoryCut(current.FinalTable, problem.VariableCount);

                if (sourceRow == -1)
                {
                    report.AddText("Не знайдено рядок із дробовим вільним членом серед базисних x-змінних.");
                    return new GomoryResult(current.Result, current.FinalTable, cutCount, false);
                }

                cutCount++;

                report.AddTitle("Побудова відсікання Гоморі " + cutCount);
                report.AddText("Для побудови відсікання обрано рядок " + sourceRow + " (" + current.FinalTable.RowNames[sourceRow] + ").");
                report.AddText("Вільний член цього рядка дорівнює " + Math.Round(current.FinalTable[sourceRow, current.FinalTable.Columns - 1], 6) + ".");
                report.AddText("Дробова частина вільного члена дорівнює " + Math.Round(FractionalPart(current.FinalTable[sourceRow, current.FinalTable.Columns - 1]), 6) + ".");

                double[] cutRow = BuildGomoryCutRow(current.FinalTable, sourceRow, report);

                string rowName = "g" + cutCount;

                current.FinalTable.AddRowBeforeLast(rowName, cutRow);

                report.AddText("До симплекс-таблиці додано новий рядок-відсікання " + rowName + ".");
                report.AddText("Симплекс-таблиця після додавання відсікання:");
                report.AddText(current.FinalTable.ToString());

                current = simplexSolver.ContinueFromTableToOptimal(current.FinalTable, problem, report);
            }

            report.AddText("Перевищено максимальну кількість відсікань Гоморі.");
            return new GomoryResult(current.Result, current.FinalTable, cutCount, false);
        }

        private bool IsIntegerVector(double[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (!IsInteger(values[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsInteger(double value)
        {
            return Math.Abs(value - Math.Round(value)) <= Epsilon;
        }

        private int FindRowForGomoryCut(SimplexTable table, int variableCount)
        {
            int selectedRow = -1;
            double maxFraction = 0;

            for (int i = 0; i < table.Rows - 1; i++)
            {
                string rowName = table.RowNames[i];

                if (!rowName.StartsWith("x"))
                {
                    continue;
                }

                int variableIndex;

                if (!int.TryParse(rowName.Substring(1), out variableIndex))
                {
                    continue;
                }

                if (variableIndex < 1 || variableIndex > variableCount)
                {
                    continue;
                }

                double freeMember = table[i, table.Columns - 1];
                double fraction = FractionalPart(freeMember);

                if (fraction > Epsilon && fraction > maxFraction)
                {
                    maxFraction = fraction;
                    selectedRow = i;
                }
            }

            return selectedRow;
        }

        private double[] BuildGomoryCutRow(SimplexTable table, int sourceRow, ReportBuilder report)
        {
            double[] cutRow = new double[table.Columns];

            report.AddText("Коефіцієнти нового обмеження утворюються з дробових частин коефіцієнтів вибраного рядка.");
            report.AddText("Для лекційної симплекс-таблиці рядок відсікання записується з від'ємними дробовими частинами.");

            for (int j = 0; j < table.Columns - 1; j++)
            {
                double coefficient = table[sourceRow, j];
                double fraction = FractionalPart(coefficient);
                cutRow[j] = -fraction;

                report.AddText("Стовпець " + table.ColumnNames[j] + ": коефіцієнт " + Math.Round(coefficient, 6) +
                               ", дробова частина " + Math.Round(fraction, 6) +
                               ", у новий рядок записано " + Math.Round(cutRow[j], 6) + ".");
            }

            double freeMember = table[sourceRow, table.Columns - 1];
            double freeFraction = FractionalPart(freeMember);
            cutRow[table.Columns - 1] = -freeFraction;

            report.AddText("Вільний член: " + Math.Round(freeMember, 6) +
                           ", дробова частина " + Math.Round(freeFraction, 6) +
                           ", у новий рядок записано " + Math.Round(cutRow[table.Columns - 1], 6) + ".");

            return cutRow;
        }

        private double FractionalPart(double value)
        {
            double result = value - Math.Floor(value);

            if (Math.Abs(result) <= Epsilon || Math.Abs(result - 1) <= Epsilon)
            {
                return 0;
            }

            return result;
        }
    }
}