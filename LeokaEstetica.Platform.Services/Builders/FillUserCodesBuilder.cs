using LeokaEstetica.Platform.Database.Abstractions.User;
using LeokaEstetica.Platform.Models.Dto.Output.Resume;

namespace LeokaEstetica.Platform.Services.Builders;

/// <summary>
/// Класс билдера записывает код пользователей.
/// </summary>
public static class FillUserCodesBuilder
{
    /// <summary>
    /// Метод записывает коды пользователей.
    /// </summary>
    /// <returns>Результирующий список.</returns>
    public static async Task<List<ResumeOutput>> Fill(IEnumerable<ResumeOutput> resumes, 
        IUserRepository userRepository)
    {
        // Получаем словарь пользователей для поиска кодов, чтобы получить скорость поиска O(1).
        var userCodesDict = await userRepository.GetUsersCodesAsync();

        var result = new List<ResumeOutput>();
        foreach (var r in resumes)
        {
            r.UserCode = userCodesDict[r.UserId];
            result.Add(r);
        }

        return result;
    }
}