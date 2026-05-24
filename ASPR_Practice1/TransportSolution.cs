using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class TransportSolution
    {
        public string MethodName { get; private set; }
        public TransportInput Input { get; private set; }
        public double[,] Plan { get; private set; }
        public bool[,] Basis { get; private set; }
        public double TotalCost { get; private set; }
        public double[] PotentialsU { get; private set; }
        public double[] PotentialsV { get; private set; }
        public double[,] Estimates { get; private set; }
        public bool IsOptimal { get; private set; }

        public TransportSolution(
            string methodName,
            TransportInput input,
            double[,] plan,
            bool[,] basis,
            double totalCost,
            double[] potentialsU,
            double[] potentialsV,
            double[,] estimates,
            bool isOptimal)
        {
            MethodName = methodName;
            Input = input;
            Plan = plan;
            Basis = basis;
            TotalCost = totalCost;
            PotentialsU = potentialsU;
            PotentialsV = potentialsV;
            Estimates = estimates;
            IsOptimal = isOptimal;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Метод: " + MethodName);
            builder.AppendLine("Транспортний план:");
            builder.AppendLine(PlanToString(Plan));
            builder.AppendLine("Загальна вартість перевезень: " + TotalCost.ToString("0.####"));

            if (PotentialsU != null && PotentialsV != null)
            {
                builder.AppendLine("Потенціали постачальників:");
                builder.AppendLine(VectorToString(PotentialsU, "u"));

                builder.AppendLine("Потенціали споживачів:");
                builder.AppendLine(VectorToString(PotentialsV, "v"));
            }

            if (Estimates != null)
            {
                builder.AppendLine("Оцінки небазисних клітинок:");
                builder.AppendLine(PlanToString(Estimates));
            }

            if (IsOptimal)
            {
                builder.AppendLine("План є оптимальним.");
            }
            else
            {
                builder.AppendLine("План не є оптимальним або оптимальність не перевірялася.");
            }

            return builder.ToString();
        }

        private string PlanToString(double[,] matrix)
        {
            StringBuilder builder = new StringBuilder();

            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            builder.Append("          ");

            for (int j = 0; j < columns; j++)
            {
                builder.Append(("D" + (j + 1)).PadLeft(10));
            }

            builder.AppendLine();

            for (int i = 0; i < rows; i++)
            {
                builder.Append(("S" + (i + 1)).PadLeft(10));

                for (int j = 0; j < columns; j++)
                {
                    builder.Append(matrix[i, j].ToString("0.####").PadLeft(10));
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private string VectorToString(double[] vector, string prefix)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < vector.Length; i++)
            {
                builder.Append(prefix);
                builder.Append(i + 1);
                builder.Append(" = ");
                builder.Append(vector[i].ToString("0.####"));
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}