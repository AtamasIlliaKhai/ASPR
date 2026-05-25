using ASPR_Practice1;

namespace ASPR_Practice1
{
    internal class DualPairSolver
    {
        public DualPairResult SolvePair(LinearProgrammingProblem primal, ReportBuilder report)
        {
            report.AddTitle("Побудова пари взаємно двоїстих задач");

            LinearProgrammingProblem dual = DualProblemBuilder.BuildDualForMaxLessOrEqual(primal);

            report.AddText(DualProblemBuilder.BuildDualExplanation(primal, dual));

            ModifiedJordanSimplexSolver solver = new ModifiedJordanSimplexSolver();

            report.AddTitle("Розв'язання прямої задачі Z");
            SimplexResult primalResult = solver.Solve(primal, report);

            report.AddTitle("Розв'язання двоїстої задачі W");
            SimplexResult dualResult = solver.Solve(dual, report);

            report.AddTitle("Порівняння результатів");
            if (primalResult.IsOptimal && dualResult.IsOptimal)
            {
                report.AddText("Max(Z) = " + primalResult.ObjectiveValue.ToString("0.####"));
                report.AddText("Min(W) = " + dualResult.ObjectiveValue.ToString("0.####"));
                report.AddText("Різниця = " + System.Math.Abs(primalResult.ObjectiveValue - dualResult.ObjectiveValue).ToString("0.####"));
            }

            return new DualPairResult(primal, dual, primalResult, dualResult);
        }
    }
}