using System;
using System.Text;

namespace ASPR_Practice1
{
    internal static class MatrixGameSolver
    {
        private const double Epsilon = 0.0000001;

        public static NatureGameResult SolveNatureGame(
            double[,] utilityMatrix,
            double hurwiczAlpha,
            double[] probabilities,
            ReportBuilder report)
        {
            NatureGameInput input = new NatureGameInput(
                utilityMatrix,
                hurwiczAlpha,
                probabilities,
                "Гра з природою, введена користувачем"
            );

            return SolveNatureGame(input, report);
        }

        public static NatureGameResult SolveNatureGame(NatureGameInput input, ReportBuilder report)
        {
            ValidateInput(input.UtilityMatrix, input.HurwiczAlpha, input.Probabilities);

            double[,] utilityMatrix = input.UtilityMatrix;
            double hurwiczAlpha = input.HurwiczAlpha;
            double[] probabilities = input.Probabilities;

            report.AddTitle("Розв'язання гри з природою");
            report.AddText(input.Description);
            report.AddText("");
            report.AddText("Матриця корисності U:");
            report.AddText(MatrixToString(utilityMatrix));
            report.AddText("Коефіцієнт оптимізму для критерію Гурвіца: " + FormatNumber(hurwiczAlpha));
            report.AddText("Ймовірності станів природи для критерію Байєса:");
            report.AddText(VectorToString(probabilities, "p"));
            report.AddText("");

            double[] rowMinimums = CalculateRowMinimums(utilityMatrix);
            int[] waldStrategies = FindMaxIndexes(rowMinimums);

            report.AddTitle("Критерій Вальда");
            report.AddText("За критерієм Вальда для кожної стратегії гравця знаходиться мінімальний виграш.");
            report.AddText("Після цього серед знайдених мінімальних виграшів обирається найбільше значення.");
            report.AddText("Розрахунок мінімальних виграшів:");
            AddRowMinimumsToReport(utilityMatrix, rowMinimums, report);
            report.AddText("Найбільше серед мінімальних значень: " + FormatNumber(Max(rowMinimums)));
            report.AddText("Оптимальна стратегія за критерієм Вальда: " + StrategiesToString(waldStrategies));
            report.AddText("");

            double[] rowMaximums = CalculateRowMaximums(utilityMatrix);
            int[] optimismStrategies = FindMaxIndexes(rowMaximums);

            report.AddTitle("Критерій оптимізму");
            report.AddText("За критерієм оптимізму для кожної стратегії знаходиться максимальний виграш.");
            report.AddText("Після цього серед знайдених максимальних виграшів обирається найбільше значення.");
            report.AddText("Розрахунок максимальних виграшів:");
            AddRowMaximumsToReport(utilityMatrix, rowMaximums, report);
            report.AddText("Найбільше серед максимальних значень: " + FormatNumber(Max(rowMaximums)));
            report.AddText("Оптимальна стратегія за критерієм оптимізму: " + StrategiesToString(optimismStrategies));
            report.AddText("");

            double[] hurwiczValues = CalculateHurwiczValues(rowMinimums, rowMaximums, hurwiczAlpha);
            int[] hurwiczStrategies = FindMaxIndexes(hurwiczValues);

            report.AddTitle("Критерій Гурвіца");
            report.AddText("За критерієм Гурвіца оцінка стратегії поєднує найкращий і найгірший результати.");
            report.AddText("Використовується формула:");
            report.AddText("H_i = alpha * max(A_i) + (1 - alpha) * min(A_i)");
            report.AddText("Розрахунок оцінок Гурвіца:");
            AddHurwiczToReport(rowMinimums, rowMaximums, hurwiczValues, hurwiczAlpha, report);
            report.AddText("Найбільше значення за критерієм Гурвіца: " + FormatNumber(Max(hurwiczValues)));
            report.AddText("Оптимальна стратегія за критерієм Гурвіца: " + StrategiesToString(hurwiczStrategies));
            report.AddText("");

            double[,] riskMatrix = BuildRiskMatrix(utilityMatrix);
            double[] maxRisks = CalculateRowMaximums(riskMatrix);
            int[] savageStrategies = FindMinIndexes(maxRisks);

            report.AddTitle("Критерій Севіджа");
            report.AddText("За критерієм Севіджа спочатку будується матриця ризиків.");
            report.AddText("Для кожного стану природи знаходиться найбільший виграш у відповідному стовпці.");
            report.AddText("Потім із цього найбільшого виграшу віднімається кожен елемент стовпця.");
            report.AddText("Матриця ризиків:");
            report.AddText(MatrixToString(riskMatrix));
            report.AddText("Максимальні ризики за рядками:");
            AddRowMaximumsToReport(riskMatrix, maxRisks, report);
            report.AddText("Найменше серед максимальних ризиків: " + FormatNumber(Min(maxRisks)));
            report.AddText("Оптимальна стратегія за критерієм Севіджа: " + StrategiesToString(savageStrategies));
            report.AddText("");

            double[] bayesValues = CalculateBayesValues(utilityMatrix, probabilities);
            int[] bayesStrategies = FindMaxIndexes(bayesValues);

            report.AddTitle("Критерій Байєса");
            report.AddText("За критерієм Байєса для кожної стратегії обчислюється математичне сподівання виграшу.");
            report.AddText("Для цього виграші множаться на відповідні ймовірності станів природи.");
            report.AddText("Розрахунок значень за критерієм Байєса:");
            AddBayesToReport(utilityMatrix, probabilities, bayesValues, report);
            report.AddText("Найбільше значення за критерієм Байєса: " + FormatNumber(Max(bayesValues)));
            report.AddText("Оптимальна стратегія за критерієм Байєса: " + StrategiesToString(bayesStrategies));
            report.AddText("");

            double[] laplaceValues = CalculateLaplaceValues(utilityMatrix);
            int[] laplaceStrategies = FindMaxIndexes(laplaceValues);

            report.AddTitle("Критерій Лапласа");
            report.AddText("За критерієм Лапласа всі стани природи вважаються рівноймовірними.");
            report.AddText("Тому для кожної стратегії обчислюється середнє арифметичне виграшів.");
            report.AddText("Розрахунок значень за критерієм Лапласа:");
            AddLaplaceToReport(utilityMatrix, laplaceValues, report);
            report.AddText("Найбільше значення за критерієм Лапласа: " + FormatNumber(Max(laplaceValues)));
            report.AddText("Оптимальна стратегія за критерієм Лапласа: " + StrategiesToString(laplaceStrategies));
            report.AddText("");

            return new NatureGameResult(
                utilityMatrix,
                hurwiczAlpha,
                probabilities,
                rowMinimums,
                waldStrategies,
                rowMaximums,
                optimismStrategies,
                hurwiczValues,
                hurwiczStrategies,
                riskMatrix,
                maxRisks,
                savageStrategies,
                bayesValues,
                bayesStrategies,
                laplaceValues,
                laplaceStrategies);
        }

        public static double[,] CreatePractice4TestMatrix()
        {
            return NatureGameInput.CreatePractice4Test1().UtilityMatrix;
        }

        public static double[,] CreatePractice4Variant3()
        {
            return NatureGameInput.CreatePractice4Variant3().UtilityMatrix;
        }

        private static void ValidateInput(double[,] matrix, double alpha, double[] probabilities)
        {
            if (matrix == null)
            {
                throw new Exception("Матриця корисності не задана.");
            }

            if (probabilities == null)
            {
                throw new Exception("Ймовірності станів природи не задані.");
            }

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            if (rows == 0 || columns == 0)
            {
                throw new Exception("Матриця корисності не може бути порожньою.");
            }

            if (probabilities.Length != columns)
            {
                throw new Exception("Кількість ймовірностей має збігатися з кількістю стовпців матриці.");
            }

            if (alpha < -Epsilon || alpha > 1 + Epsilon)
            {
                throw new Exception("Коефіцієнт оптимізму має бути в межах від 0 до 1.");
            }

            double sum = 0;

            for (int i = 0; i < probabilities.Length; i++)
            {
                if (probabilities[i] < -Epsilon)
                {
                    throw new Exception("Ймовірності не можуть бути від'ємними.");
                }

                sum += probabilities[i];
            }

            if (Math.Abs(sum - 1.0) > 0.0001)
            {
                throw new Exception("Сума ймовірностей має дорівнювати 1.");
            }
        }

        private static double[] CalculateRowMinimums(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            double[] result = new double[rows];

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

                result[i] = min;
            }

            return result;
        }

        private static double[] CalculateRowMaximums(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            double[] result = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                double max = matrix[i, 0];

                for (int j = 1; j < columns; j++)
                {
                    if (matrix[i, j] > max)
                    {
                        max = matrix[i, j];
                    }
                }

                result[i] = max;
            }

            return result;
        }

        private static double[] CalculateHurwiczValues(double[] rowMinimums, double[] rowMaximums, double alpha)
        {
            double[] result = new double[rowMinimums.Length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = alpha * rowMaximums[i] + (1.0 - alpha) * rowMinimums[i];
            }

            return result;
        }

        private static double[,] BuildRiskMatrix(double[,] utilityMatrix)
        {
            int rows = utilityMatrix.GetLength(0);
            int columns = utilityMatrix.GetLength(1);

            double[,] riskMatrix = new double[rows, columns];

            for (int j = 0; j < columns; j++)
            {
                double columnMax = utilityMatrix[0, j];

                for (int i = 1; i < rows; i++)
                {
                    if (utilityMatrix[i, j] > columnMax)
                    {
                        columnMax = utilityMatrix[i, j];
                    }
                }

                for (int i = 0; i < rows; i++)
                {
                    riskMatrix[i, j] = columnMax - utilityMatrix[i, j];
                }
            }

            return riskMatrix;
        }

        private static double[] CalculateBayesValues(double[,] matrix, double[] probabilities)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            double[] result = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                double value = 0;

                for (int j = 0; j < columns; j++)
                {
                    value += matrix[i, j] * probabilities[j];
                }

                result[i] = value;
            }

            return result;
        }

        private static double[] CalculateLaplaceValues(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            double[] result = new double[rows];

            for (int i = 0; i < rows; i++)
            {
                double sum = 0;

                for (int j = 0; j < columns; j++)
                {
                    sum += matrix[i, j];
                }

                result[i] = sum / columns;
            }

            return result;
        }

        private static void AddRowMinimumsToReport(double[,] matrix, double[] rowMinimums, ReportBuilder report)
        {
            for (int i = 0; i < rowMinimums.Length; i++)
            {
                report.AddText("A" + (i + 1) + ": min = " + FormatNumber(rowMinimums[i]));
            }
        }

        private static void AddRowMaximumsToReport(double[,] matrix, double[] rowMaximums, ReportBuilder report)
        {
            for (int i = 0; i < rowMaximums.Length; i++)
            {
                report.AddText("A" + (i + 1) + ": max = " + FormatNumber(rowMaximums[i]));
            }
        }

        private static void AddHurwiczToReport(
            double[] rowMinimums,
            double[] rowMaximums,
            double[] hurwiczValues,
            double alpha,
            ReportBuilder report)
        {
            for (int i = 0; i < hurwiczValues.Length; i++)
            {
                string text =
                    "H" + (i + 1) + " = " +
                    FormatNumber(alpha) + " * " + FormatNumber(rowMaximums[i]) +
                    " + " + FormatNumber(1.0 - alpha) + " * " + FormatNumber(rowMinimums[i]) +
                    " = " + FormatNumber(hurwiczValues[i]);

                report.AddText(text);
            }
        }

        private static void AddBayesToReport(double[,] matrix, double[] probabilities, double[] bayesValues, ReportBuilder report)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("B");
                builder.Append(i + 1);
                builder.Append(" = ");

                for (int j = 0; j < columns; j++)
                {
                    if (j > 0)
                    {
                        builder.Append(" + ");
                    }

                    builder.Append(FormatNumber(matrix[i, j]));
                    builder.Append(" * ");
                    builder.Append(FormatNumber(probabilities[j]));
                }

                builder.Append(" = ");
                builder.Append(FormatNumber(bayesValues[i]));

                report.AddText(builder.ToString());
            }
        }

        private static void AddLaplaceToReport(double[,] matrix, double[] laplaceValues, ReportBuilder report)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                StringBuilder builder = new StringBuilder();

                builder.Append("L");
                builder.Append(i + 1);
                builder.Append(" = (");

                for (int j = 0; j < columns; j++)
                {
                    if (j > 0)
                    {
                        builder.Append(" + ");
                    }

                    builder.Append(FormatNumber(matrix[i, j]));
                }

                builder.Append(") / ");
                builder.Append(columns);
                builder.Append(" = ");
                builder.Append(FormatNumber(laplaceValues[i]));

                report.AddText(builder.ToString());
            }
        }

        private static int[] FindMaxIndexes(double[] values)
        {
            double max = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
            }

            return FindIndexesByValue(values, max);
        }

        private static int[] FindMinIndexes(double[] values)
        {
            double min = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] < min)
                {
                    min = values[i];
                }
            }

            return FindIndexesByValue(values, min);
        }

        private static int[] FindIndexesByValue(double[] values, double target)
        {
            int count = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (Math.Abs(values[i] - target) <= Epsilon)
                {
                    count++;
                }
            }

            int[] indexes = new int[count];
            int index = 0;

            for (int i = 0; i < values.Length; i++)
            {
                if (Math.Abs(values[i] - target) <= Epsilon)
                {
                    indexes[index] = i;
                    index++;
                }
            }

            return indexes;
        }

        private static double Max(double[] values)
        {
            double max = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
            }

            return max;
        }

        private static double Min(double[] values)
        {
            double min = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] < min)
                {
                    min = values[i];
                }
            }

            return min;
        }

        private static string StrategiesToString(int[] strategies)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < strategies.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append("; ");
                }

                builder.Append("A");
                builder.Append(strategies[i] + 1);
            }

            return builder.ToString();
        }

        private static string MatrixToString(double[,] matrix)
        {
            StringBuilder builder = new StringBuilder();

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            builder.Append("          ");

            for (int j = 0; j < columns; j++)
            {
                builder.Append(("П" + (j + 1)).PadLeft(10));
            }

            builder.AppendLine();

            for (int i = 0; i < rows; i++)
            {
                builder.Append(("A" + (i + 1)).PadLeft(10));

                for (int j = 0; j < columns; j++)
                {
                    builder.Append(FormatNumber(matrix[i, j]).PadLeft(10));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string VectorToString(double[] values, string prefix)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                builder.Append(prefix);
                builder.Append(i + 1);
                builder.Append(" = ");
                builder.Append(FormatNumber(values[i]));
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string FormatNumber(double value)
        {
            return Math.Round(value, 6).ToString("0.######");
        }
    }
}