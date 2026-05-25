using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class ModifiedJordanSimplexSolver
    {
        private const double Epsilon = 0.0000001;

        public SimplexResult Solve(LinearProgrammingProblem problem, ReportBuilder report)
        {
            report.AddTitle("Початок розв'язання задачі лінійного програмування");
            report.AddText(problem.ToString());

            SimplexTableau tableau = BuildInitialTableau(problem, report);

            report.AddTitle("Початкова симплекс-таблиця");
            report.AddText(tableau.ToString());

            bool hasArtificialVariables = tableau.ArtificialVariableCount > 0;

            if (hasArtificialVariables)
            {
                report.AddTitle("Етап 1. Пошук опорного розв'язку");
                SolvePhaseOne(tableau, report);

                double artificialSum = CalculateArtificialSum(tableau);

                report.AddText("Сума штучних змінних після першого етапу: " + artificialSum.ToString("0.####"));

                if (artificialSum > Epsilon)
                {
                    return new SimplexResult(
                        false,
                        false,
                        true,
                        new double[problem.VariableCount],
                        0);
                }
            }
            else
            {
                report.AddTitle("Опорний розв'язок уже задано одиничними додатковими змінними");
            }

            report.AddTitle("Етап 2. Пошук оптимального розв'язку");
            PreparePhaseTwoObjective(tableau, problem, report);

            bool unbounded = Optimize(tableau, report);

            if (unbounded)
            {
                return new SimplexResult(
                    false,
                    true,
                    false,
                    new double[problem.VariableCount],
                    0);
            }

            double[] solution = ExtractSolution(tableau, problem.VariableCount);
            double objective = CalculateOriginalObjective(problem, solution);

            report.AddTitle("Фінальна симплекс-таблиця");
            report.AddText(tableau.ToString());

            report.AddTitle("Оптимальний розв'язок");
            for (int i = 0; i < solution.Length; i++)
            {
                report.AddText("x" + (i + 1) + " = " + solution[i].ToString("0.####"));
            }

            report.AddText("F = " + objective.ToString("0.####"));

            return new SimplexResult(
                true,
                false,
                false,
                solution,
                objective);
        }

        private SimplexTableau BuildInitialTableau(LinearProgrammingProblem problem, ReportBuilder report)
        {
            int constraints = problem.ConstraintCount;
            int originalVariables = problem.VariableCount;

            int slackCount = 0;
            int artificialCount = 0;

            for (int i = 0; i < constraints; i++)
            {
                InequalitySign sign = problem.Signs[i];
                double rightSide = problem.RightSides[i];

                if (rightSide < 0)
                {
                    sign = ReverseSign(sign);
                }

                if (sign == InequalitySign.LessOrEqual)
                {
                    slackCount++;
                }
                else if (sign == InequalitySign.GreaterOrEqual)
                {
                    slackCount++;
                    artificialCount++;
                }
                else
                {
                    artificialCount++;
                }
            }

            int totalVariables = originalVariables + slackCount + artificialCount;

            SimplexTableau tableau = new SimplexTableau(
                constraints,
                totalVariables,
                originalVariables,
                artificialCount);

            int slackIndex = originalVariables;
            int artificialIndex = originalVariables + slackCount;

            for (int i = 0; i < constraints; i++)
            {
                double multiplier = 1;
                InequalitySign sign = problem.Signs[i];
                double rightSide = problem.RightSides[i];

                if (rightSide < 0)
                {
                    multiplier = -1;
                    sign = ReverseSign(sign);
                    rightSide = -rightSide;
                }

                for (int j = 0; j < originalVariables; j++)
                {
                    tableau.Data[i, j] = problem.Coefficients[i, j] * multiplier;
                }

                tableau.RightSides[i] = rightSide;

                if (sign == InequalitySign.LessOrEqual)
                {
                    tableau.Data[i, slackIndex] = 1;
                    tableau.Basis[i] = slackIndex;
                    tableau.VariableNames[slackIndex] = "s" + (slackIndex - originalVariables + 1);
                    slackIndex++;
                }
                else if (sign == InequalitySign.GreaterOrEqual)
                {
                    tableau.Data[i, slackIndex] = -1;
                    tableau.VariableNames[slackIndex] = "s" + (slackIndex - originalVariables + 1);
                    slackIndex++;

                    tableau.Data[i, artificialIndex] = 1;
                    tableau.Basis[i] = artificialIndex;
                    tableau.IsArtificial[artificialIndex] = true;
                    tableau.VariableNames[artificialIndex] = "a" + (artificialIndex - originalVariables - slackCount + 1);
                    artificialIndex++;
                }
                else
                {
                    tableau.Data[i, artificialIndex] = 1;
                    tableau.Basis[i] = artificialIndex;
                    tableau.IsArtificial[artificialIndex] = true;
                    tableau.VariableNames[artificialIndex] = "a" + (artificialIndex - originalVariables - slackCount + 1);
                    artificialIndex++;
                }
            }

            for (int j = 0; j < originalVariables; j++)
            {
                tableau.VariableNames[j] = "x" + (j + 1);
            }

            report.AddText("Кількість основних змінних: " + originalVariables);
            report.AddText("Кількість додаткових змінних: " + slackCount);
            report.AddText("Кількість штучних змінних: " + artificialCount);

            return tableau;
        }

        private InequalitySign ReverseSign(InequalitySign sign)
        {
            if (sign == InequalitySign.LessOrEqual)
            {
                return InequalitySign.GreaterOrEqual;
            }

            if (sign == InequalitySign.GreaterOrEqual)
            {
                return InequalitySign.LessOrEqual;
            }

            return InequalitySign.Equal;
        }

        private void SolvePhaseOne(SimplexTableau tableau, ReportBuilder report)
        {
            double[] phaseOneObjective = new double[tableau.VariableCount];

            for (int j = 0; j < tableau.VariableCount; j++)
            {
                if (tableau.IsArtificial[j])
                {
                    phaseOneObjective[j] = -1;
                }
                else
                {
                    phaseOneObjective[j] = 0;
                }
            }

            tableau.SetObjective(phaseOneObjective);

            report.AddText("Функція першого етапу: максимізувати від'ємну суму штучних змінних.");
            report.AddText(tableau.ToString());

            bool unbounded = Optimize(tableau, report);

            if (unbounded)
            {
                throw new Exception("На першому етапі отримано необмеженість, що не повинно виникати для допоміжної задачі.");
            }
        }

        private void PreparePhaseTwoObjective(SimplexTableau tableau, LinearProgrammingProblem problem, ReportBuilder report)
        {
            double[] objective = new double[tableau.VariableCount];

            for (int j = 0; j < problem.VariableCount; j++)
            {
                if (problem.OptimizationType == OptimizationType.Maximize)
                {
                    objective[j] = problem.Objective[j];
                }
                else
                {
                    objective[j] = -problem.Objective[j];
                }
            }

            for (int j = problem.VariableCount; j < tableau.VariableCount; j++)
            {
                objective[j] = 0;
            }

            tableau.SetObjective(objective);

            report.AddText("Симплекс-таблиця для другого етапу:");
            report.AddText(tableau.ToString());
        }

        private bool Optimize(SimplexTableau tableau, ReportBuilder report)
        {
            int iteration = 1;

            while (true)
            {
                int enteringColumn = FindEnteringColumn(tableau);

                if (enteringColumn == -1)
                {
                    report.AddText("У рядку оцінок немає додатних елементів. Поточний план оптимальний.");
                    return false;
                }

                int leavingRow = FindLeavingRow(tableau, enteringColumn);

                if (leavingRow == -1)
                {
                    report.AddText("Для напрямного стовпця немає додатних елементів. Функція не обмежена.");
                    return true;
                }

                report.AddTitle("Ітерація " + iteration);
                report.AddText("Напрямний стовпець: " + tableau.VariableNames[enteringColumn]);
                report.AddText("Напрямний рядок: " + (leavingRow + 1));
                report.AddText("Напрямний елемент: " + tableau.Data[leavingRow, enteringColumn].ToString("0.####"));

                PerformModifiedJordanElimination(tableau, leavingRow, enteringColumn, report);

                report.AddText("Симплекс-таблиця після МЖВ:");
                report.AddText(tableau.ToString());

                iteration++;

                if (iteration > 200)
                {
                    throw new Exception("Перевищено максимальну кількість ітерацій.");
                }
            }
        }

        private int FindEnteringColumn(SimplexTableau tableau)
        {
            int enteringColumn = -1;
            double bestValue = Epsilon;

            for (int j = 0; j < tableau.VariableCount; j++)
            {
                if (tableau.IsArtificial[j])
                {
                    continue;
                }

                if (tableau.ObjectiveRow[j] > bestValue)
                {
                    bestValue = tableau.ObjectiveRow[j];
                    enteringColumn = j;
                }
            }

            return enteringColumn;
        }

        private int FindLeavingRow(SimplexTableau tableau, int enteringColumn)
        {
            int leavingRow = -1;
            double bestRatio = 0;

            for (int i = 0; i < tableau.ConstraintCount; i++)
            {
                double element = tableau.Data[i, enteringColumn];

                if (element > Epsilon)
                {
                    double ratio = tableau.RightSides[i] / element;

                    if (leavingRow == -1 || ratio < bestRatio - Epsilon)
                    {
                        bestRatio = ratio;
                        leavingRow = i;
                    }
                }
            }

            return leavingRow;
        }

        private void PerformModifiedJordanElimination(
            SimplexTableau tableau,
            int pivotRow,
            int pivotColumn,
            ReportBuilder report)
        {
            double pivot = tableau.Data[pivotRow, pivotColumn];

            report.AddText("Виконуємо МЖВ.");
            report.AddText("Новий базисний елемент: " + tableau.VariableNames[pivotColumn]);

            for (int j = 0; j < tableau.VariableCount; j++)
            {
                tableau.Data[pivotRow, j] = tableau.Data[pivotRow, j] / pivot;
            }

            tableau.RightSides[pivotRow] = tableau.RightSides[pivotRow] / pivot;

            for (int i = 0; i < tableau.ConstraintCount; i++)
            {
                if (i == pivotRow)
                {
                    continue;
                }

                double factor = tableau.Data[i, pivotColumn];

                if (Math.Abs(factor) <= Epsilon)
                {
                    continue;
                }

                for (int j = 0; j < tableau.VariableCount; j++)
                {
                    tableau.Data[i, j] = tableau.Data[i, j] - factor * tableau.Data[pivotRow, j];
                }

                tableau.RightSides[i] = tableau.RightSides[i] - factor * tableau.RightSides[pivotRow];
            }

            double objectiveFactor = tableau.ObjectiveRow[pivotColumn];

            for (int j = 0; j < tableau.VariableCount; j++)
            {
                tableau.ObjectiveRow[j] = tableau.ObjectiveRow[j] - objectiveFactor * tableau.Data[pivotRow, j];
            }

            tableau.ObjectiveValue = tableau.ObjectiveValue + objectiveFactor * tableau.RightSides[pivotRow];

            tableau.Basis[pivotRow] = pivotColumn;
        }

        private double CalculateArtificialSum(SimplexTableau tableau)
        {
            double sum = 0;

            for (int i = 0; i < tableau.ConstraintCount; i++)
            {
                int variable = tableau.Basis[i];

                if (tableau.IsArtificial[variable])
                {
                    sum += Math.Abs(tableau.RightSides[i]);
                }
            }

            return sum;
        }

        private double[] ExtractSolution(SimplexTableau tableau, int originalVariableCount)
        {
            double[] solution = new double[originalVariableCount];

            for (int i = 0; i < tableau.ConstraintCount; i++)
            {
                int variable = tableau.Basis[i];

                if (variable >= 0 && variable < originalVariableCount)
                {
                    solution[variable] = tableau.RightSides[i];
                }
            }

            for (int i = 0; i < solution.Length; i++)
            {
                if (Math.Abs(solution[i]) <= Epsilon)
                {
                    solution[i] = 0;
                }
            }

            return solution;
        }

        private double CalculateOriginalObjective(LinearProgrammingProblem problem, double[] solution)
        {
            double value = 0;

            for (int i = 0; i < solution.Length; i++)
            {
                value += problem.Objective[i] * solution[i];
            }

            return value;
        }
    }
}