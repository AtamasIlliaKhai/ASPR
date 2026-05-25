using System.Text;

namespace ASPR_Practice1
{
    internal class GraphicalProblem
    {
        public double C1 { get; private set; }
        public double C2 { get; private set; }
        public OptimizationType OptimizationType { get; private set; }
        public GraphicalConstraint[] Constraints { get; private set; }
        public string Description { get; private set; }
        public bool X1Nonnegative { get; private set; }
        public bool X2Nonnegative { get; private set; }

        public GraphicalProblem(
            double c1,
            double c2,
            OptimizationType optimizationType,
            GraphicalConstraint[] constraints,
            bool x1Nonnegative,
            bool x2Nonnegative,
            string description)
        {
            C1 = c1;
            C2 = c2;
            OptimizationType = optimizationType;
            Constraints = constraints;
            X1Nonnegative = x1Nonnegative;
            X2Nonnegative = x2Nonnegative;
            Description = description;
        }

        public static GraphicalProblem CreatePractice2Variant3Primal()
        {
            GraphicalConstraint[] constraints = new GraphicalConstraint[]
            {
                new GraphicalConstraint(2, -1, InequalitySign.Equal, 1, "2x1 - x2 = 1"),
                new GraphicalConstraint(1, -3, InequalitySign.LessOrEqual, -2, "x1 - 3x2 <= -2")
            };

            return new GraphicalProblem(
                2,
                1,
                OptimizationType.Maximize,
                constraints,
                true,
                true,
                "Графічна пряма задача Z. Практична робота 2, варіант 3.");
        }

        public static GraphicalProblem CreatePractice2Variant3Dual()
        {
            GraphicalConstraint[] constraints = new GraphicalConstraint[]
            {
                new GraphicalConstraint(2, 1, InequalitySign.GreaterOrEqual, 2, "2u1 + u2 >= 2"),
                new GraphicalConstraint(-1, -3, InequalitySign.GreaterOrEqual, 1, "-u1 - 3u2 >= 1")
            };

            return new GraphicalProblem(
                1,
                -2,
                OptimizationType.Minimize,
                constraints,
                false,
                true,
                "Графічна двоїста задача W для варіанту 3. u1 є вільною змінною, u2 >= 0.");
        }

        public double CalculateObjective(double x1, double x2)
        {
            return C1 * x1 + C2 * x2;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(Description);
            builder.AppendLine();

            builder.Append("F = ");
            builder.Append(C1.ToString("0.####"));
            builder.Append("x1");

            if (C2 >= 0)
            {
                builder.Append(" + ");
                builder.Append(C2.ToString("0.####"));
            }
            else
            {
                builder.Append(" - ");
                builder.Append((-C2).ToString("0.####"));
            }

            builder.Append("x2");

            if (OptimizationType == OptimizationType.Maximize)
            {
                builder.AppendLine(" -> max");
            }
            else
            {
                builder.AppendLine(" -> min");
            }

            builder.AppendLine();

            for (int i = 0; i < Constraints.Length; i++)
            {
                builder.AppendLine(Constraints[i].ToString());
            }

            if (X1Nonnegative)
            {
                builder.AppendLine("x1 >= 0");
            }
            else
            {
                builder.AppendLine("x1 - вільна змінна");
            }

            if (X2Nonnegative)
            {
                builder.AppendLine("x2 >= 0");
            }
            else
            {
                builder.AppendLine("x2 - вільна змінна");
            }

            return builder.ToString();
        }
    }
}