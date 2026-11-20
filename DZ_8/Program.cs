using System;
using System.Collections.Generic;

namespace TaskManagerApp
{
    public enum ProjectStatus { Проект, Исполнение, Закрыт }
    public enum TaskStatus { Назначена, ВРаботе, НаПроверке, Выполнена }

    public class Employee
    {
        public string Name { get; set; }

        public Employee(string name)
        {
            Name = name;
        }
    }

    public class Report
    {
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public Employee Author { get; set; }

        public Report(string text, Employee author)
        {
            Text = text;
            Date = DateTime.Now;
            Author = author;
        }
    }

    public class Task
    {
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public Employee Initiator { get; set; }
        public Employee Assignee { get; set; } // Исполнитель
        public TaskStatus Status { get; set; }
        public Report Report { get; set; }

        public Task(string description, DateTime deadline, Employee initiator)
        {
            Description = description;
            Deadline = deadline;
            Initiator = initiator;
            Status = TaskStatus.Назначена;
            Report = null;
        }

        public void TakeToWork(Employee employee)
        {
            if (Status == TaskStatus.Назначена && Assignee == employee)
            {
                Status = TaskStatus.ВРаботе;
                Console.WriteLine($"[{employee.Name}] взял задачу '{Description}' в работу.");
            }
            else
            {
                Console.WriteLine($"Ошибка: нельзя взять задачу в работу.");
            }
        }

        public void Delegate(Employee newEmployee)
        {
            if (Status == TaskStatus.Назначена || Status == TaskStatus.ВРаботе)
            {
                Assignee = newEmployee;
                Status = TaskStatus.Назначена;
                Console.WriteLine($"Задача '{Description}' делегирована сотруднику {newEmployee.Name}.");
            }
        }

        public void Reject()
        {
            if (Status == TaskStatus.Назначена)
            {
                Assignee = null;
                Console.WriteLine($"Задача '{Description}' отклонена и возвращена тимлиду.");
            }
        }

        public void WriteReport(string text)
        {
            if (Status == TaskStatus.ВРаботе)
            {
                Report = new Report(text, Assignee);
                Status = TaskStatus.НаПроверке;
                Console.WriteLine($"Отчет по задаче '{Description}' отправлен на проверку.");
            }
            else
            {
                Console.WriteLine("Ошибка: нельзя написать отчет, задача не в работе.");
            }
        }
    }
    public class Project
    {
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public Employee Customer { get; set; } // Заказчик
        public Employee TeamLead { get; set; } // Тимлид
        public List<Task> Tasks { get; set; }
        public ProjectStatus Status { get; set; }

        public Project(string description, DateTime deadline, Employee customer, Employee teamLead)
        {
            Description = description;
            Deadline = deadline;
            Customer = customer;
            TeamLead = teamLead;
            Tasks = new List<Task>();
            Status = ProjectStatus.Проект;
        }

        public void CreateTask(string description, DateTime deadline, Employee assignee)
        {
            if (Status != ProjectStatus.Проект)
            {
                Console.WriteLine("Ошибка: нельзя создавать задачи, проект уже в работе или закрыт.");
                return;
            }

            Task newTask = new Task(description, deadline, TeamLead);
            newTask.Assignee = assignee;
            Tasks.Add(newTask);
            Console.WriteLine($"Создана задача '{description}' для {assignee.Name}.");
        }

        public void StartExecution()
        {
            if (Status == ProjectStatus.Проект)
            {
                Status = ProjectStatus.Исполнение;
                Console.WriteLine("Проект переведен в статус 'Исполнение'.");
            }
        }

        public void ReviewReport(Task task, bool isApproved)
        {
            if (task.Status != TaskStatus.НаПроверке)
            {
                Console.WriteLine("Ошибка: у задачи нет отчета на проверке.");
                return;
            }

            if (isApproved)
            {
                task.Status = TaskStatus.Выполнена;
                Console.WriteLine($"Тимлид УТВЕРДИЛ отчет по задаче '{task.Description}'. Задача выполнена.");
            }
            else
            {
                task.Status = TaskStatus.ВРаботе;
                task.Report = null;
                Console.WriteLine($"Тимлид ВЕРНУЛ на доработку отчет по задаче '{task.Description}'.");
            }

            CheckProjectClosure();
        }

        private void CheckProjectClosure()
        {
            foreach (var task in Tasks)
            {
                if (task.Status != TaskStatus.Выполнена) return;
            }
            Status = ProjectStatus.Закрыт;
            Console.WriteLine("ВСЕ ЗАДАЧИ ВЫПОЛНЕНЫ! Проект ЗАКРЫТ.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Employee> team = new List<Employee>();
            for (int i = 1; i <= 10; i++)
            {
                team.Add(new Employee($"Сотрудник_{i}"));
            }
            Employee customer = new Employee("Заказчик_Иван");
            Employee teamLead = new Employee("Тимлид_Петр");

            Project project = new Project("Разработка нового сайта", DateTime.Now.AddMonths(1), customer, teamLead);

            for (int i = 0; i < 10; i++)
            {
                project.CreateTask($"Задача №{i + 1}", DateTime.Now.AddDays(7), team[i]);
            }

            project.StartExecution();

            Console.WriteLine("\n--- СИМУЛЯЦИЯ РАБОТЫ ---");

            Task task1 = project.Tasks[0];
            task1.TakeToWork(team[0]);
            task1.WriteReport("Сделал главную страницу.");

            // Сотрудник 2 берет задачу, но делегирует её Сотруднику 3
            Task task2 = project.Tasks[1];
            task2.TakeToWork(team[1]);
            task2.Delegate(team[2]);

            // Сотрудник 4 отклоняет задачу
            Task task4 = project.Tasks[3];
            task4.Reject();

            // Тимлид проверяет отчет по первой задаче и утверждает его
            project.ReviewReport(task1, true);

            // Сотрудник 5 берет задачу, пишет отчет, а Тимлид отправляет на доработку
            Task task5 = project.Tasks[4];
            task5.TakeToWork(team[4]);
            task5.WriteReport("Сделал что-то не то.");
            project.ReviewReport(task5, false);

            Console.WriteLine("\n--- ИМИТАЦИЯ ВЫПОЛНЕНИЯ ОСТАЛЬНЫХ ЗАДАЧ ---");

            // Переназначаем отклоненную задачу Тимлидом
            task4.Assignee = team[3];
            Console.WriteLine($"\n[Имитация] Тимлид переназначил отклоненную задачу '{task4.Description}' на {team[3].Name}.");

            for (int i = 0; i < 10; i++)
            {
                Task currentTask = project.Tasks[i];

                if (currentTask.Status != TaskStatus.Выполнена && currentTask.Assignee != null)
                {
                    if (currentTask.Status == TaskStatus.Назначена)
                    {
                        currentTask.TakeToWork(currentTask.Assignee);
                    }
                    if (currentTask.Status == TaskStatus.ВРаботе)
                    {
                        currentTask.WriteReport("Исправленный/Финальный отчет.");
                    }

                    if (currentTask.Status == TaskStatus.НаПроверке)
                    {
                        project.ReviewReport(currentTask, true);
                    }
                }
            }

            Console.WriteLine($"\nИтоговый статус проекта: {project.Status}");
        }
    }
}