using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class NatureGameResult
    {
        public double[,] UtilityMatrix { get; private set; }
        public double HurwiczAlpha { get; private set; }
        public double[] Probabilities { get; private set; }

        public double[] WaldRowMinimums { get; private set; }
        public int[] WaldStrategies { get; private set; }

        public double[] OptimismRowMaximums { get; private set; }
        public int[] OptimismStrategies { get; private set; }

        public double[] HurwiczValues { get; private set; }
        public int[] HurwiczStrategies { get; private set; }

        public double[,] RiskMatrix { get; private set; }
        public double[] SavageRowMaximumRisks { get; private set; }
        public int[] SavageStrategies { get; private set; }

        public double[] BayesValues { get; private set; }
        public int[] BayesStrategies { get; private set; }

        public double[] LaplaceValues { get; private set; }
        public int[] LaplaceStrategies { get; private set; }

        public NatureGameResult(
            double[,] utilityMatrix,
            double hurwiczAlpha,
            double[] probabilities,
            double[] waldRowMinimums,
            int[] waldStrategies,
            double[] optimismRowMaximums,
            int[] optimismStrategies,
            double[] hurwiczValues,
            int[] hurwiczStrategies,
            double[,] riskMatrix,
            double[] savageRowMaximumRisks,
            int[] savageStrategies,
            double[] bayesValues,
            int[] bayesStrategies,
            double[] laplaceValues,
            int[] laplaceStrategies)
        {
            UtilityMatrix = utilityMatrix;
            HurwiczAlpha = hurwiczAlpha;
            Probabilities = probabilities;

            WaldRowMinimums = waldRowMinimums;
            WaldStrategies = waldStrategies;

            OptimismRowMaximums = optimismRowMaximums;
            OptimismStrategies = optimismStrategies;

            HurwiczValues = hurwiczValues;
            HurwiczStrategies = hurwiczStrategies;

            RiskMatrix = riskMatrix;
            SavageRowMaximumRisks = savageRowMaximumRisks;
            SavageStrategies = savageStrategies;

            BayesValues = bayesValues;
            BayesStrategies = bayesStrategies;

            LaplaceValues = laplaceValues;
            LaplaceStrategies = laplaceStrategies;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Результати розв'язання гри з природою:");
            builder.AppendLine();

            builder.AppendLine("Критерій Вальда: " + FormatStrategies(WaldStrategies));
            builder.AppendLine("Критерій оптимізму: " + FormatStrategies(OptimismStrategies));
            builder.AppendLine("Критерій Гурвіца: " + FormatStrategies(HurwiczStrategies));
            builder.AppendLine("Критерій Севіджа: " + FormatStrategies(SavageStrategies));
            builder.AppendLine("Критерій Байєса: " + FormatStrategies(BayesStrategies));
            builder.AppendLine("Критерій Лапласа: " + FormatStrategies(LaplaceStrategies));

            return builder.ToString();
        }

        private string FormatStrategies(int[] strategies)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < strategies.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append("; ");
                }

                builder.Append("A");
                builder.Append(strategies[i] + 1);
            }

            return builder.ToString();
        }
    }
}