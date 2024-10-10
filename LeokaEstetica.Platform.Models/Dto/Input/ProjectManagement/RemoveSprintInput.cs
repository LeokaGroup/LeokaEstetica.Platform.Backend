namespace LeokaEstetica.Platform.Models.Dto.Input.ProjectManagement
{
    /// <summary>
    /// Класс входной модели удаления спринта.
    /// </summary>
    public class RemoveSprintInput
    {
        /// <summary>
        /// Id спринта в рамках проекта.
        /// </summary>
        public long ProjectSprintId { get; set; }

        /// <summary>
        /// Id проекта.
        /// </summary>
        public long ProjectId { get; set; }

        /// <summary>
        /// Список Id задач в рамках проекта, которые нужно исключить из спринта.
        /// </summary>
        public IEnumerable<long>? ProjectTaskIds { get; set; }
    }
}
