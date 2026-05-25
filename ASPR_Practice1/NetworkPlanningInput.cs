using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class NetworkPlanningInput
    {
        public ProjectTask[] Tasks { get; private set; }
        public string Description { get; private set; }

        public NetworkPlanningInput(ProjectTask[] tasks, string description)
        {
            Tasks = tasks;
            Description = description;
        }

        public static NetworkPlanningInput CreateTestProblem()
        {
            ProjectTask[] tasks = new ProjectTask[]
            {
                new ProjectTask(1,  new int[] { },          10, 3),
                new ProjectTask(2,  new int[] { },          12, 4),
                new ProjectTask(3,  new int[] { },           7, 2),
                new ProjectTask(4,  new int[] { 3 },        10, 3),
                new ProjectTask(5,  new int[] { 3 },        15, 6),
                new ProjectTask(6,  new int[] { 1, 2, 4 },   5, 1),
                new ProjectTask(7,  new int[] { 2, 4 },     13, 3),
                new ProjectTask(8,  new int[] { 1, 2, 4 },  12, 4),
                new ProjectTask(9,  new int[] { 3 },        11, 5),
                new ProjectTask(10, new int[] { 5, 6, 7 },  10, 6),
                new ProjectTask(11, new int[] { 9 },         8, 4)
            };

            string description =
                "Тестова задача сіткового планування\r\n" +
                "№ | Попередні роботи | Тривалість | Кількість людей\r\n" +
                "1 | -       | 10 | 3\r\n" +
                "2 | -       | 12 | 4\r\n" +
                "3 | -       | 7  | 2\r\n" +
                "4 | 3       | 10 | 3\r\n" +
                "5 | 3       | 15 | 6\r\n" +
                "6 | 1,2,4   | 5  | 1\r\n" +
                "7 | 2,4     | 13 | 3\r\n" +
                "8 | 1,2,4   | 12 | 4\r\n" +
                "9 | 3       | 11 | 5\r\n" +
                "10 | 5,6,7  | 10 | 6\r\n" +
                "11 | 9      | 8  | 4";

            return new NetworkPlanningInput(tasks, description);
        }

        public static NetworkPlanningInput CreateVariant3()
        {
            return CreateTestProblem();
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(Description);
            builder.AppendLine();
            builder.AppendLine("Вхідні роботи:");

            for (int i = 0; i < Tasks.Length; i++)
            {
                builder.AppendLine(Tasks[i].ToString());
            }

            return builder.ToString();
        }
    }
}