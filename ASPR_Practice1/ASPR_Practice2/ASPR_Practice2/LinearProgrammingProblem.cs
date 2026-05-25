using System;
using System.Text;

namespace ASPR_Practice1
{
    internal enum InequalitySign
    {
        LessOrEqual,
        GreaterOrEqual,
        Equal
    }

    internal enum OptimizationType
    {
        Maximize,
        Minimize
    }

    internal class LinearProgrammingProblem
    {
        public double[,] Coefficients { get; private set; }
        public InequalitySign[] Signs { get; private set; }
        public double[] RightSides { get; private set; }
        public double[] Objective { get; private set; }
        public OptimizationType OptimizationType { get; private set; }
        public string Description { get; private set; }

        public int ConstraintCount
        {
            get { return RightSides.Length; }
        }

        public int VariableCount
        {
            get { return Objective.Length; }
        }

        public LinearProgrammingProblem(
            double[,] coefficients,
            InequalitySign[] signs,
            double[] rightSides,
            double[] objective,
            OptimizationType optimizationType,
            string description)
        {
            Coefficients = coefficients;
            Signs = signs;
            RightSides = rightSides;
            Objective = objective;
            OptimizationType = optimizationType;
            Description = description;
        }

        public static LinearProgrammingProblem CreateVariant3Practice2()
        {
            double[,] coefficients = new double[,]
            {
                { 1,  1, -1, -2 },
                { 1,  1,  1, -1 },
                { 2, -1,  3,  4 }
            };

            InequalitySign[] signs = new InequalitySign[]
            {
                InequalitySign.LessOrEqual,
                InequalitySign.GreaterOrEqual,
                InequalitySign.LessOrEqual
            };

            double[] rightSides = new double[]
            {
                6,
                5,
                10
            };

            double[] objective = new double[]
            {
                1, 2, -1, -1
            };

            return new LinearProgrammingProblem(
                coefficients,
                signs,
                rightSides,
                objective,
                OptimizationType.Maximize,
                "Практична робота 2. Варіант 3. У другій нерівності знак змінено на протилежний.");
        }

        public static LinearProgrammingProblem CreateTestProblem1()
        {
            double[,] coefficients = new double[,]
            {
                { 1, 1 },
                { 1, 0 },
                { 0, 1 }
            };

            InequalitySign[] signs = new InequalitySign[]
            {
                InequalitySign.LessOrEqual,
                InequalitySign.LessOrEqual,
                InequalitySign.LessOrEqual
            };

            double[] rightSides = new double[]
            {
                4,
                2,
                3
            };

            double[] objective = new double[]
            {
                3, 2
            };

            return new LinearProgrammingProblem(
                coefficients,
                signs,
                rightSides,
                objective,
                OptimizationType.Maximize,
                "Тестовий приклад 1. Очікуваний результат: x1 = 2, x2 = 2, Fmax = 10.");
        }

        public static LinearProgrammingProblem CreateTestProblem2()
        {
            double[,] coefficients = new double[,]
            {
                { 1, 2 },
                { 2, 1 },
                { 1, 0 },
                { 0, 1 }
            };

            InequalitySign[] signs = new InequalitySign[]
            {
                InequalitySign.GreaterOrEqual,
                InequalitySign.GreaterOrEqual,
                InequalitySign.LessOrEqual,
                InequalitySign.LessOrEqual
            };

            double[] rightSides = new double[]
            {
                4,
                4,
                4,
                4
            };

            double[] objective = new double[]
            {
                1, 1
            };

            return new LinearProgrammingProblem(
                coefficients,
                signs,
                rightSides,
                objective,
                OptimizationType.Maximize,
                "Тестовий приклад 2. Задача з обмеженнями типу >=.");
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(Description);
            builder.AppendLine();

            builder.Append("F = ");

            for (int j = 0; j < VariableCount; j++)
            {
                if (j > 0)
                {
                    if (Objective[j] >= 0)
                    {
                        builder.Append(" + ");
                    }
                    else
                    {
                        builder.Append(" - ");
                    }

                    builder.Append(Math.Abs(Objective[j]).ToString("0.####"));
                }
                else
                {
                    builder.Append(Objective[j].ToString("0.####"));
                }

                builder.Append("x");
                builder.Append(j + 1);
            }

            if (OptimizationType == OptimizationType.Maximize)
            {
                builder.AppendLine(" -> max");
            }
            else
            {
                builder.AppendLine(" -> min");
            }

            builder.AppendLine();

            for (int i = 0; i < ConstraintCount; i++)
            {
                for (int j = 0; j < VariableCount; j++)
                {
                    if (j > 0)
                    {
                        if (Coefficients[i, j] >= 0)
                        {
                            builder.Append(" + ");
                        }
                        else
                        {
                            builder.Append(" - ");
                        }

                        builder.Append(Math.Abs(Coefficients[i, j]).ToString("0.####"));
                    }
                    else
                    {
                        builder.Append(Coefficients[i, j].ToString("0.####"));
                    }

                    builder.Append("x");
                    builder.Append(j + 1);
                }

                builder.Append(" ");

                if (Signs[i] == InequalitySign.LessOrEqual)
                {
                    builder.Append("<= ");
                }
                else if (Signs[i] == InequalitySign.GreaterOrEqual)
                {
                    builder.Append(">= ");
                }
                else
                {
                    builder.Append("= ");
                }

                builder.AppendLine(RightSides[i].ToString("0.####"));
            }

            builder.AppendLine("xj >= 0");

            return builder.ToString();
        }
    }
}