using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class DualPairResult
    {
        public LinearProgrammingProblem PrimalProblem { get; private set; }
        public LinearProgrammingProblem DualProblem { get; private set; }
        public SimplexResult PrimalResult { get; private set; }
        public SimplexResult DualResult { get; private set; }

        public DualPairResult(
            LinearProgrammingProblem primalProblem,
            LinearProgrammingProblem dualProblem,
            SimplexResult primalResult,
            SimplexResult dualResult)
        {
            PrimalProblem = primalProblem;
            DualProblem = dualProblem;
            PrimalResult = primalResult;
            DualResult = dualResult;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Результат розв'язання пари взаємно двоїстих задач");
            builder.AppendLine();

            builder.AppendLine("Пряма задача Z:");
            builder.AppendLine(PrimalResult.ToString());

            builder.AppendLine("Двоїста задача W:");
            builder.AppendLine(DualResult.ToString());

            if (PrimalResult.IsOptimal && DualResult.IsOptimal)
            {
                builder.AppendLine("Порівняння значень цільових функцій:");
                builder.AppendLine("Max(Z) = " + PrimalResult.ObjectiveValue.ToString("0.####"));
                builder.AppendLine("Min(W) = " + DualResult.ObjectiveValue.ToString("0.####"));
                builder.AppendLine("|Max(Z) - Min(W)| = " + Math.Abs(PrimalResult.ObjectiveValue - DualResult.ObjectiveValue).ToString("0.####"));

                if (Math.Abs(PrimalResult.ObjectiveValue - DualResult.ObjectiveValue) <= 0.0001)
                {
                    builder.AppendLine("Значення цільових функцій збігаються. Основна теорема двоїстості виконується.");
                }
                else
                {
                    builder.AppendLine("Значення цільових функцій не збігаються. Потрібна перевірка постановки задач.");
                }
            }
            else if (PrimalResult.IsUnbounded && DualResult.IsInfeasible)
            {
                builder.AppendLine("Пряма задача є необмеженою, а двоїста є несумісною.");
            }
            else if (PrimalResult.IsInfeasible && DualResult.IsUnbounded)
            {
                builder.AppendLine("Пряма задача є несумісною, а двоїста є необмеженою.");
            }

            return builder.ToString();
        }
    }
}