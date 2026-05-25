using System.Text;

namespace ASPR_Practice1
{
    internal static class DualProblemBuilder
    {
        public static LinearProgrammingProblem BuildDualForMaxLessOrEqual(LinearProgrammingProblem primal)
        {
            int primalConstraints = primal.ConstraintCount;
            int primalVariables = primal.VariableCount;

            double[,] dualCoefficients = new double[primalVariables, primalConstraints];

            for (int i = 0; i < primalVariables; i++)
            {
                for (int j = 0; j < primalConstraints; j++)
                {
                    dualCoefficients[i, j] = primal.Coefficients[j, i];
                }
            }

            InequalitySign[] dualSigns = new InequalitySign[primalVariables];

            for (int i = 0; i < primalVariables; i++)
            {
                dualSigns[i] = InequalitySign.GreaterOrEqual;
            }

            double[] dualRightSides = new double[primalVariables];

            for (int i = 0; i < primalVariables; i++)
            {
                dualRightSides[i] = primal.Objective[i];
            }

            double[] dualObjective = new double[primalConstraints];

            for (int i = 0; i < primalConstraints; i++)
            {
                dualObjective[i] = primal.RightSides[i];
            }

            return new LinearProgrammingProblem(
                dualCoefficients,
                dualSigns,
                dualRightSides,
                dualObjective,
                OptimizationType.Minimize,
                "Двоїста задача W, побудована для прямої задачі Z.");
        }

        public static string BuildDualExplanation(LinearProgrammingProblem primal, LinearProgrammingProblem dual)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Побудова двоїстої задачі:");
            builder.AppendLine("Пряма задача Z є задачею на максимум.");
            builder.AppendLine("Обмеження прямої задачі мають вигляд <=, а змінні xj є невід'ємними.");
            builder.AppendLine("Тому двоїста задача W є задачею на мінімум.");
            builder.AppendLine("Коефіцієнти правих частин прямої задачі стають коефіцієнтами цільової функції W.");
            builder.AppendLine("Коефіцієнти цільової функції Z стають правими частинами обмежень W.");
            builder.AppendLine("Матриця обмежень двоїстої задачі є транспонованою матрицею обмежень прямої задачі.");
            builder.AppendLine();

            builder.AppendLine("Пряма задача:");
            builder.AppendLine(primal.ToString());

            builder.AppendLine("Двоїста задача:");
            builder.AppendLine(dual.ToString());

            return builder.ToString();
        }
    }
}