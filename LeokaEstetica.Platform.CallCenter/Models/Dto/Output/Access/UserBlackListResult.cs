namespace LeokaEstetica.Platform.CallCenter.Models.Dto.Output.Access;

/// <summary>
/// Класс выходной модели результата ЧС пользователей.
/// </summary>
public class UserBlackListResult
{
    /// <summary>
    /// Список пользователей в ЧС.
    /// </summary>
    public IEnumerable<UserBlackListOutput> UsersBlackList { get; set; }

    /// <summary>
    /// Кол-во.
    /// </summary>
    public int Count => UsersBlackList.Count();
}