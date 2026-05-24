using System;

namespace ASPR_Practice1
{
    internal class NatureGameInput
    {
        public double[,] UtilityMatrix { get; private set; }
        public double HurwiczAlpha { get; private set; }
        public double[] Probabilities { get; private set; }
        public string Description { get; private set; }

        public NatureGameInput(double[,] utilityMatrix, double hurwiczAlpha, double[] probabilities, string description)
        {
            UtilityMatrix = utilityMatrix;
            HurwiczAlpha = hurwiczAlpha;
            Probabilities = probabilities;
            Description = description;
        }

        public static NatureGameInput CreatePractice4Variant3()
        {
            double[,] matrix = new double[,]
            {
                {  1,  2,  4, 12 },
                {  2,  6, -3,  6 },
                { -2, -2,  6,  6 }
            };

            double alpha = 0.4;

            double[] probabilities = new double[]
            {
                0.4, 0.1, 0.2, 0.3
            };

            string description =
                "Практична робота 4. Варіант 3\r\n" +
                "Матриця корисності U:\r\n" +
                "1   2   4   12\r\n" +
                "2   6  -3    6\r\n" +
                "-2 -2   6    6\r\n" +
                "Коефіцієнт оптимізму для критерію Гурвіца: 0.4\r\n" +
                "Ймовірності станів природи для критерію Байєса: 0.4; 0.1; 0.2; 0.3";

            return new NatureGameInput(matrix, alpha, probabilities, description);
        }

        public static NatureGameInput CreatePractice4Test1()
        {
            double[,] matrix = new double[,]
            {
                { -1,  1,  1,  4 },
                { -1, -2,  2,  3 },
                {  3, -1,  3,  2 }
            };

            double alpha = 0.3;

            double[] probabilities = new double[]
            {
                0.2, 0.4, 0.1, 0.3
            };

            string description =
                "Тестовий приклад 1 для гри з природою\r\n" +
                "Матриця корисності U:\r\n" +
                "-1   1   1   4\r\n" +
                "-1  -2   2   3\r\n" +
                " 3  -1   3   2\r\n" +
                "Коефіцієнт оптимізму для критерію Гурвіца: 0.3\r\n" +
                "Ймовірності станів природи: 0.2; 0.4; 0.1; 0.3";

            return new NatureGameInput(matrix, alpha, probabilities, description);
        }

        public static NatureGameInput CreatePractice4Test2()
        {
            double[,] matrix = new double[,]
            {
                { 2, -1, 3, 4 },
                { -1, 2, 3, 7 },
                { 5, 4, 6, 2 }
            };

            double alpha = 0.4;

            double[] probabilities = new double[]
            {
                0.4, 0.1, 0.2, 0.3
            };

            string description =
                "Тестовий приклад 2 для гри з природою\r\n" +
                "Матриця корисності U:\r\n" +
                " 2  -1   3   4\r\n" +
                "-1   2   3   7\r\n" +
                " 5   4   6   2\r\n" +
                "Коефіцієнт оптимізму для критерію Гурвіца: 0.4\r\n" +
                "Ймовірності станів природи: 0.4; 0.1; 0.2; 0.3";

            return new NatureGameInput(matrix, alpha, probabilities, description);
        }
    }
}