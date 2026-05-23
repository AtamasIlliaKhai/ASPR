using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class Inequality
    {
        public double[] Coefficients { get; private set; }
        public InequalitySign Sign { get; private set; }
        public double RightPart { get; private set; }

        public Inequality(double[] coefficients, InequalitySign sign, double rightPart)
        {
            Coefficients = coefficients;
            Sign = sign;
            RightPart = rightPart;
        }

        public double[] ToSimplexRow()
        {
            double[] row = new double[Coefficients.Length + 1];

            for (int i = 0; i < Coefficients.Length; i++)
            {
                row[i] = Coefficients[i];
            }

            row[Coefficients.Length] = RightPart;

            return row;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < Coefficients.Length; i++)
            {
                if (i > 0)
                {
                    if (Coefficients[i] >= 0)
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
                    if (Coefficients[i] < 0)
                    {
                        builder.Append("-");
                    }
                }

                builder.Append(Math.Abs(Coefficients[i]).ToString("0.####"));
                builder.Append("*x");
                builder.Append(i + 1);
            }

            if (Sign == InequalitySign.LessOrEqual)
            {
                builder.Append(" <= ");
            }
            else if (Sign == InequalitySign.GreaterOrEqual)
            {
                builder.Append(" >= ");
            }
            else
            {
                builder.Append(" = ");
            }

            builder.Append(RightPart.ToString("0.####"));

            return builder.ToString();
        }
    }
}