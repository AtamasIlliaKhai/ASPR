using System.Text;

namespace ASPR_Practice1
{
    internal class SimplexResult
    {
        public double[] X { get; private set; }
        public double Z { get; private set; }
        public bool IsOptimal { get; private set; }
        public bool IsUnbounded { get; private set; }
        public bool IsInconsistent { get; private set; }

        public SimplexResult(double[] x, double z, bool isOptimal, bool isUnbounded, bool isInconsistent)
        {
            X = x;
            Z = z;
            IsOptimal = isOptimal;
            IsUnbounded = isUnbounded;
            IsInconsistent = isInconsistent;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            if (IsInconsistent)
            {
                return "Система обмежень є суперечливою.";
            }

            if (IsUnbounded)
            {
                return "Функція не обмежена зверху.";
            }

            builder.Append("X = (");

            for (int i = 0; i < X.Length; i++)
            {
                builder.Append(System.Math.Round(X[i], 4).ToString("0.####"));

                if (i < X.Length - 1)
                {
                    builder.Append("; ");
                }
            }

            builder.AppendLine(")");

            if (IsOptimal)
            {
                builder.AppendLine("Maximize(Z) = " + System.Math.Round(Z, 4).ToString("0.####"));
                builder.AppendLine("Розв'язок є оптимальним.");
            }
            else
            {
                builder.AppendLine("Z = " + System.Math.Round(Z, 4).ToString("0.####"));
                builder.AppendLine("Знайдено опорний розв'язок.");
            }

            return builder.ToString();
        }
    }
}