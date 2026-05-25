using System;

namespace ASPR_Practice1
{
    internal class GraphicalConstraint
    {
        public double A1 { get; private set; }
        public double A2 { get; private set; }
        public InequalitySign Sign { get; private set; }
        public double B { get; private set; }
        public string Name { get; private set; }

        public GraphicalConstraint(double a1, double a2, InequalitySign sign, double b, string name)
        {
            A1 = a1;
            A2 = a2;
            Sign = sign;
            B = b;
            Name = name;
        }

        public bool IsSatisfied(double x1, double x2)
        {
            double left = A1 * x1 + A2 * x2;

            if (Sign == InequalitySign.LessOrEqual)
            {
                return left <= B + 0.000001;
            }

            if (Sign == InequalitySign.GreaterOrEqual)
            {
                return left >= B - 0.000001;
            }

            return Math.Abs(left - B) <= 0.000001;
        }

        public override string ToString()
        {
            string signText = "=";

            if (Sign == InequalitySign.LessOrEqual)
            {
                signText = "<=";
            }
            else if (Sign == InequalitySign.GreaterOrEqual)
            {
                signText = ">=";
            }

            return A1.ToString("0.####") + "x1 + " + A2.ToString("0.####") + "x2 " + signText + " " + B.ToString("0.####");
        }
    }
}