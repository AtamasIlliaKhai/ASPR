using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class SimplexTableau
    {
        public double[,] Data;
        public double[] RightSides;
        public double[] ObjectiveRow;
        public double ObjectiveValue;
        public int[] Basis;
        public string[] VariableNames;
        public bool[] IsArtificial;

        public int ConstraintCount { get; private set; }
        public int VariableCount { get; private set; }
        public int OriginalVariableCount { get; private set; }
        public int ArtificialVariableCount { get; private set; }

        public SimplexTableau(
            int constraintCount,
            int variableCount,
            int originalVariableCount,
            int artificialVariableCount)
        {
            ConstraintCount = constraintCount;
            VariableCount = variableCount;
            OriginalVariableCount = originalVariableCount;
            ArtificialVariableCount = artificialVariableCount;

            Data = new double[constraintCount, variableCount];
            RightSides = new double[constraintCount];
            ObjectiveRow = new double[variableCount];
            Basis = new int[constraintCount];
            VariableNames = new string[variableCount];
            IsArtificial = new bool[variableCount];
            ObjectiveValue = 0;
        }

        public void SetObjective(double[] coefficients)
        {
            for (int j = 0; j < VariableCount; j++)
            {
                ObjectiveRow[j] = coefficients[j];
            }

            ObjectiveValue = 0;

            for (int i = 0; i < ConstraintCount; i++)
            {
                int basicVariable = Basis[i];
                double basicCost = coefficients[basicVariable];

                if (Math.Abs(basicCost) <= 0.0000001)
                {
                    continue;
                }

                for (int j = 0; j < VariableCount; j++)
                {
                    ObjectiveRow[j] = ObjectiveRow[j] - basicCost * Data[i, j];
                }

                ObjectiveValue = ObjectiveValue + basicCost * RightSides[i];
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("Базис".PadRight(10));

            for (int j = 0; j < VariableCount; j++)
            {
                builder.Append(VariableNames[j].PadLeft(10));
            }

            builder.Append("Вільний член".PadLeft(16));
            builder.AppendLine();

            for (int i = 0; i < ConstraintCount; i++)
            {
                builder.Append(VariableNames[Basis[i]].PadRight(10));

                for (int j = 0; j < VariableCount; j++)
                {
                    builder.Append(FormatNumber(Data[i, j]).PadLeft(10));
                }

                builder.Append(FormatNumber(RightSides[i]).PadLeft(16));
                builder.AppendLine();
            }

            builder.Append("Оцінки".PadRight(10));

            for (int j = 0; j < VariableCount; j++)
            {
                builder.Append(FormatNumber(ObjectiveRow[j]).PadLeft(10));
            }

            builder.Append(FormatNumber(ObjectiveValue).PadLeft(16));
            builder.AppendLine();

            return builder.ToString();
        }

        private string FormatNumber(double value)
        {
            if (Math.Abs(value) <= 0.0000001)
            {
                value = 0;
            }

            return value.ToString("0.####");
        }
    }
}