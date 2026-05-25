using System;
using System.Text;

namespace ASPR_Practice1
{
    internal class NetworkPlanningResult
    {
        public NetworkPlanningInput Input { get; private set; }
        public ProjectTask[] Tasks { get; private set; }
        public int ProjectDuration { get; private set; }
        public int[] CriticalPath { get; private set; }
        public int[] InitialResourceLoad { get; private set; }
        public int[] OptimizedResourceLoad { get; private set; }
        public int InitialPeakLoad { get; private set; }
        public int OptimizedPeakLoad { get; private set; }
        public string NetworkGraphMermaid { get; private set; }
        public string GanttProTable { get; private set; }

        public NetworkPlanningResult(
            NetworkPlanningInput input,
            ProjectTask[] tasks,
            int projectDuration,
            int[] criticalPath,
            int[] initialResourceLoad,
            int[] optimizedResourceLoad,
            int initialPeakLoad,
            int optimizedPeakLoad,
            string networkGraphMermaid,
            string ganttProTable)
        {
            Input = input;
            Tasks = tasks;
            ProjectDuration = projectDuration;
            CriticalPath = criticalPath;
            InitialResourceLoad = initialResourceLoad;
            OptimizedResourceLoad = optimizedResourceLoad;
            InitialPeakLoad = initialPeakLoad;
            OptimizedPeakLoad = optimizedPeakLoad;
            NetworkGraphMermaid = networkGraphMermaid;
            GanttProTable = ganttProTable;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Результат сіткового планування:");
            builder.AppendLine();
            builder.AppendLine("Тривалість проєкту: " + ProjectDuration);
            builder.AppendLine("Критичний шлях: " + PathToString(CriticalPath));
            builder.AppendLine("Пікове навантаження початкового календарного плану: " + InitialPeakLoad);
            builder.AppendLine("Пікове навантаження оптимізованого календарного плану: " + OptimizedPeakLoad);
            builder.AppendLine();

            builder.AppendLine("Параметри робіт:");
            builder.AppendLine("№\tES\tEF\tLS\tLF\tR\tStart\tFinish\tPeople\tCritical");

            for (int i = 0; i < Tasks.Length; i++)
            {
                ProjectTask task = Tasks[i];

                builder.Append(task.Id);
                builder.Append("\t");
                builder.Append(task.EarlyStart);
                builder.Append("\t");
                builder.Append(task.EarlyFinish);
                builder.Append("\t");
                builder.Append(task.LateStart);
                builder.Append("\t");
                builder.Append(task.LateFinish);
                builder.Append("\t");
                builder.Append(task.Reserve);
                builder.Append("\t");
                builder.Append(task.CalendarStart);
                builder.Append("\t");
                builder.Append(task.CalendarFinish);
                builder.Append("\t");
                builder.Append(task.People);
                builder.Append("\t");

                if (task.IsCritical())
                {
                    builder.Append("так");
                }
                else
                {
                    builder.Append("ні");
                }

                builder.AppendLine();
            }

            builder.AppendLine();
            builder.AppendLine("Сітковий графік у форматі Mermaid:");
            builder.AppendLine(NetworkGraphMermaid);

            builder.AppendLine();
            builder.AppendLine("Таблиця для перевірки в GANTTPRO:");
            builder.AppendLine(GanttProTable);

            return builder.ToString();
        }

        private string PathToString(int[] path)
        {
            if (path == null || path.Length == 0)
            {
                return "-";
            }

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < path.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(" -> ");
                }

                builder.Append(path[i]);
            }

            return builder.ToString();
        }
    }
}