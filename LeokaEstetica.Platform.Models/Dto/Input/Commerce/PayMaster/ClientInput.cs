namespace LeokaEstetica.Platform.Models.Dto.Input.Commerce.PayMaster;

/// <summary>
/// Класс пользователя в ПС.
/// </summary>
public class ClientInput
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="email"></param>
    public ClientInput(string email)
    {
        Email = email;
    }

    /// <summary>
    /// Почта пользователя в ПС.
    /// </summary>
    public string Email { get; set; }
}