using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class TransportInput
    {
        public double[,] Costs { get; private set; }
        public double[] Supplies { get; private set; }
        public double[] Demands { get; private set; }
        public string Description { get; private set; }

        public int SupplierCount
        {
            get { return Supplies.Length; }
        }

        public int ConsumerCount
        {
            get { return Demands.Length; }
        }

        public TransportInput(double[,] costs, double[] supplies, double[] demands, string description)
        {
            Costs = costs;
            Supplies = supplies;
            Demands = demands;
            Description = description;
        }

        public double TotalSupply()
        {
            double sum = 0;

            for (int i = 0; i < Supplies.Length; i++)
            {
                sum += Supplies[i];
            }

            return sum;
        }

        public double TotalDemand()
        {
            double sum = 0;

            for (int j = 0; j < Demands.Length; j++)
            {
                sum += Demands[j];
            }

            return sum;
        }

        public bool IsClosed()
        {
            return Math.Abs(TotalSupply() - TotalDemand()) < 0.0000001;
        }

        public static TransportInput CreateTestProblem()
        {
            double[,] costs = new double[,]
            {
                { 10, 9, 7, 10 },
                {  5, 8, 6, 11 },
                { 11, 9, 7,  9 }
            };

            double[] supplies = new double[]
            {
                40, 45, 25
            };

            double[] demands = new double[]
            {
                25, 10, 35, 40
            };

            string description =
                "Тестова транспортна задача\r\n" +
                "Матриця вартостей C:\r\n" +
                "10  9  7 10\r\n" +
                " 5  8  6 11\r\n" +
                "11  9  7  9\r\n" +
                "Запаси постачальників: 40; 45; 25\r\n" +
                "Потреби споживачів: 25; 10; 35; 40";

            return new TransportInput(costs, supplies, demands, description);
        }

        public static TransportInput CreateVariant3()
        {
            double[,] costs = new double[,]
            {
                { 10, 9, 7, 10 },
                {  5, 8, 6, 11 },
                { 11, 9, 7,  9 }
            };

            double[] supplies = new double[]
            {
                40, 45, 25
            };

            double[] demands = new double[]
            {
                25, 10, 35, 40
            };

            string description =
                "Практична робота 5. Варіант 3\r\n" +
                "Матриця вартостей C:\r\n" +
                "10  9  7 10\r\n" +
                " 5  8  6 11\r\n" +
                "11  9  7  9\r\n" +
                "Запаси постачальників: 40; 45; 25\r\n" +
                "Потреби споживачів: 25; 10; 35; 40";

            return new TransportInput(costs, supplies, demands, description);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(Description);
            builder.AppendLine();
            builder.AppendLine("Сума запасів: " + TotalSupply().ToString("0.####"));
            builder.AppendLine("Сума потреб: " + TotalDemand().ToString("0.####"));

            if (IsClosed())
            {
                builder.AppendLine("Тип задачі: закрита транспортна задача.");
            }
            else
            {
                builder.AppendLine("Тип задачі: відкрита транспортна задача.");
            }

            return builder.ToString();
        }
    }
}