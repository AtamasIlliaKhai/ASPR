using System;
using System.Text;

namespace ASPR_Practice1
{
    internal static class TransportSolver
    {
        private const double Epsilon = 0.0000001;

        private class Cell
        {
            public int Row;
            public int Column;

            public Cell(int row, int column)
            {
                Row = row;
                Column = column;
            }
        }

        public static TransportSolution SolveNorthWestCorner(TransportInput originalInput, ReportBuilder report)
        {
            TransportInput input = Balance(originalInput, report);

            report.AddTitle("Метод північно-західного кута");
            report.AddText("Вхідні дані:");
            report.AddText(input.ToString());
            report.AddText("Матриця вартостей:");
            report.AddText(MatrixToString(input.Costs));

            int rows = input.SupplierCount;
            int columns = input.ConsumerCount;

            double[,] plan = new double[rows, columns];
            bool[,] basis = new bool[rows, columns];

            double[] supplies = CopyVector(input.Supplies);
            double[] demands = CopyVector(input.Demands);

            int i = 0;
            int j = 0;

            while (i < rows && j < columns)
            {
                double value = Math.Min(supplies[i], demands[j]);

                plan[i, j] = value;
                basis[i, j] = true;

                report.AddText("У клітинку S" + (i + 1) + "D" + (j + 1) +
                               " записано min(" + supplies[i].ToString("0.####") +
                               "; " + demands[j].ToString("0.####") +
                               ") = " + value.ToString("0.####") + ".");

                supplies[i] -= value;
                demands[j] -= value;

                if (Math.Abs(supplies[i]) <= Epsilon && Math.Abs(demands[j]) <= Epsilon)
                {
                    if (i + 1 < rows && j + 1 < columns)
                    {
                        basis[i, j + 1] = true;
                        report.AddText("Для збереження кількості базисних клітинок додано нульову базисну клітинку S" +
                                       (i + 1) + "D" + (j + 2) + ".");
                    }

                    i++;
                    j++;
                }
                else if (Math.Abs(supplies[i]) <= Epsilon)
                {
                    i++;
                }
                else if (Math.Abs(demands[j]) <= Epsilon)
                {
                    j++;
                }
            }

            EnsureBasisCount(plan, basis, report);

            double cost = CalculateTotalCost(input, plan);

            report.AddText("Опорний план, отриманий методом північно-західного кута:");
            report.AddText(MatrixToString(plan));
            report.AddText("Загальна вартість: " + cost.ToString("0.####"));

            return new TransportSolution(
                "Метод північно-західного кута",
                input,
                plan,
                basis,
                cost,
                null,
                null,
                null,
                false);
        }

        public static TransportSolution SolveMinimumElement(TransportInput originalInput, ReportBuilder report)
        {
            TransportInput input = Balance(originalInput, report);

            report.AddTitle("Метод мінімального елемента");
            report.AddText("Вхідні дані:");
            report.AddText(input.ToString());
            report.AddText("Матриця вартостей:");
            report.AddText(MatrixToString(input.Costs));

            int rows = input.SupplierCount;
            int columns = input.ConsumerCount;

            double[,] plan = new double[rows, columns];
            bool[,] basis = new bool[rows, columns];

            double[] supplies = CopyVector(input.Supplies);
            double[] demands = CopyVector(input.Demands);

            bool[] rowClosed = new bool[rows];
            bool[] columnClosed = new bool[columns];

            while (!AllClosed(rowClosed) || !AllClosed(columnClosed))
            {
                int bestRow = -1;
                int bestColumn = -1;
                double bestCost = double.MaxValue;

                for (int i = 0; i < rows; i++)
                {
                    if (rowClosed[i])
                    {
                        continue;
                    }

                    for (int j = 0; j < columns; j++)
                    {
                        if (columnClosed[j])
                        {
                            continue;
                        }

                        if (input.Costs[i, j] < bestCost)
                        {
                            bestCost = input.Costs[i, j];
                            bestRow = i;
                            bestColumn = j;
                        }
                    }
                }

                if (bestRow == -1 || bestColumn == -1)
                {
                    break;
                }

                double value = Math.Min(supplies[bestRow], demands[bestColumn]);

                plan[bestRow, bestColumn] = value;
                basis[bestRow, bestColumn] = true;

                report.AddText("Обрано клітинку з мінімальною вартістю S" + (bestRow + 1) +
                               "D" + (bestColumn + 1) + ", c = " + bestCost.ToString("0.####") +
                               ". Записано min(" + supplies[bestRow].ToString("0.####") +
                               "; " + demands[bestColumn].ToString("0.####") +
                               ") = " + value.ToString("0.####") + ".");

                supplies[bestRow] -= value;
                demands[bestColumn] -= value;

                if (Math.Abs(supplies[bestRow]) <= Epsilon)
                {
                    rowClosed[bestRow] = true;
                }

                if (Math.Abs(demands[bestColumn]) <= Epsilon)
                {
                    columnClosed[bestColumn] = true;
                }
            }

            EnsureBasisCount(plan, basis, report);

            double cost = CalculateTotalCost(input, plan);

            report.AddText("Опорний план, отриманий методом мінімального елемента:");
            report.AddText(MatrixToString(plan));
            report.AddText("Загальна вартість: " + cost.ToString("0.####"));

            return new TransportSolution(
                "Метод мінімального елемента",
                input,
                plan,
                basis,
                cost,
                null,
                null,
                null,
                false);
        }

        public static TransportSolution SolvePotentials(TransportInput originalInput, ReportBuilder report)
        {
            report.AddTitle("Метод потенціалів");
            report.AddText("Спочатку будується початковий опорний план перевезень методом північно-західного кута.");
            report.AddText("Після цього виконується перевірка оптимальності плану методом потенціалів.");

            TransportSolution initial = SolveNorthWestCorner(originalInput, report);

            return OptimizeByPotentials(initial, report);
        }

        public static TransportSolution OptimizeByPotentials(TransportSolution initialSolution, ReportBuilder report)
        {
            TransportInput input = initialSolution.Input;

            int rows = input.SupplierCount;
            int columns = input.ConsumerCount;

            double[,] plan = CopyMatrix(initialSolution.Plan);
            bool[,] basis = CopyBasis(initialSolution.Basis);

            report.AddTitle("Метод потенціалів");
            report.AddText("Початковий опорний план:");
            report.AddText(MatrixToString(plan));

            int iteration = 1;

            while (iteration <= 100)
            {
                report.AddTitle("Ітерація методу потенціалів " + iteration);

                double[] u;
                double[] v;

                CalculatePotentials(input.Costs, basis, out u, out v);

                report.AddText("Потенціали постачальників:");
                report.AddText(VectorToString(u, "u"));

                report.AddText("Потенціали споживачів:");
                report.AddText(VectorToString(v, "v"));

                double[,] estimates = CalculateEstimates(input.Costs, basis, u, v);

                report.AddText("Оцінки клітинок:");
                report.AddText(MatrixToString(estimates));

                int enteringRow = -1;
                int enteringColumn = -1;
                double mostNegative = 0;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (!basis[i, j] && estimates[i, j] < mostNegative - Epsilon)
                        {
                            mostNegative = estimates[i, j];
                            enteringRow = i;
                            enteringColumn = j;
                        }
                    }
                }

                if (enteringRow == -1)
                {
                    double finalCost = CalculateTotalCost(input, plan);

                    report.AddText("Усі оцінки небазисних клітинок невід'ємні. План є оптимальним.");
                    report.AddText("Оптимальний план:");
                    report.AddText(MatrixToString(plan));
                    report.AddText("Мінімальна вартість перевезень: " + finalCost.ToString("0.####"));

                    return new TransportSolution(
                        "Метод потенціалів",
                        input,
                        plan,
                        basis,
                        finalCost,
                        u,
                        v,
                        estimates,
                        true);
                }

                report.AddText("Найменша від'ємна оцінка: " + mostNegative.ToString("0.####") +
                               " у клітинці S" + (enteringRow + 1) + "D" + (enteringColumn + 1) + ".");

                Cell[] cycle = FindCycle(basis, enteringRow, enteringColumn);

                if (cycle == null)
                {
                    throw new Exception("Не вдалося побудувати цикл перерахунку для методу потенціалів.");
                }

                report.AddText("Побудований цикл перерахунку:");
                report.AddText(CycleToString(cycle));

                double theta = double.MaxValue;

                for (int k = 1; k < cycle.Length; k += 2)
                {
                    double current = plan[cycle[k].Row, cycle[k].Column];

                    if (current < theta)
                    {
                        theta = current;
                    }
                }

                report.AddText("Мінімальне значення у клітинках зі знаком мінус: " + theta.ToString("0.####") + ".");

                for (int k = 0; k < cycle.Length; k++)
                {
                    int r = cycle[k].Row;
                    int c = cycle[k].Column;

                    if (k % 2 == 0)
                    {
                        plan[r, c] += theta;
                    }
                    else
                    {
                        plan[r, c] -= theta;
                    }
                }

                basis[enteringRow, enteringColumn] = true;

                bool removed = false;

                for (int k = 1; k < cycle.Length; k += 2)
                {
                    int r = cycle[k].Row;
                    int c = cycle[k].Column;

                    if (!removed && Math.Abs(plan[r, c]) <= Epsilon)
                    {
                        basis[r, c] = false;
                        removed = true;
                    }
                }

                EnsureBasisCount(plan, basis, report);

                report.AddText("План після перерахунку:");
                report.AddText(MatrixToString(plan));

                iteration++;
            }

            throw new Exception("Перевищено максимальну кількість ітерацій методу потенціалів.");
        }

        private static TransportInput Balance(TransportInput input, ReportBuilder report)
        {
            double totalSupply = input.TotalSupply();
            double totalDemand = input.TotalDemand();

            if (Math.Abs(totalSupply - totalDemand) <= Epsilon)
            {
                return input;
            }

            int rows = input.SupplierCount;
            int columns = input.ConsumerCount;

            if (totalSupply > totalDemand)
            {
                double difference = totalSupply - totalDemand;

                double[,] newCosts = new double[rows, columns + 1];
                double[] newDemands = new double[columns + 1];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        newCosts[i, j] = input.Costs[i, j];
                    }

                    newCosts[i, columns] = 0;
                }

                for (int j = 0; j < columns; j++)
                {
                    newDemands[j] = input.Demands[j];
                }

                newDemands[columns] = difference;

                report.AddText("Задача відкрита. Сума запасів більша за суму потреб.");
                report.AddText("Додано фіктивний пункт призначення з потребою " + difference.ToString("0.####") + ".");

                return new TransportInput(newCosts, CopyVector(input.Supplies), newDemands, input.Description);
            }
            else
            {
                double difference = totalDemand - totalSupply;

                double[,] newCosts = new double[rows + 1, columns];
                double[] newSupplies = new double[rows + 1];

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        newCosts[i, j] = input.Costs[i, j];
                    }
                }

                for (int j = 0; j < columns; j++)
                {
                    newCosts[rows, j] = 0;
                }

                for (int i = 0; i < rows; i++)
                {
                    newSupplies[i] = input.Supplies[i];
                }

                newSupplies[rows] = difference;

                report.AddText("Задача відкрита. Сума потреб більша за суму запасів.");
                report.AddText("Додано фіктивний пункт відправлення із запасом " + difference.ToString("0.####") + ".");

                return new TransportInput(newCosts, newSupplies, CopyVector(input.Demands), input.Description);
            }
        }

        private static void CalculatePotentials(double[,] costs, bool[,] basis, out double[] u, out double[] v)
        {
            int rows = costs.GetLength(0);
            int columns = costs.GetLength(1);

            u = new double[rows];
            v = new double[columns];

            bool[] uKnown = new bool[rows];
            bool[] vKnown = new bool[columns];

            u[0] = 0;
            uKnown[0] = true;

            bool changed = true;

            while (changed)
            {
                changed = false;

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (!basis[i, j])
                        {
                            continue;
                        }

                        if (uKnown[i] && !vKnown[j])
                        {
                            v[j] = costs[i, j] - u[i];
                            vKnown[j] = true;
                            changed = true;
                        }
                        else if (!uKnown[i] && vKnown[j])
                        {
                            u[i] = costs[i, j] - v[j];
                            uKnown[i] = true;
                            changed = true;
                        }
                    }
                }
            }
        }

        private static double[,] CalculateEstimates(double[,] costs, bool[,] basis, double[] u, double[] v)
        {
            int rows = costs.GetLength(0);
            int columns = costs.GetLength(1);

            double[,] estimates = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (basis[i, j])
                    {
                        estimates[i, j] = 0;
                    }
                    else
                    {
                        estimates[i, j] = costs[i, j] - (u[i] + v[j]);
                    }
                }
            }

            return estimates;
        }

        private static Cell[] FindCycle(bool[,] basis, int startRow, int startColumn)
        {
            Cell[] path = new Cell[basis.GetLength(0) * basis.GetLength(1) + 1];

            path[0] = new Cell(startRow, startColumn);

            Cell[] result = FindCycleRecursive(basis, startRow, startColumn, path, 1, true, startRow, startColumn);

            if (result != null)
            {
                return result;
            }

            return FindCycleRecursive(basis, startRow, startColumn, path, 1, false, startRow, startColumn);
        }

        private static Cell[] FindCycleRecursive(
            bool[,] basis,
            int currentRow,
            int currentColumn,
            Cell[] path,
            int pathLength,
            bool moveByRow,
            int startRow,
            int startColumn)
        {
            int rows = basis.GetLength(0);
            int columns = basis.GetLength(1);

            if (moveByRow)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (j == currentColumn)
                    {
                        continue;
                    }

                    if (currentRow == startRow && j == startColumn && pathLength >= 4)
                    {
                        return CopyPath(path, pathLength);
                    }

                    if (!basis[currentRow, j])
                    {
                        continue;
                    }

                    if (ContainsCell(path, pathLength, currentRow, j))
                    {
                        continue;
                    }

                    path[pathLength] = new Cell(currentRow, j);

                    Cell[] result = FindCycleRecursive(
                        basis,
                        currentRow,
                        j,
                        path,
                        pathLength + 1,
                        !moveByRow,
                        startRow,
                        startColumn);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            else
            {
                for (int i = 0; i < rows; i++)
                {
                    if (i == currentRow)
                    {
                        continue;
                    }

                    if (i == startRow && currentColumn == startColumn && pathLength >= 4)
                    {
                        return CopyPath(path, pathLength);
                    }

                    if (!basis[i, currentColumn])
                    {
                        continue;
                    }

                    if (ContainsCell(path, pathLength, i, currentColumn))
                    {
                        continue;
                    }

                    path[pathLength] = new Cell(i, currentColumn);

                    Cell[] result = FindCycleRecursive(
                        basis,
                        i,
                        currentColumn,
                        path,
                        pathLength + 1,
                        !moveByRow,
                        startRow,
                        startColumn);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        private static Cell[] CopyPath(Cell[] path, int pathLength)
        {
            Cell[] result = new Cell[pathLength];

            for (int i = 0; i < pathLength; i++)
            {
                result[i] = new Cell(path[i].Row, path[i].Column);
            }

            return result;
        }

        private static bool ContainsCell(Cell[] path, int length, int row, int column)
        {
            for (int i = 0; i < length; i++)
            {
                if (path[i].Row == row && path[i].Column == column)
                {
                    return true;
                }
            }

            return false;
        }

        private static void EnsureBasisCount(double[,] plan, bool[,] basis, ReportBuilder report)
        {
            int rows = plan.GetLength(0);
            int columns = plan.GetLength(1);
            int required = rows + columns - 1;
            int current = CountBasis(basis);

            if (current >= required)
            {
                return;
            }

            for (int i = 0; i < rows && current < required; i++)
            {
                for (int j = 0; j < columns && current < required; j++)
                {
                    if (!basis[i, j] && Math.Abs(plan[i, j]) <= Epsilon)
                    {
                        basis[i, j] = true;
                        current++;

                        report.AddText("Додано нульову базисну клітинку S" + (i + 1) +
                                       "D" + (j + 1) + " для уникнення виродження.");
                    }
                }
            }
        }

        private static int CountBasis(bool[,] basis)
        {
            int count = 0;

            for (int i = 0; i < basis.GetLength(0); i++)
            {
                for (int j = 0; j < basis.GetLength(1); j++)
                {
                    if (basis[i, j])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static bool AllClosed(bool[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if (!values[i])
                {
                    return false;
                }
            }

            return true;
        }

        private static double CalculateTotalCost(TransportInput input, double[,] plan)
        {
            double cost = 0;

            for (int i = 0; i < plan.GetLength(0); i++)
            {
                for (int j = 0; j < plan.GetLength(1); j++)
                {
                    cost += plan[i, j] * input.Costs[i, j];
                }
            }

            return cost;
        }

        private static double[] CopyVector(double[] source)
        {
            double[] result = new double[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                result[i] = source[i];
            }

            return result;
        }

        private static double[,] CopyMatrix(double[,] source)
        {
            double[,] result = new double[source.GetLength(0), source.GetLength(1)];

            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    result[i, j] = source[i, j];
                }
            }

            return result;
        }

        private static bool[,] CopyBasis(bool[,] source)
        {
            bool[,] result = new bool[source.GetLength(0), source.GetLength(1)];

            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    result[i, j] = source[i, j];
                }
            }

            return result;
        }

        private static string MatrixToString(double[,] matrix)
        {
            StringBuilder builder = new StringBuilder();

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            builder.Append("          ");

            for (int j = 0; j < columns; j++)
            {
                builder.Append(("D" + (j + 1)).PadLeft(10));
            }

            builder.AppendLine();

            for (int i = 0; i < rows; i++)
            {
                builder.Append(("S" + (i + 1)).PadLeft(10));

                for (int j = 0; j < columns; j++)
                {
                    builder.Append(matrix[i, j].ToString("0.####").PadLeft(10));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string VectorToString(double[] vector, string prefix)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < vector.Length; i++)
            {
                builder.Append(prefix);
                builder.Append(i + 1);
                builder.Append(" = ");
                builder.Append(vector[i].ToString("0.####"));
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string CycleToString(Cell[] cycle)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < cycle.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(" -> ");
                }

                if (i % 2 == 0)
                {
                    builder.Append("+");
                }
                else
                {
                    builder.Append("-");
                }

                builder.Append("S");
                builder.Append(cycle[i].Row + 1);
                builder.Append("D");
                builder.Append(cycle[i].Column + 1);
            }

            return builder.ToString();
        }
    }
}