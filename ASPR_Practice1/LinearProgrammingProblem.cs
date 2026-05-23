using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class LinearProgrammingProblem
    {
        public double[] GoalCoefficients { get; private set; }
        public GoalType GoalType { get; private set; }
        public Inequality[] Inequalities { get; private set; }

        /*
         * PreparedRowsForSimplexTable — це готові рядки для симплекс-таблиці.
         * Формат одного рядка:
         *
         * коефіцієнти x1, x2, ..., xn, вільний член
         *
         * Наприклад:
         * 2, -1, 3, 4, 10
         */
        public double[,] PreparedRowsForSimplexTable { get; private set; }

        public string OriginalDescription { get; private set; }

        public int VariableCount
        {
            get { return GoalCoefficients.Length; }
        }

        public int ConstraintCount
        {
            get
            {
                if (PreparedRowsForSimplexTable != null)
                {
                    return PreparedRowsForSimplexTable.GetLength(0);
                }

                return Inequalities.Length;
            }
        }

        public LinearProgrammingProblem(double[] goalCoefficients, GoalType goalType, Inequality[] inequalities)
        {
            GoalCoefficients = goalCoefficients;
            GoalType = goalType;
            Inequalities = inequalities;
            PreparedRowsForSimplexTable = null;
            OriginalDescription = "";
        }

        public LinearProgrammingProblem(
            double[] goalCoefficients,
            GoalType goalType,
            double[,] preparedRowsForSimplexTable,
            string originalDescription)
        {
            GoalCoefficients = goalCoefficients;
            GoalType = goalType;
            PreparedRowsForSimplexTable = preparedRowsForSimplexTable;
            Inequalities = new Inequality[0];
            OriginalDescription = originalDescription;
        }

        public double[,] GetSimplexRows()
        {
            if (PreparedRowsForSimplexTable != null)
            {
                return CopyRows(PreparedRowsForSimplexTable);
            }

            double[,] rows = new double[Inequalities.Length, VariableCount + 1];

            for (int i = 0; i < Inequalities.Length; i++)
            {
                double[] row = Inequalities[i].ToSimplexRow();

                for (int j = 0; j < row.Length; j++)
                {
                    rows[i, j] = row[j];
                }
            }

            return rows;
        }

        private double[,] CopyRows(double[,] source)
        {
            int rows = source.GetLength(0);
            int columns = source.GetLength(1);

            double[,] result = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    result[i, j] = source[i, j];
                }
            }

            return result;
        }

        /*
         * Власний варіант 3.
         *
         * Початкова задача:
         * Z = 3*x1 + x2 + x3 - x4 -> max
         *
         * -x1 + x2 + x3 + x4 <= 2
         * x1 - x2 + x3 + x4 <= 2
         * x1 + x2 - x3 + x4 <= 2
         * x1 + x2 + x3 - x4 <= 2
         *
         * За умовою у другій нерівності знак змінено на протилежний:
         * x1 - x2 + x3 + x4 >= 2
         *
         * Для лекційного алгоритму нижче задаються готові рядки таблиці.
         */
        public static LinearProgrammingProblem CreateVariant3()
        {
            double[] goal = new double[]
            {
                3, 1, 1, -1
            };

            double[,] rows = new double[,]
            {
                { -1,  1,  1,  1,  2 },
                {  1, -1,  1,  1,  2 },
                {  1,  1, -1,  1,  2 },
                {  1,  1,  1, -1,  2 }
            };

            string description =
                "Варіант 3\r\n" +
                "Z = 3*x1 + 1*x2 + 1*x3 - 1*x4 -> max\r\n" +
                "При обмеженнях:\r\n" +
                "-1*x1 + 1*x2 + 1*x3 + 1*x4 <= 2\r\n" +
                "1*x1 - 1*x2 + 1*x3 + 1*x4 >= 2\r\n" +
                "1*x1 + 1*x2 - 1*x3 + 1*x4 <= 2\r\n" +
                "1*x1 + 1*x2 + 1*x3 - 1*x4 <= 2\r\n" +
                "xj >= 0";

            return new LinearProgrammingProblem(goal, GoalType.Maximize, rows, description);
        }

        /*
         * Тестовий приклад зі зразка Зарі.
         * Він потрібен для перевірки, що програма повторює логіку зразкового звіту.
         *
         * Очікуваний результат:
         * X = (0; 5.3333; 0; 0.3333)
         * Z = 10.6667
         */
        public static LinearProgrammingProblem CreateZariaSample()
        {
            double[] goal = new double[]
            {
                1, 2, 1, 0
            };

            double[,] rows = new double[,]
            {
                { 2, -1, 3,  4, 10 },
                { 1,  1, 1, -1,  5 },
                { 1,  2, 2,  4, 12 }
            };

            string description =
                "Тестовий приклад зі зразка\r\n" +
                "Z = 1*x1 + 2*x2 + 1*x3 + 0*x4 -> max\r\n" +
                "При обмеженнях:\r\n" +
                "2*x1 - 1*x2 + 3*x3 + 4*x4 >= 10\r\n" +
                "1*x1 + 1*x2 + 1*x3 - 1*x4 >= 5\r\n" +
                "1*x1 + 2*x2 + 2*x3 + 4*x4 >= 12\r\n" +
                "xj >= 0";

            return new LinearProgrammingProblem(goal, GoalType.Maximize, rows, description);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(OriginalDescription))
            {
                return OriginalDescription;
            }

            StringBuilder builder = new StringBuilder();

            builder.Append("Z = ");

            for (int i = 0; i < GoalCoefficients.Length; i++)
            {
                if (i > 0)
                {
                    if (GoalCoefficients[i] >= 0)
                    {
                        builder.Append(" + ");
                    }
                    else
                    {
                        builder.Append(" - ");
                    }
                }
                else
                {
                    if (GoalCoefficients[i] < 0)
                    {
                        builder.Append("-");
                    }
                }

                builder.Append(Math.Abs(GoalCoefficients[i]).ToString("0.####"));
                builder.Append("*x");
                builder.Append(i + 1);
            }

            if (GoalType == GoalType.Maximize)
            {
                builder.AppendLine(" -> max");
            }
            else
            {
                builder.AppendLine(" -> min");
            }

            builder.AppendLine("При обмеженнях:");

            for (int i = 0; i < Inequalities.Length; i++)
            {
                builder.AppendLine(Inequalities[i].ToString());
            }

            builder.Append("xj >= 0");

            return builder.ToString();
        }
    }
}