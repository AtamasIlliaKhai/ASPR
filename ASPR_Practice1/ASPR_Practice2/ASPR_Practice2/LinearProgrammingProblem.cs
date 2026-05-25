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

        public static LinearProgrammingProblem CreatePractice1BVariant3Primal()
        {
            double[,] coefficients = new double[,]
            {
                { -1,  1,  1,  1 },
                {  1, -1,  1,  1 },
                {  1,  1, -1,  1 },
                {  1,  1,  1, -1 }
            };

            InequalitySign[] signs = new InequalitySign[]
            {
                InequalitySign.LessOrEqual,
                InequalitySign.LessOrEqual,
                InequalitySign.LessOrEqual,
                InequalitySign.LessOrEqual
            };

            double[] rightSides = new double[]
            {
                2, 2, 2, 2
            };

            double[] objective = new double[]
            {
                3, 1, 1, -1
            };

            return new LinearProgrammingProblem(
                coefficients,
                signs,
                rightSides,
                objective,
                OptimizationType.Maximize,
                "Пряма задача Z. Практична робота 1B, варіант 3.");
        }

        public static LinearProgrammingProblem CreateLectureDualExamplePrimal()
        {
            double[,] coefficients = new double[,]
            {
                { 1, 1, -1, -2 },
                { 1, 1,  1, -1 },
                { 2,-1,  3,  4 }
            };

            InequalitySign[] signs = new InequalitySign[]
            {
                InequalitySign.LessOrEqual,
                InequalitySign.GreaterOrEqual,
                InequalitySign.LessOrEqual
            };

            double[] rightSides = new double[]
            {
                6, 5, 10
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
                "Лекційний приклад пари двоїстих задач.");
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(Description);
            builder.AppendLine();

            builder.Append("F = ");

            for (int j = 0; j < VariableCount; j++)
            {
                AppendTerm(builder, Objective[j], "x" + (j + 1), j == 0);
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
                    AppendTerm(builder, Coefficients[i, j], "x" + (j + 1), j == 0);
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

        private void AppendTerm(StringBuilder builder, double coefficient, string variableName, bool first)
        {
            if (first)
            {
                builder.Append(coefficient.ToString("0.####"));
                builder.Append(variableName);
                return;
            }

            if (coefficient >= 0)
            {
                builder.Append(" + ");
                builder.Append(coefficient.ToString("0.####"));
            }
            else
            {
                builder.Append(" - ");
                builder.Append(Math.Abs(coefficient).ToString("0.####"));
            }

            builder.Append(variableName);
        }
    }
}