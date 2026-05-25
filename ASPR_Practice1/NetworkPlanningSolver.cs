using System;
using System.Text;

namespace ASPR_Practice1
{
    internal static class NetworkPlanningSolver
    {
        public static NetworkPlanningResult Solve(NetworkPlanningInput input, ReportBuilder report)
        {
            ProjectTask[] tasks = CopyTasks(input.Tasks);

            report.AddTitle("Розв'язання задачі сіткового планування");
            report.AddText("Вхідні дані:");
            report.AddText(input.ToString());

            ValidateTasks(tasks);

            int[] order = BuildTopologicalOrder(tasks);

            report.AddTitle("Топологічний порядок робіт");
            report.AddText(IntArrayToString(order));

            CalculateEarlyDates(tasks, order, report);

            int projectDuration = CalculateProjectDuration(tasks);

            report.AddTitle("Довжина критичного шляху");
            report.AddText("Довжина критичного шляху дорівнює максимальному ранньому завершенню робіт.");
            report.AddText("Тривалість проєкту: " + projectDuration);

            CalculateLateDates(tasks, order, projectDuration, report);

            CalculateReserves(tasks, report);

            int[] criticalPath = BuildCriticalPath(tasks, report);

            BuildInitialCalendar(tasks, report);

            int[] initialLoad = BuildResourceLoad(tasks, projectDuration);
            int initialPeak = Max(initialLoad);

            report.AddTitle("Початковий графік завантаження ресурсів");
            report.AddText(ResourceLoadToString(initialLoad));
            report.AddText("Максимальне навантаження: " + initialPeak);

            OptimizeCalendar(tasks, projectDuration, report);

            int[] optimizedLoad = BuildResourceLoad(tasks, projectDuration);
            int optimizedPeak = Max(optimizedLoad);

            report.AddTitle("Оптимізований графік завантаження ресурсів");
            report.AddText(ResourceLoadToString(optimizedLoad));
            report.AddText("Максимальне навантаження після оптимізації: " + optimizedPeak);

            string mermaidGraph = BuildMermaidNetworkGraph(tasks, criticalPath);
            string ganttProTable = BuildGanttProTable(tasks);

            report.AddTitle("Графічне подання сіткового графіка");
            report.AddText("Нижче наведено сітковий граф у форматі Mermaid.");
            report.AddText(mermaidGraph);

            report.AddTitle("Таблиця для перевірки в GANTTPRO");
            report.AddText(ganttProTable);

            report.AddTitle("Діаграма Ганта у текстовому вигляді");
            report.AddText(BuildGanttText(tasks, projectDuration));

            return new NetworkPlanningResult(
                input,
                tasks,
                projectDuration,
                criticalPath,
                initialLoad,
                optimizedLoad,
                initialPeak,
                optimizedPeak,
                mermaidGraph,
                ganttProTable);
        }

        private static void ValidateTasks(ProjectTask[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
            {
                throw new Exception("Перелік робіт порожній.");
            }

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].Duration <= 0)
                {
                    throw new Exception("Тривалість роботи " + tasks[i].Id + " має бути більшою за нуль.");
                }

                if (tasks[i].People <= 0)
                {
                    throw new Exception("Кількість людей для роботи " + tasks[i].Id + " має бути більшою за нуль.");
                }

                for (int j = 0; j < tasks[i].Predecessors.Length; j++)
                {
                    int predecessorId = tasks[i].Predecessors[j];

                    if (FindTaskIndex(tasks, predecessorId) == -1)
                    {
                        throw new Exception("Для роботи " + tasks[i].Id + " задано неіснуючу попередню роботу " + predecessorId + ".");
                    }

                    if (predecessorId == tasks[i].Id)
                    {
                        throw new Exception("Робота " + tasks[i].Id + " не може бути попередником самої себе.");
                    }
                }
            }
        }

        private static int[] BuildTopologicalOrder(ProjectTask[] tasks)
        {
            int count = tasks.Length;
            int[] order = new int[count];
            bool[] added = new bool[count];
            int orderIndex = 0;

            while (orderIndex < count)
            {
                bool progress = false;

                for (int i = 0; i < count; i++)
                {
                    if (added[i])
                    {
                        continue;
                    }

                    bool allPredecessorsAdded = true;

                    for (int j = 0; j < tasks[i].Predecessors.Length; j++)
                    {
                        int predecessorIndex = FindTaskIndex(tasks, tasks[i].Predecessors[j]);

                        if (predecessorIndex == -1 || !added[predecessorIndex])
                        {
                            allPredecessorsAdded = false;
                            break;
                        }
                    }

                    if (allPredecessorsAdded)
                    {
                        order[orderIndex] = tasks[i].Id;
                        added[i] = true;
                        orderIndex++;
                        progress = true;
                    }
                }

                if (!progress)
                {
                    throw new Exception("У графі робіт знайдено цикл залежностей.");
                }
            }

            return order;
        }

        private static void CalculateEarlyDates(ProjectTask[] tasks, int[] order, ReportBuilder report)
        {
            report.AddTitle("Прямий прохід. Розрахунок ранніх термінів");

            for (int k = 0; k < order.Length; k++)
            {
                ProjectTask task = GetTaskById(tasks, order[k]);

                int earlyStart = 0;

                for (int i = 0; i < task.Predecessors.Length; i++)
                {
                    ProjectTask predecessor = GetTaskById(tasks, task.Predecessors[i]);

                    if (predecessor.EarlyFinish > earlyStart)
                    {
                        earlyStart = predecessor.EarlyFinish;
                    }
                }

                task.EarlyStart = earlyStart;
                task.EarlyFinish = task.EarlyStart + task.Duration;

                report.AddText(
                    "Робота " + task.Id +
                    ": ES = " + task.EarlyStart +
                    ", EF = ES + t = " + task.EarlyStart + " + " + task.Duration +
                    " = " + task.EarlyFinish + ".");
            }
        }

        private static int CalculateProjectDuration(ProjectTask[] tasks)
        {
            int duration = tasks[0].EarlyFinish;

            for (int i = 1; i < tasks.Length; i++)
            {
                if (tasks[i].EarlyFinish > duration)
                {
                    duration = tasks[i].EarlyFinish;
                }
            }

            return duration;
        }

        private static void CalculateLateDates(ProjectTask[] tasks, int[] order, int projectDuration, ReportBuilder report)
        {
            report.AddTitle("Зворотний прохід. Розрахунок пізніх термінів");

            for (int k = order.Length - 1; k >= 0; k--)
            {
                ProjectTask task = GetTaskById(tasks, order[k]);
                int[] successors = FindSuccessors(tasks, task.Id);

                if (successors.Length == 0)
                {
                    task.LateFinish = projectDuration;
                }
                else
                {
                    int minLateStart = GetTaskById(tasks, successors[0]).LateStart;

                    for (int i = 1; i < successors.Length; i++)
                    {
                        ProjectTask successor = GetTaskById(tasks, successors[i]);

                        if (successor.LateStart < minLateStart)
                        {
                            minLateStart = successor.LateStart;
                        }
                    }

                    task.LateFinish = minLateStart;
                }

                task.LateStart = task.LateFinish - task.Duration;

                report.AddText(
                    "Робота " + task.Id +
                    ": LF = " + task.LateFinish +
                    ", LS = LF - t = " + task.LateFinish + " - " + task.Duration +
                    " = " + task.LateStart + ".");
            }
        }

        private static void CalculateReserves(ProjectTask[] tasks, ReportBuilder report)
        {
            report.AddTitle("Розрахунок резервів часу");

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i].Reserve = tasks[i].LateStart - tasks[i].EarlyStart;

                report.AddText(
                    "Робота " + tasks[i].Id +
                    ": R = LS - ES = " + tasks[i].LateStart +
                    " - " + tasks[i].EarlyStart +
                    " = " + tasks[i].Reserve + ".");
            }
        }

        private static int[] BuildCriticalPath(ProjectTask[] tasks, ReportBuilder report)
        {
            report.AddTitle("Пошук критичного шляху");

            int count = 0;

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].IsCritical())
                {
                    count++;
                }
            }

            int[] criticalIds = new int[count];
            int index = 0;

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].IsCritical())
                {
                    criticalIds[index] = tasks[i].Id;
                    index++;
                }
            }

            int[] path = BuildOneCriticalPath(tasks);

            report.AddText("Критичні роботи: " + IntArrayToString(criticalIds));
            report.AddText("Критичний шлях: " + IntArrayToString(path));

            return path;
        }

        private static int[] BuildOneCriticalPath(ProjectTask[] tasks)
        {
            ProjectTask startTask = null;

            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].IsCritical())
                {
                    continue;
                }

                if (tasks[i].Predecessors.Length == 0)
                {
                    if (startTask == null || tasks[i].EarlyStart < startTask.EarlyStart)
                    {
                        startTask = tasks[i];
                    }
                }
            }

            if (startTask == null)
            {
                return new int[] { };
            }

            int[] path = new int[tasks.Length];
            int pathLength = 0;
            ProjectTask current = startTask;

            while (current != null)
            {
                path[pathLength] = current.Id;
                pathLength++;

                ProjectTask next = null;

                for (int i = 0; i < tasks.Length; i++)
                {
                    if (!tasks[i].IsCritical())
                    {
                        continue;
                    }

                    if (HasPredecessor(tasks[i], current.Id) && tasks[i].EarlyStart == current.EarlyFinish)
                    {
                        next = tasks[i];
                        break;
                    }
                }

                current = next;
            }

            int[] result = new int[pathLength];

            for (int i = 0; i < pathLength; i++)
            {
                result[i] = path[i];
            }

            return result;
        }

        private static void BuildInitialCalendar(ProjectTask[] tasks, ReportBuilder report)
        {
            report.AddTitle("Побудова початкового календарного плану");

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i].CalendarStart = tasks[i].EarlyStart;
                tasks[i].CalendarFinish = tasks[i].EarlyFinish;

                report.AddText(
                    "Робота " + tasks[i].Id +
                    ": Start = " + tasks[i].CalendarStart +
                    ", Finish = " + tasks[i].CalendarFinish + ".");
            }
        }

        private static void OptimizeCalendar(ProjectTask[] tasks, int projectDuration, ReportBuilder report)
        {
            report.AddTitle("Оптимізація графіка завантаження ресурсів");

            ProjectTask[] sorted = CopyTaskReferences(tasks);

            for (int i = 0; i < sorted.Length - 1; i++)
            {
                for (int j = i + 1; j < sorted.Length; j++)
                {
                    if (sorted[j].Reserve > sorted[i].Reserve)
                    {
                        ProjectTask temp = sorted[i];
                        sorted[i] = sorted[j];
                        sorted[j] = temp;
                    }
                }
            }

            for (int k = 0; k < sorted.Length; k++)
            {
                ProjectTask task = sorted[k];

                if (task.IsCritical())
                {
                    continue;
                }

                int bestStart = task.CalendarStart;
                int bestPeak = int.MaxValue;
                int bestSumSquares = int.MaxValue;

                for (int start = task.EarlyStart; start <= task.LateStart; start++)
                {
                    int oldStart = task.CalendarStart;
                    int oldFinish = task.CalendarFinish;

                    task.CalendarStart = start;
                    task.CalendarFinish = start + task.Duration;

                    if (!CalendarDependenciesAreValid(tasks))
                    {
                        task.CalendarStart = oldStart;
                        task.CalendarFinish = oldFinish;
                        continue;
                    }

                    int[] load = BuildResourceLoad(tasks, projectDuration);
                    int peak = Max(load);
                    int sumSquares = SumSquares(load);

                    if (peak < bestPeak || (peak == bestPeak && sumSquares < bestSumSquares))
                    {
                        bestPeak = peak;
                        bestSumSquares = sumSquares;
                        bestStart = start;
                    }

                    task.CalendarStart = oldStart;
                    task.CalendarFinish = oldFinish;
                }

                task.CalendarStart = bestStart;
                task.CalendarFinish = bestStart + task.Duration;

                report.AddText(
                    "Роботу " + task.Id +
                    " заплановано з " + task.CalendarStart +
                    " до " + task.CalendarFinish + ".");
            }
        }

        private static bool CalendarDependenciesAreValid(ProjectTask[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                ProjectTask task = tasks[i];

                for (int j = 0; j < task.Predecessors.Length; j++)
                {
                    ProjectTask predecessor = GetTaskById(tasks, task.Predecessors[j]);

                    if (task.CalendarStart < predecessor.CalendarFinish)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static int[] BuildResourceLoad(ProjectTask[] tasks, int projectDuration)
        {
            int[] load = new int[projectDuration];

            for (int i = 0; i < tasks.Length; i++)
            {
                for (int day = tasks[i].CalendarStart; day < tasks[i].CalendarFinish; day++)
                {
                    if (day >= 0 && day < projectDuration)
                    {
                        load[day] += tasks[i].People;
                    }
                }
            }

            return load;
        }

        private static string BuildMermaidNetworkGraph(ProjectTask[] tasks, int[] criticalPath)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("flowchart LR");
            builder.AppendLine("    Start((Start))");
            builder.AppendLine("    Finish((Finish))");

            for (int i = 0; i < tasks.Length; i++)
            {
                ProjectTask task = tasks[i];

                builder.Append("    A");
                builder.Append(task.Id);
                builder.Append("[\"");
                builder.Append("A");
                builder.Append(task.Id);
                builder.Append("<br/>t=");
                builder.Append(task.Duration);
                builder.Append(", people=");
                builder.Append(task.People);
                builder.Append("<br/>ES=");
                builder.Append(task.EarlyStart);
                builder.Append(", EF=");
                builder.Append(task.EarlyFinish);
                builder.Append("<br/>LS=");
                builder.Append(task.LateStart);
                builder.Append(", LF=");
                builder.Append(task.LateFinish);
                builder.Append("<br/>R=");
                builder.Append(task.Reserve);
                builder.Append("\"]");
                builder.AppendLine();
            }

            for (int i = 0; i < tasks.Length; i++)
            {
                ProjectTask task = tasks[i];

                if (task.Predecessors.Length == 0)
                {
                    builder.Append("    Start --> A");
                    builder.Append(task.Id);
                    builder.AppendLine();
                }

                for (int j = 0; j < task.Predecessors.Length; j++)
                {
                    builder.Append("    A");
                    builder.Append(task.Predecessors[j]);
                    builder.Append(" --> A");
                    builder.Append(task.Id);
                    builder.AppendLine();
                }

                if (FindSuccessors(tasks, task.Id).Length == 0)
                {
                    builder.Append("    A");
                    builder.Append(task.Id);
                    builder.Append(" --> Finish");
                    builder.AppendLine();
                }
            }

            builder.AppendLine("    classDef critical fill:#ffd6d6,stroke:#d00000,stroke-width:3px;");
            builder.AppendLine("    classDef normal fill:#eef5ff,stroke:#336699,stroke-width:1px;");

            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].IsCritical())
                {
                    builder.Append("    class A");
                    builder.Append(tasks[i].Id);
                    builder.AppendLine(" critical;");
                }
                else
                {
                    builder.Append("    class A");
                    builder.Append(tasks[i].Id);
                    builder.AppendLine(" normal;");
                }
            }

            return builder.ToString();
        }

        private static string BuildGanttProTable(ProjectTask[] tasks)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("ID;Task name;Duration;Start day;Finish day;Predecessors;People;Critical");

            for (int i = 0; i < tasks.Length; i++)
            {
                ProjectTask task = tasks[i];

                builder.Append(task.Id);
                builder.Append(";A");
                builder.Append(task.Id);
                builder.Append(";");
                builder.Append(task.Duration);
                builder.Append(";");
                builder.Append(task.CalendarStart);
                builder.Append(";");
                builder.Append(task.CalendarFinish);
                builder.Append(";");
                builder.Append(PredecessorsToString(task.Predecessors));
                builder.Append(";");
                builder.Append(task.People);
                builder.Append(";");

                if (task.IsCritical())
                {
                    builder.Append("yes");
                }
                else
                {
                    builder.Append("no");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string PredecessorsToString(int[] predecessors)
        {
            if (predecessors == null || predecessors.Length == 0)
            {
                return "";
            }

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < predecessors.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                }

                builder.Append(predecessors[i]);
            }

            return builder.ToString();
        }

        private static string ResourceLoadToString(int[] load)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("День\tЛюдей");

            for (int i = 0; i < load.Length; i++)
            {
                builder.Append(i);
                builder.Append("\t");
                builder.Append(load[i]);
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string BuildGanttText(ProjectTask[] tasks, int projectDuration)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("       ");

            for (int day = 0; day < projectDuration; day++)
            {
                builder.Append((day % 10).ToString());
            }

            builder.AppendLine();

            for (int i = 0; i < tasks.Length; i++)
            {
                ProjectTask task = tasks[i];

                builder.Append("A");
                builder.Append(task.Id.ToString().PadLeft(2));
                builder.Append("   ");

                for (int day = 0; day < projectDuration; day++)
                {
                    if (day >= task.CalendarStart && day < task.CalendarFinish)
                    {
                        if (task.IsCritical())
                        {
                            builder.Append("#");
                        }
                        else
                        {
                            builder.Append("=");
                        }
                    }
                    else if (day >= task.EarlyStart && day < task.LateFinish && !task.IsCritical())
                    {
                        builder.Append(".");
                    }
                    else
                    {
                        builder.Append(" ");
                    }
                }

                builder.Append("  ");
                builder.Append(task.People);
                builder.Append(" людей");
                builder.AppendLine();
            }

            builder.AppendLine();
            builder.AppendLine("# - критична робота");
            builder.AppendLine("= - некритична запланована робота");
            builder.AppendLine(". - доступний резерв часу");

            return builder.ToString();
        }

        private static int[] FindSuccessors(ProjectTask[] tasks, int taskId)
        {
            int count = 0;

            for (int i = 0; i < tasks.Length; i++)
            {
                if (HasPredecessor(tasks[i], taskId))
                {
                    count++;
                }
            }

            int[] successors = new int[count];
            int index = 0;

            for (int i = 0; i < tasks.Length; i++)
            {
                if (HasPredecessor(tasks[i], taskId))
                {
                    successors[index] = tasks[i].Id;
                    index++;
                }
            }

            return successors;
        }

        private static bool HasPredecessor(ProjectTask task, int predecessorId)
        {
            for (int i = 0; i < task.Predecessors.Length; i++)
            {
                if (task.Predecessors[i] == predecessorId)
                {
                    return true;
                }
            }

            return false;
        }

        private static ProjectTask GetTaskById(ProjectTask[] tasks, int id)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].Id == id)
                {
                    return tasks[i];
                }
            }

            throw new Exception("Роботу з номером " + id + " не знайдено.");
        }

        private static int FindTaskIndex(ProjectTask[] tasks, int id)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                if (tasks[i].Id == id)
                {
                    return i;
                }
            }

            return -1;
        }

        private static int Max(int[] values)
        {
            if (values.Length == 0)
            {
                return 0;
            }

            int max = values[0];

            for (int i = 1; i < values.Length; i++)
            {
                if (values[i] > max)
                {
                    max = values[i];
                }
            }

            return max;
        }

        private static int SumSquares(int[] values)
        {
            int sum = 0;

            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i] * values[i];
            }

            return sum;
        }

        private static ProjectTask[] CopyTasks(ProjectTask[] source)
        {
            ProjectTask[] result = new ProjectTask[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                int[] predecessors = new int[source[i].Predecessors.Length];

                for (int j = 0; j < predecessors.Length; j++)
                {
                    predecessors[j] = source[i].Predecessors[j];
                }

                result[i] = new ProjectTask(
                    source[i].Id,
                    predecessors,
                    source[i].Duration,
                    source[i].People);
            }

            return result;
        }

        private static ProjectTask[] CopyTaskReferences(ProjectTask[] source)
        {
            ProjectTask[] result = new ProjectTask[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                result[i] = source[i];
            }

            return result;
        }

        private static string IntArrayToString(int[] values)
        {
            if (values == null || values.Length == 0)
            {
                return "-";
            }

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(" -> ");
                }

                builder.Append(values[i]);
            }

            return builder.ToString();
        }
    }
}