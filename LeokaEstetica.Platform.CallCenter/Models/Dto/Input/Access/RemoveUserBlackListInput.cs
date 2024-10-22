namespace LeokaEstetica.Platform.CallCenter.Models.Dto.Input.Access
{
    /// <summary>
    /// Класс входной модели удаления пользователя из ЧС.
    /// </summary>
    public class RemoveUserBlackListInput
    {
        /// <summary>
        /// Id пользователя.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Почта.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Номер телефона.
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Id пользователя в системе ВКонтакте.
        /// </summary>
        public long? VkUserId { get; set; }
    }
}
