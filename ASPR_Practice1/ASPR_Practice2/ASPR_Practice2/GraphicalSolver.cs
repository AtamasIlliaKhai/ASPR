using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class GraphicalSolver
    {
        private const double Epsilon = 0.000001;

        public GraphicalResult Solve(GraphicalProblem problem, ReportBuilder report)
        {
            report.AddTitle("Графічний метод розв'язання задачі лінійного програмування");
            report.AddText(problem.ToString());

            GraphicalConstraint[] allConstraints = BuildFullConstraintList(problem);

            GraphicalPoint[] vertices = FindFeasibleVertices(allConstraints, report);

            report.AddTitle("Вершини допустимої області");
            if (vertices.Length == 0)
            {
                report.AddText("Допустимих вершин не знайдено.");
            }
            else
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    double value = problem.CalculateObjective(vertices[i].X1, vertices[i].X2);
                    report.AddText(vertices[i].ToString() + ", F = " + value.ToString("0.####"));
                }
            }

            bool unbounded = IsObjectiveUnbounded(problem, allConstraints, report);

            if (unbounded)
            {
                return new GraphicalResult(
                    false,
                    true,
                    false,
                    null,
                    0,
                    vertices);
            }

            if (vertices.Length == 0)
            {
                return new GraphicalResult(
                    false,
                    false,
                    true,
                    null,
                    0,
                    vertices);
            }

            GraphicalPoint bestPoint = vertices[0];
            double bestValue = problem.CalculateObjective(bestPoint.X1, bestPoint.X2);

            for (int i = 1; i < vertices.Length; i++)
            {
                double value = problem.CalculateObjective(vertices[i].X1, vertices[i].X2);

                if (problem.OptimizationType == OptimizationType.Maximize)
                {
                    if (value > bestValue)
                    {
                        bestValue = value;
                        bestPoint = vertices[i];
                    }
                }
                else
                {
                    if (value < bestValue)
                    {
                        bestValue = value;
                        bestPoint = vertices[i];
                    }
                }
            }

            report.AddTitle("Вибір оптимальної вершини");
            report.AddText("Оптимальна точка: " + bestPoint.ToString());
            report.AddText("Значення функції: " + bestValue.ToString("0.####"));

            return new GraphicalResult(
                true,
                false,
                false,
                bestPoint,
                bestValue,
                vertices);
        }

        private GraphicalConstraint[] BuildFullConstraintList(GraphicalProblem problem)
        {
            int extra = 0;

            if (problem.X1Nonnegative)
            {
                extra++;
            }

            if (problem.X2Nonnegative)
            {
                extra++;
            }

            GraphicalConstraint[] result = new GraphicalConstraint[problem.Constraints.Length + extra];

            int index = 0;

            for (int i = 0; i < problem.Constraints.Length; i++)
            {
                result[index] = problem.Constraints[i];
                index++;
            }

            if (problem.X1Nonnegative)
            {
                result[index] = new GraphicalConstraint(1, 0, InequalitySign.GreaterOrEqual, 0, "x1 >= 0");
                index++;
            }

            if (problem.X2Nonnegative)
            {
                result[index] = new GraphicalConstraint(0, 1, InequalitySign.GreaterOrEqual, 0, "x2 >= 0");
                index++;
            }

            return result;
        }

        private GraphicalPoint[] FindFeasibleVertices(GraphicalConstraint[] constraints, ReportBuilder report)
        {
            GraphicalPoint[] temp = new GraphicalPoint[constraints.Length * constraints.Length + 10];
            int count = 0;

            for (int i = 0; i < constraints.Length; i++)
            {
                for (int j = i + 1; j < constraints.Length; j++)
                {
                    GraphicalPoint point = Intersect(constraints[i], constraints[j]);

                    if (point == null)
                    {
                        continue;
                    }

                    if (IsFeasible(point, constraints))
                    {
                        if (!ContainsPoint(temp, count, point))
                        {
                            temp[count] = point;
                            count++;

                            report.AddText("Точка перетину " + constraints[i].Name + " і " + constraints[j].Name + " є допустимою: " + point.ToString());
                        }
                    }
                    else
                    {
                        report.AddText("Точка перетину " + constraints[i].Name + " і " + constraints[j].Name + " не належить допустимій області: " + point.ToString());
                    }
                }
            }

            GraphicalPoint[] result = new GraphicalPoint[count];

            for (int i = 0; i < count; i++)
            {
                result[i] = temp[i];
            }

            return result;
        }

        private GraphicalPoint Intersect(GraphicalConstraint c1, GraphicalConstraint c2)
        {
            double determinant = c1.A1 * c2.A2 - c2.A1 * c1.A2;

            if (Math.Abs(determinant) <= Epsilon)
            {
                return null;
            }

            double x1 = (c1.B * c2.A2 - c2.B * c1.A2) / determinant;
            double x2 = (c1.A1 * c2.B - c2.A1 * c1.B) / determinant;

            return new GraphicalPoint(x1, x2);
        }

        private bool IsFeasible(GraphicalPoint point, GraphicalConstraint[] constraints)
        {
            for (int i = 0; i < constraints.Length; i++)
            {
                if (!constraints[i].IsSatisfied(point.X1, point.X2))
                {
                    return false;
                }
            }

            return true;
        }

        private bool ContainsPoint(GraphicalPoint[] points, int count, GraphicalPoint point)
        {
            for (int i = 0; i < count; i++)
            {
                if (Math.Abs(points[i].X1 - point.X1) <= Epsilon &&
                    Math.Abs(points[i].X2 - point.X2) <= Epsilon)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsObjectiveUnbounded(GraphicalProblem problem, GraphicalConstraint[] constraints, ReportBuilder report)
        {
            double[][] directions = BuildCandidateDirections(constraints);

            for (int i = 0; i < directions.Length; i++)
            {
                double dx1 = directions[i][0];
                double dx2 = directions[i][1];

                if (Math.Abs(dx1) <= Epsilon && Math.Abs(dx2) <= Epsilon)
                {
                    continue;
                }

                if (IsFeasibleRay(dx1, dx2, constraints))
                {
                    double objectiveDirection = problem.C1 * dx1 + problem.C2 * dx2;

                    if (problem.OptimizationType == OptimizationType.Maximize && objectiveDirection > Epsilon)
                    {
                        report.AddTitle("Перевірка необмеженості");
                        report.AddText("Знайдено допустимий напрям (" + dx1.ToString("0.####") + "; " + dx2.ToString("0.####") + "), у якому функція мети зростає.");
                        return true;
                    }

                    if (problem.OptimizationType == OptimizationType.Minimize && objectiveDirection < -Epsilon)
                    {
                        report.AddTitle("Перевірка необмеженості");
                        report.AddText("Знайдено допустимий напрям (" + dx1.ToString("0.####") + "; " + dx2.ToString("0.####") + "), у якому функція мети спадає.");
                        return true;
                    }
                }
            }

            return false;
        }

        private double[][] BuildCandidateDirections(GraphicalConstraint[] constraints)
        {
            double[][] temp = new double[constraints.Length * 2 + 4][];
            int count = 0;

            temp[count] = new double[] { 1, 0 };
            count++;

            temp[count] = new double[] { 0, 1 };
            count++;

            temp[count] = new double[] { -1, 0 };
            count++;

            temp[count] = new double[] { 0, -1 };
            count++;

            for (int i = 0; i < constraints.Length; i++)
            {
                double dx1 = constraints[i].A2;
                double dx2 = -constraints[i].A1;

                temp[count] = new double[] { dx1, dx2 };
                count++;

                temp[count] = new double[] { -dx1, -dx2 };
                count++;
            }

            double[][] result = new double[count][];

            for (int i = 0; i < count; i++)
            {
                result[i] = temp[i];
            }

            return result;
        }

        private bool IsFeasibleRay(double dx1, double dx2, GraphicalConstraint[] constraints)
        {
            for (int i = 0; i < constraints.Length; i++)
            {
                double value = constraints[i].A1 * dx1 + constraints[i].A2 * dx2;

                if (constraints[i].Sign == InequalitySign.LessOrEqual)
                {
                    if (value > Epsilon)
                    {
                        return false;
                    }
                }
                else if (constraints[i].Sign == InequalitySign.GreaterOrEqual)
                {
                    if (value < -Epsilon)
                    {
                        return false;
                    }
                }
                else
                {
                    if (Math.Abs(value) > Epsilon)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}