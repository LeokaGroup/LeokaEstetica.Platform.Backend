namespace LeokaEstetica.Platform.Models.Dto.Output.ProjectManagment
{
    /// <summary>
    /// Класс результата выходной модели спринтов
    /// </summary>
    public class TaskSprintListResult
    {
        /// <summary>
        /// Список спринтов со статусом Новая
        /// </summary>
        public IEnumerable<TaskSprintExtendedOutput>? SprintsNew { get; set; }

        /// <summary>
        /// Список спринтов со статусом В работе
        /// </summary>
        public IEnumerable<TaskSprintExtendedOutput>? SprintsInWork { get; set; }

        /// <summary>
        /// Список спринтов со статусом Завершен
        /// </summary>
        public IEnumerable<TaskSprintExtendedOutput>? SprintsCompleted { get; set; }
    }
}
