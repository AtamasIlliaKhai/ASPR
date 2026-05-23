using System;

namespace ASPR_Practice1
{
    internal static class JordanSolver
    {
        private const double Epsilon = 0.0000001;

        public static Matrix JordanElimination(Matrix source, int pivotRow, int pivotColumn)
        {
            Matrix result = new Matrix(source.Rows, source.Columns);

            double pivot = source[pivotRow, pivotColumn];

            if (Math.Abs(pivot) < Epsilon)
            {
                throw new Exception("Розв'язувальний елемент дорівнює нулю. ЗЖВ виконати неможливо.");
            }

            for (int i = 0; i < source.Rows; i++)
            {
                for (int j = 0; j < source.Columns; j++)
                {
                    if (i == pivotRow && j == pivotColumn)
                    {
                        result[i, j] = 1.0 / pivot;
                    }
                    else if (i == pivotRow)
                    {
                        result[i, j] = -source[i, j] / pivot;
                    }
                    else if (j == pivotColumn)
                    {
                        result[i, j] = source[i, j] / pivot;
                    }
                    else
                    {
                        result[i, j] =
                            (source[i, j] * pivot - source[i, pivotColumn] * source[pivotRow, j]) / pivot;
                    }
                }
            }

            return result;
        }

        public static Matrix FindInverseMatrix(Matrix matrix, ReportBuilder report)
        {
            if (matrix.Rows != matrix.Columns)
            {
                throw new Exception("Обернена матриця існує тільки для квадратної матриці.");
            }

            Matrix current = matrix.Copy();

            report.AddTitle("Пошук оберненої матриці за допомогою ЗЖВ");
            report.AddMatrix("Вхідна матриця A:", current);

            for (int k = 0; k < current.Rows; k++)
            {
                if (Math.Abs(current[k, k]) < Epsilon)
                {
                    int swapRow = FindNonZeroRow(current, k, k);

                    if (swapRow == -1)
                    {
                        throw new Exception("Матриця вироджена. Обернена матриця не існує.");
                    }

                    current.SwapRows(k, swapRow);

                    report.AddText("Виконано перестановку рядків " + (k + 1) + " і " + (swapRow + 1));
                    report.AddMatrix("Матриця після перестановки:", current);
                }

                report.AddStep(
                    k + 1,
                    "Розв'язувальний елемент A[" + (k + 1) + "," + (k + 1) + "] = " +
                    Math.Round(current[k, k], 4)
                );

                current = JordanElimination(current, k, k);

                report.AddMatrix("Матриця після виконання ЗЖВ:", current);
            }

            report.AddMatrix("Обернена матриця A^(-1):", current);

            return current;
        }

        public static int FindRank(Matrix matrix, ReportBuilder report)
        {
            Matrix current = matrix.Copy();

            int rank = 0;
            int row = 0;
            int column = 0;
            int step = 1;

            report.AddTitle("Обчислення рангу матриці за допомогою ЗЖВ");
            report.AddMatrix("Вхідна матриця:", current);

            while (row < current.Rows && column < current.Columns)
            {
                int pivotRow = FindNonZeroRow(current, row, column);

                if (pivotRow == -1)
                {
                    column++;
                    continue;
                }

                if (pivotRow != row)
                {
                    current.SwapRows(row, pivotRow);

                    report.AddText("Виконано перестановку рядків " + (row + 1) + " і " + (pivotRow + 1));
                    report.AddMatrix("Матриця після перестановки:", current);
                }

                report.AddStep(
                    step,
                    "Розв'язувальний елемент A[" + (row + 1) + "," + (column + 1) + "] = " +
                    Math.Round(current[row, column], 4)
                );

                current = JordanElimination(current, row, column);

                report.AddMatrix("Матриця після виконання ЗЖВ:", current);

                rank++;
                row++;
                column++;
                step++;
            }

            report.AddText("Ранг матриці дорівнює кількості виконаних ЗЖВ з ненульовими розв'язувальними елементами.");
            report.AddText("Ранг матриці: " + rank);

            return rank;
        }

        public static Matrix SolveByGauss(Matrix matrixA, Matrix matrixB, ReportBuilder report)
        {
            if (matrixA.Rows != matrixA.Columns)
            {
                throw new Exception("Матриця A повинна бути квадратною.");
            }

            if (matrixB.Columns != 1 || matrixB.Rows != matrixA.Rows)
            {
                throw new Exception("Матриця B повинна бути вектором-стовпцем відповідної розмірності.");
            }

            int n = matrixA.Rows;

            Matrix workingMatrix = new Matrix(n, n + 1);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    workingMatrix[i, j] = matrixA[i, j];
                }

                workingMatrix[i, n] = -matrixB[i, 0];
            }

            double[][] formulas = new double[n][];

            report.AddTitle("Розв'язання СЛАР методом Гауса з використанням ЗЖВ");
            report.AddMatrix("Вхідна матриця A:", matrixA);
            report.AddMatrix("Вхідна матриця B:", matrixB);
            report.AddMatrix("Система, приведена до вигляду AX - B = 0:", workingMatrix);

            for (int step = 0; step < n; step++)
            {
                if (Math.Abs(workingMatrix[0, 0]) < Epsilon)
                {
                    int swapRow = FindNonZeroRow(workingMatrix, 0, 0);

                    if (swapRow == -1)
                    {
                        throw new Exception("Система не має єдиного розв'язку.");
                    }

                    workingMatrix.SwapRows(0, swapRow);

                    report.AddText("Виконано перестановку рядків 1 і " + (swapRow + 1));
                    report.AddMatrix("Матриця після перестановки:", workingMatrix);
                }

                report.AddStep(
                    step + 1,
                    "Розв'язувальний елемент A[1,1] = " + Math.Round(workingMatrix[0, 0], 4)
                );

                Matrix afterJordan = JordanElimination(workingMatrix, 0, 0);

                formulas[step] = new double[afterJordan.Columns - 1];

                for (int j = 1; j < afterJordan.Columns; j++)
                {
                    formulas[step][j - 1] = afterJordan[0, j];
                }

                report.AddText(BuildFormulaText(step, formulas[step]));
                report.AddMatrix("Матриця після виконання ЗЖВ:", afterJordan);

                if (step < n - 1)
                {
                    workingMatrix = afterJordan.RemoveFirstRowAndColumn();
                    report.AddMatrix("Матриця після викреслення розв'язувального рядка і стовпця:", workingMatrix);
                }
            }

            Matrix result = new Matrix(n, 1);

            report.AddTitle("Зворотний хід методу Гауса");

            for (int i = n - 1; i >= 0; i--)
            {
                double[] formula = formulas[i];

                double value = formula[formula.Length - 1];

                for (int j = 0; j < formula.Length - 1; j++)
                {
                    value += formula[j] * result[i + j + 1, 0];
                }

                result[i, 0] = value;

                report.AddText("x" + (i + 1) + " = " + Math.Round(value, 4));
            }

            report.AddMatrix("Вектор розв'язку X:", result);

            return result;
        }

        private static string BuildFormulaText(int step, double[] formula)
        {
            string text = "Розв'язувальний рядок: x" + (step + 1) + " = ";

            for (int i = 0; i < formula.Length - 1; i++)
            {
                text += "(" + Math.Round(formula[i], 4) + ") * x" + (step + i + 2);

                if (i < formula.Length - 2)
                {
                    text += " + ";
                }
            }

            if (formula.Length > 1)
            {
                text += " + ";
            }

            text += "(" + Math.Round(formula[formula.Length - 1], 4) + ")";

            return text;
        }

        private static int FindNonZeroRow(Matrix matrix, int startRow, int column)
        {
            for (int i = startRow; i < matrix.Rows; i++)
            {
                if (Math.Abs(matrix[i, column]) > Epsilon)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}