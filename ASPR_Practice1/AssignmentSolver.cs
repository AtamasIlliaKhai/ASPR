using System;
using System.Text;

namespace ASPR_Practice1
{
    internal static class AssignmentSolver
    {
        private const double Epsilon = 0.0000001;

        public static AssignmentSolution Solve(AssignmentInput input, ReportBuilder report)
        {
            report.AddTitle("Розв'язання задачі про призначення угорським методом");
            report.AddText("Вхідні дані:");
            report.AddText(input.ToString());
            report.AddText("Початкова матриця:");
            report.AddText(MatrixToString(input.Matrix));

            double[,] squareMatrix = MakeSquareMatrix(input.Matrix, input.IsMaximization, report);

            report.AddText("Квадратна матриця для розрахунку:");
            report.AddText(MatrixToString(squareMatrix));

            double[,] workingMatrix = CopyMatrix(squareMatrix);

            if (input.IsMaximization)
            {
                workingMatrix = ConvertMaxToMin(workingMatrix, report);
            }

            ReduceRows(workingMatrix, report);
            ReduceColumns(workingMatrix, report);

            int size = workingMatrix.GetLength(0);

            bool[,] starredZeros = new bool[size, size];
            bool[,] primedZeros = new bool[size, size];
            bool[] coveredRows = new bool[size];
            bool[] coveredColumns = new bool[size];

            StarInitialZeros(workingMatrix, starredZeros, report);
            CoverColumnsWithStarredZeros(starredZeros, coveredColumns, report);

            int iteration = 1;

            while (CountCoveredColumns(coveredColumns) < size)
            {
                report.AddTitle("Ітерація угорського методу " + iteration);
                report.AddText("Поточна робоча матриця:");
                report.AddText(MatrixToString(workingMatrix));
                report.AddText("Кількість покритих стовпців: " + CountCoveredColumns(coveredColumns));

                int zeroRow;
                int zeroColumn;

                bool foundUncoveredZero = FindUncoveredZero(
                    workingMatrix,
                    coveredRows,
                    coveredColumns,
                    out zeroRow,
                    out zeroColumn);

                while (!foundUncoveredZero)
                {
                    double minUncovered = FindMinimumUncoveredValue(workingMatrix, coveredRows, coveredColumns);

                    report.AddText("Непокритих нулів немає.");
                    report.AddText("Мінімальний непокритий елемент: " + minUncovered.ToString("0.####"));

                    TransformMatrix(workingMatrix, coveredRows, coveredColumns, minUncovered, report);

                    report.AddText("Матриця після перетворення:");
                    report.AddText(MatrixToString(workingMatrix));

                    foundUncoveredZero = FindUncoveredZero(
                        workingMatrix,
                        coveredRows,
                        coveredColumns,
                        out zeroRow,
                        out zeroColumn);
                }

                primedZeros[zeroRow, zeroColumn] = true;

                report.AddText("Знайдено непокритий нуль у клітинці A" + (zeroRow + 1) + "B" + (zeroColumn + 1) + ".");
                report.AddText("Цей нуль позначено штрихом.");

                int starredColumn = FindStarInRow(starredZeros, zeroRow);

                if (starredColumn != -1)
                {
                    coveredRows[zeroRow] = true;
                    coveredColumns[starredColumn] = false;

                    report.AddText("У рядку A" + (zeroRow + 1) + " є позначений нуль у стовпці B" + (starredColumn + 1) + ".");
                    report.AddText("Рядок A" + (zeroRow + 1) + " покрито, стовпець B" + (starredColumn + 1) + " відкрито.");
                }
                else
                {
                    report.AddText("У рядку A" + (zeroRow + 1) + " немає позначеного зіркою нуля.");
                    report.AddText("Будується чергувальний ланцюг нулів.");

                    AugmentPath(starredZeros, primedZeros, zeroRow, zeroColumn, report);

                    ClearMatrix(primedZeros);
                    ClearVector(coveredRows);
                    ClearVector(coveredColumns);

                    CoverColumnsWithStarredZeros(starredZeros, coveredColumns, report);
                }

                iteration++;

                if (iteration > 200)
                {
                    throw new Exception("Перевищено максимальну кількість ітерацій угорського методу.");
                }
            }

            int[] assignment = BuildAssignment(starredZeros, input.WorkerCount, input.JobCount);
            double totalValue = CalculateTotalValue(input.Matrix, assignment);

            report.AddTitle("Фінальний план призначень");
            report.AddText("Матриця позначених нулів:");
            report.AddText(BooleanMatrixToString(starredZeros));
            report.AddText("Призначення:");
            report.AddText(AssignmentToString(input, assignment));
            report.AddText("Значення цільової функції: " + totalValue.ToString("0.####"));

            return new AssignmentSolution(input, assignment, totalValue, workingMatrix, true);
        }

        private static double[,] MakeSquareMatrix(double[,] source, bool isMaximization, ReportBuilder report)
        {
            int rows = source.GetLength(0);
            int columns = source.GetLength(1);
            int size = Math.Max(rows, columns);

            double[,] result = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i < rows && j < columns)
                    {
                        result[i, j] = source[i, j];
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }

            if (rows != columns)
            {
                report.AddText("Матриця не є квадратною.");
                report.AddText("Додано фіктивні рядки або стовпці з нульовими значеннями.");
            }

            return result;
        }

        private static double[,] ConvertMaxToMin(double[,] matrix, ReportBuilder report)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            double max = matrix[0, 0];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (matrix[i, j] > max)
                    {
                        max = matrix[i, j];
                    }
                }
            }

            double[,] result = new double[rows, columns];

            report.AddText("Задача максимізації перетворюється на задачу мінімізації.");
            report.AddText("Максимальний елемент матриці: " + max.ToString("0.####"));
            report.AddText("Кожен елемент нової матриці обчислюється як max - c_ij.");

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = max - matrix[i, j];
                }
            }

            report.AddText("Матриця після перетворення задачі максимізації:");
            report.AddText(MatrixToString(result));

            return result;
        }

        private static void ReduceRows(double[,] matrix, ReportBuilder report)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            report.AddTitle("Редукція рядків");
            report.AddText("У кожному рядку знаходиться мінімальний елемент.");
            report.AddText("Після цього він віднімається від усіх елементів відповідного рядка.");

            for (int i = 0; i < rows; i++)
            {
                double min = matrix[i, 0];

                for (int j = 1; j < columns; j++)
                {
                    if (matrix[i, j] < min)
                    {
                        min = matrix[i, j];
                    }
                }

                report.AddText("Мінімум рядка A" + (i + 1) + " дорівнює " + min.ToString("0.####") + ".");

                for (int j = 0; j < columns; j++)
                {
                    matrix[i, j] -= min;
                }
            }

            report.AddText("Матриця після редукції рядків:");
            report.AddText(MatrixToString(matrix));
        }

        private static void ReduceColumns(double[,] matrix, ReportBuilder report)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            report.AddTitle("Редукція стовпців");
            report.AddText("У кожному стовпці знаходиться мінімальний елемент.");
            report.AddText("Після цього він віднімається від усіх елементів відповідного стовпця.");

            for (int j = 0; j < columns; j++)
            {
                double min = matrix[0, j];

                for (int i = 1; i < rows; i++)
                {
                    if (matrix[i, j] < min)
                    {
                        min = matrix[i, j];
                    }
                }

                report.AddText("Мінімум стовпця B" + (j + 1) + " дорівнює " + min.ToString("0.####") + ".");

                for (int i = 0; i < rows; i++)
                {
                    matrix[i, j] -= min;
                }
            }

            report.AddText("Матриця після редукції стовпців:");
            report.AddText(MatrixToString(matrix));
        }

        private static void StarInitialZeros(double[,] matrix, bool[,] starredZeros, ReportBuilder report)
        {
            int size = matrix.GetLength(0);

            bool[] rowHasStar = new bool[size];
            bool[] columnHasStar = new bool[size];

            report.AddTitle("Початкове позначення незалежних нулів");

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (Math.Abs(matrix[i, j]) <= Epsilon && !rowHasStar[i] && !columnHasStar[j])
                    {
                        starredZeros[i, j] = true;
                        rowHasStar[i] = true;
                        columnHasStar[j] = true;

                        report.AddText("Позначено нуль у клітинці A" + (i + 1) + "B" + (j + 1) + ".");

                        break;
                    }
                }
            }

            report.AddText("Матриця позначених нулів:");
            report.AddText(BooleanMatrixToString(starredZeros));
        }

        private static void CoverColumnsWithStarredZeros(bool[,] starredZeros, bool[] coveredColumns, ReportBuilder report)
        {
            int size = starredZeros.GetLength(0);

            for (int j = 0; j < size; j++)
            {
                coveredColumns[j] = false;
            }

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (starredZeros[i, j])
                    {
                        coveredColumns[j] = true;
                    }
                }
            }

            report.AddText("Покрито стовпці, у яких є позначені нулі:");
            report.AddText(VectorToString(coveredColumns, "B"));
        }

        private static bool FindUncoveredZero(
            double[,] matrix,
            bool[] coveredRows,
            bool[] coveredColumns,
            out int zeroRow,
            out int zeroColumn)
        {
            int size = matrix.GetLength(0);

            for (int i = 0; i < size; i++)
            {
                if (coveredRows[i])
                {
                    continue;
                }

                for (int j = 0; j < size; j++)
                {
                    if (coveredColumns[j])
                    {
                        continue;
                    }

                    if (Math.Abs(matrix[i, j]) <= Epsilon)
                    {
                        zeroRow = i;
                        zeroColumn = j;
                        return true;
                    }
                }
            }

            zeroRow = -1;
            zeroColumn = -1;
            return false;
        }

        private static double FindMinimumUncoveredValue(
            double[,] matrix,
            bool[] coveredRows,
            bool[] coveredColumns)
        {
            int size = matrix.GetLength(0);

            double min = double.MaxValue;

            for (int i = 0; i < size; i++)
            {
                if (coveredRows[i])
                {
                    continue;
                }

                for (int j = 0; j < size; j++)
                {
                    if (coveredColumns[j])
                    {
                        continue;
                    }

                    if (matrix[i, j] < min)
                    {
                        min = matrix[i, j];
                    }
                }
            }

            return min;
        }

        private static void TransformMatrix(
            double[,] matrix,
            bool[] coveredRows,
            bool[] coveredColumns,
            double value,
            ReportBuilder report)
        {
            int size = matrix.GetLength(0);

            report.AddText("Від усіх непокритих елементів віднімається " + value.ToString("0.####") + ".");
            report.AddText("До елементів на перетині покритих рядків і покритих стовпців додається " + value.ToString("0.####") + ".");

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!coveredRows[i] && !coveredColumns[j])
                    {
                        matrix[i, j] -= value;
                    }
                    else if (coveredRows[i] && coveredColumns[j])
                    {
                        matrix[i, j] += value;
                    }
                }
            }
        }

        private static int FindStarInRow(bool[,] starredZeros, int row)
        {
            int size = starredZeros.GetLength(1);

            for (int j = 0; j < size; j++)
            {
                if (starredZeros[row, j])
                {
                    return j;
                }
            }

            return -1;
        }

        private static int FindStarInColumn(bool[,] starredZeros, int column)
        {
            int size = starredZeros.GetLength(0);

            for (int i = 0; i < size; i++)
            {
                if (starredZeros[i, column])
                {
                    return i;
                }
            }

            return -1;
        }

        private static int FindPrimeInRow(bool[,] primedZeros, int row)
        {
            int size = primedZeros.GetLength(1);

            for (int j = 0; j < size; j++)
            {
                if (primedZeros[row, j])
                {
                    return j;
                }
            }

            return -1;
        }

        private static void AugmentPath(
            bool[,] starredZeros,
            bool[,] primedZeros,
            int startRow,
            int startColumn,
            ReportBuilder report)
        {
            int size = starredZeros.GetLength(0);
            int[] pathRows = new int[size * 2 + 1];
            int[] pathColumns = new int[size * 2 + 1];

            int pathLength = 1;
            pathRows[0] = startRow;
            pathColumns[0] = startColumn;

            bool done = false;

            while (!done)
            {
                int row = FindStarInColumn(starredZeros, pathColumns[pathLength - 1]);

                if (row != -1)
                {
                    pathRows[pathLength] = row;
                    pathColumns[pathLength] = pathColumns[pathLength - 1];
                    pathLength++;

                    int column = FindPrimeInRow(primedZeros, pathRows[pathLength - 1]);

                    pathRows[pathLength] = pathRows[pathLength - 1];
                    pathColumns[pathLength] = column;
                    pathLength++;
                }
                else
                {
                    done = true;
                }
            }

            report.AddText("Чергувальний ланцюг:");
            report.AddText(PathToString(pathRows, pathColumns, pathLength));

            for (int k = 0; k < pathLength; k++)
            {
                int r = pathRows[k];
                int c = pathColumns[k];

                starredZeros[r, c] = !starredZeros[r, c];
            }

            report.AddText("Після перетворення ланцюга позначені нулі оновлено.");
            report.AddText(BooleanMatrixToString(starredZeros));
        }

        private static int CountCoveredColumns(bool[] coveredColumns)
        {
            int count = 0;

            for (int i = 0; i < coveredColumns.Length; i++)
            {
                if (coveredColumns[i])
                {
                    count++;
                }
            }

            return count;
        }

        private static int[] BuildAssignment(bool[,] starredZeros, int originalRows, int originalColumns)
        {
            int[] assignment = new int[originalRows];

            for (int i = 0; i < originalRows; i++)
            {
                assignment[i] = -1;
            }

            for (int i = 0; i < originalRows; i++)
            {
                for (int j = 0; j < originalColumns; j++)
                {
                    if (starredZeros[i, j])
                    {
                        assignment[i] = j;
                    }
                }
            }

            return assignment;
        }

        private static double CalculateTotalValue(double[,] originalMatrix, int[] assignment)
        {
            double total = 0;

            for (int i = 0; i < assignment.Length; i++)
            {
                if (assignment[i] >= 0)
                {
                    total += originalMatrix[i, assignment[i]];
                }
            }

            return total;
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

        private static void ClearMatrix(bool[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = false;
                }
            }
        }

        private static void ClearVector(bool[] vector)
        {
            for (int i = 0; i < vector.Length; i++)
            {
                vector[i] = false;
            }
        }

        private static string MatrixToString(double[,] matrix)
        {
            StringBuilder builder = new StringBuilder();

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            builder.Append("          ");

            for (int j = 0; j < columns; j++)
            {
                builder.Append(("B" + (j + 1)).PadLeft(10));
            }

            builder.AppendLine();

            for (int i = 0; i < rows; i++)
            {
                builder.Append(("A" + (i + 1)).PadLeft(10));

                for (int j = 0; j < columns; j++)
                {
                    builder.Append(matrix[i, j].ToString("0.####").PadLeft(10));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string BooleanMatrixToString(bool[,] matrix)
        {
            StringBuilder builder = new StringBuilder();

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            builder.Append("          ");

            for (int j = 0; j < columns; j++)
            {
                builder.Append(("B" + (j + 1)).PadLeft(10));
            }

            builder.AppendLine();

            for (int i = 0; i < rows; i++)
            {
                builder.Append(("A" + (i + 1)).PadLeft(10));

                for (int j = 0; j < columns; j++)
                {
                    if (matrix[i, j])
                    {
                        builder.Append("*".PadLeft(10));
                    }
                    else
                    {
                        builder.Append(".".PadLeft(10));
                    }
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string VectorToString(bool[] values, string prefix)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                builder.Append(prefix);
                builder.Append(i + 1);
                builder.Append(": ");

                if (values[i])
                {
                    builder.AppendLine("покрито");
                }
                else
                {
                    builder.AppendLine("не покрито");
                }
            }

            return builder.ToString();
        }

        private static string AssignmentToString(AssignmentInput input, int[] assignment)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < assignment.Length; i++)
            {
                if (assignment[i] >= 0)
                {
                    builder.AppendLine("A" + (i + 1) + " -> B" + (assignment[i] + 1) +
                                       ", значення = " + input.Matrix[i, assignment[i]].ToString("0.####"));
                }
                else
                {
                    builder.AppendLine("A" + (i + 1) + " -> призначення відсутнє");
                }
            }

            return builder.ToString();
        }

        private static string PathToString(int[] rows, int[] columns, int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                if (i > 0)
                {
                    builder.Append(" -> ");
                }

                builder.Append("A");
                builder.Append(rows[i] + 1);
                builder.Append("B");
                builder.Append(columns[i] + 1);
            }

            return builder.ToString();
        }
    }
}